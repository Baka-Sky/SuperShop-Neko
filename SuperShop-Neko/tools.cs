using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Text.Json;

namespace SuperShop_Neko
{
    public partial class tools : UserControl
    {
        // 主题色相关
        private Color themeColor = Color.Empty;
        private bool useThemeColor = false;

        // 所有按钮的列表（用于批量应用主题色）
        private List<AntdUI.Button> allButtons = new List<AntdUI.Button>();

        public tools()
        {
            InitializeComponent();

            // 收集所有按钮到列表
            CollectAllButtons();

            // 加载主题色配置
            LoadThemeColorConfig();
        }

        /// <summary>
        /// 收集所有按钮到列表
        /// </summary>
        private void CollectAllButtons()
        {
            // 将所有按钮添加到列表中
            allButtons = new List<AntdUI.Button>
            {
                button1, button2, button3, button4, button5, button6,
                button7, button8, button9, button10, button11, button12,
                button13, button14, button15, button16, button17, button18
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

        // 下面是你原有的代码保持不变...
        // ===================================================================
        // 通用的异步启动方法（带UAC提权）
        private async Task<bool> LaunchToolAsync(string relativePath, Control? button = null)  // 使用可空类型
        {
            try
            {
                // 禁用按钮防止重复点击
                if (button != null)
                    SetButtonEnabled(button, false);

                // 获取当前程序的运行目录
                string currentDir = AppDomain.CurrentDomain.BaseDirectory;

                // 拼接目标文件路径
                string targetFilePath = Path.Combine(currentDir, relativePath);

                // 检查文件是否存在
                if (!File.Exists(targetFilePath))
                {
                    string errorMsg = $"文件不存在: {targetFilePath}";
                    ShowStatus($"[{Path.GetFileName(relativePath)}] {errorMsg}", true);
                    return false;
                }

                ShowStatus($"正在启动 {Path.GetFileName(relativePath)}...");

                // 创建进程启动信息
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = targetFilePath,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(targetFilePath)
                };

                // 检查是否需要管理员权限（根据文件特征判断）
                if (NeedAdminPermission(relativePath))
                {
                    startInfo.Verb = "runas"; // 请求管理员权限
                }

                // 异步启动进程
                await Task.Run(() =>
                {
                    try
                    {
                        Process? process = Process.Start(startInfo);
                        // 短暂等待确保进程已启动
                        Task.Delay(500).Wait();
                    }
                    catch (Exception ex)
                    {
                        // 如果用户取消了UAC请求，重新以普通权限尝试
                        if (ex.Message.Contains("取消") || ex.Message.Contains("denied") || ex.Message.Contains("cancel"))
                        {
                            startInfo.Verb = ""; // 移除管理员请求
                            Process.Start(startInfo);
                        }
                        else
                        {
                            throw;
                        }
                    }
                });

                ShowStatus($"{Path.GetFileName(relativePath)} 启动成功！");
                return true;
            }
            catch (Exception ex)
            {
                string errorMsg = $"启动失败: {ex.Message}";
                ShowStatus($"[{Path.GetFileName(relativePath)}] {errorMsg}", true);
                return false;
            }
            finally
            {
                // 重新启用按钮
                if (button != null)
                    SetButtonEnabled(button, true);
            }
        }

        // 设置按钮启用状态（兼容AntdUI和WinForms）
        private void SetButtonEnabled(Control button, bool enabled)
        {
            if (button == null) return;  // 添加null检查

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => SetButtonEnabled(button, enabled)));
                return;
            }

            // 使用动态类型检查，避免类型转换错误
            var buttonType = button.GetType();

