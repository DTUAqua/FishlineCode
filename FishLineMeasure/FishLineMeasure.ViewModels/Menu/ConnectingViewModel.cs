using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Menu
{
   public class ConnectingViewModel : AViewModel
    {
        private bool _connectingDone;
        private string _BluetoothDeviceName;
        public bool ConnectingDone
        {
            get { return _connectingDone; }
            set
            {
                _connectingDone = value;
                RaisePropertyChanged(nameof(ConnectingDone));
                ConntingDoneChanged();
            }
        }

        private void ConntingDoneChanged()
        {
            if (_connectingDone == true)
                this.Close();

        }

        public string BluetoothDeviceName
        {
            get { return _BluetoothDeviceName; }
            set
            {
                _BluetoothDeviceName = value;
                RaisePropertyChanged(nameof(BluetoothDeviceName));
            }
        }

        public ConnectingViewModel()
        {
            WindowHeight = 150;
            WindowWidth = 450;
            MinWindowHeight = 150;
            MinWindowWidth = 450;
            WindowTitle = "Forbinder";
        }
    }
}
