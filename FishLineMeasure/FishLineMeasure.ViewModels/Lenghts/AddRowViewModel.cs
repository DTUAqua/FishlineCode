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

        private static ObservableCollection<BoxCatagoryControlViewModel> _lookupLists;
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
            if (LookupLists != null)
            {
                //Reset all lookups to not checked.
                foreach (var l in _lookupLists)
                    l.UnCheckAll();
            }
            else
            {
                IsLoading = true;

                Task.Run(() => LoadLookupLists())
                .ContinueWith(t => new Action(() =>
                {
                    IsLoading = false;
                }).Dispatch());
            }
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

                    //Convert from ILookupEntity to LookupItemViewModels
                    var lstLookupItems = lstLookups.Select(x => Lookups.LookupItemViewModel.Create(x)).ToList();

                    var cat = new BoxCatagoryControlViewModel(lookupListName, lstLookupItems);
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
                        error = "Vælg venligst mindst en lookup";
                    else
                    {
                        var selectedLookups = LookupLists.Where(x => x.HasSelectedLookup).Select(x => x.SelectedLookup).ToList();

                        if (!selectedLookups.Where(x => x.Type == typeof(L_Species).Name).Any())
                            error = "En måling skal altid gemmes under en art. Vælg venligst en art også.";
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

            IsDirty = true ;
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
            var vmLookups = new Lookups.LookupsViewModel();
            var task = vmLookups.SyncLookupsAsync();
            task.ContinueWith(ta => new Action(() =>
            {
                vmLookups.Close();
            }).Dispatch());
            AppRegionManager.LoadWindowViewFromViewModel(vmLookups);
           
        }

        #endregion
    }
}
