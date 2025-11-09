using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Timer;

namespace Timer
{
    public enum TimerState
    {
        Stopped,
        Working,
        Breaking,
        Notifying
    }
    /// <summary>
    /// 纯数据模型，不包含任何命令或业务逻辑
    /// </summary>
    public class TimerModel : INotifyPropertyChanged
    {
        private Brush _restColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4CAF50"));
        private Brush _workColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4CAF50"));
        private Brush _notifyColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF9800"));
        private TimerState _state = TimerState.Stopped;
        private string _remainingTimeDisplay = "40:00";
        private string _todayUsageDisplay = "00:00:00";
        private string _currentState = "准备开始";
        private double _progress;
        private string _notificationMessage = "工作中···";
        private bool _showNotification;

        public TimerSettings Settings { get; set; } = new TimerSettings();

        public TimerModel()
        {
             var time = TimeSpan.FromMinutes(Settings.WorkTime);
            _remainingTimeDisplay = $"{time:mm\\:ss}";
        }

        public TimerState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
                UpdateCurrentStateDisplay();
            }
        }

        public string RemainingTimeDisplay
        {
            get => _remainingTimeDisplay;
            set
            {
                _remainingTimeDisplay = value;
                OnPropertyChanged();
            }
        }

        public string TodayUsageDisplay
        {
            get => _todayUsageDisplay;
            set
            {
                _todayUsageDisplay = value;
                OnPropertyChanged();
            }
        }

        public string CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                OnPropertyChanged(nameof(TextColor));
                OnPropertyChanged();
            }
        }

        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        public string NotificationMessage
        {
            get => _notificationMessage;
            set
            {
                _notificationMessage = value;
                OnPropertyChanged();
            }
        }

        public bool ShowNotification
        {
            get => _showNotification;
            set
            {
                _showNotification = value;
                OnPropertyChanged();
            }
        }

        private void UpdateCurrentStateDisplay()
        {
            CurrentState = State switch
            {
                TimerState.Working => "工作中...",
                TimerState.Breaking => "休息中...",
                TimerState.Notifying => "即将休息!",
                _ => "准备开始"
            };
        }

        public Brush TextColor
        {
            get
            {
                return State switch
                {
                    TimerState.Working => _workColor,
                    TimerState.Breaking => _restColor,
                    TimerState.Notifying => _notifyColor,
                    _ => new SolidColorBrush(Colors.Black)
                };
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
