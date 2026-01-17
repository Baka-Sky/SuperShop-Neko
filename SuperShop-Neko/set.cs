using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;

namespace SuperShop_Neko
{
    public partial class set : UserControl
    {
        private Config config;
        private bool isInitializing = true;

        public set()
        {
            InitializeComponent();

            // 加载配置并初始化
            LoadConfigSilently();
            isInitializing = false;
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

                // 如果是true，静默获取主题色
                if (config.color)
                {
                    SilentUpdateThemeColor();
                }
            }
            catch
            {
                // 静默失败
            }
        }

        // 静默更新主题色
        private void SilentUpdateThemeColor()
        {
            try
            {
                // 从注册表获取Windows主题色
                Color themeColor = WindowsThemeColorHelper.GetWindowsThemeColor();

                // 更新配置
                config.ThemeColor = themeColor;

                // 保存配置
                ConfigManager.SaveConfig(config);
            }
            catch
            {
                // 静默失败
            }
        }

        private void color_CheckedChanged(object sender, AntdUI.BoolEventArgs e)
        {
            // 如果是初始化阶段，不处理事件
            if (isInitializing) return;

            try
            {
                // 更新配置
                config.color = e.Value;

                if (e.Value) // 如果开关打开
                {
                    // 获取主题色
                    SilentUpdateThemeColor();
                }
                else
                {
                    // 关闭时重置为黑色
                    config.RGB = "0,0,0";
                }

                // 保存配置
                ConfigManager.SaveConfig(config);

                // 立即重新加载配置，确保获取到最新的RGB值
                config = ConfigManager.LoadConfig();

                // 强制立即应用主题色到所有按钮
                ForceApplyThemeColorImmediately();
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