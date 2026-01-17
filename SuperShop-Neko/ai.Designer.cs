namespace SuperShop_Neko
{
    partial class ai
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
            send = new AntdUI.Button();
            input = new AntdUI.Input();
            panel2 = new AntdUI.Panel();
            output = new AntdUI.Input();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(send);
            panel1.Controls.Add(input);
            panel1.Location = new Point(0, 402);
            panel1.Name = "panel1";
            panel1.Size = new Size(1103, 48);
            panel1.TabIndex = 0;
            panel1.Text = "panel1";
            // 
            // send
            // 
            send.DefaultBack = Color.AliceBlue;
            send.Font = new Font("MiSans Medium", 8F);
            send.Location = new Point(1001, 8);
            send.Name = "send";
            send.Size = new Size(94, 34);
            send.TabIndex = 1;
            send.Text = "发送";
            send.Click += send_Click;
            // 
            // input
            // 
            input.Font = new Font("MiSans Medium", 10F);
            input.Location = new Point(3, 5);
            input.Name = "input";
            input.Size = new Size(999, 40);
            input.TabIndex = 0;
            input.Text = "向AI提问";
            // 
            // panel2
            // 
            panel2.Controls.Add(output);
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(1103, 396);
            panel2.TabIndex = 2;
            panel2.Text = "panel2";
            // 
            // output
            // 
            output.AutoScroll = true;
            output.Font = new Font("MiSans Medium", 10F);
            output.Location = new Point(3, 3);
            output.Multiline = true;
            output.Name = "output";
            output.ReadOnly = true;
            output.Size = new Size(1097, 390);
            output.TabIndex = 2;
            // 
            // ai
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(panel2);
            Controls.Add(panel1);
            Font = new Font("MiSans Medium", 13F);
            Margin = new Padding(4);
            Name = "ai";
            Size = new Size(1103, 450);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel panel1;
        private AntdUI.Button send;
        private AntdUI.Input input;
        private AntdUI.Panel panel2;
        private AntdUI.Input output;
    }
}
