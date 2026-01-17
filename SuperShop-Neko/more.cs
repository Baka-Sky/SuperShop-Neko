using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;

namespace SuperShop_Neko
{
    public partial class more : UserControl
    {
        // 确保使用正确的窗体名称 viedoplayer
        private viedoplayer _viedoplayerForm;

        // 主题色相关
        private Color themeColor = Color.Empty;
        private bool useThemeColor = false;

        // 所有按钮的列表
        private List<AntdUI.Button> allButtons = new List<AntdUI.Button>();

        // 事件声明
        public event EventHandler OnSwitchToAbout;
        public event EventHandler OnSwitchToSet;
        public event EventHandler OnSwitchToAI;  // 新增AI页面切换事件

        public more()
        {
            InitializeComponent();

            // 收集所有按钮
            CollectAllButtons();

            // 加载主题色配置
            LoadThemeColorConfig();

            // 设置点击事件
            button4.Click += button4_Click;
            setbtn.Click += setbtn_Click;
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;  // 新增button3点击事件绑定

            // 订阅父窗体关闭事件
            var parentForm = this.FindForm();
            if (parentForm != null)
            {
                parentForm.FormClosed += ParentForm_FormClosed;
            }
        }

        /// <summary>
        /// 收集所有按钮
        /// </summary>
        private void CollectAllButtons()
        {
            // 将所有按钮添加到列表中（添加button3）
            allButtons = new List<AntdUI.Button>
            {
                button1, setbtn, button4, button2, button3  // 添加button3
                // 如果有更多按钮，在这里添加
            };
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
                            ApplyThemeToAllButtons();
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
                foreach (var button in allButtons)
                {
                    if (button != null)
                    {
                        ApplyColorToButton(button, themeColor);
                    }
                }
            }
            catch
            {
                // 静默失败
            }
        }

        /// <summary>
        /// 应用颜色到单个按钮
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

        /// <summary>
        /// 刷新主题色（当配置改变时调用）
        /// </summary>
        public void RefreshTheme()
        {
            try
            {
                LoadThemeColorConfig();
                if (useThemeColor && themeColor != Color.Empty)
                {
                    ApplyThemeToAllButtons();
                }
                else
                {
                    // 如果关闭主题色，重置按钮到设计器颜色
                    ResetButtonsToDefault();
                }
            }
            catch
            {
                // 静默失败
            }
        }

        /// <summary>
        /// 重置按钮到设计器默认颜色
        /// </summary>
        private void ResetButtonsToDefault()
        {
            try
            {
                foreach (var button in allButtons)
                {
                    if (button != null)
                    {
                        // 重置为默认值
                        button.BackColor = Color.Empty;
                        button.DefaultBack = Color.Empty;
                        button.BackHover = Color.Empty;
                        button.ForeColor = Color.Empty;
                        button.Invalidate();
                    }
                }
            }
            catch
            {
                // 忽略错误
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ShowViedoplayerForm();
        }

        /// <summary>
        /// 显示viedoplayer窗体
        /// </summary>
        private void ShowViedoplayerForm()
        {
            try
            {
                // 检查窗体是否已存在且未关闭
                if (_viedoplayerForm != null)
                {
                    // 如果窗体已释放，清理引用
                    if (_viedoplayerForm.IsDisposed)
                    {
                        _viedoplayerForm = null;
                    }
                    else
                    {
                        // 激活现有窗体
                        ActivateExistingForm();
                        return;
                    }
                }

                // 创建新的viedoplayer窗体
                CreateNewViedoplayerForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开视频播放器：{ex.Message}\n\n错误类型：{ex.GetType().Name}",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 激活已存在的窗体
        /// </summary>
        private void ActivateExistingForm()
        {
            // 如果窗体最小化，恢复它
            if (_viedoplayerForm.WindowState == FormWindowState.Minimized)
            {
                _viedoplayerForm.WindowState = FormWindowState.Normal;
            }

            // 激活并置顶显示
            _viedoplayerForm.BringToFront();
            _viedoplayerForm.Focus();

            // 可选：闪烁窗体标题栏提醒用户
            FlashWindow(_viedoplayerForm.Handle);
        }

        /// <summary>
        /// 创建新的viedoplayer窗体
        /// </summary>
        private void CreateNewViedoplayerForm()
        {
            // 创建窗体实例
            _viedoplayerForm = new viedoplayer();

            // 配置窗体
            ConfigureFormSettings();

            // 订阅事件
            SubscribeToFormEvents();

            // 显示窗体
            _viedoplayerForm.Show();

            // 调试信息
            Console.WriteLine("viedoplayer窗体已打开");
        }

        /// <summary>
        /// 配置窗体设置
        /// </summary>
        private void ConfigureFormSettings()
        {
            // 设置窗体启动位置
            _viedoplayerForm.StartPosition = FormStartPosition.CenterScreen;

            // 设置窗体标题（如果窗体没有设置的话）
            _viedoplayerForm.Text = "视频播放器";

            // 设置图标（如果主窗体有图标）
            var mainForm = this.FindForm();
            if (mainForm != null && mainForm.Icon != null)
            {
                _viedoplayerForm.Icon = mainForm.Icon;
            }

            // 确保窗体显示在任务栏
            _viedoplayerForm.ShowInTaskbar = true;
        }

        /// <summary>
        /// 订阅窗体事件
        /// </summary>
        private void SubscribeToFormEvents()
        {
            _viedoplayerForm.FormClosed += ViedoplayerForm_FormClosed;
            _viedoplayerForm.FormClosing += ViedoplayerForm_FormClosing;
        }

        /// <summary>
        /// viedoplayer窗体关闭事件
        /// </summary>
        private void ViedoplayerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 清理窗体引用
            if (_viedoplayerForm != null)
            {
                // 取消订阅事件
                _viedoplayerForm.FormClosed -= ViedoplayerForm_FormClosed;
                _viedoplayerForm.FormClosing -= ViedoplayerForm_FormClosing;

                // 释放资源
                _viedoplayerForm.Dispose();
                _viedoplayerForm = null;
            }

            Console.WriteLine("viedoplayer窗体已关闭");
        }

        /// <summary>
        /// viedoplayer窗体正在关闭事件
        /// </summary>
        private void ViedoplayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 可以在这里添加关闭前的检查逻辑
            // 例如：询问用户是否确认关闭
            /*
            if (MessageBox.Show("确定要关闭视频播放器吗？", "确认", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            */
        }

        /// <summary>
        /// 父窗体关闭事件
        /// </summary>
        private void ParentForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 关闭viedoplayer窗体
            CloseViedoplayerForm();
        }

