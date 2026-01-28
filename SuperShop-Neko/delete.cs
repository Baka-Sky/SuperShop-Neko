using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;

namespace SuperShop_Neko
{
    public partial class delete : UserControl
    {
        // 添加事件
        public event EventHandler DeleteCompleted;

        public delete()
        {
            InitializeComponent();
        }

        private async void del_Click(object sender, EventArgs e)
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
                // 显示加载状态
                Cursor = Cursors.WaitCursor;
                del.Enabled = false;

                // 准备数据
                var requestData = new Dictionary<string, string>
                {
                    { "软件名", app.Text.Trim() }
                };

                // 发送带鉴权的POST请求
                var response = await AuthHelper.SendAuthPostRequest("/delete_app", requestData);

                // 处理响应
                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"删除响应: {responseText}");

                    // 使用JsonDocument解析响应
                    using (JsonDocument doc = JsonDocument.Parse(responseText))
                    {
                        JsonElement root = doc.RootElement;
                        bool success = root.TryGetProperty("success", out JsonElement successElement) && successElement.GetBoolean();

                        if (success)
                        {
                            MessageBox.Show($"软件 '{app.Text}' 已成功删除！", "成功",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // 清空输入框
                            app.Clear();

                            // 触发删除完成事件
                            DeleteCompleted?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            string error = root.TryGetProperty("error", out JsonElement errorElement)
                                ? errorElement.GetString()
                                : "未知错误";
                            if (root.TryGetProperty("error_code", out JsonElement errorCodeElement) &&
                                errorCodeElement.GetString() == "APP_NOT_FOUND")
                            {
                                MessageBox.Show($"软件 '{app.Text}' 不存在！", "删除失败",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                MessageBox.Show($"删除失败: {error}", "错误",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    string errorMessage = $"API请求失败: {response.StatusCode}";

                    try
                    {
                        using (JsonDocument doc = JsonDocument.Parse(errorText))
                        {
                            JsonElement root = doc.RootElement;
                            if (root.TryGetProperty("error", out JsonElement errorElement))
                            {
                                errorMessage = errorElement.GetString();
                            }
                        }
                    }
                    catch
                    {
                        errorMessage += $"\n响应: {errorText}";
                    }

                    MessageBox.Show(errorMessage, "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 恢复控件状态
                Cursor = Cursors.Default;
                del.Enabled = true;
            }
        }

        // 按Enter键触发删除
        private void app_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                del_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            // 保持原有的panel1点击事件
        }
    }
}