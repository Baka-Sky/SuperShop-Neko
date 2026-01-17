using System;
using System.Text;
using System.Windows.Forms;

namespace SuperShop_Neko
{
    public partial class error : AntdUI.Window
    {
        public error()
        {
            InitializeComponent();

            // 设置窗体属性确保正确显示
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.ShowInTaskbar = true;
        }

        /// <summary>
        /// 在错误窗体中显示详细的错误信息
        /// </summary>
        /// <param name="exception">发生的异常对象</param>
        /// <param name="context">错误发生的上下文描述</param>
        public void DisplayError(Exception exception, string context)
        {
            // 构建详细的错误信息
            string errorDetails = BuildErrorDetails(exception, context);

            // 假设你的文本框叫 errortext（根据原代码）
            if (errortext != null && !errortext.IsDisposed)
            {
                errortext.Text = errorDetails;

                // 将光标移动到开头，方便用户查看
                errortext.SelectionStart = 0;
                errortext.ScrollToCaret();
            }
        }

        /// <summary>
        /// 构建格式化的错误详细信息字符串
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="context">错误上下文</param>
        /// <returns>格式化的错误信息字符串</returns>
        private string BuildErrorDetails(Exception exception, string context)
        {
            StringBuilder sb = new StringBuilder();

            // ========== 基础信息部分 ==========
            sb.AppendLine($"【发生时间】 {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"【错误上下文】 {context}");
            sb.AppendLine();

            // ========== 错误详情部分 ==========
            sb.AppendLine("【错误详情】");
            sb.AppendLine($"异常类型: {exception.GetType().FullName}");
            sb.AppendLine($"错误消息: {exception.Message}");
            sb.AppendLine();

            // ========== 堆栈跟踪部分 ==========
            sb.AppendLine("【堆栈跟踪】");
            sb.AppendLine(exception.StackTrace ?? "无堆栈信息");
            sb.AppendLine();

            // ========== 内部异常部分 ==========
            if (exception.InnerException != null)
            {
                sb.AppendLine("【内部异常】");
                sb.AppendLine($"类型: {exception.InnerException.GetType().FullName}");
                sb.AppendLine($"消息: {exception.InnerException.Message}");
                sb.AppendLine($"堆栈: {exception.InnerException.StackTrace ?? "无堆栈信息"}");
                sb.AppendLine();
            }

            // ========== 系统信息部分 ==========
            sb.AppendLine("【系统信息】");
            sb.AppendLine($"操作系统: {Environment.OSVersion}");
            sb.AppendLine($"CLR版本: {Environment.Version}");
            sb.AppendLine($"64位系统: {Environment.Is64BitOperatingSystem}");
            sb.AppendLine($"64位进程: {Environment.Is64BitProcess}");
            sb.AppendLine($"工作目录: {Environment.CurrentDirectory}");
            sb.AppendLine($"用户名: {Environment.UserName}");
            sb.AppendLine($"机器名: {Environment.MachineName}");

            return sb.ToString();
        }

        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 复制错误信息按钮点击事件
        /// </summary>
        private void copyButton_Click(object sender, EventArgs e)
        {
            if (errortext != null && !string.IsNullOrEmpty(errortext.Text))
            {
                try
                {
                    Clipboard.SetText(errortext.Text);
                }
                catch
                {
                    // 忽略复制失败
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
   
        }

        private void reboot_Click_1(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void kill_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}