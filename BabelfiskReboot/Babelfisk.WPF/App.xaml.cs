using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Babelfisk.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Bootstrapper _bootStrapper;

        private Mutex _mutex;

        #region pinvoke


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;


        #endregion


        protected override void OnStartup(StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

            base.OnStartup(e);

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
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;

            bool blnCreatedNew = true;

            try
            {
                _mutex = new Mutex(true, @"Global\FishLineApp", out blnCreatedNew);
            }
            catch(UnauthorizedAccessException ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
                return;
            }
           
            if (blnCreatedNew)
            {
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
                MessageBox.Show("Fishline is already running. You can only have once instance of Fishline running at the same time.");

                try
                {
                    Process curProcess = Process.GetCurrentProcess();
                    foreach(Process process in Process.GetProcessesByName(curProcess.ProcessName))
                    {
                        if(process.Id != curProcess.Id)
                        {
                            ShowWindow(process.MainWindowHandle, SW_RESTORE);
                            SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }
                catch { }
                finally
                {
                    Application.Current.Shutdown();
                }
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

            Anchor.Core.Loggers.Logger.LogError(ex, "App->HandleException().");

            var val = MessageBox.Show("An unexpected exception occurred. " + ex.Message, "Error", MessageBoxButton.OK);

            // Environment.Exit(1);

            // Allow OnExit to execute before terminating
            Application.Current?.Shutdown(1);
        }



        protected override void OnExit(ExitEventArgs e)
        {
            ReleaseMutex();
            base.OnExit(e);
        }

        private void ReleaseMutex()
        {
            if(_mutex != null)
            {
                try
                {
                    _mutex.Dispose();
                }
                catch { }
            }
        }

    }
}
