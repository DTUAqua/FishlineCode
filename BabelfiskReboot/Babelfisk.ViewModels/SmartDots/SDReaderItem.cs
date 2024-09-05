using Babelfisk.Entities.Sprattus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.SmartDots
{
    public class SDReaderItem : AViewModel
    {
        private R_SDReader _sdreader;
        private bool _isSelected;
        private bool _isAdded;
        private int _numberOfReadings;

        private SelectSDReadersViewModel _selectSDReadersVM;

        #region Properties


        public R_SDReader SDReader
        {
            get { return _sdreader; }
            set 
            {
                _sdreader = value;
                RaisePropertyChanged(() => SDReader);
            }
        }


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if(!IsAdded)
                    _isSelected = value;
                if (_selectSDReadersVM != null)
                    _selectSDReadersVM.RaiseIsAllSelected();
                RaisePropertyChanged(() => IsSelected);
            }
        }


        public bool IsAdded
        {
            get { return _isAdded; }
            set
            {
                _isAdded = value;
                RaisePropertyChanged(() => IsAdded);
            }
        }


        public int NumberOfReadings
        {
            get { return _numberOfReadings; }
            set
            {
                _numberOfReadings = value;
                RaisePropertyChanged(() => NumberOfReadings);
            }
        }


        #endregion

        public SDReaderItem(SelectSDReadersViewModel vmssdr, R_SDReader reader)
        {
            _selectSDReadersVM = vmssdr;
            _sdreader = reader;
            _isSelected = false;
            _isAdded = false;
        }

        public void SetIsSelected(bool val)
        {
            _isSelected = val;
            RaisePropertyChanged(() => IsSelected);
        }
    }
}
