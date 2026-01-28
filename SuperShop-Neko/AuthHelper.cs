using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SuperShop_Neko
{
    public static class AuthHelper
    {
        // 客户端配置（必须与Python后端完全一致）
        private const string CLIENT_NAME = "SuperShop_Neko";
        private const string CLIENT_ID = "supershop_neko_client_v1";
        private const string SECRET_SALT = "baka233_supershop_secret_2024";
        private const string API_BASE_URL = "http://171.80.1.4:25568";

        /// <summary>
        /// 生成客户端令牌
        /// </summary>
        public static string GenerateClientToken(string timestamp)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string rawString = $"{CLIENT_ID}_{timestamp}_{SECRET_SALT}";
                byte[] bytes = Encoding.UTF8.GetBytes(rawString);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// 获取当前时间戳（Unix时间戳，秒）
        /// </summary>
        public static string GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        /// <summary>
        /// 创建带有鉴权头的HTTP请求
        /// </summary>
        public static HttpRequestMessage CreateAuthRequest(HttpMethod method, string url, string contentJson = null)
        {
            var request = new HttpRequestMessage(method, url);

            // 添加鉴权头
            string timestamp = GetCurrentTimestamp();
            string token = GenerateClientToken(timestamp);

            request.Headers.Add("X-Client-ID", CLIENT_ID);
            request.Headers.Add("X-Client-Token", token);
            request.Headers.Add("X-Timestamp", timestamp);

            // 添加JSON内容（如果需要）
            if (!string.IsNullOrEmpty(contentJson))
            {
                request.Content = new StringContent(contentJson, Encoding.UTF8, "application/json");
            }

            return request;
        }

        /// <summary>
        /// 验证客户端连接
        /// </summary>
        public static async Task<bool> VerifyClientConnection()
        {
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                try
                {
                    // 先测试健康检查
                    var healthResponse = await httpClient.GetAsync($"{API_BASE_URL}/health");
                    if (!healthResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("健康检查失败");
                        return false;
                    }

                    // 验证客户端
                    var verifyData = new Dictionary<string, string>
                    {
                        { "client_name", CLIENT_NAME }
                    };

                    string json = JsonSerializer.Serialize(verifyData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync($"{API_BASE_URL}/client/verify", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseText = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"客户端验证响应: {responseText}");

                        using (JsonDocument doc = JsonDocument.Parse(responseText))
                        {
                            JsonElement root = doc.RootElement;
                            return root.TryGetProperty("success", out JsonElement successElement) &&
                                   successElement.GetBoolean();
                        }
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"客户端验证异常: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 发送带鉴权的POST请求
        /// </summary>
        public static async Task<HttpResponseMessage> SendAuthPostRequest(string endpoint, object data)
        {
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                string json = JsonSerializer.Serialize(data);
                var request = CreateAuthRequest(HttpMethod.Post, $"{API_BASE_URL}{endpoint}", json);
                return await httpClient.SendAsync(request);
            }
        }

        /// <summary>
        /// 发送带鉴权的GET请求
        /// </summary>
        public static async Task<HttpResponseMessage> SendAuthGetRequest(string endpoint)
        {
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                var request = CreateAuthRequest(HttpMethod.Get, $"{API_BASE_URL}{endpoint}");
                return await httpClient.SendAsync(request);
            }
        }

        /// <summary>
        /// 发送带鉴权的POST请求（简单版本，直接发送JSON字符串）
        /// </summary>
        public static async Task<HttpResponseMessage> SendAuthPost(string endpoint, string jsonData)
        {
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                var request = CreateAuthRequest(HttpMethod.Post, $"{API_BASE_URL}{endpoint}", jsonData);
                return await httpClient.SendAsync(request);
            }
        }
    }
}