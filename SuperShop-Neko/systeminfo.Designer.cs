namespace SuperShop_Neko
{
    partial class systeminfo
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
            mother = new AntdUI.Label();
            label1 = new AntdUI.Label();
            panel2 = new AntdUI.Panel();
            windows = new AntdUI.Label();
            label2 = new AntdUI.Label();
            panel3 = new AntdUI.Panel();
            runtime = new AntdUI.Label();
            label3 = new AntdUI.Label();
            panel4 = new AntdUI.Panel();
            moreinfo = new AntdUI.Input();
            infotips = new AntdUI.Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(mother);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(346, 147);
            panel1.TabIndex = 0;
            panel1.Text = "panel1";
            // 
            // mother
            // 
            mother.BackColor = Color.Transparent;
            mother.Font = new Font("MiSans Medium", 9F);
            mother.Location = new Point(20, 59);
            mother.Name = "mother";
            mother.Size = new Size(325, 74);
            mother.TabIndex = 1;
            mother.Text = "";
            mother.Click += mother_Click;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("MiSans Medium", 14F);
            label1.Location = new Point(18, 13);
            label1.Name = "label1";
            label1.Size = new Size(157, 23);
            label1.TabIndex = 0;
            label1.Text = "主板信息";
            // 
            // panel2
            // 
            panel2.Controls.Add(windows);
            panel2.Controls.Add(label2);
            panel2.Location = new Point(355, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(375, 147);
            panel2.TabIndex = 1;
            panel2.Text = "panel2";
            // 
            // windows
            // 
            windows.BackColor = Color.Transparent;
            windows.Font = new Font("MiSans Medium", 9F);
            windows.Location = new Point(20, 58);
            windows.Name = "windows";
            windows.Size = new Size(352, 74);
            windows.TabIndex = 2;
            windows.Text = "";
            windows.Click += windows_Click;
            // 
            // label2
            // 
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("MiSans Medium", 14F);
            label2.Location = new Point(20, 13);
            label2.Name = "label2";
            label2.Size = new Size(124, 23);
            label2.TabIndex = 1;
            label2.Text = "系统信息";
            // 
            // panel3
            // 
            panel3.Controls.Add(runtime);
            panel3.Controls.Add(label3);
            panel3.Location = new Point(736, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(364, 147);
            panel3.TabIndex = 2;
            panel3.Text = "panel3";
            // 
            // runtime
            // 
            runtime.BackColor = Color.Transparent;
            runtime.Font = new Font("MiSans Medium", 9F);
            runtime.Location = new Point(21, 59);
            runtime.Name = "runtime";
            runtime.Size = new Size(340, 74);
            runtime.TabIndex = 3;
            runtime.Text = "";
            runtime.Click += runtime_Click;
            // 
            // label3
            // 
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("MiSans Medium", 14F);
            label3.Location = new Point(21, 13);
            label3.Name = "label3";
            label3.Size = new Size(171, 23);
            label3.TabIndex = 2;
            label3.Text = "当前运行时间";
            // 
            // panel4
            // 
            panel4.Controls.Add(moreinfo);
            panel4.Controls.Add(infotips);
            panel4.Location = new Point(3, 156);
            panel4.Name = "panel4";
            panel4.Size = new Size(1097, 291);
            panel4.TabIndex = 2;
            panel4.Text = "panel4";
            // 
            // moreinfo
            // 
            moreinfo.AutoScroll = true;
            moreinfo.Location = new Point(3, 46);
            moreinfo.Multiline = true;
            moreinfo.Name = "moreinfo";
            moreinfo.ReadOnly = true;
            moreinfo.Size = new Size(1091, 242);
            moreinfo.TabIndex = 4;
            // 
            // infotips
            // 
            infotips.BackColor = Color.Transparent;
            infotips.Font = new Font("MiSans Medium", 14F);
            infotips.Location = new Point(18, 17);
            infotips.Name = "infotips";
            infotips.Size = new Size(168, 23);
            infotips.TabIndex = 2;
            infotips.Text = "硬件详细信息";
            // 
            // systeminfo
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.Control;
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Font = new Font("MiSans Medium", 9F);
            Margin = new Padding(4);
            Name = "systeminfo";
            Size = new Size(1103, 450);
            Load += systeminfo_Load;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel panel1;
        private AntdUI.Panel panel2;
        private AntdUI.Panel panel3;
        private AntdUI.Panel panel4;
        private AntdUI.Label mother;
        private AntdUI.Label label1;
        private AntdUI.Label windows;
        private AntdUI.Label label2;
        private AntdUI.Label runtime;
        private AntdUI.Label label3;
        private AntdUI.Label infotips;
        private AntdUI.Input moreinfo;
    }
}
