using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using Babelfisk.ViewModels.TreeView.TreeItemFilters;

namespace Babelfisk.ViewModels.TreeView
{
    public class MainTreeViewModel : AViewModel
    {
        public enum TreeDepth
        {
            Year = 1,
            Cruise = 2, 
            Trip = 3,
            Sample = 4, 
            SpeciesList = 5
        }

        private DelegateCommand _cmdImportRekreaData;
        private DelegateCommand _cmdNewYear;
        private DelegateCommand _cmdRefresh;
        private DelegateCommand<string> _cmdTripType;

        private TreeDepth _enmTreeDepth;

        private bool _blnIsFilterVisible;

        private ObservableCollection<YearTreeItemViewModel> _colYears;

        private string _strSelectedTreeType = "all";

        private bool _blnIsDeletingEntity;


        #region Properties


        public TreeDepth MaxDepth
        {
            get { return _enmTreeDepth; }
        }

        public bool IsFilterVisible
        {
            get { return _blnIsFilterVisible; }
            set
            {
                _blnIsFilterVisible = value;
                RaisePropertyChanged(() => IsFilterVisible);
            }
        }


        public ObservableCollection<YearTreeItemViewModel> Years
        {
            get { return _colYears; }
            private set
            {
                _colYears = value;
                RaisePropertyChanged(() => Years);
            }
        }

        public string SelectedTreeType
        {
            get { return _strSelectedTreeType; }
            set
            {
                _strSelectedTreeType = value;
                RaisePropertyChanged(() => SelectedTreeType);
            }
        }


        public bool IsDeletingEntity
        {
            get { return _blnIsDeletingEntity; }
            set
            {
                _blnIsDeletingEntity = value;
                RaisePropertyChanged(() => IsDeletingEntity);
            }
        }

        #endregion

        public MainTreeViewModel(bool blnIsFilterVisible = false, TreeDepth enmTreeDepth = TreeDepth.SpeciesList, bool blnInitialize = true)
        {
            _enmTreeDepth = enmTreeDepth;
            _blnIsFilterVisible = blnIsFilterVisible;
            if(blnInitialize)
                InitializeTreeViewAsync();
        }


        public Task InitializeTreeViewAsync()
        {
            try
            {
                IsLoading = true;
                TripTypeCommand.RaiseCanExecuteChanged();
            }
            catch(Exception e)
            {
                LogError(e);
            }

            return Task.Factory.StartNew(InitializeTreeView).ContinueWith(InitializingTreeViewDone);
        }


        private void InitializingTreeViewDone(IAsyncResult res)
        {
            new Action(() =>
            {
                try
                {
                    IsLoading = false;
                    TripTypeCommand.RaiseCanExecuteChanged();
                }
                catch (Exception e)
                {
                    LogError(e);
                }
            }).Dispatch();
        }

