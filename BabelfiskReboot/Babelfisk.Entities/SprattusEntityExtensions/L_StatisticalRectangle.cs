using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_StatisticalRectangle : ILookupEntity, IComparable
    {
        /// <summary>
        /// gearQuality code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.statisticalRectangle; }
        }

        public string FilterValue
        {
            get { return String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12}", statisticalRectangle, latitudeDecMin, latitudeDecMax, longitudeDecMin, longitudeDecMax, latitudeDecMid, longitudeDecMid, latitudeTextMin, latitudeTextMax, longitudeTextMin, longitudeTextMax, latitudeTextMid, longitudeTextMid); }
        }


        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string UIDisplay
        {
            get { return String.Format("{0}", this.statisticalRectangle); }
        }


        public override bool Equals(object obj)
        {
            L_StatisticalRectangle other = obj as L_StatisticalRectangle;

            if (other == null)
                return false;

            return this.L_statisticalRectangleId == other.L_statisticalRectangleId &&
                (this.statisticalRectangle == null ? other.statisticalRectangle == null : this.statisticalRectangle.Equals(other.statisticalRectangle))
                ;
        }

        /*
        public override int GetHashCode()
        {
            return L_statisticalRectangleId.GetHashCode() ^
                   (statisticalRectangle == null ? 0 : statisticalRectangle.GetHashCode())
                    ;
        }
        */

        public int CompareTo(object obj)
        {
            L_StatisticalRectangle other = obj as L_StatisticalRectangle;

            if (other == null)
                return -1;

            return this.UIDisplay.ToUpper().CompareTo(other.UIDisplay.ToUpper());
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(statisticalRectangle))
                return "Angiv venligst kode.";

            if (statisticalRectangle.Length > 4)
                return "Koden må kun bestå af 4 tegn.";

            if (latitudeDecMin.HasValue && (latitudeDecMin.Value < -90 || latitudeDecMin > 90))
                return "Min bred. skal være mellem -90 og 90";

            if (latitudeDecMax.HasValue && (latitudeDecMax.Value < -90 || latitudeDecMax > 90))
                return "Max bred. skal være mellem -90 og 90";

            if (latitudeDecMid.HasValue && (latitudeDecMid.Value < -90 || latitudeDecMid > 90))
                return "Midt bred. skal være mellem -90 og 90";


            if (longitudeDecMin.HasValue && (longitudeDecMin.Value < -180 || longitudeDecMin > 180))
                return "Min læng. skal være mellem -180 og 180";

            if (longitudeDecMax.HasValue && (longitudeDecMax.Value < -180 || longitudeDecMax > 180))
                return "Max læng. skal være mellem -180 og 180";

            if (longitudeDecMid.HasValue && (longitudeDecMid.Value < -180 || longitudeDecMid > 180))
                return "Midt læng. skal være mellem -180 og 180";

            if (latitudeTextMin != null && latitudeTextMin.Length > 12)
                return "Min bred. streng må kun bestå af 12 tegn";

            if (latitudeTextMax != null && latitudeTextMax.Length > 12)
                return "Max bred. streng må kun bestå af 12 tegn";

            if (latitudeTextMid != null && latitudeTextMid.Length > 12)
                return "Midt bred. streng må kun bestå af 12 tegn";

            if (longitudeTextMin != null && longitudeTextMin.Length > 13)
                return "Min læng. streng må kun bestå af 13 tegn";

            if (longitudeTextMax != null && longitudeTextMax.Length > 13)
                return "Max læng. streng må kun bestå af 13 tegn";

            if (longitudeTextMid != null && longitudeTextMid.Length > 13)
                return "Midt læng. streng må kun bestå af 13 tegn";

            if (lst.OfType<L_StatisticalRectangle>().Where(x => (x.L_statisticalRectangleId != L_statisticalRectangleId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.statisticalRectangle.Equals(statisticalRectangle, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En statistisk rektangel med kode '{0}' eksisterer allerede.", statisticalRectangle);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return UIDisplay; }
        }


        public void NewLookupCreated()
        {
        }

    }
}
