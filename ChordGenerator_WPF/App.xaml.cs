using System.Windows;

namespace ChordGenerator_WPF
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected Mutex Mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            Mutex = new Mutex(true, ResourceAssembly.GetName().Name);
            if (!Mutex.WaitOne())
            {
                Current.Shutdown();
                return;
            }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Mutex != null)
            {
                Mutex.ReleaseMutex();
            }
            base.OnExit(e);
        }
    }
}
