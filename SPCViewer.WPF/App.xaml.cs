using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace SPCViewer.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Handles App startup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Any())
                Properties.Add("args", e.Args);
            base.OnStartup(e);
        }

        /// <summary>
        /// Handles Unhandled Exception
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unexpected exception has occurred. Shutting down the application. \nMessage: " + e.Exception.Message, "Unhandled Exception",
                MessageBoxButton.OK, MessageBoxImage.Error);
            // Prevent default unhandled exception processing
            e.Handled = true;
            Environment.Exit(0);
        }
    }
}
