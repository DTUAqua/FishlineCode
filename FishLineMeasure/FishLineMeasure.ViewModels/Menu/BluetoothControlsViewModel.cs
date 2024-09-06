using System;
using System.Linq;
using FishLineMeasure.ViewModels.Infrastructure.Bluetooth;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using FishLineMeasure.ViewModels.Infrastructure.Bluetooth.GattServices;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using FishLineMeasure.ViewModels;
using FishLineMeasure.BusinessLogic;

namespace FishLineMeasure.ViewModels.Menu
{
    public class BluetoothControlsViewModel : AViewModel 
    {
        private ObservableCollection<BluetoothInformationClass> _bluetoothList;

        private DelegateCommand _cmdRefresh;
        private DelegateCommand _cmdConnect;
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdDisconnect;
        private DelegateCommand _cmdHelp;

        public event Action<BluetoothControlsViewModel,double?,Unit> OnDataValueChanged;
        private BluetoothLEWatchers _ble;

        private GattDeviceService _GDS = null;
        private GattCharacteristic _GattCharacteristic = null;
        private GattCharacteristic _GGattCharacteristic = null;
        public GattCommunicationStatus _status;
        public ConnectingViewModel _CW;
        
        public BluetoothConnectionStatus _ConnectionStatus = BluetoothConnectionStatus.Disconnected;
        public BluetoothInformationClass _selectedBluetoothDevice;

        private bool _disconnectButtonEnabled = false;
        private double? _latestValue;
        private Unit? _latestUnit;
        private bool _OnProgramStarted;
        private bool _informedUserOfErrors;


        #region Properties


        public BluetoothLEWatchers BLE
        {
            get { return _ble; }
            set
            {
                _ble = value;
                RaisePropertyChanged(nameof(BLE));
            }
        }



        public ObservableCollection<BluetoothInformationClass> BluetoothList
        {
            get { return _bluetoothList; }
            set
            {
                _bluetoothList = value;
                RaisePropertyChanged(nameof(BluetoothList));
            }
        }


        public BluetoothConnectionStatus ConnectionStatus
        {
            get { return _ConnectionStatus; }
            set
            {
                _ConnectionStatus = value;
                RaisePropertyChanged(nameof(ConnectionStatus));
                RaisePropertyChanged(nameof(ConnectionStatusString));
                if (_ConnectionStatus == BluetoothConnectionStatus.Connected)
                    DisconnectButtonEnabled = true;
                else
                    DisconnectButtonEnabled = false;         
                RaisePropertyChanged(nameof(DisconnectButtonEnabled));
            }
        }


        public BluetoothInformationClass SelectedBluetoothDevice
        {
            get { return _selectedBluetoothDevice; }
            set
            {
                _selectedBluetoothDevice = value;
                RaisePropertyChanged(nameof(SelectedBluetoothDevice));
                RaisePropertyChanged(nameof(HasSelectedBluetoothDevice));

            }
        }


        public bool HasSelectedBluetoothDevice
        {
            get { return SelectedBluetoothDevice != null; }
        }


        public string ConnectionStatusString
        {
            get { return _ConnectionStatus.ToString(); }
        }
      

        public bool DisconnectButtonEnabled
        {
            get
            {
                return _disconnectButtonEnabled;
            }
            set
            {
                _disconnectButtonEnabled = value;
                RaisePropertyChanged(nameof(DisconnectButtonEnabled));
            }
        }

       
        public double? LatestValue
        {
            get { return _latestValue; }
            set
            {
                _latestValue = value;
                if (_latestValue == null)
                    _latestUnit = null;

                RaisePropertyChanged(nameof(LatestValue));
                RaisePropertyChanged(nameof(HasLatestValue));
                RaisePropertyChanged(nameof(LatestUnitString));
            }
        }


        public bool HasLatestValue
        {
            get { return _latestValue.HasValue; }
        }


        public string LatestUnitString
        {
            get { return _latestUnit.HasValue ? _latestUnit.Value.ToString().ToLower() : ""; }
        }


