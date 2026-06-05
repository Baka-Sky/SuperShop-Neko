using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Linq;

namespace SuperShop_Neko
{
    public static class AuthHelper
    {
        // API配置
        private const string API_BASE_URL = "http://api.baka233.top:25568";

        // 动态会话相关
        private static string _sessionId = null;
        private static string _secretKey = null;
        private static DateTime _sessionExpireTime = DateTime.MinValue;
        private static readonly object _sessionLock = new object();

        /// <summary>
        /// 获取当前时间戳（Unix时间戳，秒）
        /// </summary>
        public static string GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        /// <summary>
        /// 生成请求签名
        /// </summary>
        private static string GenerateSignature(Dictionary<string, object> parameters, string secretKey, string timestamp)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // 将参数排序后拼接
                var sortedParams = parameters != null
                    ? parameters.OrderBy(x => x.Key).ToList()
                    : new List<KeyValuePair<string, object>>();

                string signStr = secretKey + timestamp;
                foreach (var kv in sortedParams)
                {
                    signStr += $"{kv.Key}{kv.Value?.ToString() ?? ""}";
                }

                byte[] bytes = Encoding.UTF8.GetBytes(signStr);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// 确保有有效的会话（自动获取或刷新）
        /// </summary>
        private static async Task<bool> EnsureSessionAsync()
        {
            lock (_sessionLock)
            {
                // 如果会话有效且未过期，直接返回
                if (!string.IsNullOrEmpty(_sessionId) &&
                    !string.IsNullOrEmpty(_secretKey) &&
                    DateTime.Now < _sessionExpireTime)
                {
                    return true;
                }
            }

            // 会话无效或已过期，重新获取
            return await RefreshSessionAsync();
        }

