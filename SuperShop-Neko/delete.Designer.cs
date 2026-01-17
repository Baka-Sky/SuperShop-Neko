namespace SuperShop_Neko
{
    partial class delete
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
            del = new AntdUI.Button();
            app = new AntdUI.Input();
            label2 = new AntdUI.Label();
            label1 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(del);
            panel1.Controls.Add(app);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Font = new Font("MiSans Normal", 9F);
            panel1.Location = new Point(3, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(274, 416);
            panel1.TabIndex = 0;
            panel1.Text = "panel1";
            panel1.Click += panel1_Click;
            // 
            // del
            // 
            del.Font = new Font("MiSans Normal", 9F);
            del.Location = new Point(2, 362);
            del.Name = "del";
            del.Size = new Size(268, 43);
            del.TabIndex = 10;
            del.Text = "删除";
            del.Click += del_Click;
            // 
            // app
            // 
            app.Font = new Font("MiSans Normal", 9F);
            app.Location = new Point(0, 82);
            app.Name = "app";
            app.Size = new Size(271, 49);
            app.TabIndex = 3;
            // 
            // label2
            // 
            label2.BackColor = Color.Transparent;
            label2.Location = new Point(12, 59);
            label2.Name = "label2";
            label2.Size = new Size(54, 20);
            label2.TabIndex = 2;
            label2.Text = "软件名";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.White;
            label1.Font = new Font("MiSans Medium", 14.8F);
            label1.Location = new Point(3, 9);
            label1.Name = "label1";
            label1.Size = new Size(240, 33);
            label1.TabIndex = 1;
            label1.Text = "从超级小铺删除软件";
            // 
            // delete
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(panel1);
            Font = new Font("MiSans Semibold", 9F, FontStyle.Bold);
            Name = "delete";
            Size = new Size(280, 419);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel panel1;
        private Label label1;
        private AntdUI.Label label2;
        private AntdUI.Input app;
        private AntdUI.Button del;
    }
}
