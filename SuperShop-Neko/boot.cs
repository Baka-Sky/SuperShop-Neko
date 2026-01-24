using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntdUI;
using Microsoft.Win32;

namespace SuperShop_Neko
{
    public partial class boot : AntdUI.Window
    {
        // Windows API 导入
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern int AddFontResource(string lpszFilename);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WriteProfileString(string lpszSection, string lpszKeyName, string lpszString);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_FONTCHANGE = 0x001D;

        // 定义要检查的文件列表
        private readonly string[] _requiredFiles = new string[]
        {
            @"tools\AIDA64\aida64.exe",
            @"tools\ASSSDBenchmark\ASSSDBenchmark.exe",
            @"tools\CoreTemp\Core Temp x64.exe",
            @"tools\CPUZ\cpuz_x64.exe",
            @"tools\CrystalDiskInfo\DiskInfo64S.exe",
            @"tools\CrystalDiskMark\DiskMark64S.exe",
            @"tools\FurMark_win64\FurMark_GUI.exe",
            @"tools\HDTune\HDTune.exe",
            @"tools\iva\iva.exe",
            @"tools\Keyboard Test Utility\Keyboard Test Utility.exe",
            @"tools\RWEverything\Rw.exe",
            @"tools\SSDZ\SSDZ.exe",
            @"tools\ThrottleStop\ThrottleStop.exe",
            @"tools\wPrime\wPrime.exe",
            @"tools\色域检测\monitorinfo.exe",
            @"tools\DiskGenius\DiskGenius.exe",
            @"tools\XIANGQI\xiangqi.exe"
        };

        // MiSans字体列表
        private readonly string[] _misansFonts = new string[]
        {
            "MiSans-Bold",
            "MiSans-Demibold",
            "MiSans-ExtraLight",
            "MiSans-Heavy",
            "MiSans-Light",
            "MiSans-Medium",
            "MiSans-Normal",
            "MiSans-Regular",
            "MiSans-Semibold",
            "MiSans-Thin"
        };

        // 用于临时加载字体
        private PrivateFontCollection _privateFontCollection;
        // 字体安装标志文件路径
        private readonly string _fontInstallFlagFile;

        public boot()
        {
            InitializeComponent();

            // 初始化字体安装标志文件路径
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appDataPath, "SuperShop_Neko");
            Directory.CreateDirectory(appFolder);
            _fontInstallFlagFile = Path.Combine(appFolder, "fonts_installed.flag");
        }

        private async void boot_Load(object sender, EventArgs e)
        {
            // 开始启动检查
            await StartBootCheckAsync();
        }

