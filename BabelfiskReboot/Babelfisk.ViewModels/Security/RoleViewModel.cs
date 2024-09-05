using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using Babelfisk.Entities.SprattusSecurity;
using System.Threading.Tasks;
using Anchor.Core;

namespace Babelfisk.ViewModels.Security
{
    public class RoleViewModel : AViewModel
    {
        private DelegateCommand _cmdSave;
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdDelete;

        private static List<KeyValuePair<string, string>> _lstLanguages = null;

        private Role _role;

        private bool _blnIsChecked;

        private List<FishLineTasks> _lstCheckableTasks;

        private Action<RoleViewModel, ObjectState> _saveCallback;


        public bool IsChecked
        {
            get { return _blnIsChecked; }
            set
            {
                _blnIsChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }


        public Role Role
        {
            get { return _role; }
            private set
            {
                _role = value;
                RaisePropertyChanged(() => Role);
            }
        }


        public List<FishLineTasks> CheckableTasks
        {
            get { return _lstCheckableTasks; }
            set
            {
                _lstCheckableTasks = value;
                RaisePropertyChanged(() => CheckableTasks);
            }
        }



        public bool IsNew
        {
            get { return _role != null && _role.ChangeTracker.State == ObjectState.Added; }
        }



        public string NameLocal
        {
            get
            {
                return _role == null ? null : _role.Role1;
            }
            set
            {
                if (_role != null)
                {
                    _role.Role1 = value;
                    RaisePropertyChanged(() => NameLocal);
                }
            }
        }


        public bool IsAdministratorRole
        {
            get { return _role.RoleId_PK == 4; } //Is administrator role
        }



        public List<FishLineTasks> Tasks
        {
            get { return _role.FishLineTasks == null ? null : _role.FishLineTasks.OrderBy(x => TranslateTask(x.Value)).ToList(); }
        }

        public RoleViewModel(Role role, bool blnIsChecked = false)
        {
            _role = role;

            _blnIsChecked = blnIsChecked;

            if (role == null)
                _role = new Role();

            WindowWidth = 500;
            WindowHeight = 450;
        }


        public void InitializeForAddEditAsync(Action<RoleViewModel, ObjectState> saveCallback)
        {
            _saveCallback = saveCallback;

            IsLoading = true;
            WindowTitle = IsNew ? "Ny rolle" : "Rediger rolle";

            Task.Factory.StartNew(InitializeForAddEdit).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }

        private void InitializeForAddEdit()
        {
            List<FishLineTasks> lstTasks = null;
            try
            {
                var man = new Babelfisk.BusinessLogic.SecurityManager();

                lstTasks = man.GetTasks();

                if (lstTasks != null)
                {
                    lstTasks = lstTasks.OrderBy(x => TranslateTask(x.Value)).ToList();
                    lstTasks.ForEach(t => t.IsChecked = RoleContainsTask(t));
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                DispatchMessageBox(string.Format("En uventet fejl opstod. {0}.", e.Message));
            }

            new Action(() =>
            {
                CheckableTasks = lstTasks;
            }).Dispatch();
        }


        private string TranslateTask(string key)
        {
            string res = key ?? "";

            try
            {
                if (Translater.HasTranslation("BabelfiskTasks", key))
                    res = Translate("BabelfiskTasks", key);
            }
            catch { }

            return res;
        }


        private bool RoleContainsTask(FishLineTasks task)
        {
            return _role != null && _role.FishLineTasks != null && _role.FishLineTasks.Where(x => x.FishLineTaskId == task.FishLineTaskId).Count() > 0;
        }



        #region Save Command


        public DelegateCommand SaveCommand
        {
            get { return _cmdSave ?? (_cmdSave = new DelegateCommand(SaveAsync)); }
        }


        private void SaveAsync()
        {
            ValidateAllProperties(true);

            if (HasErrors)
                return;

            if (string.IsNullOrWhiteSpace(NameLocal))
            {
                Error = "Navngiv venligst rollen.";
                return;
            }

            //Give warning if no permissions have been selected.
            if (!CheckableTasks.Where(x => x.IsChecked).Any() && AppRegionManager.ShowMessageBox("Der er ikke valgt nogen rettigheder for rollen. Er du sikker på du vil fortsætte?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;


            IsLoading = true;
            Task.Factory.StartNew(Save).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }



        private void Save()
        {
            try
            {
                //Clone role, so if anything fails during update, nothing is lost.
                var role = _role.Clone();

                role.FishLineTasks.Clear();

                //Update tasks
                if (CheckableTasks.Where(x => x.IsChecked).Any())
                    role.FishLineTasks.AddRange(CheckableTasks.Where(x => x.IsChecked));

                var man = new Babelfisk.BusinessLogic.SecurityManager();

                var state = role.ChangeTracker.State;
                var res = man.UpdateRole(ref role);

                if (res.DatabaseOperationStatus != Entities.DatabaseOperationStatus.Successful)
                {
                    if(res.Message != null && res.Message.Contains("FK_RoleUserRole", StringComparison.InvariantCultureIgnoreCase))
                        DispatchMessageBox(string.Format("En eller flere brugere har rollen der prøves at slettes. Fjern venligst rollen fra alle brugere først og prøv igen."));
                    else if(res.Message != null && res.Message.Contains("DuplicateKey", StringComparison.InvariantCultureIgnoreCase))
                        DispatchMessageBox(string.Format("En rolle med samme navn eksisterer allerede. Rediger venligst navnet og prøv igen."));
                    else
                        DispatchMessageBox(string.Format("En uventet fejl opstod. {0}.", res.Message));
                }
                else
                {
                    new Action(() =>
                    {
                        if (state != ObjectState.Deleted)
                            Role = role;

                        if (_saveCallback != null)
                            _saveCallback(this, state);

                        Close();
                    }).Dispatch();
                    //Success
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                DispatchMessageBox(string.Format("En uventet fejl opstod. {0}.", e.Message));
            }
        }


        #endregion



        #region Cancel Command


        public DelegateCommand CancelCommand
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(Cancel)); }
        }


        private void Cancel()
        {
            Close();
        }


        #endregion


        #region Cancel Command


        public DelegateCommand DeleteCommand
        {
            get { return _cmdDelete ?? (_cmdDelete = new DelegateCommand(Delete)); }
        }


        private void Delete()
        {
            if (AppRegionManager.ShowMessageBox("Er du sikker på du vil slette rollen?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            _role.ChangeTracker.State = ObjectState.Deleted;

            IsLoading = true;
            Task.Factory.StartNew(Save).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        #endregion
    }
}
