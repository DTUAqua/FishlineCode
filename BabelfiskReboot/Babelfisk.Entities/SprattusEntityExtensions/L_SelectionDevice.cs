using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SelectionDevice : ILookupEntity, IComparable
    {
        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.selectionDevice.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.selectionDevice, this.description);
            }
        }


        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string UIDisplay
        {
            get
            {
                string str = this.selectionDevice;
                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_SelectionDevice other = obj as L_SelectionDevice;

            if (other == null)
                return false;

            return this.L_selectionDeviceId == other.L_selectionDeviceId &&
                (this.selectionDevice != null ? this.selectionDevice.Equals(other.selectionDevice) : other.selectionDevice == null) &&
                (description != null ? description.Equals(other.description) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            L_SelectionDevice other = obj as L_SelectionDevice;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(selectionDevice))
                return "Angiv venligst kode.";

            if (selectionDevice.Length > 30)
                return "Koden må kun bestå af 30 tegn.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 50 tegn.";

            if (lst.OfType<L_SelectionDevice>().Where(x => (x.L_selectionDeviceId != L_selectionDeviceId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.selectionDevice.Equals(selectionDevice, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Et selektionsudstyr med kode '{0}' eksisterer allerede.", selectionDevice);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.selectionDevice); }
        }


        public void NewLookupCreated()
        {
        }
    }
}
