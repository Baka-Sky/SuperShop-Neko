using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Notifications;

namespace SuperShop_Neko
{
    public partial class Form1 : AntdUI.Window
    {
        // 用户控件实例
        private welcome f1;
        private app f2;
        private tools f3;
        private more f4;
        private user f5;
        private bool _isInitialLoad = true;

        // 按钮颜色管理相关
        private Color activeButtonColor = Color.AliceBlue;
        private AntdUI.Button currentActiveButton;

        // 水印相关
        private AntdUI.Watermark.Config? debugWatermarkConfig;
        private Form? debugWatermarkForm;

        // 存储主题配置
        private Color themeColor = Color.Empty;
        private bool useThemeColor = false;

        // 所有AntdUI按钮的列表
        private List<AntdUI.Button> allAntdButtons = new List<AntdUI.Button>();

        // 当前显示的页面类型
        public enum PageType { Welcome, App, Tools, More, Set, About, AI, User }
        private PageType currentPage = PageType.Welcome;

        public Form1()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.None;

            this.Load += Form1_Load;
            this.Shown += Form1_Shown;
            this.Resize += Form1_Resize;

#if DEBUG
            this.Shown += Form1_Shown_Debug;
#endif
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 收集所有AntdUI按钮
            CollectAllAntdButtons();

            // 初始化控件但不立即显示
            InitializeControls();

            // 加载主题色配置
            LoadThemeColorConfig();

