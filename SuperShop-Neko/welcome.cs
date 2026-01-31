using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

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

        // 用于WebView2的用户数据文件夹路径
        private readonly string _webViewDataPath;

        public welcome()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.None;
            this.Dock = DockStyle.Fill;

            // 设置WebView2的用户数据文件夹路径
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _webViewDataPath = System.IO.Path.Combine(appDataPath, "SuperShop_Neko", "WebView2Cache");

            // 确保文件夹存在
            System.IO.Directory.CreateDirectory(_webViewDataPath);

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
                // 清除WebView2缓存
                ClearWebView2Cache();

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
        /// 清除WebView2缓存
        /// </summary>
        private void ClearWebView2Cache()
        {
            try
            {
                if (System.IO.Directory.Exists(_webViewDataPath))
                {
                    Console.WriteLine($"正在清除WebView2缓存: {_webViewDataPath}");

                    // 删除整个缓存文件夹
                    System.IO.Directory.Delete(_webViewDataPath, true);
                    Console.WriteLine("WebView2缓存已清除");

                    // 重新创建空文件夹
                    System.IO.Directory.CreateDirectory(_webViewDataPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"清除WebView2缓存失败: {ex.Message}");
                // 如果清除失败，继续使用现有缓存
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

                // 创建WebView2环境
                var environment = await CoreWebView2Environment.CreateAsync(userDataFolder: _webViewDataPath);

                // 配置WebView2控件
                await webview.EnsureCoreWebView2Async(environment);

                // 禁用拖动、滚动和滚动条
                ConfigureWebViewSettings();

                // 加载页面
                webview.CoreWebView2.Navigate("https://shop.baka233.top/Logo/html.html");

                // 等待页面加载完成
                await Task.Delay(1000);

                // 禁用滚动并居中显示
                DisableScrollingAndCenter();

                // 设置导航完成事件
                webview.CoreWebView2.NavigationCompleted += async (sender, e) =>
                {
                    if (e.IsSuccess)
                    {
                        Console.WriteLine("页面加载完成");

                        // 等待一段时间后确保内容加载完成
                        await Task.Delay(500);

                        // 再次确保滚动条隐藏
                        await EnsureScrollbarHidden();
                    }
                };

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
        /// 确保滚动条隐藏
        /// </summary>
        private async Task EnsureScrollbarHidden()
        {
            try
            {
                string script = @"
                    // 再次确保所有滚动条都隐藏
                    document.body.style.overflow = 'hidden';
                    document.documentElement.style.overflow = 'hidden';
                    
                    // 隐藏WebKit滚动条
                    document.body.style.webkitOverflowScrolling = 'auto';
                    
                    // 对于WebKit浏览器
                    var style = document.createElement('style');
                    style.innerHTML = '::-webkit-scrollbar { display: none !important; }';
                    document.head.appendChild(style);
                    
                    // 确保内容居中
                    document.body.style.display = 'flex';
                    document.body.style.justifyContent = 'center';
                    document.body.style.alignItems = 'center';
                    document.body.style.margin = '0';
                    document.body.style.padding = '0';
                    
                    // 滚动到顶部
                    window.scrollTo(0, 0);
                ";

                await webview.CoreWebView2.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"确保滚动条隐藏失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 配置WebView2设置以禁用拖动和滚动
        /// </summary>
        private void ConfigureWebViewSettings()
        {
            if (webview == null || webview.CoreWebView2 == null)
                return;

            try
            {
                // 禁用默认上下文菜单
                webview.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

                // 禁用DevTools
                webview.CoreWebView2.Settings.AreDevToolsEnabled = false;

                // 注入脚本以禁用滚动条和滚动行为
                InjectScrollbarDisablingScripts();

                // 禁用鼠标滚轮滚动（使用更兼容的方式）
                DisableMouseWheelScrolling();

                // 禁用键盘控制（使用KeyDown事件）
                DisableKeyboardScrolling();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"配置WebView设置失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 注入脚本以禁用滚动条和滚动行为
        /// </summary>
        private void InjectScrollbarDisablingScripts()
        {
            try
            {
                // 注入CSS样式来隐藏滚动条并禁用溢出
                string disableScrollCSS = @"
                    <style id='disableScrollStyle'>
                        html, body {
                            overflow: hidden !important;
                        }
                        body::-webkit-scrollbar {
                            display: none !important;
                        }
                        * {
                            -webkit-user-select: none !important;
                            -moz-user-select: none !important;
                            -ms-user-select: none !important;
                            user-select: none !important;
                        }
                    </style>
                ";

                // 注入JavaScript来禁用各种滚动和拖动行为
                string disableScrollJS = @"
                    // 禁止拖拽
                    document.addEventListener('dragstart', function(e) {
                        e.preventDefault();
                        return false;
                    }, false);
                    
                    // 禁止选择文本
                    document.addEventListener('selectstart', function(e) {
                        e.preventDefault();
                        return false;
                    }, false);
                    
                    // 禁止右键菜单
                    document.addEventListener('contextmenu', function(e) {
                        e.preventDefault();
                        return false;
                    }, false);
                    
                    // 禁止鼠标滚轮
                    document.addEventListener('wheel', function(e) {
                        e.preventDefault();
                        return false;
                    });
                    
                    // 禁止触摸移动
                    document.addEventListener('touchmove', function(e) {
                        e.preventDefault();
                        return false;
                    });
                    
                    // 强制页面内容居中显示
                    function centerContent() {
                        const body = document.body;
                        const html = document.documentElement;
                        
                        // 设置最小高度为视口高度
                        body.style.minHeight = '100vh';
                        html.style.minHeight = '100vh';
                        
                        // 使用flexbox居中内容
                        body.style.display = 'flex';
                        body.style.justifyContent = 'center';
                        body.style.alignItems = 'center';
                        body.style.margin = '0';
                        body.style.padding = '0';
                        
                        // 确保所有直接子元素也居中
                        Array.from(body.children).forEach(child => {
                            if (!child.id || child.id !== 'disableScrollStyle') {
                                child.style.margin = 'auto';
                                child.style.maxWidth = '100%';
                                child.style.maxHeight = '100%';
                            }
                        });
                        
                        // 隐藏滚动条
                        body.style.overflow = 'hidden';
                        html.style.overflow = 'hidden';
                    }
                    
                    // 初始居中
                    centerContent();
                    
                    // 监听窗口大小变化重新居中
                    window.addEventListener('resize', centerContent);
                    
                    // 禁止iframe滚动
                    document.querySelectorAll('iframe').forEach(iframe => {
                        iframe.style.overflow = 'hidden';
                    });
                ";

                // 首先注入CSS
                webview.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(disableScrollCSS);

                // 等待页面加载后注入JS并执行居中
                webview.CoreWebView2.DOMContentLoaded += (sender, e) =>
                {
                    webview.CoreWebView2.ExecuteScriptAsync(disableScrollJS);
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"注入滚动禁用脚本失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 禁用滚动并居中显示
        /// </summary>
        private async void DisableScrollingAndCenter()
        {
            try
            {
                if (webview == null || webview.CoreWebView2 == null)
                    return;

                // 等待页面完全加载
                await Task.Delay(1500);

                // 执行JavaScript来确保页面内容居中
                string centerScript = @"
                    (function() {
                        // 确保body使用flex布局
                        document.body.style.display = 'flex';
                        document.body.style.justifyContent = 'center';
                        document.body.style.alignItems = 'center';
                        document.body.style.height = '100%';
                        document.body.style.width = '100%';
                        document.body.style.margin = '0';
                        document.body.style.padding = '0';
                        
                        // 查找主要内容元素并居中
                        const mainContent = document.querySelector('body > *:not(script):not(style)');
                        if (mainContent) {
                            mainContent.style.margin = 'auto';
                            mainContent.style.display = 'block';
                        }
                        
                        // 滚动到顶部
                        window.scrollTo(0, 0);
                        
                        // 禁用iframe滚动
                        var iframes = document.getElementsByTagName('iframe');
                        for (var i = 0; i < iframes.length; i++) {
                            iframes[i].style.overflow = 'hidden';
                        }
                    })();
                ";

                await webview.CoreWebView2.ExecuteScriptAsync(centerScript);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"居中显示失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 禁用鼠标滚轮滚动
        /// </summary>
        private void DisableMouseWheelScrolling()
        {
            try
            {
                // 使用更简单的方法禁用鼠标滚轮
                webview.MouseWheel += (sender, e) =>
                {
                    // 简单处理，不执行任何操作
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"禁用鼠标滚轮失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 禁用键盘滚动
        /// </summary>
        private void DisableKeyboardScrolling()
        {
            try
            {
                // 使用KeyDown事件处理键盘
                webview.KeyDown += (sender, e) =>
                {
                    // 检查是否是滚动相关的按键
                    bool isScrollKey = e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                                      e.KeyCode == Keys.Left || e.KeyCode == Keys.Right ||
                                      e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown ||
                                      e.KeyCode == Keys.Home || e.KeyCode == Keys.End ||
                                      e.KeyCode == Keys.Space;

                    if (isScrollKey)
                    {
                        // 阻止事件继续传播
                        e.SuppressKeyPress = true;
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"禁用键盘滚动失败: {ex.Message}");
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
        /// 完全重置WebView2（包括清除缓存）
        /// </summary>
        public async Task ResetWebView2Async()
        {
            try
            {
                if (webview != null && !webview.IsDisposed)
                {
                    // 导航到空白页面
                    webview.CoreWebView2?.Navigate("about:blank");

                    // 等待一下
                    await Task.Delay(100);

                    // 清除缓存
                    ClearWebView2Cache();

                    // 重新初始化
                    _webViewInitialized = false;
                    await InitializeWebViewAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"重置WebView2失败: {ex.Message}");
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