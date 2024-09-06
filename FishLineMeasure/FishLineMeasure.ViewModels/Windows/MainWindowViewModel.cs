using FishLineMeasure.ViewModels.Menu;
using FishLineMeasure.ViewModels.Overview;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Bluetooth;

namespace FishLineMeasure.ViewModels.Windows
{
    public class MainWindowViewModel : AViewModel
    {
       
        private DelegateCommand _cmdExitApplication;

        private OverviewViewModel _vmOverview;

        private TopMenuViewModel _vmMenu;


        public TopMenuViewModel Menu
        {
            get { return _vmMenu; }
        }


        public OverviewViewModel Overview
        {
            get { return _vmOverview; }
        }


        public MainWindowViewModel()
        {
            MinWindowHeight = 515;
            MinWindowHeight = 600;

            _vmMain = this;
        }

        public override void Refresh(RefreshOptions r = null)
        {
            base.Refresh(r);

            RefreshAllNotifiableProperties();
        }


        public void Initialize()
        {
            _vmMenu = new TopMenuViewModel();
            AppRegionManager.LoadViewFromViewModel(RegionName.MenuRegion, _vmMenu);

            ShowOverview();
        }


        public void ShowOverview()
        {
            if (_vmOverview == null)
            {
                _vmOverview = new OverviewViewModel();
                _vmOverview.InitializeAsync();
            }

            AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, _vmOverview);
        }


        public override void FireClosing(object sender, CancelEventArgs e)
        {
            base.FireClosing(sender, e);

            if (Menu.BCWLE.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                Menu.BCWLE.DisconnectFromDevice();
            }
        }


        #region Exit application command

        public DelegateCommand ExitApplicationCommand
        {
            get
            {
                if (_cmdExitApplication == null)
                    _cmdExitApplication = new DelegateCommand(() => ExitApplication());

                return _cmdExitApplication;
            }
        }


        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        #endregion

    }
}
