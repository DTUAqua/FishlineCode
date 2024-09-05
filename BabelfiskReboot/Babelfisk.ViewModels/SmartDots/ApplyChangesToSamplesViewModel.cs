using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.SmartDots
{
    
    public class ApplyChangesToSamplesViewModel: AViewModel
    {
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdApplyChanges;

        private List<ModifiedSampleItem> _lstModifiedSamples;
        private List<ModifiedSampleItem> _lstFilteredModifiedSamples;
        private List<ModifiedSampleItem> _lstSelectedModifiedSamples;

        private string _searchString;
        private bool closed = false;

        #region Properties
        public List<ModifiedSampleItem> ModifiedSampleList
        {
            get { return _lstModifiedSamples; }
            set
            {
                _lstModifiedSamples = value;
                RaisePropertyChanged(() => ModifiedSampleList);
                RaisePropertyChanged(() => TotalSamples);
            }
        }

        public List<ModifiedSampleItem> FilteredModifiedSampleList
        {
            get { return _lstFilteredModifiedSamples; }
            set
            {
                _lstFilteredModifiedSamples = value;
                RaisePropertyChanged(() => FilteredModifiedSampleList);
                RaisePropertyChanged(() => TotalVisibleSamples);
            }
        }


        public List<ModifiedSampleItem> SelectedModifiedSampleList
        {
            get { return _lstSelectedModifiedSamples; }
            set
            {
                _lstSelectedModifiedSamples = value;
                RaisePropertyChanged(() => SelectedModifiedSampleList);
            }
        }

        public string SearchString
        {
            get { return _searchString; }
            set 
            {
                _searchString = value;
                RaisePropertyChanged(() => SearchString);
            }
        }

        public int TotalVisibleSamples
        {
            get { return _lstFilteredModifiedSamples == null ? 0 : _lstFilteredModifiedSamples.Count; }
        }

        public int TotalSamples
        {
            get { return _lstModifiedSamples == null ? 0 : _lstModifiedSamples.Count; }
        }

        public bool? IsAllSelected
        {
            get
            {

                if (_lstFilteredModifiedSamples != null && _lstFilteredModifiedSamples.Count > 0 && _lstFilteredModifiedSamples.Count == _lstFilteredModifiedSamples.Where(x => x.IsSelected).ToList().Count())
                    return true;
                else if (_lstFilteredModifiedSamples != null && _lstFilteredModifiedSamples.Any(x => x.IsSelected))
                    return null;
                else
                    return false;
            }
            set
            {
                SetAllCheckboxes();
                RaisePropertyChanged(() => IsAllSelected);

            }
        }

        public bool HasSelectedSamples
        {
            get { return IsAllSelected == null || IsAllSelected == true; }
        }

        public int SelectedSamplesCount
        {
            get { return _lstFilteredModifiedSamples != null ? _lstFilteredModifiedSamples.Where(x => x.IsSelected).Count() : 0; }
        }
        #endregion

        public ApplyChangesToSamplesViewModel(SDSamplesViewModel sdSamples)
        {
            WindowWidth = 900;
            WindowHeight = 500;

            var st = sdSamples.EventSpeciesString;
            WindowTitle = string.Format(Translate("ApplyChangesToSamplesView", "Title"), sdSamples.Event == null  ? "" : (sdSamples.Event.name ?? ""), string.IsNullOrWhiteSpace(st) ? "" : st);

            _lstModifiedSamples = new List<ModifiedSampleItem>();
        }

        public void FilterModifiedSamples()
        {
            if (_lstModifiedSamples == null)
            {
                FilteredModifiedSampleList = new List<ModifiedSampleItem>();
                return;
            }

            IEnumerable<ModifiedSampleItem> lstSamples = _lstModifiedSamples;

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                var search = SearchString ?? "";

                //TODO ad search in sample.speciesCode column
                lstSamples = lstSamples.Where(x => !string.IsNullOrEmpty(x.Sample.animalId) && x.Sample.animalId.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                     (x.SelectionAnimal.AnimalId != 0 && x.SelectionAnimal.AnimalId.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.SelectionAnimal.SpeciesCode) && x.SelectionAnimal.SpeciesCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.cruise) && x.Sample.cruise.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.SelectionAnimal.Cruise) && x.SelectionAnimal.Cruise.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.trip) && x.Sample.trip.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.SelectionAnimal.Trip) && x.SelectionAnimal.Trip.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.station) && x.Sample.station.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.SelectionAnimal.Station) && x.SelectionAnimal.Station.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.DFUArea) && x.Sample.DFUArea.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.SelectionAnimal.AreaCode) && x.SelectionAnimal.AreaCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.statisticalRectangle) && x.Sample.statisticalRectangle.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.SelectionAnimal.StatisticalRectangle) && x.SelectionAnimal.StatisticalRectangle.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.L_Stock != null && x.Sample.L_Stock.stockCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.SelectionAnimal.StockCode) && x.SelectionAnimal.StockCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.fishWeightG.HasValue && x.Sample.fishWeightG.Value.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.SelectionAnimal.WeightG.HasValue && x.SelectionAnimal.WeightG.Value.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.fishLengthMM.HasValue && x.Sample.fishLengthMM.Value.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.SelectionAnimal.LengthMM.HasValue && x.SelectionAnimal.LengthMM.Value.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.sexCode) && x.Sample.sexCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.SelectionAnimal.SexCode) && x.SelectionAnimal.SexCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.Maturity != null && x.Sample.Maturity.UIDisplay.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.SelectionAnimalMaturity) && x.SelectionAnimalMaturity.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.L_OtolithReadingRemark != null && x.Sample.L_OtolithReadingRemark.otolithReadingRemark.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.SelectionAnimal.OtolithReadingRemarkCode) && x.SelectionAnimal.OtolithReadingRemarkCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.edgeStructure) && x.Sample.edgeStructure.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.SelectionAnimal.EdgeStructureCode) && x.SelectionAnimal.EdgeStructureCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.comments) && x.Sample.comments.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.SelectionAnimalComment) && x.SelectionAnimalComment.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.catchDate != null && x.Sample.catchDate.Value.ToString("dd-MM-yyyy").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.SelectionAnimal.GearStartDate != null && x.SelectionAnimal.GearStartDate.Value.ToString("dd-MM-yyyy").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)));
            }

            FilteredModifiedSampleList = lstSamples.ToList();
        }

        private void SetAllCheckboxes()
        {
            if (_lstFilteredModifiedSamples != null && _lstFilteredModifiedSamples.Count > 0)
            {
                if (_lstFilteredModifiedSamples.Count == _lstFilteredModifiedSamples.Where(x => x.IsSelected).ToList().Count())
                    foreach (var item in _lstFilteredModifiedSamples)
                    {
                        item.IsSelected = false;
                    }
                else if (_lstFilteredModifiedSamples.Any(x => x.IsSelected))
                    foreach (var item in _lstFilteredModifiedSamples)
                    {
                        item.IsSelected = false;
                    }
                else
                    foreach (var item in _lstFilteredModifiedSamples)
                    {
                        item.IsSelected = true;
                    }
            }

            RaisePropertyChanged(() => FilteredModifiedSampleList);
            RaiseIsAllSelected();
        }

        public void RaiseIsAllSelected()
        {
            RaisePropertyChanged(() => IsAllSelected);
            RaisePropertyChanged(() => HasSelectedSamples);
            RaisePropertyChanged(() => SelectedSamplesCount);
        }

        public override void FireClosing(object sender, CancelEventArgs e)
        {
            if (closed)
                return;
            else
            {
                var res = AppRegionManager.ShowMessageBox(Translate("ApplyChangesToSamplesView", "CantLeaveTheWindow"), System.Windows.MessageBoxButton.OK);
                e.Cancel = true;
            }
           

        }


        #region Cancel Command
        public DelegateCommand CancelCommand
        {
            get
            {
                if (_cmdCancel == null)
                    _cmdCancel = new DelegateCommand(() => Cancel());

                return _cmdCancel;
            }
        }

        private void Cancel()
        {
            SelectedModifiedSampleList = null;
            closed = true;
            Close();
        }

        #endregion

        #region Apply Changes Command

        public DelegateCommand ApplyChangesCommand
        {
            get
            {
                if (_cmdApplyChanges == null)
                    _cmdApplyChanges = new DelegateCommand(() => Save());

                return _cmdApplyChanges;
            }
        }

        private void Save()
        {

            if (!HasSelectedSamples)
            {
                var res = AppRegionManager.ShowMessageBox(Translate("ApplyChangesToSamplesView", "WarningNoSelectedSamples"), System.Windows.MessageBoxButton.OK);
                return;
            }

            if(FilteredModifiedSampleList != null && FilteredModifiedSampleList.Count > 0 && HasSelectedSamples)
            {
                SelectedModifiedSampleList = FilteredModifiedSampleList.Where(x => x.IsSelected).ToList();
            }
            closed = true;
            Close();
        }
        #endregion

    }
}
