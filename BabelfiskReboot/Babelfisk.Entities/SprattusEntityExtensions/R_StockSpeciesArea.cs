using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class R_StockSpeciesArea : ILookupEntity, IComparable
    {
        private static List<L_Stock> _lstStocks;

        private static List<L_Species> _lstSpecies;

        private static List<L_DFUArea> _lstAreas;

        private static List<L_StatisticalRectangle> _lstStatisticalRectangles;

        private static List<int> _lstQuarters;


        private static Dictionary<int, L_Stock> _dicStocks = null;
        private static Dictionary<string, L_Species> _dicSpecies = null;
        private static Dictionary<string, L_DFUArea> _dicAreas = null;
        private static Dictionary<string, L_StatisticalRectangle> _dicStatisticalRectangles = null;


        public static List<L_Stock> Stocks
        {
            get { return _lstStocks; }
            set
            {
                _lstStocks = value;
                if (value != null)
                    _dicStocks = value.ToDictionary(x => x.L_stockId);
            }
        }

        public string L_StockCodeDisplay
        {
            get
            {
                if (_dicStocks == null || !_dicStocks.ContainsKey(L_stockId))
                    return "";

                return _dicStocks[L_stockId].stockCode;
            }
        }

        public static List<L_Species> Species
        {
            get { return _lstSpecies; }
            set
            {
                _lstSpecies = value;
                if (value != null)
                    _dicSpecies = value.ToDictionary(x => x.speciesCode);
            }
        }

        public string L_SpeciesUIDisplay
        {
            get
            {
                if (speciesCode == null || _dicSpecies == null || !_dicSpecies.ContainsKey(speciesCode))
                    return "";

                return _dicSpecies[speciesCode].UIDisplay;
            }
        }


        public static List<L_DFUArea> Areas
        {
            get { return _lstAreas; }
            set
            {
                _lstAreas = value;
                if (value != null)
                    _dicAreas = value.ToDictionary(x => x.DFUArea);
            }
        }


        public string L_DFUAreaUIDisplay
        {
            get
            {
                if (DFUArea == null || _dicAreas == null || !_dicAreas.ContainsKey(DFUArea))
                    return "";

                return _dicAreas[DFUArea].UIDisplay;
            }
        }


        public static List<L_StatisticalRectangle> StatisticalRectangles
        {
            get { return _lstStatisticalRectangles; }
            set
            {
                _lstStatisticalRectangles = value;
                if (value != null)
                    _dicStatisticalRectangles = value.ToDictionary(x => x.statisticalRectangle);
            }
        }


        public string L_StatisticalRectangleUIDisplay
        {
            get
            {
                if (statisticalRectangle == null || _dicStatisticalRectangles == null || !_dicStatisticalRectangles.ContainsKey(statisticalRectangle))
                    return "";

                return _dicStatisticalRectangles[statisticalRectangle].UIDisplay;
            }
        }


        public static List<int> Quarters
        {
            get 
            {
                if (_lstQuarters == null)
                    _lstQuarters = new List<int>() { 1, 2, 3, 4 };
                return _lstQuarters;
            }
        }


        public string Id
        {
            get { return this.r_StockSpeciesAreaId.ToString(); }
        }


        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2} {3} {4}", this.L_Stock != null ? this.L_Stock.stockCode : L_StockCodeDisplay, this.speciesCode, this.DFUArea, this.statisticalRectangle ?? "", this.quarter.HasValue ? this.quarter.ToString() : "");
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

                return FilterValue;
            }
        }

        public override bool Equals(object obj)
        {
            R_StockSpeciesArea other = obj as R_StockSpeciesArea;

            if (other == null)
                return false;

            return this.r_StockSpeciesAreaId == other.r_StockSpeciesAreaId &&
                   this.L_stockId == other.L_stockId &&
                  (this.speciesCode != null ? this.speciesCode.Equals(other.speciesCode) : other.speciesCode == null) &&
                  (DFUArea != null ? DFUArea.Equals(other.DFUArea) : other.DFUArea == null) &&
                  (statisticalRectangle != null ? statisticalRectangle.Equals(other.statisticalRectangle) : other.statisticalRectangle == null) &&
                  (quarter != null ? quarter.Equals(other.quarter) : other.quarter == null);
        }

        public int CompareTo(object obj)
        {
            R_StockSpeciesArea other = obj as R_StockSpeciesArea;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }
        public void NewLookupCreated()
        {
        }

        public string GetErrors(List<ILookupEntity> lst)
        {
            if (L_stockId == -1 || L_stockId == 0)
                return "Angiv venligst bestand.";

            if (string.IsNullOrWhiteSpace(speciesCode))
                return "Angiv venligst art.";

            if (string.IsNullOrWhiteSpace(DFUArea))
                return "Angiv venligst område.";

            if (lst.OfType<R_StockSpeciesArea>().Where(x => (x.r_StockSpeciesAreaId != r_StockSpeciesAreaId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.L_stockId.Equals(L_stockId) &&
                                                                                                                                                                               (x.speciesCode != null ? x.speciesCode.Equals(speciesCode) : speciesCode == null) &&
                                                                                                                                                                               (x.DFUArea != null ? x.DFUArea.Equals(DFUArea) : DFUArea == null) &&
                                                                                                                                                                               (x.statisticalRectangle != null ? x.statisticalRectangle.Equals(statisticalRectangle) : statisticalRectangle == null) &&
                                                                                                                                                                               (x.quarter != null ? x.quarter.Equals(quarter) : quarter == null)).Count() > 0)
                return string.Format("Relationen '{0}' eksisterer allerede.", UIDisplay);

            return null;
        }


        public override string ToString()
        {
            return FilterValue;
        }
    }
}
