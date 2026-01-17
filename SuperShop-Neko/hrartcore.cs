using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Win32;


namespace SuperShop_Neko
{
    public class Config
    {
        public bool color { get; set; }
        public string RGB { get; set; }

        [JsonIgnore]
        public Color ThemeColor
        {
            get
            {
                if (!string.IsNullOrEmpty(RGB))
                {
                    var parts = RGB.Split(',');
                    if (parts.Length == 3 &&
                        int.TryParse(parts[0], out int r) &&
                        int.TryParse(parts[1], out int g) &&
                        int.TryParse(parts[2], out int b))
                    {
                        return Color.FromArgb(r, g, b);
                    }
                }
                return Color.Black;
            }
            set
            {
                RGB = $"{value.R},{value.G},{value.B}";
            }
        }
    }

    public static class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "config.json"
        );

        public static void EnsureConfigExists()
        {
            if (!File.Exists(ConfigPath))
            {
                var defaultConfig = new Config
                {
                    color = false,
                    RGB = "0,0,0"
                };
                SaveConfig(defaultConfig);
            }
        }

        public static Config LoadConfig()
        {
            EnsureConfigExists();

            try
            {
                string json = File.ReadAllText(ConfigPath);
                var config = JsonSerializer.Deserialize<Config>(json);

                if (config == null)
                {
                    config = new Config { color = false, RGB = "0,0,0" };
                }
                else if (string.IsNullOrEmpty(config.RGB))
                {
                    config.RGB = "0,0,0";
                }

                SaveConfig(config);
                return config;
            }
            catch
            {
                var defaultConfig = new Config { color = false, RGB = "0,0,0" };
                SaveConfig(defaultConfig);
                return defaultConfig;
            }
        }

        public static void SaveConfig(Config config)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(ConfigPath, json);
            }
            catch
            {
                // 保存失败，忽略错误
            }
        }
    }

    public static class WindowsThemeColorHelper
    {
        // 从注册表获取真正的Windows主题色
        public static Color GetWindowsThemeColor()
        {
            try
            {
                // 方法1: 获取现代Windows的主题色（Windows 10/11）
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM"))
                {
                    if (key != null)
                    {
                        // 获取主题色（这是一个ABGR格式的整数）
                        var colorizationColor = key.GetValue("ColorizationColor")?.ToString();
                        if (!string.IsNullOrEmpty(colorizationColor) &&
                            int.TryParse(colorizationColor, out int colorValue))
                        {
                            // ABGR格式转换为RGB
                            // ABGR格式：AARRGGBB，需要转换为ARGB
                            int a = (colorValue >> 24) & 0xFF;
                            int r = (colorValue >> 16) & 0xFF;
                            int g = (colorValue >> 8) & 0xFF;
                            int b = colorValue & 0xFF;

                            return Color.FromArgb(r, g, b);
                        }

                        // 获取主题色（另一种格式）
                        var colorizationColorBalance = key.GetValue("ColorizationColorBalance")?.ToString();
                        if (!string.IsNullOrEmpty(colorizationColorBalance) &&
                            int.TryParse(colorizationColorBalance, out int colorValue2))
                        {
                            // 转换为RGB
                            return Color.FromArgb(
                                (colorValue2 >> 16) & 0xFF,
                                (colorValue2 >> 8) & 0xFF,
                                colorValue2 & 0xFF
                            );
                        }
                    }
                }
            }
            catch { }

            try
            {
                // 方法2: 获取强调色（Windows 10/11）
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent"))
                {
                    if (key != null)
                    {
                        var accentPalette = key.GetValue("AccentPalette") as byte[];
                        if (accentPalette != null && accentPalette.Length >= 32)
                        {
                            // 强调色通常在偏移量0x1C处
                            return Color.FromArgb(
                                accentPalette[28],  // R
                                accentPalette[29],  // G
                                accentPalette[30]   // B
                            );
                        }
                    }
                }
            }
            catch { }

            try
            {
                // 方法3: 获取活动窗口标题栏颜色（传统方法）
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors"))
                {
                    if (key != null)
                    {
                        var activeTitle = key.GetValue("ActiveTitle")?.ToString();
                        if (!string.IsNullOrEmpty(activeTitle))
                        {
                            var rgb = activeTitle.Split(' ');
                            if (rgb.Length >= 3 &&
                                int.TryParse(rgb[0], out int r) &&
                                int.TryParse(rgb[1], out int g) &&
                                int.TryParse(rgb[2], out int b))
                            {
                                return Color.FromArgb(r, g, b);
                            }
                        }
                    }
                }
            }
            catch { }

            try
            {
                // 方法4: 获取Windows主题文件中的颜色
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\History\Colors"))
                {
                    if (key != null)
                    {
                        var colorNames = key.GetValueNames();
                        if (colorNames.Length > 0)
                        {
                            var lastColor = key.GetValue(colorNames[colorNames.Length - 1])?.ToString();
                            if (!string.IsNullOrEmpty(lastColor) && lastColor.Length == 8)
                            {
                                // ARGB十六进制格式
                                return Color.FromArgb(
                                    Convert.ToInt32(lastColor.Substring(2, 2), 16),  // R
                                    Convert.ToInt32(lastColor.Substring(4, 2), 16),  // G
                                    Convert.ToInt32(lastColor.Substring(6, 2), 16)   // B
                                );
                            }
                        }
                    }
                }
            }
            catch { }

            // 默认颜色
            return Color.FromArgb(0, 120, 215); // Windows默认蓝色
        }

        // 获取主题颜色名称（如果有）
        public static string GetThemeColorName()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM"))
                {
                    if (key != null)
                    {
                        var colorPrevalence = key.GetValue("ColorPrevalence")?.ToString();
                        return colorPrevalence ?? "未知";
                    }
                }
            }
            catch { }

            return "默认";
        }
    }

    internal class hrartcore
    {
        // 这里可以添加 hrartcore 类的其他功能
        public static void ApplyThemeColor(Color color)
        {
            // 应用主题色到UI的示例方法
            // 你可以根据实际需求实现
        }

        public static Color GetThemeColorFromConfig()
        {
            var config = ConfigManager.LoadConfig();
            return config.ThemeColor;
        }

        // 调试方法：显示所有注册表颜色值
        public static string GetAllRegistryColors()
        {
            var result = new System.Text.StringBuilder();
            result.AppendLine("注册表中的颜色值:");

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors"))
                {
                    if (key != null)
                    {
                        foreach (string valueName in key.GetValueNames())
                        {
                            var value = key.GetValue(valueName)?.ToString();
                            result.AppendLine($"{valueName}: {value}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.AppendLine($"读取失败: {ex.Message}");
            }

            return result.ToString();
        }
    }
}