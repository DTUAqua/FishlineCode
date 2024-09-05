using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Babelfisk.Entities.Sprattus
{
    public partial class AnimalFile : OfflineEntity
    {
        //Keep a memory value of animal file type, so parsing the string value is not neccesary more than once.
        private AnimalFileType? _animalFileType;

        public AnimalFileType FileTypeEnum
        {
            get
            {
                if (!_animalFileType.HasValue)
                {
                    AnimalFileType enm = AnimalFileType.OtolithImage;

                    if (!string.IsNullOrWhiteSpace(fileType))
                        Enum.TryParse(fileType, out enm);

                    _animalFileType = enm;
                }

                return _animalFileType.Value;
            }
            set
            {
                fileType = value.ToString();
                _animalFileType = value;

                OnPropertyChanged("FileTypeEnum");
            }
        }


        private bool _blnIsLoading;

        public bool IsLoading
        {
            get { return _blnIsLoading; }
            set
            {
                _blnIsLoading = value;
                OnNavigationPropertyChanged("IsLoading");
            }
        }


        public string FullFilePath
        {
            get
            {
                return filePath ?? "";
            }
        }
    }
}
