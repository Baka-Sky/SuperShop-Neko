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
using System.Diagnostics; // 添加Diagnostics用于调试

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

        // 防重复点击相关
        private bool _isShowingVersionInfo = false;
        private DateTime _lastVersionInfoTime = DateTime.MinValue;
        private readonly TimeSpan _clickInterval = TimeSpan.FromMilliseconds(1000); // 1秒防抖

        public more()
        {
            InitializeComponent();

            // 收集所有按钮
            CollectAllButtons();

            // 加载主题色配置
            LoadThemeColorConfig();

            // 设置点击事件（确保只绑定一次）
            SetupClickEvents();

            // 订阅父窗体关闭事件
            var parentForm = this.FindForm();
            if (parentForm != null)
            {
                parentForm.FormClosed += ParentForm_FormClosed;
            }

            Debug.WriteLine("[more] 控件初始化完成");
        }

        /// <summary>
        /// 设置点击事件（确保只绑定一次）
        /// </summary>
        private void SetupClickEvents()
        {
            Debug.WriteLine("[more] 开始绑定按钮点击事件");

            // 移除可能已存在的事件处理程序
            button4.Click -= button4_Click;
            setbtn.Click -= setbtn_Click;
            button1.Click -= button1_Click;
            button2.Click -= button2_Click;
            button3.Click -= button3_Click;

            // 重新绑定事件
            button4.Click += button4_Click;
            setbtn.Click += setbtn_Click;
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;

            Debug.WriteLine($"[more] 按钮事件绑定完成: {button1.Name}, {button2.Name}, {button3.Name}, {setbtn.Name}, {button4.Name}");
        }

        /// <summary>
        /// 收集所有按钮
        /// </summary>
        private void CollectAllButtons()
        {
            Debug.WriteLine("[more] 开始收集按钮");

            // 将所有按钮添加到列表中（添加button3）
            allButtons = new List<AntdUI.Button>
            {
                button1, setbtn, button4, button2, button3  // 添加button3
                // 如果有更多按钮，在这里添加
            };

            Debug.WriteLine($"[more] 收集到 {allButtons.Count} 个按钮");
        }

        /// <summary>
        /// 加载主题色配置
        /// </summary>
        private void LoadThemeColorConfig()
        {
            try
            {
                Debug.WriteLine("[more] 开始加载主题色配置");

                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                if (!File.Exists(configPath))
                {
                    useThemeColor = false;
                    Debug.WriteLine("[more] 配置文件不存在");
                    return;
                }

                string json = File.ReadAllText(configPath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 读取color配置
                if (root.TryGetProperty("color", out var colorElement))
                {
                    useThemeColor = colorElement.GetBoolean();
                    Debug.WriteLine($"[more] color配置: {useThemeColor}");

                    if (useThemeColor && root.TryGetProperty("RGB", out var rgbElement))
                    {
                        string rgbString = rgbElement.GetString() ?? "0,0,0";
                        Debug.WriteLine($"[more] RGB字符串: {rgbString}");

                        string[] parts = rgbString.Split(',');

                        if (parts.Length == 3 &&
                            int.TryParse(parts[0], out int r) &&
                            int.TryParse(parts[1], out int g) &&
                            int.TryParse(parts[2], out int b))
                        {
                            themeColor = Color.FromArgb(r, g, b);
                            Debug.WriteLine($"[more] 主题色解析成功: R={r}, G={g}, B={b}");
                            ApplyThemeToAllButtons();
                        }
                        else
                        {
                            Debug.WriteLine("[more] RGB格式解析失败");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("[more] 配置文件中未找到color属性");
                }
            }
            catch (Exception ex)
            {
                useThemeColor = false;
                Debug.WriteLine($"[more] 加载主题色配置异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 应用主题色到所有按钮
        /// </summary>
        private void ApplyThemeToAllButtons()
        {
            if (!useThemeColor || themeColor == Color.Empty)
            {
                Debug.WriteLine("[more] 未启用主题色或主题色为空");
                return;
            }

            try
            {
                Debug.WriteLine($"[more] 开始应用主题色到 {allButtons.Count} 个按钮");
                int successCount = 0;

                foreach (var button in allButtons)
                {
                    if (button != null)
                    {
                        try
                        {
                            ApplyColorToButton(button, themeColor);
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[more] 应用主题色到按钮 {button.Name} 失败: {ex.Message}");
                        }
                    }
                }

                Debug.WriteLine($"[more] 主题色应用完成: 成功 {successCount}/{allButtons.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[more] 应用主题色异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 应用颜色到单个按钮
        /// </summary>
        private void ApplyColorToButton(AntdUI.Button button, Color color)
        {
            if (button == null)
            {
                Debug.WriteLine("[more] 按钮为空，无法应用颜色");
                return;
            }

            try
            {
                Debug.WriteLine($"[more] 应用颜色到按钮: {button.Name}");

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

                // 注意：这里删除了文字颜色改变的逻辑
                // 文字颜色保持设计器中的设置，不随背景色改变

                button.Invalidate();
                Debug.WriteLine($"[more] 按钮 {button.Name} 颜色应用完成");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[more] 应用颜色到按钮 {button.Name} 异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 刷新主题色（当配置改变时调用）
        /// </summary>
        public void RefreshTheme()
        {
            try
            {
                Debug.WriteLine("[more] 开始刷新主题");
                LoadThemeColorConfig();

                if (useThemeColor && themeColor != Color.Empty)
                {
                    Debug.WriteLine("[more] 应用新主题色");
                    ApplyThemeToAllButtons();
                }
                else
                {
                    Debug.WriteLine("[more] 重置按钮到默认颜色");
                    ResetButtonsToDefault();
                }

                Debug.WriteLine("[more] 主题刷新完成");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[more] 刷新主题异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 重置按钮到设计器默认颜色
        /// </summary>
        private void ResetButtonsToDefault()
        {
            try
            {
                Debug.WriteLine($"[more] 开始重置 {allButtons.Count} 个按钮的默认颜色");
                int successCount = 0;

                foreach (var button in allButtons)
                {
                    if (button != null)
                    {
                        try
                        {
                            // 重置为默认值
                            button.BackColor = Color.Empty;
                            button.DefaultBack = Color.Empty;
                            button.BackHover = Color.Empty;
                            // 注意：ForeColor不重置，保持原来的文字颜色
                            button.Invalidate();
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[more] 重置按钮 {button.Name} 失败: {ex.Message}");
                        }
                    }
                }

                Debug.WriteLine($"[more] 按钮重置完成: 成功 {successCount}/{allButtons.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[more] 重置按钮异常: {ex.Message}");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"[more] button4点击事件触发: {DateTime.Now:HH:mm:ss.fff}");
            ShowViedoplayerForm();
        }

        /// <summary>
        /// 显示viedoplayer窗体
        /// </summary>
        private void ShowViedoplayerForm()
        {
            Debug.WriteLine($"[more] 开始显示viedoplayer窗体: {DateTime.Now:HH:mm:ss.fff}");

            try
            {
                // 检查窗体是否已存在且未关闭
                if (_viedoplayerForm != null)
                {
                    // 如果窗体已释放，清理引用
                    if (_viedoplayerForm.IsDisposed)
                    {
                        _viedoplayerForm = null;
                        Debug.WriteLine("[more] 清理已释放的窗体引用");
                    }
                    else
                    {
                        // 激活现有窗体
                        Debug.WriteLine("[more] 激活已存在的窗体");
                        ActivateExistingForm();
                        return;
                    }
                }

                // 创建新的viedoplayer窗体
                Debug.WriteLine("[more] 创建新窗体");
                CreateNewViedoplayerForm();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[more] 打开视频播放器异常: {ex.Message}");
                MessageBox.Show($"无法打开视频播放器：{ex.Message}\n\n错误类型：{ex.GetType().Name}",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 激活已存在的窗体
        /// </summary>
        private void ActivateExistingForm()
        {
            Debug.WriteLine("[more] 激活现有窗体");

            // 如果窗体最小化，恢复它
            if (_viedoplayerForm.WindowState == FormWindowState.Minimized)
            {
                _viedoplayerForm.WindowState = FormWindowState.Normal;
                Debug.WriteLine("[more] 窗体从最小化恢复");
            }

            // 激活并置顶显示
            _viedoplayerForm.BringToFront();
            _viedoplayerForm.Focus();
            Debug.WriteLine("[more] 窗体已激活并置顶");

            // 可选：闪烁窗体标题栏提醒用户
            try
            {
                FlashWindow(_viedoplayerForm.Handle);
                Debug.WriteLine("[more] 窗体闪烁提醒");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[more] 窗体闪烁失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建新的viedoplayer窗体
        /// </summary>
        private void CreateNewViedoplayerForm()
        {
            Debug.WriteLine("[more] 创建新窗体实例");

            // 创建窗体实例
            _viedoplayerForm = new viedoplayer();

            // 配置窗体
            ConfigureFormSettings();

            // 订阅事件
            SubscribeToFormEvents();

            // 显示窗体
            _viedoplayerForm.Show();
            Debug.WriteLine("[more] 窗体已显示");

            // 调试信息
            Console.WriteLine("viedoplayer窗体已打开");
        }

        /// <summary>
        /// 配置窗体设置
        /// </summary>
        private void ConfigureFormSettings()
        {
            Debug.WriteLine("[more] 配置窗体设置");

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
            Debug.WriteLine("[more] 窗体配置完成");
        }

        /// <summary>
        /// 订阅窗体事件
        /// </summary>
        private void SubscribeToFormEvents()
        {
            Debug.WriteLine("[more] 订阅窗体事件");

            _viedoplayerForm.FormClosed += ViedoplayerForm_FormClosed;
            _viedoplayerForm.FormClosing += ViedoplayerForm_FormClosing;
        }

        /// <summary>
        /// viedoplayer窗体关闭事件
        /// </summary>
        private void ViedoplayerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Debug.WriteLine("[more] viedoplayer窗体关闭事件");

            // 清理窗体引用
            if (_viedoplayerForm != null)
            {
                // 取消订阅事件
                _viedoplayerForm.FormClosed -= ViedoplayerForm_FormClosed;
                _viedoplayerForm.FormClosing -= ViedoplayerForm_FormClosing;

                // 释放资源
                _viedoplayerForm.Dispose();
                _viedoplayerForm = null;
                Debug.WriteLine("[more] 窗体引用已清理");
            }

            Console.WriteLine("viedoplayer窗体已关闭");
        }

        /// <summary>
        /// viedoplayer窗体正在关闭事件
        /// </summary>
        private void ViedoplayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("[more] viedoplayer窗体正在关闭");
        }

        /// <summary>
        /// 父窗体关闭事件
        /// </summary>
        private void ParentForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Debug.WriteLine("[more] 父窗体关闭事件");

            // 关闭viedoplayer窗体
            CloseViedoplayerForm();
        }

        /// <summary>
        /// 关闭viedoplayer窗体
        /// </summary>
        private void CloseViedoplayerForm()
        {
            Debug.WriteLine("[more] 关闭viedoplayer窗体");

            if (_viedoplayerForm != null && !_viedoplayerForm.IsDisposed)
            {
                // 先取消订阅事件
                _viedoplayerForm.FormClosed -= ViedoplayerForm_FormClosed;
                _viedoplayerForm.FormClosing -= ViedoplayerForm_FormClosing;

                // 关闭窗体
                _viedoplayerForm.Close();
                _viedoplayerForm.Dispose();
                _viedoplayerForm = null;
                Debug.WriteLine("[more] 窗体已关闭并清理");
            }
            else
            {
                Debug.WriteLine("[more] 窗体已为空或已释放");
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
            catch (Exception ex)
            {
                Debug.WriteLine($"[more] 闪烁窗体失败: {ex.Message}");
            }
        }

        private void setbtn_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"[more] setbtn点击事件触发: {DateTime.Now:HH:mm:ss.fff}");

            // 触发切换到Set页面的事件
            OnSwitchToSet?.Invoke(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"[more] button1点击事件触发: {DateTime.Now:HH:mm:ss.fff}");

            // 检查是否正在显示版本信息
            if (_isShowingVersionInfo)
            {
                Debug.WriteLine($"[more] 版本信息正在显示中，忽略本次点击");
                return;
            }

            // 检查时间间隔，防止快速重复点击
            TimeSpan timeSinceLastClick = DateTime.Now - _lastVersionInfoTime;
            if (timeSinceLastClick < _clickInterval)
            {
                Debug.WriteLine($"[more] 点击过于频繁: {timeSinceLastClick.TotalMilliseconds:F0}ms < {_clickInterval.TotalMilliseconds:F0}ms，忽略本次点击");
                return;
            }

            // 这里调用版本信息功能
            ShowVersionInformation();
        }

        /// <summary>
        /// 显示版本信息（带防重复处理）
        /// </summary>
        private void ShowVersionInformation()
        {
            try
            {
                // 设置标志，防止重复进入
                _isShowingVersionInfo = true;
                _lastVersionInfoTime = DateTime.Now;

                Debug.WriteLine($"[more] 开始显示版本信息: {DateTime.Now:HH:mm:ss.fff}");

                // 创建heartengine实例
                using (var engine = new heartengine())
                {
                    // 获取版本信息
                    string versionInfo = engine.GetVersionInfo();
                    Debug.WriteLine($"[more] 获取到版本信息，长度: {versionInfo.Length}");

                    // 使用AntdUI的Message显示（推荐，更美观）
                    //AntdUI.Message.info(versionInfo, "📊 软件版本信息");

                    // 或者使用系统的MessageBox（备选）
                    // 注意：这里只显示一次MessageBox
                    Debug.WriteLine($"[more] 即将显示MessageBox");

                    // 使用BeginInvoke确保在UI线程显示，避免阻塞
                    this.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            MessageBox.Show(versionInfo, "版本信息",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            Debug.WriteLine($"[more] MessageBox已显示");
                        }
                        finally
                        {
                            // 重置标志
                            _isShowingVersionInfo = false;
                            Debug.WriteLine($"[more] 版本信息显示完成，标志已重置");
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[more] 获取版本信息异常: {ex.Message}");

                // 使用AntdUI显示错误
                //AntdUI.Message.error($"获取版本信息失败:\n{ex.Message}", "❌ 错误");

                // 或者使用系统的MessageBox
                // 同样使用BeginInvoke确保线程安全
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        MessageBox.Show($"获取版本信息失败:\n{ex.Message}", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        _isShowingVersionInfo = false;
                        Debug.WriteLine($"[more] 错误信息显示完成，标志已重置");
                    }
                }));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"[more] button2点击事件触发: {DateTime.Now:HH:mm:ss.fff}");

            // 触发切换到About页面的事件
            OnSwitchToAbout?.Invoke(this, EventArgs.Empty);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"[more] button3点击事件触发: {DateTime.Now:HH:mm:ss.fff}");

            // 触发切换到AI页面的事件
            OnSwitchToAI?.Invoke(this, EventArgs.Empty);
        }

        // Windows API声明（用于窗体闪烁）
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FlashWindowInfo pwfi);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct FlashWindowInfo
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        /// <summary>
        /// 重置版本信息显示状态（供外部调用，如遇到问题）
        /// </summary>
        public void ResetVersionInfoState()
        {
            _isShowingVersionInfo = false;
            Debug.WriteLine($"[more] 版本信息状态已重置");
        }
    }
}