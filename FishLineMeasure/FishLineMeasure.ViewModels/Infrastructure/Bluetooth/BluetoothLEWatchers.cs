using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;
using Microsoft.Practices.Prism.ViewModel;
using System.Threading;
using Anchor.Core;

namespace FishLineMeasure.ViewModels.Infrastructure.Bluetooth
{
    public class BluetoothLEWatchers : AViewModel
    {
        public event Action<BluetoothLEWatchers, BluetoothInformationClass> OnBluetoothDeviceFound;
        public event Action<BluetoothLEWatchers, bool> OnFirstSearchChangePropertyValue;
        private BluetoothLEAdvertisementWatcher watcher;
 
        private AutoResetEvent _waitHandler = new AutoResetEvent(false);

        private object _searchingLock = new object();

        private bool _watcherRun; 
       
        public bool IsSearching
        {
            get
            {
                return _watcherRun;
            }
            set
            {
                _watcherRun = value;
                RaisePropertyChanged(nameof(IsSearching));
            }
        }

        private bool _watcherStopped;

        public bool watcherStopped
        {
            get
            {
                return _watcherStopped;
            }
            set
            {
                _watcherStopped = value;
                RaisePropertyChanged(nameof(watcherStopped));
            }
        }

        private string _stopBluetoothAddress = null;

        public Task Search(string stopWhenFoundBluetoothAddress = null, bool suppresIsSeachingUI = false)
        {
            bool searching = false;

            lock (_searchingLock)
            {
                searching = IsSearching;

                if (searching)
                    return Task.FromResult(true);

                if (suppresIsSeachingUI)
                    _watcherRun = true;
                else
                    IsSearching = true;
            }

            _stopBluetoothAddress = stopWhenFoundBluetoothAddress;

            _waitHandler.Reset();

            return Task.Run(() =>
            {
                startWatcher();

                _waitHandler.WaitOne(AppSettings.BluetoothSearchTimeoutSeconds*1000);

                watcher.Stop();

                _stopBluetoothAddress = null;

                //Set WatcherRun property on UI thread
                new Action(() =>
                {
                    lock (_searchingLock)
                    {
                        if (suppresIsSeachingUI)
                            _watcherRun = false;
                        else
                            IsSearching = false;
                    }

                    if (OnFirstSearchChangePropertyValue != null)
                        OnFirstSearchChangePropertyValue(this, true);
                }).DispatchInvoke();
            });
        }


        public void StopSearch()
        {
            _waitHandler.Set();
        }
        

        public BluetoothLEWatchers()
        {

        }

        public void startWatcher()
        {
            if (watcher == null)
            {
                watcher = new BluetoothLEAdvertisementWatcher();

                watcher.ScanningMode = BluetoothLEScanningMode.Active;

                // Only activate the watcher when we're recieving values >= -80
                //  watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;

                // Stop watching if the value drops below -90 (user walked away)
                //  watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;

                // Register callback for when we see an advertisements
                watcher.Received += OnAdvertisementReceived;
                watcher.Stopped += Watcher_Stopped;

                // Wait 5 seconds to make sure the device is really out of range
                watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
                watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);
            }

            // Starting watching for advertisements
            watcher.Start();
        }

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            if (!string.IsNullOrEmpty(eventArgs.Advertisement.LocalName))
            {
                var btc = new BluetoothInformationClass
                {
                    Name = eventArgs.Advertisement.LocalName,
                    BluetoothAddress = eventArgs.BluetoothAddress,
                    SignalStrength = eventArgs.RawSignalStrengthInDBm
                };

                if (OnBluetoothDeviceFound != null)
                    OnBluetoothDeviceFound(this, btc);

                var sb = _stopBluetoothAddress;

                if (!string.IsNullOrWhiteSpace(sb) && (btc.BluetoothAddressString ?? "").Equals(sb, StringComparison.InvariantCultureIgnoreCase))
                    StopSearch();
            }
        }

        private void Watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            watcherStopped = true;
        }

    }
}
