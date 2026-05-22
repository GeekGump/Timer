using HandyControl.Controls;
using System.Windows;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
namespace Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public static MainWindow? instance;
        private readonly MainViewModel _viewModel;
        private NotifyWindow? _notifyWindow;
        private readonly TimerModel _timerModel;
        private RoundWindow? _notification;
        public MainWindow()
        {
            instance = this;
            InitializeComponent();
            // 创建共享的数据模型
            _timerModel = new TimerModel();
            Notification.MaxWidthProperty.OverrideMetadata(typeof(Notification), new FrameworkPropertyMetadata(300.0));
            Notification.MaxHeightProperty.OverrideMetadata(typeof(Notification), new FrameworkPropertyMetadata(300.0));

            //Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#004CAF50"));
            //Notification.BackgroundProperty.OverrideMetadata(typeof(Notification), new FrameworkPropertyMetadata(brush));
            //Notification.MarginProperty.OverrideMetadata(typeof(Notification), new FrameworkPropertyMetadata(new Thickness(15)));
            // 创建视图模型
            _viewModel = new MainViewModel(_timerModel);
            _viewModel.ShowNotification += ShowNotificationWindow;
            DataContext = _viewModel;
            LoadAutoStartState();
            _viewModel.StartWorkCommand.Execute(null);
        }

        private void ShowNotificationWindow(string message)
        {
            _notifyWindow = new NotifyWindow(_timerModel);
            _notification = new RoundWindow(_notifyWindow);
            _notification.Show();
            _notifyWindow.SetNotification(_notification);
            _notification.ShowWithAnimation(staysOpen: true);
            _notification.Topmost = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; // 取消窗口关闭
            Hide();
            _viewModel.SaveCurrentUsage();
            _notification?.Hide();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {



        }


        private void OpenButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Show();             // 显示主窗口
            this.Activate();         // 激活窗口（避免被遮挡）
        }

        private void CloseButtonClicked(object sender, RoutedEventArgs e)
        {
            TrayIcon.Dispose(); // 释放托盘图标资源
            Application.Current.Shutdown(); // 关闭应用
        }

        private void TrayIcon_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.Show();             // 显示主窗口
            this.Activate();         // 激活窗口（避免被遮挡）
        }

        private void NotifyClick(object sender, RoutedEventArgs e)
        {
            ShowNotificationWindow("");
        }
        // 注册表键名（建议使用唯一的应用标识）
        private const string AppRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "Timer";

        // 加载当前自动启动状态
        private void LoadAutoStartState()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(AppRegistryKey))
                {
                    AutoStartToggle.IsChecked = key?.GetValue(AppName) != null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法读取自动启动设置: {ex.Message}");
                AutoStartToggle.IsChecked = false;
            }
        }
        // 切换按钮打开时（启用自动启动）
        private void AutoStartToggle_Checked(object sender, RoutedEventArgs e)
        {
            SetAutoStart(true);
        }

        // 切换按钮关闭时（禁用自动启动）
        private void AutoStartToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SetAutoStart(false);
        }

        // 设置自动启动状态
        private void SetAutoStart(bool enable)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(AppRegistryKey, true))
                {
                    if (enable)
                    {
                        // 获取当前可执行文件路径（带引号防止空格问题）
                        string appPath = $"\"{Process.GetCurrentProcess().MainModule.FileName}\"";
                        key.SetValue(AppName, appPath);
                    }
                    else
                    {
                        // 删除注册表项
                        if (key.GetValue(AppName) != null)
                            key.DeleteValue(AppName);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("需要管理员权限才能修改启动项", "权限错误", MessageBoxButton.OK, MessageBoxImage.Error);
                AutoStartToggle.IsChecked = !AutoStartToggle.IsChecked; // 恢复之前状态
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                AutoStartToggle.IsChecked = !AutoStartToggle.IsChecked; // 恢复之前状态
            }
        }
    }
}