using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SDReaderExperience : ILookupEntity, IComparable
    {
        public string Id
        {
            get { return this.L_SDReaderExperienceId.ToString(); }
        }


        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.readerExperience, this.description ?? "");
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

                str += this.readerExperience;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }

        public override bool Equals(object obj)
        {
            L_SDReaderExperience other = obj as L_SDReaderExperience;

            if (other == null)
                return false;

            return this.L_SDReaderExperienceId == other.L_SDReaderExperienceId &&
                (this.readerExperience != null ? this.readerExperience.Equals(other.readerExperience) : other.readerExperience == null) &&
                (description != null ? description.Equals(other.description) : other.description == null);
        }

        public int CompareTo(object obj)
        {
            L_SDReaderExperience other = obj as L_SDReaderExperience;

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
            if (string.IsNullOrWhiteSpace(readerExperience))
                return "Angiv venligst en erfaringskode.";

            if (readerExperience.Length > 50)
                return "Erfaringskode må kun bestå af 50 tegn.";

            if (description != null && description.Length > 500)
                return "Beskrivelse må kun bestå af 500 tegn.";
           
            if (lst.OfType<L_SDReaderExperience>().Where(x => (x.L_SDReaderExperienceId != L_SDReaderExperienceId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.readerExperience.Equals(readerExperience, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Erfaringskoden '{0}' eksisterer allerede.", readerExperience);

            return null;
        }

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { readerExperience, this.description != null ? this.description : "" }; }
        }
    }
}
