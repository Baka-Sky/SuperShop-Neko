using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;

namespace SuperShop_Neko
{
    public partial class user : UserControl
    {
        // 当前登录用户信息
        private string currentUserName = "";
        private string currentUserUploaderName = "";
        private string currentUserId = "";
        private string currentUserSu = "None";

        // 直接引用Designer中的控件
        private AntdUI.Input txtWho = null;  // 用户名输入框
        private AntdUI.Input txtPassword = null;  // 密码输入框
        private AntdUI.Button btnLogin = null;  // 登录按钮
        private AntdUI.Button btnReg = null;  // 注册按钮

        public user()
        {
            InitializeComponent();

            // 初始化控件引用 - 直接使用Designer中的控件
            InitializeControlReferences();

            // 确保初始状态正确
            ShowLoginControls();
            LoadUserConfig();
        }

        // 初始化控件引用
        private void InitializeControlReferences()
        {
            // 直接在userpanel中查找控件
            txtWho = FindAntdInput("who", userpanel);
            txtPassword = FindAntdInput("password", userpanel);
            btnLogin = FindAntdButton("login", userpanel);
            btnReg = FindAntdButton("reg", userpanel);

            DebugInfo($"找到控件 - who: {(txtWho != null ? "是" : "否")}, password: {(txtPassword != null ? "是" : "否")}, login: {(btnLogin != null ? "是" : "否")}, reg: {(btnReg != null ? "是" : "否")}");
        }

        // 查找AntdUI.Input控件
        private AntdUI.Input FindAntdInput(string controlName, Control container)
        {
            if (container == null) return null;

            foreach (Control control in container.Controls)
            {
                if (control.Name == controlName && control is AntdUI.Input)
                {
                    return control as AntdUI.Input;
                }
            }

            return null;
        }

        // 查找AntdUI.Button控件
        private AntdUI.Button FindAntdButton(string controlName, Control container)
        {
            if (container == null) return null;

            foreach (Control control in container.Controls)
            {
                if (control.Name == controlName && control is AntdUI.Button)
                {
                    return control as AntdUI.Button;
                }
            }

            return null;
        }

        // 显示登录控件（用户名、密码、登录按钮、注册按钮）
        private void ShowLoginControls()
        {
            DebugInfo("显示登录控件");

            // 显示userpanel本身
            if (userpanel != null)
            {
                userpanel.Visible = true;
                userpanel.Enabled = true;
                DebugInfo($"userpanel可见性: {userpanel.Visible}");
            }

            // 隐藏用户信息控件
            username.Visible = false;
            upwho.Visible = false;
            userid.Visible = false;
            day.Visible = false;
            ifsu.Visible = false;
            exit.Visible = false;

            DebugInfo("登录控件显示完成");
        }

        // 显示用户信息控件（隐藏登录控件）
        private void ShowUserInfoControls()
        {
            DebugInfo("显示用户信息控件");

            // 隐藏userpanel本身
            if (userpanel != null)
            {
                userpanel.Visible = false;
                DebugInfo($"userpanel隐藏");
            }

            // 显示用户信息控件
            username.Visible = true;
            upwho.Visible = true;
            userid.Visible = true;
            day.Visible = true;
            ifsu.Visible = true;
            exit.Visible = true;

            DebugInfo("用户信息控件显示完成");
        }

