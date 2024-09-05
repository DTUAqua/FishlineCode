using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;
using Babelfisk.BusinessLogic.Offline;
using Anchor.Core.Comparers;
using Babelfisk.Entities.Sprattus;
using Babelfisk.BusinessLogic.DataInput;
using Babelfisk.Entities;
using Babelfisk.BusinessLogic;
using System.Threading;


namespace Babelfisk.ViewModels.Offline
{
    public class GoOnlineViewModel : AViewModel
    {
        private DelegateCommand _cmdGoOnline;
        private DelegateCommand _cmdCancel;

        private List<OfflineChangesItem> _offlineChanges;

        private bool _blnIsSynchronizing;

        private OnlineProcessState _processState;

        private OfflineChangesItem _selectedOfflineItem;



        #region Properties


        public OfflineChangesItem SelectedOfflineItem
        {
            get { return _selectedOfflineItem; }
            set
            {
                _selectedOfflineItem = value;
                RaisePropertyChanged(() => SelectedOfflineItem);
            }
        }


        public List<OfflineChangesItem> OfflineChanges
        {
            get { return _offlineChanges; }
            set
            {
                _offlineChanges = value;
                RaisePropertyChanged(() => OfflineChanges);
                RaisePropertyChanged(() => HasOfflineChanges);
            }
        }


        public bool HasOfflineChanges
        {
            get { return _offlineChanges != null && _offlineChanges.Count > 0; }
        }


        public bool IsSynchronizing
        {
            get { return _blnIsSynchronizing; }
            set
            {
                _blnIsSynchronizing = value;
                RaisePropertyChanged(() => IsSynchronizing);
            }
        }


        public OnlineProcessState ProcessState
        {
            get { return _processState; }
            set
            {
                _processState = value;
                RaisePropertyChanged(() => ProcessState);
            }
        }


        #endregion


        public GoOnlineViewModel()
        {
            WindowWidth = 600;
            WindowHeight = 350;

            WindowTitle = "Gå online";

            InitializeAsync();

            try
            {
                //Make sure memory is freeed, when loading new forms.
                GC.Collect();
            }
            catch { }
        }


        private void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(Initialize).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private void Initialize()
        {
            List<OfflineChangesItem> lstChanges = new List<OfflineChangesItem>();
            try
            {
                var od = BusinessLogic.Offline.OfflineDictionary.Instance;

                var lstLookups = od.ChangedLookups.Values.Select(x => new OfflineChangesItem(x, false)).ToList();
                var lstCruises = od.ChangedCruises.Values.Select(x => new OfflineChangesItem(x)).ToList();
                var lstTrips = od.ChangedTrips.Values.Select(x => new OfflineChangesItem(x)).ToList();
                
                lstChanges.AddRange(lstLookups.OrderBy(x => x.OfflineItem.EntityType.Name).ThenBy(x => x.OfflineItem.Name, new StringNumberComparer()));
                lstChanges.AddRange(lstCruises.OrderBy(x => x.OfflineItem.Name, new StringNumberComparer()));
                lstChanges.AddRange(lstTrips.OrderBy(x => x.OfflineItem.Name, new StringNumberComparer()));
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }

            new Action(() =>
            {
                OfflineChanges = lstChanges;

            }).Dispatch();
        }



        #region Go Online Command


        public DelegateCommand GoOnlineCommand
        {
            get
            {
                return _cmdGoOnline ?? (_cmdGoOnline = new DelegateCommand(GoOnlineAsync));
            }
        }


