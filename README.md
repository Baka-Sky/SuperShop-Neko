# SuperShop Neko - 超级小铺 Neko

<table> <tr> <td><img src="https://img.shields.io/badge/.NET-10-512BD4" alt=".NET 10"></td> <td><img src="https://img.shields.io/badge/Windows-10/11-0078D6" alt="Windows"></td> <td><img src="https://img.shields.io/badge/C%23-12.0-239120" alt="C# 12"></td> <td><img src="https://img.shields.io/badge/WebView2-1.0.0-0078D6" alt="WebView2"></td> </tr> <tr> <td><img src="https://img.shields.io/badge/MySQL-Database-4479A1" alt="MySQL"></td> <td><img src="https://img.shields.io/badge/WinForms-UI-0078D4" alt="WinForms"></td> <td><img src="https://img.shields.io/badge/异步编程-支持-61DAFB" alt="异步编程"></td> <td><img src="https://img.shields.io/badge/SOAP-Web服务-FF6D01" alt="SOAP"></td> </tr> </table>


## 🎯 项目简介

SuperShop Neko（超级小铺Neko）是一个功能强大的桌面应用程序工具集，专为硬件测试、系统监控和软件管理而设计。该项目采用了现代化的UI设计，集成了多种实用工具和功能模块，基于 .NET 10 开发。

## ✨ 主要特性

### 🛠️ 工具模块 (`tools`)
- **硬件检测工具**：CPU-Z、AIDA64、Core Temp、HDTune等
- **性能测试工具**：FurMark、CrystalDiskMark、wPrime、国际象棋跑分
- **系统监控工具**：磁盘信息检测、屏幕色域检测、键盘测试工具
- **批量启动功能**：支持一键启动所有测试工具
- **UAC提权支持**：自动识别需要管理员权限的工具

### 📦 应用商店模块 (`app`)
- **软件下载管理**：集中管理各类软件下载链接
- **数据库集成**：MySQL数据库存储软件信息
- **上传/删除功能**：支持软件信息的上传和删除
- **出处查看**：支持查看软件来源和出处链接

### 🏠 欢迎页面 (`welcome`)
- **Bing集成**：内置WebView2显示Bing搜索
- **天气查询**：支持全国城市天气查询
- **更新检测**：自动检测软件更新信息
- **多服务器支持**：支持多个更新服务器切换

### 🎥 视频播放器 (`viedoplayer`)
- **本地文件播放**：支持多种音视频格式
- **WebView2渲染**：高质量视频播放体验
- **文件选择对话框**：方便的文件选择功能

### 🎨 更多功能 (`more`)
- **设置面板**：主题色配置、功能开关
- **版本信息**：详细的系统信息展示
- **调试工具**：内置调试和崩溃测试功能

## 🏗️ 技术架构

### 核心技术
- **.NET 10**：现代化.NET平台
- **AntdUI**：现代化UI组件库
- **WebView2**：现代浏览器控件集成
- **MySQL**：数据存储和管理
- **SOAP Web Services**：天气查询服务
- **WinForms**：传统桌面应用界面

### 项目结构
```
SuperShop_Neko/
├── Form1.cs           # 主窗体控制器
├── welcome.cs         # 欢迎页面
├── app.cs             # 应用商店模块
├── tools.cs           # 工具管理模块
├── more.cs            # 设置和更多功能
├── viedoplayer.cs     # 视频播放器
├── upload.cs          # 软件上传功能
├── delete.cs          # 软件删除功能
├── error.cs           # 错误处理界面
├── heartengine.cs     # 核心引擎（天气、更新等）
├── debuger.cs         # 调试监控系统
└── FuckWelcomeHDPI.cs # DPI适配工具
```

## 🚀 快速开始

### 环境要求
- Windows 10/11 操作系统
- .NET 10 SDK 或运行时
- Microsoft Edge WebView2 运行时
- MySQL 数据库服务器（可选，用于软件管理功能）

### 编译和运行
1. 克隆项目到本地：
```bash
git clone <repository-url>
cd SuperShop_Neko
```

2. 还原NuGet包：
```bash
dotnet restore
```

3. 编译项目：
```bash
dotnet build
```

4. 运行应用程序：
```bash
dotnet run
```

## ⚙️ 配置说明

### 数据库配置
如果需要使用软件上传/下载功能，请配置数据库连接：
1. 修改 `upload.cs` 和 `app.cs` 中的 `mysqlcon` 连接字符串：
```csharp
public static string mysqlcon = "server=your-server;database=supershop;user=your-user;password=your-password;";
```

### 主题配置
应用程序支持自定义主题色，配置保存在 `config.json`：
```json
{
  "color": true,
  "RGB": "255,100,100"
}
```

## 📁 目录结构

```
SuperShop_Neko/
├── Tools/                    # 硬件测试工具目录
│   ├── AIDA64/
│   ├── CPUZ/
│   ├── CrystalDiskInfo/
│   ├── FurMark_win64/
│   └── ...其他工具
├── Properties/              # 项目属性
├── Resources/              # 资源文件（图标等）
└── *.cs                    # C#源代码文件
```

## 🔧 主要功能模块详解

### 1. 主题系统
- **动态主题色**：支持自定义RGB颜色主题
- **按钮状态管理**：激活按钮高亮显示
- **实时刷新**：主题变更即时生效

### 2. 工具启动器
- **异步启动**：非阻塞式进程启动
- **权限管理**：自动UAC提权
- **错误处理**：完善的异常处理和状态反馈

### 3. 软件管理
- **增删改查**：完整的软件信息管理
- **链接验证**：自动补全HTTP协议
- **多线程操作**：数据库操作不阻塞UI

### 4. 调试系统
- **实时监控**：窗体生命周期、线程、网络、鼠标点击
- **日志系统**：分级日志输出
- **性能统计**：内存、CPU、网络使用情况

### 5. 错误处理
- **优雅崩溃**：详细的错误信息展示
- **恢复选项**：重启或退出选择
- **信息复制**：支持错误信息复制

## 🎨 UI设计特点

1. **现代化界面**：采用AntdUI组件库，界面美观
2. **响应式布局**：自动适应不同DPI设置
3. **图标系统**：丰富的图标资源
4. **水印系统**：Debug模式下显示构建信息

## 🔍 调试和开发

### 调试模式
在Debug模式下，应用程序会：
1. 显示调试水印
2. 启用全局监控系统
3. 输出详细日志信息

### 测试功能
应用程序内置测试功能：
1. **崩溃测试**：测试异常处理机制
2. **卡死测试**：测试UI响应性
3. **批量启动**：测试工具启动功能

## 📱 兼容性

### 操作系统
- Windows 10 (1809及以上)
- Windows 11 (所有版本)

### 屏幕DPI
- 支持100%-400% DPI缩放
- 自动适配高DPI显示器

### 运行环境
- .NET 10 运行时
- WebView2 运行时
- 管理员权限（部分功能需要）

