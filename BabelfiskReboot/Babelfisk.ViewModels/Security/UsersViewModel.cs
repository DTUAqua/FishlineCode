using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Anchor.Core;
using Babelfisk.Entities;

namespace Babelfisk.ViewModels.Security
{
    public class UsersViewModel : AViewModel
    {
        public event Action<UserViewModel> OnUserSelectedChanged;

        private List<UserViewModel> _lstUsers;

        private ICollectionView _colViewUsers;

        private UserViewModel _selectedUser;

        private DelegateCommand<UserViewModel> _cmdSelectUser;
        private DelegateCommand _cmdNewUser;
        private DelegateCommand<UserViewModel> _cmdDeleteUser;
        private DelegateCommand _cmdClose;

        private string _strSearchString;
        private bool _blnSortDescending;

        public List<UserViewModel> Users
        {
            get { return _lstUsers; }
            set
            {
                _lstUsers = value;
                RaisePropertyChanged(() => Users);
            }
        }


        public UserViewModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != null)
                {
                    _selectedUser.OnCancelClicked -= selectedUser_OnCancelClicked;
                    _selectedUser.OnOKClicked -= selectedUser_OnOKClicked;
                    _selectedUser.Dispose();
                }

                _selectedUser = value;

                if (_selectedUser != null)
                {
                    _selectedUser.OnCancelClicked += selectedUser_OnCancelClicked;
                    _selectedUser.OnOKClicked += selectedUser_OnOKClicked;
                    _selectedUser.InitializeForEditing();
                }

                RaisePropertyChanged(() => SelectedUser);
                RaisePropertyChanged(() => HasSelectedUser);

