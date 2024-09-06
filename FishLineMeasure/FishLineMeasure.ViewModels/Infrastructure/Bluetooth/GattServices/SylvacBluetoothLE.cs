namespace FishLineMeasure.ViewModels.Infrastructure.Bluetooth.GattServices
{
    class SylvacBluetoothLE
    {
        public static string SERVICE_A = "00001800-0000-1000-8000-00805f9b34fb";
        public static string SERVICE_B = "00001801-0000-1000-8000-00805f9b34fb";
        public static string METROLOGY = "c1b25000-caaf-6d0e-4c33-7dae30052840";

        public static string CHARACTERISTIC_M = "00002a00-0000-1000-8000-00805f9b34fb";
        public static string CHARACTERISTIC_N = "00002a01-0000-1000-8000-00805f9b34fb";
        public static string CHARACTERISTIC_O = "00002a04-0000-1000-8000-00805f9b34fb";
        public static string CLIENT_CHARACTERISTIC_CONFIG = "00002902-0000-1000-8000-00805f9b34fb";

        public static string DATA_RECEIVED = "c1b25010-caaf-6d0e-4c33-7dae30052840";
        public static string NOT_USED = "c1b25011-caaf-6d0e-4c33-7dae30052840";
        public static string DATA_REQUEST_OR_COMMAND = "c1b25012-caaf-6d0e-4c33-7dae30052840";
        public static string ANSWER_TO_REQUEST_OR_COMMAND = "c1b25013-caaf-6d0e-4c33-7dae30052840";
    }
}
