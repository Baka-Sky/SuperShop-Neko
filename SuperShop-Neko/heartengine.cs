using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;



namespace SuperShop_Neko
{
    public class heartengine : IDisposable
    {
        private readonly HttpClient _httpClient;

        public heartengine()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        }

        #region 版本信息功能

        /// <summary>
        /// 获取完整的版本信息
        /// </summary>
        public string GetVersionInfo()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("═══════════════════════════════════════════");
                sb.AppendLine("          超级小铺 Neko - 版本信息");
                sb.AppendLine("═══════════════════════════════════════════");
                sb.AppendLine();

                // 1. 软件版本号
                sb.AppendLine($"📦 软件版本号: {GetSoftwareVersion()}");

                // 2. 内核版本号
                sb.AppendLine($"⚙️  内核版本号: {GetCoreVersion()}");

                // 3. 软件构建时间戳
                sb.AppendLine($"🕐 软件构建时间: {GetBuildTimestamp()}");

                // 4. .NET版本号
                sb.AppendLine($"🔧 .NET 运行时: {GetDotNetVersion()}");

                // 5. WebView2版本号
                sb.AppendLine($"🌐 WebView2版本: {GetWebView2Version()}");

                // 6. C#语言版本
                sb.AppendLine($"💻 C# 语言版本: {GetCSharpVersion()}");

                // 7. 系统内核版本
                sb.AppendLine($"🖥️  系统内核版本: {GetWindowsKernelVersion()}");

                // 8. 软件运行目录
                sb.AppendLine($"📁 软件运行目录: {GetApplicationDirectory()}");

                // 9. 操作系统信息
                sb.AppendLine($"💿 操作系统: {GetOSInfo()}");

                // 10. 系统架构
                sb.AppendLine($"🏗️  系统架构: {GetSystemArchitecture()}");

                // 11. 内存信息
                sb.AppendLine($"💾 内存使用: {GetMemoryInfo()}");

                // 12. 处理器信息
                sb.AppendLine($"🚀 处理器: {GetProcessorInfo()}");