        /// <summary>
        /// 刷新/获取新的会话密钥
        /// </summary>
        public static async Task<bool> RefreshSessionAsync()
        {
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                try
                {
                    Console.WriteLine("[AuthHelper] 正在获取新的会话密钥...");

                    var response = await httpClient.PostAsync(
                        $"{API_BASE_URL}/auth/get_session",
                        new StringContent("{}", Encoding.UTF8, "application/json")
                    );

                    if (response.IsSuccessStatusCode)
                    {
                        string responseText = await response.Content.ReadAsStringAsync();
                        using (JsonDocument doc = JsonDocument.Parse(responseText))
                        {
                            JsonElement root = doc.RootElement;
                            if (root.TryGetProperty("success", out JsonElement successElement) &&
                                successElement.GetBoolean())
                            {
                                _sessionId = root.GetProperty("session_id").GetString();
                                _secretKey = root.GetProperty("secret_key").GetString();
                                long expireTimestamp = root.GetProperty("expire").GetInt64();
                                _sessionExpireTime = DateTimeOffset.FromUnixTimeSeconds(expireTimestamp).LocalDateTime;

                                Console.WriteLine($"[AuthHelper] 会话密钥获取成功，过期时间: {_sessionExpireTime:yyyy-MM-dd HH:mm:ss}");
                                return true;
                            }
                            else
                            {
                                string error = root.TryGetProperty("error", out JsonElement errorElement)
                                    ? errorElement.GetString()
                                    : "未知错误";
                                Console.WriteLine($"[AuthHelper] 获取会话密钥失败: {error}");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[AuthHelper] 获取会话密钥HTTP错误: {response.StatusCode}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AuthHelper] 获取会话密钥异常: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 关闭当前会话
        /// </summary>
        public static async Task CloseSessionAsync()
        {
            if (string.IsNullOrEmpty(_sessionId))
                return;

            using (var httpClient = HttpClientFactory.CreateClient())
            {
                try
                {
                    var data = new Dictionary<string, string>
                    {
                        { "session_id", _sessionId }
                    };
                    string json = JsonSerializer.Serialize(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    await httpClient.PostAsync($"{API_BASE_URL}/auth/close_session", content);
                    Console.WriteLine("[AuthHelper] 会话已关闭");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AuthHelper] 关闭会话异常: {ex.Message}");
                }
                finally
                {
                    _sessionId = null;
                    _secretKey = null;
                    _sessionExpireTime = DateTime.MinValue;
                }
            }
        }

        /// <summary>
        /// 创建带有动态鉴权头的HTTP请求
        /// </summary>
        private static async Task<HttpRequestMessage> CreateAuthRequestAsync(HttpMethod method, string url, string contentJson = null)
        {
            // 确保有有效的会话
            if (!await EnsureSessionAsync())
            {
                throw new Exception("无法获取有效的会话密钥，请检查网络连接");
            }

            var request = new HttpRequestMessage(method, url);

            // 获取请求参数用于签名
            Dictionary<string, object> parameters = null;
            if (!string.IsNullOrEmpty(contentJson))
            {
                try
                {
                    parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(contentJson);
                }
                catch
                {
                    parameters = new Dictionary<string, object>();
                }
            }

            // 生成签名
            string timestamp = GetCurrentTimestamp();
            string signature = GenerateSignature(parameters, _secretKey, timestamp);

            // 添加鉴权头
            request.Headers.Add("X-Session-Id", _sessionId);
            request.Headers.Add("X-Timestamp", timestamp);
            request.Headers.Add("X-Signature", signature);

            // 添加JSON内容（如果需要）
            if (!string.IsNullOrEmpty(contentJson))
            {
                request.Content = new StringContent(contentJson, Encoding.UTF8, "application/json");
            }

            return request;
        }

        /// <summary>
        /// 发送带动态鉴权的POST请求
        /// </summary>
        public static async Task<HttpResponseMessage> SendAuthPostRequest(string endpoint, object data)
        {
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                string json = JsonSerializer.Serialize(data);
                var request = await CreateAuthRequestAsync(HttpMethod.Post, $"{API_BASE_URL}{endpoint}", json);
                return await httpClient.SendAsync(request);
            }
        }

        /// <summary>
        /// 发送带动态鉴权的GET请求
        /// </summary>
        public static async Task<HttpResponseMessage> SendAuthGetRequest(string endpoint)
        {
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                var request = await CreateAuthRequestAsync(HttpMethod.Get, $"{API_BASE_URL}{endpoint}");
                return await httpClient.SendAsync(request);
            }
        }

        /// <summary>
        /// 发送带动态鉴权的POST请求（简单版本）
        /// </summary>
        public static async Task<HttpResponseMessage> SendAuthPost(string endpoint, string jsonData)
        {
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                var request = await CreateAuthRequestAsync(HttpMethod.Post, $"{API_BASE_URL}{endpoint}", jsonData);
                return await httpClient.SendAsync(request);
            }
        }

        /// <summary>
        /// 使用后门发送请求（绕过鉴权，用于调试）
        /// </summary>
        public static async Task<HttpResponseMessage> SendBackdoorPostRequest(string endpoint, object data, string devPassword = "jianghao0523")
        {
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                string json = JsonSerializer.Serialize(data);
                var request = new HttpRequestMessage(HttpMethod.Post, $"{API_BASE_URL}{endpoint}")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("X-Dev-Password", devPassword);
                return await httpClient.SendAsync(request);
            }
        }

        /// <summary>
        /// 测试API连接（健康检查）
        /// </summary>
        public static async Task<bool> TestConnectionAsync()
        {
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                try
                {
                    var response = await httpClient.GetAsync($"{API_BASE_URL}/health");
                    Console.WriteLine($"[AuthHelper] 健康检查: {response.StatusCode}");
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AuthHelper] 连接测试失败: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 检查会话是否有效
        /// </summary>
        public static bool IsSessionValid()
        {
            lock (_sessionLock)
            {
                return !string.IsNullOrEmpty(_sessionId) &&
                       !string.IsNullOrEmpty(_secretKey) &&
                       DateTime.Now < _sessionExpireTime;
            }
        }

        /// <summary>
        /// 获取会话剩余秒数
        /// </summary>
        public static int GetSessionRemainingSeconds()
        {
            lock (_sessionLock)
            {
                if (string.IsNullOrEmpty(_sessionId) || DateTime.Now >= _sessionExpireTime)
                    return 0;
                return (int)(_sessionExpireTime - DateTime.Now).TotalSeconds;
            }
        }
    }
}