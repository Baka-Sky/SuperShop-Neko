namespace SuperShop_Neko
{
    partial class upload
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
            load = new AntdUI.Button();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            by = new AntdUI.Input();
            who = new AntdUI.Input();
            dwntext = new AntdUI.Input();
            app = new AntdUI.Input();
            label1 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(load);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(by);
            panel1.Controls.Add(who);
            panel1.Controls.Add(dwntext);
            panel1.Controls.Add(app);
            panel1.Controls.Add(label1);
            panel1.Font = new Font("MiSans Normal", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            panel1.ForeColor = SystemColors.ActiveCaptionText;
            panel1.HandCursor = Cursors.HSplit;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(274, 411);
            panel1.TabIndex = 0;
            panel1.Text = "panel1";
            // 
            // load
            // 
            load.Location = new Point(2, 362);
            load.Name = "load";
            load.Size = new Size(268, 43);
            load.TabIndex = 9;
            load.Text = "上传!";
            load.Click += load_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.White;
            label5.Font = new Font("MiSans Medium", 9F);
            label5.Location = new Point(12, 287);
            label5.Name = "label5";
            label5.Size = new Size(69, 20);
            label5.TabIndex = 8;
            label5.Text = "软件出处";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.White;
            label4.Font = new Font("MiSans Medium", 9F);
            label4.Location = new Point(12, 211);
            label4.Name = "label4";
            label4.Size = new Size(54, 20);
            label4.TabIndex = 7;
            label4.Text = "上传者";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.White;
            label3.Font = new Font("MiSans Medium", 9F);
            label3.Location = new Point(12, 135);
            label3.Name = "label3";
            label3.Size = new Size(39, 20);
            label3.TabIndex = 6;
            label3.Text = "链接";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.White;
            label2.Font = new Font("MiSans Medium", 9F);
            label2.Location = new Point(12, 59);
            label2.Name = "label2";
            label2.Size = new Size(54, 20);
            label2.TabIndex = 5;
            label2.Text = "软件名";
            // 
            // by
            // 
            by.Location = new Point(0, 310);
            by.Name = "by";
            by.Size = new Size(271, 49);
            by.TabIndex = 4;
            // 
            // who
            // 
            who.Location = new Point(0, 234);
            who.Name = "who";
            who.Size = new Size(271, 49);
            who.TabIndex = 3;
            // 
            // dwntext
            // 
            dwntext.Location = new Point(0, 158);
            dwntext.Name = "dwntext";
            dwntext.Size = new Size(271, 49);
            dwntext.TabIndex = 2;
            // 
            // app
            // 
            app.Location = new Point(0, 82);
            app.Name = "app";
            app.Size = new Size(271, 49);
            app.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.White;
            label1.Font = new Font("MiSans Medium", 14.8F);
            label1.Location = new Point(3, 9);
            label1.Name = "label1";
            label1.Size = new Size(240, 33);
            label1.TabIndex = 0;
            label1.Text = "上传软件到超级小铺";
            // 
            // upload
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            BackColor = Color.White;
            Controls.Add(panel1);
            Font = new Font("MiSans Semibold", 9F, FontStyle.Bold);
            Name = "upload";
            Size = new Size(280, 419);
            Load += upload_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel panel1;
        private Label label1;
        private AntdUI.Input who;
        private AntdUI.Input dwntext;
        private AntdUI.Input app;
        private AntdUI.Button load;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private AntdUI.Input by;
    }
}