                sb.AppendLine();
                sb.AppendLine("═══════════════════════════════════════════");
                sb.AppendLine("                          HeartEngine4 & HeartCore1");
                sb.AppendLine("═══════════════════════════════════════════");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"❌ 获取版本信息失败:\n{ex.Message}";
            }
        }

        /// <summary>
        /// 获取软件版本号
        /// </summary>
        private string GetSoftwareVersion()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

                // 1. 优先获取文件版本（AssemblyFileVersion）
                var fileVersionAttr = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                if (fileVersionAttr != null && !string.IsNullOrEmpty(fileVersionAttr.Version))
                {
                    return fileVersionAttr.Version;
                }

                // 2. 获取程序集版本（AssemblyVersion）
                var version = assembly.GetName().Version;
                if (version != null)
                {
                    // 格式化版本号（如 1.2.3.4）
                    return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
                }

                // 3. 默认版本
                return "1.0.0.0";
            }
            catch
            {
                return "1.0.0.0";
            }
        }

        /// <summary>
        /// 获取内核版本号
        /// </summary>
        private string GetCoreVersion()
        {
            try
            {
                // 返回你的引擎版本
                return "HeartEngine4 & HeartCore1";
            }
            catch
            {
                return "HeartEngine4 & HeartCore1";
            }
        }

        /// <summary>
        /// 获取构建时间戳
        /// </summary>
        private string GetBuildTimestamp()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                var buildTime = File.GetLastWriteTime(assembly.Location);
                return buildTime.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
            catch
            {
                return "未知时间";
            }
        }

        /// <summary>
        /// 获取.NET版本
        /// </summary>
        private string GetDotNetVersion()
        {
            try
            {
                // 获取详细的.NET版本信息
                var version = Environment.Version;
                var description = RuntimeInformation.FrameworkDescription;
                return $"{description} (v{version})";
            }
            catch
            {
                return $"NET {Environment.Version}";
            }
        }

        /// <summary>
        /// 获取WebView2版本
        /// </summary>
        private string GetWebView2Version()
        {
            try
            {
                // 方法1: 通过注册表获取
                string[] registryPaths = new string[]
                {
                    @"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}",
                    @"SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}"
                };

                foreach (var path in registryPaths)
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path))
                    {
                        if (key != null)
                        {
                            var version = key.GetValue("pv")?.ToString();
                            if (!string.IsNullOrEmpty(version))
                                return version;
                        }
                    }
                }

                // 方法2: 通过安装路径检查
                string[] possiblePaths = new string[]
                {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft", "EdgeWebView", "Application"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft", "EdgeWebView", "Application"),
                    @"C:\Program Files (x86)\Microsoft\EdgeWebView\Application",
                    @"C:\Program Files\Microsoft\EdgeWebView\Application"
                };

                foreach (var path in possiblePaths)
                {
                    if (Directory.Exists(path))
                    {
                        var dirs = Directory.GetDirectories(path);
                        foreach (var dir in dirs)
                        {
                            var dirName = Path.GetFileName(dir);
                            if (System.Version.TryParse(dirName, out _))
                            {
                                return dirName;
                            }
                        }
                    }
                }

                return "未检测到WebView2";
            }
            catch
            {
                return "检测失败";
            }
        }

        /// <summary>
        /// 获取C#版本
        /// </summary>
        private string GetCSharpVersion()
        {
            try
            {
                // 根据.NET版本返回对应的C#版本
                var version = Environment.Version;

                if (version.Major >= 8) return "C# 12.0 (或更高)";
                if (version.Major >= 7) return "C# 11.0";
                if (version.Major >= 6) return "C# 10.0";
                if (version.Major >= 5) return "C# 9.0";
                if (version.Major == 4 && version.Minor >= 8) return "C# 7.3";
                if (version.Major == 4 && version.Minor >= 7) return "C# 7.2";
                if (version.Major == 4 && version.Minor >= 6) return "C# 7.0";

                return "C# (早期版本)";
            }
            catch
            {
                return "C# 版本未知";
            }
        }

        /// <summary>
        /// 获取Windows内核版本
        /// </summary>
        private string GetWindowsKernelVersion()
        {
            try
            {
                var osVersion = Environment.OSVersion;
                return $"Windows NT {osVersion.Version.Major}.{osVersion.Version.Minor} (Build {osVersion.Version.Build}.{osVersion.Version.Revision})";
            }
            catch
            {
                return "Windows NT 未知版本";
            }
        }

        /// <summary>
        /// 获取应用程序运行目录
        /// </summary>
        private string GetApplicationDirectory()
        {
            try
            {
                var directory = AppDomain.CurrentDomain.BaseDirectory;
                // 如果路径太长，可以截断显示
                if (directory.Length > 60)
                {
                    return "..." + directory.Substring(directory.Length - 50);
                }
                return directory;
            }
            catch
            {
                return "未知目录";
            }
        }

        /// <summary>
        /// 获取操作系统信息
        /// </summary>
        private string GetOSInfo()
        {
            try
            {
                string osName = "Windows";

                // 尝试获取更详细的Windows版本
                if (Environment.OSVersion.Version.Major == 10)
                {
                    if (Environment.OSVersion.Version.Build >= 22000) osName = "Windows 11";
                    else osName = "Windows 10";
                }
                else if (Environment.OSVersion.Version.Major == 6)
                {
                    if (Environment.OSVersion.Version.Minor == 3) osName = "Windows 8.1";
                    else if (Environment.OSVersion.Version.Minor == 2) osName = "Windows 8";
                    else if (Environment.OSVersion.Version.Minor == 1) osName = "Windows 7";
                    else if (Environment.OSVersion.Version.Minor == 0) osName = "Windows Vista";
                }

                return $"{osName} {Environment.OSVersion.Version} {(Environment.Is64BitOperatingSystem ? "64位" : "32位")}";
            }
            catch
            {
                return "未知操作系统";
            }
        }

        /// <summary>
        /// 获取系统架构
        /// </summary>
        private string GetSystemArchitecture()
        {
            try
            {
                return $"{RuntimeInformation.OSArchitecture} 架构";
            }
            catch
            {
                return Environment.Is64BitOperatingSystem ? "x64" : "x86";
            }
        }

        /// <summary>
        /// 获取内存信息
        /// </summary>
        private string GetMemoryInfo()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var usedMemoryMB = process.WorkingSet64 / (1024 * 1024);
                var privateMemoryMB = process.PrivateMemorySize64 / (1024 * 1024);

                return $"工作集: {usedMemoryMB} MB | 私有内存: {privateMemoryMB} MB";
            }
            catch
            {
                return "内存信息不可用";
            }
        }

        /// <summary>
        /// 获取处理器信息
        /// </summary>
        private string GetProcessorInfo()
        {
            try
            {
                return $"{Environment.ProcessorCount} 个逻辑处理器 | {(Environment.Is64BitProcess ? "64位" : "32位")} 进程";
            }
            catch
            {
                return $"{Environment.ProcessorCount} 核心";
            }
        }

        #endregion

        #region 天气服务功能

        /// <summary>
        /// 根据城市名称获取天气信息
        /// </summary>
        public async Task<string[]> GetWeatherByCityName(string cityName)
        {
            try
            {
                string soapRequest = $"""
                    <?xml version="1.0" encoding="utf-8"?>
                    <soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
                                   xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
                                   xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
                      <soap:Body>
                        <getWeatherbyCityName xmlns="http://WebXml.com.cn/">
                          <theCityName>{EscapeXml(cityName)}</theCityName>
                        </getWeatherbyCityName>
                      </soap:Body>
                    </soap:Envelope>
                    """;

                var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
                content.Headers.Add("SOAPAction", "http://WebXml.com.cn/getWeatherbyCityName");

                var response = await _httpClient.PostAsync(
                    "http://www.webxml.com.cn/WebServices/WeatherWebService.asmx",
                    content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"HTTP错误: {response.StatusCode}");
                }

                string responseXml = await response.Content.ReadAsStringAsync();
                return ParseWeatherResponse(responseXml);
            }
            catch (Exception ex)
            {
                throw new Exception($"获取天气失败: {ex.Message}", ex);
            }
        }





        private string[] ParseWeatherResponse(string xml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                nsManager.AddNamespace("ns", "http://WebXml.com.cn/");

                var stringNodes = doc.SelectNodes(
                    "/soap:Envelope/soap:Body/ns:getWeatherbyCityNameResponse/ns:getWeatherbyCityNameResult/ns:string",
                    nsManager);

                if (stringNodes == null || stringNodes.Count == 0)
                {
                    return new string[] { "未找到天气信息" };
                }

                var result = new string[stringNodes.Count];
                for (int i = 0; i < stringNodes.Count; i++)
                {
                    result[i] = stringNodes[i]?.InnerText ?? "";
                }

                return result;
            }
            catch (Exception ex)
            {
                return new string[] { $"解析错误: {ex.Message}" };
            }
        }

        /// <summary>
        /// 获取格式化的天气信息
        /// </summary>
        public async Task<string> GetFormattedWeather(string cityName)
        {
            try
            {
                var weatherData = await GetWeatherByCityName(cityName);

                if (weatherData.Length > 8 && string.IsNullOrEmpty(weatherData[8]))
                {
                    return "暂时不支持您查询的城市";
                }

                if (weatherData.Length >= 23)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"城市：{weatherData[1]} ({weatherData[0]})");
                    sb.AppendLine($"更新时间：{weatherData[4]} {weatherData[5]}");
                    sb.AppendLine($"天气：{weatherData[6]}");
                    sb.AppendLine($"气温：{weatherData[5]}");
                    sb.AppendLine($"风力：{weatherData[7]}");

                    if (weatherData.Length > 10)
                    {
                        sb.AppendLine("\n详细信息：");
                        for (int i = 10; i < Math.Min(weatherData.Length, 15); i++)
                        {
                            if (!string.IsNullOrEmpty(weatherData[i]))
                            {
                                sb.AppendLine($"  {weatherData[i]}");
                            }
                        }
                    }

                    return sb.ToString();
                }
                else if (weatherData.Length > 0)
                {
                    return string.Join("\n", weatherData);
                }
                else
                {
                    return "未获取到天气数据";
                }
            }
            catch (Exception ex)
            {
                return $"查询失败: {ex.Message}";
            }
        }

        private string EscapeXml(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        #endregion

        #region 更新服务功能

        /// <summary>
        /// 获取更新文字（针对welcome.cs使用）
        /// </summary>
        public async Task<string> GetUpdateTextAsync(string updateUrl = null)
        {
            try
            {
                // 如果提供了URL，直接使用
                if (!string.IsNullOrEmpty(updateUrl))
                {
                    return await GetUrlContentAsync(updateUrl);
                }

                // 否则尝试多个可能的URL
                string[] possibleUrls = new string[]
                {
                    "https://shop.baka233.top/update/uptext.txt",
                    "http://shop.baka233.top/update/uptext.txt",
                };

                foreach (var url in possibleUrls)
                {
                    try
                    {
                        var content = await GetUrlContentAsync(url);
                        if (!string.IsNullOrEmpty(content))
                        {
                            return content;
                        }
                    }
                    catch
                    {
                        // 继续尝试下一个URL
                        continue;
                    }
                }

                throw new Exception("所有更新服务器都不可用");
            }
            catch (Exception ex)
            {
                throw new Exception($"获取更新失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 获取URL内容
        /// </summary>
        private async Task<string> GetUrlContentAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return content.Trim();
                }
                else
                {
                    throw new Exception($"HTTP错误: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"URL {url} 失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 检查更新状态
        /// </summary>
        public async Task<UpdateCheckResult> CheckUpdateAsync(string currentVersion = null)
        {
            try
            {
                string updateText = await GetUpdateTextAsync();

                return new UpdateCheckResult
                {
                    Success = true,
                    UpdateText = updateText,
                    HasUpdate = string.IsNullOrEmpty(currentVersion) ||
                                !updateText.Contains(currentVersion),
                    CheckTime = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                return new UpdateCheckResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    HasUpdate = false,
                    CheckTime = DateTime.Now
                };
            }
        }

        #endregion

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    /// <summary>
    /// 更新检查结果
    /// </summary>
    public class UpdateCheckResult
    {
        public bool Success { get; set; }
        public string UpdateText { get; set; }
        public bool HasUpdate { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CheckTime { get; set; }
    }
}