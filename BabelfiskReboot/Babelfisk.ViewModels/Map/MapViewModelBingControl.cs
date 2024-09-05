using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Babelfisk.BusinessLogic.Settings;
using Babelfisk.Entities.Sprattus;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;

namespace Babelfisk.ViewModels.Map
{
    public class MapViewModelBingControl: AViewModel, IMapViewModel
    {
        private MapDisplayMode _enmDisplayMode;

        private bool _blnInitialized;

        private bool _blnIsEnabled;

        private bool _blnIsHidden;

        private bool _blnIsWindow = false;

        private bool _blnShowWebBrowser;

        private bool _blnIsConnected;

        private object _objSource;

        private List<MapPoint> _lstPoints;

        private DelegateCommand _cmdShowMapInWindow;

        private DelegateCommand _cmdRefeshMap;

        #region Properties

        public bool IsHidden
        {
            get { return _blnIsHidden; }
            set
            {
                _blnIsHidden = value;
                RaisePropertyChanged(() => IsHidden);
            }
        }


        public bool IsWindow
        {
            get { return _blnIsWindow; }
            set
            {
                _blnIsWindow = true;
                RaisePropertyChanged(() => IsWindow);
            }
        }


        public object Content
        {
            get { return _objSource; }
            set
            {
                _objSource = value;
                RaisePropertyChanged(() => Content);
            }
        }

        public List<MapPoint> Points
        {
            get { return _lstPoints; }
            set
            {
                if (value == null)
                    _lstPoints = null;
                else
                    _lstPoints = value.Where(x => !(x.LatitudeStart == null && x.LongitudeStart == null && x.LatitudeStop == null && x.LongitudeStop == null)).ToList();

                RaisePropertyChanged(() => Points);
                RaisePropertyChanged(() => HasPoints);
                RaisePropertyChanged(() => ShowWebBrowser);
            }
        }


        public bool HasPoints
        {
            get { return (Points != null && Points.Count > 0 && Points.Where(x => !string.IsNullOrEmpty(x.LatitudeStop) && !string.IsNullOrEmpty(x.LongitudeStop)).Count() > 0) || !_blnInitialized; }
        }


        public bool ShowWebBrowser
        {
            get
            {
                return _blnShowWebBrowser;
                // return (HasPoints && IsConnected) || !_blnInitialized;
            }
            set
            {
                _blnShowWebBrowser = value;
                RaisePropertyChanged(() => ShowWebBrowser);
            }
        }


        public bool IsPointsSelected
        {
            get { return _enmDisplayMode == MapDisplayMode.Points; }
            set
            {
                if (_enmDisplayMode == MapDisplayMode.Lines && value == true)
                {
                    _enmDisplayMode = MapDisplayMode.Points;
                    Refresh();
                }
                else if (_enmDisplayMode == MapDisplayMode.Points && value == false)
                {
                    _enmDisplayMode = MapDisplayMode.Lines;
                    Refresh();
                }

                if (_enmDisplayMode.ToString() != Settings.Instance.MapDisplayMode)
                    Settings.Instance.MapDisplayMode = _enmDisplayMode.ToString();

                RaisePropertyChanged(() => IsPointsSelected);
            }
        }


        public bool IsConnected
        {
            get { return _blnIsConnected; }
            set
            {
                _blnIsConnected = value;
                RaisePropertyChanged(() => IsConnected);
                RaisePropertyChanged(() => ShowWebBrowser);
                RaisePropertyChanged(() => ShowNoConnectionMessage);
            }
        }

        public bool ShowNoConnectionMessage
        {
            get { return !IsConnected && HasPoints; }
        }


        public bool IsEnabled
        {
            get { return _blnIsEnabled; }
            set
            {
                _blnIsEnabled = value;

                if (value && !_blnInitialized)
                    RefreshAsync();

                if (value != BusinessLogic.Settings.Settings.Instance.IsMapEnabled)
                    Settings.Instance.IsMapEnabled = value;

                RaisePropertyChanged(() => IsEnabled);
            }
        }

        #endregion


        public MapViewModelBingControl()
        {
            try
            {
                Enum.TryParse(Settings.Instance.MapDisplayMode, out _enmDisplayMode);
                _blnIsEnabled = Settings.Instance.IsMapEnabled;
                WindowWidth = 700;
                WindowHeight = 500;
                _blnInitialized = false;
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }


        public Task RefreshAsync()
        {
            return Task.Factory.StartNew(Refresh);
        }

        public void Refresh()
        {
            try
            {
                if (!IsEnabled)
                    return;

                _blnInitialized = true;

                if (Points == null || Points.Count == 0 || Points.Where(x => !string.IsNullOrEmpty(x.LatitudeStop) && !string.IsNullOrEmpty(x.LongitudeStop)).Count() == 0)
                {
                    new Action(() =>
                    {
                        ShowWebBrowser = false;
                        RaisePropertyChanged(() => HasPoints);
                        RaisePropertyChanged(() => ShowNoConnectionMessage);
                    }).Dispatch();
                    return;
                }

                if (!MapViewModel.IsConnectedToNet())
                {
                    new Action(() =>
                    {
                        IsConnected = false;
                        ShowWebBrowser = false;
                    }).Dispatch();
                    return;
                }

                if (!IsConnected)
                {
                    new Action(() =>
                    {
                        IsConnected = true;
                    }).Dispatch();
                }


                //Refresh this is a new thread, so the webbrowser has time to load before data is depicted.
                Task.Factory.StartNew(() =>
                {
                    new Action(() =>
                    {
                        try
                        {
                            PushUIMessage("Rebuild");

                            if (!ShowWebBrowser)
                                ShowWebBrowser = true;
                        }
                        catch { };
                    }).Dispatch();
                });
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }



        #region Show map in window Command


        public DelegateCommand ShowMapInWindowCommand
        {
            get { return _cmdShowMapInWindow ?? (_cmdShowMapInWindow = new DelegateCommand(ShowMapInWindow)); }
        }

        private void ShowMapInWindow()
        {
            try
            {
                MapViewModelBingControl mvm = new MapViewModelBingControl();
                mvm.Points = _lstPoints;
                mvm.IsWindow = true;
                mvm.WindowTitle = this.WindowTitle;
                mvm.IsHidden = false;

                AppRegionManager.LoadWindowViewFromViewModel(mvm, false, "WindowToolBoxSimple");
                mvm.RefreshAsync();
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }

        #endregion


        #region Refresh Map Command


        public DelegateCommand RefreshMapCommand
        {
            get { return _cmdRefeshMap ?? (_cmdRefeshMap = new DelegateCommand(RefreshMap)); }
        }

        private void RefreshMap()
        {
            try
            {
                RefreshAsync();
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }

        #endregion
    }
}
