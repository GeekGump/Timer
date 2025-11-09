using System;
using System.Diagnostics;
using System.Media;
using System.Windows.Resources;
using System.Windows.Threading;
using Timer;
using SysTimer = System.Timers.Timer;
using System.Windows;
namespace Timer
{
    public class TimerService
    {
        //private readonly SysTimer _timer;
        private readonly DispatcherTimer _timer;
        private DateTime _sessionStartTime;
        private TimeSpan _sessionDuration;

        public TimerModel Model { get; }
        public TimeSpan TodayTotalUsage { get; set; }
        public DateTime CurrentSessionStart { get; private set; }

        public event Action<string> NotificationRequested;

        public TimerService(TimerModel model)
        {
            Model = model;
            //_timer = new SysTimer(1000); // 1秒间隔
            //_timer.Elapsed += OnTimerElapsed;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick +=OnTimerElapsed ;

        }

        public void StartWork()
        {
            if (Model.State == TimerState.Stopped || Model.State == TimerState.Breaking)
            {
                Model.State = TimerState.Working;
                _sessionDuration = TimeSpan.FromMinutes(Model.Settings.WorkTime);
                StartSession();
            }
        }

        public void StartBreak()
        {
            if (Model.State == TimerState.Working || Model.State == TimerState.Notifying)
            {
                Model.NotificationMessage = "休息中";
                Model.State = TimerState.Breaking;
                _sessionDuration = TimeSpan.FromMinutes(Model.Settings.BreakTime);
                StartSession();
            }
        }

        public void Stop()
        {
            Model.State = TimerState.Stopped;
            _timer.Stop();

            if (CurrentSessionStart != DateTime.MinValue)
            {
                TodayTotalUsage += DateTime.Now - CurrentSessionStart;
                CurrentSessionStart = DateTime.MinValue;
                Model.TodayUsageDisplay = TodayTotalUsage.ToString(@"hh\:mm\:ss");
            }
        }

        private void StartSession()
        {
            _sessionStartTime = DateTime.Now;
            CurrentSessionStart = _sessionStartTime;
            _timer.Start();
            UpdateDisplay();
        }

        private void OnTimerElapsed(object? sender, EventArgs e)
        {
            var elapsed = DateTime.Now - _sessionStartTime;
            var remainingTime = _sessionDuration - elapsed;

            // 更新显示
            UpdateDisplay(remainingTime);

            // 检查是否需要通知
            if (Model.State == TimerState.Working &&
                remainingTime <= TimeSpan.FromMinutes(Model.Settings.NotificationThreshold) &&
                Model.State != TimerState.Notifying)
            {
                Model.State = TimerState.Notifying;
                var message = $"工作阶段剩余";
                Model.NotificationMessage = message;
                Model.ShowNotification = true;
                NotificationRequested?.Invoke(message);
            }

            // 检查是否结束
            if (elapsed >= _sessionDuration)
            {
                _timer.Stop();

                // 记录使用时间
                if (Model.State == TimerState.Working || Model.State == TimerState.Notifying)
                {
                    TodayTotalUsage += _sessionDuration;
                    Model.TodayUsageDisplay = TodayTotalUsage.ToString(@"hh\:mm\:ss");
                }

                // 自动切换到下一阶段
                if (Model.State == TimerState.Working || Model.State == TimerState.Notifying)
                {
                    PlayAlarmSound("Alarm04.wav");
                    StartBreak();
                    // Play break notification sound
                }
                else
                {
                    // Play end of break sound
                    PlayAlarmSound("Ring01.wav");
                }
            }
        }

        private void UpdateDisplay(TimeSpan? remainingTime = null)
        {
            var time = remainingTime ?? _sessionDuration;
            Model.RemainingTimeDisplay = $"{time:mm\\:ss}";

            // 更新进度
            if (Model.State == TimerState.Working || Model.State == TimerState.Notifying)
            {
                Model.Progress = 1.0 - time.TotalMinutes / Model.Settings.WorkTime;
            }
            else if (Model.State == TimerState.Breaking)
            {
                Model.Progress = 1.0 - time.TotalMinutes / Model.Settings.BreakTime;
            }
        }

        public void ResetTodayUsage()
        {
            TodayTotalUsage = TimeSpan.Zero;
            Model.TodayUsageDisplay = "00:00:00";
        }

        public void PlayAlarmSound(string fileName)
        {
            try
            {
                // 1. 获取资源流
                Uri uri = new Uri("pack://application:,,,/Resources/" + fileName);
                StreamResourceInfo streamInfo = Application.GetResourceStream(uri);

                if (streamInfo != null)
                {
                    // 2. 创建 SoundPlayer 并播放
                    using (SoundPlayer player = new SoundPlayer(streamInfo.Stream))
                    {
                        player.Play(); // 异步播放，不阻塞UI
                    }
                }
                else
                {
                    MessageBox.Show("音频资源未找到！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"播放音频失败：{ex.Message}");
            }
        }
    }
}