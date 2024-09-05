using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
     [Serializable]
    public partial class L_PlatformType : ILookupEntity, IComparable
    {
        /// <summary>
        /// platformType code is used as reference to other tables
        /// </summary>
         public string Id
         {
             get { return this.platformType; }
         }

        public string FilterValue
        {
            get { return String.Format("{0} {1}", this.platformType, this.description == null ? "" : this.description.Trim()); }
        }

        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string UIDisplay
        {
            get { return CodeDescription; }
        }

        public string CodeDescription
        {
            get { return String.Format("{0} - {1}", this.platformType, this.description == null ? "" : this.description.Trim()); }
        }

        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.platformType); }
        }


        public override bool Equals(object obj)
        {
            L_PlatformType other = obj as L_PlatformType;

            if (other == null)
                return false;

            return this.L_platformTypeId == other.L_platformTypeId;
        }

         /*
        public override int GetHashCode()
        {
            return L_platformTypeId.GetHashCode() ^
                   (platformType == null ? 0 : platformType.GetHashCode()) ^
                   (description == null ? 0 : description.GetHashCode());
        }
         */

        public int CompareTo(object obj)
        {
            L_PlatformType other = obj as L_PlatformType;

            if (other == null)
                return -1;

            return this.CodeDescription.CompareTo(other.CodeDescription);
        }

        public void BeforeSave()
        {
        }

        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(platformType))
                return "Angiv venligst kode.";

            if (platformType != null && platformType.Length > 2)
                return "Koden må kun bestå af 2 tegn.";

            if(description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_PlatformType>().Where(x => (x.L_platformTypeId != L_platformTypeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.platformType.Equals(platformType, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En skibstype med kode '{0}' eksisterer allerede.", platformType);

            return null;
        }


        public void NewLookupCreated()
        {
        }
      
    }
}
