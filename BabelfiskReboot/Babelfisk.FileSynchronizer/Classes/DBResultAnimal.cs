using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.FileSynchronizer.Classes
{
    public class DBResultAnimal
    {
        public int AnimalId
        {
            get;
            set;
        }

        public int? AnimalFileId
        {
            get;
            set;
        }

        public string FilePath
        {
            get;
            set;
        }

        public bool? AutoAdded
        {
            get;
            set;
        }

        private string _strFileName = null;

        public string FileName
        {
            get 
            {
                if (_strFileName == null)
                    _strFileName = FilePath == null ? "" : System.IO.Path.GetFileName(FilePath);

                return _strFileName;
            }
        }


        public DBResultAnimal()
        {
        }


        public DBResultAnimal(int intAnimalId, string strPath, bool blnAutoAdded)
        {
            AnimalId = intAnimalId;
            FilePath = strPath;
            AutoAdded = blnAutoAdded;
            AnimalFileId = null;
        }
    }
}
