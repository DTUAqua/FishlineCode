using Babelfisk.Entities.FileEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.SmartDots.TreeViews
{
    public class FileTreeViewItem : AViewModel
    {
        private FolderTreeViewItem _folder;
        private FileSystemFile _fsf;

        private string _relativePath;

        private string _fileName;

        private bool _isChecked;


        #region Properties


        public string Path
        {
            get { return _relativePath; }
        }

        public string FileName
        {
            get { return _fileName; }
        }


        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }


        #endregion


        public FileTreeViewItem(FolderTreeViewItem folder, FileSystemFile file)
        {
            _folder = folder;
            _relativePath = file.Path;
            _fsf = file;

            _fileName = file.Name;
        }


    }
}
