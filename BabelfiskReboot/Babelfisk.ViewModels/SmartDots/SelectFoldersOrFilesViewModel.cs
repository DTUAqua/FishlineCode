using Babelfisk.ViewModels.SmartDots.TreeViews;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.SmartDots
{
    public class SelectFoldersOrFilesViewModel : AViewModel
    {
        private DelegateCommand _cmdOK;
        private DelegateCommand _cmdCancel;

        private bool _selectFolders;

        private bool _selectFiles;

        private bool _showFiles = false;

        private TreeViews.FolderTreeViewModel _folderTreeView;

        private bool _isCancelled;


        #region Properties


        public TreeViews.FolderTreeViewModel FoldersTree
        {
            get { return _folderTreeView; }
        }


        public bool SelectFolders
        {
            get { return _selectFolders; }
        }

        public bool ShowFiles
        {
            get { return _showFiles; }
            set
            {
                _showFiles = value;
                RaisePropertyChanged(() => ShowFiles);
            }
        }

        public bool SelectFiles
        {
            get { return _selectFiles; }
        }


        public bool IsCancelled
        {
            get { return _isCancelled; }
        }



        #endregion



        public SelectFoldersOrFilesViewModel(bool selectFolders = true, bool showFiles = false, bool selectFiles = true, string[] preselectFolders = null)
        {
            _selectFolders = selectFolders;
            _selectFiles = selectFiles;
            _showFiles = showFiles;
            _isCancelled = false;

            _folderTreeView = new TreeViews.FolderTreeViewModel(selectFolders, showFiles, preselectFolders);
        }


        public string[] GetSelectedFiles()
        {
            string[] res = null;

            List<string> filePaths = new List<string>();
            if (_folderTreeView.RootFolders != null)
            {
                foreach (var i in _folderTreeView.RootFolders)
                {
                    GetSelectedFiles(i, filePaths);
                }

                if (filePaths.Count > 0)
                    res = filePaths.ToArray();
            }

            return res;
        }


        private void GetSelectedFiles(FolderTreeViewItem ftvi, List<string> files)
        {
            if (ftvi.FilesLoaded && ftvi.HasFiles)
            {
                foreach (var f in ftvi.AllFiles)
                {
                    if (f.IsChecked)
                        files.Add(f.Path);
                }
            }

            if (ftvi.HasChildren && !ftvi.HasDummyChild)
            {
                foreach (var c in ftvi.Children)
                {
                    GetSelectedFiles(c, files);
                }
            }
        }


        public string[] GetSelectedFolders()
        {
            string[] res = null;

            List<string> folders = new List<string>();
            if (_folderTreeView.RootFolders != null)
            {
                foreach (var i in _folderTreeView.RootFolders)
                {
                    GetSelectedFolders(i, folders);
                }

                if(folders.Count > 0)
                    res = folders.ToArray();
            }

            return res;
        }


        private void GetSelectedFolders(FolderTreeViewItem ftvi, List<string> folders)
        {
            if (!ftvi.IsChecked.HasValue)
            {
                if (ftvi.HasChildren && !ftvi.HasDummyChild)
                {
                   // bool childChecked = false;
                    foreach (var c in ftvi.Children)
                    {
                        if (!c.IsChecked.HasValue || c.IsChecked.Value)
                        {
                            GetSelectedFolders(c, folders);
                            //childChecked = true;
                        }
                    }
                    //if (!childChecked && ftvi.Directory != null)
                    //    folders.Add(ftvi.Directory.Path);
                }
                /*else
                {
                    //If the folder is fully checked, add it.
                    if (ftvi.Directory != null && ftvi.IsChecked.HasValue && ftvi.IsChecked.Value)
                        folders.Add(ftvi.Directory.Path);
                }*/
            } else if(ftvi.IsChecked.Value)
            {
                //If the folder is fully checked, add it.
                if (ftvi.Directory != null && ftvi.IsChecked.HasValue && ftvi.IsChecked.Value)
                    folders.Add(ftvi.Directory.Path);
            }
        }


        #region OK Command


        public DelegateCommand OKCommand

        {
            get { return _cmdOK ?? (_cmdOK = new DelegateCommand(OK)); }
        }


        public void OK()
        {
            this.Close();
        }

        #endregion


        #region Cancel Command


        public DelegateCommand CancelCommand

        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(Cancel)); }
        }


        public void Cancel()
        {
            _isCancelled = true;
            this.Close();
        }

        #endregion
    }
}
