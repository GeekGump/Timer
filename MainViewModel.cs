using System.ComponentModel;
using System.Windows.Input;
using Timer;
using System.IO;
namespace Timer
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TimerService _timerService;
        private readonly DataService _dataService;

        public MainViewModel(TimerModel model)
        {
            Model = model;
            _timerService = new TimerService(model);
            _dataService = new DataService(Path.Combine(Config.AppFolder,"usage_data.json"));

            // 订阅事件
            _timerService.NotificationRequested += OnNotificationRequested;

            // 初始化命令
            StartWorkCommand = new RelayCommand(_ => _timerService.StartWork(),
                _ => Model.State == TimerState.Stopped || Model.State == TimerState.Breaking);

            StartBreakCommand = new RelayCommand(_ => _timerService.StartBreak(),
                _ => Model.State == TimerState.Working || Model.State == TimerState.Notifying);

            StopCommand = new RelayCommand(_ => _timerService.Stop(),
                _ => Model.State != TimerState.Stopped);

            ResetTodayCommand = new RelayCommand(_ => ResetTodayUsage());

            // 加载今日数据
            LoadTodayUsage();

            this.Model.PropertyChanged += OnModelPropertyChanged;
        }

        public TimerModel Model { get; }

        public ICommand StartWorkCommand { get; }
        public ICommand StartBreakCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ResetTodayCommand { get; }

        public event Action<string> ShowNotification;

        private void OnNotificationRequested(string message)
        {
            ShowNotification?.Invoke(message);
        }

        private void LoadTodayUsage()
        {
            var todayRecord = _dataService.GetTodayUsage();
            if (todayRecord != null)
            {
                _timerService.TodayTotalUsage = todayRecord.TotalUsage;
                Model.TodayUsageDisplay = todayRecord.TotalUsage.ToString(@"hh\:mm\:ss");
            }
        }

        private void ResetTodayUsage()
        {
            _timerService.ResetTodayUsage();
            _dataService.SaveUsageRecord(_timerService.TodayTotalUsage, 1, 1);
        }

        public void SaveCurrentUsage()
        {
            _dataService.SaveUsageRecord(_timerService.TodayTotalUsage, 1, 1);
        }

        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TimerModel.State))
            {
                (StartWorkCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (StartBreakCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (StopCommand as RelayCommand)?.RaiseCanExecuteChanged();
                if(e.Equals(TimerState.Breaking))
                {
                    SaveCurrentUsage();
                }
            }
           
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public static class AppIcons
    {
        // 指向 Resources 文件夹下的 notify.ico（pack URI 格式）
        public static string NotifyIcon => "pack://application:,,,/Resources/time.ico";
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}