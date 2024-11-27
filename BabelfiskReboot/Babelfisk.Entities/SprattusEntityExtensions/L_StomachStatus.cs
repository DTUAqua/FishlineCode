using Babelfisk.Entities.Sprattus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_StomachStatus : ILookupEntity, IComparable
    {
        public string Id
        {
            get { return this.L_StomachStatusId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.stomachStatus, this.description);
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

                if(num.HasValue)
                    str = num + " - ";

                str += this.stomachStatus;

                //if(!string.IsNullOrWhiteSpace(description))
                //    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_StomachStatus other = obj as L_StomachStatus;

            if(other == null)
                return false;

            return this.L_StomachStatusId == other.L_StomachStatusId &&
                (stomachStatus == null ? other.stomachStatus == null : this.stomachStatus.Equals(other.stomachStatus)) &&
                (this.description == null ? other.description == null : this.description.Equals(other.description))
                ;
        }


        public int CompareTo(object obj)
        {
            L_StomachStatus other = obj as L_StomachStatus;

            if(other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if(string.IsNullOrWhiteSpace(stomachStatus))
                return "Angiv venligst kode.";

            if(stomachStatus.Length > 20)
                return "Koden må kun bestå af 20 tegn.";

            if(description != null && description.Length > 500)
                return "Beskrivelse må kun bestå af 500 tegn.";

            if(lst.OfType<L_StomachStatus>().Where(x => (x.L_StomachStatusId != L_StomachStatusId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.stomachStatus.Equals(stomachStatus, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", stomachStatus);

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
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.stomachStatus }; }
        }

        public void NewLookupCreated()
        {
        }
    }
}
