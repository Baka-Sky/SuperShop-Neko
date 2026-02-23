namespace SuperShop_Neko
{
    partial class user
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
            username = new AntdUI.Label();
            day = new AntdUI.Label();
            userid = new AntdUI.Label();
            label4 = new AntdUI.Label();
            ifsu = new AntdUI.Label();
            userpanel = new AntdUI.Panel();
            reg = new AntdUI.Button();
            login = new AntdUI.Button();
            password = new AntdUI.Input();
            who = new AntdUI.Input();
            label6 = new AntdUI.Label();
            exit = new AntdUI.Button();
            upwho = new AntdUI.Label();
            userpanel.SuspendLayout();
            SuspendLayout();
            // 
            // username
            // 
            username.Font = new Font("MiSans Medium", 15F);
            username.Location = new Point(31, 29);
            username.Name = "username";
            username.Size = new Size(166, 29);
            username.TabIndex = 0;
            username.Text = "你好,DevUser";
            // 
            // day
            // 
            day.Font = new Font("MiSans Medium", 15F);
            day.Location = new Point(31, 65);
            day.Name = "day";
            day.Size = new Size(281, 29);
            day.TabIndex = 1;
            day.Text = "超级小铺已陪伴您 XX天";
            // 
            // userid
            // 
            userid.Font = new Font("MiSans Medium", 15F);
            userid.Location = new Point(31, 363);
            userid.Name = "userid";
            userid.Size = new Size(486, 29);
            userid.TabIndex = 2;
            userid.Text = "UserID:";
            // 
            // label4
            // 
            label4.Font = new Font("MiSans Medium", 12F);
            label4.Location = new Point(31, 397);
            label4.Name = "label4";
            label4.Size = new Size(486, 29);
            label4.TabIndex = 3;
            label4.Text = "此代码可用于找回用户";
            // 
            // ifsu
            // 
            ifsu.Font = new Font("MiSans Medium", 15F);
            ifsu.Location = new Point(31, 171);
            ifsu.Name = "ifsu";
            ifsu.Size = new Size(281, 29);
            ifsu.TabIndex = 4;
            ifsu.Text = "权限为:XXX";
            // 
            // userpanel
            // 
            userpanel.Controls.Add(reg);
            userpanel.Controls.Add(login);
            userpanel.Controls.Add(password);
            userpanel.Controls.Add(who);
            userpanel.Controls.Add(label6);
            userpanel.Location = new Point(344, 133);
            userpanel.Name = "userpanel";
            userpanel.Size = new Size(391, 201);
            userpanel.TabIndex = 5;
            userpanel.Text = "panel1";
            // 
            // reg
            // 
            reg.Location = new Point(199, 155);
            reg.Name = "reg";
            reg.Size = new Size(176, 37);
            reg.TabIndex = 8;
            reg.Text = "注册";
            reg.Click += reg_Click;
            // 
            // login
            // 
            login.Location = new Point(12, 155);
            login.Name = "login";
            login.Size = new Size(181, 37);
            login.TabIndex = 7;
            login.Text = "登录";
            login.Click += login_Click;
            // 
            // password
            // 
            password.Location = new Point(3, 105);
            password.Name = "password";
            password.Size = new Size(385, 50);
            password.TabIndex = 2;
            // 
            // who
            // 
            who.Location = new Point(3, 49);
            who.Name = "who";
            who.Size = new Size(385, 50);
            who.TabIndex = 1;
            // 
            // label6
            // 
            label6.BackColor = Color.White;
            label6.Font = new Font("MiSans Medium", 9F);
            label6.Location = new Point(19, 14);
            label6.Name = "label6";
            label6.Size = new Size(157, 29);
            label6.TabIndex = 0;
            label6.Text = "您好 请登录";
            // 
            // exit
            // 
            exit.Location = new Point(983, 397);
            exit.Name = "exit";
            exit.Size = new Size(108, 50);
            exit.TabIndex = 6;
            exit.Text = "退出";
            exit.Click += exit_Click;
            // 
            // upwho
            // 
            upwho.Font = new Font("MiSans Medium", 15F);
            upwho.Location = new Point(31, 203);
            upwho.Name = "upwho";
            upwho.Size = new Size(281, 29);
            upwho.TabIndex = 7;
            upwho.Text = "代称为:XXX";
            // 
            // user
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(upwho);
            Controls.Add(exit);
            Controls.Add(userpanel);
            Controls.Add(ifsu);
            Controls.Add(label4);
            Controls.Add(userid);
            Controls.Add(day);
            Controls.Add(username);
            Font = new Font("MiSans Medium", 13F);
            Name = "user";
            Size = new Size(1103, 450);
            userpanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Label username;
        private AntdUI.Label day;
        private AntdUI.Label userid;
        private AntdUI.Label label4;
        private AntdUI.Label ifsu;
        private AntdUI.Panel userpanel;
        private AntdUI.Label label6;
        private AntdUI.Button exit;
        private AntdUI.Input password;
        private AntdUI.Input who;
        private AntdUI.Button reg;
        private AntdUI.Button login;
        private AntdUI.Label upwho;
    }
}
