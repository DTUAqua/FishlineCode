using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Babelfisk.ViewModels.Lookup
{
    public class LookupColumn
    {
        public enum LookupColumnType
        {
            TextBox,
            Dropdown,
            CheckBox
        }

        protected string _strHeader;

        protected bool _blnIsReadOnly;

        protected string _strBindingProperty;

        protected DataGridLength? _width;

        protected LookupColumnType _columnType;

        protected double? _dblMinWidth;

        protected bool _blnIsEditReadOnly = false;

        private object _objTargetNullValue = null;

        
        public string Header
        {
            get { return _strHeader; }
 
        }

        public bool IsReadOnly
        {
            get { return _blnIsReadOnly; }
       
        }

        public string BindingProperty
        {
            get { return _strBindingProperty; }
        }


        public DataGridLength? Width
        {
            get { return _width; }
        }


        public double? MinWidth
        {
            get { return _dblMinWidth; }
        }


        public LookupColumnType ColumnType
        {
            get { return _columnType; }
        }


        public bool IsEditReadOnly
        {
            get { return _blnIsEditReadOnly; }
            set { _blnIsEditReadOnly = value; }
        }


        public object TargetNullValue
        {
            get { return _objTargetNullValue; }
            set { _objTargetNullValue = value; }
        }


        protected LookupColumn()
        {
            _columnType = LookupColumnType.TextBox;
        }

        public static LookupColumn Create(string strHeader, string strBindingProperty, bool blnReadOnly, DataGridLength? width = null, double? minWidth = null, LookupColumnType columnType = LookupColumnType.TextBox, object targetNullValue = null)
        {
            return new LookupColumn() { _strHeader = strHeader, _strBindingProperty = strBindingProperty, _blnIsReadOnly = blnReadOnly, _width = width, _dblMinWidth = minWidth, _columnType = columnType, TargetNullValue = targetNullValue};
        }


        public override string ToString()
        {
            return Header;
        }

    }
}
