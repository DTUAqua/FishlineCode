using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.Input
{
    public class FilterableKeyValuePair<T> : AViewModel, Anchor.Core.Controls.IFilteredComboBoxItem
    {
        private string _strKey;

        private T _value;

        private object _tag;


        public string Key
        {
            get { return _strKey; }
            set
            {
                _strKey = value;
                RaisePropertyChanged(() => Key);
            }
        }

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged(() => Value);
            }
        }


        public object Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                RaisePropertyChanged(() => Tag);
            }
        }

        public string CompareValue
        {
            get { return _strKey ?? ""; }
        }


        public 
            FilterableKeyValuePair(string key, T val)
        {
            _strKey = key;
            _value = val;
        }

       
    }
}
