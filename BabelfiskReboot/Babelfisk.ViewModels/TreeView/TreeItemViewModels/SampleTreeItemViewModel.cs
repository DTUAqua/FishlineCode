using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;

namespace Babelfisk.ViewModels.TreeView
{
    public class SampleTreeItemViewModel : TreeItemViewModel
    {
        private DelegateCommand _cmdNewSpeciesList;
        private DelegateCommand _cmdExportRaisedData;

        public override string Header
        {
            get { return SampleEntity.station; }
        }

        public Sample SampleEntity
        {
            get { return _item as Sample; }
            set
            {
                _item = value;
                RaisePropertyChanged(() => SampleEntity);
            }
        }


        public bool IsOfflineCreated
        {
            get { return SampleEntity != null && SampleEntity.OfflineState == ObjectState.Added; }
        }


        public SampleTreeItemViewModel(TreeItemViewModel parent, Sample sample)
            : base(parent, sample.HasSpeciesList && parent.TreeRoot.MaxDepth != MainTreeViewModel.TreeDepth.Sample)
        {
            SampleEntity = sample;
        }


        protected override void LoadChildren()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                base.Children = new ObservableCollection<TreeItemViewModel>() { new SpeciesListTreeItemViewModel(this, SampleEntity) };
            }));
        }


        internal override void RefreshTree(object entity)
        {
            var colChilds = base.Children.OfType<SpeciesListTreeItemViewModel>().ToList();
            Sample s = entity as Sample;

            if (s != null)
            {
                SampleEntity = s;

                //If node has not been expanded yet, update its child state
                if (colChilds == null || colChilds.Count == 0)
                {
                    SetHasChildren(s.HasSpeciesList);
                    IsExpanded = false;
                    return;
                }

                if (s.HasSpeciesList)
                    return;
                else
                    colChilds.Clear();

                var col = colChilds.OfType<TreeItemViewModel>().ToObservableCollection();
                _colChildren = col;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    base.Children = col;
                }));
            }
        }


        /// <summary>
        /// Load station
        /// </summary>
        public bool LoadSampleView()
        {
            var vm = new ViewModels.Input.StationViewModel(SampleEntity.sampleId, SampleEntity.tripId);
            return AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vm);
        }



        #region Export raised data command


        public DelegateCommand ExportRaisedDataCommand
        {
            get { return _cmdExportRaisedData ?? (_cmdExportRaisedData = new DelegateCommand(ExportRaisedData)); }
        }


        private void ExportRaisedData()
        {
            int intYear = (Parent.Parent as CruiseTreeItemViewModel).CruiseEntity.year;
            int intCruise = (Parent as TripTreeItemViewModel).TripEntity.cruiseId;
            int intTripId = SampleEntity.tripId;
            int intSampleId = SampleEntity.sampleId;

            Menu.MainMenuViewModel.ExportData(new List<int>() { intYear, intCruise, intTripId, intSampleId });
        }


        #endregion


        #region New Species List Command

        public DelegateCommand NewSpeciesListCommand
        {
            get { return _cmdNewSpeciesList ?? (_cmdNewSpeciesList = new DelegateCommand(NewSpeciesList)); }
        }


        public void NewSpeciesList()
        {
            this.IsSelected = true;

            Input.SpeciesListViewModel.LoadSpeciesList(SampleEntity.sampleId);
        }

        #endregion



        public override bool CanDeleteEntity
        {
            get
            {
                return base.CanDeleteEntity ||
                       (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline && SampleEntity != null && SampleEntity.OfflineState == ObjectState.Added)
                       ;
            }
        }


        protected override string DeleteConfirmationMessage
        {
            get
            {
                return string.Format("Du er ved at slette stationen '{0}'?", Header);
            }
        }

        protected override Entities.DatabaseOperationResult DeleteEntity(out string strSuccessMessage)
        {
            strSuccessMessage = "Stationen blev slettet korrekt.";
            var datMan = new BusinessLogic.DataInput.DataInputManager();
            return datMan.DeleteSample(SampleEntity.sampleId);
        }
    }
}
