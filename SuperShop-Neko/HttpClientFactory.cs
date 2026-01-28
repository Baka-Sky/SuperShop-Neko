using System;
using System.Net.Http;

namespace SuperShop_Neko
{
    public static class HttpClientFactory
    {
        /// <summary>
        /// 创建新的HttpClient实例（推荐每次使用都创建新的）
        /// </summary>
        public static HttpClient CreateClient()
        {
            return new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }
    }
}