        private void GoOnlineAsync()
        {
            //If user regrets going online, return.
            if (OfflineChanges != null && OfflineChanges.Count > 0 && OfflineChanges.Where(x => x.IsChecked == false).Any() &&
                AppRegionManager.ShowMessageBox("Fravalgte tilføjelser/ændringer vil ikke blive synkroniseret til land, men vil blive slettet, efter applikationen er online igen. Er du sikker på du vil fortsætte?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            IsSynchronizing = true;
            SelectedOfflineItem = null;
            Task.Factory.StartNew(GoOnline).ContinueWith(t => new Action(() => IsSynchronizing = false).Dispatch());
        }


        private void GoOnline()
        {
            try
            {
                OfflineManager om = new OfflineManager();

                new Action(() => ProcessState = OnlineProcessState.TestingConnectivity).Dispatch();

                bool blnAvailable = om.IsLandDatabaseAvailable();

                //If no connection to the database could be established, return with an error message.
                if (!blnAvailable)
                {
                    new Action(() => 
                    {
                        ProcessState = OnlineProcessState.Idle;
                        AppRegionManager.ShowMessageBox("Det var ikke muligt at oprette forbindelse til databasen på land. Tjek venligst at computeren har adgang til internettet og prøv igen.");
                    }).Dispatch();
                    return;
                }

                new Action(() => ProcessState = OnlineProcessState.SyncLookups).Dispatch();

                //If synchronizing lookups fails, return (so the user can try again).
                if(!SyncLookups(om))
                {
                    new Action(() => { ProcessState = OnlineProcessState.Idle; }).Dispatch();
                    return;
                }

                new Action(() => ProcessState = OnlineProcessState.SyncCruises).Dispatch();

                //If synchronizing cruises fails, return (so the user can try again).
                if (!SyncCruises(om))
                {
                    new Action(() => { ProcessState = OnlineProcessState.Idle; }).Dispatch();
                    return;
                }

                //If synchronizing trips fails, return (so the user can try again).
                new Action(() => ProcessState = OnlineProcessState.SyncTrips).Dispatch();

                if (!SyncTrips(om))
                {
                    new Action(() => { ProcessState = OnlineProcessState.Idle; }).Dispatch();
                    return;
                }

                //Save offline state
                BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline = false;
                BusinessLogic.Settings.Settings.Instance.Save();

                //Save any changes to the offline dictionary (maybe it should be reset here actually)
                OfflineDictionary.Instance.Save();
            }
            catch (Exception e)
            {
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
                new Action(() => ProcessState = OnlineProcessState.Idle).Dispatch();
                Anchor.Core.Loggers.Logger.LogError(e);
                return;
            }

            Babelfisk.ViewModels.Security.BackupManager.Instance.EndCurrentBackupSession(DateTime.UtcNow);

            //Sleep so UI has time to show updates before going online.
            Thread.Sleep(1000);

            new Action(() =>
            {
                BusinessLogic.Settings.Settings.Instance.OfflineStatus.RaiseAllPropertiesChanged();
                MainTree.InitializeTreeViewAsync();
                AppRegionManager.RefreshWindowTitle();
                IsSynchronizing = false;
                Close();

                if(_offlineChanges.Count > 0)
                    AppRegionManager.ShowMessageBoxDefaultTimeout("Ændringer/tilføjelser blev synkroniseret korrekt og programmet er online igen.");
                else
                    AppRegionManager.ShowMessageBoxDefaultTimeout("Programmet er online igen.");
            }).Dispatch();
        }


        private bool SyncLookups(OfflineManager om)
        {
            SyncDatabaseOperationResult res = om.SynchronizeLookups();
            if (res.DatabaseOperationResult.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
            {
                DispatchMessageBox(String.Format("Det var ikke muligt at synkronisere alle koder - synkroniseringen annulleres. " + res.DatabaseOperationResult.Message));
                return false;
            }

            //Set lookups as synchronized
            new Action(() =>
            {
                _offlineChanges.Where(x => typeof(ILookupEntity).IsAssignableFrom(x.OfflineItem.EntityType)).ToList().ForEach(x =>
                {
                    x.IsSynchronized = true;
                });
            }).Dispatch();

            return true;
        }


        private bool SyncCruises(OfflineManager om)
        {
            DataInputManager datMan = new DataInputManager();

            foreach (var itm in _offlineChanges.Where(x => x.IsChecked && x.OfflineItem.EntityType == typeof(Cruise)))
            {
                Cruise c = datMan.GetEntity<Cruise>(itm.OfflineItem.Id);

                SyncDatabaseOperationResult res = om.SynchronizeCruise(ref c);

                if (res.DatabaseOperationResult.DatabaseOperationStatus == Entities.DatabaseOperationStatus.DuplicateRecordException)
                {
                    var are = new AutoResetEvent(false);
                    CompareEntitiesViewModel ceVM = null;
                    //Show user differences between the two cruises and make the select overwriting method.
                    new Action(() =>
                    {
                        ceVM = new CompareEntitiesViewModel(itm.OfflineItem.Name, c.OfflineComparisonEntity as Cruise, c);
                        AppRegionManager.LoadWindowViewFromViewModel(ceVM, true, "WindowWithBorderStyle");
                        are.Set();
                    }).Dispatch();

                    are.WaitOne();

                    if (ceVM != null && ceVM.IsSyncCancelled)
                        return false;

                    c.OverwritingMethod = ceVM == null ? OverwritingMethod.ServerWins : ceVM.SelectedOverwritingMethod;

                    //Mark it as modified now (if added), since a record already exists.
                    if (c.ChangeTracker.State == ObjectState.Added)
                    {
                        c.AssignNavigationPropertyWithoutChanges("cruiseId", (res.ExistingEntity as Cruise).cruiseId);
                        c.MarkAsModified();
                    }

                    res = om.SynchronizeCruise(ref c);
                }

                if (res.DatabaseOperationResult.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                {
                    DispatchMessageBox(String.Format("Togtet '{0}' kunne ikke synkroniseres. Synkroniseringen annulleres og du kan derfor prøve at gå online igen eller fravælge at synkronisere det pågældende togt.", c.year + " - " + c.cruise1));
                    return false;
                }

                new Action(() =>
                {
                    itm.IsSynchronized = true;
                    itm.IsChecked = false;
                }).Dispatch();

            }

            return true;
        }


        private bool SyncTrips(OfflineManager om)
        {
            DataInputManager datMan = new DataInputManager();

            foreach (var itm in _offlineChanges.Where(x => x.IsChecked && x.OfflineItem.EntityType == typeof(Trip)))
            {
                Trip t = datMan.GetEntity<Trip>(itm.OfflineItem.Id);
                //Make a copy of the trip, so any changes to it, won't be reflected anywhere.
                t = t.Clone();
               
                SyncDatabaseOperationResult res = om.SynchronizeTrip(ref t);

                if (res.DatabaseOperationResult.DatabaseOperationStatus == Entities.DatabaseOperationStatus.DuplicateRecordException)
                {
                    CompareEntitiesViewModel ceVM = null;

                    if (t.OfflineComparisonEntity != null)
                    {
                        var are = new AutoResetEvent(false);
                        //Show user differences between the two cruises and make the select overwriting method.
                        new Action(() =>
                        {
                            ceVM = new CompareEntitiesViewModel(itm.OfflineItem.Name, t.OfflineComparisonEntity as Trip, t);
                            AppRegionManager.LoadWindowViewFromViewModel(ceVM, true, "WindowWithBorderStyle");
                            are.Set();
                        }).Dispatch();

                        are.WaitOne();

                        if (ceVM != null && ceVM.IsSyncCancelled)
                            return false;

                        //Loop through all trips and samples and apply overwriting method and ids
                        t.OverwritingMethod = ceVM == null ? OverwritingMethod.ServerWins : ceVM.SelectedOverwritingMethod;

                        //Mark it as modified now (if added), since a record already exists.
                        if (t.ChangeTracker.State == ObjectState.Added)
                        {
                            t.AssignNavigationPropertyWithoutChanges("tripId", (t.OfflineComparisonEntity as Trip).tripId);
                            t.MarkAsModified();

                            //Handle HVN
                            if (t.IsHVN && t.Sample.Count > 0 && (t.OfflineComparisonEntity as Trip).Sample.Count > 0)
                            {
                                t.Sample.First().AssignNavigationPropertyWithoutChanges("sampleId", (t.OfflineComparisonEntity as Trip).Sample.First().sampleId);
                                t.Sample.First().MarkAsModified();
                            }
                        }
                    }

                    if (!t.IsHVN)
                    {
                        foreach (var s in t.Sample)
                        {
                            if (s.OfflineComparisonEntity != null)
                            {
                                var are = new AutoResetEvent(false);
                                //Show user differences between the two cruises and make the select overwriting method.
                                new Action(() =>
                                {
                                    ceVM = new CompareEntitiesViewModel(itm.OfflineItem.Name + " -> " + s.station, s.OfflineComparisonEntity as Sample, s);
                                    AppRegionManager.LoadWindowViewFromViewModel(ceVM, true, "WindowWithBorderStyle");
                                    are.Set();
                                }).Dispatch();

                                are.WaitOne();

                                if (ceVM != null && ceVM.IsSyncCancelled)
                                    return false;

                                s.OverwritingMethod = ceVM == null ? OverwritingMethod.ServerWins : ceVM.SelectedOverwritingMethod;

                                //Mark it as modified now (if added), since a record already exists.
                                if (s.ChangeTracker.State == ObjectState.Added)
                                {
                                    s.AssignNavigationPropertyWithoutChanges("sampleId", (s.OfflineComparisonEntity as Sample).sampleId);
                                    s.MarkAsModified();
                                }
                            }
                        }
                    }

                    res = om.SynchronizeTrip(ref t);
                }

                if (res.DatabaseOperationResult.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                {
                    Anchor.Core.Loggers.Logger.Log( Anchor.Core.Loggers.LogType.Error, string.Format("An error occured while going online. Error message: {0}.", res.DatabaseOperationResult.Message ?? ""));
                    DispatchMessageBox(String.Format("Turen '{0}' kunne ikke synkroniseres. Synkroniseringen afbrydes og du kan derfor prøve at gå online igen eller/og fravælge at synkronisere det pågældende togt.\nBesked til Administrator: {1}", itm.OfflineItem.Name, res.DatabaseOperationResult.Message ?? "N/A"));
                    return false;
                }

                var itmCopy = itm;
                new Action(() =>
                {
                    itmCopy.IsSynchronized = true;
                    itmCopy.IsChecked = false;
                }).Dispatch();
            }

            return true;
        }


        #endregion


        #region Cancel Command


        public DelegateCommand CancelCommand
        {
            get
            {
                return _cmdCancel ?? (_cmdCancel = new DelegateCommand(Cancel));
            }
        }


        private void Cancel()
        {
            try
            {
                //Save offline dictionary if user cancels synchronization, so if he/she closes the application and opens it again,
                //the offlinestate is restored.
                OfflineDictionary.Instance.Save();
            }
            catch
            { }

            if (_offlineChanges.Where(x => x.IsSynchronized).Count() > 0)
            {
                AppRegionManager.ShowMessageBox("Noget af offline-dataen er blevet synkroniseret til land (rækker markeret med grøn baggrundsfarve). Dette data vil derfor ikke optræde på listen igen, næste gang du vælger at gå online og synkronisere data til land.", System.Windows.MessageBoxButton.OK);
            }

            Close();
        }


        #endregion


        public override void FireClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            base.FireClosing(sender, e);

            if (IsSynchronizing)
            {
                e.Cancel = true;
                DispatchMessageBox("Vent venligst indtil synkroniseringen er færdig, inden du lukker vinduet.");
            }
        }
    }
}
