using Babelfisk.Entities.Sprattus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.SmartDots
{
    public class ModifiedSampleItem :AViewModel
    {
        private ApplyChangesToSamplesViewModel _acVM;
        private SDSample _sample;
        private SelectionAnimal _selectionAnimal;
        private bool _isSelected;

        public String SelectionAnimalComment
        {
            get { return _selectionAnimal != null ? ((_selectionAnimal.SpeciesListRemark ?? "") + " " + (_selectionAnimal.AnimalRemark ?? "")).Trim() : ""; }
        }

        public String SelectionAnimalMaturity
        {
            get { return _selectionAnimal != null && _selectionAnimal.MaturityIndex != null  && !string.IsNullOrEmpty(_selectionAnimal.MaturityIndexMethod) ? ((_selectionAnimal.MaturityIndex.Value.ToString() ?? "") + " - " + (_selectionAnimal.MaturityIndexMethod ?? "")).Trim() : null; }
        }

        #region Properties
        public SDSample Sample
        {
            get { return _sample; }
        }

        public SelectionAnimal SelectionAnimal
        {
            get { return _selectionAnimal; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
                if (_acVM != null)
                    _acVM.RaiseIsAllSelected();
            }
        }

        #endregion

        public ModifiedSampleItem(ApplyChangesToSamplesViewModel acVm, SDSample sample, SelectionAnimal selectionAnimal)
        {
            _acVM = acVm;
            _sample = sample;
            _selectionAnimal = selectionAnimal;
            IsSelected = true;
        }

    }
}
