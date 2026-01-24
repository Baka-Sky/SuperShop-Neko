namespace SuperShop_Neko
{
    partial class boot
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
            pictureBox2 = new PictureBox();
            progress1 = new AntdUI.Progress();
            pictureBox1 = new PictureBox();
            booting = new AntdUI.Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.未标题_11;
            pictureBox2.Location = new Point(2, -8);
            pictureBox2.Margin = new Padding(5, 4, 5, 4);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(336, 136);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // progress1
            // 
            progress1.Location = new Point(-1, 105);
            progress1.Name = "progress1";
            progress1.Size = new Size(336, 23);
            progress1.TabIndex = 4;
            progress1.Text = "progress1";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.未标题_1;
            pictureBox1.Location = new Point(-4, -12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(356, 162);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // booting
            // 
            booting.Font = new Font("Microsoft YaHei UI", 12.75F, FontStyle.Bold);
            booting.Location = new Point(12, 95);
            booting.Name = "booting";
            booting.Size = new Size(323, 23);
            booting.TabIndex = 2;
            booting.Text = "自检阶段";
            booting.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // boot
            // 
            ClientSize = new Size(347, 147);
            Controls.Add(booting);
            Controls.Add(pictureBox1);
            Name = "boot";
            StartPosition = FormStartPosition.CenterScreen;
            Load += boot_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            // 
            // boot
            // 

        }

        #endregion
        private PictureBox pictureBox2;
        private AntdUI.Progress progress1;
        private PictureBox pictureBox1;
        private AntdUI.Label booting;
    }
}