        private void InitializeTreeView()
        {
            try
            {
                var datRes = new BusinessLogic.DataRetrievalManager();

                var lstYears = datRes.GetTreeViewYears();
                _colYears = lstYears.OrderByDescending(x => x.Year).Select(x => new YearTreeItemViewModel(this, x)).ToObservableCollection();

                new Action(() =>
                {
                    try
                    {
                        Years = _colYears;
                    }
                    catch(Exception e)
                    {
                        LogError(e);
                    }
                }).Dispatch();
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }


        public Task RefreshTreeAsync(Action OnDone = null)
        {
            IsLoading = true;
            TripTypeCommand.RaiseCanExecuteChanged();

            return Task.Factory.StartNew(() => RefreshTree(OnDone)).ContinueWith(InitializingTreeViewDone);
        }


        private void RefreshTree(Action OnDone)
        {
            try
            {
                var datRes = new BusinessLogic.DataRetrievalManager();

                var lstYears = datRes.GetTreeViewYears();

                var newYears = lstYears.ToDictionary(x => x.Year);
                var existingYears = Years.ToDictionary(x => x.YearEntity.Year);
                var colYears = Years.ToList();

                //Remove any old years that are to be removed
                foreach (var kvExisting in existingYears)
                    if (!newYears.ContainsKey(kvExisting.Key))
                        colYears.Remove(kvExisting.Value);
                    else
                        kvExisting.Value.RefreshTree(newYears[kvExisting.Key]);

                //Add any new years
                foreach (var kvNew in newYears)
                    if (!existingYears.ContainsKey(kvNew.Key))
                        colYears.Add(new YearTreeItemViewModel(this, kvNew.Value));

                var col = colYears.OrderByDescending(x => x.YearEntity.Year).ToObservableCollection();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Years = col;

                    if (OnDone != null)
                        OnDone();
                }));
            }
            catch (Exception e)
            {
                DispatchMessageBox("En unventet fejl opstod under opdateringen af træet. " + e.Message);
            }
        }


        #region Trip Type Command

        public DelegateCommand<string> TripTypeCommand
        {
            get { return _cmdTripType ?? (_cmdTripType = new DelegateCommand<string>(p => SetTripType(p), p => CanExecuteTripTypeCommand(p))); }
        }

        public bool CanExecuteTripTypeCommand(string strTripType)
        {
            return !IsLoading;
        }


        private void SetTripType(string strTripType)
        {
            if (_colYears == null || IsLoading)
                return;

            IsLoading = true;
            TripTypeCommand.RaiseCanExecuteChanged();

            SelectedTreeType = strTripType.ToLower();

            TripItemFilter tf = new TripItemFilter(strTripType);

            if (strTripType.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                tf = null;

            //Start new thread updating trips counts for selected trip type.
            Task.Factory.StartNew(() =>
            {
                List<Action> lstUpdates = new List<Action>();

                try
                {
                    //Get all cruises where trips have not been loaded yet.
                    var dicCruiseIds = _colYears.SelectMany(x => x.Children).OfType<CruiseTreeItemViewModel>().Where(x => !x.TripsLoaded).ToDictionary(x => x.CruiseId, x => x);

                    //Get all trip counts for cruises where trips have not been loaded yet.
                    var datRes = new BusinessLogic.DataRetrievalManager();
                    var dicRes = datRes.GetTreeViewTripCounts(dicCruiseIds.Keys.ToArray(), tf == null ? null : tf.TripType);

                    //Update trip counts for cruises where trips are not loaded yet.
                    foreach (var cc in dicRes)
                        if (dicCruiseIds.ContainsKey(cc.Key))
                        {
                            dicCruiseIds[cc.Key].SetTripCount(cc.Value);
                            lstUpdates.Add(new Action(dicCruiseIds[cc.Key].UpdateTripCount));
                        }

                    //Update trip counts for cruises where trips are already loaded.
                    foreach (var y in _colYears)
                    {
                        y.FilterAndSort(tf);
                    }
                }
                catch { }

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    //Update trip count (UI) for cruises where trips are not loaded yet.
                    foreach (var a in lstUpdates)
                        a();

                    IsLoading = false;
                    TripTypeCommand.RaiseCanExecuteChanged();
                }));
            });

        }

        #endregion


        #region New Year Command


        public DelegateCommand NewYearCommand
        {
            get { return _cmdNewYear ?? (_cmdNewYear = new DelegateCommand(() => AddYearAndCruise())); }
        }


        private void AddYearAndCruise()
        {
            var vm = new ViewModels.Input.CruiseViewModel(null);

            AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vm);
        }


        #endregion


        #region Refresh Command


        public DelegateCommand RefreshCommand
        {
            get { return _cmdRefresh ?? (_cmdRefresh = new DelegateCommand(() => RefreshTree())); }
        }


        private void RefreshTree()
        {
            RefreshTreeAsync();
        }


        #endregion


        #region Import Rekrea Data Command


        public DelegateCommand ImportRekreaDataCommand
        {
            get { return _cmdImportRekreaData ?? (_cmdImportRekreaData = new DelegateCommand(() => ImportRekreaData())); }
        }


        public static void ImportRekreaData()
        {
            Import.ImportRekreaDataViewModel vm = new Import.ImportRekreaDataViewModel();
            _appRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderStyle");
        }


        #endregion



        public void ExpandToAndSelectCruise(int intYear, int intCruiseId)
        {
            new Action(() => IsLoading = true).Dispatch();
            Task.Factory.StartNew(() =>
            {
                var node = this.GetCruiseNodeIfLoaded(intCruiseId);

                if (node == null)
                {
                    var yNode = this.Years.Where(x => x.YearEntity.Year == intYear).FirstOrDefault();
                    if (yNode != null)
                    {
                        var t = yNode.SetIsExpanded(true);
                        Task.WaitAll(t);
                        SelectAndCheckNode(this.GetCruiseNodeIfLoaded(intCruiseId));
                    }
                }
                else
                    SelectAndCheckNode(node);

                new Action(() => IsLoading = false).Dispatch();
            });
        }


        public void ExpandToAndSelectTrip(int intYear, int intCruiseId, int intTripId)
        {
            new Action(() => IsLoading = true).Dispatch();
            Task.Factory.StartNew(() =>
            {
                var node = this.GetTripNodeIfLoaded(intTripId);

                if (node == null)
                {
                    var yNode = this.Years.Where(x => x.YearEntity.Year == intYear).FirstOrDefault();
                    if (yNode != null)
                    {
                        var t = yNode.SetIsExpanded(true);
                        Task.WaitAll(t);
                        var cNode = yNode.Children.OfType<CruiseTreeItemViewModel>().Where(y => y.CruiseEntity.cruiseId == intCruiseId).FirstOrDefault();

                        if (cNode != null)
                        {
                            var tc = cNode.SetIsExpanded(true);
                            Task.WaitAll(tc);
                            SelectAndCheckNode(this.GetTripNodeIfLoaded(intTripId));
                        }
                    }
                }
                else
                    SelectAndCheckNode(node);
                new Action(() => IsLoading = false).Dispatch();
            });
        }


        public void ExpandToAndSelectSample(int intYear, int intCruiseId, int intTripId, int intSampleId)
        {
            new Action(() => IsLoading = true).Dispatch();
            Task.Factory.StartNew(() =>
            {
                var node = this.GetSampleNodeIfLoaded(intSampleId);

                if (node == null)
                {
                    var yNode = this.Years.Where(x => x.YearEntity.Year == intYear).FirstOrDefault();
                    if (yNode != null)
                    {
                        var t = yNode.SetIsExpanded(true);
                        Task.WaitAll(t);
                        var cNode = yNode.Children.OfType<CruiseTreeItemViewModel>().Where(y => y.CruiseEntity.cruiseId == intCruiseId).FirstOrDefault();

                        if (cNode != null)
                        {
                            var tc = cNode.SetIsExpanded(true);
                            Task.WaitAll(tc);

                            var tNode = cNode.Children.OfType<TripTreeItemViewModel>().Where(trip => trip.TripEntity.tripId == intTripId).FirstOrDefault();
                            if (tNode != null)
                            {
                                var tt = tNode.SetIsExpanded(true);
                                Task.WaitAll(tt);
                                SelectAndCheckNode(this.GetSampleNodeIfLoaded(intSampleId));
                            }
                        }
                    }
                }
                else
                    SelectAndCheckNode(node);

                new Action(() => IsLoading = false).Dispatch();
            });
        }


        private void SelectAndCheckNode(TreeView.TreeItemViewModel node)
        {
            if(node != null)
                new Action(() => { node.IsSelected = true; node.IsChecked = true; MainTree.ExpandToNode(node); IsLoading = false; }).Dispatch();
        }



        public void SelectTreeNode(int intId, TreeNodeLevel nodeLevel)
        {
            //First dispatch to the main thread (so it will be fired after any other dispatched updates to the tree).
            new Action(() =>
            {
                //Select node asynchroneously.
                Task.Factory.StartNew(() =>
                {
                    TreeItemViewModel node = null;

                    //Select node if possible
                    switch (nodeLevel)
                    {
                        case TreeNodeLevel.Cruise:
                            node = this.GetCruiseNodeIfLoaded(intId);
                            break;

                        case TreeNodeLevel.Trip:
                            node = this.GetTripNodeIfLoaded(intId);
                            break;

                        case TreeNodeLevel.Sample:
                            node = this.GetSampleNodeIfLoaded(intId);
                            break;

                        case TreeNodeLevel.SpeciesList:
                            node = this.GetSpeciesListNodeIfLoaded(intId);
                            break;
                    }

                    if (node != null)
                        new Action(() => { node.IsSelected = true; this.ExpandToNode(node); }).Dispatch();
                });
            }).Dispatch();
        }


        public CruiseTreeItemViewModel GetCruiseNodeIfLoaded(int intCruiseId)
        {
            if (_colYears == null)
                return null;

            var cruiseTreeItems = _colYears.SelectMany(x => x.Children.OfType<CruiseTreeItemViewModel>().Where(y => y.CruiseId == intCruiseId));

            return cruiseTreeItems.FirstOrDefault();
        }


        public TripTreeItemViewModel GetTripNodeIfLoaded(int intTripId)
        {
            if (_colYears == null)
                return null;

           var tripTreeItems = _colYears.SelectMany(x => x.Children.OfType<CruiseTreeItemViewModel>()).SelectMany(x => x.Children.OfType<TripTreeItemViewModel>()).Where(x => x.TripEntity.tripId == intTripId);

           return tripTreeItems.FirstOrDefault();
        }

        public SampleTreeItemViewModel GetSampleNodeIfLoaded(int intSampleId)
        {
            if (_colYears == null)
                return null;

            var treeItems = _colYears.SelectMany(x => x.Children.OfType<CruiseTreeItemViewModel>())
                                         .SelectMany(x => x.Children.OfType<TripTreeItemViewModel>())
                                         .SelectMany(x => x.Children.OfType<SampleTreeItemViewModel>())
                                         .Where(x => x.SampleEntity.sampleId == intSampleId);

            return treeItems.FirstOrDefault();
        }


        public SpeciesListTreeItemViewModel GetSpeciesListNodeIfLoaded(int intSampleId)
        {
            if (_colYears == null)
                return null;

            //First search for specieslist tree items below sample tree items.
            var treeItems = _colYears.SelectMany(x => x.Children.OfType<CruiseTreeItemViewModel>())
                                         .SelectMany(x => x.Children.OfType<TripTreeItemViewModel>())
                                         .SelectMany(x => x.Children.OfType<SampleTreeItemViewModel>())
                                         .SelectMany(x => x.Children.OfType<SpeciesListTreeItemViewModel>())
                                         .Where(x => x.SampleEntity.sampleId == intSampleId);

            //If none is found, look for specieslist tree items below trip tree items.
            if(treeItems.Count() == 0)
                treeItems = _colYears.SelectMany(x => x.Children.OfType<CruiseTreeItemViewModel>())
                                         .SelectMany(x => x.Children.OfType<TripTreeItemViewModel>())
                                         .SelectMany(x => x.Children.OfType<SpeciesListTreeItemViewModel>())
                                         .Where(x => x.SampleEntity.sampleId == intSampleId);

            return treeItems.FirstOrDefault();
        }


        public YearTreeItemViewModel GetYearNodeIfLoaded(int intYear)
        {
            if (_colYears == null)
                return null;

            var yearTreeItems = _colYears.Where(y => y.YearEntity.Year == intYear);

            return yearTreeItems.FirstOrDefault();

        }


        public void ExpandToNode(TreeItemViewModel tivm)
        {
            if (tivm.Parent != null)
            {
                ExpandToNode(tivm.Parent);
                if(!tivm.Parent.IsExpanded)
                    tivm.Parent.IsExpanded = true;
            }
        }


        /// <summary>
        /// Deselect any selected tree nodes.
        /// </summary>
        public Task DeselectAllAsync()
        {
            return Task.Factory.StartNew(DeselectAll);
        }



        /// <summary>
        /// Deselect any selected tree nodes.
        /// </summary>
        public void DeselectAll()
        {
            foreach (var itm in _colYears)
                Deselect(itm);
        }


        /// <summary>
        /// Recursive method deselecting any selected node from the passed TreeItemViewModel and down-wards.
        /// </summary>
        private void Deselect(TreeItemViewModel tivm)
        {
            try
            {
                if (tivm.IsSelected)
                {
                    new Action(() =>
                    {
                        tivm.IsSelected = false;
                    }).Dispatch();
                }

                if (tivm.Children != null)
                    for (int i = 0; i < tivm.Children.Count; i++)
                        Deselect(tivm.Children[i]);
            }
            catch
            { 
                //Just swallow the exception, the try-catch is only here to make sure that this relatively unimportant method does not crash the app in any way. 
            }
        }
    }
}
