using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using AntdUI;

namespace SuperShop_Neko
{
    public partial class app : UserControl
    {
        private List<AppItem> dataList = new List<AppItem>();
        private bool isLoading = false;

        // 主题色相关
        private Color themeColor = Color.Empty;
        private bool useThemeColor = false;

        public app()
        {
            InitializeComponent();
            InitializeDataGridView();
            LoadThemeColorConfig();
        }

        #region 初始化方法
        private void InitializeDataGridView()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            查看出处.UseColumnTextForButtonValue = true;
            下载.UseColumnTextForButtonValue = true;
            dataGridView1.CellClick += DataGridView1_CellClick;
        }

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

        private void ApplyThemeToButtons()
        {
            if (!useThemeColor || themeColor == Color.Empty) return;
            try
            {
                ApplyColorToButton(upbutton, themeColor);
                ApplyColorToButton(delbtn, themeColor);
                ApplyColorToButton(reload, themeColor);
                ApplyThemeToDataGridView();
            }
            catch { }
        }

        private void ApplyColorToButton(Control button, Color color)
        {
            if (button == null) return;
            try
            {
                if (button is AntdUI.Button antdButton)
                {
                    antdButton.BackColor = color;
                    antdButton.DefaultBack = color;
                    Color hoverColor = Color.FromArgb(
                        Math.Min(color.R + 20, 255),
                        Math.Min(color.G + 20, 255),
                        Math.Min(color.B + 20, 255)
                    );
                    antdButton.BackHover = hoverColor;
                    antdButton.Invalidate();
                }
                else if (button is System.Windows.Forms.Button winButton)
                {
                    winButton.BackColor = color;
                    winButton.Invalidate();
                }
            }
            catch { }
        }

        private void ApplyThemeToDataGridView()
        {
            try
            {
                if (useThemeColor && themeColor != Color.Empty)
                {
                    dataGridView1.RowsDefaultCellStyle.BackColor = Color.White;
                    dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                    dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = themeColor;
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
                    dataGridView1.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(
                        Math.Min(themeColor.R + 40, 255),
                        Math.Min(themeColor.G + 40, 255),
                        Math.Min(themeColor.B + 40, 255)
                    );
                    dataGridView1.RowsDefaultCellStyle.SelectionForeColor = Color.White;
                    dataGridView1.Invalidate();
                }
            }
            catch { }
        }
        #endregion

        #region API调用方法
        private async Task<bool> TestApiConnection()
        {
            return await AuthHelper.VerifyClientConnection();
        }

