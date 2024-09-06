using FishLineMeasure.ViewModels.Infrastructure;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using FishLineMeasure.ViewModels.CustomControls;
using Anchor.Core;
using System.Collections.Generic;
using FishLineMeasure.ViewModels.Lookups;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Lenghts
{
    public class OrderViewModel : AViewModel
    {
        private DelegateCommand _cmdSave;
        private DelegateCommand<OrderClass> _cmdDelete;
        private DelegateCommand _cmdAdd;
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdAddGroup;
        private DelegateCommand _cmdDeleteGroup;

        private ObservableCollection<OrderClass> _ordersList;

        private ObservableCollection<OrderClassGroup> _groupsList;

        private OrderClassGroup _selectedGroup;


        #region Properties 


        public ObservableCollection<OrderClassGroup> GroupsList
        {
            get { return _groupsList; }
            set
            {
                _groupsList = value;
                RaisePropertyChanged(() => GroupsList);
            }
        }


        public OrderClassGroup SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                var old = _selectedGroup;
                _selectedGroup = value;
                RaisePropertyChanged(() => SelectedGroup);

                //Make sure to save any changes to editable list, before changing to a saved one.
                if (old != null && old.IsEditable)
                    old.OrderClasses = OrdersList.ToList();

                //Set the new list to be viewed
                if(_selectedGroup != null)
                    OrdersList = _selectedGroup.OrderClasses.ToObservableCollection();

                RaisePropertyChanged(() => IsGroupEditable);
            }
        }


        public bool IsGroupEditable
        {
            get { return _selectedGroup != null && _selectedGroup.IsEditable; }
        }


     


        public ObservableCollection<OrderClass> OrdersList
        {
            get { return _ordersList; }
            set
            {
                if(_ordersList != null)
                    OrdersList.CollectionChanged -= OrdersList_CollectionChanged;

                _ordersList = value;

                if (_ordersList != null)
                    OrdersList.CollectionChanged += OrdersList_CollectionChanged;

                RaisePropertyChanged(() => OrdersList);
            }
        }

        #endregion

        public OrderViewModel()
        {
            WindowTitle = "Vælg rækkefølge af længdegrupper";
            WindowWidth = 700;
            WindowHeight = 600;
            MinWindowHeight = 400;
            MinWindowWidth = 500;

            AdjustWindowWidthHeightToScreen();

            IsDirty = false;
        }

        private void OrdersList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Make sure orderslist i raised, so the UI number order is updated.
            RaisePropertyChanged(() => OrdersList);
        }

        public Task InitializeAsync()
        {
            IsLoading = true;

            return Task.Run(() =>
            {
                try
                {
                    var lstGroups = LoadLengthGroupsCollectionFromSettings().OrderBy(x => !x.IsEditable).ThenBy(x => x.Name).ToList();

                    var selectedGroupName = BusinessLogic.Settings.Settings.Instance.SelectedLengthGroupName;

                    var editableGroupName = "<Standard>";
                    var editableGroup = lstGroups.Where(x => x.IsEditable).FirstOrDefault();
                    if (editableGroup == null)
                    {
                        editableGroup = new OrderClassGroup() { Name = editableGroupName, OrderClasses = new List<OrderClass>(), IsEditable = true };
                        lstGroups.Insert(0, editableGroup);
                    }
                    else
                        editableGroup.Name = editableGroupName;

                   OrderClassGroup selectedGroup = null;

                    if (!string.IsNullOrWhiteSpace(selectedGroupName))
                        selectedGroup = lstGroups.Where(x => x.Name != null && x.Name.Equals(selectedGroupName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    if (selectedGroup == null)
                        selectedGroup = lstGroups.FirstOrDefault();

                    new Action(() =>
                    {
                        GroupsList = lstGroups.ToObservableCollection();
                        SelectedGroup = selectedGroup;

                        OrdersList = selectedGroup.OrderClasses.ToObservableCollection();
                    }).Dispatch();
                }
                catch(Exception e)
                {
                    LogError(e);
                    DispatchMessageBox("En uventet fejl opstod. " + e.Message);
                }
            })
            .ContinueWith(t => new Action(() =>
            {
                IsLoading = false;
            }).Dispatch());
        }


        public static List<OrderClassGroup> LoadLengthGroupsCollectionFromSettings()
        {
            var lst = new List<OrderClassGroup>();

            try
            {
                var lg = BusinessLogic.Settings.Settings.Instance.LengthGroupsCollection;

                if (string.IsNullOrWhiteSpace(lg))
                    return lst;

                var arr = System.Text.Encoding.UTF8.GetBytes(lg);
                var lstTmp = arr.ToObjectDataContract<List<OrderClassGroup>>(new Type[] { typeof(OrderClass), typeof(LookupItemViewModel), typeof(AViewModel), typeof(OrderClassGroup) });

                if (lstTmp != null)
                    lst = lstTmp;
            }
            catch(Exception e)
            {
                LogError(e);
            }

            return lst;
        }


        public static bool SaveLengthGroupsCollectionToSettings(IEnumerable<OrderClassGroup> lst)
        {
            bool res = false;

            try
            {
                if (lst == null)
                    BusinessLogic.Settings.Settings.Instance.LengthGroupsCollection = null;
                else
                {
                    byte[] arr = lst.ToList().ToByteArrayDataContract();
                    string str = System.Text.Encoding.UTF8.GetString(arr);

                    BusinessLogic.Settings.Settings.Instance.LengthGroupsCollection = str;
                }

                res = true;
            }
            catch (Exception e)
            {
                AViewModel.LogError(e);
            }

            return res;
        }


        #region Add Row Command


        public DelegateCommand AddRowCommand
        {
            get { return _cmdAdd ?? (_cmdAdd = new DelegateCommand(AddRow)); }
        }


        private void AddRow()
        {
          //  Catagories.ToList().ForEach(x => x.Options.ForEach(xx => xx.IsChecked = false));
            var vmRows = new AddRowViewModel();
            vmRows.InitializeAsync();
            AppRegionManager.LoadWindowViewFromViewModel(vmRows, true, "WindowToolBox");
            if (vmRows.IsDirty == true)
            {
                var lstSelectedLookups = vmRows.LookupLists.Where(x => x.HasSelectedLookup).Select(x => x.SelectedLookup).ToList();
                OrdersList.Add(new OrderClass(lstSelectedLookups));
            } 
        }


        #endregion


        #region Save Command


        public DelegateCommand SaveCommand
        {
            get { return _cmdSave ?? (_cmdSave = new DelegateCommand(Save)); }
        }


        /// <summary>
        /// Save length groups to settings.
        /// </summary>
        private void Save()
        {
            if (SelectedGroup != null)
            {
                SelectedGroup.OrderClasses = OrdersList.ToList();
                BusinessLogic.Settings.Settings.Instance.SelectedLengthGroupName = SelectedGroup.Name;
            }

            SaveLengthGroupsCollectionToSettings(GroupsList);

            IsDirty = true;
            Close();
        }



        #endregion


        #region Cancel Command

        public DelegateCommand CancelCommand
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(CancelThis)); }
        }

        private void CancelThis()
        {
            this.Close();
        }

        #endregion


        #region Delete Row Command

        public DelegateCommand<OrderClass> DeleteRowCommand
        {
            get { return _cmdDelete ?? (_cmdDelete = new DelegateCommand<OrderClass>(oc => DeleteRow(oc))); }
        }

        private void DeleteRow(OrderClass oc)
        {
            if (oc == null)
                return;

            if (AppRegionManager.ShowMessageBox(string.Format("Er du sikker på du vil fjerne gruppen '{0}'?", oc.GroupString), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            OrdersList.Remove(oc);
        }

        #endregion


        #region Edit Row Command


        #endregion


        #region Add Group Command

        public DelegateCommand AddGroupCommand
        {
            get { return _cmdAddGroup ?? (_cmdAddGroup = new DelegateCommand(AddGroup)); }
        }

        private void AddGroup()
        {
            if(OrdersList.Count == 0)
            {
                AppRegionManager.ShowMessageBox("Tilføj venligst mindst en længdefordeling, inden du gemmer rækkefølgen under et navn.");
                return;
            }


            AddOrderClassGroupViewModel vm = new AddOrderClassGroupViewModel(this);
            vm.WindowTitle = "Gem rækkefølge under et navn";
            AppRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowToolBox");

            if (vm.IsCanceled)
                return;

            SelectedGroup.OrderClasses = OrdersList.ToList();

            var og = new OrderClassGroup() { Name = vm.Name, OrderClasses = OrdersList.ToList() };
            GroupsList.Add(og);
            GroupsList = GroupsList.OrderBy(x => !x.IsEditable).ThenBy(x => x.Name).ToObservableCollection();

            SelectedGroup = og;
            // SaveLengthGroupsCollectionToSettings(GroupsList);
        }

        #endregion


        #region Delete Group Command

        public DelegateCommand DeleteGroupCommand
        {
            get { return _cmdDeleteGroup ?? (_cmdDeleteGroup = new DelegateCommand(DeleteGroup)); }
        }

        private void DeleteGroup()
        {
            if (SelectedGroup == null)
            {
                AppRegionManager.ShowMessageBox("Vælg venligst en gemt rækkefølge fra nedrulningslisten, inden du sletter.");
                return;
            }

            if (AppRegionManager.ShowMessageBox(string.Format("Er du sikker på den gemte rækkefølge med navn '{0}' skal slettes?", SelectedGroup.Name ?? ""), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            GroupsList.Remove(SelectedGroup);
            SelectedGroup = GroupsList.FirstOrDefault();

            SaveLengthGroupsCollectionToSettings(GroupsList);

            AppRegionManager.ShowMessageBox("Den gemte rækkefølge blev slettet korrekt.", 3);
        }

        #endregion


        public override void Dispose()
        {
            try
            {
                base.Dispose();

                OrdersList.CollectionChanged -= OrdersList_CollectionChanged;
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }
    }
}
