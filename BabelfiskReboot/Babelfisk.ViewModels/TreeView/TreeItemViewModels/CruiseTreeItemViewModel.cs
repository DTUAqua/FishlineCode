using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Anchor.Core.Comparers;
using Babelfisk.ViewModels.TreeView.TreeItemFilters;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;

namespace Babelfisk.ViewModels.TreeView
{
    public class CruiseTreeItemViewModel : TreeItemViewModel
    {
        private DelegateCommand<string> _cmdNewTrip;
        private DelegateCommand _cmdExportRaisedData;


        private List<TreeItemViewModel> _lstAllTrips;

        private int _intTripCount;


        public int CruiseId
        {
            get { return CruiseEntity.cruiseId; }
        }

        public override string Header
        {
            get { return CruiseEntity.cruise1; }
        }


        public bool IsOfflineCreated
        {
            get { return CruiseEntity != null && CruiseEntity.OfflineState == ObjectState.Added; }
        }
     

        public Cruise CruiseEntity
        {
            get { return _item as Cruise; }
            private set
            {
                _item = value;
                RaisePropertyChanged(() => CruiseEntity);
                RaisePropertyChanged(() => CruiseId);
                RaisePropertyChanged(() => Header);
            }
        }


        public bool TripsLoaded
        {
            get { return _lstAllTrips != null; }
        }


        private TripItemFilter TripFilter
        {
            get { return _treeItemFilter as TripItemFilter; }
        }


        public CruiseTreeItemViewModel(TreeItemViewModel parent, Cruise cruise, ITreeItemFilter treeItemFilter = null)
            : base(parent, cruise.HasTrips && parent.TreeRoot.MaxDepth != MainTreeViewModel.TreeDepth.Cruise, treeItemFilter)
        {
            CruiseEntity = cruise;
           
        }

       
        protected override void LoadChildren()
        {
            var datRes = new BusinessLogic.DataRetrievalManager();

            var lstTrips = datRes.GetTreeViewTrips(CruiseEntity.cruiseId);

            _lstAllTrips = lstTrips.OrderBy(x => x.tripType).ThenByDescending(x => x.trip1, new StringNumberComparer()).Select(x => (TreeItemViewModel)new TripTreeItemViewModel(this, x)).ToList();

            FilterAndSort(_treeItemFilter);
        }

        internal override void RefreshTree(object entity)
        {
            try
            {
                var colTrips = base.Children.OfType<TripTreeItemViewModel>().ToList();
                var cruise = entity as Cruise;

                if (cruise != null)
                {
                    CruiseEntity = cruise;

                    //If node has not been expanded yet, update its child state
                    if (colTrips == null || colTrips.Count == 0)
                    {
                        SetHasChildren(cruise.HasTrips);
                        IsExpanded = false;
                        return;
                    }

                    var datRes = new BusinessLogic.DataRetrievalManager();

                    var lstTrips = datRes.GetTreeViewTrips(CruiseEntity.cruiseId);
                    var newTrips = lstTrips.ToDictionary(x => x.tripId);

                    var existingTrips = colTrips.ToDictionary(x => x.TripEntity.tripId);

                    //Remove any old trips that are to be removed
                    foreach (var kvExisting in existingTrips)
                        if (!newTrips.ContainsKey(kvExisting.Key))
                            colTrips.Remove(kvExisting.Value);
                        else
                            kvExisting.Value.RefreshTree(newTrips[kvExisting.Key]);

                    //Add any new trips
                    foreach (var kvNew in newTrips)
                        if (!existingTrips.ContainsKey(kvNew.Key))
                            colTrips.Add(new TripTreeItemViewModel(this, kvNew.Value));

                    //Sort cruises
                    _lstAllTrips = colTrips.OrderBy(x => x.TripEntity.tripType).ThenByDescending(x => x.TripEntity.trip1, new StringNumberComparer()).OfType<TreeItemViewModel>().ToList();

                    FilterAndSort(_treeItemFilter);
                }
            }
            catch (Exception e)
            {
                DispatchMessageBox("En uventet fejl opstod under opdateringen af træet. " + e.Message);
            }
        }


        /// <summary>
        /// Filters and sort the treeviewitem children.
        /// </summary>
        /// <param name="treeFilter"></param>
        public override void FilterAndSort(ITreeItemFilter treeFilter)
        {
            base.FilterAndSort(treeFilter);

            if (_lstAllTrips == null)
                return;

            ObservableCollection<TreeItemViewModel> colTrips = null;

            //Filter on trip type
            if (TripFilter != null && !string.IsNullOrEmpty(TripFilter.TripType))
                colTrips = _lstAllTrips.OfType<TripTreeItemViewModel>().Where(x => x.TripType.Equals(TripFilter.TripType, StringComparison.InvariantCultureIgnoreCase)).OfType<TreeItemViewModel>().ToObservableCollection();
            else
                colTrips = _lstAllTrips.ToObservableCollection();

            _colChildren = colTrips;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                base.Children = colTrips;
            }));
        }


        #region Trip count methods

        private void UpdateTripCount(ITreeItemFilter treeFilter)
        {
            var datRes = new BusinessLogic.DataRetrievalManager();

            _intTripCount = datRes.GetTreeViewTripCount(CruiseEntity.cruiseId, (treeFilter as TripItemFilter) == null ? null : (treeFilter as TripItemFilter).TripType);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateTripCount();
            }));
        }


        public void UpdateTripCount()
        {
            if (_intTripCount > 0)
                base.AddDummy();
            else
                base.Children = new ObservableCollection<TreeItemViewModel>();
        }


        public void SetTripCount(int intTripCount)
        {
            _intTripCount = intTripCount;
        }

        #endregion


        protected override void OnSelected()
        {
            
        }



        #region New Trip Command


        public DelegateCommand<string> NewTripCommand
        {
            get { return _cmdNewTrip ?? (_cmdNewTrip = new DelegateCommand<string>(tripType => NewTrip(tripType))); }
        }


        private void NewTrip(string strTripType)
        {
            this.IsSelected = true;
            Input.CruiseViewModel.NewTrip(strTripType, CruiseEntity.cruiseId);
        }



        #endregion


        public bool LoadCruiseView()
        {
            var vm = new ViewModels.Input.CruiseViewModel(CruiseId);
            return AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vm);
        }



        #region Export raised data command


        public DelegateCommand ExportRaisedDataCommand
        {
            get { return _cmdExportRaisedData ?? (_cmdExportRaisedData = new DelegateCommand(ExportRaisedData)); }
        }


        private void ExportRaisedData()
        {
            int intYear = (Parent as YearTreeItemViewModel).YearEntity.Year;
            int intCruise = CruiseEntity.cruiseId;

            Menu.MainMenuViewModel.ExportData(new List<int>() { intYear, intCruise });
        }


        #endregion



        public override bool CanDeleteEntity
        {
            get
            {
                return base.CanDeleteEntity ||
                       (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline && CruiseEntity != null && CruiseEntity.OfflineState == ObjectState.Added)
                       ;
            }
        }


        protected override string DeleteConfirmationMessage
        {
            get
            {
                return string.Format("Du er ved at slette togtet '{0}'?", Header);
            }
        }

        protected override Entities.DatabaseOperationResult DeleteEntity(out string strSuccessMessage)
        {
            strSuccessMessage = "Togtet blev slettet korrekt.";
            var datMan = new BusinessLogic.DataInput.DataInputManager();
            return datMan.DeleteCruise(CruiseId);
        }

    }
}
