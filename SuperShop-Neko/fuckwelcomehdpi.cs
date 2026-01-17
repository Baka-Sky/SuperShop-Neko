using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace SuperShop_Neko
{
    public static class FuckWelcomeHDPI
    {
        // 存储控件状态
        private static Dictionary<Control, ControlState> _controlStates = new Dictionary<Control, ControlState>();

        // DPI检测
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        private const int LOGPIXELSX = 88;

        /// <summary>
        /// 只修复welcome控件的DPI问题（使用你的那套布局逻辑）
        /// </summary>
        public static void FixWelcomeOnly(Control container, welcome welcomeControl, bool isInitialLoad = false)
        {
            if (container == null || welcomeControl == null) return;

            try
            {
                // 获取DPI比例
                float dpiScale = GetDpiScale();
                bool isHighDPI = dpiScale > 1.1f;

                DebugLog($"修复welcome - DPI: {dpiScale}, 高DPI: {isHighDPI}, 初始加载: {isInitialLoad}");

                // 只在需要时执行完整修复流程
                if (isHighDPI || isInitialLoad)
                {
                    ExecuteWithLayoutProtection(container, () =>
                    {
                        PrepareControlForDisplay(welcomeControl, container);
                        ForceLayoutUpdate(container);
                    });

                    // 如果是初始加载，延迟刷新
                    if (isInitialLoad)
                    {
                        container.BeginInvoke(new Action(() =>
                        {
                            ForceLayoutUpdate(container);
                        }), 100);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog($"修复welcome错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 为控件显示做准备（你的PrepareControlForDisplay逻辑）
        /// </summary>
        private static void PrepareControlForDisplay(UserControl control, Control container)
        {
            if (control == null || container == null) return;

            DebugLog($"准备显示控件: {control.Name}");

            // 先隐藏，设置完成后再显示
            control.Visible = false;

            // 使用 Dock 填充
            control.Dock = DockStyle.Fill;

            // 确保父容器大小正确
            control.Parent = container;

            // 强制设置大小
            control.Size = container.ClientSize;
            control.Location = new Point(0, 0);

            // 启用双缓冲减少闪烁
            SetDoubleBuffered(control);

            // 禁用控件的自动缩放
            if (control is welcome)
            {
                control.AutoScaleMode = AutoScaleMode.None;
                DisableAutoScaling(control);
            }

            // 存储原始状态
            if (!_controlStates.ContainsKey(control))
            {
                _controlStates[control] = new ControlState
                {
                    OriginalSize = control.Size,
                    OriginalLocation = control.Location,
                    Parent = container
                };
            }

            // 现在显示
            control.Visible = true;

            DebugLog($"控件准备完成: {control.Name}, 大小: {control.Size}");
        }

        /// <summary>
        /// 在布局保护下执行操作（你的ExecuteWithLayoutProtection逻辑）
        /// </summary>
        private static void ExecuteWithLayoutProtection(Control container, Action action)
        {
            if (container == null) return;

            bool layoutSuspended = false;

            try
            {
                // 暂停布局
                container.SuspendLayout();
                layoutSuspended = true;

                // 执行操作
                action?.Invoke();
            }
            finally
            {
                if (layoutSuspended)
                {
                    // 恢复布局，并强制立即重新布局
                    container.ResumeLayout(false); // false = 不立即布局

                    // 手动触发布局
                    container.BeginInvoke(new Action(() =>
                    {
                        container.PerformLayout();
                        ForceLayoutUpdate(container);
                    }));
                }
            }
        }

        /// <summary>
        /// 强制更新布局（你的ForceLayoutUpdate逻辑）
        /// </summary>
        private static void ForceLayoutUpdate(Control container)
        {
            if (container == null) return;

            try
            {
                DebugLog($"强制更新布局: {container.Name}");

                // 强制刷新布局链
                container.Invalidate();
                container.Update();

                if (container.Controls.Count > 0)
                {
                    Control currentControl = container.Controls[0];
                    currentControl.Invalidate();
                    currentControl.Update();
                    currentControl.Refresh();
                }
            }
            catch (Exception ex)
            {
                DebugLog($"强制更新布局错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 调整控件大小（你的AdjustControlSize逻辑）
        /// </summary>
        public static void AdjustControlSize(Control container)
        {
            if (container == null || container.Controls.Count == 0) return;

            Control currentControl = container.Controls[0];

            // 确保控件大小与容器匹配
            if (currentControl.Size != container.ClientSize)
            {
                currentControl.Size = container.ClientSize;
                currentControl.Location = new Point(0, 0);
                DebugLog($"调整控件大小: {currentControl.Name} -> {container.ClientSize}");
            }
        }

        /// <summary>
        /// 刷新控件布局（你的RefreshControlLayout逻辑）
        /// </summary>
        public static void RefreshControlLayout(Control container)
        {
            if (container == null || container.Controls.Count == 0) return;

            DebugLog($"刷新控件布局: {container.Name}");

            // 重新应用布局
            Control currentControl = container.Controls[0];

            ExecuteWithLayoutProtection(container, () =>
            {
                // 临时移除并重新添加（触发布局重新计算）
                container.Controls.Remove(currentControl);
                container.Controls.Add(currentControl);

                // 确保Dock设置有效
                currentControl.Dock = DockStyle.Fill;
            });
        }

        /// <summary>
        /// 禁用自动缩放（你的DisableAutoScaling逻辑）
        /// </summary>
        private static void DisableAutoScaling(Control control)
        {
            if (control == null) return;

            // 禁用控件的自动缩放
            if (control is UserControl userControl)
            {
                userControl.AutoScaleMode = AutoScaleMode.None;
            }

            // 递归处理所有子控件
            foreach (Control child in control.Controls)
            {
                DisableAutoScaling(child);
            }
        }

        /// <summary>
        /// 设置双缓冲（你的SetDoubleBuffered逻辑）
        /// </summary>
        private static void SetDoubleBuffered(Control control)
        {
            try
            {
                typeof(Control).GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)
                    ?.SetValue(control, true, null);
            }
            catch
            {
                // 忽略错误
            }
        }

        /// <summary>
        /// 获取系统DPI比例
        /// </summary>
        private static float GetDpiScale()
        {
            try
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                if (hdc != IntPtr.Zero)
                {
                    try
                    {
                        int dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
                        return dpiX / 96f;
                    }
                    finally
                    {
                        ReleaseDC(IntPtr.Zero, hdc);
                    }
                }
            }
            catch { }
            return 1.0f;
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        private static void DebugLog(string message)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[FuckWelcomeHDPI] {DateTime.Now:HH:mm:ss.fff}: {message}");
#endif
        }

        /// <summary>
        /// 初始化控件（你的InitializeControls逻辑的简化版）
        /// </summary>
        public static void InitializeWelcomeControl(out welcome welcomeControl)
        {
            welcomeControl = new welcome();
            welcomeControl.AutoScaleMode = AutoScaleMode.None;
            welcomeControl.Visible = false;
            DisableAutoScaling(welcomeControl);
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public static void Cleanup()
        {
            foreach (var state in _controlStates.Values)
            {
                if (state.Control != null)
                {
                    state.Control.Dispose();
                }
            }
            _controlStates.Clear();
        }

        /// <summary>
        /// 控件状态类
        /// </summary>
        private class ControlState
        {
            public Control Control { get; set; }
            public Size OriginalSize { get; set; }
            public Point OriginalLocation { get; set; }
            public Control Parent { get; set; }
        }
    }
}