        public bool OnProgramStarted
        {
            get { return _OnProgramStarted; }
            set
            {
                _OnProgramStarted = value;
                RaisePropertyChanged(nameof(OnProgramStarted));
            }
        }


        public bool InformedUserOfErrors
        {
            get { return _informedUserOfErrors; }
            set
            {
                _informedUserOfErrors = value;
                RaisePropertyChanged(nameof(InformedUserOfErrors));
            }
        }

        #endregion

        public BluetoothControlsViewModel()
        {
            MinWindowHeight = 425;
            MinWindowWidth = 400;
            WindowHeight = 525;
            WindowWidth = 600;

            base.AdjustWindowWidthHeightToScreen();

            WindowTitle = "Bluetooth Forbindelse";
            InformedUserOfErrors = false;
            DisconnectButtonEnabled = false;
            OnProgramStarted = !string.IsNullOrWhiteSpace(AppSettings.BluetoothAddress); //Only try to reconnect to bluetooth device, if it was connected to one previously;
            BluetoothList = new ObservableCollection<BluetoothInformationClass>();
            BLE = new BluetoothLEWatchers();
            BLE.OnBluetoothDeviceFound += Ble_OnBluetoothDeviceFound;
            BLE.OnFirstSearchChangePropertyValue += BLE_OnFirstSearchCompleted;

        }

        private void BLE_OnFirstSearchCompleted(BluetoothLEWatchers arg1, bool arg2)
        {
            OnProgramStarted = false;
            BLE.OnFirstSearchChangePropertyValue -= BLE_OnFirstSearchCompleted;
        }

        #region Run Search again and Clear list Command
        public DelegateCommand RefreshBlueToothDeviceCommand
        {
            get { return _cmdRefresh ?? (_cmdRefresh = new DelegateCommand(refreshBTDevices)); }
        }

        private void refreshBTDevices()
        {
            BluetoothList.Clear();
            BLE.Search();
        }

        #endregion

