using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities
{
    [DataContract]
    public class ServiceResult
    {
        private DatabaseOperationStatus _opStatus;
        private string _strMessage;
        private string _strCode;
        private object _data;

        [DataMember]
        public DatabaseOperationStatus Result
        {
            get { return _opStatus; }
            set { _opStatus = value; }
        }

        [DataMember]
        public string Message
        {
            get { return _strMessage; }
            set { _strMessage = value; }
        }

        [DataMember]
        public string Code
        {
            get { return _strCode; }
            set { _strCode = value; }
        }


        [DataMember]
        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }



        public ServiceResult(DatabaseOperationStatus status)
            : this(status, null, null, null)
        { }

        public ServiceResult(DatabaseOperationStatus status, string strMassage)
            : this(status, strMassage, null, null)
        { }


        public ServiceResult(DatabaseOperationStatus status, string strMassage, object data)
             : this(status, strMassage, null, data)
        { }

        public ServiceResult(DatabaseOperationStatus status, string strMassage, string strCode, object data)
        {
            _opStatus = status;
            _strMessage = strMassage;
            _strCode = strCode;
            _data = data;
        }



        public static ServiceResult CreateSuccessResult()
        {
            return new ServiceResult(DatabaseOperationStatus.Successful);
        }


        
    }
}
