namespace SuperShop_Neko
{
    partial class error
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(error));
            page = new AntdUI.PageHeader();
            pictureBox1 = new PictureBox();
            label1 = new AntdUI.Label();
            label2 = new AntdUI.Label();
            errortext = new AntdUI.Input();
            reboot = new AntdUI.Button();
            kill = new AntdUI.Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // page
            // 
            page.BackColor = Color.FromArgb(255, 224, 192);
            page.Location = new Point(0, 0);
            page.MaximizeBox = false;
            page.Name = "page";
            page.ShowButton = true;
            page.ShowIcon = true;
            page.Size = new Size(865, 38);
            page.TabIndex = 1;
            page.Text = "超级小铺Neko";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(728, 58);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(112, 87);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.Font = new Font("Microsoft YaHei UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label1.Location = new Point(24, 61);
            label1.Name = "label1";
            label1.Size = new Size(234, 35);
            label1.TabIndex = 3;
            label1.Text = "出错了!  ▌°Д °;)っ";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.Font = new Font("Microsoft YaHei UI", 11F);
            label2.Location = new Point(24, 91);
            label2.Name = "label2";
            label2.Size = new Size(579, 57);
            label2.TabIndex = 4;
            label2.Text = "超级小铺出现了无法自动恢复的问题  详细问题如下  这可能是未知问题 \r\n或者是开发时遗留的问题";
            // 
            // errortext
            // 
            errortext.AutoScroll = true;
            errortext.Font = new Font("Microsoft YaHei UI", 12F);
            errortext.Location = new Point(13, 151);
            errortext.Multiline = true;
            errortext.Name = "errortext";
            errortext.ReadOnly = true;
            errortext.Size = new Size(833, 250);
            errortext.TabIndex = 5;
            // 
            // reboot
            // 
            reboot.DefaultBack = Color.PaleGreen;
            reboot.Font = new Font("Microsoft YaHei UI", 12F);
            reboot.Location = new Point(13, 402);
            reboot.Name = "reboot";
            reboot.Size = new Size(420, 51);
            reboot.TabIndex = 6;
            reboot.Text = "尝试重启超级小铺";
            reboot.Click += reboot_Click_1;
            // 
            // kill
            // 
            kill.DefaultBack = Color.Gray;
            kill.Font = new Font("Microsoft YaHei UI", 12F);
            kill.Location = new Point(439, 402);
            kill.Name = "kill";
            kill.Size = new Size(407, 51);
            kill.TabIndex = 7;
            kill.Text = "强制结束超级小铺进程";
            kill.Click += kill_Click_1;
            // 
            // error
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            BackColor = Color.White;
            ClientSize = new Size(865, 463);
            Controls.Add(kill);
            Controls.Add(reboot);
            Controls.Add(errortext);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(page);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "error";
            Text = "error";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.PageHeader page;
        private PictureBox pictureBox1;
        private AntdUI.Label label1;
        private AntdUI.Label label2;
        private AntdUI.Input errortext;
        private AntdUI.Button reboot;
        private AntdUI.Button kill;
    }
}