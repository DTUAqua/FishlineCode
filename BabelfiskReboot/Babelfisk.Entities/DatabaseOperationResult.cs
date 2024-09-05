using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Babelfisk.Entities
{
    [DataContract]
    public struct DatabaseOperationResult
    {
        private DatabaseOperationStatus m_opStatus;
        private string m_strMessage;
        private string m_strUIMessage;

        private Dictionary<string, string> _properties;


        public DatabaseOperationResult(DatabaseOperationStatus status)
            : this(status, "")
        {
        }

        public DatabaseOperationResult(DatabaseOperationStatus status, string strMassage)
            : this(status, strMassage, strMassage)
        {
        }

        public DatabaseOperationResult(DatabaseOperationStatus status, string strMassage, string strUIMessage)
            : this(status, strMassage, strUIMessage, null)
        {
        }

        public DatabaseOperationResult(DatabaseOperationStatus status, string strMassage, string strUIMessage, Dictionary<string, string> properties)
        {
            m_opStatus = status;
            m_strMessage = strMassage;
            m_strUIMessage = strUIMessage;
            _properties = properties;
        }


        public string GetProperty(string name)
        {
            if (_properties == null)
                return null;

            if (!_properties.ContainsKey(name))
                return null;

            return _properties[name];
        }


        public static DatabaseOperationResult CreateSuccessResult()
        {
            return new DatabaseOperationResult(DatabaseOperationStatus.Successful);
        }

        [DataMember]
        public DatabaseOperationStatus DatabaseOperationStatus
        {
            get { return m_opStatus; }
            set { m_opStatus = value; }
        }

        [DataMember]
        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        [DataMember]
        public String UIMessage
        {
            get { return m_strUIMessage; }
            set { m_strUIMessage = value; }
        }


        [DataMember]
        public Dictionary<string, string> Properties
        {
            get { return _properties; }
            set
            {
                _properties = value;
            }
        }
    }
}
