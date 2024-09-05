using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.SmartDots
{
    public class OtolithFileItem : AViewModel
    {
        private string _imagepath;
        private string _imageDirectory;
        private string _imageName;
        private bool _isSelected;
        private SelectOtolithImagesViewModel _selectOtolithImagesVM;

        #region Properties

        public string ImagePath
        {
            get { return _imagepath; }
            set
            {
                _imagepath = value;
                RaisePropertyChanged(() => ImagePath);
            }
        }

        public string ImageDirectory
        {
            get { return _imageDirectory; }
            set
            {
                _imageDirectory = value;
                RaisePropertyChanged(() => ImageDirectory);
            }
        }

        public string ImageName
        {
            get { return _imageName; }
            set
            {
                _imageName = value;
                RaisePropertyChanged(() => ImageName);
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (_selectOtolithImagesVM != null)
                    _selectOtolithImagesVM.RaiseIsAllSelected();
                RaisePropertyChanged(() => IsSelected);
            }
        }

        #endregion

        public OtolithFileItem(SelectOtolithImagesViewModel vmso, string path)
        {
            _selectOtolithImagesVM = vmso;
            var directory = Path.GetDirectoryName(path);

            //Make sure directory has the same format as used else where (not starting with \ but ending with \)
            if(!string.IsNullOrWhiteSpace(directory))
            {
                if (directory.StartsWith(@"\"))
                    directory = directory.Substring(1);

                if (!directory.EndsWith(@"\"))
                    directory = directory + @"\";
            }
            var fileName = Path.GetFileName(path);

            _imagepath = path;
            _imageDirectory = !string.IsNullOrEmpty(directory) ? directory : "";
            _imageName = !string.IsNullOrEmpty(fileName) ? fileName : "";
            _isSelected = false;

        }

        public void SetIsSelected(bool val)
        {
            _isSelected = val;
            RaisePropertyChanged(() => IsSelected);
        }
    }
}
