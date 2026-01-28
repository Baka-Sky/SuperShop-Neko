using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using System.Threading.Tasks;

namespace SuperShop_Neko
{
    public partial class upload : UserControl
    {
        public event EventHandler UploadCompleted;

        public upload()
        {
            InitializeComponent();
        }

        // 检查软件名是否重复
        private async Task<bool> CheckDuplicateName(string softwareName)
        {
            if (string.IsNullOrWhiteSpace(softwareName))
                return false;

            try
            {
                var checkData = new Dictionary<string, string>
                {
                    { "软件名", softwareName.Trim() }
                };

                var response = await AuthHelper.SendAuthPostRequest("/check_duplicate", checkData);

                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(responseText))
                    {
                        JsonElement root = doc.RootElement;
                        if (root.TryGetProperty("success", out JsonElement successElement) &&
                            successElement.GetBoolean())
                        {
                            if (root.TryGetProperty("exists", out JsonElement existsElement))
                            {
                                return existsElement.GetBoolean();
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

        // 搜索相似的软件名
        private async Task<List<string>> SearchSimilarNames(string softwareName)
        {
            var similarNames = new List<string>();

            if (string.IsNullOrWhiteSpace(softwareName))
                return similarNames;

            try
            {
                var searchData = new Dictionary<string, string>
                {
                    { "软件名", softwareName.Trim() }
                };

                var response = await AuthHelper.SendAuthPostRequest("/search_similar", searchData);

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
                                    if (!string.IsNullOrEmpty(name))
                                        similarNames.Add(name);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // 忽略搜索错误
            }

            return similarNames;
        }

        private async void load_Click(object sender, EventArgs e)
        {
            string softwareName = app.Text.Trim();

            // 检查必填字段是否为空
            if (string.IsNullOrWhiteSpace(softwareName))
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

            try
            {
                // 显示加载状态
                Cursor = Cursors.WaitCursor;
                load.Enabled = false;

                // 第一步：检查软件名是否重复
                bool isDuplicate = await CheckDuplicateName(softwareName);
                if (isDuplicate)
                {
                    // 搜索相似的软件名，给用户提示
                    var similarNames = await SearchSimilarNames(softwareName);

                    string errorMessage = $"软件名 '{softwareName}' 已存在！";

                    if (similarNames.Count > 0)
                    {
                        errorMessage += "\n\n相似的软件名：";
                        foreach (var name in similarNames)
                        {
                            errorMessage += $"\n• {name}";
                        }
                        errorMessage += "\n\n请修改软件名或使用其他名称。";
                    }
                    else
                    {
                        errorMessage += "\n\n请使用其他软件名。";
                    }

                    MessageBox.Show(errorMessage, "名称重复",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    app.Focus();
                    app.SelectAll();
                    return;
                }

                // 第二步：如果名称不重复，执行上传
                var requestData = new Dictionary<string, string>
                {
                    { "软件名", softwareName },
                    { "链接", dwntext.Text.Trim() },
                    { "上传者", who.Text.Trim() },
                    { "出处", by.Text.Trim() }
                };

                var response = await AuthHelper.SendAuthPostRequest("/add_app", requestData);

                // 处理响应
                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(responseText))
                    {
                        JsonElement root = doc.RootElement;
                        bool success = root.TryGetProperty("success", out JsonElement successElement) &&
                                       successElement.GetBoolean();

                        if (success)
                        {
                            MessageBox.Show("上传成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // 清空输入框
                            app.Clear();
                            dwntext.Clear();
                            who.Clear();
                            by.Clear();

                            // 触发上传完成事件
                            UploadCompleted?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            string error = root.TryGetProperty("error", out JsonElement errorElement)
                                ? errorElement.GetString()
                                : "未知错误";

                            if (root.TryGetProperty("error_code", out JsonElement errorCodeElement))
                            {
                                string errorCode = errorCodeElement.GetString();
                                if (errorCode == "DUPLICATE_NAME" || errorCode == "DUPLICATE_ENTRY")
                                {
                                    // 再次检查（以防万一）
                                    MessageBox.Show($"软件名 '{softwareName}' 已存在！\n\n请使用其他名称。",
                                        "名称重复", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    app.Focus();
                                    app.SelectAll();
                                }
                                else
                                {
                                    MessageBox.Show($"上传失败: {error}", "错误",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show($"上传失败: {error}", "错误",
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
                Cursor = Cursors.Default;
                load.Enabled = true;
            }
        }

        // 实时检查软件名是否重复（在文本框失去焦点时）
        private async void app_Leave(object sender, EventArgs e)
        {
            string softwareName = app.Text.Trim();
            if (string.IsNullOrWhiteSpace(softwareName))
                return;

            try
            {
                // 显示检查中...
                Cursor = Cursors.WaitCursor;

                bool isDuplicate = await CheckDuplicateName(softwareName);

                if (isDuplicate)
                {
                    // 设置文本框背景色为警告色
                    app.BackColor = Color.LightPink;

                    // 搜索相似的软件名
                    var similarNames = await SearchSimilarNames(softwareName);

                    string tooltipText = $"软件名 '{softwareName}' 已存在！";
                    if (similarNames.Count > 0)
                    {
                        tooltipText += "\n相似的软件名：";
                        foreach (var name in similarNames)
                        {
                            tooltipText += $"\n{name}";
                        }
                    }

                    // 可以添加一个ToolTip控件来显示提示
                    MessageBox.Show(tooltipText, "名称重复警告",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    app.BackColor = SystemColors.Window;
                }
            }
            catch
            {
                // 忽略检查错误
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void upload_Load(object sender, EventArgs e)
        {
            // 初始化代码
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            // 保持原有的panel2点击事件
        }
    }
}