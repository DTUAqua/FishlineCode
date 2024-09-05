using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_GearQuality : ILookupEntity, IComparable
    {
        /// <summary>
        /// gearQuality code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.gearQuality; }
        }

        public string FilterValue
        {
            get { return String.Format("{0} {1} {2}", this.num, this.gearQuality, this.description); }
        }


        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string UIDisplay
        {
            get 
            {
                string str = "";

                if (num.HasValue)
                    str = num + " - ";

                str += this.gearQuality;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }

        
        public override bool Equals(object obj)
        {
            L_GearQuality other = obj as L_GearQuality;

            if (other == null)
                return false;

            return this.L_gearQualityId == other.L_gearQualityId;
        }
        
        /*
        public override int GetHashCode()
        {
            return L_gearQualityId.GetHashCode() ^
                   (gearQuality == null ? 0 : gearQuality.GetHashCode()) ^
                   (description == null ? 0 : description.GetHashCode())
                    ;
        }*/
        

        public int CompareTo(object obj)
        {
            L_GearQuality other = obj as L_GearQuality;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(gearQuality))
                return "Angiv venligst kode.";

            if (gearQuality.Length > 1)
                return "Koden må kun bestå af 1 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Angiv venligst bemærkninger.";

            if (description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_GearQuality>().Where(x => (x.L_gearQualityId != L_gearQualityId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.gearQuality.Equals(gearQuality, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Redskabskvalitet med kode '{0}' eksisterer allerede.", gearQuality);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.gearQuality); }
        }

        public void NewLookupCreated()
        {
        }
    }
}
