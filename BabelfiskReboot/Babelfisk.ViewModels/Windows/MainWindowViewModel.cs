using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Babelfisk.BusinessLogic;
using Babelfisk.Entities.SprattusSecurity;
using Anchor.Core;
using Babelfisk.ViewModels.Security;
using System.Globalization;
using System.Reflection;

namespace Babelfisk.ViewModels.Windows
{
    public class MainWindowViewModel : AViewModel
    {
        private Menu.MainMenuViewModel _vmMainMenu;

        private static TreeView.MainTreeViewModel _vmMainTree;

        private DelegateCommand _cmdExitApplication;


        public static TreeView.MainTreeViewModel TreeView
        {
            get { return _vmMainTree; }
        }

        public MainWindowViewModel()
        {
        }

        public override void Refresh(RefreshOptions r = null)
        {
            base.Refresh(r);

            RefreshAllNotifiableProperties();
        }


        public void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(LoadLanguages)
            .ContinueWith(t =>
            {
                new Action(() =>
                {
                    IsLoading = false;
                    LogIn();
                }).Dispatch();
            });
        }


        private void Initialize()
        {
            LogIn();
            //InitializeMainViewModel();
        }


        private void LoadLanguages()
        {
            try
            {
                string asmName = "Babelfisk.WPF"; //Assembly.GetEntryAssembly().GetName().Name;
                BusinessLogic.Settings.Settings.Instance.CopyResourceToAppDataIfDifferent(asmName + ".Xml.Languages.da-DK.xml", "Xml\\Languages\\da-DK.xml");
                BusinessLogic.Settings.Settings.Instance.CopyResourceToAppDataIfDifferent(asmName + ".Xml.Languages.en-US.xml", "Xml\\Languages\\en-US.xml");
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e, "Failed to copy language files locally.");
            }

            Anchor.Core.Language.Translater t;
            CultureInfo curCulture = null;
            try
            {
                var lan = BusinessLogic.Settings.Settings.Instance.Language; // "da-DK";
                curCulture = new CultureInfo(lan);
                t = Anchor.Core.Language.TranslaterFactory.Instance.ApplyTranslater(curCulture);
            }
            catch (Exception e)
            {
                LogError(e);
                curCulture = new CultureInfo("da-DK");
                t = Anchor.Core.Language.TranslaterFactory.Instance.ApplyTranslater(curCulture);
            }
            //System.Threading.Thread.Sleep(10000);
            if (curCulture != null)
            {
                new Action(() =>
                {
                    try
                    {
                        curCulture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
                        curCulture.DateTimeFormat.DateSeparator = "-";

                        System.Threading.Thread.CurrentThread.CurrentCulture = curCulture;
                        System.Threading.Thread.CurrentThread.CurrentUICulture = curCulture;
                    }
                    catch (Exception e)
                    {
                        Anchor.Core.Loggers.Logger.LogError(e);
                    }
                }).Dispatch();
            }
        }


        public void InitializeMainViewModel()
        {
            //LoadDefaultUserAsync();

            _vmMainMenu = new Menu.MainMenuViewModel(this);
            AppRegionManager.LoadViewFromViewModel(RegionName.MenuRegion, _vmMainMenu);

            _vmMainTree = new TreeView.MainTreeViewModel();
            AppRegionManager.LoadViewFromViewModel(RegionName.LeftRegion, _vmMainTree);

            var vmStart = new StartViewModel();
            AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vmStart);
        }


      /*  private void LoadDefaultUserAsync()
        {
            Task.Factory.StartNew(() =>
            {
                var sec = new SecurityManager();
                Users usr = sec.LogIn("no", "no");

                new Action(() =>
                {
                    AppRegionManager.User = usr;
                }).Dispatch();

                //Base user settings on user id, since it newer changes.
                BusinessLogic.Settings.Settings.Instance.LoadUserSettings(usr.UserId.ToString());
            });
        }*/


        public void LogIn()
        {
            LogOnViewModel logOn = new LogOnViewModel();
            logOn.InitializeAsync();
            logOn.OnUserLogedOn += new Action<LogOnViewModel>(logOn_OnUserLogedOn);
            AppRegionManager.ShowLoginScreen(logOn);
        }

        protected void logOn_OnUserLogedOn(LogOnViewModel obj)
        {
            try
            {
                AppRegionManager.User = obj.User;
                BusinessLogic.Settings.Settings.Instance.LoadUserSettings(obj.UserName);

                if (obj.SelectedLanguage != null)
                {
                    BusinessLogic.Settings.Settings.Instance.Language = obj.SelectedLanguage;
                    ApplyLanguage(obj.SelectedLanguage);
                }

                InitializeMainViewModel();

                //Sync users for offline login
                var t = Offline.OfflineStaticMethods.SyncUsersAsync(AppRegionManager);

                t.ContinueWith(tt => AViewModel.InitializeDFUPersonFromLoggedInUserAsync());
            }
            catch(Exception e)
            {
                LogError(e);
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
