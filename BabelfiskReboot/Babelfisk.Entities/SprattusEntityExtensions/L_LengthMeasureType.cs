using Babelfisk.Entities.Sprattus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_LengthMeasureType : ILookupEntity, IComparable
    {
        public string Id 
        {
            get { return this.L_lengthMeasureTypeId.ToString(); }
        }

        public string FilterValue
        {
            get { return String.Format("{0} {1}", this.lengthMeasureType, this.description); }
        }

        public string DefaultSortValue
        {
            get { return FilterValue; }
        }

        public string UIDisplay
        {
            get { return String.Format("{0} - {1}", this.lengthMeasureType, this.description); }
        }

        public override bool Equals(object obj)
        {
            L_LengthMeasureType other = obj as L_LengthMeasureType;

            if (other == null)
                return false;

            return this.L_lengthMeasureTypeId == other.L_lengthMeasureTypeId;
        }

        public int CompareTo(object obj)
        {
            L_LengthMeasureType other = obj as L_LengthMeasureType;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
            
        }

        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(lengthMeasureType))
                return "Angiv venligst kode.";

            if (lengthMeasureType.Length > 5)
                return "Koden må kun bestå af 5 tegn.";

            if (description != null && description.Length > 500)
                return "Beskrivelse må kun bestå af 500 tegn.";

            if (RDBES != null && RDBES.Length > 50)
                return "RDBES må kun bestå af 50 tegn.";

            //if (string.IsNullOrWhiteSpace(description))
            //    return "Alle nye/ændrede længdemålingstyper skal have en beskrivelse.";

            if (lst.OfType<L_LengthMeasureType>().Where(x => (x.L_lengthMeasureTypeId != L_lengthMeasureTypeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.lengthMeasureType.Equals(lengthMeasureType, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En længdemålingstype med kode '{0}' eksisterer allerede.", lengthMeasureType);

            return null;
        }

        public string CompareValue
        {
            get { return String.Format("{0}", this.lengthMeasureType); }
        }

        public void NewLookupCreated()
        { 
        }
    }
}
