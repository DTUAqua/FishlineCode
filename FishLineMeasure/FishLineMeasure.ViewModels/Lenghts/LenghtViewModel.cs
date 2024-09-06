using System;
using System.Collections.Generic;
using System.Linq;
using FishLineMeasure.ViewModels.Menu;
using FishLineMeasure.ViewModels.Infrastructure;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using FishLineMeasure.ViewModels.Overview;
using System.Xml.Linq;
using Anchor.Core;
using System.Threading.Tasks;
using System.IO;
using Windows.Devices.Bluetooth;
using System.ComponentModel;
using System.Globalization;
using FishLineMeasure.BusinessLogic;

namespace FishLineMeasure.ViewModels.Lenghts
{
    public class LenghtViewModel : AViewModel
    {
        private DelegateCommand _cmdArrowUp;
        private DelegateCommand _cmdArrowDown;
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdOrderView;
        private DelegateCommand _cmdNextStation;
        private DelegateCommand<MeasurementsClass> _cmdDeleteRow;
        private DelegateCommand _cmdDeleteMeasurementsForActiveOrder;
        
        private OverviewViewModel Copy;
        private ObservableCollection<OrderClass> _orders;
        private ObservableCollection<MeasurementsClass> _colMeasurements;
        private ObservableCollection<StationViewModel> StationsCollection;
        private OrderClass _selectedOrder;
        private XElement _xElmStation;
        private XElement _xElmMeasurements;


        #region Properties

        public int MeasurementsInLengthGroup
        {
            get
            {
                int count = 0;
                try
                {
                    if (SelectedOrder != null)
                    {
                        var gs = SelectedOrder.GroupString;

                        if (gs != null)
                        {
                            var lst = Measurements.Where(x => x.Lookups.GroupString.Equals(gs, StringComparison.InvariantCultureIgnoreCase)).ToList();
                            count = lst.Count;
                        }
                    }
                }
                catch { }

                return count;
            }
        }

        private void RaiseMeasurementsChanged()
        {
            RaisePropertyChanged(nameof(MeasurementsInLengthGroup));
        }


        public ObservableCollection<OrderClass> Orders
        {
            get
            {
                return _orders;
            }
            set

            {
                _orders = value;
                RaisePropertyChanged(() => Orders);
                RaisePropertyChanged(() => HasOrders);
                RaisePropertyChanged(() => HasMoreThanTwoOrders);
            }
        }

        public bool HasOrders
        {
            get { return Orders != null && Orders.Count > 0; }

        }

        public bool HasMoreThanTwoOrders
        {
            get { return Orders != null && Orders.Count > 1; }

        }

        public OrderClass PrevOrder
        {
            get
            {
                var odr = GetPrevOrder();
                return odr;
            }
        }

        public OrderClass NextOrder
        {
            get
            {
                var odr = GetNextOrder();
                return odr;
            }
        }

