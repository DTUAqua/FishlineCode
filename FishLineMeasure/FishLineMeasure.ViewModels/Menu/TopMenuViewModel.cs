using FishLineMeasure.ViewModels.Settings;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Menu
{
    public class TopMenuViewModel : AViewModel
    {
        private DelegateCommand _cmdSettings;
        private DelegateCommand _cmdOpenBlueetothWindow;
        private DelegateCommand _cmdTurnOffScreen;
        private DelegateCommand _cmdShutdown;



        private BluetoothControlsViewModel _BCWLE;
          
        public BluetoothControlsViewModel BCWLE
        {
            get { return _BCWLE; }
            set
            {
                _BCWLE = value;
                RaisePropertyChanged(nameof(BCWLE));
                 
            }
        }

        public TopMenuViewModel()
        {
            BCWLE = new BluetoothControlsViewModel();
            var t =  BCWLE.Initialize();
            t.ContinueWith(tt =>
            {
                var sv = BCWLE.BluetoothList.Where(x => x.BluetoothAddressString == AppSettings.BluetoothAddress).FirstOrDefault();
                if (sv != null)
                {
                    BCWLE.SelectedBluetoothDevice = sv;
                }
            });
        }


        #region Settings Command


        public DelegateCommand SettingsCommand
        {
            get { return _cmdSettings ?? (_cmdSettings = new DelegateCommand(OpenSettings)); }
        }

        public void OpenSettings()
        {
           var vmSettings = new SettingsViewModel();
            AppRegionManager.LoadWindowViewFromViewModel(vmSettings, true, "WindowToolBox");
        }


        #endregion


        #region Open Bluetooth Window Command

        public DelegateCommand BluetoothWindowCommand
        {
            get { return _cmdOpenBlueetothWindow ?? (_cmdOpenBlueetothWindow = new DelegateCommand(OpenBluetoothTab)); }
        }

        private void OpenBluetoothTab()
        {
            if (BCWLE != null && BCWLE.BLE != null && !BCWLE.BLE.IsSearching)
                BCWLE.RefreshBlueToothDeviceCommand.Execute();

            AppRegionManager.LoadWindowViewFromViewModel(BCWLE, true, "WindowToolBox");
        }


        #endregion


        #region Turn Screen Off Command


        public DelegateCommand TurnScreenOffCommand
        {
            get { return _cmdTurnOffScreen ?? (_cmdTurnOffScreen = new DelegateCommand(TurnScreenOff)); }
        }


        private void TurnScreenOff()
        {
            try
            {
                if(BusinessLogic.Settings.Settings.Instance.ShowWarningOnScreenCloseCommand &&
                   AppRegionManager.ShowMessageBox("Ønsker du at slukke skærmen for at spare på batteriet? Du vil fortsat kunne foretage målinger med bluetooth-enheden.\n\n(Denne besked kan slås fra under indstillinger)", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                {
                    return;
                }

                Infrastructure.Auxiliary.ScreenInterop.TurnScreenOff();
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }


        #endregion


        #region ShutdownCommand

        public DelegateCommand ShutdownCommand
        {
            get { return _cmdShutdown ?? (_cmdShutdown = new DelegateCommand(Shutdown)); }
        }


        private void Shutdown()
        {
            if(AppRegionManager.ShowMessageBox("Er du sikker på du ønsker at lukke programmet?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            System.Windows.Application.Current.Shutdown();
        }

        #endregion

        }
}
