using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.Security
{
    public class BackupFile : AViewModel
    {
        private string _strFilePath;

        private DateTime _dtBackupDate;

        private string _strGroup;

        private object _objTag;


        #region Properties


        public string FilePath
        {
            get { return _strFilePath; }
        }


        public string FileName
        {
            get { return string.IsNullOrWhiteSpace(_strFilePath) ? "" : System.IO.Path.GetFileName(_strFilePath); }
        }


        public DateTime BackupDateUTC
        {
            get { return _dtBackupDate; }
        }

        public DateTime BackupDateLocal
        {
            get { return _dtBackupDate.ToLocalTime(); }
        }


        public string Group
        {
            get { return _strGroup; }
        }


        public object Tag
        {
            get { return _objTag; }
            set
            {
                _objTag = value;
                RaisePropertyChanged(() => Tag);
            }
        }


        #endregion



        public BackupFile(string strFilePath, DateTime backupDate, string strGroup = null)
        {
            _strFilePath = strFilePath;
            _dtBackupDate = backupDate;
            _strGroup = strGroup;
        }
    }
}
