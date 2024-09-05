using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Babelfisk.ViewModels.Windows;
using System.Windows;
using Anchor.Core;
using System.Threading;
using Babelfisk.ViewModels.Lookup;
using Microsoft.Win32;
using Ionic.Zip;
using System.IO;

namespace Babelfisk.ViewModels.Menu
{
    public class MainMenuViewModel : AViewModel
    {
        private DelegateCommand _cmdUsers;
        private DelegateCommand _cmdLookups;

        private DelegateCommand _cmdShowStart;
        private DelegateCommand _cmdLogOut;
        private DelegateCommand _cmdShowUserDetails;

        private DelegateCommand _cmdExportData;

        private DelegateCommand _cmdBackupData;

        private DelegateCommand _cmdOfflineOnline;

        private DelegateCommand _cmdReports;

        private DelegateCommand _cmdShowSmartDotsAdministration;

        MainWindowViewModel _vmMainWindow;

        public string GoOnline
        {
            get { return Translate("MainMenu", "GoOnline"); }
        }

        public string GoOffline
        {
            get { return Translate("MainMenu", "GoOffline"); }
        }

        public MainMenuViewModel(MainWindowViewModel vmMainWindow)
        {
            _vmMainWindow = vmMainWindow;
        }


        public override void Refresh(RefreshOptions r = null)
        {
            base.Refresh(r);

            RefreshAllNotifiableProperties();
        }


        private static bool CheckForUnsavedData()
        {
            bool blnUnsavedData = _appRegionManager.HasRegionUnsavedData(RegionName.MainRegion);

            if (blnUnsavedData)
            {
                var res = _appRegionManager.ShowMessageBox(Translater.Translate("Warning", "3"), System.Windows.MessageBoxButton.YesNo);

                if (res == System.Windows.MessageBoxResult.No)
                    return true;
            }

            return false;
        }


        #region Show Settings Command


        public DelegateCommand UsersCommand
        {
            get
            {
                if (_cmdUsers == null)
                    _cmdUsers = new DelegateCommand(() => Users());

                return _cmdUsers;
            }
        }


        private void Users()
        {
            if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
            {
                AppRegionManager.ShowMessageBox(Translate("MainMenu", "WarningAddEditUsersOffline"));
                return;
            }

            Security.UsersViewModel users = new Security.UsersViewModel();
            AppRegionManager.LoadWindowViewFromViewModel(users, true, "WindowWithBorderStyle");
        }

        #endregion


        #region Show Start Command


        public DelegateCommand ShowStartCommand
        {
            get
            {
                if (_cmdShowStart == null)
                    _cmdShowStart = new DelegateCommand(() => ShowStart());

                return _cmdShowStart;
            }
        }


        public static bool ShowStart()
        {
            try
            {
                GC.Collect();
            }
            catch { }
            var vmStart = new StartViewModel();
            return _appRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vmStart);
        }


        #endregion


        #region Log out command

        public DelegateCommand LogOutCommand
        {
            get
            {
                if (_cmdLogOut == null)
                    _cmdLogOut = new DelegateCommand(() => LogOut());

                return _cmdLogOut;
            }
        }


        private void LogOut()
        {
            if (CheckForUnsavedData())
                return;

            BusinessLogic.Settings.Settings.Instance.Save();

            AppRegionManager.ClearRegions();

            Security.BackupManager.Instance.Stop();

            _vmMainWindow.LogIn();
        }

        #endregion


        #region Show User Details command

        public DelegateCommand ShowUserDetailsCommand
        {
            get
            {
                if (_cmdShowUserDetails == null)
                    _cmdShowUserDetails = new DelegateCommand(() => ShowUserDetails());

                return _cmdShowUserDetails;
            }
        }


        private void ShowUserDetails()
        {
            if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
            {
                AppRegionManager.ShowMessageBox(Translate("MainMenu", "WarningEditUsersOffline"));
                return;
            }

            AppRegionManager.ShowLoadingWindow(Translate("MainMenu", "LoadingUserDetails"));
            Task.Factory.StartNew(() =>
            {
                while (User == null)
                    Thread.Sleep(500);

                new Action(() =>
                {
                    AppRegionManager.HideLoadingWindow();
                    Security.UserViewModel user = new Security.UserViewModel(User.UserId);
                    user.OnCancelClicked += user_OnCancelClicked;
                    user.OnOKClicked += user_OnOKClicked;

                    AppRegionManager.LoadWindowViewFromViewModel(user, true, "WindowWithBorderStyle");
                }).Dispatch();
            });
        }

        protected void user_OnOKClicked(Security.UserViewModel obj)
        {
            obj.OnOKClicked -= user_OnOKClicked;
            obj.Close();
        }

        protected void user_OnCancelClicked(Security.UserViewModel obj)
        {
            obj.OnCancelClicked -= user_OnCancelClicked;
            obj.Close();
        }

