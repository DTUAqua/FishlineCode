using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using Anchor.Core;

namespace Babelfisk.Entities
{
    [DataContract(IsReference=true)]
    public class DataVersioning
    {
        private string _strVersioningFilePath;

        private static object _objSaveLock = new object();

        [DataMember]
        private Dictionary<string, long> _dicVersions;


        public bool ContainsType(string strType)
        {
            if (_dicVersions == null)
                return false;

            return _dicVersions.ContainsKey(strType);
        }


        public long GetVersion(string strType)
        {
            if (!ContainsType(strType))
                return 0;

            return _dicVersions[strType];
        }


        private DataVersioning()
        {
            _dicVersions = new Dictionary<string, long>();
        }


        public static DataVersioning CreateEmpty()
        {
            return new DataVersioning();
        }

        public static DataVersioning LoadFromFile(string strVersioningFilePath)
        {
            DataVersioning dv = new DataVersioning();
           
            if (File.Exists(strVersioningFilePath))
            {
                try
                {
                    byte[] arr = File.ReadAllBytes(strVersioningFilePath);

                    dv = arr.ToObjectDataContract<DataVersioning>();
                }
                catch(Exception e)
                {
                    Anchor.Core.Loggers.Logger.LogError(e);
                }
            }

            dv._strVersioningFilePath = strVersioningFilePath;

            return dv;
        }


        public void Set(string strType, long lngVersion)
        {
            if (!_dicVersions.ContainsKey(strType))
                _dicVersions.Add(strType, lngVersion);
            else
                _dicVersions[strType] = lngVersion;
        }

        public void IncrementAndSave(string strType)
        {
            if (!_dicVersions.ContainsKey(strType))
                _dicVersions.Add(strType, 0);

            _dicVersions[strType] = (_dicVersions[strType] + 1) % long.MaxValue;

            Save();
        }


        public void Save()
        {
            lock (_objSaveLock)
            {
                try
                {
                    byte[] arr = this.ToByteArrayDataContract();
                    File.WriteAllBytes(_strVersioningFilePath, arr);
                }
                catch(Exception e)
                {
                    Anchor.Core.Loggers.Logger.LogError(e, "Could not save versioning file: " + (_strVersioningFilePath == null ? "NULL" : _strVersioningFilePath));
                }
            }
        }
    }
}
