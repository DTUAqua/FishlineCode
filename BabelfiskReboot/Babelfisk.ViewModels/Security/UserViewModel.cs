using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.SprattusSecurity;
using Microsoft.Practices.Prism.Commands;
using System.Threading.Tasks;
using Anchor.Core;

namespace Babelfisk.ViewModels.Security
{
    public class UserViewModel : AViewModel
    {
        public event Action<UserViewModel> OnOKClicked;
        public event Action<UserViewModel> OnCancelClicked;

        private DelegateCommand _cmdOK;
        private DelegateCommand _cmdCancel;
        private DelegateCommand<RoleViewModel> _cmdEditRole;
        private DelegateCommand _cmdAddRole;

        private bool _blnIsNew;

        private Users _user;

        private List<RoleViewModel> _lstRoles;

        private string _strPassword;

        private string _strPasswordRepeat;

        private bool _blnIsLoadingRoles;

        private int? _intUserId;

        private bool _blnForcePasswordChange;


        #region Properties


        public Users UserEntity
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => UserEntity);
                RaisePropertyChanged(() => UserId);
                RaisePropertyChanged(() => UserName);
                RaisePropertyChanged(() => FirstAndLastName);
                RaisePropertyChanged(() => Password);
                RaisePropertyChanged(() => PasswordRepeat);
                RaisePropertyChanged(() => FirstName);
                RaisePropertyChanged(() => LastName);
                RaisePropertyChanged(() => Email);
                RaisePropertyChanged(() => IsActive);
            }
        }


        public bool IsNew
        {
            get { return _blnIsNew; }
            private set
            {
                _blnIsNew = value;
                RaisePropertyChanged(() => IsNew);
            }
        }


        public bool IsAdmin
        {
            get { return _user != null && _user.IsAdmin; }
        }


        public bool IsLoggedInUser
        {
            get { return _user != null && !IsNew && _user.UserId == AppRegionManager.User.UserId; }
        }

        public bool HasSecurityRights
        {
            get { return AppRegionManager.User.HasTask(Entities.SecurityTask.EditUsers); }
        }


        public int UserId
        {
            get { return _user.UserId; }
        }


        /// <summary>
        /// Is user account active
        /// </summary>
        public bool IsActive
        {
            get { return _user.IsActive; }
            set
            {
                if (_user.IsActive != value)
                {
                    _user.IsActive = value;
                    RaisePropertyChanged(() => IsActive);
                }
            }
        }


        /// <summary>
        /// String get/set property User entity FirstName field
        /// </summary>
        public string FirstName
        {
            get { return _user == null ? null : _user.FirstName; }
            set
            {
                if (_user.FirstName != value)
                {
                    _user.FirstName = value;
                    RaisePropertyChanged(() => FirstName);
                }
            }
        }


        /// <summary>
        /// String get/set property User entity FirstName field
        /// </summary>
        public string LastName
        {
            get { return _user == null ? null : _user.LastName; }
            set
            {
                if (_user.LastName != value)
                {
                    _user.LastName = value;
                    RaisePropertyChanged(() => LastName);
                }
            }
        }


        public string FirstAndLastName
        {
            get { return string.Format("{0} {1}", _user.FirstName, _user.LastName); }
        }



        /// <summary>
        /// String get/set property User entity Email field
        /// </summary>
        public string Email
        {
            get { return _user == null ? null : _user.email; }
            set
            {
                if (_user.email != value)
                {
                    _user.email = value;
                    RaisePropertyChanged(() => Email);
                }
            }
        }


        public string UserName
        {
            get { return _user == null ? null : _user.UserName; }
            set
            {
                if (_user.UserName != value)
                {
                    _user.UserName = value;
                    RaisePropertyChanged(() => UserName);
                }
            }
        }



        public string Password
        {
            get { return _strPassword; }
            set
            {
                if (_strPassword != value)
                {
                    _strPassword = value;
                    _user.Password = _strPassword;

                    RaisePropertyChanged(() => Password);
                }
            }
        }


        public string PasswordRepeat
        {
            get { return _strPasswordRepeat; }
            set
            {
                if (_strPasswordRepeat != value)
                {
                    _strPasswordRepeat = value;
                    RaisePropertyChanged(() => PasswordRepeat);
                }
            }
        }


        public string RoleString
        {
            get
            {
                return (_user == null || _user.Role.Count == 0) ? null : string.Join(", ", _user.Role.Select(x => x.Role1));
            }
        }

        public bool IsLoadingRoles
        {
            get { return _blnIsLoadingRoles; }
            set
            {
                _blnIsLoadingRoles = value;
                RaisePropertyChanged(() => IsLoadingRoles);
            }
        }


        public List<RoleViewModel> Roles
        {
            get 
            {
                if (_lstRoles == null && !_blnIsLoadingRoles)
                {
                    LoadRolesAsync();
                }

                return _lstRoles;
            }
            set
            {
                _lstRoles = value;
                RaisePropertyChanged(() => Roles);
            }
        }

        public string YesString
        {
            get { return string.Format(" ({0})", Translate("Common", "Yes")); }
        }

        public string NoString
        {
            get { return string.Format(" ({0})", Translate("Common", "No")); }
        }


        #endregion


        private UserViewModel()
        { 
            WindowWidth = 700;
            WindowHeight = 575;
            WindowTitle = Translater.Translate("UserView", "UserProfile");
        }

        public UserViewModel(int intUserId, bool blnForcePasswordChange = false) 
            : this()
        {
            _intUserId = intUserId;
            _blnForcePasswordChange = blnForcePasswordChange;
            InitializeAsync();
        }


        public UserViewModel(Users usr)
            : this()
        {
            if (usr == null)
            {
                UserEntity = new Users();
                UserEntity.PasswordHint = "";
                UserEntity.country_FK = 1; //Hardcode user to DK
                UserEntity.IsActive = true;
                IsNew = true;
            }
            else
                UserEntity = usr;
        }


        private void InitializeAsync()
        {
            IsLoading = true;

            _user = new Users();
           
            _blnIsNew = _intUserId == null;

            Task.Factory.StartNew(Initialize);
        }


        private void Initialize()
        {
            try
            {
                if (!_blnIsNew)
                {
                    BusinessLogic.SecurityManager secMan = new BusinessLogic.SecurityManager();
                    Users usr = secMan.GetUserById(_intUserId.Value);

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        UserEntity = usr;
                    }));
                }
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("{0}. {1}", Translater.Translate("Errors", "1"), e.Message));
            }
            finally
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadRolesAsync();
                    InitializeForEditing();
                    IsLoading = false;
                }));
            }
        }

        public void InitializeForEditing()
        {
            
        }


        private void LoadRolesAsync()
        {
            IsLoadingRoles = true;
            Task.Factory.StartNew(LoadRoles);
        }

        private void LoadRoles()
        {
            try
            {
                BusinessLogic.SecurityManager secMan = new BusinessLogic.SecurityManager();

                var lstRoles = secMan.GetRoles();
                List<RoleViewModel> lstRoleVMs = lstRoles.Select(r => new RoleViewModel(r, UserContainsRole(r))).ToList();

               (new Action(() =>
                {
                    Roles = lstRoleVMs;
                })).Dispatch();
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("{0}. {1}", Translater.Translate("Errors", "1"), e.Message));
            }
            finally
            {
                (new Action(() =>
                {
                    IsLoadingRoles = false;
                })).Dispatch();
            }
        }

        private bool UserContainsRole(Role role)
        {
            return _user != null && _user.Role != null && _user.Role.Where(x => x.RoleId_PK == role.RoleId_PK).Count() > 0;
        }


        #region OKCommand


        public DelegateCommand OKCommand
        {
            get
            {
                if (_cmdOK == null)
                    _cmdOK = new DelegateCommand(() => OK());

                return _cmdOK;
            }
        }


        public void OK()
        {
            if (HasErrors)
            {
                AppRegionManager.ShowMessageBox(Error);
                return;
            }

            try
            {
                string strError = null;

                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                    strError = Translater.Translate("UserView", "ErrorUserNameAndName");
                else if (UserName != null && UserName.Length > 10)
                    strError = "Brugernavn må ikke være længere end 10 tegn.";
                else if (FirstName != null && FirstName.Length > 50)
                    strError = "Fornavn må ikke være længere end 50 tegn.";
                else if (LastName != null && LastName.Length > 50)
                    strError = "Efternavn må ikke være længere end 50 tegn.";
                else if (string.IsNullOrWhiteSpace(Email))
                    strError = Translater.Translate("UserView", "ErrorEmail");
                else if (Email != null && Email.Length > 50)
                    strError = "Email må ikke være længere end 50 tegn.";
                else if ((_blnForcePasswordChange || IsNew) && _strPassword == null)
                    strError = Translater.Translate("UserView", "ErrorPassword");
                else if ((_blnForcePasswordChange || IsNew) && _strPasswordRepeat == null)
                    strError = Translater.Translate("UserView", "ErrorPasswordRepeat");
                else if ((_strPassword != null || _strPasswordRepeat != null) && _strPassword != _strPasswordRepeat)
                    strError = Translater.Translate("UserView", "ErrorPasswords");
                else if (_strPassword != null && _strPassword.Length < 6)
                    strError = Translater.Translate("UserView", "ErrorPasswordLength");
                else if (Roles == null || !Roles.Where(x => x.IsChecked).Any())
                    strError = Translater.Translate("UserView", "ErrorRoles");

                if (strError != null)
                {
                    AppRegionManager.ShowMessageBox(strError);
                    return;
                }

                if (_user.Role == null)
                    _user.Role = new TrackableCollection<Role>();

                if (!IsNew)
                    _user.MarkAsModified();

                //Add new roles (if any)
                foreach (RoleViewModel rvm in Roles)
                {
                    if (rvm.IsChecked && _user.Role.Where(x => x.RoleId_PK == rvm.Role.RoleId_PK).Count() == 0)
                        _user.Role.Add(rvm.Role);
                }

                var vRoles = _user.Role.ToList();

                //Remove unchecked roles
                foreach (var role in vRoles)
                    if (Roles.Where(x => x.Role.RoleId_PK == role.RoleId_PK && x.IsChecked).Count() == 0)
                        _user.Role.Remove(role);
            }
            catch (Exception e)
            {
                AppRegionManager.ShowMessageBox(string.Format(Translater.Translate("Common", "Error"), e.Message));
                return;
            }
            IsLoading = true;
            Task.Factory.StartNew(SaveUser).ContinueWith(SaveUserDone);
        }

        private void SaveUserDone(Task t)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                IsLoading = false;
            }));
        }


        private void SaveUser()
        {
            try
            {
                BusinessLogic.SecurityManager secMan = new BusinessLogic.SecurityManager();

                Users tmpUser = _user;

                if(!string.IsNullOrWhiteSpace(_strPassword))
                    tmpUser.Password = Entities.Hash.ComputeHash(_strPassword);

                Entities.DatabaseOperationResult dor = secMan.SaveChanges(tmpUser);

                if (dor.DatabaseOperationStatus == Entities.DatabaseOperationStatus.ValidationError)
                {
                    DispatchMessageBox(Translater.Translate("UserView", dor.Message));
                    return;
                }

                if (dor.DatabaseOperationStatus == Entities.DatabaseOperationStatus.Successful)
                {
                    //Make sure password is the same (if a new one has been specified).
                    if (IsLoggedInUser)
                    {
                        BusinessLogic.SecurityManager.CurrentUser.UserName = tmpUser.UserName;
                        BusinessLogic.SecurityManager.CurrentUser.Password = tmpUser.Password;
                    }

                    Users usr = secMan.GetUser(_user.UserName);

                    new Action(() =>
                    {
                        if (IsLoggedInUser)
                        {
                            AppRegionManager.User = usr;
                            //Refresh all regions
                            //AppRegionManager.RefreshRegion();
                        }

                        UserEntity = usr;
                        
                        _strPassword = null;
                        _strPasswordRepeat = null;
                        Roles = null;

                        AppRegionManager.RefreshRegion();

                        if (!HasSecurityRights)
                            AppRegionManager.ShowMessageBox(Translate("Common", "ChangesSavedMessage"), 2);
                    }).Dispatch();

                    //Update offline users.
                    Offline.OfflineStaticMethods.SyncUsersAsync(AppRegionManager);
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        AppRegionManager.ShowMessageBox(string.Format("{0} {1}",Translate("Errors","1"), dor.Message));
                    }));
                    return;
                }

                new Action(() =>
                {
                    _blnForcePasswordChange = false;
                    if (OnOKClicked != null)
                        OnOKClicked(this);

                    RefreshAllNotifiableProperties();
                }).Dispatch();
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("{0}. {1}", Translater.Translate("Errors", "1"), e.Message));
            }
        }




        #endregion


        #region Cancel Command


        public DelegateCommand CancelCommand
        {
            get
            {
                if (_cmdCancel == null)
                    _cmdCancel = new DelegateCommand(() => CancelAsync());

                return _cmdCancel;
            }
        }


        public void CancelAsync()
        {
            if (_blnForcePasswordChange)
            {
                AppRegionManager.ShowMessageBox(Translate("UserView", "InfoAddNewPasword"));
                return;
            }

            IsLoading = true;

            if (_blnIsNew)
            {
                if (OnCancelClicked != null)
                    OnCancelClicked(this);
            }
            else
                Task.Factory.StartNew(Cancel);
        }


        public void Cancel()
        {
            try
            {
                Users usr = null;

                if (!IsNew)
                {
                    BusinessLogic.SecurityManager secMan = new BusinessLogic.SecurityManager();
                    usr = secMan.GetUserById(_user.UserId);
                }
                
                (new Action(() =>
                {
                    UserEntity = usr;

                    _strPassword = null;
                    _strPasswordRepeat = null;
                    Roles = null;

                    if (OnCancelClicked != null)
                        OnCancelClicked(this);

                })).Dispatch();
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("{0}. {1}", Translater.Translate("Errors", "1"), e.Message));
            }
            finally
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsLoading = false;
                }));
            }
        }

        #endregion


        #region Add Role Command


        public DelegateCommand AddRoleCommand
        {
            get
            {
                if (_cmdAddRole == null)
                    _cmdAddRole = new DelegateCommand(() => AddRole());

                return _cmdAddRole;
            }
        }


        public void AddRole()
        {
            var rvm = new RoleViewModel(null);
            rvm.InitializeForAddEditAsync(RoleSaved);
            AppRegionManager.LoadWindowViewFromViewModel(rvm, true, "WindowToolBox");
        }


        #endregion


        #region Role Edit Command


        public DelegateCommand<RoleViewModel> EditRoleCommand
        {
            get { return _cmdEditRole ?? (_cmdEditRole = new DelegateCommand<RoleViewModel>(p => EditRole(p))); }
        }


        public void EditRole(RoleViewModel rvm)
        {
            rvm.InitializeForAddEditAsync(RoleSaved);
            AppRegionManager.LoadWindowViewFromViewModel(rvm, true, "WindowToolBox");
        }


        private void RoleSaved(RoleViewModel rvm, ObjectState savedState)
        {
            if (savedState == ObjectState.Deleted && Roles.Where(x => x.Role.RoleId_PK == rvm.Role.RoleId_PK).Any())
            {
                Roles.Remove(rvm);
                Roles = Roles.ToList();
            }
            else if (savedState == ObjectState.Added)
            {
                Roles.Add(rvm);
                Roles = Roles.ToList();
            }

        }


        #endregion


        public override void FireClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            base.FireClosing(sender, e);

            if (_blnForcePasswordChange)
            {
                e.Cancel = true;
                AppRegionManager.ShowMessageBox(Translate("UserView", "InfoAddNewPasword"));
            }
        }
    }
}
