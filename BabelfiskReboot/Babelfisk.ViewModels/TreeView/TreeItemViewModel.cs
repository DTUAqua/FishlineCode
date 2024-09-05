using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Babelfisk.ViewModels.TreeView.TreeItemFilters;
using System.ServiceModel;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;

namespace Babelfisk.ViewModels.TreeView
{
    public class TreeItemViewModel : AViewModel
    {
        private DelegateCommand _cmdDeleteEntity;

        private bool _blnIsExpanded;
        private bool _blnIsSelected;
        private bool _blnIsVisible = true;
        private bool? _blnIsChecked = false;

        protected ObservableCollection<TreeItemViewModel> _colChildren;

        private TreeItemViewModel _parent;

        private bool _blnIsDummy;

        protected ITreeItemFilter _treeItemFilter;

        protected object _item;

        private static readonly TreeItemViewModel _dummyChild = new TreeItemViewModel() { IsDummy = true };


        #region Properties


        public ObservableCollection<TreeItemViewModel> Children
        {
            get { return _colChildren ?? (_colChildren = new ObservableCollection<TreeItemViewModel>()); }
            protected set
            {
                _colChildren = value;
                RaisePropertyChanged(() => Children);
                RaisePropertyChanged(() => HasChildren);
            }
        }


        /// <summary>
        /// Reference to parent treeview item.
        /// </summary>
        public TreeItemViewModel Parent
        {
            get { return _parent; }
        }


        /// <summary>
        /// Flag indicating whether assigned entity can be deleted or not.
        /// </summary>
        public virtual bool CanDeleteEntity
        {
            get
            {
                return  User.HasTask(Entities.SecurityTask.DeleteData) && !BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline;
            }
        }


        /// <summary>
        /// Reference to the root of the tree view.
        /// </summary>
        public MainTreeViewModel TreeRoot
        {
            get
            {
                TreeItemViewModel vmParent = (this is YearTreeItemViewModel) ? this : Parent;
                while (vmParent != null && !(vmParent is YearTreeItemViewModel))
                    vmParent = vmParent.Parent;

                return (vmParent as YearTreeItemViewModel).Tree;
            }
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
            get { return "Arbejder"; }
        }


        #endregion



        /// <summary>
        /// Private delfault contstructor, forcing paramterized constructors.
        /// </summary>
        private TreeItemViewModel() { }

        protected TreeItemViewModel(TreeItemViewModel parent, bool hasChildren = true, ITreeItemFilter tripFilter = null)
        {
            _parent = parent;
            _treeItemFilter = tripFilter;
            _colChildren = new ObservableCollection<TreeItemViewModel>();

            if (hasChildren)
                AddDummy();

            if (parent != null && parent.IsChecked.HasValue && parent.IsChecked.Value)
                _blnIsChecked = true;
        }


     
        public Task SetIsExpanded(bool blnValue)
        {
            if (blnValue != _blnIsExpanded)
            {
                _blnIsExpanded = blnValue;
                RaisePropertyChanged(() => IsExpanded);
            }

            // Expand all the way up to the root.
            if (_blnIsExpanded && _parent != null)
                _parent.IsExpanded = true;

            // Lazy load the child items, if necessary.
            if (this.HasDummyChild && blnValue)
            {
                return Task.Factory.StartNew(LoadChildrenFailSafe);
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


        protected void SetChildIsChecked(bool? blnValue)
        {
            _blnIsChecked = blnValue;
            if (Children != null && Children.Count > 0)
            {
                foreach (var itm in Children)
                {
                    itm.SetChildIsChecked(blnValue);
                }
            }

            RaisePropertyChanged(() => IsChecked);
        }


        protected virtual void CheckedChanged(bool? blnOldValue, bool? blnNewValue)
        {
            if (Parent != null)
                Parent.ChildCheckedStateChanged(blnNewValue);

            if(Children != null && Children.Count > 0)
            {
                foreach (var itm in Children)
                {
                    itm.SetChildIsChecked(blnNewValue.HasValue && blnNewValue.Value ? true : false);
                }
            }
        }


        protected virtual void OnSelected()
        {

        }


        protected virtual void LoadChildren()
        {
        }


        public virtual void FilterAndSort(ITreeItemFilter treeFilter)
        {
            _treeItemFilter = treeFilter;

            if (Children != null)
                foreach (var c in Children)
                    c.FilterAndSort(treeFilter);
        }


        public Task RefreshTreeAsync()
        {
            return Task.Factory.StartNew(() => RefreshTree(_item));
        }


        public Task RefreshTreeAsync(object entity)
        {
            return Task.Factory.StartNew(() => RefreshTree(entity));
        }


        internal virtual void RefreshTree(object entity)
        {

        }


        /// <summary>
        /// Add dummy loading node
        /// </summary>
        protected void AddDummy()
        {
            _colChildren = new ObservableCollection<TreeItemViewModel>();
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


        #region Delete Entity Command


        public DelegateCommand DeleteEntityCommand
        {
            get { return _cmdDeleteEntity ?? (_cmdDeleteEntity = new DelegateCommand(DeleteEntityAsync)); }
        }


        protected virtual void DeleteEntityAsync()
        {
            if (!CanDeleteEntity)
            {
                AppRegionManager.ShowMessageBox("Du har ikke rettigheder til at slette.");
                return;
            }

            string strMsg = string.Format("{0} Hvis du ønsker at fortsætte, indtast venligst dit kodeord og klik OK.", DeleteConfirmationMessage);

            var vm = new Security.ReEnterPasswordViewModel();
            vm.Message = strMsg;

            AppRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderAutoHeightStyle");

            if (!vm.OKClicked)
                return;

            //Reset start view
            var vmStart = new StartViewModel();
            if (!_appRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vmStart))
                return;

            TreeRoot.IsDeletingEntity = true;
            Task.Factory.StartNew(DeleteEntitySync).ContinueWith(t => new Action(() => TreeRoot.IsDeletingEntity = false).Dispatch());
        }


        private void DeleteEntitySync()
        {
            try
            {
                string strMsg = "Objektet blev slettet korrekt.";
                var res = DeleteEntity(out strMsg);

                if (res.DatabaseOperationStatus == Entities.DatabaseOperationStatus.Successful)
                {
                    new Action(() =>
                    {
                        //Refresh tree
                        TreeRoot.RefreshTreeAsync();

                        AppRegionManager.ShowMessageBox(strMsg, 3);
                    }).Dispatch();
                }
                else
                    DispatchMessageBox("En uventet fejl opstod. " + res.Message);
            }
            catch (Exception e)
            {
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }
        }


        protected virtual string DeleteConfirmationMessage
        {
            get { return "Er du sikker på du vil slette det valgte?"; }
        }


        protected virtual Entities.DatabaseOperationResult DeleteEntity(out string strSuccessMessage)
        {
            strSuccessMessage = "Objektet blev slettet korrekt.";
            //Do nothing - have the inherited classes override this method.
            return Entities.DatabaseOperationResult.CreateSuccessResult();
        }


        #endregion
    }
}
