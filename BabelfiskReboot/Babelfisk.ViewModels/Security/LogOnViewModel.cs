using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Babelfisk.Entities.SprattusSecurity;
using Microsoft.Practices.Prism.Commands;
using System.Text.RegularExpressions;
using Anchor.Core;
using System.IO;
using System.Xml.Linq;

namespace Babelfisk.ViewModels.Security
{
    public class LogOnViewModel : AViewModel
    {
        public event Action<LogOnViewModel> OnUserLogedOn;

        private string _strUserName;
        private string _strPassword;

        private Users _user;

        private DelegateCommand _cmdLogOn;
        private DelegateCommand _cmdClose;

        private bool _blnIsLoadingLanguages = false;

        private List<KeyValuePair<string, string>> _lstLanguages;

        private string _strSelectedLanguage;

        private bool _blnLanguageChanged = false;


        #region Properties

        public string UserName
        {
            get { return _strUserName; }
            set
            {
                _strUserName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        public string Password
        {
            get { return _strPassword; }
            set
            {
                _strPassword = value;
                RaisePropertyChanged(() => Password);
            }
        }


        public Users User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => User);
            }
        }


        public List<KeyValuePair<string, string>> Languages
        {
            get { return _lstLanguages; }
            set
            {
                _lstLanguages = value;
                RaisePropertyChanged(() => Languages);
            }
        }


        public string SelectedLanguage
        {
            get { return _strSelectedLanguage; }
            set
            {
                _strSelectedLanguage = value;
                RaisePropertyChanged(() => SelectedLanguage);
                RaisePropertyChanged(() => HasLanguageChanged);
                RaisePropertyChanged(() => ShowLanguageRestartWarning);
                RaisePropertyChanged(() => SelectedLanguageToolTip);
            }
        }


        public string SelectedLanguageToolTip
        {
            get
            {
                string str = null;
                if (_lstLanguages != null)
                {
                    var kvs = _lstLanguages.Where(x => x.Key == SelectedLanguage);

                    if (kvs.Any())
                        str = kvs.First().Value;
                }

                return str;
            }
        }


        public bool HasLanguageChanged
        {
            get { return !BusinessLogic.Settings.Settings.Instance.Language.Equals(SelectedLanguage, StringComparison.InvariantCultureIgnoreCase); }
        }


        public bool ShowLanguageRestartWarning
        {
            get { return IsLanguagePropertySet && HasLanguageChanged; }
        }

        public bool IsLoadingLanguages
        {
            get { return _blnIsLoadingLanguages; }
            set
            {
                _blnIsLoadingLanguages = value;
                RaisePropertyChanged(() => IsLoadingLanguages);
            }
        }


      

        #endregion


        public LogOnViewModel()
        {
            UserName = BusinessLogic.Settings.Settings.Instance.UserName;
        }


        public Task InitializeAsync()
        {
            IsLoadingLanguages = false;
            return Task.Factory.StartNew(InitializeLanguages);
        }


        private void InitializeLanguages()
        {
            var lstLanguages = new List<KeyValuePair<string, string>>();

            try
            {
                string strPath = BusinessLogic.Settings.Settings.Instance.LanguageFolder;

                string[] arrLanguages = Directory.GetFiles(strPath, "*.xml");

                foreach (var lanPath in arrLanguages)
                {
                    //Make sure the file name is of 5 characters (to sort out other xml files which are not language files)
                    if (System.IO.Path.GetFileNameWithoutExtension(lanPath).Length != 5)
                        continue;

                    XElement xLan = XElement.Load(lanPath);

                    //If document element is not Language, the xml-file is not a language file - skip it.
                    if (xLan.Name != "Language")
                        continue;

                    string strType = xLan.Attribute("type") == null ? null : xLan.Attribute("type").Value;
                    string strName = xLan.Attribute("name") == null ? null : xLan.Attribute("name").Value;
                    string strNameEng = xLan.Attribute("nameEng") == null ? null : xLan.Attribute("nameEng").Value;

                    if (strType != null && strName != null && strNameEng != null)
                    {
                        lstLanguages.Add(new KeyValuePair<string, string>(strType, string.Format("{0}  -  {1} ({2})", strType.Substring(0, 2).ToUpper(), strName, strNameEng)));
                    }
                }

            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e, "An error occured while loading languages from disk.");
            }

