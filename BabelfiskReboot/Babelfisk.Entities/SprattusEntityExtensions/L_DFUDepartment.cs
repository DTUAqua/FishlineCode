using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    [Serializable]
    public partial class L_DFUDepartment : ILookupEntity, IComparable
    {
        /// <summary>
        /// dfuDepartment code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.dfuDepartment; }
        }

        public string FilterValue
        {
            get { return String.Format("{0} {1}", this.dfuDepartment, this.description); } 
        }

        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string CodeDescription
        {
            get { return String.Format("{0} - {1}", this.dfuDepartment, this.description); }
        }


        public string UIDisplay
        {
            get { return CodeDescription; }
        }


        public override bool Equals(object obj)
        {
            L_DFUDepartment other = obj as L_DFUDepartment;

            if (other == null)
                return false;

            return this.L_dfuDepartmentId == other._l_dfuDepartmentId;
        }

        /*
        public override int GetHashCode()
        {
            return L_dfuDepartmentId.GetHashCode() ^
                (dfuDepartment == null ? 0 : dfuDepartment.GetHashCode()) ^
                (description == null ? 0 : description.GetHashCode());
        }
        */

        public int CompareTo(object obj)
        {
            L_DFUDepartment other = obj as L_DFUDepartment;

            if (other == null)
                return -1;

            return this.CodeDescription.CompareTo(other.CodeDescription);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(dfuDepartment))
                return "Angiv venligst sektionskode.";

            if (dfuDepartment.Length > 3)
                return "Koden må kun bestå af 3 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Angiv venligst beskrivelse.";

            if (description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_DFUDepartment>().Where(x => (x.L_dfuDepartmentId != L_dfuDepartmentId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.dfuDepartment.Equals(dfuDepartment, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En sektion med kode '{0}' eksisterer allerede.", dfuDepartment);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.dfuDepartment); }
        }

        public void NewLookupCreated()
        {
        }
    }
}