        #region Close bluetooth Window Command
        public DelegateCommand CancelBluetoothCommand
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(CancelThis)); }
        }

        private void CancelThis()
        {
            this.Close();
        }

        #endregion

        #region Connnect to BLuetooth Device choosen 
        public DelegateCommand ConnectBLuetoothCommand
        {
            get { return _cmdConnect ?? (_cmdConnect = new DelegateCommand(ConnectBluetoothAsync)); }
        }

        private async void ConnectBluetoothAsync()
        {
            try
            {
                if (_ble.IsSearching)
                    _ble.StopSearch();

                if (SelectedBluetoothDevice != null)
                {
                    InformedUserOfErrors = false;
                    ulong bluetoothAC = SelectedBluetoothDevice.BluetoothAddress;
                    var dev = await BluetoothLEDevice.FromBluetoothAddressAsync(bluetoothAC);

                    if (dev != null)
                    {
                        var status = await ConnectAsync(dev);

                        if (status == ConnectionState.AlreadyConnected)
                            DispatchMessageBox("Du er allerede forbundet til denne enhed.", 5);
                    }
                }
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }

        private async Task<ConnectionState> ConnectAsync(BluetoothLEDevice dev, int retries = 3)
        {
            DisconnectFromDevice(false, false);

            if (_CW == null)
            {
                new Action(() =>
                {
                    _CW = new ConnectingViewModel();

                    Action<object, AViewModel> evt = null;
                    evt = (sender, e) =>
                    {
                        if(evt != null && _CW != null)
                            _CW.Closed -= evt;

                        _CW = null;
                    };

                    _CW.Closed += evt;
                    _CW.ConnectingDone = false;
                    _CW.BluetoothDeviceName = dev.Name;
                    AppRegionManager.LoadWindowViewFromViewModel(_CW, false, "WindowToolBox");
                }).DispatchInvoke();
            }

            try
            {
                var ddId = dev.BluetoothAddress.ToString();
                await _ble.Search(ddId, true);

                dev = await BluetoothLEDevice.FromBluetoothAddressAsync(dev.BluetoothAddress);

                if (dev == null)
                {
                    if (retries > 0)
                        return await ConnectAsync(dev, retries - 1);
                    else
                        DispatchMessageBox("Forbindelsen til enheden blev afbrudt, prøv venligst igen.");
                    return ConnectionState.Unknown;
                }


                if (dev.ConnectionStatus == BluetoothConnectionStatus.Connected && ConnectionStatus == BluetoothConnectionStatus.Connected && _GGattCharacteristic != null)
                {
                    if (ddId == AppSettings.BluetoothAddress)
                    {
                        return ConnectionState.AlreadyConnected;
                    }
                    else //if (AppRegionManager.ShowMessageBox($"Ønsker du at forbinde til {dev.Name}", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                    {
                        ClearBluetoothLEDevice(dev);
                        return await ConnectAsync(dev, retries - 1);
                    }
                }



                var dId = dev.DeviceInformation.Id;

                if (dev.DeviceInformation.Pairing.IsPaired == false)
                {
                    dev.DeviceInformation.Pairing.Custom.PairingRequested += Custom_PairingRequested;
                    DevicePairingResult prslt = null;
                    var task = dev.DeviceInformation.Pairing.Custom.PairAsync(DevicePairingKinds.ConfirmOnly, DevicePairingProtectionLevel.Default).AsTask();
                    if (await Task.WhenAny(task, Task.Delay(10000)) == task)
                    {
                        prslt = task.Result;
                    }
                    // prslt = await bDevice.DeviceInformation.Pairing.Custom.PairAsync(DevicePairingKinds.ConfirmOnly, DevicePairingProtectionLevel.Default);
                    dev.DeviceInformation.Pairing.Custom.PairingRequested -= Custom_PairingRequested;

                    if (prslt != null && prslt.Status != DevicePairingResultStatus.Paired)
                    {
                        DispatchMessageBox("Bluetooth-parringen med enheden fejlede. Prøv venligst at nulstille bluetooth på enheden og prøv igen.");
                        _CW.ConnectingDone = true;
                        return ConnectionState.Unknown;
                    }
                    else
                    {
                        // The pairing can take some time to complete. If you don't wait you may have issues. 5 seconds seems to do the trick.
                        await Task.Run(() => System.Threading.Thread.Sleep(5000)); //try 5 second lay.

                        if (dev != null)
                        {
                            dev.ConnectionStatusChanged -= BDevice_ConnectionStatusChanged;
                            dev.Dispose();
                        }
                        //Reload device so that the GATT services are there. This is why we wait.                     
                        dev = await BluetoothLEDevice.FromIdAsync(dId);
                    }
                }

                dev.ConnectionStatusChanged -= BDevice_ConnectionStatusChanged;
                dev.ConnectionStatusChanged += BDevice_ConnectionStatusChanged;

                _GDS = dev.GetGattService(Guid.Parse(SylvacBluetoothLE.METROLOGY));

                _GGattCharacteristic = _GDS.GetCharacteristics(Guid.Parse(SylvacBluetoothLE.ANSWER_TO_REQUEST_OR_COMMAND)).FirstOrDefault();
                var stats = await _GGattCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (stats == GattCommunicationStatus.Unreachable)
                {
                    await UnPair(dev, true);
                    ClearBluetoothLEDevice(dev);

                    if (retries <= 0)
                    {
                        if (_CW != null)
                            _CW.ConnectingDone = true;
                        DispatchMessageBox("Enheden kunne ikke nås, prøv venligst igen.");
                    }
                    else
                        return await ConnectAsync(dev, retries - 1);
                    return ConnectionState.Unknown;
                }

                _GattCharacteristic = _GDS.GetCharacteristics(Guid.Parse(SylvacBluetoothLE.DATA_RECEIVED)).FirstOrDefault();

                _status = await _GattCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);
                if (_status == GattCommunicationStatus.Success)
                    _GattCharacteristic.ValueChanged += C_ValueChanged;

                if (dev != null)
                    ConnectionStatus = dev.ConnectionStatus;

                if (SelectedBluetoothDevice != null)
                    AppSettings.BluetoothAddress = SelectedBluetoothDevice.BluetoothAddressString;

                this.Close();

                if (ConnectionStatus == BluetoothConnectionStatus.Connected && BLE.IsSearching)
                    BLE.StopSearch();
            }
            catch (Exception ex)
            {
                ClearBluetoothLEDevice(dev);
                ConnectionStatus = BluetoothConnectionStatus.Disconnected;
                if (_status == GattCommunicationStatus.Unreachable)
                {
                    if (_CW != null)
                        _CW.ConnectingDone = true;

                    DispatchMessageBox("Der opstod et problem med forbindelsen.");
                }
                else if (ex.Message.Contains("The attribute requires authentication before it can be read or written."))
                {
                    var info = dev;
                    await UnPair(dev, true);
                    return await ConnectAsync(info);
                }
                else if (ex.Message.Contains("8060005"))
                {
                    await UnPair(dev, true);
                    DispatchMessageBox("Genstart Programmet og nulstil bluetooth på måleenheden");
                }
                else if (ex.Message.Contains("Element not found"))
                {
                    if (retries <= 0)
                        DispatchMessageBox("Fejl 1. Det var ikke muligt at oprette forbindelse til enheden.\r\n" + "Prøv venligst at nulstille bluetooth på enheden og prøv igen.");
                    else
                    {
                        if(retries == 2)
                        {
                            await UnPair(dev, true);
                            await Task.Run(() => System.Threading.Thread.Sleep(3000));
                        }

                        return await ConnectAsync(dev, retries - 1);
                    }
                }
            }
            finally
            {
                if (_CW != null)
                    _CW.ConnectingDone = true;

                if (dev != null)
                    ConnectionStatus = dev.ConnectionStatus;
            }

            return ConnectionStatus == BluetoothConnectionStatus.Connected ? ConnectionState.Connected : ConnectionState.Unknown;
        } 
        
        private async Task UnPair(BluetoothLEDevice dev, bool suppressMessages = false)
        {
            try
            {
                if (dev != null)
                {
                    var tempb = dev;
                    var task = dev.DeviceInformation.Pairing.UnpairAsync().AsTask();

                    //Only use max 10 seconds to unpair.
                    if (await Task.WhenAny(task, Task.Delay(10000)) == task)
                    {
                        if(!suppressMessages)
                            DispatchMessageBox($"{tempb.Name} blev succesfuldt unpaired");
                       //Success
                    }
                    else
                    {
                        if(!suppressMessages)
                            DispatchMessageBox($"Unparing af {tempb.Name} fejlede.\r\n" + $"Genstart bluetooth på enheden og genstart programmet");
                    }
                }
            }
            catch { }
        }

        private void Custom_PairingRequested(DeviceInformationCustomPairing sender, DevicePairingRequestedEventArgs args)
        {
            args.Accept();
        }

        private async void BDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            if (sender != null)
            {
                new Action(() =>
                {
                    ConnectionStatus = sender.ConnectionStatus;
                }).Dispatch();
            }

            if(sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
            {
                var dev = await GetConnectedDevice();
                if(dev != null)
                    ClearBluetoothLEDevice(dev);
            }

            if (InformedUserOfErrors == false)
            {
                if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
                {
                    //DispatchMessageBox("Programmet mistede Forbindelse til enheden.");
                    //AppSettings.BluetoothAddress = null; //TODO
                    LatestValue = null;
                    InformedUserOfErrors = true;
                }
            }

        }

        private void ClearBluetoothLEDevice(BluetoothLEDevice dev)
        {
            try
            {
                if (dev != null)
                    dev.ConnectionStatusChanged -= BDevice_ConnectionStatusChanged;

                if (_GattCharacteristic != null)
                    _GattCharacteristic.ValueChanged -= C_ValueChanged;

    
                _GDS?.Dispose();
                dev?.Dispose();

                _GattCharacteristic = null;
                _GGattCharacteristic = null;
                LatestValue = null;
                GC.Collect();
                ConnectionStatus = BluetoothConnectionStatus.Disconnected;
                //AppSettings.BluetoothAddress = null; //TODO
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }

        private void C_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            try
            {
                var res = args.CharacteristicValue;

                var dataReader = DataReader.FromBuffer(res);
                var output = dataReader.ReadString(res.Length);
                _latestUnit = Unit.MM;
                LatestValue = output.ToDouble();
                if (OnDataValueChanged != null)
                {
                    OnDataValueChanged(this, _latestValue, _latestUnit.Value);
                }
            }
            catch(Exception e)
            {
                LogError(e);
            }

        }

        #endregion

        #region Disconnect From Bluetooth Device

        public DelegateCommand DisconnectCommand
        {
            get { return _cmdDisconnect ?? (_cmdDisconnect = new DelegateCommand(() => DisconnectFromDevice(true))); }
        }

        public async void DisconnectFromDevice(bool resetDefaultBluetoothAddress = false, bool showMessage = true)
        {
            var dev = await GetConnectedDevice();

            if(dev != null)
                ClearBluetoothLEDevice(dev);

            if(resetDefaultBluetoothAddress)
                AppSettings.BluetoothAddress = null;

            if(showMessage)
                DispatchMessageBox("Forbindelsen til enheden er afbrudt.", 3);
        }

        #endregion

        #region Show Help window Command

        public DelegateCommand ShowHelpWindowCommand
        {
            get { return _cmdHelp ?? (_cmdHelp = new DelegateCommand(ShowHelp)); }
        }

        private void ShowHelp()
        {
            var vmHelp = new BluetoothHelperViewModel();
            AppRegionManager.LoadWindowViewFromViewModel(vmHelp, true, "WindowToolBox");
        }

        #endregion

        private void Ble_OnBluetoothDeviceFound(BluetoothLEWatchers watchers, BluetoothInformationClass bc)
        {
            new Action(() =>
            {
                try
                {
                    var lst = BluetoothList.ToList();
                    var dev = lst.Where(x => x.BluetoothAddress == bc.BluetoothAddress).FirstOrDefault();

                    //IF not yet in list, add it
                    if (dev == null)
                        lst.Add(bc);
                    else //If already there, replace it to make sure everything is updated.
                    {
                        var index = lst.IndexOf(dev);
                        if (index >= 0)
                            lst[index] = bc;
                    }


                    var prevSelected = SelectedBluetoothDevice;

                    //Order by signal strength again.
                    BluetoothList = lst.OrderBy(x => x.SignalStrength).ToObservableCollection();

                    SelectedBluetoothDevice = prevSelected == null ? null : lst.Where(x => x.BluetoothAddress == prevSelected.BluetoothAddress).FirstOrDefault();

                    if (OnProgramStarted == true && AppSettings.BluetoothAutoConnect && !string.IsNullOrWhiteSpace(AppSettings.BluetoothAddress) && (AppSettings.BluetoothAddress ?? "").Equals(bc.BluetoothAddressString, StringComparison.InvariantCultureIgnoreCase))
                        ConnectFromAppSettings();
                }
                catch(Exception e)
                {
                    LogError(e);
                }
            }).Dispatch();
        }

        public Task Initialize()
        {
            return BLE.Search();
        }

        public async void ConnectFromAppSettings()
        {
            try
            {
                if (_ble.IsSearching)
                    _ble.StopSearch();

                foreach (var device in BluetoothList)
                {
                    if ((device.BluetoothAddressString ?? "").Equals(AppSettings.BluetoothAddress, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var bDevice = await GetConnectedDevice();
                        if(bDevice != null)
                          await ConnectAsync(bDevice);

                        break;
                    }
                }
            }
            catch(Exception e)
            {
                LogError(e);
                DispatchMessageBox("En uventet fejl opstod. " + (e.Message ?? ""));
            }
        }


        private async Task<BluetoothLEDevice> GetConnectedDevice()
        {
            if (string.IsNullOrWhiteSpace(AppSettings.BluetoothAddress))
                return null;

            try
            {
                ulong bluetoothAC = Convert.ToUInt64(AppSettings.BluetoothAddress);
                var dev = await BluetoothLEDevice.FromBluetoothAddressAsync(bluetoothAC);

                return dev;
            }
            catch(Exception e)
            {
                LogError(e);
            }

            return null;
        }
    }
}
