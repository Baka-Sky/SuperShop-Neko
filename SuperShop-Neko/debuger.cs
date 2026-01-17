using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SuperShop_Neko
{
    internal class debuger : IDisposable
    {
        // Windows API
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleTitle(string lpConsoleTitle);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const int VK_LBUTTON = 0x01;
        private const int VK_RBUTTON = 0x02;

        // 单例
        private static debuger _instance;
        private static readonly object _lock = new object();

        // 控制台
        private IntPtr _consoleHandle;
        private bool _isConsoleAllocated = false;
        private bool _isDisposed = false;
        private bool _isInitialized = false;
        private bool _usePowerShell = false;
        private Process _consoleProcess = null;
        private StreamWriter _consoleInput = null;

        // 日志
        private readonly ConcurrentQueue<LogEntry> _logQueue = new ConcurrentQueue<LogEntry>();
        private Thread _logWorkerThread;
        private volatile bool _logWorkerRunning = true;

        // 监控
        private static int _totalFormsCreated = 0;
        private static int _totalFormsShown = 0;
        private static int _totalFormsLoaded = 0;
        private static int _totalThreadsCreated = 0;
        private static int _totalThreadsReleased = 0;
        private static int _totalNetworkRequests = 0;
        private static int _totalMouseClicks = 0;
        private static readonly ConcurrentDictionary<string, FormInfo> _activeForms = new ConcurrentDictionary<string, FormInfo>();
        private static readonly ConcurrentDictionary<int, ThreadInfo> _activeThreads = new ConcurrentDictionary<int, ThreadInfo>();
        private static readonly ConcurrentDictionary<string, NetworkRequest> _activeNetworkRequests = new ConcurrentDictionary<string, NetworkRequest>();

        // 日志级别
        public enum LogLevel
        {
            INFO,
            WARNING,
            ERROR,
            SUCCESS,
            FORM,
            THREAD,
            NETWORK,
            MOUSE,
            SYSTEM,
            CS_FILE
        }

        private class LogEntry
        {
            public string Message { get; set; }
            public LogLevel Level { get; set; }
            public DateTime Timestamp { get; set; }
            public int ThreadId { get; set; }
        }

        private class FormInfo
        {
            public string Name { get; set; }
            public Type FormType { get; set; }
            public DateTime CreatedTime { get; set; }
            public DateTime? ShownTime { get; set; }
            public DateTime? LoadedTime { get; set; }
            public IntPtr Handle { get; set; }
            public bool IsLoaded { get; set; }
        }

        private class ThreadInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public bool IsBackground { get; set; }
            public string StackTrace { get; set; }
        }

        private class NetworkRequest
        {
            public string Id { get; set; }
            public string Url { get; set; }
            public string Method { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public int? StatusCode { get; set; }
            public long? DurationMs { get; set; }
        }

        // 单例
        public static debuger Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null || _instance._isDisposed)
                    {
                        _instance = new debuger();
                    }
                    return _instance;
                }
            }
        }

        private debuger() { }

        // ========== 初始化 ==========
        public void Initialize()
        {
#if DEBUG
            if (_isInitialized || _isDisposed) return;

            try
            {
                _isInitialized = true;

                // 检测操作系统版本，决定使用哪种控制台
                DetectConsoleType();

                // 创建控制台
                if (_usePowerShell)
                {
                    StartPowerShellConsole();
                }
                else
                {
                    StartClassicConsole();
                }

                // 显示标题
                ShowTitle();

                // 启动日志线程
                StartLogWorker();

                // 扫描所有窗体类
                ScanAllFormClasses();

                // 安装全局监控
                InstallGlobalMonitors();

                // 记录启动
                LogInternal("=== 全局监控系统已启动 ===", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);
                LogInternal($"启动时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);
                LogInternal($"进程: {Process.GetCurrentProcess().ProcessName} (PID: {Process.GetCurrentProcess().Id})",
                    LogLevel.SYSTEM, Environment.CurrentManagedThreadId);
                LogInternal($"控制台模式: {(_usePowerShell ? "PowerShell 7+" : "传统CMD")}",
                    LogLevel.SYSTEM, Environment.CurrentManagedThreadId);

                // 开始实时监控
                StartRealtimeMonitoring();

                LogInternal("监控系统就绪，等待操作...", LogLevel.SUCCESS, Environment.CurrentManagedThreadId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"监控启动失败: {ex.Message}");
                // 尝试回退到传统控制台
                try
                {
                    StartClassicConsole();
                }
                catch { }
            }
#endif
        }

        private void DetectConsoleType()
        {
            try
            {
                // 检测PowerShell 7+是否存在
                string[] powershellPaths = {
                    @"C:\Program Files\PowerShell\7\pwsh.exe",
                    @"C:\Program Files\PowerShell\7-preview\pwsh.exe",
                    @"C:\Program Files\PowerShell\6\pwsh.exe",
                    @"C:\Program Files (x86)\PowerShell\7\pwsh.exe",
                    @"C:\Program Files (x86)\PowerShell\7-preview\pwsh.exe"
                };

                foreach (var path in powershellPaths)
                {
                    if (File.Exists(path))
                    {
                        _usePowerShell = true;
                        return;
                    }
                }

                // 检查PowerShell Core是否在PATH中
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "where",
                        Arguments = "pwsh.exe",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    };

                    using (var process = Process.Start(startInfo))
                    {
                        if (process != null)
                        {
                            process.WaitForExit(1000);
                            var output = process.StandardOutput.ReadToEnd();
                            if (!string.IsNullOrEmpty(output) && output.Contains("pwsh.exe"))
                            {
                                _usePowerShell = true;
                                return;
                            }
                        }
                    }
                }
                catch { }

                _usePowerShell = false;
            }
            catch
            {
                _usePowerShell = false;
            }
        }

        private void StartPowerShellConsole()
        {
            try
            {
                // 先分配控制台
                AllocConsole();
                _consoleHandle = GetConsoleWindow();
                _isConsoleAllocated = true;

                // 设置控制台标题
                SetConsoleTitle($"🔍 Neko调试器 - PowerShell - {DateTime.Now:HH:mm:ss}");

                // 设置编码
                Console.OutputEncoding = Encoding.UTF8;

                // 设置缓冲区和光标
                Console.CursorVisible = false;
                Console.BufferHeight = 10000;

                // 显示控制台
                ShowWindow(_consoleHandle, SW_SHOW);

                // 设置PowerShell窗口样式
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;

                LogInternal("使用 PowerShell 控制台", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);
            }
            catch (Exception ex)
            {
                LogInternal($"PowerShell 控制台初始化失败: {ex.Message}", LogLevel.ERROR, Environment.CurrentManagedThreadId);
                throw;
            }
        }

        private void StartClassicConsole()
        {
            try
            {
                // 传统控制台模式
                AllocConsole();
                _consoleHandle = GetConsoleWindow();
                _isConsoleAllocated = true;

                // 设置控制台标题
                SetConsoleTitle($"🔍 Neko调试器 - CMD - {DateTime.Now:HH:mm:ss}");

                // 设置编码
                Console.OutputEncoding = Encoding.UTF8;

                // 设置控制台样式
                Console.CursorVisible = false;
                Console.BufferHeight = 10000;

                // 显示控制台
                ShowWindow(_consoleHandle, SW_SHOW);

                LogInternal("使用传统CMD控制台", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"传统控制台初始化失败: {ex.Message}");
                throw;
            }
        }

        private void ShowTitle()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            // 使用更适合PowerShell的配色方案
            if (_usePowerShell)
            {
                // PowerShell风格配色
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("════════════════════════════════════════════════════════════════════════════════");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("   ____                  ______               _   __  ____");
                Console.WriteLine("  / __/_ _____  ___ ____/ __/ /  ___  ___    | | / / / / /");
                Console.WriteLine(" _\\ \\/ // / _ \\/ -_) __/\\ \\/ _ \\/ _ \\/ _ \\   | |/ / /_  _/");
                Console.WriteLine("/___/\\_,_/ .__/\\__/_/ /___/_//_/\\___/ .__/   |___/   /_/");
                Console.WriteLine("        /_/                        /_/");
                Console.WriteLine("");
                Console.WriteLine("                    回望过去 · 去往未来");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("════════════════════════════════════════════════════════════════════════════════");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("🖥️  全局监控系统 - 完整操作追踪");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("📡  监控: 窗体生命周期 | 线程管理 | 网络请求 | 鼠标点击 | .cs文件");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"💻  终端: {(_usePowerShell ? "PowerShell 7+" : "传统CMD")}");
            }
            else
            {
                // 传统CMD风格配色
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("════════════════════════════════════════════════════════════════════════════════");
                Console.WriteLine("   ____                  ______               _   __  ____");
                Console.WriteLine("  / __/_ _____  ___ ____/ __/ /  ___  ___    | | / / / / /");
                Console.WriteLine(" _\\ \\/ // / _ \\/ -_) __/\\ \\/ _ \\/ _ \\/ _ \\   | |/ / /_  _/");
                Console.WriteLine("/___/\\_,_/ .__/\\__/_/ /___/_//_/\\___/ .__/   |___/   /_/");
                Console.WriteLine("        /_/                        /_/");
                Console.WriteLine("");
                Console.WriteLine("                    回望过去 · 去往未来");
                Console.WriteLine("════════════════════════════════════════════════════════════════════════════════");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("🖥️  全局监控系统 - 完整操作追踪");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("📡  监控: 窗体生命周期 | 线程管理 | 网络请求 | 鼠标点击 | .cs文件");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"💻  终端: {(_usePowerShell ? "PowerShell 7+" : "传统CMD")}");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
        }

        // ========== 日志系统 ==========
        private void StartLogWorker()
        {
            _logWorkerThread = new Thread(() =>
            {
                while (_logWorkerRunning)
                {
                    try
                    {
                        while (_logQueue.TryDequeue(out var entry))
                        {
                            WriteLogToConsole(entry);
                        }
                        Thread.Sleep(1);
                    }
                    catch { }
                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };

            _logWorkerThread.Start();
        }

        private void LogInternal(string message, LogLevel level, int threadId)
        {
#if DEBUG
            if (!_isConsoleAllocated || _isDisposed) return;

            _logQueue.Enqueue(new LogEntry
            {
                Message = message,
                Level = level,
                Timestamp = DateTime.Now,
                ThreadId = threadId
            });
#endif
        }

        private void WriteLogToConsole(LogEntry entry)
        {
            try
            {
                // 使用ConsoleColor，支持PowerShell和CMD
                Console.ForegroundColor = GetConsoleColor(entry.Level);
                var timeStr = entry.Timestamp.ToString("HH:mm:ss.fff");
                var symbol = GetLevelSymbol(entry.Level);
                Console.WriteLine($"{symbol} [{timeStr}] [T:{entry.ThreadId:00}] {entry.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            catch { }
        }

        private string GetLevelSymbol(LogLevel level)
        {
            return level switch
            {
                LogLevel.FORM => "📋",
                LogLevel.THREAD => "🧵",
                LogLevel.NETWORK => "🌐",
                LogLevel.MOUSE => "🖱️",
                LogLevel.CS_FILE => "📄",
                LogLevel.SYSTEM => "🖥️",
                LogLevel.INFO => "ℹ️",
                LogLevel.WARNING => "⚠️",
                LogLevel.ERROR => "❌",
                LogLevel.SUCCESS => "✅",
                _ => "📝"
            };
        }

        private ConsoleColor GetConsoleColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.FORM => ConsoleColor.Cyan,
                LogLevel.THREAD => ConsoleColor.Magenta,
                LogLevel.NETWORK => ConsoleColor.Blue,
                LogLevel.MOUSE => ConsoleColor.DarkCyan,
                LogLevel.CS_FILE => ConsoleColor.DarkYellow,
                LogLevel.SYSTEM => ConsoleColor.Green,
                LogLevel.INFO => ConsoleColor.White,
                LogLevel.WARNING => ConsoleColor.Yellow,
                LogLevel.ERROR => ConsoleColor.Red,
                LogLevel.SUCCESS => ConsoleColor.Green,
                _ => ConsoleColor.Gray
            };
        }

        // ========== 扫描所有窗体类 ==========
        private void ScanAllFormClasses()
        {
            try
            {
                LogInternal("开始扫描所有窗体类...", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            if (typeof(Form).IsAssignableFrom(type) && !type.IsAbstract)
                            {
                                Interlocked.Increment(ref _totalFormsCreated);

                                LogInternal($"发现窗体类: {type.FullName}", LogLevel.FORM, Environment.CurrentManagedThreadId);
                                LogInternal($"  📁 程序集: {assembly.GetName().Name}", LogLevel.INFO, Environment.CurrentManagedThreadId);
                                LogInternal($"  📝 基类: {type.BaseType?.Name ?? "Object"}", LogLevel.INFO, Environment.CurrentManagedThreadId);

                                ScanCsFileForType(type);
                            }
                        }
                    }
                    catch (ReflectionTypeLoadException) { }
                    catch { }
                }

                LogInternal($"扫描完成: 发现 {_totalFormsCreated} 个窗体类", LogLevel.SUCCESS, Environment.CurrentManagedThreadId);
            }
            catch (Exception ex)
            {
                LogInternal($"扫描失败: {ex.Message}", LogLevel.ERROR, Environment.CurrentManagedThreadId);
            }
        }

        private void ScanCsFileForType(Type formType)
        {
            try
            {
                var projectDir = FindProjectDirectory();
                if (!string.IsNullOrEmpty(projectDir))
                {
                    var csFiles = Directory.GetFiles(projectDir, "*.cs", SearchOption.AllDirectories);
                    foreach (var csFile in csFiles)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(csFile);
                        if (fileName == formType.Name)
                        {
                            var fileInfo = new FileInfo(csFile);
                            LogInternal($"  📄 源文件: {Path.GetFileName(csFile)}", LogLevel.CS_FILE, Environment.CurrentManagedThreadId);
                            LogInternal($"  ⏱️  最后修改: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}",
                                LogLevel.INFO, Environment.CurrentManagedThreadId);
                            break;
                        }
                    }
                }
            }
            catch { }
        }

        private string FindProjectDirectory()
        {
            try
            {
                var exePath = Process.GetCurrentProcess().MainModule.FileName;
                var dir = Path.GetDirectoryName(exePath);

                while (!string.IsNullOrEmpty(dir))
                {
                    if (Directory.GetFiles(dir, "*.csproj").Length > 0)
                        return dir;

                    dir = Path.GetDirectoryName(dir);
                }
            }
            catch { }

            return null;
        }

        // ========== 安装全局监控 ==========
        private void InstallGlobalMonitors()
        {
            try
            {
                // 1. 监控窗体生命周期
                MonitorFormLifecycle();

                // 2. 监控线程管理
                MonitorThreadManagement();

                // 3. 监控网络请求
                MonitorNetworkTraffic();

                // 4. 监控鼠标点击
                MonitorMouseClicks();

                // 5. 监控应用程序事件
                MonitorApplicationEvents();

                LogInternal("全局监控器安装完成", LogLevel.SUCCESS, Environment.CurrentManagedThreadId);
            }
            catch (Exception ex)
            {
                LogInternal($"监控器安装失败: {ex.Message}", LogLevel.ERROR, Environment.CurrentManagedThreadId);
            }
        }

        // ========== 1. 窗体生命周期监控 ==========
        private void MonitorFormLifecycle()
        {
            Application.AddMessageFilter(new FormLifecycleMonitor(this));
            LogInternal("窗体生命周期监控已启用", LogLevel.FORM, Environment.CurrentManagedThreadId);
        }

        private class FormLifecycleMonitor : IMessageFilter
        {
            private readonly debuger _debugger;
            private readonly ConcurrentDictionary<IntPtr, FormTracker> _formTrackers = new ConcurrentDictionary<IntPtr, FormTracker>();

            public FormLifecycleMonitor(debuger debugger)
            {
                _debugger = debugger;
            }

            public bool PreFilterMessage(ref Message m)
            {
                try
                {
                    switch (m.Msg)
                    {
                        case 0x0001: // WM_CREATE
                            OnFormCreate(m.HWnd);
                            break;

                        case 0x0018: // WM_SHOWWINDOW
                            if (m.WParam.ToInt32() == 1) // TRUE = 显示窗口
                            {
                                OnFormShow(m.HWnd);
                            }
                            break;

                        case 0x0002: // WM_DESTROY
                            OnFormDestroy(m.HWnd);
                            break;

                        case 0x0111: // WM_COMMAND
                            // 可以在这里监控窗体命令
                            break;
                    }
                }
                catch { }

                return false;
            }

            private void OnFormCreate(IntPtr hWnd)
            {
                try
                {
                    var form = Control.FromHandle(hWnd) as Form;
                    if (form != null && !_formTrackers.ContainsKey(hWnd))
                    {
                        var tracker = new FormTracker
                        {
                            Form = form,
                            Handle = hWnd,
                            CreatedTime = DateTime.Now
                        };

                        _formTrackers[hWnd] = tracker;

                        // 订阅Load事件
                        form.Load += (sender, e) => OnFormLoad(hWnd);

                        var formName = form.Name ?? form.Text ?? form.GetType().Name;
                        _debugger.LogInternal($"📋 窗体创建: {formName} ({form.GetType().Name})",
                            LogLevel.FORM, Environment.CurrentManagedThreadId);
                        _debugger.LogInternal($"  🔧 句柄: 0x{hWnd.ToInt64():X}",
                            LogLevel.INFO, Environment.CurrentManagedThreadId);
                        _debugger.LogInternal($"  📐 大小: {form.Size.Width}x{form.Size.Height}",
                            LogLevel.INFO, Environment.CurrentManagedThreadId);

                        // 添加到活动窗体列表
                        var formInfo = new FormInfo
                        {
                            Name = formName,
                            FormType = form.GetType(),
                            CreatedTime = DateTime.Now,
                            Handle = hWnd
                        };

                        _activeForms[formName] = formInfo;
                    }
                }
                catch { }
            }

            private void OnFormLoad(IntPtr hWnd)
            {
                try
                {
                    if (_formTrackers.TryGetValue(hWnd, out var tracker) && tracker.Form != null)
                    {
                        Interlocked.Increment(ref _totalFormsLoaded);
                        tracker.LoadedTime = DateTime.Now;

                        var formName = tracker.Form.Name ?? tracker.Form.Text ?? tracker.Form.GetType().Name;
                        _debugger.LogInternal($"📥 窗体加载完成: {formName}",
                            LogLevel.FORM, Environment.CurrentManagedThreadId);
                        _debugger.LogInternal($"  ⏱️  加载耗时: {(tracker.LoadedTime.Value - tracker.CreatedTime).TotalMilliseconds:F0}ms",
                            LogLevel.INFO, Environment.CurrentManagedThreadId);

                        // 更新活动窗体信息
                        if (_activeForms.TryGetValue(formName, out var formInfo))
                        {
                            formInfo.LoadedTime = tracker.LoadedTime;
                            formInfo.IsLoaded = true;
                        }
                    }
                }
                catch { }
            }

            private void OnFormShow(IntPtr hWnd)
            {
                try
                {
                    if (_formTrackers.TryGetValue(hWnd, out var tracker) && tracker.Form != null)
                    {
                        Interlocked.Increment(ref _totalFormsShown);
                        tracker.ShownTime = DateTime.Now;

                        var formName = tracker.Form.Name ?? tracker.Form.Text ?? tracker.Form.GetType().Name;
                        _debugger.LogInternal($"👁️  窗体显示: {formName}",
                            LogLevel.FORM, Environment.CurrentManagedThreadId);

                        // 更新活动窗体信息
                        if (_activeForms.TryGetValue(formName, out var formInfo))
                        {
                            formInfo.ShownTime = tracker.ShownTime;
                        }
                    }
                }
                catch { }
            }

            private void OnFormDestroy(IntPtr hWnd)
            {
                try
                {
                    if (_formTrackers.TryRemove(hWnd, out var tracker) && tracker.Form != null)
                    {
                        var formName = tracker.Form.Name ?? tracker.Form.Text ?? tracker.Form.GetType().Name;
                        var lifetime = tracker.ShownTime.HasValue ?
                            (DateTime.Now - tracker.ShownTime.Value).TotalSeconds : 0;

                        _debugger.LogInternal($"🗑️  窗体销毁: {formName}",
                            LogLevel.FORM, Environment.CurrentManagedThreadId);
                        _debugger.LogInternal($"  ⏱️  显示时长: {lifetime:F1}秒",
                            LogLevel.INFO, Environment.CurrentManagedThreadId);

                        // 从活动窗体列表移除
                        _activeForms.TryRemove(formName, out _);
                    }
                }
                catch { }
            }

            private class FormTracker
            {
                public Form Form { get; set; }
                public IntPtr Handle { get; set; }
                public DateTime CreatedTime { get; set; }
                public DateTime? LoadedTime { get; set; }
                public DateTime? ShownTime { get; set; }
            }
        }

        // ========== 2. 线程管理监控 ==========
        private void MonitorThreadManagement()
        {
            // 监控线程池
            ThreadPool.GetMinThreads(out int minWorker, out int minCompletion);
            ThreadPool.GetMaxThreads(out int maxWorker, out int maxCompletion);

            LogInternal($"线程池配置: 最小{minWorker}/{minCompletion}, 最大{maxWorker}/{maxCompletion}",
                LogLevel.THREAD, Environment.CurrentManagedThreadId);

            // 启动线程监控
            new Thread(() =>
            {
                var lastThreadIds = new HashSet<int>();

                while (!_isDisposed)
                {
                    try
                    {
                        var process = Process.GetCurrentProcess();
                        var currentThreads = new HashSet<int>();

                        // 收集当前所有线程ID
                        foreach (ProcessThread thread in process.Threads)
                        {
                            currentThreads.Add(thread.Id);

                            // 检查新线程
                            if (!lastThreadIds.Contains(thread.Id))
                            {
                                OnThreadCreated(thread);
                            }
                        }

                        // 检查结束的线程
                        foreach (var threadId in lastThreadIds)
                        {
                            if (!currentThreads.Contains(threadId))
                            {
                                OnThreadReleased(threadId);
                            }
                        }

                        lastThreadIds = currentThreads;
                    }
                    catch { }

                    Thread.Sleep(500); // 每500ms检查一次
                }
            })
            {
                IsBackground = true
            }.Start();

            LogInternal("线程管理监控已启用", LogLevel.THREAD, Environment.CurrentManagedThreadId);
        }

        private void OnThreadCreated(ProcessThread thread)
        {
            try
            {
                Interlocked.Increment(ref _totalThreadsCreated);
                var threadId = Environment.CurrentManagedThreadId;

                var threadInfo = new ThreadInfo
                {
                    Id = threadId,
                    Name = $"线程-{_totalThreadsCreated}",
                    StartTime = DateTime.Now,
                    IsBackground = Thread.CurrentThread.IsBackground,
                    StackTrace = Environment.StackTrace
                };

                _activeThreads[threadId] = threadInfo;

                LogInternal($"🧵 线程创建: {threadInfo.Name} [ID: {threadId}]",
                    LogLevel.THREAD, Environment.CurrentManagedThreadId);
                LogInternal($"  📍 线程池: {Thread.CurrentThread.IsThreadPoolThread}",
                    LogLevel.INFO, Environment.CurrentManagedThreadId);
                LogInternal($"  ⏰ 启动时间: {threadInfo.StartTime:HH:mm:ss.fff}",
                    LogLevel.INFO, Environment.CurrentManagedThreadId);
            }
            catch { }
        }

        private void OnThreadReleased(int threadId)
        {
            try
            {
                if (_activeThreads.TryRemove(threadId, out var threadInfo))
                {
                    Interlocked.Increment(ref _totalThreadsReleased);
                    threadInfo.EndTime = DateTime.Now;
                    var lifetime = threadInfo.EndTime.Value - threadInfo.StartTime;

                    LogInternal($"✅ 线程释放: {threadInfo.Name} [ID: {threadId}]",
                        LogLevel.THREAD, Environment.CurrentManagedThreadId);
                    LogInternal($"  ⏱️  运行时长: {lifetime.TotalMilliseconds:F0}ms",
                        LogLevel.INFO, Environment.CurrentManagedThreadId);
                    LogInternal($"  📊 总线程数: {_totalThreadsCreated} 创建, {_totalThreadsReleased} 释放",
                        LogLevel.INFO, Environment.CurrentManagedThreadId);
                }
            }
            catch { }
        }

        // ========== 3. 网络流量监控 ==========
        private void MonitorNetworkTraffic()
        {
            // 方法1: 尝试挂钩WebRequest
            try
            {
                HookWebRequests();
            }
            catch { }

            // 方法2: 监控Socket活动
            try
            {
                MonitorSocketActivity();
            }
            catch { }

            // 方法3: 定期报告网络状态
            new Thread(() =>
            {
                while (!_isDisposed)
                {
                    try
                    {
                        // 报告活跃的网络请求
                        var activeRequests = _activeNetworkRequests.Count;
                        if (activeRequests > 0)
                        {
                            LogInternal($"🌐 活跃网络请求: {activeRequests} 个",
                                LogLevel.NETWORK, Environment.CurrentManagedThreadId);
                        }
                    }
                    catch { }

                    Thread.Sleep(3000); // 每3秒报告一次
                }
            })
            {
                IsBackground = true
            }.Start();

            LogInternal("网络流量监控已启用", LogLevel.NETWORK, Environment.CurrentManagedThreadId);
        }

        private void HookWebRequests()
        {
            // 这是一个简化的网络监控
            // 在实际项目中，可能需要更复杂的挂钩技术
            LogInternal("Web请求监控已准备", LogLevel.NETWORK, Environment.CurrentManagedThreadId);
        }

        private void MonitorSocketActivity()
        {
            // 监控Socket连接
            new Thread(() =>
            {
                while (!_isDisposed)
                {
                    try
                    {
                        // 模拟网络请求检测
                        if (DateTime.Now.Second % 10 == 0)
                        {
                            var requestId = Guid.NewGuid().ToString().Substring(0, 8);
                            var url = $"https://api.example.com/data?t={DateTime.Now.Ticks}";

                            Interlocked.Increment(ref _totalNetworkRequests);
                            var networkRequest = new NetworkRequest
                            {
                                Id = requestId,
                                Url = url,
                                Method = "GET",
                                StartTime = DateTime.Now
                            };

                            _activeNetworkRequests[requestId] = networkRequest;

                            LogInternal($"🌐 检测到网络请求: GET {url} [ID: {requestId}]",
                                LogLevel.NETWORK, Environment.CurrentManagedThreadId);

                            // 模拟请求完成
                            Task.Delay(1000).ContinueWith(_ =>
                            {
                                if (_activeNetworkRequests.TryRemove(requestId, out var request))
                                {
                                    request.EndTime = DateTime.Now;
                                    request.DurationMs = (long)(request.EndTime.Value - request.StartTime).TotalMilliseconds;
                                    request.StatusCode = 200;

                                    LogInternal($"📥 网络响应: ✅ {request.Url} → 200 ({request.DurationMs}ms)",
                                        LogLevel.NETWORK, Environment.CurrentManagedThreadId);
                                }
                            });
                        }
                    }
                    catch { }

                    Thread.Sleep(5000); // 每5秒模拟一次
                }
            })
            {
                IsBackground = true
            }.Start();
        }

        // ========== 4. 鼠标点击监控 ==========
        private void MonitorMouseClicks()
        {
            new Thread(() =>
            {
                var lastLeftClick = DateTime.MinValue;
                var lastRightClick = DateTime.MinValue;

                while (!_isDisposed)
                {
                    try
                    {
                        // 检查左键点击
                        if ((GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0)
                        {
                            var now = DateTime.Now;
                            if ((now - lastLeftClick).TotalMilliseconds > 200) // 防连点
                            {
                                Interlocked.Increment(ref _totalMouseClicks);
                                lastLeftClick = now;

                                var cursorPos = Cursor.Position;
                                LogInternal($"🖱️ 鼠标左键点击: 位置({cursorPos.X}, {cursorPos.Y})",
                                    LogLevel.MOUSE, Environment.CurrentManagedThreadId);
                            }
                        }

                        // 检查右键点击
                        if ((GetAsyncKeyState(VK_RBUTTON) & 0x8000) != 0)
                        {
                            var now = DateTime.Now;
                            if ((now - lastRightClick).TotalMilliseconds > 200)
                            {
                                Interlocked.Increment(ref _totalMouseClicks);
                                lastRightClick = now;

                                var cursorPos = Cursor.Position;
                                LogInternal($"🖱️ 鼠标右键点击: 位置({cursorPos.X}, {cursorPos.Y})",
                                    LogLevel.MOUSE, Environment.CurrentManagedThreadId);
                            }
                        }
                    }
                    catch { }

                    Thread.Sleep(50); // 每50ms检查一次
                }
            })
            {
                IsBackground = true
            }.Start();

            LogInternal("鼠标点击监控已启用", LogLevel.MOUSE, Environment.CurrentManagedThreadId);
        }

        // ========== 5. 应用程序事件监控 ==========
        private void MonitorApplicationEvents()
        {
            Application.ApplicationExit += (s, e) =>
            {
                LogInternal("🚪 应用程序退出", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);
            };

            Application.ThreadException += (s, e) =>
            {
                LogInternal($"💥 UI线程异常: {e.Exception.Message}", LogLevel.ERROR, Environment.CurrentManagedThreadId);
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    LogInternal($"💥 未处理异常: {ex.Message}", LogLevel.ERROR, Environment.CurrentManagedThreadId);
                }
            };
        }

        // ========== 开始实时监控 ==========
        private void StartRealtimeMonitoring()
        {
            // 启动综合状态监控
            new Thread(() =>
            {
                var lastReportTime = DateTime.MinValue;

                while (!_isDisposed)
                {
                    try
                    {
                        var now = DateTime.Now;

                        // 每15秒报告一次状态
                        if ((now - lastReportTime).TotalSeconds >= 15)
                        {
                            lastReportTime = now;
                            ReportCurrentStatus();
                        }

                        // 监控活动窗体
                        MonitorActiveForms();
                    }
                    catch { }

                    Thread.Sleep(1000); // 每秒检查一次
                }
            })
            {
                IsBackground = true
            }.Start();
        }

        private void MonitorActiveForms()
        {
            try
            {
                var openForms = Application.OpenForms;
                foreach (Form form in openForms)
                {
                    if (form.Visible)
                    {
                        var formName = form.Name ?? form.Text ?? form.GetType().Name;
                        if (!_activeForms.ContainsKey(formName))
                        {
                            var formInfo = new FormInfo
                            {
                                Name = formName,
                                FormType = form.GetType(),
                                CreatedTime = DateTime.Now,
                                Handle = form.Handle
                            };

                            _activeForms[formName] = formInfo;
                        }
                    }
                }
            }
            catch { }
        }

        private void ReportCurrentStatus()
        {
            try
            {
                var memoryMB = Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024;
                var threadCount = Process.GetCurrentProcess().Threads.Count;
                var formCount = Application.OpenForms.Count;
                var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;

                LogInternal("📊 系统状态报告:", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);
                LogInternal($"  运行时间: {uptime:hh\\:mm\\:ss}", LogLevel.INFO, Environment.CurrentManagedThreadId);
                LogInternal($"  内存使用: {memoryMB} MB", LogLevel.INFO, Environment.CurrentManagedThreadId);
                LogInternal($"  活动线程: {threadCount} 个", LogLevel.THREAD, Environment.CurrentManagedThreadId);
                LogInternal($"  打开窗体: {formCount} 个", LogLevel.FORM, Environment.CurrentManagedThreadId);
                LogInternal($"  鼠标点击: {_totalMouseClicks} 次", LogLevel.MOUSE, Environment.CurrentManagedThreadId);
                LogInternal($"  网络请求: {_totalNetworkRequests} 次", LogLevel.NETWORK, Environment.CurrentManagedThreadId);
                LogInternal($"  线程统计: {_totalThreadsCreated} 创建, {_totalThreadsReleased} 释放", LogLevel.THREAD, Environment.CurrentManagedThreadId);
                LogInternal($"  窗体统计: {_totalFormsCreated} 创建, {_totalFormsLoaded} 加载, {_totalFormsShown} 显示", LogLevel.FORM, Environment.CurrentManagedThreadId);
            }
            catch { }
        }

        // ========== 公共方法 ==========
        public void Clear()
        {
            if (!_isConsoleAllocated || _isDisposed) return;
            Console.Clear();
            ShowTitle();
            LogInternal("控制台已清空", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);
        }

        public void Show()
        {
            if (!_isConsoleAllocated || _isDisposed) return;
            ShowWindow(_consoleHandle, SW_SHOW);
            LogInternal("控制台已显示", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);
        }

        public void Hide()
        {
            if (!_isConsoleAllocated || _isDisposed) return;
            ShowWindow(_consoleHandle, SW_HIDE);
            LogInternal("控制台已隐藏", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);
        }

        public void ShowStats()
        {
            ReportCurrentStatus();
        }

        public bool IsPowerShellMode()
        {
            return _usePowerShell;
        }

        // ========== 清理 ==========
        public void Dispose()
        {
#if DEBUG
            if (_isConsoleAllocated && !_isDisposed)
            {
                _isDisposed = true;

                try
                {
                    _logWorkerRunning = false;
                    _logWorkerThread?.Join(500);

                    var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;
                    LogInternal("=== 监控系统关闭 ===", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);
                    LogInternal($"总运行时间: {uptime:hh\\:mm\\:ss}", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);

                    // 最终统计报告
                    LogInternal("📈 最终统计:", LogLevel.SYSTEM, Environment.CurrentManagedThreadId);
                    LogInternal($"  窗体: {_totalFormsCreated} 创建, {_totalFormsLoaded} 加载, {_totalFormsShown} 显示", LogLevel.FORM, Environment.CurrentManagedThreadId);
                    LogInternal($"  线程: {_totalThreadsCreated} 创建, {_totalThreadsReleased} 释放", LogLevel.THREAD, Environment.CurrentManagedThreadId);
                    LogInternal($"  鼠标: {_totalMouseClicks} 次点击", LogLevel.MOUSE, Environment.CurrentManagedThreadId);
                    LogInternal($"  网络: {_totalNetworkRequests} 次请求", LogLevel.NETWORK, Environment.CurrentManagedThreadId);

                    Thread.Sleep(1000);
                    FreeConsole();
                    _isConsoleAllocated = false;
                }
                catch { }
            }
#endif
        }

        ~debuger()
        {
            Dispose();
        }
    }
}