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
    public partial class delete : UserControl
    {
        // 数据库连接字符串（与upload.cs保持一致）
        public static string mysqlcon = "server=api.baka233.top;database=supershop;user=sudabaka;password=jianghao0523;";

        public delete()
        {
            InitializeComponent();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            // 保持原有的panel1点击事件
        }

        private void del_Click(object sender, EventArgs e)
        {
            // 检查输入框是否为空
            if (string.IsNullOrWhiteSpace(app.Text))
            {
                MessageBox.Show("请输入要删除的软件名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                app.Focus();
                return;
            }

            // 确认删除操作
            DialogResult result = MessageBox.Show(
                $"确定要删除软件 '{app.Text}' 的所有信息吗？",
                "确认删除",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlcon))
                {
                    connection.Open();

                    // 先查询是否存在该软件
                    string checkSql = "SELECT COUNT(*) FROM app WHERE 软件名 = @appName";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@appName", app.Text.Trim());
                        long count = (long)checkCommand.ExecuteScalar();

                        if (count == 0)
                        {
                            MessageBox.Show($"软件 '{app.Text}' 不存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            app.Focus();
                            return;
                        }
                    }

                    // 执行删除操作
                    string deleteSql = "DELETE FROM app WHERE 软件名 = @appName";
                    using (MySqlCommand deleteCommand = new MySqlCommand(deleteSql, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@appName", app.Text.Trim());

                        int rowsAffected = deleteCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"软件 '{app.Text}' 已成功删除！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // 清空输入框
                            app.Clear();
                        }
                        else
                        {
                            MessageBox.Show("删除失败，请重试！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // 可选：添加一个按键事件，按Enter键触发删除

        }
    }
