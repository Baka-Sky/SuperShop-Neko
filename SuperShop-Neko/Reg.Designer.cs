namespace SuperShop_Neko
{
    partial class Reg
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
            label1 = new AntdUI.Label();
            label2 = new AntdUI.Label();
            username = new AntdUI.Input();
            label3 = new AntdUI.Label();
            password = new AntdUI.Input();
            GO = new AntdUI.Button();
            text111 = new AntdUI.Label();
            uploadname = new AntdUI.Input();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Font = new Font("MiSans Semibold", 14F, FontStyle.Bold);
            label1.Location = new Point(21, 19);
            label1.Name = "label1";
            label1.Size = new Size(209, 32);
            label1.TabIndex = 0;
            label1.Text = "注册超级小铺账号";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.Font = new Font("MiSans Semibold", 10F, FontStyle.Bold);
            label2.Location = new Point(20, 88);
            label2.Name = "label2";
            label2.Size = new Size(58, 26);
            label2.TabIndex = 1;
            label2.Text = "用户名";
            // 
            // username
            // 
            username.Location = new Point(12, 120);
            username.Name = "username";
            username.Size = new Size(320, 50);
            username.TabIndex = 2;
            username.TextChanged += username_TextChanged;
            // 
            // label3
            // 
            label3.Font = new Font("MiSans Semibold", 10F, FontStyle.Bold);
            label3.Location = new Point(20, 176);
            label3.Name = "label3";
            label3.Size = new Size(58, 26);
            label3.TabIndex = 3;
            label3.Text = "密码";
            // 
            // password
            // 
            password.Location = new Point(12, 208);
            password.Name = "password";
            password.Size = new Size(653, 50);
            password.TabIndex = 4;
            password.TextChanged += password_TextChanged;
            // 
            // GO
            // 
            GO.DefaultBack = Color.AliceBlue;
            GO.Location = new Point(12, 288);
            GO.Name = "GO";
            GO.Size = new Size(653, 50);
            GO.TabIndex = 5;
            GO.Text = "注册！";
            GO.Click += GO_Click;
            // 
            // text111
            // 
            text111.Font = new Font("MiSans Semibold", 10F, FontStyle.Bold);
            text111.Location = new Point(347, 88);
            text111.Name = "text111";
            text111.Size = new Size(93, 26);
            text111.TabIndex = 6;
            text111.Text = "上传者代称";
            // 
            // uploadname
            // 
            uploadname.Location = new Point(338, 120);
            uploadname.Name = "uploadname";
            uploadname.Size = new Size(327, 50);
            uploadname.TabIndex = 7;
//            uploadname.TextChanged += input1_TextChanged;
            // 
            // Reg
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            ClientSize = new Size(677, 350);
            Controls.Add(uploadname);
            Controls.Add(text111);
            Controls.Add(GO);
            Controls.Add(password);
            Controls.Add(label3);
            Controls.Add(username);
            Controls.Add(label2);
            Controls.Add(label1);
            Font = new Font("MiSans Semibold", 9F, FontStyle.Bold);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Reg";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "注册-Beta";
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Label label1;
        private AntdUI.Label label2;
        private AntdUI.Input username;
        private AntdUI.Label label3;
        private AntdUI.Input password;
        private AntdUI.Button GO;
        private AntdUI.Label text111;
        private AntdUI.Input uploadname;
    }
}