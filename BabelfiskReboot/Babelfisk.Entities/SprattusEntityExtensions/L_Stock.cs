using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_Stock : ILookupEntity, IComparable
    {
        public string Id
        {
            get { return this.L_stockId.ToString(); }
        }


        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.stockCode, this.description ?? "");
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

                str += this.stockCode;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }

        public override bool Equals(object obj)
        {
            L_Stock other = obj as L_Stock;

            if (other == null)
                return false;

            return this.L_stockId == other.L_stockId &&
                (this.stockCode != null ? this.stockCode.Equals(other.stockCode) : other.stockCode == null) &&
                (description != null ? description.Equals(other.description) : other.description == null);
        }

        public int CompareTo(object obj)
        {
            L_Stock other = obj as L_Stock;

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
            if (string.IsNullOrWhiteSpace(stockCode))
                return "Angiv venligst bestandskode.";

            if (stockCode.Length > 50)
                return "Koden må kun bestå af 50 tegn.";

            if (description != null && description.Length > 1000)
                return "Beskrivelse må kun bestå af 1000 tegn.";

            if (lst.OfType<L_Stock>().Where(x => (x.L_stockId != L_stockId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.stockCode.Equals(stockCode, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Bestandskoden '{0}' eksisterer allerede.", stockCode);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { stockCode, this.description != null ? this.description : "" }; }
        }

    }
}

