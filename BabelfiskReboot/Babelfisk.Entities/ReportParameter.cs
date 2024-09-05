using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using Anchor.Core;

namespace Babelfisk.Entities
{
    [DataContract]
    public class ReportParameter : INotifyPropertyChanged
    {
        [DataMember]
        private int _intId;


        [DataMember]
        private string _strDisplayName;


        /// <summary>
        /// SQL, Integer, Decimal, String, List
        /// </summary>
        [DataMember]
        private string _strType;


        [DataMember]
        private string _parameterName;

        
        /// <summary>
        /// Integer, Decimal, String, Boolean
        /// </summary>
        [DataMember]
        private string _strReturnType;

        
        [DataMember]
        private string _strSelectionMode;

        
        [DataMember]
        private bool _blnIsOptional;

       
        [DataMember]
        private string _strStatement;


        [DataMember]
        private ObservableCollection<ParameterListItem> _list;


        [DataMember]
        private int _intNumberOfLines;


        [DataMember]
        private int _intMaximumLength;


        /// <summary>
        /// A reference to another parameter (that has a list of values with x numbers selected). The internal list of the parameter is then filtered to the selection of the referenced parameter list.
        /// This property is only relevant for multiselection lists.
        /// </summary>
        [DataMember]
        private int? _intReferenceParameterIdFilterBy;

        
        #region Properties


        public int Id
        {
            get { return _intId; }
            set { _intId = value; OnPropertyChanged("Id"); }
        }


        public string DisplayName
        {
            get { return _strDisplayName; }
            set { _strDisplayName = value; OnPropertyChanged("DisplayName"); }
        }


        public string ParameterType
        {
            get { return _strType; }
            set
            {
                _strType = value;
                OnPropertyChanged("ParameterType");
                OnPropertyChanged("IsSQLType");
                OnPropertyChanged("IsListType");
                OnPropertyChanged("IsValueType");
                OnPropertyChanged("IsFileType");
            }
        }


        public string ParameterName
        {
            get { return _parameterName; }
            set { _parameterName = value; OnPropertyChanged("ParameterName"); }
        }


        public string ReturnType
        {
            get { return _strReturnType; }
            set { _strReturnType = value; OnPropertyChanged("ReturnType"); }
        }


        public string SelectionMode
        {
            get { return _strSelectionMode; }
            set 
            { 
                _strSelectionMode = value; 
                OnPropertyChanged("SelectionMode");
                OnPropertyChanged("IsMultiSelect");
            }
        }


        public bool IsOptional
        {
            get { return _blnIsOptional; }
            set { _blnIsOptional = value; OnPropertyChanged("IsOptional"); }
        }


        public string Statement
        {
            get { return _strStatement; }
            set { _strStatement = value; OnPropertyChanged("Statement"); }
        }


        public int MaximumLength
        {
            get { return _intMaximumLength; }
            set { _intMaximumLength = value; OnPropertyChanged("MaximumLength"); }
        }


        public int NumberOfLines
        {
            get { return _intNumberOfLines; }
            set { _intNumberOfLines = value; OnPropertyChanged("NumberOfLines"); }
        }


        public ObservableCollection<ParameterListItem> List
        {
            get { return _list; }
            set { _list = value; OnPropertyChanged("List"); }
        }


        public bool IsSQLType
        {
            get { return ParameterType == "SQL"; }
        }


        public bool IsListType
        {
            get { return ParameterType == "List"; }
        }


        public bool IsValueType
        {
            get { return ParameterType == "Value"; }
        }


        public bool IsFileType
        {
            get { return ParameterType == "File"; }
        }



        public bool IsMultiSelect
        {
            get { return SelectionMode == "CheckBoxList" && (IsListType || IsSQLType); }
        }


        public int? ReferenceParameterIdFilterBy
        {
            get { return _intReferenceParameterIdFilterBy; }
            set { _intReferenceParameterIdFilterBy = value; OnPropertyChanged("ReferenceParameterIdFilterBy"); }
        }


        #endregion


        public ReportParameter()
        {
            _list = new ObservableCollection<ParameterListItem>();
            NumberOfLines = 1;
            MaximumLength = 0;
        }


        public static ReportParameter Load(XElement p)
        {
            if (p == null || p.Name != "Parameter")
                return null;

            ReportParameter rp = new ReportParameter();

            if(p.Attribute("id") != null)
                Int32.TryParse(p.Attribute("id").Value, out rp._intId);

            if (p.Element("DisplayName") != null)
                rp._strDisplayName = p.Element("DisplayName").Value;

            if (p.Element("Type") != null)
                rp._strType = p.Element("Type").Value;

            if (p.Element("ParamName") != null)
                rp._parameterName = p.Element("ParamName").Value;

            if (p.Element("ReturnType") != null)
                rp._strReturnType = p.Element("ReturnType").Value;

            if (p.Element("SelectionMode") != null)
                rp._strSelectionMode = p.Element("SelectionMode").Value;

            if (p.Element("IsOptional") != null)
                bool.TryParse(p.Element("IsOptional").Value, out rp._blnIsOptional);

            if (p.Element("Statement") != null)
                rp._strStatement = p.Element("Statement").Value;

            if(p.Element("MaximumLength") != null)
                p.Element("MaximumLength").Value.TryParseInt32(out rp._intMaximumLength);

            if (p.Element("NumberOfLines") != null)
                p.Element("NumberOfLines").Value.TryParseInt32(out rp._intNumberOfLines);

            if (p.Element("ReferenceParameterIdFilterBy") != null)
            {
                int intId = 0;
                if (p.Element("ReferenceParameterIdFilterBy").Value.TryParseInt32(out intId))
                    rp._intReferenceParameterIdFilterBy = intId;
            }

            if(p.Element("List") != null && p.Element("List").Elements().Where(x => x.Attribute("displayName") != null).Any())
                rp._list = p.Element("List").Elements().Select(x => new ParameterListItem(x.Attribute("displayName").Value, x.Value)).ToObservableCollection();

            return rp;
        }


        public XElement ToXml()
        {
            return ToXml(this);
        }

        public static XElement ToXml(ReportParameter p)
        {
            XElement xeList = new XElement("List");

            if (p._list != null)
            {
                foreach (var li in p._list)
                    xeList.Add(new XElement("ListItem", new XAttribute("displayName", li.DisplayName), li.Value));
            }


            XElement xeParameter = new XElement("Parameter", new XAttribute("id", p._intId),
                                                             new XElement("DisplayName", p._strDisplayName),
                                                             new XElement("Type", p._strType),
                                                             new XElement("ParamName", p._parameterName),
                                                             new XElement("ReturnType", p._strReturnType),
                                                             new XElement("SelectionMode", p._strSelectionMode),
                                                             new XElement("IsOptional", p._blnIsOptional),
                                                             new XElement("MaximumLength", p._intMaximumLength),
                                                             new XElement("NumberOfLines", p._intNumberOfLines),
                                                             new XElement("ReferenceParameterIdFilterBy", p._intReferenceParameterIdFilterBy.HasValue ? p._intReferenceParameterIdFilterBy.Value.ToString() : ""),
                                                             new XElement("Statement", p._strStatement),
                                                             xeList
                                               );

            return xeParameter;
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion
    }
}
