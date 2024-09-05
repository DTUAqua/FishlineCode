using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_DFUArea : ILookupEntity, IComparable
    {
        private static List<L_DFUArea> _lstAreas;

        public static List<L_DFUArea> Areas
        {
            get { return _lstAreas; }
            set { _lstAreas = value; }
        }

        /// <summary>
        /// DFUArea code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.DFUArea; }
        }

        public string FilterValue
        {
            get { return String.Format("{0} {1} {2} {3} {4}", this.DFUArea ?? "", this.description ?? "", this.parentDFUArea ?? "", this.areaNES ?? "", this.areaICES ?? ""); } 
        }


        public string DefaultSortValue
        {
            get { return this.DFUArea ?? ""; }
        }


        public string DFUAreaUpper
        {
            get { return this.DFUArea == null ? null : this.DFUArea.ToUpper(); }
            set
            {
                this.DFUArea = value;
                OnPropertyChanged("DFUAreaUpper");
            }
        }


        public string dfuArea2
        {
            get { return L_DFUArea2 == null ? null : L_DFUArea2.DFUArea; }
            set
            {
                L_DFUArea2 = Areas.Where(x => x.DFUArea.Equals(value, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }
        }


        public string UIDisplay
        {
            get { return String.Format("{0}", this.DFUArea, this.description); }
        }


        public override bool Equals(object obj)
        {
            L_DFUArea other = obj as L_DFUArea;

            if (other == null)
                return false;
            
            return this.L_DFUAreaId == other.L_DFUAreaId &&
                   (DFUArea != null ? DFUArea.Equals(other.DFUArea, StringComparison.InvariantCultureIgnoreCase) : other.DFUArea == null) &&
                   (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null) &&
                   (parentDFUArea != null ? parentDFUArea.Equals(other.parentDFUArea, StringComparison.InvariantCultureIgnoreCase) : other.parentDFUArea == null)
                   ;
        }

        /*
        public override int GetHashCode()
        {
            return L_DFUAreaId.GetHashCode() ^
                (DFUArea == null ? 0 : DFUArea.GetHashCode()) ^
                (description == null ? 0 : description.GetHashCode());
        }
        */

        public int CompareTo(object obj)
        {
            L_DFUArea other = obj as L_DFUArea;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(DFUArea))
                return "Angiv venligst kode.";

            if (DFUArea.Length > 3)
                return "Koden må kun bestå af 3 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Angiv venligst bemærkninger.";

            if (description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (areaNES != null && areaNES.Length > 10)
                return "NES kode må kun bestå af 10 tegn.";

            if (lst.OfType<L_DFUArea>().Where(x => (x.L_DFUAreaId != L_DFUAreaId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.DFUArea.Equals(DFUArea, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Et område med kode '{0}' eksisterer allerede.", DFUArea);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.DFUArea); }
        }

        public void NewLookupCreated()
        {
        }
    }
}
