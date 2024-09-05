using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_GearInfoType : ILookupEntity, IComparable
    {
        /// <summary>
        /// gearInfoType code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.gearInfoType; }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2} {3}", this.gearInfoType, this.description, dfuBase_dfuref, unit);
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
                string str = this.gearInfoType;
                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_GearInfoType other = obj as L_GearInfoType;

            if (other == null)
                return false;

            return this.L_gearInfoTypeId == other.L_gearInfoTypeId;
        }

        /*
        public override int GetHashCode()
        {
            return L_gearInfoTypeId.GetHashCode() ^
                  (gearInfoType == null ? 0 : gearInfoType.GetHashCode()) ^
                  (description == null ? 0 : description.GetHashCode());
        }
        */

        public int CompareTo(object obj)
        {
            L_GearInfoType other = obj as L_GearInfoType;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(gearInfoType))
                return "Angiv venligst kode.";

            if (gearInfoType.Length > 20)
                return "Koden må kun bestå af 20 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Angiv venligst bemærkninger.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (dfuBase_dfuref != null && dfuBase_dfuref.Length > 80)
                return "DFU Base ref. må kun bestå af 50 tegn.";

            if (unit != null && unit.Length > 50)
                return "Enhed må kun bestå af 50 tegn.";

            if (lst.OfType<L_GearInfoType>().Where(x => (x.L_gearInfoTypeId != L_gearInfoTypeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.gearInfoType.Equals(gearInfoType, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En redskabsinfotype med kode '{0}' eksisterer allerede.", gearInfoType);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.gearInfoType); }
        }

        public void NewLookupCreated()
        {
        }
    }
}
