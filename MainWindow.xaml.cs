using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using Application = System.Windows.Application;
namespace Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        private readonly MainViewModel _viewModel;
        private NotifyWindow? _notifyWindow;
        private readonly TimerModel _timerModel;
        private Notification? _notification;
        public MainWindow()
        {
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

        }

        private void ShowNotificationWindow(string message)
        {
            _notification?.Close();
            _notifyWindow = new NotifyWindow(_timerModel);
            _notifyWindow.notifyViewModel.CloseRequested += () =>
            {
                _notification?.Close();
                _notifyWindow = null;
            };
            _notification = Notification.Show(
                _notifyWindow,
                staysOpen: true
                
            );
            
            //_notification.Left = SystemParameters.WorkArea.Width - _notification.Width - 20;
            //_notification.Top = SystemParameters.WorkArea.Height - _notification.Height - 20;
            _notification.Topmost = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; // 取消窗口关闭
            Hide();
            _viewModel.SaveCurrentUsage();
            _notification?.Close();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //_notification?.Close();
            //NotifyIcon.ShowBalloonTip("HandyControl", "Hello", NotifyIconInfoType.None,"");
            Debug.WriteLine("Button clicked!");

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
            if (_notifyWindow == null)
            {
                _notification?.Close();
                _notifyWindow = new NotifyWindow(_timerModel);
                _notifyWindow.notifyViewModel.CloseRequested += () =>
                {
                    _notification?.Close();
                    _notifyWindow = null;
                };
                _notification = Notification.Show(
                    _notifyWindow,
                    staysOpen: true
                );
                _notification.Topmost = true;
            }
        }
    }
}