            // 检查是否是AntdUI.Button
            if (buttonType.FullName == "AntdUI.Button" || buttonType.Name == "Button")
            {
                // 使用反射来设置属性，避免直接类型转换
                var enabledProperty = buttonType.GetProperty("Enabled");
                if (enabledProperty != null)
                {
                    enabledProperty.SetValue(button, enabled);
                }
                else
                {
                    // 如果没有Enabled属性，尝试使用其他可能的属性名
                    var loadingProperty = buttonType.GetProperty("Loading");
                    if (loadingProperty != null)
                    {
                        loadingProperty.SetValue(button, !enabled); // Loading状态与Enabled相反
                    }
                }
            }
            else
            {
                // 如果是标准WinForms控件
                button.Enabled = enabled;
            }
        }

        // 判断工具是否需要管理员权限
        private bool NeedAdminPermission(string toolPath)
        {
            // 根据工具名称判断哪些需要管理员权限
            string toolName = toolPath.ToLower();

            // 通常需要管理员权限的工具
            if (toolName.Contains("aida64") ||
                toolName.Contains("cpuz") ||
                toolName.Contains("rw") ||
                toolName.Contains("throttlestop") ||
                toolName.Contains("diskgenius") ||
                toolName.Contains("hdtune") ||
                toolName.Contains("wprime") ||
                toolName.Contains("diskinfo") ||
                toolName.Contains("diskmark") ||
                toolName.Contains("diskgenius"))
            {
                return true;
            }

            return false;
        }

        // 显示状态信息
        private void ShowStatus(string message, bool isError = false)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowStatus(message, isError)));
                return;
            }

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {(isError ? "❌" : "✅")} {message}");

            // 如果是错误，可以显示MessageBox
            if (isError && MessageBox.Show(message + "\n\n是否重试？", "启动错误",
                MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                // 这里可以添加重试逻辑
            }
        }

        // ============ 所有按钮的点击事件 ============
        // 这里将所有sender参数作为Control传递，避免类型转换错误

        private async void button1_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\AIDA64\aida64.exe", button);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\ASSSDBenchmark\ASSSDBenchmark.exe", button);
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\CoreTemp\Core Temp x64.exe", button);
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\CPUZ\cpuz_x64.exe", button);
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\CrystalDiskInfo\DiskInfo64S.exe", button);
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\CrystalDiskMark\DiskMark64S.exe", button);
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\FurMark_win64\FurMark_GUI.exe", button);
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\HDTune\HDTune.exe", button);
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\iva\iva.exe", button);
        }

        private async void button10_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\Keyboard Test Utility\Keyboard Test Utility.exe", button);
        }

        private async void button11_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\RWEverything\Rw.exe", button);
        }

        private async void button12_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\SSDZ\SSDZ.exe", button);
        }

        private async void button13_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\ThrottleStop\ThrottleStop.exe", button);
        }

        private async void button14_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\wPrime\wPrime.exe", button);
        }

        private async void button15_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\色域检测\monitorinfo.exe", button);
        }

        private async void button16_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\DiskGenius\DiskGenius.exe", button);
        }

        private async void button17_Click(object sender, EventArgs e)
        {
            Control? button = sender as Control;
            if (button != null)
                await LaunchToolAsync(@"tools\XIANGQI\xiangqi.exe", button);
        }

        private async void button18_Click(object sender, EventArgs e)
        {
            // 批量启动所有工具
            Control? button = sender as Control;
            if (button != null)
            {
                SetButtonEnabled(button, false);
                await LaunchAllToolsAsync();
                SetButtonEnabled(button, true);
            }
        }

        // 可选：批量启动所有工具的方法
        public async Task LaunchAllToolsAsync()
        {
            var toolsList = new List<string>
            {
                @"tools\AIDA64\aida64.exe",
                @"tools\ASSSDBenchmark\ASSSDBenchmark.exe",
                @"tools\CoreTemp\Core Temp x64.exe",
                @"tools\CPUZ\cpuz_x64.exe",
                @"tools\CrystalDiskInfo\DiskInfo64S.exe",
                @"tools\CrystalDiskMark\DiskMark64S.exe",
                @"tools\FurMark_win64\FurMark_GUI.exe",
                @"tools\HDTune\HDTune.exe",
                @"tools\iva\iva.exe",
                @"tools\Keyboard Test Utility\Keyboard Test Utility.exe",
                @"tools\RWEverything\Rw.exe",
                @"tools\SSDZ\SSDZ.exe",
                @"tools\ThrottleStop\ThrottleStop.exe",
                @"tools\wPrime\wPrime.exe",
                @"tools\色域检测\monitorinfo.exe",
                @"tools\DiskGenius\DiskGenius.exe",
                @"tools\XIANGQI\xiangqi.exe"
            };

            foreach (var tool in toolsList)
            {
                await LaunchToolAsync(tool);
                await Task.Delay(100); // 短暂延迟，避免同时启动太多进程
            }
        }
    }
}