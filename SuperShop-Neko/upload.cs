using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySqlConnector;

namespace SuperShop_Neko
{
    public partial class upload : UserControl
    {
        // 数据库连接字符串
        public static string mysqlcon = "server=api.baka233.top;database=supershop;user=sudabaka;password=jianghao0523;";

        public upload()
        {
            InitializeComponent();
        }

        private void upload_Load(object sender, EventArgs e)
        {
            // 页面加载时的初始化代码可以放在这里
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            // 保持原有的panel2点击事件
        }

        private void load_Click(object sender, EventArgs e)
        {
            // 检查必填字段是否为空
            if (string.IsNullOrWhiteSpace(app.Text))
            {
                MessageBox.Show("请输入软件名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                app.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(dwntext.Text))
            {
                MessageBox.Show("请输入下载链接！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dwntext.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(who.Text))
            {
                MessageBox.Show("请输入上传者！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                who.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(by.Text))
            {
                MessageBox.Show("请输入出处！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                by.Focus();
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlcon))
                {
                    connection.Open();

                    // SQL插入语句
                    string sql = "INSERT INTO app (软件名, 链接, 上传者, 出处) VALUES (@appName, @link, @uploader, @source)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        // 使用参数化查询防止SQL注入
                        command.Parameters.AddWithValue("@appName", app.Text.Trim());
                        command.Parameters.AddWithValue("@link", dwntext.Text.Trim());
                        command.Parameters.AddWithValue("@uploader", who.Text.Trim());
                        command.Parameters.AddWithValue("@source", by.Text.Trim());

                        // 执行插入操作
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("上传已完成！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // 清空输入框
                            app.Clear();
                            dwntext.Clear();
                            who.Clear();
                            by.Clear();
                        }
                        else
                        {
                            MessageBox.Show("上传失败，请重试！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"数据库错误: {ex.Message}", "数据库错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 假设您有以下TextBox控件，请确保它们的名称正确：
        // private System.Windows.Forms.TextBox app;     // 软件名
        // private System.Windows.Forms.TextBox dwntext; // 链接
        // private System.Windows.Forms.TextBox who;     // 上传者
        // private System.Windows.Forms.TextBox by;      // 出处
    }
}