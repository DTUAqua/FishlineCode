using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Babelfisk.ViewModels.Lookup
{
    public class ComboboxLookupColumn : LookupColumn
    {
        protected string _itemsSource;

        protected string _strDisplayValue;

        protected string _strSelectedValuePath;

        protected string _strNonEditDisplayPath;

        protected Func<System.Collections.IEnumerable> _funcItems;

       
        public Func<System.Collections.IEnumerable> GetComboboxItemMethod
        {
            get { return _funcItems; }
            set
            {
                _funcItems = value;
            }
        }



        public string ItemsSource
        {
            get { return _itemsSource; }
        }

        public string DisplayValuePath
        {
            get { return _strDisplayValue; }
        }


        public string SelectedValuePath
        {
            get { return _strSelectedValuePath; }
        }

        public string NonEditDisplayPath
        {
            get { return _strNonEditDisplayPath; }
        }

        private ComboboxLookupColumn()
        {
            _columnType = LookupColumnType.Dropdown;
        }

        public static LookupColumn Create(string strHeader, string strSelectedItem, bool blnReadOnly, string strItemsSource, string strDisplayValuePath, string strSelectedValuePath, string strNonEditDisplayPath, DataGridLength? width = null, double? minWidth = null, bool blnIsEditReadOnly = false)
        {
            return new ComboboxLookupColumn() { _strHeader = strHeader, _strBindingProperty = strSelectedItem, _blnIsReadOnly = blnReadOnly, _width = width, _itemsSource = strItemsSource, _strDisplayValue = strDisplayValuePath, _strSelectedValuePath = strSelectedValuePath, _strNonEditDisplayPath = strNonEditDisplayPath, _dblMinWidth = minWidth, _blnIsEditReadOnly = blnIsEditReadOnly };
        }

        public static LookupColumn Create(string strHeader, string strSelectedItem, bool blnReadOnly, Func<System.Collections.IEnumerable> getComboboxItemsMethod, string strDisplayValuePath, string strSelectedValuePath, string strNonEditDisplayPath, DataGridLength? width = null, double? minWidth = null, bool blnIsEditReadOnly = false)
        {
            return new ComboboxLookupColumn() { _strHeader = strHeader, _strBindingProperty = strSelectedItem, _blnIsReadOnly = blnReadOnly, _width = width, _funcItems = getComboboxItemsMethod, _strDisplayValue = strDisplayValuePath, _strSelectedValuePath = strSelectedValuePath, _strNonEditDisplayPath = strNonEditDisplayPath, _dblMinWidth = minWidth, _blnIsEditReadOnly = blnIsEditReadOnly };
        }



        public System.Collections.IEnumerable GetComboboxItems(Type t)
        {
            if (_funcItems != null)
                return _funcItems();

            return LookupViewModel.GetPropertyValue(t, ItemsSource) as System.Collections.IEnumerable;
        }
    }
}