        /// <summary>
        /// 关闭viedoplayer窗体
        /// </summary>
        private void CloseViedoplayerForm()
        {
            if (_viedoplayerForm != null && !_viedoplayerForm.IsDisposed)
            {
                // 先取消订阅事件
                _viedoplayerForm.FormClosed -= ViedoplayerForm_FormClosed;
                _viedoplayerForm.FormClosing -= ViedoplayerForm_FormClosing;

                // 关闭窗体
                _viedoplayerForm.Close();
                _viedoplayerForm.Dispose();
                _viedoplayerForm = null;
            }
        }

        /// <summary>
        /// 闪烁窗体（可选功能）
        /// </summary>
        private void FlashWindow(IntPtr handle)
        {
            try
            {
                // 使用Windows API闪烁窗体标题栏
                FlashWindowInfo fwi = new FlashWindowInfo();
                fwi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fwi));
                fwi.hwnd = handle;
                fwi.dwFlags = 0x00000003; // FLASHW_ALL | FLASHW_TIMERNOFG
                fwi.uCount = 3; // 闪烁3次
                fwi.dwTimeout = 0;

                FlashWindowEx(ref fwi);
            }
            catch
            {
                // 忽略闪烁失败的错误
            }
        }

        // Windows API声明（用于窗体闪烁）
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FlashWindowInfo pwfi);

        private void setbtn_Click(object sender, EventArgs e)
        {
            // 触发切换到Set页面的事件
            OnSwitchToSet?.Invoke(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 这里调用版本信息功能
            ShowVersionInformation();
        }

        /// <summary>
        /// 显示版本信息
        /// </summary>
        private void ShowVersionInformation()
        {
            try
            {
                // 创建heartengine实例
                using (var engine = new heartengine())
                {
                    // 获取版本信息
                    string versionInfo = engine.GetVersionInfo();

                    // 使用AntdUI的Message显示（推荐，更美观）
                    //AntdUI.Message.info(versionInfo, "📊 软件版本信息");

                    // 或者使用系统的MessageBox（备选）
                    MessageBox.Show(versionInfo, "版本信息",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // 使用AntdUI显示错误
                //AntdUI.Message.error($"获取版本信息失败:\n{ex.Message}", "❌ 错误");

                // 或者使用系统的MessageBox
                MessageBox.Show($"获取版本信息失败:\n{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 触发切换到About页面的事件
            OnSwitchToAbout?.Invoke(this, EventArgs.Empty);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 触发切换到AI页面的事件
            OnSwitchToAI?.Invoke(this, EventArgs.Empty);
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct FlashWindowInfo
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }
    }
}