using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace SuperShop_Neko
{
    public partial class app : UserControl
    {
        public static string mysqlcon = "server=api.baka233.top;database=supershop;user=sudabaka;password=jianghao0523;";

        private List<AppItem> dataList = new List<AppItem>();
        private bool isLoading = false;

        // 主题色相关
        private Color themeColor = Color.Empty;
        private bool useThemeColor = false;

        public app()
        {
            InitializeComponent();
            InitializeDataGridView();

            // 加载主题色配置
            LoadThemeColorConfig();
        }

        /// <summary>
        /// 加载主题色配置
        /// </summary>
        private void LoadThemeColorConfig()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                if (!File.Exists(configPath))
                {
                    useThemeColor = false;
                    return;
                }

                string json = File.ReadAllText(configPath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 读取color配置
                if (root.TryGetProperty("color", out var colorElement))
                {
                    useThemeColor = colorElement.GetBoolean();

                    if (useThemeColor && root.TryGetProperty("RGB", out var rgbElement))
                    {
                        string rgbString = rgbElement.GetString() ?? "0,0,0";
                        string[] parts = rgbString.Split(',');

                        if (parts.Length == 3 &&
                            int.TryParse(parts[0], out int r) &&
                            int.TryParse(parts[1], out int g) &&
                            int.TryParse(parts[2], out int b))
                        {
                            themeColor = Color.FromArgb(r, g, b);
                            ApplyThemeToButtons();
                        }
                    }
                }
            }
            catch
            {
                useThemeColor = false;
            }
        }

        /// <summary>
        /// 应用主题色到按钮
        /// </summary>
        private void ApplyThemeToButtons()
        {
            if (!useThemeColor || themeColor == Color.Empty) return;

            try
            {
                // 应用主题色到所有按钮
                ApplyColorToButton(upbutton, themeColor);
                ApplyColorToButton(delbtn, themeColor);
                ApplyColorToButton(reload, themeColor);

                // 如果需要，也可以应用到DataGridView的按钮列
                ApplyThemeToDataGridView();
            }
            catch
            {
                // 静默失败
            }
        }

        /// <summary>
        /// 应用颜色到单个按钮
        /// </summary>
        private void ApplyColorToButton(Control button, Color color)
        {
            if (button == null) return;

            try
            {
                // 对于AntdUI.Button
                if (button is AntdUI.Button antdButton)
                {
                    antdButton.BackColor = color;
                    antdButton.DefaultBack = color;

                    // 设置悬停色（比主题色稍亮）
                    Color hoverColor = Color.FromArgb(
                        Math.Min(color.R + 20, 255),
                        Math.Min(color.G + 20, 255),
                        Math.Min(color.B + 20, 255)
                    );
                    antdButton.BackHover = hoverColor;

                    // 设置文字颜色确保可读性


                    antdButton.Invalidate();
                }
                // 对于普通Button
                else if (button is System.Windows.Forms.Button winButton)
                {
                    winButton.BackColor = color;

                    // 设置文字颜色


                    winButton.Invalidate();
                }
            }
            catch
            {
                // 忽略错误
            }
        }

        /// <summary>
        /// 应用主题色到DataGridView的按钮列（如果需要）
        /// </summary>
        private void ApplyThemeToDataGridView()
        {
            try
            {
                // 设置DataGridView的样式
                if (useThemeColor && themeColor != Color.Empty)
                {
                    // 设置行样式
                    dataGridView1.RowsDefaultCellStyle.BackColor = Color.White;
                    dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

                    // 设置标题行样式
                    dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = themeColor;
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);

                    // 设置选中行样式
                    dataGridView1.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(
                        Math.Min(themeColor.R + 40, 255),
                        Math.Min(themeColor.G + 40, 255),
                        Math.Min(themeColor.B + 40, 255)
                    );
                    dataGridView1.RowsDefaultCellStyle.SelectionForeColor = Color.White;

                    // 刷新显示
                    dataGridView1.Invalidate();
                }
            }
            catch
            {
                // 忽略错误
            }
        }

        private void InitializeDataGridView()
        {
            // 设置 DataGridView 基本属性
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 确保按钮列显示文字
            查看出处.UseColumnTextForButtonValue = true;
            下载.UseColumnTextForButtonValue = true;

            // 绑定单元格点击事件
            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void app_Load(object sender, EventArgs e)
        {
            // 确保主题色已应用
            if (useThemeColor && themeColor != Color.Empty)
            {
                ApplyThemeToButtons();
            }

            // 使用统一的 Spin 方法加载数据
            LoadDataWithSpin();
        }

        // 统一的加载数据方法（始终使用 AntdUI.Spin.open）
        private void LoadDataWithSpin()
        {
            if (isLoading) return;
            isLoading = true;

            AntdUI.Spin.open(this, config =>
            {
                try
                {
                    config.Text = "正在连接数据库...";
                    Thread.Sleep(300);

                    config.Text = "正在查询数据...";
                    dataList.Clear();

                    using (var connection = new MySqlConnection(mysqlcon))
                    {
                        connection.Open();
                        // 查询所有字段，包括出处
                        string sql = "SELECT 软件名, 链接, 上传者, 出处 FROM app";

                        using (var command = new MySqlCommand(sql, connection))
                        using (var reader = command.ExecuteReader())
                        {
                            config.Text = "正在读取数据...";
                            int count = 0;

                            while (reader.Read())
                            {
                                var item = new AppItem
                                {
                                    软件名 = reader["软件名"]?.ToString() ?? "",
                                    链接 = reader["链接"]?.ToString() ?? "",
                                    上传者 = reader["上传者"]?.ToString() ?? "",
                                    出处 = reader["出处"]?.ToString() ?? ""
                                };

                                dataList.Add(item);
                                count++;

                                if (count % 5 == 0)
                                {
                                    config.Text = $"已读取 {count} 条数据...";
                                }
                            }

                            config.Text = $"共读取 {count} 条数据";
                        }
                    }

                    this.Invoke(new Action(() =>
                    {
                        BindDataToGridView();
                    }));

                    config.Text = "加载完成";
                    Thread.Sleep(300);
                }
                catch (Exception ex)
                {
                    config.Text = "加载失败";
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show($"数据加载失败: {ex.Message}", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                finally
                {
                    isLoading = false;
                }
            }, () =>
            {
                // 加载完成后的回调
                isLoading = false;
            });
        }

        // 绑定数据到 DataGridView
        private void BindDataToGridView()
        {
            try
            {
                // 清除现有数据
                dataGridView1.Rows.Clear();

                // 添加数据行
                foreach (var item in dataList)
                {
                    int rowIndex = dataGridView1.Rows.Add(
                        item.软件名,
                        item.链接,
                        item.上传者,
                        "查看出处",
                        "下载"
                    );

                    // 将完整对象存储在 Tag 中
                    dataGridView1.Rows[rowIndex].Tag = item;
                }

                // 如果没有数据，显示提示
                if (dataGridView1.Rows.Count == 0)
                {
                    dataGridView1.Rows.Add("", "", "暂无数据", "", "");
                    dataGridView1.Rows[0].DefaultCellStyle.ForeColor = Color.Gray;
                    dataGridView1.Rows[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据显示失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DataGridView 单元格点击事件
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // 获取当前行的数据对象
            var row = dataGridView1.Rows[e.RowIndex];

            // 检查 Tag 是否包含 AppItem 对象
            if (row.Tag is not AppItem item)
            {
                // 如果没有数据，可能是一个空行或占位行
                if (e.ColumnIndex == dataGridView1.Columns["查看出处"].Index ||
                    e.ColumnIndex == dataGridView1.Columns["下载"].Index)
                {
                    MessageBox.Show("此行为空数据行，无法执行操作", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            string softwareName = item.软件名;
            string link = item.链接;
            string source = item.出处;

            // 处理按钮点击
            if (e.ColumnIndex == dataGridView1.Columns["查看出处"].Index)
            {
                // 查看出处按钮 - 在浏览器中打开出处链接
                if (!string.IsNullOrWhiteSpace(source))
                {
                    try
                    {
                        string urlToOpen = source;
                        if (!urlToOpen.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                            !urlToOpen.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                        {
                            urlToOpen = "http://" + urlToOpen;
                        }

                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = urlToOpen,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"无法打开出处链接: {ex.Message}\n链接: {source}",
                            "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"软件 '{softwareName}' 没有出处链接", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (e.ColumnIndex == dataGridView1.Columns["下载"].Index)
            {
                // 下载按钮 - 在浏览器中打开下载链接
                if (!string.IsNullOrWhiteSpace(link))
                {
                    try
                    {
                        string urlToOpen = link;
                        if (!urlToOpen.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                            !urlToOpen.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                        {
                            urlToOpen = "http://" + urlToOpen;
                        }

                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = urlToOpen,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"无法打开下载链接: {ex.Message}\n链接: {link}",
                            "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"软件 '{softwareName}' 没有下载链接", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // 上传按钮点击事件
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var uploadControl = new upload();
                uploadControl.Size = new Size(300, 600);

                Form parentForm = GetParentForm();

                if (parentForm != null)
                {
                    AntdUI.Drawer.open(parentForm, uploadControl, AntdUI.TAlignMini.Right);
                }
                else
                {
                    MessageBox.Show("无法找到父窗体", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"错误: {ex.Message}",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 删除按钮点击事件
        private void delbtn_Click(object sender, EventArgs e)
        {
            try
            {
                var deleteControl = new delete();
                deleteControl.Size = new Size(300, 300);

                Form parentForm = GetParentForm();

                if (parentForm != null)
                {
                    AntdUI.Drawer.open(parentForm, deleteControl, AntdUI.TAlignMini.Right);
                }
                else
                {
                    MessageBox.Show("无法找到父窗体", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"错误: {ex.Message}",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 刷新按钮点击事件 - 使用统一的 Spin 方法
        private void reload_Click(object sender, EventArgs e)
        {
            if (isLoading) return;
            LoadDataWithSpin();
        }

        // app_Load_1 事件
        private void app_Load_1(object sender, EventArgs e)
        {
            // 同样使用统一的 Spin 方法
            LoadDataWithSpin();
        }

        // 获取父窗体
        private Form GetParentForm()
        {
            Control control = this;
            while (control != null && !(control is Form))
            {
                control = control.Parent;
            }

            if (control is Form form)
            {
                return form;
            }

            return Form.ActiveForm;
        }

        /// <summary>
        /// 刷新主题色（当配置改变时调用）
        /// </summary>
        public void RefreshTheme()
        {
            LoadThemeColorConfig();
            if (useThemeColor && themeColor != Color.Empty)
            {
                ApplyThemeToButtons();
            }
        }

        // 数据类
        public class AppItem
        {
            public string 软件名 { get; set; }
            public string 链接 { get; set; }
            public string 上传者 { get; set; }
            public string 出处 { get; set; }
        }
    }
}