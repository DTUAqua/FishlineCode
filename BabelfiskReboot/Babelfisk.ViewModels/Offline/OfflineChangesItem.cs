using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.BusinessLogic.Offline;

namespace Babelfisk.ViewModels.Offline
{
    public class OfflineChangesItem : AViewModel
    {
        private OfflineDictionaryItem _offlineItem;

        private bool _blnIsChecked;

        private bool _blnCanCheck;

        private bool _blnIsSynchronized;

        public OfflineDictionaryItem OfflineItem
        {
            get { return _offlineItem; }
        }


        public bool IsChecked
        {
            get { return _blnIsChecked; }
            set
            {
                _blnIsChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }


        public bool IsSynchronized
        {
            get { return _blnIsSynchronized; }
            set
            {
                _blnIsSynchronized = value;
                RaisePropertyChanged(() => IsSynchronized);
            }
        }


        public bool CanCheck
        {
            get { return _blnCanCheck; }
        }


        public OfflineChangesItem(OfflineDictionaryItem itm, bool blnCanSelect = true)
        {
            _blnCanCheck = blnCanSelect;
            _offlineItem = itm;
            IsChecked = true;
        }
    }
}
