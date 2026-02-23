using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SuperShop_Neko
{
    /// <summary>
    /// 简单的Toast通知辅助类（使用Windows原生API）
    /// </summary>
    public static class ToastHelper
    {
        // Windows API 常量
        private const int NIM_ADD = 0x00000000;
        private const int NIM_MODIFY = 0x00000001;
        private const int NIM_DELETE = 0x00000002;
        private const int NIF_MESSAGE = 0x00000001;
        private const int NIF_ICON = 0x00000002;
        private const int NIF_TIP = 0x00000004;
        private const int NIF_INFO = 0x00000010;
        private const int WM_USER = 0x0400;
        private const int NOTIFYICON_VERSION = 3;

        // 自定义消息
        private const int WM_NOTIFYICON = WM_USER + 100;
        private const int ID_TRAY = 1000;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NOTIFYICONDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uID;
            public int uFlags;
            public int uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;
            public int dwState;
            public int dwStateMask;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szInfo;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szInfoTitle;
            public int uTimeout;
            public IntPtr hBalloonIcon;
            public Guid guidItem;
            public int uVersion;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern bool Shell_NotifyIcon(int dwMessage, ref NOTIFYICONDATA lpData);

        private static Form _dummyForm;
        private static IntPtr _dummyHWnd;

        /// <summary>
        /// 初始化通知系统（在程序启动时调用）
        /// </summary>
        public static void Initialize()
        {
            try
            {
                // 创建一个隐藏窗口用于接收通知消息
                _dummyForm = new Form
                {
                    ShowInTaskbar = false,
                    WindowState = FormWindowState.Minimized
                };
                _dummyForm.Load += (s, e) => { _dummyForm.Hide(); };
                _dummyForm.Show();
                _dummyHWnd = _dummyForm.Handle;

                Debug.WriteLine("✅ ToastHelper 初始化成功");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ToastHelper 初始化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 显示更新通知
        /// </summary>
        public static void ShowUpdateNotification(string title, string content, string targetVersion)
        {
            try
            {
                if (_dummyHWnd == IntPtr.Zero)
                {
                    Debug.WriteLine("❌ 通知系统未初始化");
                    return;
                }

                NOTIFYICONDATA notifyData = new NOTIFYICONDATA();
                notifyData.cbSize = Marshal.SizeOf(typeof(NOTIFYICONDATA));
                notifyData.hWnd = _dummyHWnd;
                notifyData.uID = ID_TRAY;
                notifyData.uFlags = NIF_INFO | NIF_ICON | NIF_MESSAGE;
                notifyData.uCallbackMessage = WM_NOTIFYICON;

                // 使用程序图标
                notifyData.hIcon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath).Handle;

                // 设置通知内容
                notifyData.szInfoTitle = title;
                notifyData.szInfo = content;
                notifyData.uTimeout = 10000; // 10秒

                // 显示通知
                Shell_NotifyIcon(NIM_ADD, ref notifyData);

                // 短暂延迟后移除，避免图标残留
                System.Threading.Timer timer = null;
                timer = new System.Threading.Timer((state) =>
                {
                    Shell_NotifyIcon(NIM_DELETE, ref notifyData);
                    timer?.Dispose();
                }, null, 15000, System.Threading.Timeout.Infinite);

                Debug.WriteLine($"✅ 通知已显示: {title}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ 显示通知失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public static void Cleanup()
        {
            try
            {
                if (_dummyForm != null && !_dummyForm.IsDisposed)
                {
                    _dummyForm.Close();
                    _dummyForm.Dispose();
                }
            }
            catch { }
        }
    }
}