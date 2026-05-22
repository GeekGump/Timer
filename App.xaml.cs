using System.Configuration;
using System.Data;
using System.Windows;

namespace Timer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    using System;
    using System.Threading;
    using System.Windows;
    using System.Runtime.InteropServices;

    public partial class App : Application
    {
        // 全局唯一标识符 (GUID) - 使用你的应用唯一标识
        private const string AppUniqueId = "MyApp_Unique_ID_12345678-90AB-CDEF-1234-567890ABCDEF";

        // 互斥量对象
        private Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            // 尝试创建互斥量
            bool isNewInstance;
            _mutex = new Mutex(true, AppUniqueId, out isNewInstance);

            if (!isNewInstance)
            {
                // 已有实例在运行
                HandleExistingInstance();
                Shutdown(); // 关闭当前实例
                return;
            }

            // 注册退出事件释放互斥量
            Exit += (sender, args) => CleanupMutex();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            CleanupMutex();
            base.OnExit(e);
        }

        private void CleanupMutex()
        {
            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex.Close();
                _mutex = null;
            }
        }

        private void HandleExistingInstance()
        {
            // 方法1: 激活已有窗口 (推荐)
            //ActivateExistingWindow();

            // 方法2: 显示提示消息
            ShowDuplicateInstanceMessage();
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        private void ActivateExistingWindow()
        {
            try
            {
                // 获取当前进程
                var currentProcess = System.Diagnostics.Process.GetCurrentProcess();

                // 查找同名进程
                var processes = System.Diagnostics.Process.GetProcessesByName(currentProcess.ProcessName);

                foreach (var process in processes)
                {
                    // 跳过当前进程
                    if (process.Id == currentProcess.Id) continue;

                    // 获取主窗口句柄
                    IntPtr mainWindowHandle = process.MainWindowHandle;

                    if (mainWindowHandle != IntPtr.Zero)
                    {
                        // 还原窗口（如果最小化）
                        ShowWindow(mainWindowHandle, SW_RESTORE);

                        // 激活窗口
                        SetForegroundWindow(mainWindowHandle);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                MessageBox.Show($"无法激活已有实例: {ex.Message}");
            }
        }

        private void ShowDuplicateInstanceMessage()
        {
            MessageBox.Show("应用程序已在运行中", "重复启动",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

}
