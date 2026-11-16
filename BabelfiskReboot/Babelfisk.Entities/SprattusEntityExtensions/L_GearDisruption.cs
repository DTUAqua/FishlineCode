using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_GearDisruption : ILookupEntity, IComparable
    {
        public string Id
        {
            get { return this.L_gearDisruptionId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.gearDisruption, this.description, this.num);
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
                string str = gearDisruption.ToString();


                if(!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_GearDisruption other = obj as L_GearDisruption;

            if(other == null)
                return false;

            return this.L_gearDisruptionId == other.L_gearDisruptionId &&
                   (this.gearDisruption != null ? this.gearDisruption.Equals(other.gearDisruption) : other.gearDisruption == null) &&
                   (this.num.HasValue ? this.num.Equals(other.num) : !other.num.HasValue) &&
                   (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null) &&
                   (codeICES != null ? codeICES.Equals(other.codeICES) : other.codeICES == null);
        }


        public int CompareTo(object obj)
        {
            L_GearDisruption other = obj as L_GearDisruption;

            if(other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if(description != null && description.Length > 500)
                return "Beskrivelse må kun bestå af 500 tegn.";

            if(gearDisruption != null && gearDisruption.Length > 200)
                return "Træktidsbemærkning må kun bestå af 200 tegn.";

            if(codeICES != null && codeICES.Length > 50)
                return "ICES-kode må kun bestå af 50 tegn.";

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return UIDisplay; }
        }

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
       /* public string[] CompareValues
        {
            get { return new string[] { gearDisruption, description }; }
        }*/


        public void NewLookupCreated()
        {
        }
    }
}
