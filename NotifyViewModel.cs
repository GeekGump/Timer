using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using Timer;
using System.Windows;
namespace Timer
{
    public class NotifyViewModel : INotifyPropertyChanged
    {
        public TimerModel _model { get; }
        private Brush _progressColor = Brushes.Green;

        public NotifyViewModel(TimerModel model)
        {
            _model = model;
            //_model = new TimerModel
            //{
            //    NotificationMessage = model.NotificationMessage,
            //    RemainingTimeDisplay = model.RemainingTimeDisplay,
            //    Progress = model.Progress
            //};
            //TimerService timerService = new TimerService(_model);
            //timerService.StartWork();
            _model.PropertyChanged += OnModelPropertyChanged;
            CloseCommand = new RelayCommand(_ => CloseRequested?.Invoke());
        }

        public string Message => _model.NotificationMessage;
        public string RemainingTime => _model.RemainingTimeDisplay;
        public double Progress => _model.Progress;

        public Brush ProgressColor
        {
            get => _progressColor;
            set
            {
                _progressColor = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseCommand { get; }

        public event Action CloseRequested;

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TimerModel.Progress))
            {
                UpdateProgressColor();
            }

            // 通知视图属性已更改
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(RemainingTime));
            OnPropertyChanged(nameof(Progress));
        }

        private void UpdateProgressColor()
        {
            if (_model.Progress > 0.7)
                ProgressColor = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // 绿色
            else if (_model.Progress > 0.4)
                ProgressColor = new SolidColorBrush(Color.FromRgb(255, 193, 7));  // 黄色
            else if (_model.Progress > 0.1)
                ProgressColor = new SolidColorBrush(Color.FromRgb(255, 152, 0));  // 橙色
            else
                ProgressColor = new SolidColorBrush(Color.FromRgb(244, 67, 54));  // 红色
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}