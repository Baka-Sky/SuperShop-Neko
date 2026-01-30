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
        // 主题设置
        public bool color { get; set; }
        public string RGB { get; set; }

        // 用户信息
        public string name { get; set; } = "";
        public string password { get; set; } = "";
        public string user_id { get; set; } = "";
        public string uploader_name { get; set; } = "";
        public string su { get; set; } = "None";

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
                    RGB = "0,0,0",
                    name = "",
                    password = "",
                    user_id = "",
                    uploader_name = "",
                    su = "None"
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
                    config = new Config
                    {
                        color = false,
                        RGB = "0,0,0",
                        name = "",
                        password = "",
                        user_id = "",
                        uploader_name = "",
                        su = "None"
                    };
                }
                else
                {
                    // 确保所有字段都有默认值
                    config.name = config.name ?? "";
                    config.password = config.password ?? "";
                    config.user_id = config.user_id ?? "";
                    config.uploader_name = config.uploader_name ?? "";
                    config.su = config.su ?? "None";
                    config.RGB = config.RGB ?? "0,0,0";
                }

                return config;
            }
            catch
            {
                var defaultConfig = new Config
                {
                    color = false,
                    RGB = "0,0,0",
                    name = "",
                    password = "",
                    user_id = "",
                    uploader_name = "",
                    su = "None"
                };
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
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(ConfigPath, json);
            }
            catch
            {
                // 保存失败，忽略错误
            }
        }

        // 更新用户信息（不覆盖主题设置）
        public static void UpdateUserInfo(string username, string password, string userId = "", string uploaderName = "", string su = "None")
        {
            var config = LoadConfig();
            config.name = username;
            config.password = password;

            if (!string.IsNullOrEmpty(userId))
                config.user_id = userId;

            if (!string.IsNullOrEmpty(uploaderName))
                config.uploader_name = uploaderName;

            if (!string.IsNullOrEmpty(su))
                config.su = su;

            SaveConfig(config);
        }

        // 更新主题设置（不覆盖用户信息）
        public static void UpdateTheme(bool useTheme, Color themeColor)
        {
            var config = LoadConfig();
            config.color = useTheme;
            config.ThemeColor = themeColor;
            SaveConfig(config);
        }

        // 清除用户信息（保留主题设置）
        public static void ClearUserInfo()
        {
            var config = LoadConfig();
            config.name = "";
            config.password = "";
            config.user_id = "";
            config.uploader_name = "";
            config.su = "None";
            SaveConfig(config);
        }

        // 获取当前用户信息
        public static (string name, string password, string userId, string uploaderName, string su) GetCurrentUser()
        {
            var config = LoadConfig();
            return (config.name, config.password, config.user_id, config.uploader_name, config.su);
        }

        // 检查是否有保存的用户
        public static bool HasSavedUser()
        {
            var config = LoadConfig();
            return !string.IsNullOrEmpty(config.name) && !string.IsNullOrEmpty(config.password);
        }

        // 检查是否是管理员
        public static bool IsSuperUser()
        {
            var config = LoadConfig();
            return config.su == "Super";
        }
    }

    public static class WindowsThemeColorHelper
    {
        // 从注册表获取真正的Windows主题色
        public static Color GetWindowsThemeColor()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM"))
                {
                    if (key != null)
                    {
                        var colorizationColor = key.GetValue("ColorizationColor")?.ToString();
                        if (!string.IsNullOrEmpty(colorizationColor) &&
                            int.TryParse(colorizationColor, out int colorValue))
                        {
                            int r = (colorValue >> 16) & 0xFF;
                            int g = (colorValue >> 8) & 0xFF;
                            int b = colorValue & 0xFF;

                            return Color.FromArgb(r, g, b);
                        }

                        var colorizationColorBalance = key.GetValue("ColorizationColorBalance")?.ToString();
                        if (!string.IsNullOrEmpty(colorizationColorBalance) &&
                            int.TryParse(colorizationColorBalance, out int colorValue2))
                        {
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
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent"))
                {
                    if (key != null)
                    {
                        var accentPalette = key.GetValue("AccentPalette") as byte[];
                        if (accentPalette != null && accentPalette.Length >= 32)
                        {
                            return Color.FromArgb(
                                accentPalette[28],
                                accentPalette[29],
                                accentPalette[30]
                            );
                        }
                    }
                }
            }
            catch { }

            try
            {
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
                                return Color.FromArgb(
                                    Convert.ToInt32(lastColor.Substring(2, 2), 16),
                                    Convert.ToInt32(lastColor.Substring(4, 2), 16),
                                    Convert.ToInt32(lastColor.Substring(6, 2), 16)
                                );
                            }
                        }
                    }
                }
            }
            catch { }

            return Color.FromArgb(0, 120, 215);
        }

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
}