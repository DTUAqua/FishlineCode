using Babelfisk.Entities.FileEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;

namespace Babelfisk.ViewModels.SmartDots.TreeViews
{
    public class FolderTreeViewItem : AViewModel
    {
        private bool _blnIsExpanded;
        private bool _blnIsSelected;
        private bool _blnIsVisible = true;
        private bool? _blnIsChecked = false;
        private bool _isLoadingFiles = false;
        private bool _filesLoaded = false;

        private bool _blnIsDummy;

        protected ObservableCollection<FolderTreeViewItem> _colChildren;

        protected List<FileTreeViewItem> _lstAllFiles;
        protected ObservableCollection<FileTreeViewItem> _colFilteredFiles;

        private FolderTreeViewItem _parent;

        private static readonly FolderTreeViewItem _dummyChild = new FolderTreeViewItem(null, null) { IsDummy = true };

        private FolderTreeViewModel _tree;

        private FileSystemItem _fsi;


        #region Properties


        public ObservableCollection<FolderTreeViewItem> Children
        {
            get { return _colChildren ?? (_colChildren = new ObservableCollection<FolderTreeViewItem>()); }
            protected set
            {
                _colChildren = value;
                RaisePropertyChanged(() => Children);
                RaisePropertyChanged(() => HasChildren);
            }
        }

        public ObservableCollection<FileTreeViewItem> Files
        {
            get { return _colFilteredFiles ?? (_colFilteredFiles = new ObservableCollection<FileTreeViewItem>()); }
            protected set
            {
                _colFilteredFiles = value;
                RaisePropertyChanged(() => Files);
                RaisePropertyChanged(() => HasFiles);
            }
        }


        public List<FileTreeViewItem> AllFiles
        {
            get { return _lstAllFiles; }
        }

        public bool HasFiles
        {
            get { return _lstAllFiles != null && _lstAllFiles.Count > 0; }
        }

        /// <summary>
        /// Reference to parent treeview item.
        /// </summary>
        public FolderTreeViewItem Parent
        {
            get { return _parent; }
        }


        public FolderTreeViewModel Tree
        {
            get { return _tree; }
        }


        public FileSystemDirectory Directory
        {
            get { return _fsi as FileSystemDirectory; }
        }


        /// <summary>
        /// Gets/sets whether the treeviewitem associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _blnIsExpanded; }
            set
            {
                SetIsExpanded(value);
            }
        }


        public bool HasChildren
        {
            get { return HasDummyChild || (_colChildren != null && _colChildren.Count > 0); }
        }


        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild
        {
            get { return this.Children != null && this.Children.Count == 1 && this.Children[0] == _dummyChild; }
        }



        /// <summary>
        /// A boolean value indicating whether the treeviewitem is checked/unchecked (when tree is rendered with a checkbox)
        /// </summary>
        public bool? IsChecked
        {
            get { return _blnIsChecked; }
            set
            {
                bool? blnOld = _blnIsChecked;
                _blnIsChecked = value;
                CheckedChanged(blnOld, value);
                RaisePropertyChanged(() => IsChecked);
            }
        }


    


