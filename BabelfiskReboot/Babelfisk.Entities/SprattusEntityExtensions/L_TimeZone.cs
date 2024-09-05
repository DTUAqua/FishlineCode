using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_TimeZone : ILookupEntity, IComparable
    {
        /*public static List<L_TimeZone> AllTimeZones
        {
            get
            {
                return new List<L_TimeZone>(new L_TimeZone[] { new L_TimeZone(-3, "Grønland (Nuuk), Brasilien"),
                                                               new L_TimeZone(-2, "Grytviken, Sør-Georgia og Sør-Sandwichøyene"),
                                                               new L_TimeZone(-1, "Grønland (Ittoqqortoormiit), Azorerne, Kap Verde"),
                                                               new L_TimeZone(0, "Engelsk vintertid, Island, Færøerne, Portugal"),
                                                               new L_TimeZone(1, "Dansk vintertid, Engelsk sommertid"),
                                                               new L_TimeZone(2, "Dansk sommertid")
                                                             });
            }
        }*/

        /// <summary>
        /// speciesCode code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.timeZone.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.timeZone, this.description );
            }
        }

        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string UIDisplay
        {
            get { return string.Format("(UTC{0}{1}) - {2}", timeZone >= 0 ? "+" : "", timeZone, description); }
        }


        public override bool Equals(object obj)
        {
            L_TimeZone other = obj as L_TimeZone;

            if (other == null)
                return false;

            return this.timeZone == other.timeZone;
        }


        public int CompareTo(object obj)
        {
            L_TimeZone other = obj as L_TimeZone;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(description))
                return "Angiv venligst bemærkninger.";

            if (description != null && description.Length > 150)
                return "Beskrivelse må kun bestå af 150 tegn.";

            if (lst.OfType<L_TimeZone>().Where(x => (x.L_timeZoneId != L_timeZoneId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.timeZone.Equals(timeZone)).Count() > 0)
                return string.Format("En tidszone med kode '{0}' eksisterer allerede.", timeZone);

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


        public override string ToString()
        {
            return CompareValue;
        }
    }
}
