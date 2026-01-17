using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace SuperShop_Neko
{
    internal static class Program
    {
        private static bool isApplicationExiting = false;
        private static bool errorDisplayed = false;

        [STAThread]
        static void Main()
        {
#if DEBUG
            // 只需这一行，自动监控所有窗体
            debuger.Instance.Initialize();
#endif

            // 完全接管异常处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetDefaultFont(new Font("Microsoft YaHei UI", 9f));

            // 正常启动主窗体
            Application.Run(new Form1());
        }

        // ==================== 新增的异常处理方法 ====================

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (isApplicationExiting || errorDisplayed) return;
            errorDisplayed = true;
            ShowErrorInNewThread(e.Exception, "UI线程异常");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (isApplicationExiting || errorDisplayed) return;
            errorDisplayed = true;
            ShowErrorInNewThread(e.ExceptionObject as Exception, "非UI线程异常");
        }

        /// <summary>
        /// 在新线程中显示错误对话框（不阻塞主程序）
        /// </summary>
        private static void ShowErrorInNewThread(Exception ex, string context)
        {
            isApplicationExiting = true;

            // 先尝试关闭所有窗体
            try
            {
                foreach (Form form in Application.OpenForms)
                {
                    form.Invoke(new Action(() => form.Close()));
                }
            }
            catch { }

            // 在新线程中显示错误
            Thread errorThread = new Thread(() =>
            {
                ShowErrorDialog(ex, context);
            });

            errorThread.SetApartmentState(ApartmentState.STA);
            errorThread.IsBackground = false;
            errorThread.Start();

            // 等待错误窗体关闭
            errorThread.Join();

            // 退出应用程序
            Environment.Exit(1);
        }

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        private static void ShowErrorDialog(Exception ex, string context)
        {
            try
            {
                // 创建错误窗体
                using (error errorForm = new error())
                {
                    errorForm.DisplayError(ex, context);

                    // 运行错误窗体的消息循环
                    Application.Run(errorForm);
                }
            }
            catch (Exception errorEx)
            {
                // 如果自定义错误窗体失败，使用原生消息框
                try
                {
                    string message = $"应用程序遇到致命错误:\n\n{ex?.Message}\n\n错误类型: {ex?.GetType().Name}";
                    MessageBox.Show(message, "致命错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch
                {
                    // 最后手段：静默退出
                }
            }
        }
    }
}