            new Action(() =>
            {
                Languages = lstLanguages;

                SelectedLanguage = BusinessLogic.Settings.Settings.Instance.Language;
            }).Dispatch();
        }


        public void SetLanguage()
        {
            if (SelectedLanguage == null)
                return;

            ApplyLanguage(SelectedLanguage, false);
            RaisePropertyChanged(() => ShowLanguageRestartWarning);
            RaisePropertyChanged(() => HasLanguageChanged);
        }


        #region Log on command



        public DelegateCommand LogOnCommand
        {
            get
            {
                if (_cmdLogOn == null)
                    _cmdLogOn = new DelegateCommand(() => LogOnAsync());

                return _cmdLogOn;
            }
        }


        private void LogOnAsync()
        {
            User = null;
            IsLoading = true;

            Task.Factory.StartNew(LogOn).ContinueWith(LogOnDone);
        }


        private void LogOnDone(Task t)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                IsLoading = false;
            }));
        }


        private void LogOn()
        {
            Error = null;

            if (string.IsNullOrEmpty(UserName))
                Error = Translate("LogOnView", "ErrorEnterUsrname");
            else if (string.IsNullOrEmpty(Password))
                Error = Translate("LogOnView", "ErrorEnterPassword");

            if (Error != null)
                return;

            var secMan = new BusinessLogic.SecurityManager();

            Users user = null;
            string strFaultCode = null;
            try
            {
                user = secMan.LogIn(UserName, Password, ref strFaultCode);
            }
            catch (Exception e)
            {
                Error = Translate("Errors", "1") + ". " + e.Message;
                return;
            }

            if (strFaultCode != null)
            {
                if (strFaultCode.Equals("Deactivated", StringComparison.InvariantCultureIgnoreCase))
                {
                    Error = Translate("LogOnView", "ErrorDeactivatedUSer");
                    return;
                }
                else if (strFaultCode.Contains("InvalidUsernameOrPassword", StringComparison.InvariantCultureIgnoreCase))
                {
                    Regex reg = new Regex(@"^InvalidUserNameOrPassword;LoginAttemptsLeft:(\d+)", RegexOptions.IgnoreCase);
                    var m = reg.Match(strFaultCode);
                    if (m.Groups.Count > 1 && !string.IsNullOrWhiteSpace(m.Groups[1].Value))
                    {
                        string strAttemptsLeft = m.Groups[1].Value;
                        Error = string.Format("{0} {1}",Translate("LogOnView", "ErrorWrongPassword"), strAttemptsLeft);
                    }
                    else
                        Error = Translate("LogOnView", "ErrorUsernameOrPassword");

                    return;
                }
            }

            if (user == null)
            {
                if (!string.IsNullOrWhiteSpace(strFaultCode))
                    Error = Translate("Errors", "1") + ". " + strFaultCode;
                else
                    Error = Translate("LogOnView", "ErrorUsernameOrPassword");

                return;
            }

            if(user.Role != null && !user.HasTask(Entities.SecurityTask.ReadData))
            {
                Error = Translate("LogOnView", "ErrorNoReadDataTask");

                return;
            }

            string strPassword = Password;
            new Action(() =>
            {
                try
                {
                    User = user;

                    BusinessLogic.Settings.Settings.Instance.UserName = UserName;

                    AppRegionManager.HideLoginScreen();

                    if (OnUserLogedOn != null)
                        OnUserLogedOn(this);

                    new Action(() =>
                    {
                        PerformPasswordLengthCheck(strPassword);
                    }).Dispatch(System.Windows.Threading.DispatcherPriority.ContextIdle);
                }
                catch(Exception e)
                {
                    LogError(e);
                }
            }).Dispatch();
        }


        private void PerformPasswordLengthCheck(string strPassword)
        {
            if (!BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline && strPassword != null && strPassword.Length < 6)
            {
                AppRegionManager.ShowMessageBox(Translate("LogOnView", "WarningPaswordLength"), MessageBoxButton.OK);

                Security.UserViewModel user = new Security.UserViewModel(User.UserId, true);
                user.OnCancelClicked += user_OnCancelClicked;
                user.OnOKClicked += user_OnOKClicked;

                AppRegionManager.LoadWindowViewFromViewModel(user, true, "WindowWithBorderStyle");
            }
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


        #region Close command


        public DelegateCommand CloseCommand
        {
            get
            {
                if (_cmdClose == null)
                    _cmdClose = new DelegateCommand(() => CloseApplication());

                return _cmdClose;
            }
        }


        private void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        #endregion

    }
}