        // 加载用户配置
        private void LoadUserConfig()
        {
            try
            {
                DebugInfo("开始加载用户配置");

                var (name, pwd, userId, uploaderName, su) = ConfigManager.GetCurrentUser();

                DebugInfo($"从配置获取的用户名: '{name}', 密码长度: {pwd?.Length ?? 0}");

                // 设置用户名和密码框的值
                if (txtWho != null)
                {
                    txtWho.Text = name ?? "";
                    DebugInfo($"设置who文本框: '{txtWho.Text}'");
                }
                else
                {
                    DebugInfo("who文本框为null");
                }

                if (txtPassword != null)
                {
                    txtPassword.Text = pwd ?? "";
                    DebugInfo($"设置password文本框长度: {txtPassword.Text.Length}");
                }
                else
                {
                    DebugInfo("password文本框为null");
                }

                // 如果已有用户信息，尝试自动登录
                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(pwd))
                {
                    DebugInfo("有保存的用户信息，尝试自动登录");
                    AutoLogin();
                }
                else
                {
                    DebugInfo("无保存的用户信息，显示登录界面");
                    // 未保存登录信息，显示登录界面
                    ShowLoginControls();
                }
            }
            catch (Exception ex)
            {
                DebugInfo($"加载用户配置时出错: {ex.Message}");
                // 忽略错误，显示登录界面
                ShowLoginControls();
            }
        }

        // 自动登录
        private async void AutoLogin()
        {
            DebugInfo("开始自动登录");

            var (name, pwd, userId, uploaderName, su) = ConfigManager.GetCurrentUser();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(pwd))
            {
                DebugInfo("自动登录：用户名或密码为空");
                ShowLoginControls();
                return;
            }

