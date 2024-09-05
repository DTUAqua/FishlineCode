using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SDEventType : ILookupEntity, IComparable
    {
        private static List<KeyValuePair<string, string>> _lstAgeUpdatingMethods;

        private static Dictionary<string, KeyValuePair<string, string>> _dicAgeUpdatingMethods = null;

        public static List<KeyValuePair<string, string>> AgeUpdatingMethods
        {
            get { return _lstAgeUpdatingMethods; }
            set
            {
                _lstAgeUpdatingMethods = value;
                if (value != null)
                    _dicAgeUpdatingMethods = value.ToDictionary(x => x.Key, y => y);
            }
        }

        public string AgeUpdatingMethodUIDisplay
        {
            get
            {
                if (_dicAgeUpdatingMethods == null || !_dicAgeUpdatingMethods.ContainsKey(this.ageUpdatingMethod))
                    return "";

                return _dicAgeUpdatingMethods[this.ageUpdatingMethod].Value;
            }
        }


        public string Id
        {
            get { return this.L_sdEventTypeId.ToString(); }
        }


        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.eventType, this.description ?? "");
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

                str += this.eventType;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }

        public override bool Equals(object obj)
        {
            L_SDEventType other = obj as L_SDEventType;

            if (other == null)
                return false;

            return this.L_sdEventTypeId == other.L_sdEventTypeId &&
                (this.eventType != null ? this.eventType.Equals(other.eventType) : other.eventType == null) &&
                (description != null ? description.Equals(other.description) : other.description == null) && 
                (ageUpdatingMethod == other.ageUpdatingMethod);
        }

        public int CompareTo(object obj)
        {
            L_SDEventType other = obj as L_SDEventType;

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
            if (string.IsNullOrWhiteSpace(eventType))
                return "Angiv venligst hændelsestype.";

            if (eventType.Length > 100)
                return "Koden må kun bestå af 100 tegn.";

            if (description != null && description.Length > 500)
                return "Beskrivelse må kun bestå af 500 tegn.";

            if (lst.OfType<L_SDEventType>().Where(x => (x.L_sdEventTypeId != L_sdEventTypeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.eventType.Equals(eventType, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Hændelsestypen '{0}' eksisterer allerede.", eventType);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { eventType, description == null ? "" : description }; }
        }
    }
}
