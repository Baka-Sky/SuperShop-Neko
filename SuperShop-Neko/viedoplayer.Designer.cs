namespace SuperShop_Neko
{
    partial class viedoplayer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            page = new AntdUI.PageHeader();
            label1 = new AntdUI.Label();
            button1 = new AntdUI.Button();
            load = new AntdUI.Button();
            webview = new Microsoft.Web.WebView2.WinForms.WebView2();
            page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webview).BeginInit();
            SuspendLayout();
            // 
            // page
            // 
            page.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            page.BackColor = Color.FromArgb(255, 224, 192);
            page.Controls.Add(label1);
            page.Controls.Add(button1);
            page.Controls.Add(load);
            page.Location = new Point(1, 0);
            page.MaximizeBox = false;
            page.Name = "page";
            page.ShowButton = true;
            page.ShowIcon = true;
            page.Size = new Size(1057, 38);
            page.TabIndex = 1;
            page.Text = "SuperShop--ViedoPlayer";
            // 
            // label1
            // 
            label1.Location = new Point(504, 5);
            label1.Name = "label1";
            label1.Size = new Size(327, 29);
            label1.TabIndex = 2;
            label1.Text = "";
            // 
            // button1
            // 
            button1.Location = new Point(354, 2);
            button1.Name = "button1";
            button1.Size = new Size(144, 35);
            button1.TabIndex = 1;
            button1.Text = "加载至Webview";
            button1.Click += button1_Click;
            // 
            // load
            // 
            load.Location = new Point(225, 2);
            load.Name = "load";
            load.Size = new Size(123, 35);
            load.TabIndex = 0;
            load.Text = "载入视频/音频";
            load.Click += load_Click;
            // 
            // webview
            // 
            webview.AllowExternalDrop = true;
            webview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webview.CreationProperties = null;
            webview.DefaultBackgroundColor = Color.White;
            webview.Location = new Point(1, 39);
            webview.Name = "webview";
            webview.Size = new Size(1057, 503);
            webview.TabIndex = 2;
            webview.ZoomFactor = 1D;
            // 
            // viedoplayer
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            BackColor = Color.White;
            ClientSize = new Size(1059, 544);
            Controls.Add(webview);
            Controls.Add(page);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "viedoplayer";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "viedoplayer";
            page.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webview).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.PageHeader page;
        private AntdUI.Button button1;
        private AntdUI.Button load;
        private Microsoft.Web.WebView2.WinForms.WebView2 webview;
        private AntdUI.Label label1;
    }
}