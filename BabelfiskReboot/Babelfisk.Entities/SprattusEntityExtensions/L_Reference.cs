using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_Reference : ILookupEntity, IComparable
    {
        private string _strText;

        private bool _blnIsChecked;

        private bool _blnIsDirty;

        public string Text
        {
            get { return UIDisplay; }
        }


        public bool IsChecked
        {
            get { return _blnIsChecked; }
            set
            {
                _blnIsChecked = value;
                IsDirty = true;
                OnPropertyChanged("IsChecked");
            }
        }


        public bool IsDirty
        {
            get { return _blnIsDirty; }
            set
            {
                _blnIsDirty = value;
                OnPropertyChanged("IsDirty");
            }
        }


        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.L_referenceId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.reference, this.description);
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
                string str = this.reference;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_Reference other = obj as L_Reference;

            if (other == null)
                return false;

            return this.L_referenceId == other.L_referenceId &&
                (this.reference != null ? this.reference.Equals(other.reference, StringComparison.InvariantCultureIgnoreCase) : other.reference == null) &&
                (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            L_Reference other = obj as L_Reference;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return "Angiv venligst kode.";

            if (reference.Length > 20)
                return "Koden må kun bestå af 20 tegn.";

            if (description != null && description.Length > 250)
                return "Beskrivelse må kun bestå af 250 tegn.";

            if (lst.OfType<L_Reference>().Where(x => (x.L_referenceId != L_referenceId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.reference.Equals(reference, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", reference);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.reference); }
        }


        public void NewLookupCreated()
        {
        }
    }
}
