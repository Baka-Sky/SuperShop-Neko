using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SuperShop_Neko
{
    public partial class welcome : UserControl
    {
        // 定义多个可能的更新URL
        private readonly List<string> _updateUrls = new List<string>
        {
            "http://shop.bakasky.top/update/uptext.txt",
            "https://shop.bakasky.top/update/uptext.txt",
            "https://shop.baka233.top/update/uptext.txt",
            "http://shop.baka233.top/update/uptext.txt"
        };

        private bool _webViewInitialized = false;
        private bool _isUpdating = false;
        private int _currentUrlIndex = 0;

        private static bool _globalUpdateLoaded = false;
        private static string _globalUpdateText = null;
        private static Color _globalUpdateColor = Color.Black;
        private bool _instanceUpdateCalled = false;

        public welcome()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.None;
            this.Dock = DockStyle.Fill;

            // Load事件处理
            this.Load += Welcome_Load;
        }

        /// <summary>
        /// Load事件处理
        /// </summary>
        private async void Welcome_Load(object sender, EventArgs e)
        {
            try
            {
                // 并行执行两个任务
                await Task.WhenAll(
                    LoadUpdateTextAsync(),      // 加载更新信息
                    InitializeWebViewAsync()    // 初始化WebView2
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Welcome加载失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 异步初始化WebView2并加载Bing
        /// </summary>
        private async Task InitializeWebViewAsync()
        {
            try
            {
                if (webview == null || this.IsDisposed || _webViewInitialized)
                    return;

                // 简单显示加载状态
                if (webview.InvokeRequired)
                {
                    webview.Invoke(new Action(() =>
                    {
                        webview.Visible = false;
                    }));
                }
                else
                {
                    webview.Visible = false;
                }

                // 异步初始化WebView2
                await webview.EnsureCoreWebView2Async(null);

                // 加载Bing
                webview.CoreWebView2.Navigate("https://shop.baka233.top/Logo/html.html");
                _webViewInitialized = true;

                // 显示WebView
                if (webview.InvokeRequired)
                {
                    webview.Invoke(new Action(() =>
                    {
                        webview.Visible = true;
                    }));
                }
                else
                {
                    webview.Visible = true;
                }
            }
            catch (Exception ex)
            {
                // 出错时隐藏webview
                if (webview != null && !webview.IsDisposed)
                {
                    if (webview.InvokeRequired)
                    {
                        webview.Invoke(new Action(() =>
                        {
                            webview.Visible = false;
                        }));
                    }
                    else
                    {
                        webview.Visible = false;
                    }
                }
                Console.WriteLine($"WebView2初始化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 异步加载更新文本（使用heartengine）
        /// </summary>
        private async Task LoadUpdateTextAsync()
        {
            if (_globalUpdateLoaded && !string.IsNullOrEmpty(_globalUpdateText))
            {
                UpdateVersionText(_globalUpdateText, _globalUpdateColor);
                return;
            }

            if (_isUpdating || _instanceUpdateCalled)
                return;

            _isUpdating = true;
            _instanceUpdateCalled = true;

            try
            {
                if (versiontext == null || this.IsDisposed)
                    return;

                // 设置加载状态
                SetVersionTextSafe("正在获取更新信息...", Color.Gray);

                // 使用heartengine获取更新信息
                string updateText = null;
                Exception lastError = null;

                // 尝试所有URL
                for (int i = 0; i < _updateUrls.Count; i++)
                {
                    try
                    {
                        string currentUrl = _updateUrls[i];
                        _currentUrlIndex = i;

                        using (var engine = new heartengine())
                        {
                            updateText = await engine.GetUpdateTextAsync(currentUrl);

                            if (!string.IsNullOrEmpty(updateText))
                            {
                                Console.WriteLine($"成功从 {currentUrl} 获取更新");
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lastError = ex;
                        Console.WriteLine($"URL {_updateUrls[i]} 失败: {ex.Message}");
                        updateText = null;
                    }
                }

                // 检查结果
                if (string.IsNullOrEmpty(updateText))
                {
                    string errorMsg = lastError != null ? lastError.Message : "未知错误";
                    throw new Exception($"所有更新服务器都不可用\n最后错误: {errorMsg}");
                }

                // 缓存结果
                _globalUpdateText = updateText;
                _globalUpdateColor = Color.Black;
                _globalUpdateLoaded = true;

                UpdateVersionText(updateText, Color.Black);
            }
            catch (Exception ex)
            {
                string errorMessage = $"获取更新失败: {ex.Message}";
                _globalUpdateText = errorMessage;
                _globalUpdateColor = Color.Red;
                _globalUpdateLoaded = true;
                UpdateVersionText(errorMessage, Color.Red);
            }
            finally
            {
                _isUpdating = false;
            }
        }

        /// <summary>
        /// 安全地设置版本文本
        /// </summary>
        private void SetVersionTextSafe(string text, Color color)
        {
            if (versiontext == null || versiontext.IsDisposed || this.IsDisposed)
                return;

            if (versiontext.InvokeRequired)
            {
                versiontext.Invoke(new Action(() =>
                {
                    versiontext.Text = text;
                    versiontext.ForeColor = color;
                    versiontext.SelectionStart = 0;
                    versiontext.ScrollToCaret();
                }));
            }
            else
            {
                versiontext.Text = text;
                versiontext.ForeColor = color;
                versiontext.SelectionStart = 0;
                versiontext.ScrollToCaret();
            }
        }

        /// <summary>
        /// 更新版本文本
        /// </summary>
        private void UpdateVersionText(string text, Color color)
        {
            SetVersionTextSafe(text, color);
        }

        /// <summary>
        /// 刷新更新文本
        /// </summary>
        public async void RefreshUpdateText()
        {
            // 重置状态
            _globalUpdateLoaded = false;
            _globalUpdateText = null;
            _instanceUpdateCalled = false;
            _isUpdating = false;
            _currentUrlIndex = 0;

            // 重新加载
            await LoadUpdateTextAsync();
        }

        /// <summary>
        /// 切换到下一个更新URL
        /// </summary>
        public void SwitchToNextUpdateUrl()
        {
            _currentUrlIndex = (_currentUrlIndex + 1) % _updateUrls.Count;
            Console.WriteLine($"切换到URL: {_updateUrls[_currentUrlIndex]}");

            // 重新加载
            RefreshUpdateText();
        }

        /// <summary>
        /// 获取当前使用的URL
        /// </summary>
        public string GetCurrentUpdateUrl()
        {
            if (_updateUrls.Count > 0)
                return _updateUrls[_currentUrlIndex];
            return string.Empty;
        }

        /// <summary>
        /// 获取所有可用的URL
        /// </summary>
        public List<string> GetAllUpdateUrls()
        {
            return new List<string>(_updateUrls);
        }

        /// <summary>
        /// 刷新WebView
        /// </summary>
        public void RefreshWebView()
        {
            if (webview != null && _webViewInitialized && webview.CoreWebView2 != null)
            {
                try
                {
                    webview.CoreWebView2.Reload();
                }
                catch
                {
                    // 忽略错误
                }
            }
        }

        /// <summary>
        /// 天气查询
        /// </summary>
        private void weathergo_Click(object sender, EventArgs e)
        {
            _ = WeatherGoAsync();
        }

        private async Task WeatherGoAsync()
        {
            string city = weatherinput.Text.Trim();

            if (string.IsNullOrEmpty(city))
            {
                weatheroutput.Text = "请输入城市名称";
                weatheroutput.SelectionStart = 0;
                weatheroutput.ScrollToCaret();
                return;
            }

            weatheroutput.Text = "正在查询天气...";
            weatheroutput.SelectionStart = 0;
            weatheroutput.ScrollToCaret();

            if (weathergo != null)
                weathergo.Enabled = false;

            try
            {
                using (var weatherService = new heartengine())
                {
                    string weatherInfo = await weatherService.GetFormattedWeather(city);
                    weatheroutput.Text = weatherInfo;
                    weatheroutput.SelectionStart = 0;
                    weatheroutput.ScrollToCaret();
                }
            }
            catch (Exception ex)
            {
                weatheroutput.Text = $"查询失败：{ex.Message}";
                weatheroutput.SelectionStart = 0;
                weatheroutput.ScrollToCaret();
            }
            finally
            {
                if (weathergo != null)
                    weathergo.Enabled = true;
            }
        }

        /// <summary>
        /// 大小改变事件
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (this.Parent is Panel parentPanel && parentPanel.AutoScroll)
            {
                this.MaximumSize = parentPanel.ClientSize;
            }
        }
    }
}