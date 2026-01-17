namespace SuperShop_Neko
{
    partial class app
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            panel1 = new AntdUI.Panel();
            reload = new AntdUI.Button();
            delbtn = new AntdUI.Button();
            upbutton = new AntdUI.Button();
            label1 = new AntdUI.Label();
            dataGridView1 = new DataGridView();
            软件名 = new DataGridViewTextBoxColumn();
            链接 = new DataGridViewTextBoxColumn();
            上传者 = new DataGridViewTextBoxColumn();
            查看出处 = new DataGridViewButtonColumn();
            下载 = new DataGridViewButtonColumn();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(reload);
            panel1.Controls.Add(delbtn);
            panel1.Controls.Add(upbutton);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1097, 40);
            panel1.TabIndex = 0;
            panel1.Text = "panel1";
            // 
            // reload
            // 
            reload.BackColor = Color.Black;
            reload.DefaultBack = Color.AliceBlue;
            reload.Location = new Point(298, 2);
            reload.Name = "reload";
            reload.Size = new Size(94, 35);
            reload.TabIndex = 4;
            reload.Text = "刷新";
            reload.Click += reload_Click;
            // 
            // delbtn
            // 
            delbtn.BackColor = Color.Black;
            delbtn.DefaultBack = Color.AliceBlue;
            delbtn.Location = new Point(198, 2);
            delbtn.Name = "delbtn";
            delbtn.Size = new Size(94, 35);
            delbtn.TabIndex = 3;
            delbtn.Text = "删除软件";
            delbtn.Click += delbtn_Click;
            // 
            // upbutton
            // 
            upbutton.BackColor = Color.Black;
            upbutton.DefaultBack = Color.AliceBlue;
            upbutton.Location = new Point(98, 2);
            upbutton.Name = "upbutton";
            upbutton.Size = new Size(94, 35);
            upbutton.TabIndex = 2;
            upbutton.Text = "上传软件";
            upbutton.Click += button1_Click;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("MiSans Semibold", 11F, FontStyle.Bold);
            label1.Location = new Point(18, 6);
            label1.Name = "label1";
            label1.Size = new Size(94, 29);
            label1.TabIndex = 0;
            label1.Text = "下载工具";
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = true;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.BackgroundColor = Color.AliceBlue;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { 软件名, 链接, 上传者, 查看出处, 下载 });
            dataGridView1.GridColor = SystemColors.ControlLightLight;
            dataGridView1.Location = new Point(0, 49);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 27;
            dataGridView1.Size = new Size(1103, 398);
            dataGridView1.TabIndex = 5;
            // 
            // 软件名
            // 
            软件名.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            软件名.DataPropertyName = "软件名";
            软件名.HeaderText = "软件名";
            软件名.MinimumWidth = 6;
            软件名.Name = "软件名";
            软件名.ReadOnly = true;
            软件名.Resizable = DataGridViewTriState.False;
            软件名.SortMode = DataGridViewColumnSortMode.NotSortable;
            软件名.Width = 270;
            // 
            // 链接
            // 
            链接.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            链接.DataPropertyName = "链接";
            链接.HeaderText = "链接";
            链接.MinimumWidth = 6;
            链接.Name = "链接";
            链接.ReadOnly = true;
            链接.Resizable = DataGridViewTriState.False;
            链接.Width = 220;
            // 
            // 上传者
            // 
            上传者.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            上传者.DataPropertyName = "上传者";
            上传者.HeaderText = "上传者";
            上传者.MinimumWidth = 6;
            上传者.Name = "上传者";
            上传者.ReadOnly = true;
            上传者.Resizable = DataGridViewTriState.False;
            // 
            // 查看出处
            // 
            查看出处.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Font = new Font("MiSans Medium", 9F);
            查看出处.DefaultCellStyle = dataGridViewCellStyle1;
            查看出处.HeaderText = "查看出处";
            查看出处.MinimumWidth = 6;
            查看出处.Name = "查看出处";
            查看出处.ReadOnly = true;
            查看出处.Resizable = DataGridViewTriState.False;
            查看出处.Text = "查看出处";
            查看出处.ToolTipText = "查看出处";
            查看出处.UseColumnTextForButtonValue = true;
            查看出处.Width = 140;
            // 
            // 下载
            // 
            下载.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new Font("MiSans Medium", 9F);
            下载.DefaultCellStyle = dataGridViewCellStyle2;
            下载.HeaderText = "下载";
            下载.MinimumWidth = 6;
            下载.Name = "下载";
            下载.ReadOnly = true;
            下载.Resizable = DataGridViewTriState.False;
            下载.Text = "下载";
            下载.ToolTipText = "下载";
            下载.UseColumnTextForButtonValue = true;
            下载.Width = 140;
            // 
            // app
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(dataGridView1);
            Controls.Add(panel1);
            Font = new Font("MiSans Medium", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Name = "app";
            Size = new Size(1103, 450);
            Load += app_Load_1;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel panel1;
        private AntdUI.Label label1;
        private AntdUI.Button upbutton;
        private AntdUI.Button delbtn;
        private AntdUI.Button reload;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn 软件名;
        private DataGridViewTextBoxColumn 链接;
        private DataGridViewTextBoxColumn 上传者;
        private DataGridViewButtonColumn 查看出处;
        private DataGridViewButtonColumn 下载;
    }
}