        /// <summary>
        /// Get/Set whether the treeviewitem associated with this item is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _blnIsSelected; }
            set
            {
                _blnIsSelected = value;
                RaisePropertyChanged(() => IsSelected);
                if (_blnIsSelected)
                    OnSelected();
            }
        }


        /// <summary>
        /// Get/Set whether the treeviewitem associated with this item is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return _blnIsVisible; }
            set
            {
                _blnIsVisible = value;
                RaisePropertyChanged(() => IsVisible);
            }
        }



        public bool IsDummy
        {
            get { return _blnIsDummy; }
            private set { _blnIsDummy = value; }
        }


        public virtual string Header
        {
            get { return (_fsi == null || IsLoading) ? "Arbejder" : _fsi.Name; }
        }


        public bool IsLoadingFiles
        {
            get { return _isLoadingFiles; }
            set
            {
                _isLoadingFiles = value;
                RaisePropertyChanged(() => IsLoadingFiles);
            }
        }


        public bool FilesLoaded
        {
            get { return _filesLoaded; }
            set
            {
                _filesLoaded = value;
                RaisePropertyChanged(() => FilesLoaded);
            }
        }


        #endregion


        public FolderTreeViewItem(FolderTreeViewModel tree, FileSystemItem fi)
        {
            _fsi = fi;
            _tree = tree;

            if(fi is FileSystemDirectory && (fi as FileSystemDirectory).HasSubdirectories)
                AddDummy();
        }


        protected virtual void OnSelected()
        {
            if (_tree != null && IsSelected)
            {
                _tree.SelectedItem = this;

                if(_tree.LoadFilesWhenItemSelected && !_filesLoaded)
                {
                    LoadFilesAsync();
                }
            }
        }


        public void LoadFilesAsync()
        {
            IsLoadingFiles = true;
            Task.Factory.StartNew(LoadFiles).ContinueWith(t => new Action(() => { IsLoadingFiles = false; }).Dispatch());
        }


        private void LoadFiles()
        {
            try
            {
                var datRes = new BusinessLogic.SmartDots.SmartDotsManager();
                var lst = datRes.GetFolderContent(_fsi.Path, Entities.FileSystemType.File);
                //System.Threading.Thread.Sleep(200);
                var sn = new Anchor.Core.Comparers.StringNumberComparer();
                var col = lst.OfType<FileSystemFile>().OrderBy(x => x.Name, sn).Select(x => new FileTreeViewItem(this, x)).ToList();

                //Set collection before action, so this thread can access the new collection, after the LoadChildren method call.
                _lstAllFiles = col;

                new Action(() =>
                {
                    FilesLoaded = true;
                }).Dispatch();

                FilterFiles();
            }
            catch { }
        }

        
        public void FilterFiles()
        {
            try
            {
                ObservableCollection<FileTreeViewItem> col = new ObservableCollection<FileTreeViewItem>();

                string search = Tree.SearchStringName;

                if (string.IsNullOrWhiteSpace(search))
                    col = _lstAllFiles.ToObservableCollection();
                else
                    col = _lstAllFiles.Where(x => x.FileName != null && x.FileName.Contains(search, StringComparison.InvariantCultureIgnoreCase)).ToObservableCollection();

                new Action(() =>
                {
                    Files = col;
                }).Dispatch();
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }


        public void LoadChildren()
        {
            try
            {
                var datRes = new BusinessLogic.SmartDots.SmartDotsManager();
                var lst = datRes.GetFolderContent(_fsi.Path, Entities.FileSystemType.Directory);
                //System.Threading.Thread.Sleep(200);
                var sn = new Anchor.Core.Comparers.StringNumberComparer();
                var col = lst.OfType<FileSystemDirectory>().OrderBy(x => x.Name, sn).Select(x => new FolderTreeViewItem(_tree, x) { _parent = this, _blnIsChecked = IsChecked }).ToObservableCollection();

                //Set collection before action, so this thread can access the new collection, after the LoadChildren method call.
                _colChildren = col;
                new Action(() =>
                {
                    Children = col;
                }).Dispatch();
            }
            catch { }
        }


        protected virtual void CheckedChanged(bool? blnOldValue, bool? blnNewValue)
        {
            if (Parent != null)
                Parent.ChildCheckedStateChanged(blnNewValue);

            if (Children != null && Children.Count > 0)
            {
                foreach (var itm in Children)
                {
                    itm.SetChildIsChecked(blnNewValue.HasValue && blnNewValue.Value ? true : false);
                }
            }
        }

        public void SetChildIsChecked(bool? blnValue)
        {
            _blnIsChecked = blnValue;
            if (Children != null && Children.Count > 0)
            {
                foreach (var itm in Children)
                {
                    if(itm != null)
                        itm.SetChildIsChecked(blnValue);
                }
            }

            RaisePropertyChanged(() => IsChecked);
        }

        private void ChildCheckedStateChanged(bool? blnState)
        {
            if (Children != null)
            {
                int intCheckedCount = Children.Where(x => x.IsChecked.HasValue && x.IsChecked.Value).Count();
                if (intCheckedCount == Children.Count)
                    _blnIsChecked = true;
                else if (intCheckedCount > 0 || Children.Where(x => !x.IsChecked.HasValue).Count() > 0)
                    _blnIsChecked = null;
                else
                    _blnIsChecked = false;

                RaisePropertyChanged(() => IsChecked);
            }

            if (Parent != null)
                Parent.ChildCheckedStateChanged(blnState);
        }



        public Task SetIsExpanded(bool blnValue, bool loadChild = true)
        {
            if (blnValue != _blnIsExpanded)
            {
                _blnIsExpanded = blnValue;
                RaisePropertyChanged(() => IsExpanded);
            }

            // Expand all the way up to the root.
            if (_blnIsExpanded && _parent != null && !_parent.IsExpanded)
                _parent.IsExpanded = true;

            // Lazy load the child items, if necessary.
            if (loadChild && this.HasDummyChild && blnValue)
            {
                IsLoading = true;
                return Task.Factory.StartNew(LoadChildrenFailSafe).ContinueWith(t => new Action(() => { }).Dispatch());
                //this.Children.Remove(_dummyChild);
            }

            return Anchor.Core.GeneralExtensions.TaskFromResult(false);
        }


        private void LoadChildrenFailSafe()
        {
            try
            {
                this.LoadChildren();
            }
            catch (Exception e)
            {
                if (e is TimeoutException || e is EndpointNotFoundException)
                    DispatchMessageBox("Det var ikke muligt at oprette forbindelse til databasen.");
                else
                    DispatchMessageBox("En unventet fejl opstod. " + e.Message);

                IsExpanded = false;
            }
        }


        /// <summary>
        /// Add dummy loading node
        /// </summary>
        protected void AddDummy()
        {
            _colChildren = new ObservableCollection<FolderTreeViewItem>();
            _colChildren.Add(_dummyChild);
            RaisePropertyChanged(() => Children);
        }


        protected void SetHasChildren(bool blnHasChildren)
        {
            if (blnHasChildren)
            {
                if (_colChildren == null || _colChildren.Count == 0)
                    AddDummy();
            }
            else
            {
                if (_colChildren != null && _colChildren.Count > 0)
                {
                    _colChildren.Clear();
                    RaisePropertyChanged(() => Children);
                }
            }
        }

    }
}