        private async Task StartBootCheckAsync()
        {
            Console.WriteLine("=== 开始启动检查 ===");

            // 检查MiSans字体
            bool fontsInstalled = await CheckMiSansFontsAsync();

            // 如果字体已安装到系统，重启程序
            if (fontsInstalled)
            {
                // 创建安装标志文件
                File.WriteAllText(_fontInstallFlagFile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                await RestartApplicationAsync();
                return;
            }

            // 继续其他检查
            await CheckWebView2Async();
            await CheckRequiredFilesAsync();
            await CompleteBootAsync();
        }

        private async Task<bool> CheckMiSansFontsAsync()
        {
            Console.WriteLine("步骤1: 检查MiSans字体");

            if (booting != null)
                booting.Text = "MiSans字体检查";

            await Task.Delay(500);

            // 检查字体安装标志
            if (File.Exists(_fontInstallFlagFile))
            {
                Console.WriteLine("检测到字体安装标志，跳过安装检查");

                // 等待一下让系统识别新字体
                await Task.Delay(1000);

                // 删除标志文件
                try { File.Delete(_fontInstallFlagFile); } catch { }

                // 再次检查字体
                bool fontsExist = CheckAllMiSansFonts();
                if (fontsExist)
                {
                    Console.WriteLine("✓ 字体已成功安装");
                    return false;
                }
                else
                {
                    Console.WriteLine("⚠ 字体安装标志存在但字体未找到，尝试重新检查");
                }
            }

            bool allFontsExist = CheckAllMiSansFonts();

            if (!allFontsExist)
            {
                Console.WriteLine("MiSans字体不全，尝试安装字体");

                if (booting != null)
                    booting.Text = "安装MiSans字体";

                // 尝试安装字体到系统
                bool systemInstalled = await InstallMiSansFontsToSystemAsync();

                if (systemInstalled)
                {
                    Console.WriteLine("✓ 字体已安装到系统，准备重启");
                    return true; // 返回true表示需要重启
                }

                // 如果系统安装失败，使用临时加载
                Console.WriteLine("⚠ 系统安装失败，使用临时加载");
                await LoadTempFontsAsync();
            }
            else
            {
                Console.WriteLine("MiSans字体检查通过");
            }

            return false; // 返回false表示不需要重启
        }

        private bool CheckAllMiSansFonts()
        {
            try
            {
                using (InstalledFontCollection installedFonts = new InstalledFontCollection())
                {
                    var installedFontNames = installedFonts.Families.Select(f => f.Name).ToList();

                    Console.WriteLine($"系统已安装字体数量: {installedFontNames.Count}");

                    // 尝试匹配更宽松的字体名称
                    Console.WriteLine("检查MiSans字体:");

                    int foundCount = 0;
                    foreach (var fontName in _misansFonts)
                    {
                        // 尝试多种匹配方式
                        bool exists = installedFontNames.Any(name =>
                            name.Equals(fontName, StringComparison.OrdinalIgnoreCase) ||
                            name.Replace(" ", "").Equals(fontName.Replace("-", ""), StringComparison.OrdinalIgnoreCase) ||
                            (name.Contains("MiSans", StringComparison.OrdinalIgnoreCase) &&
                             name.Contains(fontName.Replace("MiSans-", ""), StringComparison.OrdinalIgnoreCase)) ||
                            (name.Contains("MiSans", StringComparison.OrdinalIgnoreCase) &&
                             name.Contains(fontName.Replace("MiSans-", "").Replace("-", " "), StringComparison.OrdinalIgnoreCase)));

                        if (exists) foundCount++;

                        Console.WriteLine($"  {fontName}: {(exists ? "✓ 已安装" : "✗ 未安装")}");

                        // 显示匹配到的字体名称（用于调试）
                        if (exists)
                        {
                            var matchedFont = installedFontNames.FirstOrDefault(name =>
                                name.Equals(fontName, StringComparison.OrdinalIgnoreCase) ||
                                name.Replace(" ", "").Equals(fontName.Replace("-", ""), StringComparison.OrdinalIgnoreCase));
                            if (!string.IsNullOrEmpty(matchedFont))
                            {
                                Console.WriteLine($"    匹配到: {matchedFont}");
                            }
                        }
                    }

                    // 如果找到大部分字体就认为通过
                    bool fontsExist = foundCount >= _misansFonts.Length * 0.7; // 70%的字体存在就算通过

                    Console.WriteLine($"找到 {foundCount}/{_misansFonts.Length} 种字体");

                    return fontsExist;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查字体时出错: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> InstallMiSansFontsToSystemAsync()
        {
            try
            {
                string appPath = Application.StartupPath;
                string fontFolder = Path.Combine(appPath, "Sans");

                Console.WriteLine($"字体文件夹路径: {fontFolder}");

                if (!Directory.Exists(fontFolder))
                {
                    Console.WriteLine($"✗ 字体文件夹不存在: {fontFolder}");
                    MessageBox.Show($"字体文件夹不存在：{fontFolder}\n请确保Sans文件夹存在并包含MiSans字体文件。",
                        "字体缺失", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // 获取所有字体文件
                var fontFiles = Directory.GetFiles(fontFolder, "*.ttf")
                    .Concat(Directory.GetFiles(fontFolder, "*.ttc"))
                    .Concat(Directory.GetFiles(fontFolder, "*.otf"))
                    .ToArray();

                Console.WriteLine($"找到 {fontFiles.Length} 个字体文件");

                if (fontFiles.Length == 0)
                {
                    Console.WriteLine("✗ 字体文件夹中没有找到字体文件");
                    return false;
                }

                // 检查管理员权限
                if (!IsUserAdministrator())
                {
                    Console.WriteLine("⚠ 需要管理员权限安装字体到系统");

                    // 询问用户是否以管理员权限重启
                    var result = MessageBox.Show(
                        "安装系统字体需要管理员权限。\n\n" +
                        "是否要以管理员权限重新启动程序来安装字体？\n" +
                        "选择'是'将以管理员权限重启，'否'将使用临时字体。",
                        "权限提示",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // 以管理员权限重启
                        RestartAsAdministrator();
                        return false;
                    }

                    return false;
                }

                Console.WriteLine("✓ 当前以管理员身份运行，开始安装字体到系统...");

                if (booting != null)
                    booting.Text = "正在安装字体...";

                int installedCount = 0;
                foreach (var fontFile in fontFiles)
                {
                    try
                    {
                        string fontName = Path.GetFileNameWithoutExtension(fontFile);
                        Console.WriteLine($"安装字体: {fontName}");

                        if (InstallFontToSystem(fontFile))
                        {
                            installedCount++;
                            Console.WriteLine($"  ✓ {fontName} 安装成功");
                        }
                        else
                        {
                            Console.WriteLine($"  ✗ {fontName} 安装失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ✗ 安装字体失败 {Path.GetFileName(fontFile)}: {ex.Message}");
                    }

                    await Task.Delay(100);
                }

                if (installedCount > 0)
                {
                    Console.WriteLine($"✓ 成功安装 {installedCount} 种字体到系统");

                    // 通知系统字体已更改
                    SendMessage((IntPtr)HWND_BROADCAST, WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 安装字体到系统时发生错误: {ex.Message}");
            }

            return false;
        }

        private void RestartAsAdministrator()
        {
            try
            {
                Console.WriteLine("以管理员权限重启程序...");

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    UseShellExecute = true,
                    Verb = "runas" // 请求管理员权限
                };

                System.Diagnostics.Process.Start(startInfo);
                Application.Exit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"重启为管理员失败: {ex.Message}");
            }
        }

        private async Task LoadTempFontsAsync()
        {
            Console.WriteLine("使用临时加载方式...");

            if (booting != null)
                booting.Text = "加载临时字体";

            string appPath = Application.StartupPath;
            string fontFolder = Path.Combine(appPath, "Sans");

            if (!Directory.Exists(fontFolder))
            {
                Console.WriteLine("✗ 字体文件夹不存在");
                return;
            }

            var fontFiles = Directory.GetFiles(fontFolder, "*.ttf")
                .Concat(Directory.GetFiles(fontFolder, "*.ttc"))
                .Concat(Directory.GetFiles(fontFolder, "*.otf"))
                .ToArray();

            if (fontFiles.Length == 0)
            {
                Console.WriteLine("✗ 字体文件夹中没有找到字体文件");
                return;
            }

            _privateFontCollection = new PrivateFontCollection();

            try
            {
                foreach (var fontFile in fontFiles)
                {
                    try
                    {
                        string fontName = Path.GetFileName(fontFile);
                        Console.WriteLine($"临时加载字体: {fontName}");
                        _privateFontCollection.AddFontFile(fontFile);
                        await Task.Delay(50);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ 临时加载字体失败 {Path.GetFileName(fontFile)}: {ex.Message}");
                    }
                }

                Console.WriteLine($"✓ 成功临时加载 {_privateFontCollection.Families.Length} 种字体");

                // 应用字体到除了booting标签外的所有控件
                ApplyFontsToControlsExceptBooting();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 临时加载字体错误: {ex.Message}");
            }

            await Task.Delay(500);
        }

        private bool IsUserAdministrator()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        private bool InstallFontToSystem(string fontFilePath)
        {
            try
            {
                string fontName = Path.GetFileName(fontFilePath);
                string fontDestPath = Path.Combine(
                    Environment.GetEnvironmentVariable("WINDIR"),
                    "Fonts",
                    fontName);

                // 检查是否已安装
                if (!File.Exists(fontDestPath))
                {
                    // 复制字体文件到系统字体目录
                    File.Copy(fontFilePath, fontDestPath, true);
                    Console.WriteLine($"  复制字体文件到: {fontDestPath}");
                }
                else
                {
                    Console.WriteLine($"  字体已存在: {fontDestPath}");
                    return true;
                }

                // 添加字体资源
                int result = AddFontResource(fontDestPath);
                if (result == 0)
                {
                    Console.WriteLine($"  添加字体资源失败");
                    return false;
                }

                // 写入注册表
                string fontKey = Path.GetFileNameWithoutExtension(fontName) + " (TrueType)";
                WriteProfileString("fonts", fontKey, fontName);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  安装字体错误: {ex.Message}");
                return false;
            }
        }

        private void ApplyFontsToControlsExceptBooting()
        {
            if (_privateFontCollection == null || _privateFontCollection.Families.Length == 0)
                return;

            try
            {
                // 获取Regular或Normal字体
                FontFamily regularFamily = null;
                foreach (FontFamily family in _privateFontCollection.Families)
                {
                    if (family.Name.Contains("Regular", StringComparison.OrdinalIgnoreCase) ||
                        family.Name.Contains("Normal", StringComparison.OrdinalIgnoreCase))
                    {
                        regularFamily = family;
                        break;
                    }
                }

                // 如果没有Regular，使用第一个
                if (regularFamily == null)
                {
                    regularFamily = _privateFontCollection.Families[0];
                }

                Console.WriteLine($"应用字体: {regularFamily.Name} (排除booting标签)");

                // 创建字体
                var font = new Font(regularFamily, 9f);

                // 遍历控件设置字体，排除booting标签
                ApplyFontRecursiveExcept(this, font, "booting");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"应用字体时出错: {ex.Message}");
            }
        }

        private void ApplyFontRecursiveExcept(Control parent, Font font, string excludeControlName)
        {
            foreach (Control control in parent.Controls)
            {
                // 跳过booting标签
                if (control.Name == excludeControlName)
                    continue;

                try
                {
                    control.Font = font;
                }
                catch { }

                // 递归处理子控件
                if (control.HasChildren)
                {
                    ApplyFontRecursiveExcept(control, font, excludeControlName);
                }
            }
        }

        private async Task RestartApplicationAsync()
        {
            Console.WriteLine("自动重启程序...");

            if (booting != null)
                booting.Text = "字体安装完成，重启中...";

            // 延迟一下让用户能看到信息
            await Task.Delay(2000);

            // 重启程序
            Application.Restart();
            Environment.Exit(0);
        }

        private async Task CheckWebView2Async()
        {
            Console.WriteLine("\n步骤2: 检查WebView2");

            if (booting != null)
                booting.Text = "WebView2检查";

            await Task.Delay(500);

            bool webView2Installed = CheckWebView2Installation();

            if (!webView2Installed)
            {
                Console.WriteLine("✗ WebView2未安装");

                // 弹出提示
                var result = MessageBox.Show(
                    "系统未检测到WebView2运行时环境。\n\n" +
                    "WebView2是运行本软件的必要组件，是否需要现在安装？\n\n" +
                    "点击'是'前往Microsoft官网下载安装\n" +
                    "点击'否'强制进入（可能导致功能异常）",
                    "WebView2检查",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/");
                    Application.Exit();
                    return;
                }
                else if (result == DialogResult.Cancel)
                {
                    Application.Exit();
                    return;
                }
                Console.WriteLine("用户选择强制进入");
            }
            else
            {
                Console.WriteLine("✓ WebView2已安装");
            }
        }

        private bool CheckWebView2Installation()
        {
            try
            {
                // 检查注册表
                string[] registryPaths = new string[]
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
                            var version = key.GetValue("pv") as string;
                            if (!string.IsNullOrEmpty(version))
                            {
                                Console.WriteLine($"找到WebView2，版本: {version}");
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task CheckRequiredFilesAsync()
        {
            Console.WriteLine("\n步骤3: 检查必要文件");

            if (booting != null)
                booting.Text = "必要文件检查";

            await Task.Delay(500);

            bool allFilesExist = true;
            string appPath = Application.StartupPath;

            Console.WriteLine($"应用程序路径: {appPath}");
            Console.WriteLine("检查必要文件:");

            foreach (var relativePath in _requiredFiles)
            {
                string fullPath = Path.Combine(appPath, relativePath);
                bool exists = File.Exists(fullPath);

                Console.WriteLine($"  {relativePath}: {(exists ? "✓ 存在" : "✗ 缺失")}");

                if (!exists)
                {
                    allFilesExist = false;
                    Console.WriteLine($"    完整路径: {fullPath}");
                }
            }

            if (!allFilesExist)
            {
                Console.WriteLine("✗ 必要文件缺失，无法启动");
                MessageBox.Show(
                    "检测到必要的工具文件缺失，无法启动程序。\n" +
                    "请确保所有必要的文件都已放置在正确的目录中。",
                    "文件缺失",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.Exit();
            }
            else
            {
                Console.WriteLine("✓ 所有必要文件检查通过");
            }
        }

        private async Task CompleteBootAsync()
        {
            Console.WriteLine("\n步骤4: 启动主程序");

            if (booting != null)
                booting.Text = "启动...";

            Console.WriteLine("所有检查完成，正在启动主程序...");

            await Task.Delay(1000);

            this.Hide();
            var mainForm = new Form1();
            mainForm.Show();
        }

        // 清理资源
        protected override void OnClosed(EventArgs e)
        {
            if (_privateFontCollection != null)
            {
                _privateFontCollection.Dispose();
            }
            base.OnClosed(e);
        }

        private const int HWND_BROADCAST = 0xffff;
    }
}