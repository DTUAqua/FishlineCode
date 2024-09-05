using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using System.Collections.ObjectModel;
using Anchor.Core.Comparers;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Practices.Prism.Commands;

namespace Babelfisk.ViewModels.TreeView
{
    public class TripTreeItemViewModel : TreeItemViewModel
    {
        private DelegateCommand _cmdNewStation;
        private DelegateCommand _cmdNewSpeciesList;
        private DelegateCommand _cmdImportStation;
        private DelegateCommand _cmdExportRaisedData;

        public Trip TripEntity
        {
            get { return _item  as Trip; }
            set
            {
                _item = value;
                RaisePropertyChanged(() => TripEntity);
                RaisePropertyChanged(() => TripType);
                RaisePropertyChanged(() => Header);
            }
        }


        public bool IsOfflineCreated
        {
            get { return TripEntity != null && TripEntity.OfflineState == ObjectState.Added; }
        }

        public string TripType
        {
            get { return TripEntity.tripType == null ? null : TripEntity.tripType.ToLower(); }
        }

        public bool IsHVN
        {
            get { return TripType != null && TripType.Equals("hvn", StringComparison.InvariantCultureIgnoreCase); }
        }


        public TripTreeItemViewModel(TreeItemViewModel parent, Trip trip)
            : base(parent, ((!trip.tripType.Equals("hvn", StringComparison.InvariantCultureIgnoreCase) && trip.HasSamples) || 
                            (trip.tripType.Equals("hvn", StringComparison.InvariantCultureIgnoreCase) && trip.HasSpeciesLists && parent.TreeRoot.MaxDepth != MainTreeViewModel.TreeDepth.Sample)) && 
                            parent.TreeRoot.MaxDepth != MainTreeViewModel.TreeDepth.Trip)
        {
            TripEntity = trip;
        }


        public override string Header
        {
            get { return TripEntity.tripType + " - " + TripEntity.trip1; }
        }


