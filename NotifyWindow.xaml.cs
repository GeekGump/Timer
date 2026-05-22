using HandyControl.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Timer;
using Notification = HandyControl.Controls.Notification;

namespace Timer
{
    public partial class NotifyWindow : UserControl
    {
        public NotifyViewModel notifyViewModel;
        private RoundWindow? notification;
        public NotifyWindow(TimerModel model)
        {
            InitializeComponent();

            // 使用相同的数据模型创建通知视图模型
            notifyViewModel = new NotifyViewModel(model);

            notifyViewModel.CloseRequested += CloseWindow;
            this.DataContext = notifyViewModel;
            model.StateChanged += Model_StateChanged;
        }

        private void Model_StateChanged(object? sender, TimerState e)
        {
            ChangeWindowSize(e);
        }

        public void SetNotification(RoundWindow notification)
        {
            this.notification = notification;
            ChangeWindowSize(notifyViewModel._model.State);

        }

        private void CloseWindow()
        {
            notification?.Hide();
        }

        private void ChangeWindowSize(TimerState state)
        {
            if (notification == null)
                return;
            if (state == TimerState.Working)
            {
                this.Width = 300;
                this.Height = 300;
                notification.Top = SystemParameters.WorkArea.Height - notification.ActualHeight*0.9;
                notification.Left = SystemParameters.WorkArea.Width - notification.ActualHeight*1.15;
            }
            else if (state == TimerState.Breaking)
            {
                this.Width = 600;
                this.Height = 400;
                notification.Top = (SystemParameters.WorkArea.Height - notification.ActualHeight) / 2;
                notification.Left = (SystemParameters.WorkArea.Width - notification.ActualWidth) / 2;
            }
            else if (state == TimerState.Notifying)
            {
                this.Width = 300;
                this.Height = 300;
                notification.Top = SystemParameters.WorkArea.Height - notification.ActualHeight*0.9;
                notification.Left = SystemParameters.WorkArea.Width - notification.ActualHeight*1.15;
            }
            else
            {
                this.Width = 300;
                this.Height = 300;
                notification.Top = SystemParameters.WorkArea.Height - notification.ActualHeight * 0.9;
                notification.Left = SystemParameters.WorkArea.Width - notification.ActualHeight * 1.15;
                notification.Hide();
            }
        }
    }

    // 百分比转换器保持不变
    public class PercentageConverter : System.Windows.Data.IValueConverter
    {
        public static PercentageConverter Instance = new PercentageConverter();

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double percentage)
            {
                return 300 * percentage; // 简化实现
            }
            return 0;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}