using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
     [Serializable]
    public partial class Person : OfflineEntity, ILookupEntity, IComparable
    {
        /// <summary>
        /// personId is used as reference to other tables
        /// </summary>
         public string Id
         {
             get { return this.personId.ToString(); }
         }


        public string FilterValue
        {
            get { return String.Format("{0} {1} {2} {3} {4} {5} {6}", name, organization, address, zipTown, telephone, SEno, bankAccountNo); }
        }


        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string UIDisplay
        {
            get 
            {
                string str = name;
                if (!string.IsNullOrWhiteSpace(organization))
                    str += " - " + organization;

                return str;
            }
        }


        /// <summary>
        /// IFilteredCombobox filter property
        /// </summary>
        public string CompareValue
        {
            get { return UIDisplay; }
        }


        public override bool Equals(object obj)
        {
            Person other = obj as Person;

            if (other == null)
                return false;

            return  this.personId == other.personId &&
                    (this.organization == null ? other.organization == null : this.organization.Equals(other.organization)) &&
                    (this.address == null ? other.address == null : this.address.Equals(other.address)) &&
                    (this.zipTown == null ? other.zipTown == null : this.zipTown.Equals(other.zipTown)) &&
                    (this.telephone == null ? other.telephone == null : this.telephone.Equals(other.telephone)) &&
                    (this.telephonePrivate == null ? other.telephonePrivate == null : this.telephonePrivate.Equals(other.telephonePrivate)) &&
                    (this.telephoneMobile == null ? other.telephoneMobile == null : this.telephoneMobile.Equals(other.telephoneMobile)) &&
                    (this.email == null ? other.email == null : this.email.Equals(other.email)) &&
                    (this.facebook == null ? other.facebook == null : this.facebook.Equals(other.facebook)) &&
                    (this.SEno == null ? other.SEno == null : this.SEno.Equals(other.SEno)) &&
                    (this.bankAccountNo == null ? other.bankAccountNo == null : this.bankAccountNo.Equals(other.bankAccountNo))
                ;
        }

        /*
        public override int GetHashCode()
        {
            return personId.GetHashCode() ^
                   (name == null ? 0 : name.GetHashCode()) ^
                   (organization == null ? 0 : organization.GetHashCode()) ^
                   (address == null ? 0 : address.GetHashCode()) ^
                   (zipTown == null ? 0 : zipTown.GetHashCode());
        }
         */
         
        public int CompareTo(object obj)
        {
            Person other = obj as Person;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Angiv venligst navn.";

            if (name.Length > 80)
                return "Navn må kun bestå af 80 tegn.";

            if (organization != null && organization.Length > 60)
                return "Organisation må kun bestå af 60 tegn";

            if (address != null && address.Length > 60)
                return "Adresse må kun bestå af 60 tegn";

            if (zipTown != null && zipTown.Length > 30)
                return "Post nr. må kun bestå af 30 tegn";

            if (telephone != null && telephone.Length > 30)
                return "Tlf. nr. må kun bestå af 30 tegn";

            if (SEno != null && SEno.Length > 15)
                return "SE nr. må kun bestå af 15 tegn";

            if (bankAccountNo != null && bankAccountNo.Length > 15)
                return "Bankkonto må kun bestå af 15 tegn";

            return null;
        }


        public void NewLookupCreated()
        {
        }
    }
}
