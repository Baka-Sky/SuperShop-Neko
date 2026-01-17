using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Web.WebView2.WinForms;

namespace SuperShop_Neko
{
    public partial class viedoplayer : AntdUI.Window
    {
        private string _currentFilePath = "";
        private bool _webViewInitialized = false;

        public viedoplayer()
        {
            InitializeComponent();

            // 初始化时禁用播放按钮
            button1.Enabled = false;

            // 设置按钮文本
            //load.Text = "加载文件";
            //button1.Text = "播放";

            // 初始化WebView2
            InitializeWebViewAsync();
        }

        /// <summary>
        /// 异步初始化WebView2
        /// </summary>
        private async void InitializeWebViewAsync()
        {
            try
            {
                if (webview == null) return;

                // 初始化WebView2
                await webview.EnsureCoreWebView2Async(null);

                // 确保WebView2支持本地文件访问
                webview.CoreWebView2.Settings.IsWebMessageEnabled = true;
                webview.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;

                // 重要：启用本地文件访问
                webview.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;

                _webViewInitialized = true;

                // 初始显示一个简单的页面
                webview.CoreWebView2.Navigate("about:blank");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"播放器初始化失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 加载按钮点击事件 - 打开文件选择对话框
        /// </summary>
        private void load_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "选择音视频文件";
                openFileDialog.Filter = "视频文件|*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.flv;*.webm;" +
                                       "*.mpg;*.mpeg;*.m4v;*.3gp;*.3g2|" +
                                       "音频文件|*.mp3;*.wav;*.aac;*.flac;*.ogg;*.m4a;*.wma;*.ape|" +
                                       "所有文件|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = false;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _currentFilePath = openFileDialog.FileName;

                    // 显示选中的文件信息
                    ShowFileInfo(_currentFilePath);

                    // 重要：选择完了解除禁用 - 启用播放按钮
                    button1.Enabled = true;
                }
            }
        }

        /// <summary>
        /// 显示文件信息
        /// </summary>
        private void ShowFileInfo(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                string fileName = Path.GetFileName(filePath);
                string fileSize = FormatFileSize(fileInfo.Length);
                string fileExtension = Path.GetExtension(filePath).ToUpper();

                // 更新窗体标题
                this.Text = $"视频播放器 - {fileName}";

                // 更新状态显示（如果有label控件）
                if (label1 != null)
                {
                    label1.Text = $"已选择: {fileName} ({fileSize})";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取文件信息失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double len = bytes;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        /// <summary>
        /// 播放按钮点击事件 - 直接加载本地文件到WebView2
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                MessageBox.Show("请先选择文件", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(_currentFilePath))
            {
                MessageBox.Show("文件不存在或已被删除", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                button1.Enabled = false;
                return;
            }

            if (!_webViewInitialized)
            {
                MessageBox.Show("播放器未初始化完成，请稍候...", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // 重要：直接加载本地文件！
                // 转换为 file:/// 协议格式
                string fileUri = new Uri(_currentFilePath).AbsoluteUri;

                // 直接导航到本地文件
                webview.CoreWebView2.Navigate(fileUri);

                // 更新状态
                if (label1 != null)
                {
                    label1.Text = "正在播放: " + Path.GetFileName(_currentFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法播放文件: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}