using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
     [Serializable]
    public partial class DFUPerson : OfflineEntity, ILookupEntity
    {
         /// <summary>
         /// dfuPersonId is used as reference to other tables
         /// </summary>
         public string Id
         {
             get { return this.dfuPersonId.ToString(); }
         }

        private static List<L_DFUDepartment> _lstDFUDepartments;


        public static List<L_DFUDepartment> DFUDepartments
        {
            get { return _lstDFUDepartments; }
            set { _lstDFUDepartments = value; }
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return this.initials; /*UIDisplay;*/ }
        }


        public string FilterValue
        {
            get { return String.Format("{0} {1} {2}", this.initials, this.name, this.L_DFUDepartment != null ? L_DFUDepartment.dfuDepartment : ""); }
        }

        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string UIDisplay
        {
            get
            {
                string str = this.initials;
                if (!string.IsNullOrWhiteSpace(name))
                    str += " - " + name;

                return str;
            }
        }


         /// <summary>
         /// All properties below are required in the equals method (at least when an existing person is submitted to database, having
         /// all the properties below ensures that the duplicate key error does not occur).
         /// </summary>
        public override bool Equals(object obj)
        {
            DFUPerson other = obj as DFUPerson;

            if (other == null)
                return false;

            return this.dfuPersonId.Equals(dfuPersonId) && 
                (this.initials == null ? other.initials == null : this.initials.Equals(other.initials)) && 
                (this.name == null ? other.name == null :  this.name.Equals(other.name)) && 
                (this.dfuDepartment == null ? other.dfuDepartment == null : this.dfuDepartment.Equals(other.dfuDepartment));
        }


        /*
        public override int GetHashCode()
        {
            return this.dfuPersonId.GetHashCode() ^
                  (initials == null ? 0 : initials.GetHashCode()) ^
                  (name == null ? 0 : name.GetHashCode()) ^
                  (dfuDepartment == null ? 0 : dfuDepartment.GetHashCode())
                  ;
        }
        */
        
        public int CompareTo(object obj)
        {
            DFUPerson other = obj as DFUPerson;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }

        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(initials))
                return "Angiv venligst initialer.";

            if (initials.Length > 10)
                return "Initialer må kun bestå af 10 tegn.";

            if (string.IsNullOrWhiteSpace(name))
                return "angiv venligst navn.";

            if (name.Length > 80)
                return "Fulde navn må kun bestå af 80 tegn.";

            if (lst.OfType<DFUPerson>().Where(x => (x.dfuPersonId != dfuPersonId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.initials.Equals(initials, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En person med initialer '{0}' eksisterer allerede.", dfuDepartment);

            return null;
        }


        public void NewLookupCreated()
        {
        }
    }
}