            // 延迟加载，确保窗体完全初始化
            this.BeginInvoke(new Action(() =>
            {
                LoadWelcomePage(true);
                _isInitialLoad = false;

                // 启动无感的更新检查（不等待、不阻塞）
                _ = CheckForUpdateAsync();
            }));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 立即终止整个进程
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// 收集所有AntdUI按钮（不包括PictureBox）
        /// </summary>
        private void CollectAllAntdButtons()
        {
            if (homebtn is AntdUI.Button)
                allAntdButtons.Add(homebtn as AntdUI.Button);

            if (dwn is AntdUI.Button)
                allAntdButtons.Add(dwn as AntdUI.Button);

            if (tools is AntdUI.Button)
                allAntdButtons.Add(tools as AntdUI.Button);

            if (more is AntdUI.Button)
                allAntdButtons.Add(more as AntdUI.Button);
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
                            ApplyThemeToAllButtons();

                            if (page != null)
                            {
                                page.BackColor = themeColor;
                                page.Invalidate();
                            }
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
        /// 应用主题色到所有按钮
        /// </summary>
        private void ApplyThemeToAllButtons()
        {
            if (!useThemeColor || themeColor == Color.Empty) return;

            try
            {
                foreach (var button in allAntdButtons)
                {
                    if (button != null)
                    {
                        bool isActiveButton = (button == currentActiveButton);
                        if (!isActiveButton)
                        {
                            ApplyColorToButton(button, themeColor);
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 应用颜色到单个按钮
        /// </summary>
        private void ApplyColorToButton(AntdUI.Button button, Color color)
        {
            if (button == null) return;

            try
            {
                button.BackColor = color;
                button.DefaultBack = color;

                Color hoverColor = Color.FromArgb(
                    Math.Min(color.R + 20, 255),
                    Math.Min(color.G + 20, 255),
                    Math.Min(color.B + 20, 255)
                );
                button.BackHover = hoverColor;

                button.Invalidate();
            }
            catch { }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (_isInitialLoad)
            {
                this.BeginInvoke(new Action(() =>
                {
                    if (shop != null && shop.Controls.Count > 0)
                    {
                        FuckWelcomeHDPI.RefreshControlLayout(shop);
                    }
                }));
            }
        }

#if DEBUG
        private void Form1_Shown_Debug(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(async () =>
            {
                await Task.Delay(1500);
                CreateDebugWatermark();
            }));
        }

        private void CreateDebugWatermark()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var buildTime = System.IO.File.GetLastWriteTime(assembly.Location);
                string buildTimeStr = buildTime.ToString("yyyy-MM-dd HH:mm:ss");

                string watermarkText = "Debug模式 请勿分发";
                string watermarkText2 = $"构建时间: {buildTimeStr}";

                debugWatermarkConfig = new AntdUI.Watermark.Config(
                    this,
                    watermarkText,
                    watermarkText2
                );

                debugWatermarkConfig
                    .SetRotate(-30)
                    .SetOpacity(0.2f)
                    .SetFore(Color.Black)
                    .SetGap(100);

                debugWatermarkForm = AntdUI.Watermark.open(debugWatermarkConfig);

                if (debugWatermarkForm != null)
                {
                    debugWatermarkForm.TopMost = false;
                    debugWatermarkForm.Enabled = false;
                }
            }
            catch { }
        }
#endif

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (shop != null)
            {
                FuckWelcomeHDPI.AdjustControlSize(shop);
            }

#if DEBUG
            if (debugWatermarkForm != null && !debugWatermarkForm.IsDisposed)
            {
                debugWatermarkForm.Size = this.Size;
                debugWatermarkForm.Location = this.PointToScreen(Point.Empty);
                debugWatermarkForm.Refresh();
            }
#endif
        }

        private void InitializeControls()
        {
            FuckWelcomeHDPI.InitializeWelcomeControl(out f1);

            f2 = new app();
            f2.Visible = false;

            f3 = new tools();
            f3.Visible = false;

            f4 = new more();
            f4.Visible = false;

            f5 = new user();
            f5.Visible = false;

            f4.OnSwitchToAbout += F4_OnSwitchToAbout;
            f4.OnSwitchToSet += F4_OnSwitchToSet;
            f4.OnSwitchToAI += F4_OnSwitchToAI;
        }

        #region ==================== 更新检查功能 ====================

        /// <summary>
        /// 异步检查更新（完全无阻塞）
        /// </summary>
        private async Task CheckForUpdateAsync()
        {
            try
            {
                // 1. 等待2秒让界面先完成加载
                await Task.Delay(2000);

                // 2. 在主线程上更新UI - 显示Loading
                this.Invoke(new Action(() =>
                {
                    if (page != null)
                    {
                        page.Loading = true;
                        page.Text = "正在检查更新...";
                    }
                }));

                // 3. 获取服务器版本号
                string serverVersionString = await GetServerVersionAsync();
                if (string.IsNullOrEmpty(serverVersionString))
                {
                    // 获取失败，关闭Loading
                    this.Invoke(new Action(() =>
                    {
                        if (page != null) page.Loading = false;
                    }));
                    return;
                }

                // 4. 更新UI显示目标版本
                this.Invoke(new Action(() =>
                {
                    if (page != null)
                    {
                        page.Text = $"有更新! 目标版本:{serverVersionString}";
                    }
                }));

                // 5. 读取本地配置文件中的版本
                string localVersion = ReadLocalVersionFromConfig();
                if (string.IsNullOrEmpty(localVersion))
                {
                    this.Invoke(new Action(() =>
                    {
                        if (page != null) page.Loading = false;
                    }));
                    return;
                }

                // 6. 版本比较
                if (IsNewerVersionAvailable(localVersion, serverVersionString))
                {
                    // 有新版本，弹出Toast通知
                    ShowUpdateToast(serverVersionString);
                }
                else
                {
                    // 没有更新，关闭Loading
                    this.Invoke(new Action(() =>
                    {
                        if (page != null) page.Loading = false;
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查更新时出错: {ex.Message}");
                // 确保出错时关闭Loading
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        if (page != null) page.Loading = false;
                    }));
                }
                catch { }
            }
        }

        /// <summary>
        /// 从服务器获取版本号
        /// </summary>
        private async Task<string> GetServerVersionAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    string response = await client.GetStringAsync("https://shop.baka233.top/update/version.txt");
                    return response?.Trim();
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 从config.json读取本地版本号
        /// </summary>
        private string ReadLocalVersionFromConfig()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                if (!File.Exists(configPath))
                {
                    return null;
                }

                string json = File.ReadAllText(configPath);
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("Version", out JsonElement versionElement))
                    {
                        return versionElement.GetString();
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 比较版本号
        /// </summary>
        private bool IsNewerVersionAvailable(string localVersion, string serverVersion)
        {
            try
            {
                Version local = new Version(localVersion);
                Version server = new Version(serverVersion);
                return server > local;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 显示更新Toast通知
        /// </summary>
        private void ShowUpdateToast(string targetVersion)
        {
            try
            {
                string localVersion = ReadLocalVersionFromConfig() ?? "未知";

                // 创建带有按钮的更新通知Toast
                new ToastContentBuilder()
                    .AddArgument("action", "update_notification")
                    .AddArgument("target_version", targetVersion)

                    // 添加标题和内容
                    .AddText("超级小铺有新版本可用！", hintMaxLines: 1)
                    .AddText($"当前版本: {localVersion}")
                    .AddText($"目标版本: {targetVersion}")
                    .AddText("是否立即更新？")

                    // 添加按钮
                    .AddButton(new ToastButton()
                        .SetContent("立即更新")
                        .AddArgument("choice", "update_now")
                        .AddArgument("target_version", targetVersion))

                    .AddButton(new ToastButton()
                        .SetContent("稍后提醒")
                        .AddArgument("choice", "update_later"))

                    // 设置显示时间
                    .SetToastDuration(ToastDuration.Long)

                    // 显示Toast并处理激活事件
                    .Show(toast =>
                    {
                        toast.Tag = "update_notification";
                        toast.Group = "super_shop_updates";

                        // 订阅Toast激活事件
                        toast.Activated += Toast_Activated;
                    });
            }
            catch (Exception ex)
            {
                // 如果Toast失败，显示MessageBox
                ShowUpdateMessageBox(targetVersion);
            }
        }

        // Toast激活事件处理
        private async void Toast_Activated(Windows.UI.Notifications.ToastNotification sender, object args)
        {
            try
            {
                // 解析参数
                string arguments = string.Empty;
                string targetVersion = "未知";

                // 检查不同的参数获取方式
                if (args is ToastActivatedEventArgs toastArgs)
                {
                    arguments = toastArgs.Arguments;

                    // 解析参数获取target_version
                    var parameters = arguments.Split(';');
                    foreach (var param in parameters)
                    {
                        var parts = param.Split('=');
                        if (parts.Length == 2 && parts[0] == "target_version")
                        {
                            targetVersion = parts[1];
                            break;
                        }
                    }
                }

                // 延迟一下，确保Toast关闭
                await System.Threading.Tasks.Task.Delay(100);

                // 在主线程中显示消息框
                this.Invoke(new Action(() =>
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        // 解析参数
                        var parameters = arguments.Split(';');
                        string choice = "";

                        foreach (var param in parameters)
                        {
                            var parts = param.Split('=');
                            if (parts.Length == 2 && parts[0] == "choice")
                            {
                                choice = parts[1];
                                break;
                            }
                        }

                        if (choice == "update_now")
                        {
                            HandleUpdateNow(targetVersion);
                        }
                        else if (choice == "update_later")
                        {
                            HandleUpdateLater();
                        }
                    }
                    else
                    {
                        // 如果没有参数，显示默认更新对话框
                        ShowUpdateMessageBox(targetVersion);
                    }
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show($"处理Toast点击时出错: {ex.Message}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        // 备选方案：显示MessageBox
        private void ShowUpdateMessageBox(string targetVersion)
        {
            string localVersion = ReadLocalVersionFromConfig() ?? "未知";

            var result = MessageBox.Show(
                $"超级小铺有新版本可用！\n\n" +
                $"当前版本: {localVersion}\n" +
                $"目标版本: {targetVersion}\n\n" +
                "是否立即更新？",
                "更新通知",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                HandleUpdateNow(targetVersion);
            }
            else
            {
                HandleUpdateLater();
            }
        }

        /// <summary>
        /// 处理立即更新
        /// </summary>
        private async void HandleUpdateNow(string targetVersion)
        {
            try
            {
                // 将page的Text属性改为"启动更新......"
                if (page != null)
                {
                    page.Text = "启动更新......";
                }

                // 异步打开浏览器，不阻塞UI
                await Task.Run(() =>
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://shop.baka233.top/shop.exe",
                            UseShellExecute = true
                        });
                    }
                    catch
                    {
                        // 备用方法
                        Process.Start("https://shop.baka233.top/shop.exe");
                    }
                });

                MessageBox.Show("正在启动下载...\n请稍后查看浏览器下载内容。", "更新",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动下载失败: {ex.Message}\n请手动访问官网下载。", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // 尝试打开官网
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://shop.baka233.top",
                        UseShellExecute = true
                    });
                }
                catch { }
            }
        }

        /// <summary>
        /// 处理稍后提醒
        /// </summary>
        private void HandleUpdateLater()
        {
            if (page != null)
            {
                page.Loading = false;
            }

            MessageBox.Show("已选择稍后提醒，下次启动时会再次检查更新。", "更新提醒",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        private void LoadWelcomePage(bool isInitialLoad = false)
        {
            if (shop == null) return;

            shop.Controls.Clear();
            shop.Controls.Add(f1);
            FuckWelcomeHDPI.FixWelcomeOnly(shop, f1, isInitialLoad);

            UpdateButtonColors(homebtn as AntdUI.Button);
            currentPage = PageType.Welcome;
        }

        private void LoadAppPage()
        {
            if (shop == null) return;

            shop.SuspendLayout();
            try
            {
                shop.Controls.Clear();

                if (f2 == null) f2 = new app();

                f2.Visible = false;
                f2.Dock = DockStyle.Fill;
                f2.Size = shop.ClientSize;
                f2.Location = new Point(0, 0);

                shop.Controls.Add(f2);
                f2.Visible = true;

                shop.PerformLayout();
                f2.Invalidate();
                f2.Update();

                UpdateButtonColors(dwn as AntdUI.Button);
                currentPage = PageType.App;
            }
            finally
            {
                shop.ResumeLayout(true);
            }
        }

        private void LoadToolsPage()
        {
            if (shop == null) return;

            shop.SuspendLayout();
            try
            {
                shop.Controls.Clear();

                if (f3 == null) f3 = new tools();

                f3.Visible = false;
                f3.Dock = DockStyle.Fill;
                f3.Size = shop.ClientSize;
                f3.Location = new Point(0, 0);

                shop.Controls.Add(f3);
                f3.Visible = true;

                shop.PerformLayout();
                f3.Invalidate();
                f3.Update();

                UpdateButtonColors(tools as AntdUI.Button);
                currentPage = PageType.Tools;
            }
            finally
            {
                shop.ResumeLayout(true);
            }
        }

        private void LoadMorePage()
        {
            if (shop == null) return;

            shop.SuspendLayout();
            try
            {
                shop.Controls.Clear();

                if (f4 == null)
                {
                    f4 = new more();
                    f4.OnSwitchToAbout += F4_OnSwitchToAbout;
                    f4.OnSwitchToSet += F4_OnSwitchToSet;
                    f4.OnSwitchToAI += F4_OnSwitchToAI;
                }

                f4.Visible = false;
                f4.Dock = DockStyle.Fill;
                f4.Size = shop.ClientSize;
                f4.Location = new Point(0, 0);

                shop.Controls.Add(f4);
                f4.Visible = true;

                shop.PerformLayout();
                f4.Invalidate();
                f4.Update();

                UpdateButtonColors(more as AntdUI.Button);
                currentPage = PageType.More;
            }
            finally
            {
                shop.ResumeLayout(true);
            }
        }

        private void LoadUserPage()
        {
            if (shop == null) return;

            shop.SuspendLayout();
            try
            {
                shop.Controls.Clear();

                if (f5 == null)
                {
                    f5 = new user();
                }

                f5.Visible = false;
                f5.Dock = DockStyle.Fill;
                f5.Size = shop.ClientSize;
                f5.Location = new Point(0, 0);

                shop.Controls.Add(f5);
                f5.Visible = true;

                shop.PerformLayout();
                f5.Invalidate();
                f5.Update();

                SetAllButtonsToInactive();
                currentPage = PageType.User;
            }
            finally
            {
                shop.ResumeLayout(true);
            }
        }

        private void SetAllButtonsToInactive()
        {
            currentActiveButton = null;

            foreach (var button in allAntdButtons)
            {
                if (button != null)
                {
                    if (useThemeColor && themeColor != Color.Empty)
                    {
                        ApplyColorToButton(button, themeColor);
                    }
                    else
                    {
                        ResetButtonToDefault(button);
                    }
                    button.Invalidate();
                }
            }
        }

        public void SwitchToAboutPage()
        {
            if (shop == null) return;

            try
            {
                shop.SuspendLayout();
                shop.Controls.Clear();

                about aboutControl = new about();
                aboutControl.Dock = DockStyle.Fill;
                aboutControl.Size = shop.ClientSize;
                aboutControl.Location = new Point(0, 0);

                shop.Controls.Add(aboutControl);

                if (more is AntdUI.Button moreButton)
                {
                    moreButton.BackColor = activeButtonColor;
                    moreButton.DefaultBack = activeButtonColor;
                    moreButton.Invalidate();
                }

                shop.ResumeLayout(true);
                shop.Invalidate();
                shop.Update();

                currentPage = PageType.About;
            }
            catch { }
        }

        public void SwitchToSetPage()
        {
            if (shop == null) return;

            try
            {
                shop.SuspendLayout();
                shop.Controls.Clear();

                set setControl = new set();
                setControl.Dock = DockStyle.Fill;
                setControl.Size = shop.ClientSize;
                setControl.Location = new Point(0, 0);

                shop.Controls.Add(setControl);

                if (more is AntdUI.Button moreButton)
                {
                    moreButton.BackColor = activeButtonColor;
                    moreButton.DefaultBack = activeButtonColor;
                    moreButton.Invalidate();
                }

                shop.ResumeLayout(true);
                shop.Invalidate();
                shop.Update();

                currentPage = PageType.Set;
            }
            catch { }
        }

        public void SwitchToAIPage()
        {
            if (shop == null) return;

            try
            {
                shop.SuspendLayout();
                shop.Controls.Clear();

                ai aiControl = new ai();
                aiControl.Dock = DockStyle.Fill;
                aiControl.Size = shop.ClientSize;
                aiControl.Location = new Point(0, 0);

                shop.Controls.Add(aiControl);

                if (more is AntdUI.Button moreButton)
                {
                    moreButton.BackColor = activeButtonColor;
                    moreButton.DefaultBack = activeButtonColor;
                    moreButton.Invalidate();
                }

                shop.ResumeLayout(true);
                shop.Invalidate();
                shop.Update();

                currentPage = PageType.AI;
            }
            catch { }
        }

        private void F4_OnSwitchToAbout(object sender, EventArgs e)
        {
            SwitchToAboutPage();
        }

        private void F4_OnSwitchToSet(object sender, EventArgs e)
        {
            SwitchToSetPage();
        }

        private void F4_OnSwitchToAI(object sender, EventArgs e)
        {
            SwitchToAIPage();
        }

        private void homebtn_Click(object sender, EventArgs e)
        {
            LoadWelcomePage();
        }

        private void dwn_Click(object sender, EventArgs e)
        {
            LoadAppPage();
        }

        private void tools_Click(object sender, EventArgs e)
        {
            LoadToolsPage();
        }

        private void more_Click(object sender, EventArgs e)
        {
            LoadMorePage();
        }

        private void user_Click(object sender, EventArgs e)
        {
            LoadUserPage();
        }

        private void UpdateButtonColors(AntdUI.Button activeButton)
        {
            currentActiveButton = activeButton;

            foreach (var button in allAntdButtons)
            {
                if (button != null)
                {
                    bool isActive = (button == activeButton);
                    SetButtonColor(button, isActive);
                }
            }
        }

        private void SetButtonColor(AntdUI.Button button, bool isActive)
        {
            if (button == null) return;

            try
            {
                if (isActive)
                {
                    ApplyColorToButton(button, activeButtonColor);
                }
                else if (useThemeColor && themeColor != Color.Empty)
                {
                    ApplyColorToButton(button, themeColor);
                }
                else
                {
                    ResetButtonToDefault(button);
                }

                button.Invalidate();
            }
            catch { }
        }

        private void ResetButtonToDefault(AntdUI.Button button)
        {
            if (button == null) return;

            try
            {
                button.BackColor = Color.Empty;
                button.DefaultBack = Color.Empty;
                button.BackHover = Color.Empty;
                button.Invalidate();
            }
            catch { }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

#if DEBUG
            if (debugWatermarkForm != null && !debugWatermarkForm.IsDisposed)
            {
                debugWatermarkForm.Close();
                debugWatermarkForm.Dispose();
                debugWatermarkForm = null;
            }
#endif

            FuckWelcomeHDPI.Cleanup();
        }

        public void RefreshTheme()
        {
            try
            {
                LoadThemeColorConfig();

                if (currentActiveButton != null)
                {
                    UpdateButtonColors(currentActiveButton);
                }
                else
                {
                    SetAllButtonsToInactive();
                }
            }
            catch { }
        }

        public void RefreshButtonColors()
        {
            RefreshTheme();
        }

        public PageType GetCurrentPage()
        {
            return currentPage;
        }

        private void toast_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建带有按钮的更新通知Toast
                new ToastContentBuilder()
                    .AddArgument("action", "update_notification")
                    .AddArgument("current_version", "4.0.0")
                    .AddArgument("target_version", "4.0.1")

                    // 添加标题和内容
                    .AddText("超级小铺又有新更新了！", hintMaxLines: 1)
                    .AddText("当前版本: 4.0.0")
                    .AddText("目标版本: 4.0.1")
                    .AddText("是否更新？")

                    // 添加按钮
                    .AddButton(new ToastButton()
                        .SetContent("更新!")
                        .AddArgument("choice", "update_now")
                        .AddArgument("from", "toast"))

                    .AddButton(new ToastButton()
                        .SetContent("让我想想")
                        .AddArgument("choice", "update_later")
                        .AddArgument("from", "toast"))

                    // 设置显示时间
                    .SetToastDuration(ToastDuration.Long)

                    // 显示Toast并处理激活事件
                    .Show(toast =>
                    {
                        toast.Tag = "update_notification";
                        toast.Group = "super_shop_updates";

                        // 订阅Toast激活事件
                        toast.Activated += Toast_Activated;
                    });
            }
            catch (Exception ex)
            {
                // 如果Toast失败，显示MessageBox
                ShowUpdateMessageBox("4.0.1");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}