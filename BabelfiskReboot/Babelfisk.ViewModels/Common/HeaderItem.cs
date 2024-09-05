using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.Common
{
    public class HeaderItem : AViewModel
    {
        private string _strHeader;

        private string _strValue;

        private string _strTooltip;

        public string Header
        {
            get { return _strHeader; }
            set
            {
                _strHeader = value;
                RaisePropertyChanged(() => Header);
            }
        }

        public string Value
        {
            get { return _strValue; }
            set
            {
                _strValue = value;
                RaisePropertyChanged(() => Value);
            }
        }


        public string ToolTip
        {
            get { return _strTooltip; }
            set
            {
                _strTooltip = value;
                RaisePropertyChanged(() => ToolTip);
            }
        }


        public HeaderItem(string strHeader, string strValue, string strTooltip = null)
        {
            _strHeader = strHeader;
            _strValue = strValue;
            _strTooltip = strTooltip;
        }

    }
}
