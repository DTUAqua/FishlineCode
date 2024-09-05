using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class R_SDReader : ILookupEntity, IComparable
    {
        private static List<L_Species> _lstSpecies;

        private static List<L_Stock> _lstStocks;

        private static List<L_SDReaderExperience> _lstReaderExperiences;

        private static List<L_SDPreparationMethod> _lstPreperationMethods;

        private static List<DFUPerson> _lstDFUPersons;

        private static List<R_StockSpeciesArea> _lstSpeciesAreaStock;

        private static Dictionary<string, L_Species> _dicSpecies = null;
        private static Dictionary<int, L_Stock> _dicStocks = null;
        private static Dictionary<int, L_SDReaderExperience> _dicReaderExperiences = null;
        private static Dictionary<int, L_SDPreparationMethod> _dicPreparationMethods = null;
        private static Dictionary<int, DFUPerson> _dicDFUPersons = null;

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


        public static List<R_StockSpeciesArea> StockSpeciesArea
        {
            get { return _lstSpeciesAreaStock; }
            set
            {
                _lstSpeciesAreaStock = value;
            }
        }


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


        public List<L_Stock> StocksForSelectedSpecies
        {
            get
            {
                if (string.IsNullOrWhiteSpace(speciesCode) || _lstSpeciesAreaStock == null)
                    return new List<L_Stock>();

                var lst = _lstSpeciesAreaStock.Where(x => _dicStocks != null && x.speciesCode == speciesCode && _dicStocks.ContainsKey(x.L_stockId))
                                              .Select(x => _dicStocks[x.L_stockId])
                                              .OrderBy(x => x.stockCode)
                                              .ToList();

                return lst;
            }
        }


        public string SpeciesCodeForLookupList
        {
            get { return this.speciesCode; }
            set
            {
                this.speciesCode = value;
                OnPropertyChanged("SpeciesCodeForLookupList");
                OnPropertyChanged("StocksForSelectedSpecies");
            }
        }


        public static List<L_SDReaderExperience> ReaderExperiences
        {
            get { return _lstReaderExperiences; }
            set
            {
                _lstReaderExperiences = value;
                if (value != null)
                    _dicReaderExperiences = value.ToDictionary(x => x.L_SDReaderExperienceId);
            }
        }


        public static List<L_SDPreparationMethod> PreparationMethods
        {
            get { return _lstPreperationMethods; }
            set
            {
                _lstPreperationMethods = value;
                if (value != null)
                    _dicPreparationMethods = value.ToDictionary(x => x.L_sdPreparationMethodId);
            }
        }


        public static List<DFUPerson> DFUPersons
        {
            get { return _lstDFUPersons; }
            set
            {
                _lstDFUPersons = value;
                if (value != null)
                    _dicDFUPersons = value.ToDictionary(x => x.dfuPersonId);
            }
        }


        public string Id
        {
            get { return this.r_SDReaderId.ToString(); }
        }


        public string FilterValue
        {
            get
            { 
                return String.Format("{0} {1}", this.speciesCode, this.comment ?? "");
            }
        }

        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string DFUPersonDisplay
        {
            get 
            {
                if (_dicDFUPersons == null || !_dicDFUPersons.ContainsKey(this.dfuPersonId))
                    return "";

                return _dicDFUPersons[this.dfuPersonId].initials;
            }
        }


        public string StockDisplay
        {
            get
            {
                if (_dicStocks == null || !this.stockId.HasValue || !_dicStocks.ContainsKey(this.stockId.Value))
                    return "";

                return _dicStocks[this.stockId.Value].stockCode;
            }
        }


        public string ReaderExperienceDisplay
        {
            get
            {
                if (_dicReaderExperiences == null || !this.sdReaderExperienceId.HasValue || !_dicReaderExperiences.ContainsKey(this.sdReaderExperienceId.Value))
                    return "";

                return _dicReaderExperiences[this.sdReaderExperienceId.Value].readerExperience;
            }
        }


        public string PreperationMethodDisplay
        {
            get
            {
                if (_dicPreparationMethods == null || !this.sdPreparationMethodId.HasValue || !_dicPreparationMethods.ContainsKey(this.sdPreparationMethodId.Value))
                    return "";

                return _dicPreparationMethods[this.sdPreparationMethodId.Value].preparationMethod;
            }
        }


        public string UIDisplay
        {
            get
            {
                string str = "";

                str += this.speciesCode;

                if (!string.IsNullOrWhiteSpace(comment))
                    str += " - " + comment;

                return str;
            }
        }

        public override bool Equals(object obj)
        {
            R_SDReader other = obj as R_SDReader;

            if (other == null)
                return false;

            return this.r_SDReaderId == other.r_SDReaderId &&
                this.dfuPersonId == other.dfuPersonId &&
                (this.speciesCode != null ? this.speciesCode.Equals(other.speciesCode) : other.speciesCode == null) &&
                 this.stockId == other.stockId &&
                 this.firstYearAgeReadingGeneral == other.firstYearAgeReadingGeneral &&
                 this.firstYearAgeReadingCurrent == other.firstYearAgeReadingCurrent &&
                 this.sdReaderExperienceId == other.sdReaderExperienceId &&
                 this.sdPreparationMethodId == other.sdPreparationMethodId &&
                (comment != null ? comment.Equals(other.comment) : other.comment == null);
        }

        public int CompareTo(object obj)
        {
            R_SDReader other = obj as R_SDReader;

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
            if (string.IsNullOrWhiteSpace(speciesCode))
                return "Angiv venligst en art.";

            if (!stockId.HasValue)
                return "Angiv venligst en bestand.";

            if (comment != null && comment.Length > 1000)
                return "Kommentar må kun bestå af 1000 tegn.";

            if (firstYearAgeReadingGeneral.HasValue && (firstYearAgeReadingGeneral.Value < 1900 || firstYearAgeReadingGeneral.Value > DateTime.UtcNow.Year))
                return "1. års aflæsning (generelt) skal være senere end år 1900 og tidligere eller lig med år " + DateTime.UtcNow.Year;

            if (firstYearAgeReadingCurrent.HasValue && (firstYearAgeReadingCurrent.Value < 1900 || firstYearAgeReadingCurrent.Value > DateTime.UtcNow.Year))
                return "1. års aflæsning (art/bestand) skal være senere end år 1900 og tidligere eller lig med år " + DateTime.UtcNow.Year;

            if (lst.OfType<R_SDReader>().Where(x => (x.r_SDReaderId != r_SDReaderId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.dfuPersonId == dfuPersonId &&
                                                                                                                                                        x.firstYearAgeReadingGeneral == firstYearAgeReadingGeneral &&
                                                                                                                                                        x.speciesCode == speciesCode &&
                                                                                                                                                        x.stockId == stockId &&
                                                                                                                                                        x.firstYearAgeReadingCurrent == firstYearAgeReadingCurrent &&
                                                                                                                                                        x.sdReaderExperienceId == sdReaderExperienceId &&
                                                                                                                                                        x.sdPreparationMethodId == sdPreparationMethodId
                                                                                                                                                        ).Count() > 0)
                return string.Format("En aquadots aflæserekspertise med værdierne '{0} | {1} | {2} | {3} | {4} | {5} | {6}' eksisterer allerede.", DFUPersonDisplay, 
                                                                                                                                    firstYearAgeReadingGeneral.HasValue ? firstYearAgeReadingGeneral.Value.ToString() : "",
                                                                                                                                    speciesCode,
                                                                                                                                    StockDisplay,
                                                                                                                                    firstYearAgeReadingCurrent.HasValue ? firstYearAgeReadingCurrent.Value.ToString() : "",
                                                                                                                                    ReaderExperienceDisplay,
                                                                                                                                    PreperationMethodDisplay
                                                                                                                                    );

            return null;
        }

    }
}
