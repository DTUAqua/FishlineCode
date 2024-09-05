using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.Offline
{
    public class CompareItem : AViewModel
    {
        private string _strHeader;

        private string _strServerValue;

        private string _strClientValue;


        #region Properties

        public string Header
        {
            get { return _strHeader; }
        }

        public string ServerValue
        {
            get { return _strServerValue; }
        }

        public string ClientValue
        {
            get { return _strClientValue; }
        }


        public bool HasDifferences
        {
            get 
            {
                if (_strServerValue == null && _strClientValue != null)
                    return true;

                if (_strServerValue != null && _strClientValue == null)
                    return true;

                if (_strServerValue == null && _strClientValue == null)
                    return false;

                return !_strServerValue.Equals(_strClientValue); 
            }
        }

        #endregion


        public CompareItem(string strHeader, string strServerValue, string strClientValue)
        {
            _strHeader = strHeader;
            _strServerValue = strServerValue;
            _strClientValue = strClientValue;
        }

    }
}
