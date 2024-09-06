using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Infrastructure.Bluetooth
{
    public class BluetoothInformationClass
    {
        public string Name { get; set; }


        public ulong BluetoothAddress { get; set; }

        public string BluetoothAddressString { get { return BluetoothAddress.ToString(CultureInfo.InvariantCulture); } }

        public string BluetoothAddressHex {  get { return BluetoothAddress.ToString("X", CultureInfo.InvariantCulture); } }

        public int SignalStrength { get; set; }
    }
}
