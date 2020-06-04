using ItsyBitsy.Domain;
using System.Net.Http;
using System.Windows;
using System.Windows.Threading;

namespace ItsyBitsy.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Factory.Register<IRepository, Repository>();
            Factory.Register<HttpClientHandler, HttpClientHandler>();
            Exit += App_Exit;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            Settings.Instance.Save();
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception

            // Prevent default unhandled exception processing
            e.Handled = true;
        }
    }
}
