using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class R_GearTypeSelectionDevice : ILookupEntity, IComparable
    {
        private static List<L_SelectionDevice> _lstSelectionDevices;
        private static List<L_GearType> _lstGearTypes;

        private static Dictionary<string, L_SelectionDevice> _dicSelectionDevices = null;
        private static Dictionary<string, L_GearType> _dicGearTypes = null;

        public static List<L_SelectionDevice> SelectionDevices
        {
            get { return _lstSelectionDevices; }
            set
            {
                _lstSelectionDevices = value;
                if (value != null)
                    _dicSelectionDevices = value.ToDictionary(x => x.selectionDevice);
            }
        }

        public static List<L_GearType> GearTypes
        {
            get { return _lstGearTypes; }
            set
            {
                _lstGearTypes = value;
                if (value != null)
                    _dicGearTypes = value.ToDictionary(x => x.gearType);
            }
        }


        public string L_SelectionDeviceUIDisplay
        {
            get
            {
                if (_dicSelectionDevices == null || !_dicSelectionDevices.ContainsKey(this.selectionDevice))
                    return "";

                return _dicSelectionDevices[this.selectionDevice].selectionDevice;
            }
        }


        public string L_GearTypeUIDisplay
        {
            get
            {
                if (gearType == null || _dicGearTypes == null || !_dicGearTypes.ContainsKey(this.gearType))
                    return "";

                return _dicGearTypes[this.gearType].UIDisplay;
            }
        }


        /// <summary>
        /// gearId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.R_GearTypeSelectionDeviceId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", L_SelectionDeviceUIDisplay, L_GearTypeUIDisplay);
            }
        }


        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        /// <summary>
        /// Used for display in drop down list for example
        /// </summary>
        public string UIDisplay
        {
            get
            {
                string str = FilterValue;
                return str;
            }
        }


        public override bool Equals(object obj)
        {
            R_GearTypeSelectionDevice other = obj as R_GearTypeSelectionDevice;

            if (other == null)
                return false;

            return this.R_GearTypeSelectionDeviceId == other.R_GearTypeSelectionDeviceId &&
                        (selectionDevice.Equals(other.selectionDevice)) &&
                        (gearType == null ? other.gearType == null : gearType.Equals(other.gearType))
                ;
        }


        public int CompareTo(object obj)
        {
            R_GearTypeSelectionDevice other = obj as R_GearTypeSelectionDevice;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {

        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(gearType))
                return "Angiv venligst redskabstype.";

            if (string.IsNullOrWhiteSpace(selectionDevice))
                return "Angiv venligst selektionsudstyr.";

            if (lst.OfType<R_GearTypeSelectionDevice>().Where(x => (x.R_GearTypeSelectionDeviceId != R_GearTypeSelectionDeviceId || (this.ChangeTracker.State == ObjectState.Added && x != this)) &&
                                                    (x.selectionDevice != null && x.selectionDevice.Equals(selectionDevice)) &&
                                                    (x.gearType != null && x.gearType.Equals(gearType)))
                                                    .Count() > 0)
                return string.Format("En række eksisterer allerede for selektionsudstyr '{0}' og redskabstype '{1}'.", L_SelectionDeviceUIDisplay, L_GearTypeUIDisplay);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return UIDisplay; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
