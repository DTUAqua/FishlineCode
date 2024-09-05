using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class ICES_DFU_Relation_FF : ILookupEntity, IComparable
    {
        private static List<L_DFUArea> _lstAreas;
        private static List<L_StatisticalRectangle> _lstRectangles;

        private static Dictionary<string, L_StatisticalRectangle> _dicRectangles = null;
        private static Dictionary<string, L_DFUArea> _dicAreas = null;


        public static List<L_DFUArea> Areas
        {
            get { return _lstAreas.ToList(); }
            set
            {
                _lstAreas = value;
                if (value != null)
                    _dicAreas = value.ToDictionary(x => x.DFUArea.ToLower());
            }
        }


        public static List<L_StatisticalRectangle> Rectangles
        {
            get { return _lstRectangles.ToList(); }
            set
            {
                _lstRectangles = value;
                if (value != null)
                    _dicRectangles = value.ToDictionary(x => x.statisticalRectangle.ToUpper());
            }
        }


        public string AreaUIDisplay
        {
            get
            {
                if (Area == null || _dicAreas == null || !_dicAreas.ContainsKey(Area.ToLower()))
                    return "";

                return _dicAreas[Area.ToLower()].UIDisplay;
            }
        }


        public string Area_20_21Upper
        {
            get { return this.area_20_21 == null ? null : this.area_20_21.ToUpper(); }
            set
            {
                this.area_20_21 = value;
                OnPropertyChanged("Area_20_21Upper");
            }
        }


        public string Area20_21UIDisplay
        {
            get
            {
                if (area_20_21 == null || _dicAreas == null || !_dicAreas.ContainsKey(area_20_21.ToLower()))
                    return "";

                return _dicAreas[area_20_21.ToLower()].UIDisplay;
            }
        }


        public string RectangleUIDisplay
        {
            get
            {
                if (statisticalRectangle == null || _dicRectangles == null || !_dicRectangles.ContainsKey(statisticalRectangle.ToUpper()))
                    return "";

                return _dicRectangles[statisticalRectangle.ToUpper()].UIDisplay;
            }
        }


        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.ICES_DFU_Relation_FFId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.statisticalRectangle, this.Area, this.area_20_21);
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
                string str = this.statisticalRectangle;

                if (!string.IsNullOrWhiteSpace(Area))
                    str += " - " + Area;

                if (!string.IsNullOrWhiteSpace(area_20_21))
                    str += " - " + area_20_21;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            ICES_DFU_Relation_FF other = obj as ICES_DFU_Relation_FF;

            if (other == null)
                return false;

            return this.ICES_DFU_Relation_FFId == other.ICES_DFU_Relation_FFId &&
                (this.statisticalRectangle != null ? this.statisticalRectangle.Equals(other.statisticalRectangle) : other.statisticalRectangle == null) &&
                (Area != null ? Area.Equals(other.Area) : other.Area == null) &&
                (area_20_21 != null ? area_20_21.Equals(other.area_20_21) : other.area_20_21 == null)
                ;
        }

        public int CompareTo(object obj)
        {
            ICES_DFU_Relation_FF other = obj as ICES_DFU_Relation_FF;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }


        public void BeforeSave()
        {
            if (string.IsNullOrWhiteSpace(Area))
                Area = _area_20_21;
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(statisticalRectangle))
                return "Angiv venligst statistisk rektangel.";

            //Remove Area from errors because it is ignored and assigned in the BeforeSave method if missing.
            //if (string.IsNullOrWhiteSpace(Area))
            //    return "Angiv venligst område.";

            if(string.IsNullOrWhiteSpace(area_20_21))
                return "Angiv venligst område 20 21.";

            if (lst.OfType<ICES_DFU_Relation_FF>().Where(x => (x.ICES_DFU_Relation_FFId != ICES_DFU_Relation_FFId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && 
                                                              x.statisticalRectangle.Equals(statisticalRectangle, StringComparison.InvariantCultureIgnoreCase) &&
                                                              x.Area.Equals(Area, StringComparison.InvariantCultureIgnoreCase) &&
                                                               x.area_20_21.Equals(area_20_21, StringComparison.InvariantCultureIgnoreCase)
                                                              ).Count() > 0)
                return string.Format("En relation med statistisk rektangel '{0}', område '{1}' og område '{2}' eksisterer allerede.", statisticalRectangle, Area, area_20_21);

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
