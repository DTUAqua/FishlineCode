using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace FishLineMeasure.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Bootstrapper _bootStrapper;

        private Mutex _mutex;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);


        protected override void OnStartup(StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

            base.OnStartup(e);

            //Make sure UI formatting of values is consistent with the operating system settings.
            try
            {
                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            }
            catch { }

#if (DEBUG)
            RunInDebugMode();
#else
            RunInReleaseMode();
#endif
        }


        private void RunInDebugMode()
        {
            _bootStrapper = new Bootstrapper();
            _bootStrapper.Run(true);
        }


        private void RunInReleaseMode()
        {
            bool blnCreatedNew = true;
            _mutex = new Mutex(true, "FishLineMeasureApp", out blnCreatedNew);

            if (blnCreatedNew)
            {
                AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
                try
                {
                    _bootStrapper = new Bootstrapper();
                    _bootStrapper.Run(true);
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
            else
            {
                MessageBox.Show("FishLineMeasure kører allerede (du kan kun have én instans af programmet kørende ad gangen).");

                try
                {
                    Process curProcess = Process.GetCurrentProcess();
                    foreach (Process process in Process.GetProcessesByName(curProcess.ProcessName))
                    {
                        if (process.Id != curProcess.Id)
                        {
                            SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }
                catch { }

                Application.Current.Shutdown();
            }
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }


        private static void HandleException(Exception ex)
        {
            if (ex == null)
                return;

            MessageBox.Show("An unexpected exception occured. " + ex.Message);
            Anchor.Core.Loggers.Logger.LogError(ex);
            Environment.Exit(1);
        }



        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (_mutex != null)
                _mutex.Dispose();
        }

    }

}
