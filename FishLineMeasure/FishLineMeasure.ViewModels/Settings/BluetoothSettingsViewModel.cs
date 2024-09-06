using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Settings
{
    public class BluetoothSettingsViewModel : AViewModel
    {
        private int _bluetoothSearchSeconds;

        private bool _bluetoothAutoConnect;

        private SettingsViewModel _vmParent;


        #region Properties


        public int BluetoothSearchTimeoutSeconds
        {
            get { return _bluetoothSearchSeconds; }
            set
            {
                if (_bluetoothSearchSeconds != value)
                {
                    _bluetoothSearchSeconds = value;
                    IsDirty = true;
                }

                RaisePropertyChanged(nameof(BluetoothSearchTimeoutSeconds));
            }
        }

        public bool BluetoothAutoConnect
        {
            get { return _bluetoothAutoConnect; }
            set
            {
                if (_bluetoothAutoConnect != value)
                {
                    _bluetoothAutoConnect = value;
                    IsDirty = true;
                }

                RaisePropertyChanged(nameof(BluetoothAutoConnect));
            }
        }

        



        #endregion


        public BluetoothSettingsViewModel(SettingsViewModel vmParent)
        {
            _vmParent = vmParent;
            BluetoothSearchTimeoutSeconds = AppSettings.BluetoothSearchTimeoutSeconds;
            BluetoothAutoConnect = AppSettings.BluetoothAutoConnect;
            IsDirty = false;
        }



        public void Persist()
        {
            AppSettings.BluetoothSearchTimeoutSeconds = BluetoothSearchTimeoutSeconds;
            AppSettings.BluetoothAutoConnect = BluetoothAutoConnect;

            IsDirty = false;
        }
    }
}
