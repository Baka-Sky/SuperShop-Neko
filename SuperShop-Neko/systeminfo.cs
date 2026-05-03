using System;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Management;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace SuperShop_Neko
{
    public partial class systeminfo : UserControl
    {
        // 显示器相关 API 声明
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        private static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, uint Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, uint Property, ref uint PropertyRegDataType, IntPtr PropertyBuffer, uint PropertyBufferSize, ref uint RequiredSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAY_DEVICE
        {
            public uint cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public uint StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        private const uint DISPLAY_DEVICE_ACTIVE = 0x00000001;
        private const uint DISPLAY_DEVICE_PRIMARY_DEVICE = 0x00000004;

        // 显示器类 GUID - 改为只读字段，使用时复制到局部变量
        private static readonly Guid GUID_DISPLAY_DEVICE = new Guid("{4d36e96e-e325-11ce-bfc1-08002be10318}");

        private const uint DIF_PROPERTY_DISPLAY_NAME = 0x0000000A;
        private const uint DIGCF_PRESENT = 0x00000002;

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
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT Name, NumberOfCores, NumberOfLogicalProcessors FROM Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string name = obj["Name"]?.ToString()?.Trim() ?? "";
                        uint cores = obj["NumberOfCores"] != null ? (uint)obj["NumberOfCores"] : 0;
                        uint logical = obj["NumberOfLogicalProcessors"] != null ? (uint)obj["NumberOfLogicalProcessors"] : 0;

                        name = System.Text.RegularExpressions.Regex.Replace(name, @"\s+", " ");

                        if (cores > 0 && logical > 0 && logical > cores)
                        {
                            return $"{name} {cores}核{logical}线程";
                        }
                        else if (cores > 0)
                        {
                            return $"{name} {cores}核";
                        }
                        return name;
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

        // 需要过滤的虚拟显卡关键字
        private readonly string[] VirtualGPUKeywords = new string[]
        {
            "AskLinkIddDriver", "OrayIddDriver", "Oray",
            "Remote", "Virtual", "Indirect", "Mirror", "RDP", "Citrix",
            "TeamViewer", "VNC", "Sunshine", "Moonlight", "Parsec",
            "IddDriver", "IndirectDisplay", "USB Display"
        };

        private bool IsVirtualGPU(string name)
        {
            if (string.IsNullOrEmpty(name)) return true;
            foreach (string keyword in VirtualGPUKeywords)
            {
                if (name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private bool IsRealGPU(string name)
        {
            if (IsVirtualGPU(name)) return false;

            string lowerName = name.ToLower();
            string[] realBrands = { "intel", "nvidia", "amd", "radeon", "geforce", "quadro", "iris", "uhd", "rtx", "gtx", "rx" };

            foreach (string brand in realBrands)
            {
                if (lowerName.Contains(brand))
                    return true;
            }

            return !IsVirtualGPU(name);
        }

        private string GetGPUInfo()
        {
            try
            {
                List<GPUInfo> gpuList = new List<GPUInfo>();

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT Name, AdapterRAM, AdapterCompatibility, DriverVersion, VideoProcessor FROM Win32_VideoController"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string name = obj["Name"]?.ToString() ?? "";
                        string compatibility = obj["AdapterCompatibility"]?.ToString() ?? "";

                        if (string.IsNullOrEmpty(name)) continue;

                        GPUInfo gpu = new GPUInfo();
                        gpu.Name = name;
                        gpu.Compatibility = compatibility;

                        if (obj["AdapterRAM"] != null && long.TryParse(obj["AdapterRAM"].ToString(), out long ram))
                        {
                            gpu.RAMBytes = ram;
                        }

                        gpuList.Add(gpu);
                    }
                }

                var sortedGPUs = gpuList.OrderBy(g => {
                    string lowerName = g.Name.ToLower();
                    if (lowerName.Contains("intel")) return 0;
                    if (lowerName.Contains("nvidia")) return 1;
                    if (lowerName.Contains("amd")) return 2;
                    if (lowerName.Contains("radeon")) return 3;
                    if (IsVirtualGPU(g.Name)) return 100;
                    return 50;
                }).ToList();

                var realGPU = sortedGPUs.FirstOrDefault(g => IsRealGPU(g.Name));

                if (realGPU != null)
                {
                    return FormatGPUInfo(realGPU.Name, realGPU.RAMBytes);
                }

                var fallback = sortedGPUs.FirstOrDefault(g => !IsVirtualGPU(g.Name));
                if (fallback != null)
                {
                    return FormatGPUInfo(fallback.Name, fallback.RAMBytes);
                }

                var last = gpuList.FirstOrDefault(g => !string.IsNullOrEmpty(g.Name));
                if (last != null)
                    return last.Name;
            }
            catch { }
            return "未知";
        }

        private class GPUInfo
        {
            public string Name { get; set; } = "";
            public string Compatibility { get; set; } = "";
            public long RAMBytes { get; set; } = 0;
        }

        private string FormatGPUInfo(string name, long ramBytes)
        {
            name = System.Text.RegularExpressions.Regex.Replace(name, @"\s+", " ");

            if (ramBytes > 0 && ramBytes != long.MaxValue)
            {
                double ramGB = ramBytes / (1024.0 * 1024 * 1024);
                if (ramGB < 1)
                {
                    double ramMB = ramBytes / (1024.0 * 1024);
                    if (ramMB > 0)
                        return $"{name} ({ramMB:F0}MB)";
                }
                else if (ramGB < 100)
                {
                    return $"{name} ({ramGB:F1}GB)";
                }
            }
            return name;
        }

        private string GetMonitorInfo()
        {
            string monitorName = GetMonitorNameFromSetupAPI();
            if (!string.IsNullOrEmpty(monitorName) &&
                monitorName != "Generic PnP Monitor" &&
                monitorName != "通用即插即用监视器")
            {
                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;
                return $"{monitorName} ({screenWidth}x{screenHeight})";
            }

            monitorName = GetMonitorNameFromEDID();
            if (!string.IsNullOrEmpty(monitorName))
            {
                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;
                return $"{monitorName} ({screenWidth}x{screenHeight})";
            }

            try
            {
                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;
                return $"通用即插即用监视器 ({screenWidth}x{screenHeight})";
            }
            catch { }
            return "通用即插即用监视器";
        }

        private string GetMonitorNameFromSetupAPI()
        {
            try
            {
                // 关键修复：将 static readonly GUID 复制到局部变量
                Guid guid = GUID_DISPLAY_DEVICE;
                IntPtr deviceInfoSet = SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT);

                if (deviceInfoSet == IntPtr.Zero)
                    return null;

                SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
                deviceInfoData.cbSize = (uint)Marshal.SizeOf(deviceInfoData);

                for (uint memberIndex = 0; SetupDiEnumDeviceInfo(deviceInfoSet, memberIndex, ref deviceInfoData); memberIndex++)
                {
                    uint propertyRegDataType = 0;
                    uint requiredSize = 0;

                    SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, DIF_PROPERTY_DISPLAY_NAME,
                        ref propertyRegDataType, IntPtr.Zero, 0, ref requiredSize);

                    if (requiredSize > 0)
                    {
                        IntPtr buffer = Marshal.AllocHGlobal((int)requiredSize);
                        try
                        {
                            if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, DIF_PROPERTY_DISPLAY_NAME,
                                ref propertyRegDataType, buffer, requiredSize, ref requiredSize))
                            {
                                string name = Marshal.PtrToStringUni(buffer);
                                if (!string.IsNullOrEmpty(name) &&
                                    name != "Generic PnP Monitor" &&
                                    name != "通用即插即用监视器")
                                {
                                    return name;
                                }
                            }
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(buffer);
                        }
                    }
                }

                SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }
            catch { }
            return null;
        }

        private string GetMonitorNameFromEDID()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"\\.\ROOT\WMI",
                    "SELECT ProductName, ManufacturerName, UserFriendlyName FROM WmiMonitorID"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        if (obj["UserFriendlyName"] != null)
                        {
                            ushort[] nameArray = obj["UserFriendlyName"] as ushort[];
                            if (nameArray != null && nameArray.Length > 0)
                            {
                                string name = Encoding.Unicode.GetString(Array.ConvertAll(nameArray, Convert.ToByte)).TrimEnd('\0');
                                if (!string.IsNullOrEmpty(name) && !name.Contains("\0"))
                                    return name;
                            }
                        }

                        if (obj["ProductName"] != null)
                        {
                            ushort[] productArray = obj["ProductName"] as ushort[];
                            if (productArray != null && productArray.Length > 0)
                            {
                                string product = Encoding.Unicode.GetString(Array.ConvertAll(productArray, Convert.ToByte)).TrimEnd('\0');
                                if (!string.IsNullOrEmpty(product) && !product.Contains("\0"))
                                {
                                    if (obj["ManufacturerName"] != null)
                                    {
                                        ushort[] manuArray = obj["ManufacturerName"] as ushort[];
                                        if (manuArray != null && manuArray.Length > 0)
                                        {
                                            string manufacturer = Encoding.Unicode.GetString(Array.ConvertAll(manuArray, Convert.ToByte)).TrimEnd('\0');
                                            if (!string.IsNullOrEmpty(manufacturer))
                                                return $"{manufacturer} {product}".Trim();
                                        }
                                    }
                                    return product;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return null;
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
                        if (!string.IsNullOrEmpty(name) &&
                            !name.Contains("Virtual") &&
                            !name.Contains("IddDriver"))
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