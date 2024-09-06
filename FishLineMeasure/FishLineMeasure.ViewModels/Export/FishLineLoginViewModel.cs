using Babelfisk.Entities.SprattusSecurity;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Anchor.Core;
using System.ServiceModel;

namespace FishLineMeasure.ViewModels.Export
{
    public class FishLineLoginViewModel : AViewModel
    {
        private DelegateCommand _cmdLogin;
        private DelegateCommand _cmdClose;

        private string _strUserName;
        private string _strPassword;

        private Users _user;



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


        public Users LoggedInUser
        {
            get { return BusinessLogic.Settings.Settings.CurrentUser; }
            private set
            {
                BusinessLogic.Settings.Settings.CurrentUser = value;
                RaisePropertyChanged(() => LoggedInUser);
            }
        }


        #endregion




        public FishLineLoginViewModel()
        {
            WindowWidth = 700;
            WindowHeight = 480;
            WindowTitle = "Fiskeline - log ind";

            base.AdjustWindowWidthHeightToScreen();

            UserName = BusinessLogic.Settings.Settings.Instance.UserName;

            //Recent logged in user, in case it was not reset previously.
            BusinessLogic.Settings.Settings.CurrentUser = null;
        }


        #region Log on command



        public DelegateCommand LoginCommand
        {
            get
            {
                return _cmdLogin ?? (_cmdLogin = new DelegateCommand(() => LoginAsync()));
            }
        }


        private void LoginAsync()
        {
            IsLoading = true;

            Task.Factory.StartNew(LogOn).ContinueWith(t => new Action(() => { IsLoading = false; }).Dispatch());
        }


        private void LogOn()
        {
            Error = null;

            if (string.IsNullOrEmpty(UserName))
            {
                Error = "Angiv venligst et brugernavn";
                PushUIMessage("UserNameTextBoxSelectText");
            }
            else if (string.IsNullOrEmpty(Password))
            {
                Error = "Angiv venligst kodeord";
                PushUIMessage("PasswordTextBoxSelectText");
            }

            if (Error != null)
                return;

            var secMan = new BusinessLogic.Security.SecurityManager();

            Users user = null;
            string strFaultCode = null;
            try
            {
                user = secMan.LogIn(UserName, Password, ref strFaultCode);
            }
            catch (EndpointNotFoundException ep)
            {
                Error = "Det var ikke muligt at forbinde til Fiskeline. Sørg venligst for at enheden har forbindelse til internettet og prøv igen.";

                return;
            }
            catch (Exception e)
            {
                Error = "En uventet fejl opstod. " + e.Message;
                return;
            }

            if (strFaultCode != null)
            {
                if (strFaultCode.Equals("Deactivated", StringComparison.InvariantCultureIgnoreCase))
                {
                    Error = "Brugeren er deaktiveret, kontakt venligst en administrator for hjælp.";
                    LoggedInUser = null;
                    return;
                }
                else if (strFaultCode.Contains("InvalidUsernameOrPassword", StringComparison.InvariantCultureIgnoreCase))
                {
                    Regex reg = new Regex(@"^InvalidUserNameOrPassword;LoginAttemptsLeft:(\d+)", RegexOptions.IgnoreCase);
                    var m = reg.Match(strFaultCode);
                    if (m.Groups.Count > 1 && !string.IsNullOrWhiteSpace(m.Groups[1].Value))
                    {
                        string strAttemptsLeft = m.Groups[1].Value;
                        Error = string.Format("Adgangskoden var forkert, du har {0} forsøg igen.", strAttemptsLeft);
                        PushUIMessage("PasswordTextBoxSelectText");
                    }
                    else
                    {
                        Error = "Brugernavn eller adgangskode var forkert, prøv venligst igen.";
                        PushUIMessage("PasswordTextBoxSelectText");
                    }

                    LoggedInUser = null;
                    return;
                }
            }

            if (user == null)
            {
                if (!string.IsNullOrWhiteSpace(strFaultCode))
                    Error = "En uventet fejl opstod. " + strFaultCode;
                else
                    Error = "Brugernavn eller adgangskode var forkert, prøv venligst igen.";

                LoggedInUser = null;
                return;
            }


            if(!user.HasTask(Babelfisk.Entities.SecurityTask.ModifyData))
            {
                Error = "Brugeren har ikke rettigheder til at ændre data i Fiskeline og kan derfor ikke eksportere længder dertil.";
                LoggedInUser = null;
                return;
            }

            string strPassword = Password;
            new Action(() =>
            {
                BusinessLogic.Settings.Settings.Instance.UserName = UserName;
                BusinessLogic.Settings.Settings.Instance.LastLoginTime = DateTime.UtcNow;

                Close();
            }).Dispatch();
        }


        #endregion



        #region Close command


        public DelegateCommand CloseCommand
        {
            get
            {
                if (_cmdClose == null)
                    _cmdClose = new DelegateCommand(() => CloseView());

                return _cmdClose;
            }
        }


        private void CloseView()
        {
            Close();
        }


        #endregion

    }
}
