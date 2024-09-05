using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.SmartDots
{
    public class DirectoryPathItem : AViewModel
    {
        private string _pathString;
        private bool _isSelected;
        private SelectDirectoryPathViewModel _selectDirectoryPathVM;

        #region Properties

        public string PathString
        {
            get { return _pathString; }
            set
            {
                _pathString = value;
                RaisePropertyChanged(() => PathString);
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (_selectDirectoryPathVM != null)

                RaisePropertyChanged(() => IsSelected);
            }
        }

        #endregion

        public DirectoryPathItem(SelectDirectoryPathViewModel vmso, string path)
        {
            _selectDirectoryPathVM = vmso;

            _pathString = path;
            _isSelected = false;

        }

        public void SetIsSelected(bool val)
        {
            _isSelected = val;
            RaisePropertyChanged(() => IsSelected);
        }
    }
}