        private async void LoadDataWithSpin()
        {
            if (isLoading) return;
            isLoading = true;

            AntdUI.Spin.open(this, async config =>
            {
                try
                {
                    config.Text = "正在验证客户端...";

                    bool connected = await TestApiConnection();
                    if (!connected)
                    {
                        config.Text = "客户端验证失败";
                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show("客户端验证失败！\n请确保：\n1. Python后端正在运行\n2. 使用正确的软件版本\n3. 网络连接正常",
                                "鉴权错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                        return;
                    }

                    config.Text = "正在获取数据...";

                    var requestData = new Dictionary<string, object>();
                    var response = await AuthHelper.SendAuthPostRequest("/get_apps", requestData);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorText = await response.Content.ReadAsStringAsync();
                        config.Text = "请求失败";

                        // 尝试解析错误信息
                        string errorMessage = $"HTTP错误: {response.StatusCode}";
                        try
                        {
                            using (JsonDocument doc = JsonDocument.Parse(errorText))
                            {
                                JsonElement root = doc.RootElement;
                                if (root.TryGetProperty("error", out JsonElement errorElement))
                                {
                                    errorMessage = errorElement.GetString();
                                }
                                if (root.TryGetProperty("error_code", out JsonElement errorCodeElement))
                                {
                                    errorMessage += $"\n错误代码: {errorCodeElement.GetString()}";
                                }
                            }
                        }
                        catch
                        {
                            errorMessage += $"\n响应: {errorText}";
                        }

                        throw new Exception(errorMessage);
                    }

                    string responseText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API响应: {responseText}");

                    using (JsonDocument doc = JsonDocument.Parse(responseText))
                    {
                        JsonElement root = doc.RootElement;

                        if (!root.TryGetProperty("success", out JsonElement successElement) || !successElement.GetBoolean())
                        {
                            string error = root.TryGetProperty("error", out JsonElement errorElement)
                                ? errorElement.GetString()
                                : "未知错误";
                            throw new Exception($"API错误: {error}");
                        }

                        dataList.Clear();

                        if (root.TryGetProperty("data", out JsonElement dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (JsonElement item in dataElement.EnumerateArray())
                            {
                                string appId = item.TryGetProperty("AppID", out JsonElement appIdElement)
                                    ? appIdElement.GetString() ?? ""
                                    : "";
                                string downId = item.TryGetProperty("DownID", out JsonElement downIdElement)
                                    ? downIdElement.GetString() ?? ""
                                    : "";
                                string who = item.TryGetProperty("Who", out JsonElement whoElement)
                                    ? whoElement.GetString() ?? ""
                                    : "";
                                string form = item.TryGetProperty("Form", out JsonElement formElement)
                                    ? formElement.GetString() ?? ""
                                    : "";

                                dataList.Add(new AppItem
                                {
                                    软件名 = appId,
                                    链接 = downId,
                                    上传者 = who,
                                    出处 = form
                                });
                            }
                        }

                        int count = dataList.Count;
                        config.Text = $"获取到 {count} 条数据";

                        this.Invoke(new Action(() =>
                        {
                            BindDataToGridView();
                        }));

                        config.Text = "加载完成";
                    }
                }
                catch (Exception ex)
                {
                    config.Text = "加载失败";
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show($"数据加载失败: {ex.Message}", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                    Console.WriteLine($"加载异常: {ex.Message}\n{ex.StackTrace}");
                }
                finally
                {
                    isLoading = false;
                }
            }, () =>
            {
                isLoading = false;
            });
        }

        public async Task<bool> AddApp(string softwareName, string link, string uploader, string source = "")
        {
            try
            {
                var appData = new Dictionary<string, string>
                {
                    { "软件名", softwareName },
                    { "链接", link },
                    { "上传者", uploader },
                    { "出处", source }
                };

                var response = await AuthHelper.SendAuthPostRequest("/add_app", appData);
                string responseText = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"添加应用响应: {responseText}");

                using (JsonDocument doc = JsonDocument.Parse(responseText))
                {
                    JsonElement root = doc.RootElement;
                    bool success = root.TryGetProperty("success", out JsonElement successElement) && successElement.GetBoolean();

                    if (success)
                    {
                        return true;
                    }
                    else
                    {
                        string error = root.TryGetProperty("error", out JsonElement errorElement)
                            ? errorElement.GetString()
                            : "未知错误";
                        MessageBox.Show($"添加失败: {error}", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public async Task<bool> DeleteApp(string softwareName)
        {
            try
            {
                var deleteData = new Dictionary<string, string>
                {
                    { "软件名", softwareName }
                };

                var response = await AuthHelper.SendAuthPostRequest("/delete_app", deleteData);
                string responseText = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"删除应用响应: {responseText}");

                using (JsonDocument doc = JsonDocument.Parse(responseText))
                {
                    JsonElement root = doc.RootElement;
                    bool success = root.TryGetProperty("success", out JsonElement successElement) && successElement.GetBoolean();

                    if (success)
                    {
                        return true;
                    }
                    else
                    {
                        string error = root.TryGetProperty("error", out JsonElement errorElement)
                            ? errorElement.GetString()
                            : "未知错误";
                        MessageBox.Show($"删除失败: {error}", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public async Task<bool> UpdateApp(string originalName, string newName, string link, string uploader, string source = "")
        {
            try
            {
                var updateData = new Dictionary<string, string>
                {
                    { "原始软件名", originalName },
                    { "软件名", newName },
                    { "链接", link },
                    { "上传者", uploader },
                    { "出处", source }
                };

                var response = await AuthHelper.SendAuthPostRequest("/update_app", updateData);
                string responseText = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"更新应用响应: {responseText}");

                using (JsonDocument doc = JsonDocument.Parse(responseText))
                {
                    JsonElement root = doc.RootElement;
                    bool success = root.TryGetProperty("success", out JsonElement successElement) && successElement.GetBoolean();

                    if (success)
                    {
                        return true;
                    }
                    else
                    {
                        string error = root.TryGetProperty("error", out JsonElement errorElement)
                            ? errorElement.GetString()
                            : "未知错误";
                        MessageBox.Show($"更新失败: {error}", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region 界面事件处理
        private void app_Load(object sender, EventArgs e)
        {
            if (useThemeColor && themeColor != Color.Empty)
            {
                ApplyThemeToButtons();
            }
            LoadDataWithSpin();
        }

        private void reload_Click(object sender, EventArgs e)
        {
            if (isLoading) return;
            LoadDataWithSpin();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var uploadControl = new upload();
                uploadControl.Size = new Size(300, 600);

                // 监听上传完成事件
                uploadControl.UploadCompleted += (s, args) =>
                {
                    LoadDataWithSpin();
                };

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

        private void delbtn_Click(object sender, EventArgs e)
        {
            try
            {
                var deleteControl = new delete();
                deleteControl.Size = new Size(300, 300);

                // 监听删除完成事件
                deleteControl.DeleteCompleted += (s, args) =>
                {
                    LoadDataWithSpin();
                };

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

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];
            if (row.Tag is not AppItem item)
            {
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

            if (e.ColumnIndex == dataGridView1.Columns["查看出处"].Index)
            {
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

        private void app_Load_1(object sender, EventArgs e)
        {
            LoadDataWithSpin();
        }
        #endregion

        #region 辅助方法
        private void BindDataToGridView()
        {
            try
            {
                dataGridView1.Rows.Clear();

                foreach (var item in dataList)
                {
                    int rowIndex = dataGridView1.Rows.Add(
                        item.软件名,
                        item.链接,
                        item.上传者,
                        "查看出处",
                        "下载"
                    );
                    dataGridView1.Rows[rowIndex].Tag = item;
                }

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

        public void RefreshTheme()
        {
            LoadThemeColorConfig();
            if (useThemeColor && themeColor != Color.Empty)
            {
                ApplyThemeToButtons();
            }
        }
        #endregion

        #region 数据类
        public class AppItem
        {
            public string 软件名 { get; set; }
            public string 链接 { get; set; }
            public string 上传者 { get; set; }
            public string 出处 { get; set; }
        }
        #endregion
    }
}