using System.Configuration;
using System.Data;
using System.Windows;

namespace Timer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            // 保存应用程序状态
            base.OnExit(e);
        }
    }

}