            try
            {
                // 准备登录数据
                var loginData = new Dictionary<string, string>
                {
                    { "用户名", name },
                    { "密码", pwd }
                };

                // 使用 AuthHelper 发送带鉴权的请求
                var response = await AuthHelper.SendAuthPostRequest("/user/verify_credentials", loginData);

                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(responseText))
                    {
                        JsonElement root = doc.RootElement;

                        if (root.TryGetProperty("success", out JsonElement successElement) &&
                            successElement.GetBoolean() &&
                            root.TryGetProperty("authenticated", out JsonElement authElement) &&
                            authElement.GetBoolean())
                        {
                            // 登录成功
                            if (root.TryGetProperty("user", out JsonElement userElement))
                            {
                                string newUserId = userElement.TryGetProperty("UserID", out JsonElement idElement)
                                    ? idElement.GetString() ?? ""
                                    : "";
                                string userName = userElement.TryGetProperty("用户名", out JsonElement nameElement)
                                    ? nameElement.GetString() ?? name
                                    : name;
                                string newUploaderName = userElement.TryGetProperty("上传者代称", out JsonElement uploaderElement)
                                    ? uploaderElement.GetString() ?? ""
                                    : "";
                                string suValue = userElement.TryGetProperty("Su", out JsonElement suElement)
                                    ? suElement.GetString() ?? "None"
                                    : "None";

                                DebugInfo($"自动登录成功: {userName}, UserID: {newUserId}");

                                // 保存用户信息
                                currentUserName = userName;
                                currentUserUploaderName = newUploaderName;
                                currentUserId = newUserId;
                                currentUserSu = suValue;

                                // 只更新用户相关配置
                                UpdateUserConfigOnly(userName, pwd, newUserId, newUploaderName, suValue);

                                // 显示用户信息，隐藏登录控件
                                ShowUserInfo();
                                ShowUserInfoControls();

                                DebugInfo("自动登录完成，显示用户信息");
                            }
                        }
                        else
                        {
                            DebugInfo("自动登录失败：服务器返回认证失败");
                            // 自动登录失败，显示登录界面
                            ShowLoginControls();
                        }
                    }
                }
                else
                {
                    DebugInfo($"自动登录失败：HTTP状态码 {response.StatusCode}");
                    // 自动登录失败，显示登录界面
                    ShowLoginControls();
                }
            }
            catch (Exception ex)
            {
                DebugInfo($"自动登录时出错: {ex.Message}");
                // 自动登录失败，显示登录界面
                ShowLoginControls();
            }
        }

        /// <summary>
        /// 只更新用户相关的配置，不覆盖其他字段
        /// </summary>
        private void UpdateUserConfigOnly(string username, string password, string userId, string uploaderName, string su)
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                // 读取现有配置
                Dictionary<string, object> configDict;
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    configDict = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                        ?? new Dictionary<string, object>();
                }
                else
                {
                    configDict = new Dictionary<string, object>();
                }

                // 更新用户相关字段
                configDict["name"] = username;
                configDict["password"] = password;
                configDict["user_id"] = userId;
                configDict["uploader_name"] = uploaderName;
                configDict["su"] = su;

                // 保留所有其他字段不变
                // color, RGB, Version 等字段都保持不变

                // 写回文件
                string newJson = JsonSerializer.Serialize(configDict, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(configPath, newJson);

                DebugInfo($"用户配置已更新: {username}, su={su}");
            }
            catch (Exception ex)
            {
                DebugInfo($"更新用户配置失败: {ex.Message}");
            }
        }

        // 手动登录按钮点击事件
        private async void login_Click(object sender, EventArgs e)
        {
            DebugInfo("=== 开始手动登录 ===");

            string username = "";
            string userPassword = "";

            // 方法1：直接使用Designer生成的控件引用
            if (who != null)
            {
                username = who.Text.Trim();
                DebugInfo($"从who控件获取用户名: '{username}'");
            }

            if (password != null)
            {
                userPassword = password.Text.Trim();
                DebugInfo($"从password控件获取密码，长度: {userPassword.Length}");
            }

            // 方法2：如果方法1没获取到，使用我们找到的引用
            if (string.IsNullOrEmpty(username) && txtWho != null)
            {
                username = txtWho.Text.Trim();
                DebugInfo($"从txtWho引用获取用户名: '{username}'");
            }

            if (string.IsNullOrEmpty(userPassword) && txtPassword != null)
            {
                userPassword = txtPassword.Text.Trim();
                DebugInfo($"从txtPassword引用获取密码，长度: {userPassword.Length}");
            }

            // 方法3：如果还是没获取到，直接从userpanel中查找
            if (string.IsNullOrEmpty(username) && userpanel != null)
            {
                var inputControls = GetAllAntdInputs(userpanel);
                DebugInfo($"在userpanel中找到 {inputControls.Count} 个AntdUI.Input控件");

                foreach (var control in inputControls)
                {
                    DebugInfo($"控件: {control.Name}, 文本: '{control.Text}'");
                    if (control.Name == "who" || control.Name.Contains("who"))
                    {
                        username = control.Text.Trim();
                        DebugInfo($"从遍历找到的who控件获取用户名: '{username}'");
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(userPassword) && userpanel != null)
            {
                var inputControls = GetAllAntdInputs(userpanel);
                foreach (var control in inputControls)
                {
                    if (control.Name == "password" || control.Name.Contains("password"))
                    {
                        userPassword = control.Text.Trim();
                        DebugInfo($"从遍历找到的password控件获取密码，长度: {userPassword.Length}");
                        break;
                    }
                }
            }

            // 最终验证
            DebugInfo($"最终获取 - 用户名: '{username}', 密码长度: {userPassword.Length}");

            // 验证输入
            if (string.IsNullOrWhiteSpace(username))
            {
                DebugInfo("验证失败：用户名为空");
                MessageBox.Show("请输入用户名！", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(userPassword))
            {
                DebugInfo("验证失败：密码为空");
                MessageBox.Show("请输入密码！", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 显示加载状态
                Cursor = Cursors.WaitCursor;
                DebugInfo("开始网络请求...");

                // 准备登录数据
                var loginData = new Dictionary<string, string>
                {
                    { "用户名", username },
                    { "密码", userPassword }
                };

                // 使用 AuthHelper 发送带鉴权的请求
                var response = await AuthHelper.SendAuthPostRequest("/user/verify_credentials", loginData);

                DebugInfo($"收到响应，状态码: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();
                    DebugInfo($"响应内容长度: {responseText.Length}");

                    using (JsonDocument doc = JsonDocument.Parse(responseText))
                    {
                        JsonElement root = doc.RootElement;

                        if (root.TryGetProperty("success", out JsonElement successElement) &&
                            successElement.GetBoolean() &&
                            root.TryGetProperty("authenticated", out JsonElement authElement) &&
                            authElement.GetBoolean())
                        {
                            // 登录成功
                            if (root.TryGetProperty("user", out JsonElement userElement))
                            {
                                string userId = userElement.TryGetProperty("UserID", out JsonElement idElement)
                                    ? idElement.GetString() ?? ""
                                    : "";
                                string userName = userElement.TryGetProperty("用户名", out JsonElement nameElement)
                                    ? nameElement.GetString() ?? username
                                    : username;
                                string uploaderName = userElement.TryGetProperty("上传者代称", out JsonElement uploaderElement)
                                    ? uploaderElement.GetString() ?? ""
                                    : "";
                                string suValue = userElement.TryGetProperty("Su", out JsonElement suElement)
                                    ? suElement.GetString() ?? "None"
                                    : "None";

                                DebugInfo($"登录成功！用户名: {userName}, UserID: {userId}, 身份: {suValue}");

                                // 保存用户信息
                                currentUserName = userName;
                                currentUserUploaderName = uploaderName;
                                currentUserId = userId;
                                currentUserSu = suValue;

                                // 只更新用户相关配置
                                UpdateUserConfigOnly(username, userPassword, userId, uploaderName, suValue);

                                // 显示用户信息，隐藏登录控件
                                ShowUserInfo();
                                ShowUserInfoControls();

                                string userTypeText = suValue == "Super" ? "管理员" : "用户";
                                MessageBox.Show($"登录成功！身份: {userTypeText}", "成功",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                                DebugInfo("登录流程完成");
                            }
                        }
                        else
                        {
                            string error = root.TryGetProperty("error", out JsonElement errorElement)
                                ? errorElement.GetString()
                                : "登录失败";

                            DebugInfo($"登录失败: {error}");
                            MessageBox.Show($"登录失败: {error}", "错误",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    DebugInfo($"HTTP错误: {response.StatusCode}, 内容: {errorText}");
                    MessageBox.Show($"登录失败: {response.StatusCode}\n{errorText}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (HttpRequestException httpEx)
            {
                DebugInfo($"网络连接错误: {httpEx.Message}");
                MessageBox.Show($"网络连接错误: {httpEx.Message}\n请检查服务器连接", "连接错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                DebugInfo($"登录时发生异常: {ex.Message}");
                MessageBox.Show($"登录失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                DebugInfo("登录流程结束");
            }
        }

        // 获取所有AntdUI.Input控件
        private List<AntdUI.Input> GetAllAntdInputs(Control container)
        {
            List<AntdUI.Input> inputs = new List<AntdUI.Input>();

            if (container == null) return inputs;

            foreach (Control control in container.Controls)
            {
                if (control is AntdUI.Input input)
                {
                    inputs.Add(input);
                }

                // 递归查找子控件
                if (control.HasChildren)
                {
                    inputs.AddRange(GetAllAntdInputs(control));
                }
            }

            return inputs;
        }

        // 显示用户信息
        private void ShowUserInfo()
        {
            DebugInfo("显示用户信息");

            // 更新界面显示
            username.Text = $"你好，{currentUserName}";
            DebugInfo($"设置username.Text: {username.Text}");

            // 显示上传者代称
            if (!string.IsNullOrEmpty(currentUserUploaderName))
            {
                upwho.Text = $"代称为: {currentUserUploaderName}";
            }
            else
            {
                upwho.Text = "代称: 未设置";
            }
            DebugInfo($"设置upwho.Text: {upwho.Text}");

            // 显示用户类型
            string userTypeText = currentUserSu == "Super" ? "管理员" : "用户";
            ifsu.Text = $"身份: {userTypeText}";
            DebugInfo($"设置ifsu.Text: {ifsu.Text}");

            if (!string.IsNullOrEmpty(currentUserId))
            {
                userid.Text = $"UserID: {currentUserId}";
                DebugInfo($"设置userid.Text: {userid.Text}");

                // 计算已注册天数
                if (currentUserId.Length >= 6)
                {
                    try
                    {
                        string datePart = currentUserId.Substring(0, Math.Min(currentUserId.Length, 6));

                        if (datePart.Length == 6)
                        {
                            int year = int.Parse(datePart.Substring(0, 2));
                            int month = int.Parse(datePart.Substring(2, 2));
                            int dayValue = int.Parse(datePart.Substring(4, 2));

                            if (year < 100)
                            {
                                year += 2000;
                            }

                            DateTime registerDate = new DateTime(year, month, dayValue);
                            int days = (DateTime.Now - registerDate).Days;
                            if (days < 0) days = 0;

                            day.Text = $"超级小铺已陪伴您 {days} 天";
                        }
                        else
                        {
                            day.Text = "超级小铺感谢您的陪伴";
                        }
                    }
                    catch
                    {
                        day.Text = "超级小铺感谢您的陪伴";
                    }
                }
                else
                {
                    day.Text = "超级小铺感谢您的陪伴";
                }
            }
            else
            {
                userid.Text = "UserID: 未知";
                day.Text = "超级小铺欢迎您";
            }
            DebugInfo($"设置day.Text: {day.Text}");
        }

        // 退出登录
        private void exit_Click(object sender, EventArgs e)
        {
            DebugInfo("开始退出登录");

            // 清空用户信息
            currentUserName = "";
            currentUserUploaderName = "";
            currentUserId = "";
            currentUserSu = "None";

            DebugInfo("用户信息已清空");

            // 清除配置中的用户信息
            ClearUserConfigOnly();
            DebugInfo("配置信息已清除");

            // 清空输入框
            if (txtWho != null)
            {
                txtWho.Text = "";
                DebugInfo("who文本框已清空");
            }

            if (txtPassword != null)
            {
                txtPassword.Text = "";
                DebugInfo("password文本框已清空");
            }

            // 清空Designer中的控件
            if (who != null) who.Text = "";
            if (password != null) password.Text = "";

            // 显示登录控件，隐藏用户信息
            ShowLoginControls();

            MessageBox.Show("已退出登录", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            DebugInfo("退出登录完成");
        }

        /// <summary>
        /// 只清除用户相关的配置，不覆盖其他字段
        /// </summary>
        private void ClearUserConfigOnly()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                // 读取现有配置
                Dictionary<string, object> configDict;
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    configDict = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                        ?? new Dictionary<string, object>();
                }
                else
                {
                    configDict = new Dictionary<string, object>();
                }

                // 清空用户相关字段
                configDict["name"] = "";
                configDict["password"] = "";
                configDict["user_id"] = "";
                configDict["uploader_name"] = "";
                configDict["su"] = "None";

                // 保留所有其他字段不变

                // 写回文件
                string newJson = JsonSerializer.Serialize(configDict, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(configPath, newJson);

                DebugInfo("用户配置已清除");
            }
            catch (Exception ex)
            {
                DebugInfo($"清除用户配置失败: {ex.Message}");
            }
        }

        // 注册按钮点击事件
        private void reg_Click(object sender, EventArgs e)
        {
            MessageBox.Show("注册功能暂未开放，请联系管理员", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // 调试信息输出
        private void DebugInfo(string message)
        {
            // 输出到调试窗口
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] UserControl: {message}");

            // 也可以输出到控制台
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] UserControl: {message}");
        }

        // 获取当前用户信息（供其他控件调用）
        public string GetCurrentUserName() => currentUserName;
        public string GetCurrentUserUploaderName() => currentUserUploaderName;
        public string GetCurrentUserId() => currentUserId;
        public bool IsSuperUser() => currentUserSu == "Super";
        public bool IsLoggedIn() => !string.IsNullOrEmpty(currentUserName);

        // 新增方法：控制其他控件的显示/隐藏
        public void SetOtherControlsVisibility(bool isLoggedIn)
        {
            // 这里添加需要根据登录状态显示/隐藏的其他控件
            // 例如：
            // someControl.Visible = isLoggedIn;
            // anotherControl.Visible = isLoggedIn;
        }
    }
}