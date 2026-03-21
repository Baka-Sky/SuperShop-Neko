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
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using System.Windows.Forms;
using System.Text.Json;

namespace SuperShop_Neko
{
    public class heartengine : IDisposable
    {
        private readonly HttpClient _httpClient;
        private Form _mainForm;

        public heartengine(Form mainForm = null)
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            _mainForm = mainForm;
        }

        public void SetMainForm(Form mainForm)
        {
            _mainForm = mainForm;
        }

        #region 版本信息功能
        public string GetVersionInfo()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("═══════════════════════════════════════════");
                sb.AppendLine("          超级小铺 Neko - 版本信息");
                sb.AppendLine("═══════════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"📦 软件版本号: {GetSoftwareVersion()}");
                sb.AppendLine($"⚙️  内核版本号: {GetCoreVersion()}");
                sb.AppendLine($"🕐 软件构建时间: {GetBuildTimestamp()}");
                sb.AppendLine($"🔧 .NET 运行时: {GetDotNetVersion()}");
                sb.AppendLine($"🌐 WebView2版本: {GetWebView2Version()}");
                sb.AppendLine($"💻 C# 语言版本: {GetCSharpVersion()}");
                sb.AppendLine($"🖥️  系统内核版本: {GetWindowsKernelVersion()}");
                sb.AppendLine($"📁 软件运行目录: {GetApplicationDirectory()}");
                sb.AppendLine($"💿 操作系统: {GetOSInfo()}");
                sb.AppendLine($"🏗️  系统架构: {GetSystemArchitecture()}");
                sb.AppendLine($"💾 内存使用: {GetMemoryInfo()}");
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

        private string GetSoftwareVersion()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                var fileVersionAttr = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                if (fileVersionAttr != null && !string.IsNullOrEmpty(fileVersionAttr.Version))
                {
                    return fileVersionAttr.Version;
                }
                var version = assembly.GetName().Version;
                if (version != null)
                {
                    return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
                }
                return "1.0.0.0";
            }
            catch
            {
                return "1.0.0.0";
            }
        }

        private string GetCoreVersion()
        {
            return "HeartEngine4 & HeartCore1";
        }

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

        private string GetDotNetVersion()
        {
            try
            {
                var version = Environment.Version;
                var description = RuntimeInformation.FrameworkDescription;
                return $"{description} (v{version})";
            }
            catch
            {
                return $"NET {Environment.Version}";
            }
        }

        private string GetWebView2Version()
        {
            try
            {
                string[] registryPaths = new[]
                {
                    @"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}",
                    @"SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}"
                };
                foreach (var path in registryPaths)
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(path))
                    {
                        if (key != null)
                        {
                            var v = key.GetValue("pv")?.ToString();
                            if (!string.IsNullOrEmpty(v)) return v;
                        }
                    }
                }

                string[] paths =
                {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft", "EdgeWebView", "Application"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft", "EdgeWebView", "Application")
                };
                foreach (var p in paths)
                {
                    if (Directory.Exists(p))
                    {
                        foreach (var d in Directory.GetDirectories(p))
                        {
                            var name = Path.GetFileName(d);
                            if (Version.TryParse(name, out _))
                                return name;
                        }
                    }
                }
                return "未安装";
            }
            catch
            {
                return "获取失败";
            }
        }

        private string GetCSharpVersion()
        {
            try
            {
                var v = Environment.Version;
                if (v.Major >= 8) return "C# 12+";
                if (v.Major >= 7) return "C# 11";
                if (v.Major >= 6) return "C# 10";
                if (v.Major >= 5) return "C# 9";
                return "C# 7+";
            }
            catch
            {
                return "未知";
            }
        }

        private string GetWindowsKernelVersion()
        {
            try
            {
                var v = Environment.OSVersion.Version;
                return $"NT {v.Major}.{v.Minor} Build {v.Build}";
            }
            catch
            {
                return "未知";
            }
        }

        private string GetApplicationDirectory()
        {
            try
            {
                var d = AppDomain.CurrentDomain.BaseDirectory;
                return d.Length > 60 ? "…" + d.Substring(d.Length - 50) : d;
            }
            catch
            {
                return "未知";
            }
        }

        private string GetOSInfo()
        {
            try
            {
                var v = Environment.OSVersion.Version;
                string name = "Windows";

                if (v.Major == 10)
                    name = v.Build >= 2200 ? "Windows 11" : "Windows 10";
                else if (v.Major == 6)
                {
                    if (v.Minor == 3) name = "Windows 8.1";
                    else if (v.Minor == 2) name = "Windows 8";
                    else if (v.Minor == 1) name = "Windows 7";
                    else name = "Windows Vista";
                }
                return $"{name} {(Environment.Is64BitOperatingSystem ? "64位" : "32位")}";
            }
            catch
            {
                return "Windows";
            }
        }

        private string GetSystemArchitecture()
        {
            try
            {
                return RuntimeInformation.OSArchitecture.ToString();
            }
            catch
            {
                return Environment.Is64BitOperatingSystem ? "64位" : "32位";
            }
        }

        private string GetMemoryInfo()
        {
            try
            {
                var p = Process.GetCurrentProcess();
                return $"{p.WorkingSet64 / 1048576} MB";
            }
            catch
            {
                return "未知";
            }
        }

        private string GetProcessorInfo()
        {
            try
            {
                return $"{Environment.ProcessorCount} 核";
            }
            catch
            {
                return "未知";
            }
        }
        #endregion

        #region 天气功能
        public async Task<string[]> GetWeatherByCityName(string cityName)
        {
            try
            {
                string soap = $"""
                    <?xml version="1.0" encoding="utf-8"?>
                    <soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
                      <soap:Body>
                        <getWeatherbyCityName xmlns="http://WebXml.com.cn/">
                          <theCityName>{EscapeXml(cityName)}</theCityName>
                        </getWeatherbyCityName>
                      </soap:Body>
                    </soap:Envelope>
                    """;

                var c = new StringContent(soap, Encoding.UTF8, "text/xml");
                c.Headers.Add("SOAPAction", "http://WebXml.com.cn/getWeatherbyCityName");

                var r = await _httpClient.PostAsync("http://www.webxml.com.cn/WebServices/WeatherWebService.asmx", c);
                r.EnsureSuccessStatusCode();
                return ParseWeatherResponse(await r.Content.ReadAsStringAsync());
            }
            catch
            {
                return new[] { "获取天气失败" };
            }
        }

        private string[] ParseWeatherResponse(string xml)
        {
            try
            {
                var d = new XmlDocument();
                d.LoadXml(xml);
                var m = new XmlNamespaceManager(d.NameTable);
                m.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                m.AddNamespace("ns", "http://WebXml.com.cn/");

                var ns = d.SelectNodes("//ns:string", m);
                var res = new string[ns.Count];
                for (int i = 0; i < ns.Count; i++)
                    res[i] = ns[i].InnerText;

                return res;
            }
            catch
            {
                return new[] { "解析失败" };
            }
        }

        public async Task<string> GetFormattedWeather(string city)
        {
            var data = await GetWeatherByCityName(city);
            if (data.Length >= 10)
                return $"{data[1]} {data[6]} {data[5]}";
            return "无法获取天气";
        }

        private string EscapeXml(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
        #endregion

        #region 更新与Toast
        public async Task CheckForUpdateAsync()
        {
            try
            {
                string sv = await GetServerVersionAsync();
                if (string.IsNullOrWhiteSpace(sv)) return;

                string lv = ReadLocalVersionFromConfig();
                if (string.IsNullOrWhiteSpace(lv)) return;

                if (IsNewerVersionAvailable(lv, sv))
                {
                    ShowUpdateToast(sv);
                }
            }
            catch { }
        }

        private async Task<string> GetServerVersionAsync()
        {
            try
            {
                return (await _httpClient.GetStringAsync("https://shop.baka233.top/update/version.txt")).Trim();
            }
            catch
            {
                return null;
            }
        }

        public string ReadLocalVersionFromConfig()
        {
            try
            {
                string f = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                if (!File.Exists(f)) return null;
                using var doc = JsonDocument.Parse(File.ReadAllText(f));
                if (doc.RootElement.TryGetProperty("Version", out var v))
                    return v.GetString()?.Trim();
                return null;
            }
            catch
            {
                return null;
            }
        }

        private bool IsNewerVersionAvailable(string local, string server)
        {
            try
            {
                Version v1 = new Version(local.Trim());
                Version v2 = new Version(server.Trim());
                return v2 > v1;
            }
            catch
            {
                return false;
            }
        }

        public void ShowUpdateToast(string targetVersion)
        {
            try
            {
                string lv = ReadLocalVersionFromConfig() ?? "未知";
                new ToastContentBuilder()
                    .AddArgument("action", "update_notification")
                    .AddArgument("target_version", targetVersion)
                    .AddText("超级小铺有新版本可用！")
                    .AddText($"当前版本: {lv}")
                    .AddText($"最新版本: {targetVersion}")
                    .AddButton(new ToastButton().SetContent("立即更新").AddArgument("choice", "update_now"))
                    .AddButton(new ToastButton().SetContent("稍后提醒").AddArgument("choice", "update_later"))
                    .SetToastDuration(ToastDuration.Long)
                    .Show(toast =>
                    {
                        toast.Activated += (s, e) => Toast_Activated(s, e, targetVersion);
                    });
            }
            catch
            {
                ShowUpdateMessageBox(targetVersion);
            }
        }

        private async void Toast_Activated(ToastNotification sender, object args, string tv)
        {
            await Task.Delay(100);
            if (_mainForm == null || _mainForm.IsDisposed) return;

            _mainForm.Invoke(() =>
            {
                if (args is ToastActivatedEventArgs e)
                {
                    if (e.Arguments.Contains("update_now"))
                        HandleUpdateNow(tv);
                }
                else
                {
                    ShowUpdateMessageBox(tv);
                }
            });
        }

        public void ShowUpdateMessageBox(string targetVersion)
        {
            string lv = ReadLocalVersionFromConfig() ?? "未知";
            var res = MessageBox.Show(
                $"有新版本！\n当前：{lv}\n最新：{targetVersion}\n是否立即更新？",
                "更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
                HandleUpdateNow(targetVersion);
        }

        public async void HandleUpdateNow(string targetVersion)
        {
            try
            {
                await Task.Run(() =>
                {
                    Process.Start(new ProcessStartInfo("https://shop.baka233.top/shop.exe")
                    { UseShellExecute = true });
                });
                MessageBox.Show("已打开下载页面！", "更新");
            }
            catch
            {
                MessageBox.Show("更新失败，请手动前往官网下载", "错误");
            }
        }

        // 支持传入 URL，给 welcome 页面用
        public async Task<string> GetUpdateTextAsync(string url)
        {
            try
            {
                return await _httpClient.GetStringAsync(url);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    public class UpdateCheckResult
    {
        public bool Success { get; set; }
        public string UpdateText { get; set; }
        public bool HasUpdate { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CheckTime { get; set; }
    }
}
