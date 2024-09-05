using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;

namespace Babelfisk.ViewModels.SmartDots.TreeViews
{
    public class FolderTreeViewModel : AViewModel
    {
        private ObservableCollection<FolderTreeViewItem> _colRootFolders;

        private bool _isCheckFoldersEnabled = false;

        private bool _loadFilesWhenSelected = false;

        private FolderTreeViewItem _selectedItem;

        private string _searchStringName;


        #region Properties


        public ObservableCollection<FolderTreeViewItem> RootFolders
        {
            get { return _colRootFolders; }
            set
            {
                _colRootFolders = value;
                RaisePropertyChanged(() => RootFolders);
            }
        }


        public bool IsCheckFoldersEnabled
        {
            get { return _isCheckFoldersEnabled; }
            set
            {
                _isCheckFoldersEnabled = value;
                RaisePropertyChanged(() => IsCheckFoldersEnabled);
            }
        }


        public FolderTreeViewItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
                RaisePropertyChanged(() => HasSelectedItem);
            }
        }

        public bool HasSelectedItem
        {
            get { return _selectedItem != null; }
        }

        public bool LoadFilesWhenItemSelected
        {
            get { return _loadFilesWhenSelected; }
        }


        public string SearchStringName
        {
            get { return _searchStringName; }
            set
            {
                _searchStringName = value;
                RaisePropertyChanged(() => SearchStringName);

                if (SelectedItem != null)
                    SelectedItem.FilterFiles();
            }
        }


        #endregion


        public FolderTreeViewModel(bool canCheckFolders, bool loadFilesWhenSelected, string[] preselectPaths = null)
        {
            _isCheckFoldersEnabled = canCheckFolders;
            _loadFilesWhenSelected = loadFilesWhenSelected;
            InitializeAsync(preselectPaths);
        }


        public Task InitializeAsync(string[] preselectPaths = null)
        {
            try
            {
                IsLoading = true;
            }
            catch (Exception e)
            {
                LogError(e);
            }

            var tt = Task.Factory.StartNew(() => Initialize(preselectPaths)).ContinueWith(t => new Action(() => { IsLoading = false; }).Dispatch());
            return tt;
        }


        private void Initialize(string[] preselectPaths = null)
        {
            try
            {
                var datRes = new BusinessLogic.SmartDots.SmartDotsManager();

                var lstRootFolders = datRes.GetFolderContent("", Entities.FileSystemType.Directory);
                var sn = new Anchor.Core.Comparers.StringNumberComparer();
                var col = lstRootFolders.OrderBy(x => x.Name, sn).Select(x => new FolderTreeViewItem(this, x)).ToObservableCollection();

                _colRootFolders = col;
                PreselectPaths(preselectPaths);

                new Action(() =>
                {
                    try
                    {
                        RootFolders = col;
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }).Dispatch();
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }


        private void PreselectPaths(string[] preselectPaths)
        {
            if(preselectPaths == null || preselectPaths.Length == 0 || _colRootFolders == null || _colRootFolders.Count == 0)
                return;

            try
            {
                foreach(var p in preselectPaths)
                {
                    string[] folders = p.Split(new char[] {'\\', '/', System.IO.Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);

                    if (folders == null || folders.Length == 0)
                        continue;

                    PreselectPath(folders, 0, _colRootFolders);
                }
            }
            catch { }
        }


        private void PreselectPath(string[] folders, int index, IEnumerable<FolderTreeViewItem> items)
        {
            try
            {
                if (index >= folders.Length)
                    return;

                string folder = folders[index];

                foreach (var i in items)
                {
                    var dir = i.Directory;
                    if (dir == null)
                        continue;

                    if(dir.Name != null && dir.Name.Equals(folder, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (dir.HasSubdirectories && !i.IsExpanded && (index + 1) < folders.Length)
                        {
                            i.LoadChildren();
                            i.SetIsExpanded(true, false);
                        }

                        if (index + 1 >= folders.Length)
                        {
                            if(_isCheckFoldersEnabled)
                                i.IsChecked = true;
                        }
                        else
                            PreselectPath(folders, index + 1, i.Children);
                        break;
                    }
                }
            }
            catch { }
        }


    }
}
