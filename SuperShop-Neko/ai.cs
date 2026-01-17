using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperShop_Neko
{
    public partial class ai : UserControl
    {
        // API配置 - 使用你提供的API密钥和Pro-128K模型
        private const string API_KEY = "FhWqRuRfyNxAnKNXsCLP:SabkfQBNZPLxLRvdApEG";
        private const string API_URL = "https://spark-api-open.xf-yun.com/v1/chat/completions";
        private const string MODEL = "pro-128k"; // Pro-128K版本

        // HTTP客户端
        private HttpClient httpClient;
        // 对话历史
        private List<ChatMessage> chatHistory = new List<ChatMessage>();

        public ai()
        {
            InitializeComponent();
            InitializeHttpClient();
            SetupUI();
        }

        private void InitializeHttpClient()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        private void SetupUI()
        {
            // 设置文本框样式

        }

        private async void send_Click(object sender, EventArgs e)
        {
            string userMessage = input.Text.Trim();

            if (string.IsNullOrEmpty(userMessage))
            {
                MessageBox.Show("请输入消息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 禁用按钮和输入框，防止重复发送
                send.Enabled = false;
                input.Enabled = false;

                // 在输出框显示用户消息
                AddMessageToOutput($"[{DateTime.Now:HH:mm:ss}] 用户", userMessage);

                // 清空输入框
                input.Clear();

                // 显示等待提示
                AddMessageToOutput($"[{DateTime.Now:HH:mm:ss}] NekoAI", "正在思考中...");

                // 获取AI回复
                string aiResponse = await GetAIResponseAsync(userMessage);

                // 移除等待提示，显示实际回复
                output.Text = output.Text.Replace("正在思考中...", "");
                AddMessageToOutput($"[{DateTime.Now:HH:mm:ss}] NekoAI", aiResponse);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"发送失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddMessageToOutput($"[{DateTime.Now:HH:mm:ss}] 系统", $"错误: {ex.Message}");
            }
            finally
            {
                // 重新启用按钮和输入框
                send.Enabled = true;
                input.Enabled = true;
                input.Focus();
            }
        }

        private void AddMessageToOutput(string sender, string message)
        {
            // 使用换行符分隔不同消息
            if (!string.IsNullOrEmpty(output.Text))
            {
                output.AppendText(Environment.NewLine);
            }

            // 添加发送者和时间
            output.AppendText($"{sender}" + Environment.NewLine);

            // 添加消息内容（添加缩进）
            string[] lines = message.Split('\n');
            foreach (string line in lines)
            {
                output.AppendText($"    {line}" + Environment.NewLine);
            }

            // 添加分隔线
            output.AppendText(new string('-', 50) + Environment.NewLine);
        }

        private async Task<string> GetAIResponseAsync(string userMessage)
        {
            try
            {
                // 添加用户消息到历史
                chatHistory.Add(new ChatMessage { role = "user", content = userMessage });

                // 构建请求数据
                var requestData = new
                {
                    model = MODEL,
                    messages = chatHistory.ToArray(),
                    temperature = 0.7,
                    max_tokens = 4096, // Pro-128K最大输出4K tokens
                    stream = false
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };

                string json = JsonSerializer.Serialize(requestData, jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // 发送请求
                var response = await httpClient.PostAsync(API_URL, content);
                response.EnsureSuccessStatusCode();

                // 解析响应
                string responseJson = await response.Content.ReadAsStringAsync();
                var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);

                // 检查API返回状态
                if (responseObj.TryGetProperty("code", out var codeElement) &&
                    codeElement.GetInt32() != 0)
                {
                    string errorMsg = responseObj.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "未知错误";
                    throw new Exception($"API错误: {errorMsg}");
                }

                // 提取AI回复
                if (responseObj.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0)
                {
                    var firstChoice = choices[0];
                    if (firstChoice.TryGetProperty("message", out var messageElement) &&
                        messageElement.TryGetProperty("content", out var contentElement))
                    {
                        string aiResponse = contentElement.GetString();

                        // 添加AI回复到历史
                        chatHistory.Add(new ChatMessage { role = "assistant", content = aiResponse });

                        // 保持历史记录在一定范围内（可选，避免token超限）
                        if (chatHistory.Count > 10) // 保留最近10轮对话
                        {
                            chatHistory.RemoveAt(0);
                            chatHistory.RemoveAt(0); // 移除一对对话
                        }

                        return aiResponse;
                    }
                }

                throw new Exception("无法解析AI回复");
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception($"网络请求失败: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                throw new Exception($"数据解析失败: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"获取AI回复失败: {ex.Message}");
            }
        }

        // 聊天消息类
        private class ChatMessage
        {
            public string role { get; set; }
            public string content { get; set; }
        }

        // 处理输入框回车键发送
        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true; 
                send.PerformClick();
            }
        }

        // 清空对话按钮（如果需要可以添加）
        private void btnClear_Click(object sender, EventArgs e)
        {
            output.Clear();
            chatHistory.Clear();
        }


    }
}