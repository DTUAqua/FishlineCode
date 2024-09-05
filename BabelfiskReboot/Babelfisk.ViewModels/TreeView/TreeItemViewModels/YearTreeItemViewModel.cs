using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Babelfisk.ViewModels.TreeView.TreeItemFilters;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core.Comparers;

namespace Babelfisk.ViewModels.TreeView
{
    public class YearTreeItemViewModel : TreeItemViewModel
    {
        private DelegateCommand _cmdNewCruise;

        private MainTreeViewModel _parent;

        public L_Year YearEntity
        {
            get { return _item as L_Year; }
            private set
            {
                _item = value;
                RaisePropertyChanged(() => YearEntity);
            }
        }

        public MainTreeViewModel Tree
        {
            get { return _parent; }
        }

        public YearTreeItemViewModel(MainTreeViewModel parent, L_Year year, ITreeItemFilter treeItemFilter = null)
            : base(null, year.HasCruises, treeItemFilter)
        {
            _parent = parent;
            _item = year;
        }

        public override string Header
        {
            get { return YearEntity.Year.ToString(); }
        }


        protected override void LoadChildren()
        {
            var datRes = new BusinessLogic.DataRetrievalManager();

            var lstYears = datRes.GetTreeViewCruises(YearEntity.Year, (_treeItemFilter as TripItemFilter) == null ? null : (_treeItemFilter as TripItemFilter).TripType);
            _colChildren = lstYears.OrderBy(x => x.cruise1, new StringNumberComparer()).Select(x => (TreeItemViewModel)new CruiseTreeItemViewModel(this, x, _treeItemFilter)).ToObservableCollection();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                base.Children = _colChildren;
            }));
        }


        internal override void RefreshTree(object entity)
        {
            try
            {
                var colCruises = base.Children.OfType<CruiseTreeItemViewModel>().ToList();
                var year = entity as L_Year;

                //If node has not been expanded yet, update its child state
                if (colCruises == null || colCruises.Count == 0)
                {
                    SetHasChildren(year.HasCruises);
                    IsExpanded = false;
                    return;
                }

                var datRes = new BusinessLogic.DataRetrievalManager();

                var lstCruises = datRes.GetTreeViewCruises(YearEntity.Year, (_treeItemFilter as TripItemFilter) == null ? null : (_treeItemFilter as TripItemFilter).TripType);
                var newCruises = lstCruises.ToDictionary(x => x.cruiseId);

                var existingCruises = colCruises.ToDictionary(x => x.CruiseEntity.cruiseId);

                //Remove any old years that are to be removed
                foreach (var kvExisting in existingCruises)
                    if (!newCruises.ContainsKey(kvExisting.Key))
                        colCruises.Remove(kvExisting.Value);
                    else
                        kvExisting.Value.RefreshTree(newCruises[kvExisting.Key]);

                //Add any new years
                foreach (var kvNew in newCruises)
                    if (!existingCruises.ContainsKey(kvNew.Key))
                        colCruises.Add(new CruiseTreeItemViewModel(this, kvNew.Value, _treeItemFilter));

                //Sort cruises
                var col = colCruises.OrderBy(x => x.CruiseEntity.cruise1, new StringNumberComparer()).OfType<TreeItemViewModel>().ToObservableCollection();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    base.Children = col;
                }));
            }
            catch (Exception e)
            {
                DispatchMessageBox("En unventet fejl opstod under opdateringen af træet. " + e.Message);
            }
        }




        #region New Cruise Command


        public DelegateCommand NewCruiseCommand
        {
            get { return _cmdNewCruise ?? (_cmdNewCruise = new DelegateCommand(() => NewCruise())); }
        }


        private void NewCruise()
        {
            var vm = new ViewModels.Input.CruiseViewModel(null, YearEntity.Year);
            vm.OnSaveSucceeded += vm_OnSaveSucceeded;

            AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vm);
        }

        protected void vm_OnSaveSucceeded(Input.AInputViewModel obj)
        {
            _parent.RefreshTreeAsync();
        }


        #endregion
    }
}
