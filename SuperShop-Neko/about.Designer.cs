namespace SuperShop_Neko
{
    partial class about
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(about));
            pictureBox1 = new PictureBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            pictureBox2 = new PictureBox();
            label9 = new Label();
            input1 = new AntdUI.Input();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.未标题_1;
            pictureBox1.Location = new Point(12, 14);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(191, 98);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 125);
            label1.Name = "label1";
            label1.Size = new Size(329, 116);
            label1.TabIndex = 1;
            label1.Text = "感谢您使用SuperShop超级小铺!\r\n一年陪伴 感谢有你\r\n\r\nSuperShop的出现离不开他们↓";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 251);
            label2.Name = "label2";
            label2.Size = new Size(224, 29);
            label2.TabIndex = 2;
            label2.Text = "主体开发:Wells,Jiang";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 280);
            label3.Name = "label3";
            label3.Size = new Size(202, 29);
            label3.TabIndex = 3;
            label3.Text = "UI设计:Wells,Jiang";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 309);
            label4.Name = "label4";
            label4.Size = new Size(224, 29);
            label4.TabIndex = 4;
            label4.Text = "软件宣发:Wells,Jiang";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 338);
            label5.Name = "label5";
            label5.Size = new Size(307, 29);
            label5.TabIndex = 5;
            label5.Text = "HeartEngine开发:Wells,Jiang";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 367);
            label6.Name = "label6";
            label6.Size = new Size(286, 29);
            label6.TabIndex = 6;
            label6.Text = "HeartCore开发:Wells,Jiang";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 396);
            label7.Name = "label7";
            label7.Size = new Size(224, 29);
            label7.TabIndex = 7;
            label7.Text = "主体测试:Wells,Jiang";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(209, 32);
            label8.Name = "label8";
            label8.Size = new Size(234, 58);
            label8.TabIndex = 8;
            label8.Text = "SuperShop Neko\r\n软著登字第17271271号";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.cb78ae4e11e3cf263380cfafe3540b4;
            pictureBox2.Location = new Point(837, 169);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(252, 256);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 9;
            pictureBox2.TabStop = false;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("MiSans Medium", 9F);
            label9.Location = new Point(837, 139);
            label9.Name = "label9";
            label9.Size = new Size(223, 20);
            label9.TabIndex = 10;
            label9.Text = "如果您觉得制作不错 请赞助我！";
            // 
            // input1
            // 
            input1.AutoScroll = true;
            input1.Font = new Font("MiSans Medium", 9F);
            input1.Location = new Point(347, 125);
            input1.Multiline = true;
            input1.Name = "input1";
            input1.ReadOnly = true;
            input1.Size = new Size(484, 311);
            input1.TabIndex = 11;
            input1.Text = resources.GetString("input1.Text");
            // 
            // about
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(input1);
            Controls.Add(label9);
            Controls.Add(pictureBox2);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Font = new Font("MiSans Medium", 13F);
            Name = "about";
            Size = new Size(1103, 450);
            Load += about_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private PictureBox pictureBox2;
        private Label label9;
        private AntdUI.Input input1;
    }
}
