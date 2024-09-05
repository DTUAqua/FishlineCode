using Babelfisk.Entities.Sprattus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SelectionDeviceSource : ILookupEntity, IComparable
    {
        public string Id
        {
            get { return this.L_selectionDeviceSourceId.ToString(); }
        }

        public string FilterValue 
        {
            get
            {
                return String.Format("{0} {1}", this.selectionDeviceSource, this.description);
            }
        }

        public string DefaultSortValue
        {
            get { return FilterValue; }
        }

        public string UIDisplay
        {
            get { return String.Format("{0} - {1}", this.selectionDeviceSource, this.description); }
        }

        public override bool Equals(object obj)
        {
            L_SelectionDeviceSource other = obj as L_SelectionDeviceSource;

            if (other == null)
                return false;

            return this.L_selectionDeviceSourceId == other.L_selectionDeviceSourceId &&
                (this.selectionDeviceSource != null ? this.selectionDeviceSource.Equals(other.selectionDeviceSource, StringComparison.InvariantCultureIgnoreCase) : other.selectionDeviceSource == null) &&
                (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public void BeforeSave()
        {
            
        }

        public int CompareTo(object obj)
        {
            L_SelectionDeviceSource other = obj as L_SelectionDeviceSource;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(selectionDeviceSource))
                return "Angiv venligst kode.";

            if (selectionDeviceSource.Length > 2)
                return "Koden må kun bestå af 2 tegn.";

            if (description != null && description.Length > 500)
                return "Beskrivelse må kun bestå af 500 tegn.";

            if (lst.OfType<L_SelectionDeviceSource>().Where(x => (x.L_selectionDeviceSourceId != L_selectionDeviceSourceId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.selectionDeviceSource.Equals(selectionDeviceSource, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", selectionDeviceSource);

            return null;
        }

        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.selectionDeviceSource); }
        }

        public void NewLookupCreated()
        {
        }
    }
}