        protected override void LoadChildren()
        {
            ObservableCollection<TreeItemViewModel> col = null;

            var datRes = new BusinessLogic.DataRetrievalManager();
            var lstSamples = datRes.GetTreeViewSamples(TripEntity.tripId);

            var sample = lstSamples.FirstOrDefault();
            //If hvn, load the species list node instantly
            if (TripType.Equals("hvn", StringComparison.InvariantCultureIgnoreCase))
            {
                if(sample != null && sample.HasSpeciesList)
                    col = new ObservableCollection<TreeItemViewModel>() { new SpeciesListTreeItemViewModel(this, sample) };
            }
            else //if any other trip type, load the samples
            {
                col = lstSamples.OrderByDescending(x => x.station, new StringNumberComparer()).Select(x => (TreeItemViewModel)new SampleTreeItemViewModel(this, x)).ToObservableCollection();
            }

            _colChildren = col;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                base.Children = col;
            }));

        }


        internal override void RefreshTree(object entity)
        {
            var colChilds = base.Children == null ? null : base.Children.OfType<SampleTreeItemViewModel>().ToList();
           
            var trip = entity as Trip;

            if (trip != null)
            {
                TripEntity = trip;

                ObservableCollection<TreeItemViewModel> col = null;

                if (TripType.Equals("hvn", StringComparison.InvariantCultureIgnoreCase))
                {
                    var colHvnChilds = base.Children == null ? new List<SpeciesListTreeItemViewModel>() : base.Children.OfType<SpeciesListTreeItemViewModel>().ToList();

                    var datRes = new BusinessLogic.DataRetrievalManager();
                    var sample = datRes.GetTreeViewSamples(TripEntity.tripId).FirstOrDefault();

                    //If node has not been expanded yet, update its child state
                    if (colHvnChilds == null || colHvnChilds.Count == 0)
                    {
                        new Action(() =>
                        {
                            SetHasChildren((sample == null ? false : (sample.HasSpeciesList)));
                            IsExpanded = false;
                        }).Dispatch();
                        return;
                    }

                    if (sample.HasSpeciesList)
                    {
                        if (colHvnChilds.Count == 0)
                            colHvnChilds.Add(new SpeciesListTreeItemViewModel(this, sample));
                        else
                            return;
                    }
                    else
                        colHvnChilds.Clear();

                    col = colHvnChilds.OfType<TreeItemViewModel>().ToObservableCollection();
                }
                else
                {
                    //If node has not been expanded yet, update its child state
                    if (colChilds == null || colChilds.Count == 0)
                    {
                        SetHasChildren(trip.HasSamples);
                        IsExpanded = false;
                        return;
                    }

                    var datRes = new BusinessLogic.DataRetrievalManager();
                    var lstSamples = datRes.GetTreeViewSamples(TripEntity.tripId);
                    var newSamples = lstSamples.ToDictionary(x => x.sampleId);
                    var existingSamples = colChilds.ToDictionary(x => x.SampleEntity.sampleId);

                    //Remove any old trips that are to be removed
                    foreach (var kvExisting in existingSamples)
                        if (!newSamples.ContainsKey(kvExisting.Key))
                            colChilds.Remove(kvExisting.Value);
                        else
                            kvExisting.Value.RefreshTree(newSamples[kvExisting.Key]);

                    //Add any new trips
                    foreach (var kvNew in newSamples)
                        if (!existingSamples.ContainsKey(kvNew.Key))
                            colChilds.Add(new SampleTreeItemViewModel(this, kvNew.Value));

                    col = colChilds.OrderByDescending(x => x.SampleEntity.station, new StringNumberComparer()).OfType<TreeItemViewModel>().ToObservableCollection();
                }

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    base.Children = col;
                }));
            }
        }


        protected override void OnSelected()
        {
           
        }


        public bool LoadTripView()
        {
            AViewModel vm = null;

            if (TripEntity.tripType == "HVN")
                vm = new ViewModels.Input.TripHVNViewModel(TripEntity.tripId);
            else
                vm = new ViewModels.Input.TripViewModel(TripEntity.tripId);

            return AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vm);
        }


        #region New Station Command

        public DelegateCommand NewStationCommand
        {
            get { return _cmdNewStation ?? (_cmdNewStation = new DelegateCommand(NewStation)); }
        }


        private void NewStation()
        {
            this.IsSelected = true;

            Input.TripViewModel.NewStation(TripEntity.tripId);
        }


        #endregion


        #region New Species List Command

        public DelegateCommand NewSpeciesListCommand
        {
            get { return _cmdNewSpeciesList ?? (_cmdNewSpeciesList = new DelegateCommand(NewSpeciesList)); }
        }


        public void NewSpeciesList()
        {
            Task.Factory.StartNew(() =>
            {
                var datRes = new BusinessLogic.DataRetrievalManager();
                var lstSamples = datRes.GetTreeViewSamples(TripEntity.tripId);
                var s = lstSamples == null ? null : lstSamples.FirstOrDefault();

                if (s != null)
                {
                    new Action(() =>
                    {
                        this.IsSelected = true;
                        Input.SpeciesListViewModel.LoadSpeciesList(s.sampleId);
                    }).Dispatch();
                }
                else
                    DispatchMessageBox("Tilhørende station til valgte tur kunne ikke lokaliseres (den er obligatorisk for at kunne lave en ny artsliste). Hvis fejlen fortsætter, kontakt venligst en administrator.");
            });
        }

        #endregion


        #region Import Station Command

        public DelegateCommand ImportStationCommand
        {
            get { return _cmdImportStation ?? (_cmdImportStation = new DelegateCommand(() => ImportStation(TripEntity.tripId))); }
        }


        public static void ImportStation(int intTripId)
        {
            Import.ImportStationViewModel vm = new Import.ImportStationViewModel(intTripId);
            _appRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderStyle");
        }

        #endregion


        #region Export raised data command


        public DelegateCommand ExportRaisedDataCommand
        {
            get { return _cmdExportRaisedData ?? (_cmdExportRaisedData = new DelegateCommand(ExportRaisedData)); }
        }


        private void ExportRaisedData()
        {
            int intYear = (Parent as CruiseTreeItemViewModel).CruiseEntity.year;
            int intCruise = TripEntity.cruiseId;
            int intTripId = TripEntity.tripId;

            Menu.MainMenuViewModel.ExportData(new List<int>() { intYear, intCruise, intTripId });
        }


        #endregion


        public override bool CanDeleteEntity
        {
            get
            {
                return base.CanDeleteEntity ||
                       (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline && TripEntity != null && TripEntity.OfflineState == ObjectState.Added)
                       ;
            }
        }


        protected override string DeleteConfirmationMessage
        {
            get
            {
                if(IsHVN)
                    return string.Format("Du er ved at slette '{0}'.", Header);
                else
                    return string.Format("Du er ved at slette turen '{0}'.", Header);
            }
        }

        protected override Entities.DatabaseOperationResult DeleteEntity(out string strSuccessMessage)
        {
            strSuccessMessage = "Turen blev slettet korrekt.";
            var datMan = new BusinessLogic.DataInput.DataInputManager();
            return datMan.DeleteTrip(TripEntity.tripId);
        }
    }
}
