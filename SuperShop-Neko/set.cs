using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace SuperShop_Neko
{
    public partial class set : UserControl
    {
        private Config config;
        private bool isInitializing = true;

        // 主题色相关
        private Color themeColor = Color.Empty;
        private bool useThemeColor = false;

        // 所有按钮的列表
        private List<AntdUI.Button> allButtons = new List<AntdUI.Button>();

        public set()
        {
            InitializeComponent();

            // 收集所有按钮
            CollectAllButtons();

            // 加载配置并初始化
            LoadConfigSilently();
            isInitializing = false;

            Debug.WriteLine("[set] 控件初始化完成");
        }

        /// <summary>
        /// 收集所有按钮
        /// </summary>
        private void CollectAllButtons()
        {
            Debug.WriteLine("[set] 开始收集按钮");

            // 将所有AntdUI.Button按钮添加到列表中
            allButtons = new List<AntdUI.Button>
            {
                clean
                // 如果有更多AntdUI.Button按钮，在这里添加
                // 例如: button1, button2, ...
            };

            Debug.WriteLine($"[set] 收集到 {allButtons.Count} 个按钮");
        }

        /// <summary>
        /// 加载主题色配置
        /// </summary>
        private void LoadThemeColorConfig()
        {
            try
            {
                Debug.WriteLine("[set] 开始加载主题色配置");

                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                if (!File.Exists(configPath))
                {
                    useThemeColor = false;
                    Debug.WriteLine("[set] 配置文件不存在");
                    return;
                }

                string json = File.ReadAllText(configPath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 读取color配置
                if (root.TryGetProperty("color", out var colorElement))
                {
                    useThemeColor = colorElement.GetBoolean();
                    Debug.WriteLine($"[set] color配置: {useThemeColor}");

                    if (useThemeColor && root.TryGetProperty("RGB", out var rgbElement))
                    {
                        string rgbString = rgbElement.GetString() ?? "0,0,0";
                        Debug.WriteLine($"[set] RGB字符串: {rgbString}");

                        string[] parts = rgbString.Split(',');

                        if (parts.Length == 3 &&
                            int.TryParse(parts[0], out int r) &&
                            int.TryParse(parts[1], out int g) &&
                            int.TryParse(parts[2], out int b))
                        {
                            themeColor = Color.FromArgb(r, g, b);
                            Debug.WriteLine($"[set] 主题色解析成功: R={r}, G={g}, B={b}");
                            ApplyThemeToAllButtons();
                        }
                        else
                        {
                            Debug.WriteLine("[set] RGB格式解析失败");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("[set] 配置文件中未找到color属性");
                }
            }
            catch (Exception ex)
            {
                useThemeColor = false;
                Debug.WriteLine($"[set] 加载主题色配置异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 应用主题色到所有按钮
        /// </summary>
        private void ApplyThemeToAllButtons()
        {
            if (!useThemeColor || themeColor == Color.Empty)
            {
                Debug.WriteLine("[set] 未启用主题色或主题色为空");
                return;
            }

            try
            {
                Debug.WriteLine($"[set] 开始应用主题色到 {allButtons.Count} 个按钮");
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
                            Debug.WriteLine($"[set] 应用主题色到按钮 {button.Name} 失败: {ex.Message}");
                        }
                    }
                }

                Debug.WriteLine($"[set] 主题色应用完成: 成功 {successCount}/{allButtons.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[set] 应用主题色异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 应用颜色到单个按钮
        /// </summary>
        private void ApplyColorToButton(AntdUI.Button button, Color color)
        {
            if (button == null)
            {
                Debug.WriteLine("[set] 按钮为空，无法应用颜色");
                return;
            }

            try
            {
                Debug.WriteLine($"[set] 应用颜色到按钮: {button.Name}");

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

                // 文字颜色保持设计器中的设置，不随背景色改变

                button.Invalidate();
                Debug.WriteLine($"[set] 按钮 {button.Name} 颜色应用完成");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[set] 应用颜色到按钮 {button.Name} 异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 刷新主题色（当配置改变时调用）
        /// </summary>
        public void RefreshTheme()
        {
            try
            {
                Debug.WriteLine("[set] 开始刷新主题");
                LoadThemeColorConfig();

                if (useThemeColor && themeColor != Color.Empty)
                {
                    Debug.WriteLine("[set] 应用新主题色");
                    ApplyThemeToAllButtons();
                }
                else
                {
                    Debug.WriteLine("[set] 重置按钮到默认颜色");
                    ResetButtonsToDefault();
                }

                Debug.WriteLine("[set] 主题刷新完成");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[set] 刷新主题异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 重置按钮到设计器默认颜色
        /// </summary>
        private void ResetButtonsToDefault()
        {
            try
            {
                Debug.WriteLine($"[set] 开始重置 {allButtons.Count} 个按钮的默认颜色");
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
                            Debug.WriteLine($"[set] 重置按钮 {button.Name} 失败: {ex.Message}");
                        }
                    }
                }

                Debug.WriteLine($"[set] 按钮重置完成: 成功 {successCount}/{allButtons.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[set] 重置按钮异常: {ex.Message}");
            }
        }

        // 加载配置
        private void LoadConfigSilently()
        {
            try
            {
                // 加载配置
                config = ConfigManager.LoadConfig();

                // 设置Switch的初始状态
                color.Checked = config.color;

                // 加载主题色配置
                LoadThemeColorConfig();
            }
            catch
            {
                // 静默失败
            }
        }

        /// <summary>
        /// 只更新颜色相关的配置，不覆盖其他字段
        /// </summary>
        private void UpdateColorConfigOnly(bool colorEnabled, Color newColor)
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                // 读取现有配置
                Dictionary<string, object> configDict;
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    configDict = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                        ?? new Dictionary<string, object>();
                }
                else
                {
                    configDict = new Dictionary<string, object>();
                }

                // 更新颜色相关字段
                configDict["color"] = colorEnabled;
                configDict["RGB"] = $"{newColor.R},{newColor.G},{newColor.B}";

                // 保留所有其他字段不变
                // Version, name, password, user_id, uploader_name, su 等字段都保持不变

                // 写回文件
                string newJson = JsonSerializer.Serialize(configDict, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(configPath, newJson);

                Debug.WriteLine($"[set] 颜色配置已更新: color={colorEnabled}, RGB={newColor.R},{newColor.G},{newColor.B}");

                // 重新加载配置
                config = ConfigManager.LoadConfig();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[set] 更新颜色配置失败: {ex.Message}");
            }
        }

        private void color_CheckedChanged(object sender, AntdUI.BoolEventArgs e)
        {
            // 如果是初始化阶段，不处理事件
            if (isInitializing) return;

            try
            {
                if (e.Value) // 如果开关打开
                {
                    // 获取Windows主题色
                    Color newColor = WindowsThemeColorHelper.GetWindowsThemeColor();

                    // 只更新颜色相关配置
                    UpdateColorConfigOnly(true, newColor);

                    // 强制立即应用主题色
                    ForceApplyThemeColorImmediately();
                }
                else
                {
                    // 关闭时设置为黑色
                    UpdateColorConfigOnly(false, Color.Black);

                    // 强制立即应用主题色
                    ForceApplyThemeColorImmediately();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"切换主题色失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 强制立即应用主题色
        /// </summary>
        private void ForceApplyThemeColorImmediately()
        {
            try
            {
                // 重新加载配置
                config = ConfigManager.LoadConfig();

                // 通知主窗体刷新主题色
                NotifyMainFormToRefreshTheme();

                // 显示当前颜色（用于调试）
                Console.WriteLine($"当前主题色: {config.RGB}");

                // 同时刷新本控件的按钮颜色
                RefreshTheme();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"强制应用主题色失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 通知主窗体刷新主题色
        /// </summary>
        private void NotifyMainFormToRefreshTheme()
        {
            try
            {
                // 查找Form1
                Form1 mainForm = FindMainForm();
                if (mainForm != null)
                {
                    // 调用Form1的刷新方法
                    mainForm.RefreshTheme();

                    // 刷新所有子控件的主题
                    RefreshChildControlsTheme(mainForm);
                }
            }
            catch
            {
                // 静默失败
            }
        }

        /// <summary>
        /// 刷新所有子控件的主题
        /// </summary>
        private void RefreshChildControlsTheme(Form1 form)
        {
            try
            {
                // 从顶层窗体开始递归刷新
                FindAndRefreshChildControls(form);
            }
            catch
            {
                // 静默失败
            }
        }

        /// <summary>
        /// 刷新单个控件的主题（如果支持）
        /// </summary>
        private void RefreshControlThemeIfSupported(Control control)
        {
            try
            {
                // 检查是否是 app 控件
                if (control is app appControl)
                {
                    appControl.RefreshTheme();
                    return;
                }

                // 检查是否是 tools 控件
                if (control is tools toolsControl)
                {
                    toolsControl.RefreshTheme();
                    return;
                }

                // 检查是否是 more 控件
                if (control is more moreControl)
                {
                    moreControl.RefreshTheme();
                    return;
                }

                // 检查是否是 set 控件
                if (control is set setControl)
                {
                    setControl.RefreshTheme();
                    return;
                }

                // 可以在这里添加其他需要主题色的控件类型
            }
            catch
            {
                // 静默失败
            }
        }

        /// <summary>
        /// 递归查找并刷新子控件
        /// </summary>
        private void FindAndRefreshChildControls(Control parent)
        {
            try
            {
                // 先处理当前控件
                RefreshControlThemeIfSupported(parent);

                // 然后递归处理所有子控件
                if (parent.HasChildren)
                {
                    foreach (Control control in parent.Controls)
                    {
                        FindAndRefreshChildControls(control);
                    }
                }
            }
            catch
            {
                // 静默失败
            }
        }

        // 查找Form1实例
        private Form1 FindMainForm()
        {
            Control parent = this.Parent;
            while (parent != null && !(parent is Form1))
            {
                parent = parent.Parent;
            }
            return parent as Form1;
        }

        // 公开方法，供其他代码调用刷新
        public void ManualRefresh()
        {
            LoadConfigSilently();
            NotifyMainFormToRefreshTheme();
        }

        private void clean_Click(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers(); // 等待所有终结器执行完毕
        }
    }
}