using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using ThemeCommons.Extension.Native;

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
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var me = Process.GetCurrentProcess();
            var procs = Process.GetProcessesByName(
                Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()?.Location)).Where(s => s.Id != me.Id).ToArray();

            if (procs.Any() && e.Args.Any())
            {
                var receiver = procs.First();
                var hwnd = receiver.MainWindowHandle;
                NativeMethods.SendMessage(hwnd, string.Join(",", e.Args));
                me.Kill();
            }

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
