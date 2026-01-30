using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace SuperShop_Neko
{
    public partial class delete : UserControl
    {
        // 添加事件
        public event EventHandler DeleteCompleted;

        // 当前用户信息
        private string currentUserUploaderName = "";
        private bool isSuperUser = false;

        public delete()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // 从配置文件加载当前用户信息
            LoadCurrentUserInfo();

            // 根据登录状态更新UI
            UpdateUIState();
        }

        // 从配置文件加载当前用户信息
        private void LoadCurrentUserInfo()
        {
            try
            {
                // 配置文件路径
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                if (File.Exists(configPath))
                {
                    string jsonText = File.ReadAllText(configPath);
                    using (JsonDocument doc = JsonDocument.Parse(jsonText))
                    {
                        JsonElement root = doc.RootElement;

                        // 获取上传者代称
                        if (root.TryGetProperty("uploader_name", out JsonElement uploaderElement))
                        {
                            currentUserUploaderName = uploaderElement.GetString() ?? "";
                        }
                        else if (root.TryGetProperty("上传者代称", out JsonElement chineseNameElement))
                        {
                            currentUserUploaderName = chineseNameElement.GetString() ?? "";
                        }

                        // 获取用户权限
                        if (root.TryGetProperty("su", out JsonElement suElement))
                        {
                            string suValue = suElement.GetString() ?? "None";
                            isSuperUser = (suValue == "Super");
                        }
                        else if (root.TryGetProperty("Su", out JsonElement suElement2))
                        {
                            string suValue = suElement2.GetString() ?? "None";
                            isSuperUser = (suValue == "Super");
                        }
                    }
                }

                Console.WriteLine($"删除功能 - 当前用户: {currentUserUploaderName}, 管理员: {isSuperUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取用户信息失败: {ex.Message}");
            }
        }

        // 更新UI状态
        private void UpdateUIState()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateUIState));
                return;
            }

            bool isLoggedIn = !string.IsNullOrEmpty(currentUserUploaderName);

            if (isLoggedIn)
            {
                // 已登录状态
                del.Enabled = true;
                del.Text = "删除";
                app.Enabled = true;
                app.PlaceholderText = "请输入要删除的软件名";

                // 根据权限显示提示
                if (isSuperUser)
                {
                    label1.Text = "管理员模式：可删除所有软件";
                    label1.ForeColor = Color.Red;
                }
                else
                {
                    // 用户模式保持原来的提示词（比如删除软件的提示）
                    // 这里不修改label1.Text，保持原来的值
                    if (string.IsNullOrEmpty(label1.Text) || label1.Text.Contains("请先登录"))
                    {
                        label1.Text = "删除软件";
                    }
                    label1.ForeColor = SystemColors.ControlText;
                }
            }
            else
            {
                // 未登录状态
                del.Enabled = false;
                del.Text = "请先登录";
                app.Enabled = false;
                app.Text = "";
                app.PlaceholderText = "请先登录";
                label1.Text = "请先登录以使用删除功能";
                label1.ForeColor = Color.Gray;
            }
        }

        // 检查软件是否由当前用户上传
        private async Task<bool> IsAppUploadedByCurrentUser(string softwareName)
        {
            try
            {
                var requestData = new Dictionary<string, string>
                {
                    { "软件名", softwareName.Trim() }
                };

                // 发送请求查询软件信息
                var response = await AuthHelper.SendAuthPostRequest("/search_similar", requestData);

                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(responseText))
                    {
                        JsonElement root = doc.RootElement;
                        if (root.TryGetProperty("success", out JsonElement successElement) &&
                            successElement.GetBoolean() &&
                            root.TryGetProperty("results", out JsonElement resultsElement))
                        {
                            foreach (JsonElement item in resultsElement.EnumerateArray())
                            {
                                if (item.TryGetProperty("软件名", out JsonElement nameElement))
                                {
                                    string name = nameElement.GetString();
                                    if (name == softwareName && item.TryGetProperty("上传者", out JsonElement uploaderElement))
                                    {
                                        string uploader = uploaderElement.GetString();
                                        return uploader == currentUserUploaderName;
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // 获取软件的详细信息
        private async Task<Dictionary<string, string>> GetAppDetails(string softwareName)
        {
            try
            {
                var requestData = new Dictionary<string, string>
                {
                    { "软件名", softwareName.Trim() }
                };

                // 发送请求查询软件信息
                var response = await AuthHelper.SendAuthPostRequest("/search_similar", requestData);

                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(responseText))
                    {
                        JsonElement root = doc.RootElement;
                        if (root.TryGetProperty("success", out JsonElement successElement) &&
                            successElement.GetBoolean() &&
                            root.TryGetProperty("results", out JsonElement resultsElement))
                        {
                            foreach (JsonElement item in resultsElement.EnumerateArray())
                            {
                                if (item.TryGetProperty("软件名", out JsonElement nameElement))
                                {
                                    string name = nameElement.GetString();
                                    if (name == softwareName)
                                    {
                                        var details = new Dictionary<string, string>
                                        {
                                            { "软件名", name }
                                        };

                                        if (item.TryGetProperty("上传者", out JsonElement uploaderElement))
                                        {
                                            details["上传者"] = uploaderElement.GetString() ?? "";
                                        }

                                        if (item.TryGetProperty("出处", out JsonElement sourceElement))
                                        {
                                            details["出处"] = sourceElement.GetString() ?? "";
                                        }

                                        if (item.TryGetProperty("链接", out JsonElement linkElement))
                                        {
                                            details["链接"] = linkElement.GetString() ?? "";
                                        }

                                        return details;
                                    }
                                }
                            }
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private async void del_Click(object sender, EventArgs e)
        {
            // 每次点击时重新加载用户信息
            LoadCurrentUserInfo();

            // 检查是否已登录
            if (string.IsNullOrEmpty(currentUserUploaderName))
            {
                MessageBox.Show("请先登录后再操作！", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateUIState();
                return;
            }

            string softwareName = app.Text.Trim();

            // 检查输入框是否为空
            if (string.IsNullOrWhiteSpace(softwareName))
            {
                MessageBox.Show("请输入要删除的软件名！", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                app.Focus();
                return;
            }

            try
            {
                // 显示加载状态
                Cursor = Cursors.WaitCursor;
                del.Enabled = false;

                // 获取软件详细信息
                var appDetails = await GetAppDetails(softwareName);

                if (appDetails == null)
                {
                    MessageBox.Show($"软件 '{softwareName}' 不存在！", "删除失败",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    del.Enabled = true;
                    return;
                }

                string appUploader = appDetails.ContainsKey("上传者") ? appDetails["上传者"] : "";
                string appSource = appDetails.ContainsKey("出处") ? appDetails["出处"] : "";
                string appLink = appDetails.ContainsKey("链接") ? appDetails["链接"] : "";

                // 检查删除权限
                bool canDelete = false;
                string warningMessage = "";

                if (isSuperUser)
                {
                    // 管理员可以删除所有软件
                    canDelete = true;
                    warningMessage = $"您正在以管理员身份删除软件 '{softwareName}'。\n\n";
                }
                else if (appUploader == currentUserUploaderName)
                {
                    // 普通用户可以删除自己上传的软件
                    canDelete = true;
                    warningMessage = $"您正在删除自己上传的软件 '{softwareName}'。\n\n";
                }
                else
                {
                    // 普通用户不能删除他人上传的软件
                    MessageBox.Show($"您没有权限删除软件 '{softwareName}'！\n\n" +
                                  $"该软件由 '{appUploader}' 上传，\n" +
                                  $"您只能删除自己上传的软件。",
                                  "权限不足",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    del.Enabled = true;
                    return;
                }

                // 显示详细信息
                string shortLink = (appLink.Length > 50) ? appLink.Substring(0, 50) + "..." : appLink;

                string detailsMessage = $"软件名: {softwareName}\n" +
                                      $"上传者: {appUploader}\n" +
                                      $"出处: {appSource}\n" +
                                      $"链接: {shortLink}";

                string fullMessage = warningMessage +
                                   "软件详细信息：\n" + detailsMessage + "\n\n" +
                                   "确定要删除吗？";

                // 确认删除操作
                DialogResult result = MessageBox.Show(
                    fullMessage,
                    "确认删除",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result != DialogResult.Yes)
                {
                    del.Enabled = true;
                    return;
                }

                // 准备删除数据
                var requestData = new Dictionary<string, string>
                {
                    { "软件名", softwareName }
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
                            string userType = isSuperUser ? "管理员" : "用户";
                            MessageBox.Show($"{userType}已成功删除软件 '{softwareName}'！", "成功",
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
                                MessageBox.Show($"软件 '{softwareName}' 不存在！", "删除失败",
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
                UpdateUIState();
            }
        }

        // 按Enter键触发删除
        private void app_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (del.Enabled)
                {
                    del_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("请先登录后再操作！", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            // 保持原有的panel1点击事件
        }

        // 提供一个方法，用于更新用户信息
        public void UpdateUserInfo(string uploaderName, bool isSuper)
        {
            currentUserUploaderName = uploaderName;
            isSuperUser = isSuper;

            Console.WriteLine($"删除功能 - 用户信息已更新: {currentUserUploaderName}, 管理员: {isSuperUser}");

            // 更新UI状态
            UpdateUIState();
        }

        // 刷新状态
        public void RefreshLoginStatus()
        {
            LoadCurrentUserInfo();
            UpdateUIState();
        }
    }
}