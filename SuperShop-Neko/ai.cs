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
        // API配置
        private const string API_KEY = "FhWqRuRfyNxAnKNXsCLP:SabkfQBNZPLxLRvdApEG";
        private const string API_URL = "https://spark-api-open.xf-yun.com/v1/chat/completions";
        private const string MODEL = "pro-128k"; // Pro-128K版本

        // HTTP客户端
        private HttpClient httpClient;
        // 对话历史
        private List<ChatMessage> chatHistory = new List<ChatMessage>();

        // 对话消息列表（用于存储格式化后的对话）
        private List<FormattedMessage> formattedMessages = new List<FormattedMessage>();

        public ai()
        {
            InitializeComponent();
            InitializeHttpClient();
            SetupUI();
            AddWelcomeMessage();
        }

        private void InitializeHttpClient()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        private void SetupUI()
        {
        }

        private void AddWelcomeMessage()
        {
            // 添加欢迎消息到格式化列表
            formattedMessages.Add(new FormattedMessage
            {
                Sender = "系统",
                Message = "==========================================",
                IsRightAligned = false,
                IsDecoration = true
            });

            formattedMessages.Add(new FormattedMessage
            {
                Sender = "系统",
                Message = "✨ NekoAI 已启动 ✨",
                IsRightAligned = false,
                IsDecoration = true
            });

            formattedMessages.Add(new FormattedMessage
            {
                Sender = "系统",
                Message = "==========================================",
                IsRightAligned = false,
                IsDecoration = true
            });

            formattedMessages.Add(new FormattedMessage
            {
                Sender = "系统",
                Message = "你好！我是来自异世界的NekoAI，有什么可以帮助你的吗？🐱",
                IsRightAligned = false
            });

            // 更新显示
            UpdateOutputDisplay();
        }

        private async void send_Click(object sender, EventArgs e)
        {
            string userMessage = input.Text.Trim();



            try
            {
                // 禁用按钮和输入框，防止重复发送
                send.Enabled = false;
                input.Enabled = false;

                // 添加用户消息到格式化列表（右对齐）
                formattedMessages.Add(new FormattedMessage
                {
                    Sender = "用户",
                    Message = userMessage,
                    IsRightAligned = true,
                    Time = DateTime.Now
                });

                UpdateOutputDisplay();

                // 清空输入框
                input.Clear();

                // 添加等待提示（左对齐）
                formattedMessages.Add(new FormattedMessage
                {
                    Sender = "NekoAI",
                    Message = "正在思考中...🐱",
                    IsRightAligned = false,
                    IsWaiting = true
                });

                UpdateOutputDisplay();

   
                // 获取AI回复
                string aiResponse = await GetAIResponseAsync(userMessage);

                // 移除等待提示
                formattedMessages.RemoveAll(m => m.IsWaiting);

                // 添加AI回复到格式化列表（左对齐）
                formattedMessages.Add(new FormattedMessage
                {
                    Sender = "NekoAI",
                    Message = aiResponse,
                    IsRightAligned = false,
                    Time = DateTime.Now
                });

                UpdateOutputDisplay();


            }
            catch (Exception ex)
            {
                // 移除等待提示
                formattedMessages.RemoveAll(m => m.IsWaiting);

                // 添加错误消息
                formattedMessages.Add(new FormattedMessage
                {
                    Sender = "系统",
                    Message = $"抱歉，出错了: {ex.Message}",
                    IsRightAligned = false
                });


            }
            finally
            {
                // 重新启用按钮和输入框
                send.Enabled = true;
                input.Enabled = true;
                input.Focus();
            }
        }

        private void UpdateOutputDisplay()
        {
            // 清空输出框
            output.Clear();

            // 重新生成格式化后的文本
            foreach (var msg in formattedMessages)
            {
                if (msg.IsDecoration)
                {
                    // 装饰性消息（居中对齐）
                    output.AppendText(CenterAlignText(msg.Message) + Environment.NewLine);
                }
                else
                {
                    // 添加时间戳和发送者
                    string timestamp = msg.Time.HasValue ? $"[{msg.Time.Value:HH:mm:ss}]" : "";
                    string senderInfo = $"{timestamp} {msg.Sender}";

                    if (msg.IsRightAligned)
                    {
                        // 用户消息（右对齐）
                        output.AppendText(RightAlignText(senderInfo) + Environment.NewLine);
                        output.AppendText(RightAlignText(msg.Message) + Environment.NewLine);
                    }
                    else
                    {
                        // AI消息（左对齐）
                        output.AppendText(senderInfo + Environment.NewLine);

                        // 将消息按行分割并添加缩进
                        string[] lines = msg.Message.Split('\n');
                        foreach (string line in lines)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                output.AppendText("    " + line + Environment.NewLine);
                            }
                        }
                    }

                    // 添加分隔线
                    output.AppendText(new string('─', 50) + Environment.NewLine);
                }
            }
        }

        private string RightAlignText(string text, int width = 60)
        {
            // 右对齐文本
            string[] lines = text.Split('\n');
            StringBuilder result = new StringBuilder();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) && line.Length == 0) continue;

                int spacesNeeded = width - line.Length;
                if (spacesNeeded < 0) spacesNeeded = 0;

                result.Append(new string(' ', spacesNeeded) + line + Environment.NewLine);
            }

            return result.ToString().TrimEnd();
        }

        private string CenterAlignText(string text, int width = 60)
        {
            // 居中对齐文本
            string[] lines = text.Split('\n');
            StringBuilder result = new StringBuilder();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) && line.Length == 0) continue;

                int spacesNeeded = (width - line.Length) / 2;
                if (spacesNeeded < 0) spacesNeeded = 0;

                result.Append(new string(' ', spacesNeeded) + line + Environment.NewLine);
            }

            return result.ToString().TrimEnd();
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
                    max_tokens = 2048,
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

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API错误: {response.StatusCode}");
                }

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

                        // 保持历史记录在一定范围内
                        if (chatHistory.Count > 10)
                        {
                            chatHistory.RemoveAt(0);
                            chatHistory.RemoveAt(0);
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

        // 格式化消息类（用于控制对齐）
        private class FormattedMessage
        {
            public string Sender { get; set; }
            public string Message { get; set; }
            public bool IsRightAligned { get; set; }
            public bool IsDecoration { get; set; } = false;
            public bool IsWaiting { get; set; } = false;
            public DateTime? Time { get; set; }
        }

        // 处理输入框回车键发送
        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (e.Shift)
                {
                    // Shift+Enter: 换行
                    int cursorPos = input.SelectionStart;
                    input.Text = input.Text.Insert(cursorPos, Environment.NewLine);
                    input.SelectionStart = cursorPos + Environment.NewLine.Length;
                }
                else
                {
                    // Enter: 发送消息
                    e.SuppressKeyPress = true;
                    send.PerformClick();
                }
            }
        }


        // 组件销毁时清理资源

    }
}