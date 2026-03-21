namespace SuperShop_Neko
{
    partial class app
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
            reload = new AntdUI.Button();
            delbtn = new AntdUI.Button();
            upbutton = new AntdUI.Button();
            label1 = new AntdUI.Label();
            SmallPanel = new AntdUI.Panel();
            wherebtn = new AntdUI.Button();
            download = new AntdUI.Button();
            form = new AntdUI.Label();
            who = new AntdUI.Label();
            appname = new AntdUI.Label();
            SuperPanel = new Panel();
            panel1.SuspendLayout();
            SmallPanel.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(reload);
            panel1.Controls.Add(delbtn);
            panel1.Controls.Add(upbutton);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1097, 40);
            panel1.TabIndex = 0;
            panel1.Text = "panel1";
            // 
            // reload
            // 
            reload.BackColor = Color.Black;
            reload.DefaultBack = Color.AliceBlue;
            reload.Location = new Point(298, 2);
            reload.Name = "reload";
            reload.Size = new Size(94, 35);
            reload.TabIndex = 4;
            reload.Text = "刷新";
            reload.Click += reload_Click;
            // 
            // delbtn
            // 
            delbtn.BackColor = Color.Black;
            delbtn.DefaultBack = Color.AliceBlue;
            delbtn.Location = new Point(198, 2);
            delbtn.Name = "delbtn";
            delbtn.Size = new Size(94, 35);
            delbtn.TabIndex = 3;
            delbtn.Text = "删除软件";
            delbtn.Click += delbtn_Click;
            // 
            // upbutton
            // 
            upbutton.BackColor = Color.Black;
            upbutton.DefaultBack = Color.AliceBlue;
            upbutton.Location = new Point(98, 2);
            upbutton.Name = "upbutton";
            upbutton.Size = new Size(94, 35);
            upbutton.TabIndex = 2;
            upbutton.Text = "上传软件";
            upbutton.Click += button1_Click;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("MiSans Semibold", 11F, FontStyle.Bold);
            label1.Location = new Point(18, 6);
            label1.Name = "label1";
            label1.Size = new Size(94, 29);
            label1.TabIndex = 0;
            label1.Text = "下载工具";
            // 
            // SmallPanel
            // 
            SmallPanel.Controls.Add(wherebtn);
            SmallPanel.Controls.Add(download);
            SmallPanel.Controls.Add(form);
            SmallPanel.Controls.Add(who);
            SmallPanel.Controls.Add(appname);
            SmallPanel.Location = new Point(6, 52);
            SmallPanel.Name = "SmallPanel";
            SmallPanel.Size = new Size(1059, 49);
            SmallPanel.TabIndex = 5;
            SmallPanel.Text = "panel2";
            // 
            // wherebtn
            // 
            wherebtn.DefaultBack = Color.AliceBlue;
            //wherebtn.Icon = Properties.Resources.查看出处;
            wherebtn.Location = new Point(915, 4);
            wherebtn.Name = "wherebtn";
            wherebtn.Size = new Size(139, 40);
            wherebtn.TabIndex = 5;
            wherebtn.Text = "查看出处";
            // 
            // download
            // 
            download.DefaultBack = Color.AliceBlue;
            download.Icon = Properties.Resources.下载;
            download.Location = new Point(760, 4);
            download.Name = "download";
            download.Size = new Size(139, 40);
            download.TabIndex = 4;
            download.Text = "下载软件";
            // 
            // form
            // 
            form.Font = new Font("MiSans Medium", 7.799999F);
            form.Location = new Point(275, 26);
            form.Name = "form";
            form.Size = new Size(473, 15);
            form.TabIndex = 3;
            form.Text = "软件来源";
            // 
            // who
            // 
            who.Font = new Font("MiSans Medium", 7.799999F, FontStyle.Regular, GraphicsUnit.Point, 134);
            who.Location = new Point(275, 9);
            who.Name = "who";
            who.Size = new Size(473, 15);
            who.TabIndex = 2;
            who.Text = "上传者:Baka233.top";
            // 
            // appname
            // 
            appname.Font = new Font("MiSans Semibold", 7F, FontStyle.Bold);
            appname.Location = new Point(13, 9);
            appname.Name = "appname";
            appname.Size = new Size(256, 32);
            appname.TabIndex = 1;
            appname.Text = "实例软件";
            // 
            // SuperPanel
            // 
            SuperPanel.Location = new Point(3, 49);
            SuperPanel.Name = "SuperPanel";
            SuperPanel.Size = new Size(1097, 398);
            SuperPanel.TabIndex = 6;
            // 
            // app
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(SmallPanel);
            Controls.Add(SuperPanel);
            Controls.Add(panel1);
            Font = new Font("MiSans Medium", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Name = "app";
            Size = new Size(1103, 450);
            Load += app_Load_1;
            panel1.ResumeLayout(false);
            SmallPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel panel1;
        private AntdUI.Label label1;
        private AntdUI.Button upbutton;
        private AntdUI.Button delbtn;
        private AntdUI.Button reload;
        private AntdUI.Panel SmallPanel;
        private AntdUI.Button wherebtn;
        private AntdUI.Button download;
        private AntdUI.Label form;
        private AntdUI.Label who;
        private AntdUI.Label appname;
        private Panel SuperPanel;
    }
}