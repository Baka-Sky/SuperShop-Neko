using System;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Management;

namespace SuperShop_Neko
{
    public partial class systeminfo : UserControl
    {
        public systeminfo()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }

        private async void systeminfo_Load(object sender, EventArgs e)
        {
            await GetSystemInfo();
        }

        private async Task GetSystemInfo()
        {
            try
            {
                moreinfo.Text = "正在获取系统信息...";

                var result = await Task.Run(() =>
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("处理器：" + GetCPUInfo());
                    sb.AppendLine("主板：" + GetMotherboardInfo());
                    sb.AppendLine("内存：" + GetMemoryInfo());
                    sb.AppendLine("显卡：" + GetGPUInfo());
                    sb.AppendLine("显示器：" + GetMonitorInfo());
                    sb.AppendLine("磁盘：");
                    sb.Append(GetDiskInfo());
                    sb.AppendLine("声卡：");
                    sb.Append(GetAudioInfo());
                    sb.AppendLine("网卡：");
                    sb.Append(GetNetworkInfo());
                    return sb.ToString();
                });

                moreinfo.Text = result;
                moreinfo.SelectionStart = 0;
                //moreinfo.ScrollToCaret();

                // 加载到三个文本框
                mother.Text = GetMotherboardInfo();
                runtime.Text = GetSystemUptime();
                windows.Text = GetWindowsVersion();
            }
            catch (Exception ex)
            {
                moreinfo.Text = "获取失败：" + ex.Message;
            }
        }

        private string GetCPUInfo()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name,NumberOfCores FROM Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string name = obj["Name"]?.ToString()?.Trim() ?? "";
                        string cores = obj["NumberOfCores"]?.ToString() ?? "";
                        return $"{name} {cores}核";
                    }
                }
            }
            catch { }
            return "未知";
        }

        private string GetMotherboardInfo()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Manufacturer,Product FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string manufacturer = obj["Manufacturer"]?.ToString()?.Trim() ?? "";
                        string product = obj["Product"]?.ToString()?.Trim() ?? "";
                        return $"{manufacturer} {product}";
                    }
                }
            }
            catch { }
            return "未知";
        }

        private string GetMemoryInfo()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                long totalCapacity = 0;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Manufacturer,Capacity,Speed,PartNumber FROM Win32_PhysicalMemory"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string manufacturer = obj["Manufacturer"]?.ToString() ?? "";
                        string capacity = obj["Capacity"]?.ToString();
                        string speed = obj["Speed"]?.ToString() ?? "";

                        if (long.TryParse(capacity, out long size))
                        {
                            totalCapacity += size;
                            double sizeGB = size / (1024.0 * 1024 * 1024);
                            sb.Append($"{manufacturer} {sizeGB:F0}GB");
                            if (!string.IsNullOrEmpty(speed) && speed != "0")
                                sb.Append($" {speed}MHz");
                            sb.Append("; ");
                        }
                    }
                }

                double totalGB = totalCapacity / (1024.0 * 1024 * 1024);
                string memInfo = sb.ToString().TrimEnd(' ', ';');
                return $"{memInfo} (共{totalGB:F0}GB)";
            }
            catch { }
            return "未知";
        }

        private string GetGPUInfo()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name,AdapterRAM FROM Win32_VideoController WHERE CurrentHorizontalResolution != null"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string name = obj["Name"]?.ToString() ?? "";
                        string ram = obj["AdapterRAM"]?.ToString();

                        if (!string.IsNullOrEmpty(name) && !name.Contains("Microsoft"))
                        {
                            if (long.TryParse(ram, out long ramBytes))
                            {
                                double ramGB = ramBytes / (1024.0 * 1024 * 1024);
                                return $"{name} ({ramGB:F0}GB)";
                            }
                            return name;
                        }
                    }
                }
            }
            catch { }
            return "未知";
        }

        private string GetMonitorInfo()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name,ScreenWidth,ScreenHeight FROM Win32_DesktopMonitor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string name = obj["Name"]?.ToString() ?? "";
                        string width = obj["ScreenWidth"]?.ToString() ?? "";
                        string height = obj["ScreenHeight"]?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(width) && !string.IsNullOrEmpty(height))
                            return $"{name} ({width}x{height})";
                        else if (!string.IsNullOrEmpty(name))
                            return name;
                    }
                }
            }
            catch { }
            return "默认显示器";
        }

        private string GetDiskInfo()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Model,Size FROM Win32_DiskDrive"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string model = obj["Model"]?.ToString()?.Trim() ?? "";
                        string size = obj["Size"]?.ToString();

                        if (long.TryParse(size, out long sizeBytes))
                        {
                            double sizeGB = sizeBytes / (1024.0 * 1024 * 1024);
                            sb.AppendLine($"    {model} ({sizeGB:F0}GB)");
                        }
                        else
                        {
                            sb.AppendLine($"    {model}");
                        }
                    }
                }
                return sb.ToString();
            }
            catch { }
            return "    未知";
        }

        private string GetAudioInfo()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_SoundDevice"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string name = obj["Name"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(name) && !name.Contains("Virtual"))
                            sb.AppendLine($"    {name}");
                    }
                }
                return sb.ToString();
            }
            catch { }
            return "    未知";
        }

        private string GetNetworkInfo()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_NetworkAdapter WHERE NetEnabled = True"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string name = obj["Name"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(name) && !name.Contains("Virtual"))
                            sb.AppendLine($"    {name}");
                    }
                }
                return sb.ToString();
            }
            catch { }
            return "    未知";
        }

        private string GetWindowsVersion()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj["Caption"]?.ToString() ?? "未知";
                    }
                }
            }
            catch { }
            return "未知";
        }

        private string GetSystemUptime()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT LastBootUpTime FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string lastBootTime = obj["LastBootUpTime"]?.ToString() ?? "";
                        if (ManagementDateTimeConverter.ToDateTime(lastBootTime) is DateTime bootTime)
                        {
                            TimeSpan uptime = DateTime.Now - bootTime;
                            return $"{uptime.Days}天 {uptime.Hours}小时 {uptime.Minutes}分钟";
                        }
                    }
                }
            }
            catch { }
            return "未知";
        }

        private void moreinfo_Click(object sender, EventArgs e) { }
        private void mother_Click(object sender, EventArgs e) { }
        private void runtime_Click(object sender, EventArgs e) { }
        private void windows_Click(object sender, EventArgs e) { }
    }
}