        #endregion


        #region Show Smart Dots Adminisrtation Command

        public DelegateCommand ShowSmartDotsAdministrationCommand
        {
            get
            {
                if (_cmdShowSmartDotsAdministration == null)
                    _cmdShowSmartDotsAdministration = new DelegateCommand(() => ShowAquaDotsAdministration());

                return _cmdShowSmartDotsAdministration;
            }
        }


        private void ShowAquaDotsAdministration()
        {

            if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
            {
                AppRegionManager.ShowMessageBox(Translate("MainMenu", "WarningManageAquaDotsOffline"));
                return;
            }

            if (!User.HasViewSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            //No DFUPerson after login was found. Search again, in case an administrator has added one.
            var t = AViewModel.InitializeDFUPersonFromLoggedInUserAsync();
            t.Wait();

            if (DFUPersonLogin == null)
            {
                AppRegionManager.ShowMessageBox(Translate("MainMenu", "WarningNoDFUPerson"));
                return;
            }

            var vmEvents = new SmartDots.SDEventsViewModel();
            vmEvents.InitializeAsync();
            AppRegionManager.LoadWindowViewFromViewModel(vmEvents, false, "StandardWindowWithBorderStyle");
        }

      

        #endregion


        #region Lookups Command


        public DelegateCommand LookupsCommand
        {
            get
            {
                if (_cmdLookups == null)
                    _cmdLookups = new DelegateCommand(() => Lookups());

                return _cmdLookups;
            }
        }


        private void Lookups()
        {
            LookupManagerViewModel lvm = new LookupManagerViewModel();
            AppRegionManager.LoadWindowViewFromViewModel(lvm, true, "WindowWithBorderStyle");
            //  var vmStart = new StartViewModel();
            // AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vmStart);
        }


        #endregion


        #region Offline/Online Command


        public DelegateCommand OfflineOnlineCommand
        {
            get { return _cmdOfflineOnline ?? (_cmdOfflineOnline = new DelegateCommand(() => ToggleOfflineOnline())); }
        }


        private void ToggleOfflineOnline()
        {
            if (!User.HasGoOfflineTask)
            {
                AppRegionManager.ShowMessageBox(Translate("MainMenu", "WarningNoRightsToGoOffline"));
                return;
            }

            var vmStart = new StartViewModel();

            if (AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vmStart))
            {
                if (!BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                {
                    var vmGoOffline = new ViewModels.Offline.GoOfflineViewModel();
                    AppRegionManager.LoadWindowViewFromViewModel(vmGoOffline, true, "WindowWithBorderStyle");

                }
                else
                {
                    var vmGoOnline = new Offline.GoOnlineViewModel();
                    AppRegionManager.LoadWindowViewFromViewModel(vmGoOnline, true, "WindowWithBorderStyle");
                }
            }
        }


        #endregion


        #region Export Command


        public DelegateCommand ExportCommand
        {
            get { return _cmdExportData ?? (_cmdExportData = new DelegateCommand(() => ExportData())); }
        }


        public static void ExportData(List<int> lstPreselectIds = null)
        {
            if (_appRegionManager != null && _appRegionManager.User != null &&
               !_appRegionManager.User.HasExportDataTask)
            {
                _appRegionManager.ShowMessageBox(Translate("MainMenu", "WarningNoRightsToExportData"));
                return;
            }

            var vmExport = new Babelfisk.ViewModels.Export.ExportDataViewModel(lstPreselectIds);

            _appRegionManager.LoadWindowViewFromViewModel(vmExport, false, "WindowWithBorderStyle");
        }


        #endregion


        #region Backup Command


        public DelegateCommand BackupCommand
        {
            get { return _cmdBackupData ?? (_cmdBackupData = new DelegateCommand(() => OpenBackupSettings())); }
        }


        public void OpenBackupSettings()
        {
            if (ShowStart())
            {
                var vm = new Babelfisk.ViewModels.Security.BackupViewModel();

                AppRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderStyle");
            }
        }


        #endregion


        #region Reports Command


        public DelegateCommand ReportsCommand
        {
            get { return _cmdReports ?? (_cmdReports = new DelegateCommand(() => ShowReports())); }
        }


        public void ShowReports(List<int> lstPreselectIds = null)
        {
            if (!User.HasViewReportsTask)
            {
                AppRegionManager.ShowMessageBox(Translate("MainMenu", "WarningNoRightsToDownloadReport"));
                return;
            }

            var vm = new Babelfisk.ViewModels.Reporting.ReportsViewModel();
            vm.Initialize();

            _appRegionManager.LoadWindowViewFromViewModel(vm, false, /*"WindowReportWidthStyle"*/ "WindowWithBorderStyle");
        }


        #endregion
    }
}
