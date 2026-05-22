using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Timer
{
    using HandyControl.Data;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    public partial class RoundWindow : Window
    {

        /// <summary>
        /// 鼠标左键拖动窗口（替代默认标题栏拖动）
        /// </summary>
        private void WindowBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove(); // 启动窗口拖动
        }

        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 最小化按钮点击事件
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private const int WaitTime = 6;

        /// <summary>
        ///     计数
        /// </summary>
        private int _tickCount;

        /// <summary>
        ///     关闭计时器
        /// </summary>
        private DispatcherTimer? _timerClose;

        private ShowAnimation ShowAnimation { get; set; }

        private bool _shouldBeClosed;

        public RoundWindow(object content)
        {
            InitializeComponent();
            Content = content;
            this.Opacity = 0.001;
        }

        public void ShowWithAnimation(ShowAnimation showAnimation = ShowAnimation.VerticalMove, bool staysOpen = false)
        {
            ShowAnimation = showAnimation;
            RoundWindow notification = this;
            notification.Show();
            var desktopWorkingArea = SystemParameters.WorkArea;
            var leftMax = notification.Left;
            var topMax = notification.Top;
            notification.Opacity = 1;
            switch (showAnimation)
            {
                case ShowAnimation.None:
                    notification.Opacity = 1;
                    notification.Left = leftMax;
                    notification.Top = topMax;
                    break;
                case ShowAnimation.HorizontalMove:
                    notification.Opacity = 1;
                    notification.Left = desktopWorkingArea.Width;
                    notification.Top = topMax;
                    notification.BeginAnimation(LeftProperty, AnimationHelper.CreateAnimation(leftMax));
                    break;
                case ShowAnimation.VerticalMove:
                    notification.Opacity = 1;
                    notification.Left = leftMax;
                    notification.Top = desktopWorkingArea.Height;
                    notification.BeginAnimation(TopProperty, AnimationHelper.CreateAnimation(topMax));
                    break;
                case ShowAnimation.Fade:
                    notification.Left = leftMax;
                    notification.Top = topMax;
                    notification.BeginAnimation(OpacityProperty, AnimationHelper.CreateAnimation(1));
                    break;
                default:
                    notification.Opacity = 1;
                    notification.Left = leftMax;
                    notification.Top = topMax;
                    break;
            }
            if (!staysOpen) notification.StartTimer();


        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (_shouldBeClosed)
            {
                return;
            }
            var desktopWorkingArea = SystemParameters.WorkArea;

            switch (ShowAnimation)
            {
                case ShowAnimation.None:
                    break;
                case ShowAnimation.HorizontalMove:
                    {
                        var animation = AnimationHelper.CreateAnimation(desktopWorkingArea.Width);
                        animation.Completed += Animation_Completed;
                        BeginAnimation(LeftProperty, animation);
                        e.Cancel = true;
                        _shouldBeClosed = true;
                    }
                    break;
                case ShowAnimation.VerticalMove:
                    {
                        var animation = AnimationHelper.CreateAnimation(desktopWorkingArea.Height);
                        animation.Completed += Animation_Completed;
                        BeginAnimation(TopProperty, animation);
                        e.Cancel = true;
                        _shouldBeClosed = true;
                    }
                    break;
                case ShowAnimation.Fade:
                    {
                        var animation = AnimationHelper.CreateAnimation(0);
                        animation.Completed += Animation_Completed;
                        BeginAnimation(OpacityProperty, animation);
                        e.Cancel = true;
                        _shouldBeClosed = true;
                    }
                    break;
            }
        }

        private void Animation_Completed(object sender, EventArgs e) => Close();

        /// <summary>
        ///     开始计时器
        /// </summary>
        private void StartTimer()
        {
            _timerClose = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timerClose.Tick += delegate
            {
                if (IsMouseOver)
                {
                    _tickCount = 0;
                    return;
                }

                _tickCount++;
                if (_tickCount >= WaitTime) Close();
            };
            _timerClose.Start();
        }
    }
}
