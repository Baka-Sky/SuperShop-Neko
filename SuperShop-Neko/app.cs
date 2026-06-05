using AntdUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            LoadThemeColorConfig();
        }

        #region 初始化方法
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
            }
            catch { }
        }
        #endregion

        #region API调用方法
        private async Task<bool> TestApiConnection()
        {
            try
            {
                bool connected = await AuthHelper.TestConnectionAsync();
                if (!connected) return false;

                return await AuthHelper.RefreshSessionAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"连接测试异常: {ex.Message}");
                return false;
            }
        }

        private async void LoadDataWithSpin()
        {
            if (isLoading) return;
            isLoading = true;

            AntdUI.Spin.open(this, async config =>
            {
                try
                {
                    config.Text = "正在初始化安全连接...";
                    await Task.Delay(80);

                    config.Text = "正在连接服务器...";

                    bool connected = await TestApiConnection();
                    if (!connected)
                    {
                        config.Text = "连接失败";
                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show("客户端验证失败！\n请确保：\n1. Python后端正在运行\n2. 使用正确的软件版本\n3. 网络连接正常",
                                "鉴权错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                        return;
                    }
                    config.Text = "服务器连接成功";

                    config.Text = "正在获取会话密钥...";
                    await Task.Delay(50);

                    config.Text = "正在生成请求签名...";
                    await Task.Delay(50);

                    config.Text = "正在发送数据请求...";

                    var requestData = new Dictionary<string, object>();
                    var response = await AuthHelper.SendAuthPostRequest("/get_apps", requestData);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorText = await response.Content.ReadAsStringAsync();
                        config.Text = "请求失败";

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

                    config.Text = "正在解析服务器响应...";

                    string responseText = await response.Content.ReadAsStringAsync();
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

                        config.Text = "正在清空旧数据...";
                        dataList.Clear();

                        if (root.TryGetProperty("data", out JsonElement dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                        {
                            config.Text = "正在加载应用列表...";
                            int totalItems = dataElement.GetArrayLength();
                            int processed = 0;

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

                                processed++;
                                if (processed % 3 == 0 || processed == totalItems)
                                {
                                    config.Text = $"正在加载应用列表... ({processed}/{totalItems})";
                                }
                            }
                        }

                        int count = dataList.Count;
                        config.Text = $"加载完成，共 {count} 条数据";

                        this.Invoke(new Action(() =>
                        {
                            config.Text = "正在渲染界面...";
                            BindDataToGridView();
                        }));

                        await Task.Delay(100);
                        config.Text = $"完成，已加载 {count} 个应用";
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
            SmallPanel.Hide();
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
                MessageBox.Show($"错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void delbtn_Click(object sender, EventArgs e)
        {
            try
            {
                var deleteControl = new delete();
                deleteControl.Size = new Size(300, 300);

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
                MessageBox.Show($"错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 辅助方法
        private void BindDataToGridView()
        {
            try
            {
                SuperPanel.Controls.Clear();
                SuperPanel.AutoScroll = true;
                SuperPanel.HorizontalScroll.Visible = false;

                int itemWidth = SmallPanel.Width;
                int itemHeight = SmallPanel.Height;
                int startY = 10;
                int margin = 8;

                foreach (var item in dataList)
                {
                    AntdUI.Panel itemPanel = new AntdUI.Panel();
                    itemPanel.Size = new Size(itemWidth, itemHeight);
                    itemPanel.Location = new Point(6, startY);
                    itemPanel.BorderWidth = 1;
                    itemPanel.BackColor = Color.White;

                    AntdUI.Label appname = new AntdUI.Label();
                    appname.Font = SmallPanel.Controls["appname"].Font;
                    appname.Location = SmallPanel.Controls["appname"].Location;
                    appname.Size = SmallPanel.Controls["appname"].Size;
                    appname.Text = item.软件名;
                    appname.BackColor = Color.Transparent;

                    AntdUI.Label who = new AntdUI.Label();
                    who.Font = SmallPanel.Controls["who"].Font;
                    who.Location = SmallPanel.Controls["who"].Location;
                    who.Size = SmallPanel.Controls["who"].Size;
                    who.Text = "上传者: " + item.上传者;
                    who.BackColor = Color.Transparent;

                    AntdUI.Label form = new AntdUI.Label();
                    form.Font = SmallPanel.Controls["form"].Font;
                    form.Location = SmallPanel.Controls["form"].Location;
                    form.Size = SmallPanel.Controls["form"].Size;
                    form.Text = "来源: " + item.出处;
                    form.BackColor = Color.Transparent;

                    AntdUI.Button download = new AntdUI.Button();
                    download.Location = SmallPanel.Controls["download"].Location;
                    download.Size = SmallPanel.Controls["download"].Size;
                    download.Text = "下载软件";
                    download.Tag = item.链接;

                    if (useThemeColor && themeColor != Color.Empty)
                    {
                        ApplyColorToButton(download, themeColor);
                    }
                    else
                    {
                        download.DefaultBack = Color.AliceBlue;
                    }

                    download.Click += (s, e) =>
                    {
                        try
                        {
                            AntdUI.Button btn = (AntdUI.Button)s;
                            string url = btn.Tag.ToString();
                            if (!string.IsNullOrWhiteSpace(url))
                            {
                                if (!url.StartsWith("http")) url = "http://" + url;
                                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                            }
                        }
                        catch { }
                    };

                    AntdUI.Button wherebtn = new AntdUI.Button();
                    wherebtn.Location = SmallPanel.Controls["wherebtn"].Location;
                    wherebtn.Size = SmallPanel.Controls["wherebtn"].Size;
                    wherebtn.Text = "查看出处";
                    wherebtn.Tag = item.出处;

                    if (useThemeColor && themeColor != Color.Empty)
                    {
                        ApplyColorToButton(wherebtn, themeColor);
                    }
                    else
                    {
                        wherebtn.DefaultBack = Color.AliceBlue;
                    }

                    wherebtn.Click += (s, e) =>
                    {
                        try
                        {
                            AntdUI.Button btn = (AntdUI.Button)s;
                            string url = btn.Tag.ToString();
                            if (!string.IsNullOrWhiteSpace(url))
                            {
                                if (!url.StartsWith("http")) url = "http://" + url;
                                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                            }
                        }
                        catch { }
                    };

                    itemPanel.Controls.Add(appname);
                    itemPanel.Controls.Add(who);
                    itemPanel.Controls.Add(form);
                    itemPanel.Controls.Add(download);
                    itemPanel.Controls.Add(wherebtn);

                    SuperPanel.Controls.Add(itemPanel);
                    startY += itemHeight + margin;
                }

                if (dataList.Count == 0)
                {
                    AntdUI.Label tip = new AntdUI.Label();
                    tip.Text = "暂无数据";
                    tip.Font = new Font("MiSans Medium", 10F);
                    tip.ForeColor = Color.Gray;
                    tip.Location = new Point(SuperPanel.Width / 2 - 50, SuperPanel.Height / 2);
                    tip.BackColor = Color.Transparent;
                    SuperPanel.Controls.Add(tip);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载失败：" + ex.Message);
            }
        }

        private Form GetParentForm()
        {
            Control control = this;
            while (control != null && !(control is Form))
            {
                control = control.Parent;
            }
            return control as Form ?? Form.ActiveForm;
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

        private void app_Load_1(object sender, EventArgs e)
        {
            SmallPanel.Hide();     // 隐藏模板
            LoadDataWithSpin();    // 加载数据
        }
    }
}