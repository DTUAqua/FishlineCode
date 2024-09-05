using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Anchor.Core;

namespace Babelfisk.Entities
{
    [DataContract]
    public class ReportFile : INotifyPropertyChanged, IDataErrorInfo
    {
        [DataMember]
        private string _strFileName;

        [DataMember]
        private byte[] _arrDecompressedData;

        [DataMember]
        private byte[] _arrCompressedData;


        #region Properties


        public string FileName
        {
            get { return _strFileName; }
            set
            {
                _strFileName = value;
                OnPropertyChanged("FileName");
            }
        }

        public byte[] DecompressedData
        {
            get { return _arrDecompressedData; }
            set
            {
                _arrDecompressedData = value;
                OnPropertyChanged("DecompressedData");
                OnPropertyChanged("SizeInKilobytes");
            }
        }

        public byte[] CompressedData
        {
            get { return _arrCompressedData; }
            set
            {
                _arrCompressedData = value;
                OnPropertyChanged("CompressedData");
                OnPropertyChanged("SizeInKilobytes");
            }
        }


        public double SizeInKilobytes
        {
            get { return DecompressedData != null ? DecompressedData.Length * (1.0 / 1024.0) : (CompressedData != null ? CompressedData.Length * (1.0 / 1024.0) : 0); }
        }


        #endregion


        public void CompressData(bool blnClearDecompressedDataOnSuccess = false)
        {
            try
            {
                if (_arrDecompressedData != null && _arrDecompressedData.Length > 0)
                    _arrCompressedData = _arrDecompressedData.Compress();

                if (blnClearDecompressedDataOnSuccess)
                    _arrDecompressedData = null;
            }
            catch { }
        }

        public void DecompressData(bool blnClearCompressedDataOnSuccess = false)
        {
            try
            {
                if (_arrCompressedData != null && _arrCompressedData.Length > 0)
                    _arrDecompressedData = _arrCompressedData.Decompress();

                if (blnClearCompressedDataOnSuccess)
                    _arrCompressedData = null;
            }
            catch { }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion


        #region IDataErrorInfo Members

        private string _strError = null;
        private bool _blnValidate = false;
        private bool _blnSetManuelError = false;

        public string Error
        {
            get { return _strError; }
            private set
            {
                _strError = value;
                OnPropertyChanged("Error");
                OnPropertyChanged("HasErrors");
            }
        }


        public virtual bool HasErrors
        {
            get { return !String.IsNullOrEmpty(Error); }
        }

        public string this[string columnName]
        {
            get
            {
                if (!_blnValidate)
                    return null;

                //If setting an error manually, it is needed to be returned.
                if (_blnSetManuelError && !string.IsNullOrWhiteSpace(Error))
                    return Error;

                switch (columnName)
                {
                    case "FileName":
                        if (string.IsNullOrWhiteSpace(FileName))
                            return "Angiv venligst filnavn.";

                        string strExt = System.IO.Path.GetExtension(FileName);
                        if (string.IsNullOrWhiteSpace(strExt))
                            return "Angiv venligst filtype (.csv, .txt, .doc)";
                        break;
                }

                return null;
            }
        }


        /// <summary>
        /// Set an error of the object manually.
        /// </summary>
        public void SetError(string strError)
        {
            _blnValidate = true;
            _blnSetManuelError = true;
            {
                Error = strError;
                OnPropertyChanged("FileName");
            }
            _blnSetManuelError = false;
            _blnValidate = false;
        }


        public virtual void ValidateAllProperties()
        {
            PropertyInfo[] arrPropInfo = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            Error = null;
            _blnValidate = true;
            if (arrPropInfo != null && arrPropInfo.Length > 0)
            {
                foreach (var prop in arrPropInfo)
                {
                    Error = this[prop.Name];

                    OnPropertyChanged(prop.Name);

                    if (Error != null)
                        break;
                }
            }
            _blnValidate = false;
        }

        #endregion
    }
}
