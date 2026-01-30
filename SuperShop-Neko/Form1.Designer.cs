namespace SuperShop_Neko
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            page = new AntdUI.PageHeader();
            toast = new AntdUI.Button();
            user = new PictureBox();
            button2 = new AntdUI.Button();
            button1 = new AntdUI.Button();
            labelTime1 = new AntdUI.LabelTime();
            panel1 = new AntdUI.Panel();
            more = new AntdUI.Button();
            tools = new AntdUI.Button();
            dwn = new AntdUI.Button();
            homebtn = new AntdUI.Button();
            shop = new AntdUI.Panel();
            page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)user).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // page
            // 
            page.BackColor = Color.FromArgb(255, 224, 192);
            page.Controls.Add(toast);
            page.Controls.Add(user);
            page.Controls.Add(button2);
            page.Controls.Add(button1);
            page.Controls.Add(labelTime1);
            page.Location = new Point(1, 1);
            page.MaximizeBox = false;
            page.Name = "page";
            page.ShowButton = true;
            page.ShowIcon = true;
            page.Size = new Size(892, 38);
            page.TabIndex = 0;
            page.Text = "超级小铺Neko";
            // 
            // toast
            // 
            toast.Location = new Point(379, 0);
            toast.Name = "toast";
            toast.Size = new Size(151, 41);
            toast.TabIndex = 4;
            toast.Text = "测试Toast通知";
            toast.Click += toast_Click;
            // 
            // user
            // 
            user.Image = Properties.Resources.用户__1_;
            user.Location = new Point(621, 9);
            user.Name = "user";
            user.Size = new Size(20, 20);
            user.SizeMode = PictureBoxSizeMode.Zoom;
            user.TabIndex = 3;
            user.TabStop = false;
            user.Click += user_Click;
            // 
            // button2
            // 
            button2.Location = new Point(279, 0);
            button2.Name = "button2";
            button2.Size = new Size(94, 41);
            button2.TabIndex = 2;
            button2.Text = "卡-30s";
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(163, 0);
            button1.Name = "button1";
            button1.Size = new Size(110, 41);
            button1.TabIndex = 1;
            button1.Text = "炸-Superdie";
            button1.Click += button1_Click;
            // 
            // labelTime1
            // 
            labelTime1.Location = new Point(654, 8);
            labelTime1.Name = "labelTime1";
            labelTime1.Size = new Size(142, 24);
            labelTime1.TabIndex = 0;
            labelTime1.Text = "labelTime1";
            // 
            // panel1
            // 
            panel1.Controls.Add(more);
            panel1.Controls.Add(tools);
            panel1.Controls.Add(dwn);
            panel1.Controls.Add(homebtn);
            panel1.Location = new Point(5, 45);
            panel1.Name = "panel1";
            panel1.Size = new Size(883, 46);
            panel1.TabIndex = 1;
            panel1.Text = "panel1";
            // 
            // more
            // 
            more.Icon = Properties.Resources.更多;
            more.Location = new Point(666, 1);
            more.Name = "more";
            more.Size = new Size(214, 43);
            more.TabIndex = 3;
            more.Text = "更多";
            more.Click += more_Click;
            // 
            // tools
            // 
            tools.Icon = Properties.Resources.工具;
            tools.Location = new Point(446, 1);
            tools.Name = "tools";
            tools.Size = new Size(214, 43);
            tools.TabIndex = 2;
            tools.Text = "工具";
            tools.Click += tools_Click;
            // 
            // dwn
            // 
            dwn.Icon = Properties.Resources.下载;
            dwn.Location = new Point(226, 1);
            dwn.Name = "dwn";
            dwn.Size = new Size(214, 43);
            dwn.TabIndex = 1;
            dwn.Text = "下载";
            dwn.Click += dwn_Click;
            // 
            // homebtn
            // 
            homebtn.Icon = Properties.Resources.主页;
            homebtn.Location = new Point(6, 1);
            homebtn.Name = "homebtn";
            homebtn.Size = new Size(214, 43);
            homebtn.TabIndex = 0;
            homebtn.Text = "主页";
            homebtn.Click += homebtn_Click;
            // 
            // shop
            // 
            shop.Location = new Point(5, 97);
            shop.Name = "shop";
            shop.Size = new Size(883, 360);
            shop.TabIndex = 2;
            shop.Text = "shop";
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            ClientSize = new Size(892, 463);
            Controls.Add(shop);
            Controls.Add(panel1);
            Controls.Add(page);
            Font = new Font("MiSans Semibold", 9F, FontStyle.Bold);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "超级小铺";
            FormClosing += Form1_FormClosing_1;
            page.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)user).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.PageHeader page;
        private AntdUI.Panel panel1;
        private AntdUI.Button more;
        private AntdUI.Button tools;
        private AntdUI.Button dwn;
        private AntdUI.Button homebtn;
        public AntdUI.Panel shop;
        private AntdUI.LabelTime labelTime1;
        private AntdUI.Button button1;
        private AntdUI.Button button2;
        private PictureBox user;
        private AntdUI.Button toast;
    }
}
