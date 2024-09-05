using Babelfisk.Entities.Sprattus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.SmartDots
{
    public class SelectionAnimalItem : AViewModel
    {
        private SelectionAnimal _sAnimal;
        private bool _isSelected;
        private bool _isAdded = false;
        private SDSelectAnimalsViewModel _sdsAnimalsVM;

        #region Properties
      
        public SelectionAnimal SelectionAnimal
        {
            get { return _sAnimal; }
        }

        public string Stock
        {
            get { return SelectionAnimal.StockCode; }
        }

        public string LightType
        {
            get { return SelectionAnimal.LightType; }
        }

        public string PrepMethod
        {
            get { return SelectionAnimal.PreperationMethod; }
        }

        public string OtolithDescription
        {
            get { return SelectionAnimal.OtolithDescription; }
        }

        public string CruiseYearString
        {
            get { return SelectionAnimal.CruiseYear.ToString(); }
        }

        public string GearStartDateString
        {
            get { return SelectionAnimal.GearStartDate != null ? SelectionAnimal.GearStartDate.Value.ToString("dd-MM-yyyy") : "-"; }
        }

        public string Comments
        {
            get { return ((SelectionAnimal.SpeciesListRemark ?? "") + " " + (SelectionAnimal.AnimalRemark ?? "")).Trim(); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
                if (_sdsAnimalsVM != null)
                    _sdsAnimalsVM.RaiseIsAllSelected();
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
        #endregion

        public SelectionAnimalItem(SDSelectAnimalsViewModel sdsAnimals,  SelectionAnimal selectionAnimal)
        {
            _sAnimal = selectionAnimal;
            _sdsAnimalsVM = sdsAnimals;
        }

        public void SetIsSelected(bool val)
        {
            IsSelected = val;
        }

    }
}
