using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_HatchMonthReadability : ILookupEntity, IComparable
    {
        /// <summary>
        /// speciesCode code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.L_HatchMonthReadabilityId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.hatchMonthRemark, this.description);
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
                string str = "";

                if (num.HasValue)
                    str = num + " - ";

                str += this.hatchMonthRemark;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_HatchMonthReadability other = obj as L_HatchMonthReadability;

            if (other == null)
                return false;

            return this.L_HatchMonthReadabilityId == other.L_HatchMonthReadabilityId &&
                (this.hatchMonthRemark == null ? other.hatchMonthRemark == null : this.hatchMonthRemark.Equals(other.hatchMonthRemark)) &&
                (this.description == null ? other.description == null : this.description.Equals(other.description))
                ;
        }


        public int CompareTo(object obj)
        {
            L_HatchMonthReadability other = obj as L_HatchMonthReadability;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(hatchMonthRemark))
                return "Angiv venligst kode.";

            if (hatchMonthRemark.Length > 10)
                return "Koden må kun bestå af 10 tegn.";

            if (description != null && description.Length > 250)
                return "Beskrivelse må kun bestå af 250 tegn.";

            if (lst.OfType<L_HatchMonthReadability>().Where(x => (x.L_HatchMonthReadabilityId != L_HatchMonthReadabilityId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.hatchMonthRemark.Equals(hatchMonthRemark, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", hatchMonthRemark);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        /* public string CompareValue
         {
             get { return String.Format("{0} {1}", this.num, this.otolithReadingRemark); }
         }*/


        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.hatchMonthRemark }; }
        }

        public void NewLookupCreated()
        {
        }
    }
}
