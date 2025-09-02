using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using System.Text.RegularExpressions;
using FishLineMeasure.ViewModels.Infrastructure;
using System.Collections.ObjectModel;
using System.Reflection;
using System;
using System.Collections.Generic;
using FishLineMeasure.ViewModels.CustomControls;
using System.Linq;
using Babelfisk.Entities;
using System.Threading.Tasks;
using Babelfisk.Entities.Sprattus;

namespace FishLineMeasure.ViewModels.Lenghts
{
    public class AddRowViewModel : AViewModel
    {

        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdAdd;
        private DelegateCommand _cmdSyncLookups;

        private ObservableCollection<BoxCatagoryControlViewModel> _lookupLists;

        private List<ILookupEntity> _lSpecies;


        #region Properties

        public ObservableCollection<BoxCatagoryControlViewModel> LookupLists
        {
            get { return _lookupLists; }
            set
            {
                _lookupLists = value;
                RaisePropertyChanged(() => LookupLists);
            }
        }
        #endregion

        public AddRowViewModel()
        {
            WindowWidth = 930;
            WindowHeight = 925;
           
            MinWindowWidth = 520;
            MinWindowHeight = 575;

            AdjustWindowWidthHeightToScreen();

            IsDirty = false;
        }


        public void InitializeAsync()
        {
            IsLoading = true;

            Task.Run(() => LoadLookupLists())
            .ContinueWith(t => new Action(() =>
            {
                IsLoading = false;
            }).Dispatch());
        }

        private void LoadLookupLists()
        {
            try
            {
                var getInfo = new BusinessLogic.LookupManager();
                var lstTypes = Lookups.LookupsViewModel.GetLookupTypesList;

                List<BoxCatagoryControlViewModel> lst = new List<BoxCatagoryControlViewModel>();
                foreach (var t in lstTypes)
                {
                    var lookupListName = Lookups.LookupsViewModel.GetLookupDisplayName(t);
                    var lstLookups = getInfo.GetLookups(t);

                    //Skip list if no lookups were found
                    if (lstLookups == null || lstLookups.Count == 0)
                        continue;

                    if(t == typeof(L_Species))
                        _lSpecies = lstLookups;

                    //Convert from ILookupEntity to LookupItemViewModels
                    var lstLookupItems = lstLookups.Select(x => Lookups.LookupItemViewModel.Create(x)).ToList();

                    var cat = new BoxCatagoryControlViewModel(this, lookupListName, lstLookupItems, t);
                    lst.Add(cat);
                }

                new Action(() =>
                {
                    LookupLists = lst.ToObservableCollection();
                }).Dispatch();
            }
            catch (Exception e)
            {
                LogError(e);
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }
        }


        public void SelectedBoxCategoryItemChanged(BoxCatagoryControlViewModel boxCategory)
        {
            try
            {
                if(LookupLists == null || boxCategory == null || boxCategory.SelectedItem == null || boxCategory.LookupType != typeof(L_Species) || _lSpecies == null || boxCategory.SelectedItem.Entity == null)
                    return;

                var lookupSpecies = _lSpecies.Where(x => x.Id != null && x.Id.Equals(boxCategory.SelectedItem.Entity.Id, StringComparison.InvariantCulture)).FirstOrDefault() as L_Species;

                if(lookupSpecies == null || lookupSpecies.standardLengthMeasureTypeId == null)
                    return;

                var lengthMeasureList = LookupLists.Where(x => x.LookupType == typeof(L_LengthMeasureType)).FirstOrDefault();

                if(lengthMeasureList == null || lengthMeasureList.Lookups == null || lengthMeasureList.Lookups.Count == 0)
                    return;

                var item = lengthMeasureList.Lookups.Where(x => x.Entity != null && x.Entity.Id != null && x.Entity.Id.Equals(lookupSpecies.standardLengthMeasureTypeId.Value.ToString(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                if(item != null)
                    lengthMeasureList.Lookup_OnCheckedChanged(item, false, true);
            }
            catch { }
        }


        public string IsValidByRegexPattern(string input)
        {
            string validationString = null;
            string pattern = @"(([\\/:*?""'><|])|(\[+)|(\]+))";
            MatchCollection match = Regex.Matches(input, pattern);
            foreach (Match item in match)
            {
                validationString = validationString + item.Value;
            }

            return validationString;
        }


        protected override string ValidateField(string properties)
        {
            string error = null;
            if (!_blnValidate)
                return error;

            switch (properties)
            {
                case "LookupLists":
                    if (LookupLists == null || LookupLists.Count == 0 || !LookupLists.Where(x => x.HasSelectedLookup).Any())
                        error = "Vælg venligst mindst en lookup (art og længdemålingstype er obligatoriske)";
                    else
                    {
                        var selectedLookups = LookupLists.Where(x => x.HasSelectedLookup).Select(x => x.SelectedLookup).ToList();

                        if (!selectedLookups.Where(x => x.Type == typeof(L_Species).Name).Any())
                            error = "En måling skal altid gemmes under en art. Vælg venligst en art og prøv igen.";
                        else if(LookupLists.Where(x => x.LookupType == typeof(L_LengthMeasureType)).Any() && !selectedLookups.Where(x => x.Type == typeof(L_LengthMeasureType).Name).Any())
                            error = "Længdemålingstype er obligatorisk. Vælg venligst en længdemålingstype og prøv igen.";
                    }

                    break;

                default:
                    break;
            }

            return error;
        }


        #region Add Row command


        public DelegateCommand AddRowCommand
        {
            get { return _cmdAdd ?? (_cmdAdd = new DelegateCommand(AddRow)); }
        }

        private void AddRow()
        {
            ValidateAllProperties();
            if (HasErrors)
                return;

            IsDirty = true;
            this.Close();
        }


        #endregion


        #region Cancel Command

        public DelegateCommand CancelThisCommand
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(CancelThis)); }
        }

        private void CancelThis()
        {
            this.Close();
        }

        #endregion


        #region Sync Lookups again Command


        public DelegateCommand SyncLookupsCommand
        {
            get { return _cmdSyncLookups ?? (_cmdSyncLookups = new DelegateCommand(SyncLookups)); }
        }


        private void SyncLookups() // needs fixing
        {
            try
            {
                if(LookupLists != null && LookupLists.Where(x => x.HasSelectedLookup).Any())
                {
                    if(AppRegionManager.ShowMessageBox("Du har valgt en eller flere koder i listerne nedenfor. Hvis du opdaterer kodelisterne (lookups), vil dine valg blive nulstillet. Ønsker du at fortsætte?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                        return;
                }

                var vmLookups = new Lookups.LookupsViewModel();
                var task = vmLookups.SyncLookupsAsync();
                task.ContinueWith(ta => new Action(() =>
                {
                    try
                    {
                        vmLookups.Close();

                        InitializeAsync();
                    }
                    catch { }
                }).Dispatch());
                AppRegionManager.LoadWindowViewFromViewModel(vmLookups);
            }
            catch(Exception e)
            {
                LogError(e);
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }   
        }


        #endregion
    }
}
