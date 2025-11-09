using System.Windows;
using System.Windows.Controls;
using Timer;


namespace Timer
{
    public partial class NotifyWindow : UserControl
    {
        public NotifyViewModel notifyViewModel;
        public NotifyWindow(TimerModel model)
        {
            InitializeComponent();

            // 使用相同的数据模型创建通知视图模型
            notifyViewModel = new NotifyViewModel(model);
            //notifyViewModel.CloseRequested += CloseWindow;
            this.DataContext = notifyViewModel;
        }

        private void CloseWindow()
        {
            //Close();
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