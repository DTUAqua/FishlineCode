using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Babelfisk.Warehouse
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(DWMessageType))]
    public class DWMessage : INotifyPropertyChanged
    {
        [DataContract()]
        public enum DWMessageType
        {
            [EnumMember] Warning,
            [EnumMember] Error
        }

        [DataMember]
        private DWMessageType _enmMessageType;
        [DataMember]
        private string _strRecordType;
        [DataMember]
        private string _strRecordTypeId;
        [DataMember]
        private string _strDescription;
        [DataMember]
        private string _strOrigin;
        [DataMember]
        private int? _intCruiseId;

        private int _intIndex;

        [DataMember]
        public int Index
        {
            get { return _intIndex; }
            set
            {
                _intIndex = value;
                OnPropertyChanged("Index");
            }
        }


        public DWMessageType MessageType
        {
            get { return _enmMessageType; }
        }


        public string MessageTypeString
        {
            get { return _enmMessageType.ToString(); }
        }


        public string Origin
        {
            get { return _strOrigin; }
        }

        public string RecordType
        {
            get { return _strRecordType; }
        }

        public string RecordTypeId
        {
            get { return _strRecordTypeId; }
        }

        public string Description
        {
            get { return _strDescription; }
        }

        public int? CruiseId
        {
            get { return _intCruiseId; }
        }


        public static string GetSemicolonSeperatedHeader
        {
            get { return "#;Type;Oprindelse;Kilde;KildeId;Beskrivelse"; }
        }

        public string GetSemicolonSeperatedData
        {
            get { return string.Join(";", Index, MessageTypeString, Origin, RecordType, RecordTypeId, Description); }
        }


        public static DWMessage NewError(int? intCruiseId, string strOrigin, string strRecordType, string strRecordTypeId, string strDescription)
        {
            return new DWMessage() { _intCruiseId = intCruiseId, _enmMessageType = DWMessageType.Error, _strOrigin = strOrigin, _strRecordType = strRecordType, _strRecordTypeId = strRecordTypeId, _strDescription = strDescription };
        }

        public static DWMessage NewWarning(int? intCruiseId, string strOrigin, string strRecordType, string strRecordTypeId, string strDescription)
        {
            return new DWMessage() { _intCruiseId = intCruiseId, _enmMessageType = DWMessageType.Warning, _strOrigin = strOrigin, _strRecordType = strRecordType, _strRecordTypeId = strRecordTypeId, _strDescription = strDescription };
        }


        private DWMessage() { }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
