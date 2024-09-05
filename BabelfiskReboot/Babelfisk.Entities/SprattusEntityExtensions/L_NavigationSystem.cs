using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_NavigationSystem : ILookupEntity, IComparable
    {
        /// <summary>
        /// navigationSystem code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.navigationSystem; }
        }


        public string FilterValue
        {
            get { return String.Format("{0} {1}", this.navigationSystem, this.description); }
        }


        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string CodeDescription
        {
            get { return String.Format("{0} - {1}", this.navigationSystem, this.description); }
        }


        public string UIDisplay
        {
            get { return CodeDescription; }
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.navigationSystem); }
        }


        public override bool Equals(object obj)
        {
            L_NavigationSystem other = obj as L_NavigationSystem;

            if (other == null)
                return false;

            return this.L_navigationSystemId == other.L_navigationSystemId;
        }

        /*
        public override int GetHashCode()
        {
            return L_navigationSystemId.GetHashCode() ^
                   (navigationSystem == null ? 0 : navigationSystem.GetHashCode()) ^
                   (description == null ? 0 : description.GetHashCode());
        }
        */

        public int CompareTo(object obj)
        {
            L_NavigationSystem other = obj as L_NavigationSystem;

            if (other == null)
                return -1;

            return this.CodeDescription.CompareTo(other.CodeDescription);
        }

        public void BeforeSave()
        {
        }

        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(navigationSystem))
                return "Angiv venligst kode.";

            if (navigationSystem.Length > 3)
                return "Koden må kun bestå af 3 tegn.";

            if (description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";


            if (lst.OfType<L_NavigationSystem>().Where(x => (x.L_navigationSystemId != L_navigationSystemId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.navigationSystem.Equals(navigationSystem, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Et navigationssystem med kode '{0}' eksisterer allerede.", navigationSystem);

            return null;
        }

        public void NewLookupCreated()
        {
        }
    }
}