                if (OnUserSelectedChanged != null)
                    OnUserSelectedChanged(value);
            }
        }

        protected void selectedUser_OnOKClicked(UserViewModel obj)
        {
            if (_lstUsers.Where(x => x.UserEntity.UserId == obj.UserEntity.UserId).Count() == 0)
                _lstUsers.Add(obj);

            SelectedUser = null;
            SetCollectionView();
        }

        protected void selectedUser_OnCancelClicked(UserViewModel obj)
        {
            SelectedUser = null;
        }


        public bool HasSelectedUser
        {
            get { return SelectedUser != null; }
        }

       
        #region Filtering & Grouping properties


        public string SearchString
        {
            get { return _strSearchString; }
            set
            {
                _strSearchString = value;
                RaisePropertyChanged(() => SearchString);

                if (UsersCollectionView != null)
                    UsersCollectionView.Refresh();
            }
        }


        public ICollectionView UsersCollectionView
        {
            get { return _colViewUsers; }
            protected set
            {
                _colViewUsers = value;
                RaisePropertyChanged(() => UsersCollectionView);
            }
        }


        public List<KeyValuePair<string, string>> SortByItems
        {
            get
            {
                return new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(Translater.Translate("UsersView", "UserName"), "UserName"),
                    new KeyValuePair<string, string>(Translater.Translate("UsersView", "FullName"), "FirstAndLastName"),
                    new KeyValuePair<string, string>(Translater.Translate("UsersView", "Role"), "RoleString"),
                };
            }
        }


        public KeyValuePair<string, string>? SelectedSortByItem
        {
            get
            {
                string strMethod = BusinessLogic.Settings.Settings.Instance.UsersSortingMethod;

                var vItem = SortByItems.Where(x => x.Value == strMethod);

                if (strMethod == null || vItem.Count() == 0)
                    return null;

                return vItem.FirstOrDefault();
            }
            set
            {
                BusinessLogic.Settings.Settings.Instance.UsersSortingMethod = value.HasValue ? value.Value.Value : null;
                SetCollectionView();
                RaisePropertyChanged(() => SelectedSortByItem);
            }
        }


        public bool SortDescending
        {
            get { return _blnSortDescending; }
            set
            {
                _blnSortDescending = value;
                SetCollectionView();
                RaisePropertyChanged(() => SortDescending);
            }
        }


        #endregion




        public UsersViewModel()
        {
            WindowWidth = 800;
            WindowHeight = 600;
            WindowTitle = Translater.Translate("UsersView", "UsersHeader");

            InitializeAsync();
        }


        private void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(LoadUsers);
        }


        private void LoadUsers()
        {
            var users = new Babelfisk.BusinessLogic.SecurityManager().GetUsers();

            var userVMs = users.Select(x => new UserViewModel(x)).ToList();

            Users = userVMs;

            SetCollectionView();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                IsLoading = false;
            }));
        }


        private void SetCollectionView()
        {
            ICollectionView icol = CollectionViewSource.GetDefaultView(Users);

            using (icol.DeferRefresh())
            {
                //Apply filtering method
                icol.Filter = CollectionViewFilter;

                icol.GroupDescriptions.Clear();

                //Remove any existing sort descriptions
                icol.SortDescriptions.Clear();

                if (SelectedSortByItem != null /*&& SelectedSortByItem.Value.Value != "None"*/)
                    icol.SortDescriptions.Add(new SortDescription(SelectedSortByItem.Value.Value, _blnSortDescending ? ListSortDirection.Descending : ListSortDirection.Ascending));
                
                //Set culture invariant so sorting is not following danish rules (where "aa" means "å").
                icol.Culture = System.Globalization.CultureInfo.InvariantCulture;
            }

            Dispatcher.BeginInvoke(new Action(delegate()
            {
                UsersCollectionView = icol;

                //Set default sorting
                if (SelectedSortByItem == null)
                    SelectedSortByItem = SortByItems[0];
            }));
        }


        /// <summary>
        /// Method used for filtering trips.
        /// </summary>
        private bool CollectionViewFilter(object itm)
        {
            if (SearchString == "" || SearchString == null)
                return true;

            UserViewModel t = itm as UserViewModel;

            return t.UserName.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                   (t.FirstName != null && t.FirstName.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                    (t.LastName != null && t.LastName.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                   (t.RoleString != null && t.RoleString.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase));
        }


        #region Select User Command


        public DelegateCommand<UserViewModel> SelectUserCommand
        {
            get
            {
                if (_cmdSelectUser == null)
                    _cmdSelectUser = new DelegateCommand<UserViewModel>(u => SelectUser(u));

                return _cmdSelectUser;
            }
        }


        public void SelectUser(UserViewModel userVM)
        {
            SelectedUser = userVM;
        }

        #endregion


        #region Delete User Command


        public DelegateCommand<UserViewModel> DeleteUserCommand
        {
            get
            {
                if (_cmdDeleteUser == null)
                    _cmdDeleteUser = new DelegateCommand<UserViewModel>(u => DeleteUserAsync(u));

                return _cmdDeleteUser;
            }
        }


        public void DeleteUserAsync(UserViewModel userVM)
        {
            if (userVM.IsAdmin)
            {
                AppRegionManager.ShowMessageBoxDefaultTimeout("Administrator-konti kan ikke slettes.");
                return;
            }

            var vRes = AppRegionManager.ShowMessageBox(String.Format("Er du sikker på brugeren '{0}' skal fjernes fra systemet?", userVM.UserName), System.Windows.MessageBoxButton.YesNo);

            if (vRes == System.Windows.MessageBoxResult.No)
                return;

            IsLoading = true;

            Task.Factory.StartNew(new Action<object>(DeleteUser), userVM).ContinueWith(DeleteUserFinished);
        }


        private void DeleteUserFinished(Task t)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                IsLoading = false;
            }));
        }


        private void DeleteUser(object user)
        {
            UserViewModel userVM = user as UserViewModel;

            var secMan = new BusinessLogic.SecurityManager();
            bool blnCanDelete = secMan.CanDeleteUser(userVM.UserEntity);

            if (!blnCanDelete)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    AppRegionManager.ShowMessageBox(string.Format("'{0}' kan ikke fjernes fra systemet da brugeren er tilknyttet en eller flere fangster. Deaktiver brugeren i stedet, eller fjern fangsterne først.", userVM.UserName));
                }));
                return;
            }

            DatabaseOperationResult res = secMan.DeleteUser(userVM.UserEntity);

            if (res.DatabaseOperationStatus != Entities.DatabaseOperationStatus.Successful)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    AppRegionManager.ShowMessageBox("Brugeren kunne ikke slettes. En uventet fejl opstod. " + res.Message);
                }));
                return;
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                Users.Remove(userVM);
                SetCollectionView();
            }));
        }

        #endregion


        #region New User Command


        public DelegateCommand NewUserCommand
        {
            get
            {
                if (_cmdNewUser == null)
                    _cmdNewUser = new DelegateCommand(() => NewUser());

                return _cmdNewUser;
            }
        }


        public void NewUser()
        {
            SelectedUser = new UserViewModel(null);
        }

        #endregion


        #region Close Command

        public DelegateCommand CloseCommand
        {
            get
            {
                if (_cmdClose == null)
                    _cmdClose = new DelegateCommand(() => this.Close());

                return _cmdClose;
            }
        }

        #endregion
    }
}
