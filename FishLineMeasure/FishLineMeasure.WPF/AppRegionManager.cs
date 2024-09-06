using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Anchor.Core;
using Babelfisk.Entities.SprattusSecurity;
using FishLineMeasure.ViewModels;
using FishLineMeasure.WPF.Windows;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace FishLineMeasure.WPF
{
    public class AppRegionManager : NotificationObject, IAppRegionManager
    {
        private Users _user;

        public Users User
        {
            get { return _user; }
            set
            {
                _user = value;
                this.RefreshRegion();
                RaisePropertyChanged(() => User);
            }
        }

        /// <summary>
        /// Reference to KFish region manager.
        /// </summary>
        private IRegionManager _regionManager;

        private MainWindow _mainWindow;

        private LoadingWindow _loadingWindow = null;


        /// <summary>
        /// Retuns the Kfish RegionManager - used for loading views in different regions (however, the methods of this 
        /// class should be used for most tasks).
        /// </summary>
        public IRegionManager RegionManager
        {
            get { return _regionManager; }
        }



        public AppRegionManager(IRegionManager regionManager, MainWindow mw)
        {
            _regionManager = regionManager;
            _mainWindow = mw;
        }


        #region Message box methods


        public void ShowMessageBoxDefaultTimeout(string strMsg)
        {
            MessageWindow.Show(Application.Current.MainWindow, strMsg, 3);

        }

        public void ShowMessageBox(string strMsg, int? intAutoCloseTimeout = null)
        {
            MessageWindow.Show(Application.Current.MainWindow, strMsg, intAutoCloseTimeout);
        }

        public MessageBoxResult ShowMessageBox(string strMsg, MessageBoxButton enmButtons)
        {
            return MessageWindow.Show(Application.Current.MainWindow, strMsg, enmButtons);
        }

        public MessageBoxResult ShowMessageBox(object xaml, MessageBoxButton enmButtons)
        {
            return MessageWindow.Show(Application.Current.MainWindow, xaml, enmButtons);
        }

        #endregion


        #region Loading window methods


        public void ShowLoadingWindow(string strMessage, bool blnShowProgressBar = true)
        {
            if (_loadingWindow != null)
                HideLoadingWindow();

            _loadingWindow = LoadingWindow.Show(_mainWindow, strMessage, blnShowProgressBar);
        }


        public void HideLoadingWindow()
        {
            if (_loadingWindow != null)
            {
                _loadingWindow.Close();
                _loadingWindow = null;
            }
        }

        #endregion


        public void RefreshWindowTitle()
        {
            if (this._mainWindow != null)
                _mainWindow.UpdateTitle();
        }


        /// <summary>
        /// Clear specific regions. If no specific ones are specified, all are cleared.
        /// </summary>
        public void ClearRegions(params RegionName[] regions)
        {
            string[] arr = null;

            if (regions != null && regions.Length > 0)
                arr = regions.Select(x => x.ToString()).ToArray();

            foreach (var region in RegionManager.Regions)
            {
                if (arr != null && !arr.Contains(region.Name))
                    continue;

                var views = region.Views.ToList();

                foreach (var v in views)
                    region.Remove(v);
            }
        }


        /// <summary>
        /// Load a view associated to aDomainObject in the specified region enmRegionName.
        /// </summary>
        public bool LoadViewFromViewModel(RegionName enmRegionName, AViewModel aViewModel)
        {
            return LoadViewFromViewModelInternal(enmRegionName.ToString(), aViewModel);
        }


        /// <summary>
        /// Load a view from a view model T, in the specified region enmRegionName. The view model constructor called, is
        /// specified by the ConstructorParam parameters given. If no constructor exist for the specified arrConstructorArgs,
        /// an exception is thrown.
        /// </summary>
        public bool LoadViewFromViewModel<T>(RegionName enmRegionName, params ConstructorParam[] arrConstructorArgs) where T : AViewModel
        {
            return LoadViewFromViewModelInternal(enmRegionName.ToString(), ConstructViewModel<T>(arrConstructorArgs));
        }


        /// <summary>
        /// Loads view model in a new pop up window
        /// </summary>
        public void LoadWindowViewFromViewModel<T>(bool blnModal = false, params ConstructorParam[] arrConstructorArgs) where T : AViewModel
        {
            LoadViewFromViewModelInternal(RegionName.WindowRegion.ToString(), ConstructViewModel<T>(arrConstructorArgs));
        }


        /// <summary>
        /// Loads view for specified domain object in a new window.
        /// </summary>
        public void LoadWindowViewFromViewModel(AViewModel aViewModel, bool blnModal = false, string strWindowStyle = null)
        {
            LoadViewFromViewModelInternal(RegionName.WindowRegion.ToString(), aViewModel, false, blnModal, strWindowStyle);
        }


        /// <summary>
        /// Loads a view for a specified domain object in a new window (the first time). If this method is called
        /// a second time and the window i still open, the window will switch view to the new one.
        /// </summary>
        public void SwitchWindowViewFromViewModel(AViewModel aViewModel)
        {
            LoadViewFromViewModelInternal(RegionName.WindowRegion.ToString(), aViewModel, true);
        }


        /// <summary>
        /// Loads a view for a specified domain object type in a new window (the first time). If this method is called
        /// a second time and the window i still open, the window will switch view to the new one.
        /// </summary>
        public void SwitchWindowViewFromViewModel<T>(params ConstructorParam[] arrConstructorArgs) where T : AViewModel
        {
            LoadViewFromViewModelInternal(RegionName.WindowRegion.ToString(), ConstructViewModel<T>(arrConstructorArgs), true);
        }



        /// <summary>
        /// Refresh a specific region (or all regions if no region is supplied).
        /// </summary>
        public void RefreshRegion(RegionName? region = null, RefreshOptions r = null)
        {
            if (region != null)
            {
                foreach (var view in RegionManager.Regions[region.ToString()].ActiveViews)
                {
                    FrameworkElement uiElm = view as FrameworkElement;
                    RefreshView(uiElm, r ?? new RefreshOptions());
                }
            }
            else
            {
                foreach (var reg in RegionManager.Regions)
                {
                    foreach (var view in reg.ActiveViews)
                    {
                        FrameworkElement uiElm = view as FrameworkElement;
                        RefreshView(uiElm, r ?? new RefreshOptions());
                    }
                }
            }
        }


        public bool HasRegionUnsavedData(RegionName? region)
        {
            if (region != null)
            {
                foreach (var view in RegionManager.Regions[region.ToString()].ActiveViews)
                {
                    FrameworkElement uiElm = view as FrameworkElement;
                    var vm = uiElm.DataContext as AViewModel;

                    //Call refresh method
                    if (vm != null)
                        return vm.HasUnsavedData;
                }
            }

            return false;
        }



        public void InvalidateViews(RegionName? region = null)
        {
            if (region != null)
            {
                foreach (var view in RegionManager.Regions[region.ToString()].ActiveViews)
                {
                    FrameworkElement uiElm = view as FrameworkElement;

                    if (typeof(Infrastructure.IInvalidatable).IsAssignableFrom(uiElm.GetType()))
                        ((Infrastructure.IInvalidatable)uiElm).InvalidateView();

                }
            }
            else
            {
                foreach (var reg in RegionManager.Regions)
                {
                    foreach (var view in reg.ActiveViews)
                    {
                        FrameworkElement uiElm = view as FrameworkElement;

                        if (typeof(Infrastructure.IInvalidatable).IsAssignableFrom(uiElm.GetType()))
                            ((Infrastructure.IInvalidatable)uiElm).InvalidateView();
                    }
                }
            }
        }



        #region Private methods


        /// <summary>
        /// Refreshes view (both ContextBinding and by calling the refresh method of the viewmodel).
        /// </summary>
        private void RefreshView(FrameworkElement uiElm, ViewModels.RefreshOptions r)
        {
            //Refresh binding
            uiElm.DataContext = uiElm.DataContext;
            var vm = uiElm.DataContext as AViewModel;

            //Call refresh method
            if (vm != null)
                vm.Refresh(r);
        }



        /// <summary>
        /// Couple ViewModels with views
        /// </summary>
        private bool LoadViewFromViewModelInternal<T>(string enmRegionName, T aViewModel = null, bool blnSwitchViews = false, bool blnModal = false, string strStyle = null) where T : AViewModel
        {
            string strRegionName = enmRegionName.ToString();
            if (!blnSwitchViews && _regionManager.Regions.Where(x => x.Name == strRegionName).Count() == 0)
                throw new NullReferenceException(String.Format("Region with name {0} was not found.", strRegionName));

            Type t = aViewModel == null ? typeof(T) : aViewModel.GetType();

            bool blnLoaded = true;
            //Switch of view model type and load associated view. A TypeSwitch class is used, since the normal switch-statement
            //does not support type-switching (only by type string name, which is not desirable since it is not compile time validated).
            TypeSwitch.Do(t,
                          TypeSwitch.Case<ViewModels.Overview.OverviewViewModel>(() => SwitchView<Views.Overview.OverviewView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Overview.AddAndEditTripViewModel>(() => SwitchView<Views.Overview.AddAndEditTripView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Overview.AddAndEditStationViewModel>(() => SwitchView<Views.Overview.AddAndEditStationView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Lenghts.LenghtViewModel>(() => SwitchView<Views.Overview.LenghtWindow>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Lenghts.OrderViewModel>(() => SwitchView<Views.Overview.OrderView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Lenghts.AddRowViewModel>(() => SwitchView<Views.Overview.AddRowView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Menu.TopMenuViewModel>(() => SwitchView<Views.Menu.TopMenuView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Menu.BluetoothControlsViewModel>(() => SwitchView<Views.Menu.BluetoothControlsView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Menu.ConnectingViewModel>(() => SwitchView<Views.Menu.ConnectingView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.CustomControls.BoxCatagoryControlViewModel>(() => SwitchView<Views.CustomControls.BoxCatagoryControlView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Lookups.LookupsViewModel>(() => SwitchView<Views.Lookups.LookupsView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Settings.SettingsViewModel>(() => SwitchView<Views.Settings.SettingsView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Menu.BluetoothHelperViewModel>(() => SwitchView<Views.Menu.BluetoothHelperView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal,strStyle)),
                          TypeSwitch.Case<ViewModels.Export.ExpoertViewModel>(() => SwitchView<Views.Export.ExportView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Lenghts.AddOrderClassGroupViewModel>(() => SwitchView<Views.Lenghts.AddOrderClassGroupView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Export.FishLineLoginViewModel>(() => SwitchView<Views.Export.FishLineLoginView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Export.FishLineExportViewModel>(() => SwitchView<Views.Export.FishLineExportView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Case<ViewModels.Export.SelectSpeciesListViewModel>(() => SwitchView<Views.Export.SelectSpeciesListView>(enmRegionName, aViewModel, ref blnLoaded, blnSwitchViews, blnModal, strStyle)),
                          TypeSwitch.Default(() => new NullReferenceException(String.Format("ViewModel with type '{0}' does not have a view associated. Please add view model to following method: AnalyzerRegionManager->LoadViewFromViewModelInternal()", typeof(T).Name))));

            return blnLoaded;
        }


        /// <summary>
        /// Methods decides whether to pop a new window (based on enmRegionName) or to load
        /// the view in the specified region (if it is a region in the main view).
        /// </summary>
        private void SwitchView<T>(string enmRegionName, AViewModel aViewModel, ref bool blnLoaded, bool blnSwitchViews = false, bool blnModal = false, string strWindowStyle = null)
            where T : UserControl
        {
            var region = (RegionName)Enum.Parse(typeof(RegionName), enmRegionName);

            if (HasRegionUnsavedData(region))
            {
                var res = ShowMessageBox("Der er data som ikke er gemt, er du sikker på du vil forsætte uden at gemme?", System.Windows.MessageBoxButton.YesNo);

                if (res == System.Windows.MessageBoxResult.No)
                {
                    blnLoaded = false;
                    return;
                }
            }

            if (enmRegionName == RegionName.WindowRegion.ToString())
                PopWindow<T>(aViewModel, blnSwitchViews, blnModal, strWindowStyle);
            else
            {
                //   var vView = _regionManager.Regions[enmRegionName.ToString()].Views.OfType<FrameworkElement>().Where(x => x.GetType().Name == typeof(T).Name);
                /* var vView = _regionManager.Regions[enmRegionName.ToString()].Views.OfType<UserControl>().Where(x => ReferenceEquals(x.Content, aViewModel));
                 if (vView.Count() > 0 && blnSwitchViews)
                 {
                     _regionManager.Regions[enmRegionName.ToString()].Deactivate(vView.First());
                     _regionManager.Regions[enmRegionName.ToString()].Remove(vView.First());
                 }*/

                T uc;

                try
                {
                    uc = ConstructView<T>(aViewModel);
                }
                catch (Exception e)
                {
                    ShowMessageBox("An unexpected exception occured in SwitchView. " + e.Message + ", InnerException: " + (e.InnerException != null ? e.InnerException.Message : ""));
                    return;
                }

                List<UserControl> lst = _regionManager.Regions[enmRegionName.ToString()].Views.OfType<UserControl>().ToList();

                for (int i = 0; i < lst.Count; i++)
                {
                    var curItem = lst[i];

                    AViewModel avm = null;
                    if (curItem != null && (avm = curItem.DataContext as AViewModel) != null)
                    {
                        _regionManager.Regions[enmRegionName.ToString()].Deactivate(lst[i]);
                        _regionManager.Regions[enmRegionName.ToString()].Remove(lst[i]);

                        avm.Dispose();
                        lst[i] = null;

                        if (curItem is IDisposable)
                            (curItem as IDisposable).Dispose();
                    }
                }


                //Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.SystemIdle, new System.Windows.Threading.DispatcherOperationCallback(delegate { return null; }), null);
                RegionManager.Regions[enmRegionName.ToString()].Add(uc);
                RegionManager.Regions[enmRegionName.ToString()].Activate(uc);
            }
        }




        /// <summary>
        /// Construct a view model for an array of ContsructorParam (which defines which constructor of the view model to call).
        /// The view model to construct is of type T and calling this method with no arguments, invokes the default constructor.
        /// </summary>
        private T ConstructViewModel<T>(params ConstructorParam[] arrConstructorArgs)
        {
            Type[] arrTypes = new Type[] { };
            object[] arrValues = new object[] { };

            if (arrConstructorArgs.Length > 0)
            {
                arrTypes = arrConstructorArgs.Select(x => x.Type).ToArray();
                arrValues = arrConstructorArgs.Select(x => x.Value).ToArray();
            }

            var vmConstr = typeof(T).GetConstructor(arrTypes);
            var vm = vmConstr.Invoke(arrValues);
            return (T)vm;
        }



        /// <summary>
        /// Construct a view of type T and aDomainObject as view model (to the views DataContext).
        /// </summary>
        private T ConstructView<T>(AViewModel vm = null) where T : UserControl
        {
            var constr = typeof(T).GetConstructor(new Type[] { });
            T uc = constr.Invoke(null) as T;

            if (uc == null)
                return uc;

            uc.DataContext = vm;

            return uc;
        }


        /// <summary>
        /// Pops a window with the specified control
        /// </summary>
        private void PopWindow<T>(AViewModel aViewModel, bool blnSwitchViews, bool blnModal = false, string strWindowStyle = null) where T : UserControl
        {
            T View = null;

            try
            {
                View = ConstructView<T>(aViewModel);
            }
            catch (Exception e)
            {
                ShowMessageBox("An unexpected exception occured in SwitchView. " + e.Message + " InnerException: " + (e.InnerException != null ? e.InnerException.Message : ""));
                return;
            }

            if (blnSwitchViews)
            {
                RegionManager.Regions[RegionName.WindowRegion.ToString()].Add(View);
                RegionManager.Regions[RegionName.WindowRegion.ToString()].Activate(View);
            }
            else
            {
                Infrastructure.Behaviors.WindowWrapper ww = new Infrastructure.Behaviors.WindowWrapper(App.Current.MainWindow);

                Style ob = null;

                if (strWindowStyle != null)
                    ob = Application.Current.FindResource(strWindowStyle) as Style;
                else
                    ob = Application.Current.FindResource("WindowRegionStyle") as Style;

                ww.Content = View;
                ww.Owner = App.Current.MainWindow;
                // ww.Closed += this.ContentDialogClosed;
                ww.Window.PreviewKeyDown += (sender, e) => { AViewModel.StaticPreviewKeyDown(sender, e); };
                ww.Window.Closing += (obj, e) =>
                {
                    aViewModel.FireClosing(obj, e);
                    //Make sure main window is focused when a dialog is closed
                    Application.Current.FocusWindow();
                };

                ww.Closed += (obj, e) =>
                {
                    aViewModel.FireClosed(obj, e);
                    aViewModel.WindowCloseButtonClicked();

                    if (View is IDisposable)
                        (View as IDisposable).Dispose();

                    aViewModel.Dispose();
                };

                ww.Style = ob;

                if (blnModal)
                    ww.ShowDialog();
                else
                    ww.Show();
            }
        }


        #endregion



        /// <summary>
        /// Retrieve a resource in the Resources folder. GetResource("Images/Globe/Marker.png");
        /// </summary>
        public static Stream GetResource(string psResourceName)
        {
            Uri oUri = new Uri("pack://application:,,,/Resources/" + psResourceName, UriKind.RelativeOrAbsolute);
            return App.GetResourceStream(oUri).Stream;
        }


        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            List<T> foundChilds = new List<T>();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                T childType = child as T;
                if (childType == null)
                {
                    foreach (var other in FindVisualChildren<T>(child))
                        yield return other;
                }
                else
                {
                    yield return (T)child;
                }
            }
        }
    }
}
