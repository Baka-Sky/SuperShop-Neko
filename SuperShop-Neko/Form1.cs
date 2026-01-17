using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SuperShop_Neko
{
    public partial class Form1 : AntdUI.Window
    {
        // 用户控件实例
        private welcome f1;
        private app f2;
        private tools f3;
        private more f4;
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

        // 所有按钮的列表
        private List<AntdUI.Button> allButtons = new List<AntdUI.Button>();

        // 当前显示的页面类型
        public enum PageType { Welcome, App, Tools, More, Set, About, AI }
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
            // 收集所有按钮
            CollectAllButtons();

            // 初始化控件但不立即显示
            InitializeControls();

            // 加载主题色配置
            LoadThemeColorConfig();

            // 延迟加载，确保窗体完全初始化
            this.BeginInvoke(new Action(() =>
            {
                LoadWelcomePage(true);
                _isInitialLoad = false;
            }));
        }

        /// <summary>
        /// 收集所有按钮
        /// </summary>
        private void CollectAllButtons()
        {
            // 收集所有导航按钮和功能按钮
            allButtons = new List<AntdUI.Button>
            {
                homebtn, dwn, tools, more, // 导航按钮
                button1, button2           // 功能按钮
            };
        }

        /// <summary>
        /// 加载主题色配置（使用和more.cs相同的逻辑）
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
                            ApplyThemeToAllButtons();

                            // 应用到PageHeader
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
        /// 应用主题色到所有按钮（使用和more.cs相同的逻辑）
        /// </summary>
        private void ApplyThemeToAllButtons()
        {
            if (!useThemeColor || themeColor == Color.Empty) return;

            try
            {
                foreach (var button in allButtons)
                {
                    if (button != null)
                    {
                        // 非激活按钮使用主题色，激活按钮保持爱丽丝蓝
                        bool isActiveButton = (button == currentActiveButton);
                        if (!isActiveButton)
                        {
                            ApplyColorToButton(button, themeColor);
                        }
                    }
                }
            }
            catch
            {
                // 静默失败
            }
        }

        /// <summary>
        /// 应用颜色到单个按钮（使用和more.cs相同的逻辑）
        /// </summary>
        private void ApplyColorToButton(AntdUI.Button button, Color color)
        {
            if (button == null) return;

            try
            {
                // 设置按钮颜色
                button.BackColor = color;
                button.DefaultBack = color;

                // 设置悬停色（比主题色稍亮）
                Color hoverColor = Color.FromArgb(
                    Math.Min(color.R + 20, 255),
                    Math.Min(color.G + 20, 255),
                    Math.Min(color.B + 20, 255)
                );
                button.BackHover = hoverColor;

                // 设置文字颜色确保可读性

                button.Invalidate();
            }
            catch
            {
                // 忽略错误
            }
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
                    Console.WriteLine("水印创建成功");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建调试水印时出错: {ex.Message}");
                CreateFallbackWatermark();
            }
        }

        private void CreateFallbackWatermark()
        {
            try
            {
                debugWatermarkForm = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    BackColor = Color.White,
                    TransparencyKey = Color.White,
                    ShowInTaskbar = false,
                    TopMost = false,
                    ControlBox = false,
                    StartPosition = FormStartPosition.Manual,
                    Size = this.Size,
                    Location = this.PointToScreen(Point.Empty),
                    Opacity = 0.15f,
                    Enabled = false
                };

                debugWatermarkForm.Paint += (s, e) =>
                {
                    using (Font font = new Font("Arial", 14, FontStyle.Bold))
                    using (Brush brush = new SolidBrush(Color.FromArgb(50, Color.Black)))
                    {
                        e.Graphics.TranslateTransform(100, 100);
                        e.Graphics.RotateTransform(-30);
                        e.Graphics.DrawString("Debug模式\n请勿分发", font, brush, 0, 0);
                    }
                };

                debugWatermarkForm.Show();
                Console.WriteLine("备用水印创建成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"备用水印创建失败: {ex.Message}");
            }
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

            // 订阅more控件的事件
            f4.OnSwitchToAbout += F4_OnSwitchToAbout;
            f4.OnSwitchToSet += F4_OnSwitchToSet;
            f4.OnSwitchToAI += F4_OnSwitchToAI;  // 新增AI事件订阅
        }

        private void LoadWelcomePage(bool isInitialLoad = false)
        {
            if (shop == null) return;

            shop.Controls.Clear();
            shop.Controls.Add(f1);
            FuckWelcomeHDPI.FixWelcomeOnly(shop, f1, isInitialLoad);

            UpdateButtonColors(homebtn);
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

                UpdateButtonColors(dwn);
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

                UpdateButtonColors(tools);
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
                    f4.OnSwitchToAI += F4_OnSwitchToAI;  // 新增AI事件订阅
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

                UpdateButtonColors(more);
                currentPage = PageType.More;
            }
            finally
            {
                shop.ResumeLayout(true);
            }
        }

        /// <summary>
        /// 切换到About页面
        /// </summary>
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

                // 更新按钮状态
                more.BackColor = activeButtonColor;
                more.DefaultBack = activeButtonColor;
                more.Invalidate();

                shop.ResumeLayout(true);
                shop.Invalidate();
                shop.Update();

                currentPage = PageType.About;
                Console.WriteLine("已切换到About页面");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"切换到About页面失败: {ex.Message}");
                shop.ResumeLayout(true);
            }
        }

        /// <summary>
        /// 切换到Set页面
        /// </summary>
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

                // 传递Form1的引用，以便set控件可以调用RefreshButtonColors
                // 如果set控件需要这个功能，可以在set类中添加一个Form1属性

                shop.Controls.Add(setControl);

                // 更新按钮状态
                more.BackColor = activeButtonColor;
                more.DefaultBack = activeButtonColor;
                more.Invalidate();

                shop.ResumeLayout(true);
                shop.Invalidate();
                shop.Update();

                currentPage = PageType.Set;
                Console.WriteLine("已切换到Set页面");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"切换到Set页面失败: {ex.Message}");
                shop.ResumeLayout(true);
            }
        }

        /// <summary>
        /// 切换到AI页面
        /// </summary>
        public void SwitchToAIPage()
        {
            if (shop == null) return;

            try
            {
                shop.SuspendLayout();
                shop.Controls.Clear();

                ai aiControl = new ai();  // 创建ai控件
                aiControl.Dock = DockStyle.Fill;
                aiControl.Size = shop.ClientSize;
                aiControl.Location = new Point(0, 0);

                shop.Controls.Add(aiControl);

                // 更新按钮状态
                more.BackColor = activeButtonColor;
                more.DefaultBack = activeButtonColor;
                more.Invalidate();

                shop.ResumeLayout(true);
                shop.Invalidate();
                shop.Update();

                currentPage = PageType.AI;
                Console.WriteLine("已切换到AI页面");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"切换到AI页面失败: {ex.Message}");
                shop.ResumeLayout(true);
            }
        }

        /// <summary>
        /// 处理切换到About页面的事件
        /// </summary>
        private void F4_OnSwitchToAbout(object sender, EventArgs e)
        {
            SwitchToAboutPage();
        }

        /// <summary>
        /// 处理切换到Set页面的事件
        /// </summary>
        private void F4_OnSwitchToSet(object sender, EventArgs e)
        {
            SwitchToSetPage();
        }

        /// <summary>
        /// 处理切换到AI页面的事件
        /// </summary>
        private void F4_OnSwitchToAI(object sender, EventArgs e)
        {
            SwitchToAIPage();
        }

        // 按钮点击事件
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

        /// <summary>
        /// 更新所有按钮的颜色（当前激活的按钮变蓝，其他根据主题色设置）
        /// </summary>
        private void UpdateButtonColors(AntdUI.Button activeButton)
        {
            currentActiveButton = activeButton;

            foreach (var button in allButtons)
            {
                if (button != null)
                {
                    bool isActive = (button == activeButton);
                    SetButtonColor(button, isActive);
                }
            }
        }

        /// <summary>
        /// 设置单个按钮颜色
        /// </summary>
        private void SetButtonColor(AntdUI.Button button, bool isActive)
        {
            if (button == null) return;

            try
            {
                if (isActive)
                {
                    // 激活按钮：使用爱丽丝蓝
                    ApplyColorToButton(button, activeButtonColor);
                }
                else if (useThemeColor && themeColor != Color.Empty)
                {
                    // 非激活按钮且启用主题色：使用主题色
                    ApplyColorToButton(button, themeColor);
                }
                else
                {
                    // 非激活按钮且未启用主题色：重置为默认颜色
                    ResetButtonToDefault(button);
                }

                button.Invalidate();
            }
            catch
            {
                // 忽略错误
            }
        }

        /// <summary>
        /// 重置单个按钮为默认颜色   Do you like wan you see♂
        /// </summary>
        private void ResetButtonToDefault(AntdUI.Button button)
        {
            if (button == null) return;

            try
            {
                // 重置为默认值
                button.BackColor = Color.Empty;
                button.DefaultBack = Color.Empty;
                button.BackHover = Color.Empty;

                button.Invalidate();
            }
            catch
            {
                // 忽略错误
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            throw new Exception("这是测试的崩溃异常！");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("点击确定后，程序将卡死40秒\n任务栏会显示'未响应'", "警告",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            DateTime end = DateTime.Now.AddSeconds(40);
            while (DateTime.Now < end) { }

            MessageBox.Show("已恢复！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 刷新主题色（供set控件调用）
        /// </summary>
        public void RefreshTheme()
        {
            try
            {
                // 重新加载配置
                LoadThemeColorConfig();

                // 更新当前按钮颜色
                if (currentActiveButton != null)
                {
                    UpdateButtonColors(currentActiveButton);
                }
            }
            catch
            {
                // 静默失败
            }
        }

        /// <summary>
        /// 刷新主题色（供set控件调用）
        /// </summary>
        public void RefreshButtonColors()
        {
            RefreshTheme();
        }

        /// <summary>
        /// 获取当前页面类型
        /// </summary>
        public PageType GetCurrentPage()
        {
            return currentPage;
        }
    }
}