        public OrderClass SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                RaisePropertyChanged(nameof(SelectedOrder));
                RaisePropertyChanged(nameof(PrevOrder));
                RaisePropertyChanged(nameof(NextOrder));
                RaiseMeasurementsChanged();
            }
        }

        public ObservableCollection<MeasurementsClass> Measurements
        {
            get { return _colMeasurements; }
            set
            {
                _colMeasurements = value;
                RaisePropertyChanged(() => Measurements);
                RaiseMeasurementsChanged();
            }
        }

        #region Trip And Station Properties

        private string _station;
        private bool _moreStations;
        private string _NextStationOnList;


        public int Year { get; set; }
        public string Cruise { get; set; }
        public string Trip { get; set; }
        public string Station
        {
            get { return _station; }
            set
            {
                _station = value;
                RaisePropertyChanged(nameof(Station));
            }
        }
        public string NextStationOnList
        {
            get { return _NextStationOnList; }
            set
            {
                _NextStationOnList = value;
                RaisePropertyChanged(nameof(NextStationOnList));
                RaisePropertyChanged(nameof(NextStationOnListUIString));
            }
        }


        public string NextStationOnListUIString
        {
            get
            {
                return (_NextStationOnList ?? "") == "" ? "" : string.Format("({0})", _NextStationOnList);
            }
        }

        public bool MoreStations
        {
            get { return _moreStations; }
            set
            {
                _moreStations = value;
                RaisePropertyChanged(nameof(MoreStations));
            }
        }


        public int IndexOfStationSelceted { get; set; }


        #endregion



        #endregion


        public LenghtViewModel(TripViewModel selectedTrip, StationViewModel selectedStation, OverviewViewModel overviewViewModel)
        {
            MoreStations = true;
            Copy = overviewViewModel;
            Year = selectedTrip.Year;
            Cruise = selectedTrip.Cruise;
            Trip = selectedTrip.Trip;
            StationsCollection = selectedTrip.GetOrderedStations();
            IndexOfStationSelceted = StationsCollection.ToList().FindIndex(x => x.StationNumber == selectedStation.StationNumber);
            Station = StationsCollection[IndexOfStationSelceted].StationNumber;

            if (IndexOfStationSelceted - 1 < 0)
                MoreStations = false;

            if (MoreStations == true)
                NextStationOnList = StationsCollection[IndexOfStationSelceted - 1].StationNumber;

            Main.Menu.BCWLE.OnDataValueChanged += BLE_OnDataValueChanged;
            Measurements = new ObservableCollection<MeasurementsClass>();
            
        }




        /// <summary>
        /// Initializes length view.
        /// </summary>
        public void InitializeAsync()
        {
            IsLoading = true;

            Task.Run(() =>
            {
                try
                {
                    var selectedGroupName = BusinessLogic.Settings.Settings.Instance.SelectedLengthGroupName;
                    var lstGroups = OrderViewModel.LoadLengthGroupsCollectionFromSettings();
                    OrderClassGroup selectedGroup = null;

                    if (!string.IsNullOrWhiteSpace(selectedGroupName))
                        selectedGroup = lstGroups.Where(x => x.Name != null && x.Name.Equals(selectedGroupName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    if (selectedGroup == null)
                        selectedGroup = lstGroups.FirstOrDefault();

                    ObservableCollection<OrderClass> lstOrders;

                    if (selectedGroup != null && selectedGroup.OrderClasses != null)
                        lstOrders = selectedGroup.OrderClasses.ToObservableCollection();//OrderViewModel.LoadLengthGroupsFromSettings());
                    else
                        lstOrders = new ObservableCollection<OrderClass>();

                    new Action(() =>
                    {
                        Orders = lstOrders;
                        if (Orders != null && Orders.Count > 0)
                            SelectedOrder = Orders[0];
                    }).Dispatch();


                    //Load measurements from DISK
                    var lstMeasurements = LoadMeasurements();

                    new Action(() =>
                    {
                        Measurements = new ObservableCollection<MeasurementsClass>(lstMeasurements);
                    }).Dispatch();
                }
                catch (Exception e)
                {
                    LogError(e);
                    DispatchMessageBox(string.Format("En uventet fejl opstod under indlæsningen af længdegrupper og målinger. {0}", e.Message ?? ""));
                }
            })
            .ContinueWith(t => new Action(() =>
            {
                IsLoading = false;

                new Action(() =>
                {
                    try
                    {
                        if (Measurements != null && Measurements.Count > 0)
                            PushUIMessage($"LengthListEnd_{Measurements.Count}");
                    }
                    catch { }
                }).Dispatch(System.Windows.Threading.DispatcherPriority.ContextIdle);
            }).Dispatch());

        }


        /// <summary>
        /// Load measurements from station XML file.
        /// </summary>
        /// <returns></returns>
        private List<MeasurementsClass> LoadMeasurements()
        {
            AssignXElements();

            var curStation = StationsCollection[IndexOfStationSelceted];

            List<MeasurementsClass> res = curStation.GetMeasurementClasses(_xElmMeasurements);

            return res;
        }


        /// <summary>
        /// Save measurements to disk.
        /// </summary>
        public void SaveMeasurementsToDisk()
        {
            try
            {
                AssignXElements();

                _xElmStation.Save(StationsCollection[IndexOfStationSelceted].FilePath);
            }
            catch (Exception e)
            {
                LogError(e);
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }
        }


        /// <summary>
        /// Making sure _xElmStation and _xElmMeasurements are inititalized.
        /// </summary>


        private void BLE_OnDataValueChanged(BluetoothControlsViewModel arg1, double? val, Unit unit)
        {
            if (!val.HasValue)
                return;

            var decimals = BusinessLogic.Settings.Settings.Instance.LengthCMDecimals;

            double value = -1;

            //Always perform below checks in CM
            if (unit != Unit.CM)
                value = val.Value.ConvertToUnit(unit, Unit.CM).Round(decimals);
            else
                value = val.Value.Round(decimals);
            

            if (value == AppSettings.ValueForDeletingLastEntry.Round(decimals))
            {
                if (Measurements != null && Measurements.Count != 0)
                {
                    new Action(() =>
                    {
                        int index = Measurements.IndexOf(Measurements[Measurements.Count - 1]);
                        if (index != -1)
                        { 
                            DeleteMesurment(Measurements[index]);

                            if (AppSettings.FrequencySettingsStatusForDelete == true)
                                Settings.FrequencySettingsViewModel.Play(AppSettings.FrequencySettingForDeleteLenght, AppSettings.FrequencySettingRepeatForDeleteLenght);
                        }
                        else
                        {
                            DispatchMessageBox("Der er ingen målinger at slette");
                        }
                    }).Dispatch();
                }
            }

            else if (value == AppSettings.ValueForGoingToNextStation.Round(decimals))
            {
                new Action(() =>
                {
                    if (AppSettings.FrequencySettingsStatusForNextStation == true)
                        Settings.FrequencySettingsViewModel.Play(AppSettings.FrequencySettingForNextStation, AppSettings.FrequencySettingRepeatForNextStation);

                    NextStation();
                }).Dispatch();
            }

            else if (value == AppSettings.ValueForGoingToNextOrder.Round(decimals))
            {
                new Action(() =>
                {
                    int index = Orders.IndexOf(SelectedOrder);
                    var odr = GetNextOrder();
                    if (odr != null)
                        SelectedOrder = odr;

                    if (AppSettings.FrequencySettingsStatusForNextOrder == true)
                        Settings.FrequencySettingsViewModel.Play(AppSettings.FrequencySettingForNextOrder, AppSettings.FrequencySettingRepeatForNextOrder);
                }).Dispatch();
            }

            else
            {
                //If length exist, add it.
                if (val.HasValue)
                {
                    if (SelectedOrder == null || SelectedOrder.Lookups.Count == 0)
                    {
                        DispatchMessageBox("Tilføj venligst en eller flere længdefordelinger at knytte målingerne til, inden de foretages.");
                        return;
                    }

                    AddMeasurement(val.Value, unit);

                    if (AppSettings.FrequencySettingsStatusForNewLenght == true)
                        Settings.FrequencySettingsViewModel.Play(AppSettings.FrequencySettingForNewLenghtAdded, AppSettings.FrequencySettingRepeatForNewLenghtAdded);
                }
            }
        }

        private void AssignXElements()
        {
            if (_xElmStation == null)
            {
                var curStation = StationsCollection[IndexOfStationSelceted];
                _xElmStation = curStation.GetStationXml();
                _xElmMeasurements = curStation.GetMeasurementsXml(_xElmStation);
            }
        }

        /// <summary>
        /// Add a measurement to the list in memory and to XML. (This method does not save to disk).
        /// </summary>
        private void AddMeasurement(double length, Unit unit)
        {
            new Action(() =>
            {
                try
                {
                    //Always store lengths in MM
                    var lic = MeasurementsClass.Create(DateTime.UtcNow, length.ConvertToUnit(unit, Unit.MM), SelectedOrder, Unit.MM);
                    Measurements.Add(lic);
                    PushUIMessage($"LengthListEnd_{Measurements.Count}");
                    AssignXElements();

                    _xElmMeasurements.Add
                   (
                     lic.ToXElement()
                   );

                    SaveMeasurements();

                    RaiseMeasurementsChanged();
                }
                catch(Exception e)
                {
                    LogError(e);
                }
            }).Dispatch();
        }


        private void SaveMeasurements()
        {
            _xElmStation.Save($"{StationsCollection[IndexOfStationSelceted].FilePath}_[Temp].xml");
            System.Threading.Thread.Sleep(250);
            if (File.Exists($"{StationsCollection[IndexOfStationSelceted].FilePath}_[Temp].xml"))
            {
                File.Delete(StationsCollection[IndexOfStationSelceted].FilePath);
                File.Copy($"{StationsCollection[IndexOfStationSelceted].FilePath}_[Temp].xml", StationsCollection[IndexOfStationSelceted].FilePath);
                File.Delete($"{StationsCollection[IndexOfStationSelceted].FilePath}_[Temp].xml");
            }
        }


        #region Up Arrow Command 


        public DelegateCommand ArrowUpCommand
        {
            get { return _cmdArrowUp ?? (_cmdArrowUp = new DelegateCommand(MoveToPreviousOrder)); }
        }


        private OrderClass GetPrevOrder()
        {
            if (Orders.Count <= 1)
                return null;

            int index = Orders.IndexOf(SelectedOrder);
            var i = (index - 1).Mod(Orders.Count);
            var order = Orders[i];

            return order;
        }



        public void MoveToPreviousOrder()
        {
            int index = Orders.IndexOf(SelectedOrder);
            var odr = GetPrevOrder();
            if (odr != null)
                SelectedOrder = odr;
        }


        #endregion


        #region Down Arrow Command 


        public DelegateCommand ArrowDownCommand
        {
            get { return _cmdArrowDown ?? (_cmdArrowDown = new DelegateCommand(MoveToNextOrder)); }
        }


        private OrderClass GetNextOrder()
        {
            if (Orders.Count <= 1)
                return null;

            int index = Orders.IndexOf(SelectedOrder);
            var i = (index + 1).Mod(Orders.Count);
            var order = Orders[i];

            return order;
        }

        public void MoveToNextOrder()
        {
            int index = Orders.IndexOf(SelectedOrder);
            var odr = GetNextOrder();
            if (odr != null)
                SelectedOrder = odr;
        }


        #endregion


        #region Save and go to next station Command


        public DelegateCommand NextStationCommand
        {
            get { return _cmdNextStation ?? (_cmdNextStation = new DelegateCommand(() => NextStation())); }
        }

        private bool NextStation()
        {
            if (AppRegionManager.ShowMessageBox($"Er du sikker på du vil gå til næste station {NextStationOnList}?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
            {
                return false;
            }

            if (IndexOfStationSelceted - 1 < 0)
                return false;

            Copy.SelectedStation = StationsCollection[IndexOfStationSelceted - 1];
            var nextStationTogoTo = new LenghtViewModel(Copy.SelectedTrip, Copy.SelectedStation, Copy);
            nextStationTogoTo.InitializeAsync();
            
            AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, nextStationTogoTo);

            Dispose();
            return true;
        }


        #endregion


        #region Cancel and return to overview Command


        public DelegateCommand CancelAndReturnCommand
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(BackToOverview)); }
        }


        private void BackToOverview()
        {
            if (Copy.SelectedStation.StationNumber != Station)
            {
                Copy.SelectedStation = StationsCollection[IndexOfStationSelceted];
            } 
            
            AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, Copy);
        }


        #endregion


        #region Open Order Window Command


        public DelegateCommand OpenOrderWindowCommand
        {
            get { return _cmdOrderView ?? (_cmdOrderView = new DelegateCommand(ShowOrderView)); }
        }


        private void ShowOrderView()
        {
            var vmOrders = new OrderViewModel();
            vmOrders.InitializeAsync();
            AppRegionManager.LoadWindowViewFromViewModel(vmOrders, true, "WindowToolBox");

            if (!vmOrders.IsDirty)
                return;

            OrderClass newSel = null;

            //If new list have the same selected order, make sure that is selected again (otherwise select the first group in the list.
            if (SelectedOrder != null)
                newSel = vmOrders.OrdersList.Where(x => x.GroupString == SelectedOrder.GroupString).FirstOrDefault();
            
            //Asign new/edited groups list
            Orders = new ObservableCollection<OrderClass>(vmOrders.OrdersList);

            if (newSel == null && Orders.Count > 0)
                newSel = Orders.FirstOrDefault();

            //Assign selected order, if one
            SelectedOrder = newSel;
        }


        #endregion


        #region Delete Row Command

        public DelegateCommand<MeasurementsClass> DeleteRowCommand
        {
            get { return _cmdDeleteRow ?? (_cmdDeleteRow = new DelegateCommand<MeasurementsClass>(M => CheckIfUserWantsToDelete(M))); }
        }
        private void CheckIfUserWantsToDelete(MeasurementsClass m)
        {
            if (m == null)
                return;

            if (AppRegionManager.ShowMessageBox(string.Format("Er du sikker på du vil slette måling '{0} cm' oprettet {1}", m.Length, m.DateTimeLocal), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;
            DeleteMesurment(m);
        }

        private void DeleteMesurment(MeasurementsClass m)
        {
            try
            {
                Measurements.Remove(m);
                var Load = XElement.Load(StationsCollection[IndexOfStationSelceted].FilePath);
                var xMesurements = Load.Element("Measurements");
                var xTheMeasurements = xMesurements.Elements();
                string valuestring = Convert.ToString(m.Length);
                if (valuestring.Contains(','))
                    valuestring = valuestring.Replace(',', '.');

                var Nodes = xMesurements.Elements().ToList();
                Nodes = Nodes.Where(V => V.Value == valuestring).ToList();
                if (Nodes.Count > 1)
                {
                    foreach (var aNode in Nodes)
                    {
                        var xaTime = aNode.Attribute("dateTimeUTC");
                        DateTime time = DateTime.Parse(xaTime.Value);
                        if (m.DateTimeUTC == time)
                        {
                            aNode.Remove();
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var aNode in xMesurements.Elements().ToList())
                    {
                        if (aNode == Nodes[0])
                        {
                            aNode.Remove();
                            break;
                        }
                    }
                }
                Load.Save(StationsCollection[IndexOfStationSelceted].FilePath);
                _xElmStation = XElement.Load(StationsCollection[IndexOfStationSelceted].FilePath);
                _xElmMeasurements = _xElmStation.Element("Measurements");
                RaiseMeasurementsChanged();
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }

        #endregion


        public DelegateCommand DeleteMeasurementsForActiveOrderCommand
        {
            get { return _cmdDeleteMeasurementsForActiveOrder ?? (_cmdDeleteMeasurementsForActiveOrder = new DelegateCommand(DeleteMeasurementsForActiveOrder)); }
        }


        private void DeleteMeasurementsForActiveOrder()
        {
            try
            {
                if (SelectedOrder == null)
                {
                    AppRegionManager.ShowMessageBox("Der blev ikke fundet nogen længdefordeling at slette målingerne for.");
                    return;
                }

                var gs = SelectedOrder.GroupString;

                if (gs == null)
                    return;

                var lst = Measurements.Where(x => x.Lookups.GroupString.Equals(gs, StringComparison.InvariantCultureIgnoreCase)).ToList();

                if (lst.Count == 0)
                {
                    AppRegionManager.ShowMessageBox("Der blev ikke fundet nogen målinger for valgte længdefordeling.");
                    return;
                }

                if (AppRegionManager.ShowMessageBox(string.Format("Er du sikker på du vil slette alle målinger (antal: {0}) for valgte længdefordeling ({1})?", lst.Count, gs), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                    return;

                AssignXElements();

                foreach (var itm in lst)
                {
                    var timestamp = itm.DateTimeUTC.ToString("dd-MM-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);

                    var xeM = (from xeMeasurement in _xElmMeasurements.Elements()
                               let xa = xeMeasurement.Attribute("dateTimeUTC")
                               where xa != null && xa.Value != null && xa.Value.Equals(timestamp)
                               select xeMeasurement).FirstOrDefault();

                    if (xeM != null)
                    {
                        xeM.Remove();

                        Measurements.Remove(itm);
                    }
                }

                SaveMeasurements();
            }
            catch(Exception e)
            {
                LogError(e);
                AppRegionManager.ShowMessageBox("En uventet fejl opstod. " + (e.Message ?? ""));
            }
        }



        public override void Dispose()
        {
            base.Dispose();

            //Make sure to deregister any global events, once form closes.
            Main.Menu.BCWLE.OnDataValueChanged -= BLE_OnDataValueChanged;
        }
    }
}
