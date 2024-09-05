using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.SprattusSecurity
{
    public partial class FishLineTasks
    {
        private bool _blnIsChecked;


        public bool IsChecked
        {
            get { return _blnIsChecked; }
            set
            {
                _blnIsChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
    }
}
