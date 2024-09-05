using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_Parasite : ILookupEntity, IComparable
    {
        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.L_parasiteId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.num, this.description);
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
                string str = num.ToString();

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_Parasite other = obj as L_Parasite;

            if (other == null)
                return false;

            return this.L_parasiteId == other.L_parasiteId &&
                   this.num.Equals(other.num) &&
                   (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            L_Parasite other = obj as L_Parasite;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (description != null && description.Length > 100)
                return "Beskrivelse må kun bestå af 100 tegn.";

            if (lst.OfType<L_Parasite>().Where(x => (x.L_parasiteId != L_parasiteId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.num.Equals(num)).Count() > 0)
                return string.Format("En række med nummer '{0}' eksisterer allerede.", num);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
       /* public string CompareValue
        {
            get { return UIDisplay; }
        }*/

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            //MDU: 05-10-2018. Removed description, so FilteredCombobox only filters on num for L_Parasite.
            get { return new string[] { this.num.ToString()/*, description*/ }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
