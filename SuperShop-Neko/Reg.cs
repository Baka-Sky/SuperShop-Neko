using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SuperShop_Neko
{
    public partial class Reg : Form
    {
        public Reg()
        {
            InitializeComponent();
        }

        private async void GO_Click(object sender, EventArgs e)
        {
            // 修正：使用 uploadname 而不是 upname
            string username = this.username.Text.Trim();
            string userPassword = this.password.Text.Trim();
            string uploaderName = this.uploadname.Text.Trim();

            // 验证输入
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("请输入用户名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.username.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(userPassword))
            {
                MessageBox.Show("请输入密码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.password.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(uploaderName))
            {
                MessageBox.Show("请输入上传者代称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.uploadname.Focus();
                return;
            }

            if (userPassword.Length < 6)
            {
                MessageBox.Show("密码长度不能少于6位！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.password.Focus();
                return;
            }

            try
            {
                GO.Enabled = false;
                GO.Text = "注册中...";

                var regData = new Dictionary<string, string>
                {
                    { "用户名", username },
                    { "密码", userPassword },
                    { "上传者代称", uploaderName }
                };

                var response = await AuthHelper.SendAuthPostRequest("/user/register", regData);
                string responseText = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(responseText))
                {
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("success", out JsonElement successElement) && successElement.GetBoolean())
                    {
                        string userId = root.TryGetProperty("user_id", out JsonElement idElement) ? idElement.GetString() ?? "" : "";
                        MessageBox.Show($"注册成功！\n您的UserID: {userId}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        string error = root.TryGetProperty("error", out JsonElement errorElement) ? errorElement.GetString() : "注册失败";
                        MessageBox.Show($"注册失败: {error}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"注册失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                GO.Enabled = true;
                GO.Text = "注册";
            }
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void username_TextChanged(object sender, EventArgs e)
        {

        }

        private void password_TextChanged(object sender, EventArgs e)
        {

        }
    }
}