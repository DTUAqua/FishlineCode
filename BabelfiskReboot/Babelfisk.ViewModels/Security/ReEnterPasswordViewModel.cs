using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;

namespace Babelfisk.ViewModels.Security
{
    public class ReEnterPasswordViewModel : AViewModel
    {
        private DelegateCommand _cmdOK;
        private DelegateCommand _cmdClose;


        private string _strPassword;

        private bool _blnOKClicked;

        private string _strMessage;


        #region Properties


        public bool OKClicked
        {
            get { return _blnOKClicked; }
            set
            {
                _blnOKClicked = value;
                RaisePropertyChanged(() => OKClicked);
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


        public string Message
        {
            get { return _strMessage; }
            set
            {
                _strMessage = value;
                RaisePropertyChanged(() => Message);
            }
        }


        #endregion


        public ReEnterPasswordViewModel()
        {
            WindowTitle = "Advarsel";
            WindowWidth = 500;
            WindowHeight = Double.NaN;
        }



        #region OK command



        public DelegateCommand OKCommand
        {
            get
            {
                if (_cmdOK == null)
                    _cmdOK = new DelegateCommand(() => OK());

                return _cmdOK;
            }
        }


        private void OK()
        {
            OKClicked = false;
            IsLoading = true;

            Error = null;

            if (string.IsNullOrEmpty(Password))
                Error = "Angiv venligst et kodeord";

            if (Error != null)
                return;

            string strHash = Entities.Hash.ComputeHash(Password) ?? "";

            if (User.Password != strHash)
            {
                Error = "Det indtastede kodeord er forkert, prøv venligst igen.";
                return;
            }

            OKClicked = true;

            Close();
        }
        #endregion



        #region Close command


        public DelegateCommand CloseCommand
        {
            get
            {
                if (_cmdClose == null)
                    _cmdClose = new DelegateCommand(() => CloseClick());

                return _cmdClose;
            }
        }


        private void CloseClick()
        {
            Close();
        }

        #endregion
    }
}

