using AntdUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperShop_Neko
{
    public partial class Form1 : AntdUI.Window
    {
        private welcome f1;
        private app f2;
        private tools f3;
        private more f4;
        private user f5;
        private bool _isInitialLoad = true;

        private Color activeButtonColor = Color.AliceBlue;
        private AntdUI.Button currentActiveButton;

        private AntdUI.Watermark.Config? debugWatermarkConfig;
        private Form? debugWatermarkForm;

        private Color themeColor = Color.Empty;
        private bool useThemeColor = false;
        private List<AntdUI.Button> allAntdButtons = new List<AntdUI.Button>();

        public enum PageType { Welcome, App, Tools, More, Set, About, AI, User }
        private PageType currentPage = PageType.Welcome;

        public heartengine HeartEngine;

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
            HeartEngine = new heartengine(this);

            CollectAllAntdButtons();
            InitializeControls();
            LoadThemeColorConfig();

            this.BeginInvoke(new Action(() =>
            {
                LoadWelcomePage(true);
                _isInitialLoad = false;
                _ = HeartEngine.CheckForUpdateAsync();
            }));
        }

        private void CollectAllAntdButtons()
        {
            if (homebtn is AntdUI.Button) allAntdButtons.Add(homebtn as AntdUI.Button);
            if (dwn is AntdUI.Button) allAntdButtons.Add(dwn as AntdUI.Button);
            if (tools is AntdUI.Button) allAntdButtons.Add(tools as AntdUI.Button);
            if (more is AntdUI.Button) allAntdButtons.Add(more as AntdUI.Button);
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

        private void ApplyThemeToAllButtons()
        {
            if (!useThemeColor || themeColor == Color.Empty) return;
            try
            {
                foreach (var button in allAntdButtons)
                {
                    if (button != null && button != currentActiveButton)
                        ApplyColorToButton(button, themeColor);
                }
            }
            catch { }
        }

        private void ApplyColorToButton(AntdUI.Button button, Color color)
        {
            if (button == null) return;
            try
            {
                button.BackColor = color;
                button.DefaultBack = color;
                Color hover = Color.FromArgb(Math.Min(color.R + 20, 255), Math.Min(color.G + 20, 255), Math.Min(color.B + 20, 255));
                button.BackHover = hover;
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
                        FuckWelcomeHDPI.RefreshControlLayout(shop);
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

                debugWatermarkConfig = new AntdUI.Watermark.Config(
                    this, "Debug模式 请勿分发", $"构建时间: {buildTimeStr}");

                debugWatermarkConfig.SetRotate(-30).SetOpacity(0.2f).SetFore(Color.Black).SetGap(100);
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
                FuckWelcomeHDPI.AdjustControlSize(shop);

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
                UpdateButtonColors(dwn as AntdUI.Button);
                currentPage = PageType.App;
            }
            finally { shop.ResumeLayout(true); }
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
                UpdateButtonColors(tools as AntdUI.Button);
                currentPage = PageType.Tools;
            }
            finally { shop.ResumeLayout(true); }
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
                UpdateButtonColors(more as AntdUI.Button);
                currentPage = PageType.More;
            }
            finally { shop.ResumeLayout(true); }
        }

        private void LoadUserPage()
        {
            if (shop == null) return;
            shop.SuspendLayout();
            try
            {
                shop.Controls.Clear();
                if (f5 == null) f5 = new user();
                f5.Visible = false;
                f5.Dock = DockStyle.Fill;
                f5.Size = shop.ClientSize;
                f5.Location = new Point(0, 0);
                shop.Controls.Add(f5);
                f5.Visible = true;
                SetAllButtonsToInactive();
                currentPage = PageType.User;
            }
            finally { shop.ResumeLayout(true); }
        }

        private void SetAllButtonsToInactive()
        {
            currentActiveButton = null;
            foreach (var btn in allAntdButtons)
            {
                if (btn == null) continue;
                if (useThemeColor && themeColor != Color.Empty)
                    ApplyColorToButton(btn, themeColor);
                else
                    ResetButtonToDefault(btn);
                btn.Invalidate();
            }
        }

        public void SwitchToAboutPage()
        {
            if (shop == null) return;
            try
            {
                shop.SuspendLayout();
                shop.Controls.Clear();
                about c = new about();
                c.Dock = DockStyle.Fill;
                shop.Controls.Add(c);
                if (more is AntdUI.Button b)
                {
                    b.BackColor = activeButtonColor;
                    b.DefaultBack = activeButtonColor;
                    b.Invalidate();
                }
                shop.ResumeLayout(true);
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
                set c = new set();
                c.Dock = DockStyle.Fill;
                shop.Controls.Add(c);
                if (more is AntdUI.Button b)
                {
                    b.BackColor = activeButtonColor;
                    b.DefaultBack = activeButtonColor;
                    b.Invalidate();
                }
                shop.ResumeLayout(true);
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
                ai c = new ai();
                c.Dock = DockStyle.Fill;
                shop.Controls.Add(c);
                if (more is AntdUI.Button b)
                {
                    b.BackColor = activeButtonColor;
                    b.DefaultBack = activeButtonColor;
                    b.Invalidate();
                }
                shop.ResumeLayout(true);
                currentPage = PageType.AI;
            }
            catch { }
        }

        private void F4_OnSwitchToAbout(object sender, EventArgs e) => SwitchToAboutPage();
        private void F4_OnSwitchToSet(object sender, EventArgs e) => SwitchToSetPage();
        private void F4_OnSwitchToAI(object sender, EventArgs e) => SwitchToAIPage();

        private void homebtn_Click(object sender, EventArgs e) => LoadWelcomePage();
        private void dwn_Click(object sender, EventArgs e) => LoadAppPage();
        private void tools_Click(object sender, EventArgs e) => LoadToolsPage();
        private void more_Click(object sender, EventArgs e) => LoadMorePage();
        private void user_Click(object sender, EventArgs e) => LoadUserPage();

        private void UpdateButtonColors(AntdUI.Button activeButton)
        {
            currentActiveButton = activeButton;
            foreach (var btn in allAntdButtons)
            {
                if (btn == null) continue;
                SetButtonColor(btn, btn == activeButton);
            }
        }

        private void SetButtonColor(AntdUI.Button button, bool isActive)
        {
            if (button == null) return;
            try
            {
                if (isActive)
                    ApplyColorToButton(button, activeButtonColor);
                else if (useThemeColor && themeColor != Color.Empty)
                    ApplyColorToButton(button, themeColor);
                else
                    ResetButtonToDefault(button);
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

        public void RefreshTheme()
        {
            try
            {
                LoadThemeColorConfig();
                if (currentActiveButton != null)
                    UpdateButtonColors(currentActiveButton);
                else
                    SetAllButtonsToInactive();
            }
            catch { }
        }

        public void RefreshButtonColors() => RefreshTheme();
        public PageType GetCurrentPage() => currentPage;

        private void toast_Click(object sender, EventArgs e)
        {
            HeartEngine.ShowUpdateToast("4.0.1");
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
