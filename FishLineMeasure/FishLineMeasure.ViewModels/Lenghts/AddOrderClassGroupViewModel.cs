using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Lenghts
{
    public class AddOrderClassGroupViewModel : AViewModel
    {
        private DelegateCommand _cmdOK;

        private DelegateCommand _cmdCancel;

        private OrderViewModel _vmOrder;

        private bool _isCanceled = false;


        private string _name;


        #region Properties


        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }


        public bool IsCanceled
        {
            get { return _isCanceled; }
        }


        #endregion


        public AddOrderClassGroupViewModel(OrderViewModel vmOrder)
        {
            _vmOrder = vmOrder;

            WindowWidth = 600;
            WindowHeight = 420;

            AdjustWindowWidthHeightToScreen();
        }

        protected override string ValidateField(string strFieldName)
        {
            string error = null;

            if (!_blnValidate)
                return error;

            switch(strFieldName)
            {
                case "Name":
                    if (string.IsNullOrWhiteSpace(Name))
                        error = "Angiv venligst et navn til rækkefølgen af længdefordelinger.";
                    else if (_vmOrder != null && _vmOrder.GroupsList != null && _vmOrder.GroupsList.Where(x => x.Name != null && x.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase)).Any())
                        error = "En gemt rækkefølge med samme navn, findes allerede. Indtast venligst et andet navn og prøv igen.";
                    break;
            }

            return error;
        }


        #region OK Command


        public DelegateCommand OKCommand
        {
            get { return _cmdOK ?? (_cmdOK = new DelegateCommand(OK)); }
        }

        private void OK()
        {
            ValidateAllProperties();

            if (HasErrors)
                return;

            Close();
        }


        #endregion


        #region Cancel Command


        public DelegateCommand CancelCommand
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(Cancel)); }
        }

        private void Cancel()
        {
            _isCanceled = true;

            Close();
        }


        #endregion


    }
}
