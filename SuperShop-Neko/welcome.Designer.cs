namespace SuperShop_Neko
{
    partial class welcome
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new AntdUI.Panel();
            webview = new Microsoft.Web.WebView2.WinForms.WebView2();
            versiontext = new AntdUI.Input();
            weatheroutput = new AntdUI.Input();
            weatherinput = new AntdUI.Input();
            weathergo = new AntdUI.Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webview).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(webview);
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1097, 238);
            panel1.TabIndex = 0;
            panel1.Text = "panel1";
            // 
            // webview
            // 
            webview.AllowExternalDrop = true;
            webview.BackColor = SystemColors.Control;
            webview.CreationProperties = null;
            webview.DefaultBackgroundColor = Color.White;
            webview.Location = new Point(3, 3);
            webview.Name = "webview";
            webview.Size = new Size(1091, 232);
            webview.TabIndex = 0;
            webview.ZoomFactor = 1D;
            // 
            // versiontext
            // 
            versiontext.AutoScroll = true;
            versiontext.Location = new Point(-4, 245);
            versiontext.Multiline = true;
            versiontext.Name = "versiontext";
            versiontext.ReadOnly = true;
            versiontext.Size = new Size(532, 205);
            versiontext.TabIndex = 1;
            versiontext.Text = "input1";
            // 
            // weatheroutput
            // 
            weatheroutput.AutoScroll = true;
            weatheroutput.Location = new Point(534, 247);
            weatheroutput.Multiline = true;
            weatheroutput.Name = "weatheroutput";
            weatheroutput.ReadOnly = true;
            weatheroutput.Size = new Size(571, 154);
            weatheroutput.TabIndex = 2;
            // 
            // weatherinput
            // 
            weatherinput.Location = new Point(619, 399);
            weatherinput.Name = "weatherinput";
            weatherinput.Size = new Size(486, 50);
            weatherinput.TabIndex = 3;
            weatherinput.Text = "请输入天气";
            // 
            // weathergo
            // 
            weathergo.Location = new Point(535, 401);
            weathergo.Name = "weathergo";
            weathergo.Size = new Size(79, 46);
            weathergo.TabIndex = 4;
            weathergo.Text = "GO!";
            weathergo.Click += weathergo_Click;
            // 
            // welcome
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(weathergo);
            Controls.Add(weatherinput);
            Controls.Add(weatheroutput);
            Controls.Add(versiontext);
            Controls.Add(panel1);
            Font = new Font("MiSans Semibold", 9F, FontStyle.Bold);
            Name = "welcome";
            Size = new Size(1103, 450);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webview).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel panel1;
        private AntdUI.Input versiontext;
        private AntdUI.Input weatheroutput;
        private AntdUI.Input weatherinput;
        private AntdUI.Button weathergo;
        private Microsoft.Web.WebView2.WinForms.WebView2 webview;
        //private CefSharp.WinForms.ChromiumWebBrowser chromiumWebBrowser1;
    }
}