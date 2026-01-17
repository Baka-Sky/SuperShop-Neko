namespace SuperShop_Neko
{
    partial class set
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
            color = new AntdUI.Switch();
            label1 = new AntdUI.Label();
            panel2 = new AntdUI.Panel();
            label2 = new AntdUI.Label();
            clean = new AntdUI.Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(color);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1097, 53);
            panel1.TabIndex = 9;
            panel1.Text = "panel1";
            // 
            // color
            // 
            color.BackColor = Color.White;
            color.Location = new Point(1008, 8);
            color.Name = "color";
            color.Size = new Size(66, 37);
            color.TabIndex = 1;
            color.Text = "switch1";
            color.CheckedChanged += color_CheckedChanged;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("MiSans Semibold", 11F, FontStyle.Bold);
            label1.Location = new Point(23, 8);
            label1.Name = "label1";
            label1.Size = new Size(317, 37);
            label1.TabIndex = 0;
            label1.Text = "UI使用Windows默认主题色";
            // 
            // panel2
            // 
            panel2.Controls.Add(clean);
            panel2.Controls.Add(label2);
            panel2.Location = new Point(3, 62);
            panel2.Name = "panel2";
            panel2.Size = new Size(1097, 53);
            panel2.TabIndex = 10;
            panel2.Text = "panel2";
            // 
            // label2
            // 
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("MiSans Semibold", 11F, FontStyle.Bold);
            label2.Location = new Point(23, 8);
            label2.Name = "label2";
            label2.Size = new Size(317, 37);
            label2.TabIndex = 0;
            label2.Text = "强制清除GC";
            // 
            // clean
            // 
            clean.DefaultBack = Color.AliceBlue;
            clean.Location = new Point(990, 2);
            clean.Name = "clean";
            clean.Size = new Size(94, 47);
            clean.TabIndex = 1;
            clean.Text = "删除";
            clean.Click += clean_Click;
            // 
            // set
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(panel2);
            Controls.Add(panel1);
            Font = new Font("MiSans Medium", 9F);
            Name = "set";
            Size = new Size(1103, 450);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel panel1;
        private AntdUI.Label label1;
        private AntdUI.Switch color;
        private AntdUI.Panel panel2;
        private AntdUI.Button clean;
        private AntdUI.Label label2;
    }
}
