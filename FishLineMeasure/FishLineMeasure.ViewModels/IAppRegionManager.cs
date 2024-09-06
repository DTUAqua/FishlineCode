using Babelfisk.Entities.SprattusSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FishLineMeasure.ViewModels
{
    public interface IAppRegionManager
    {
        Users User
        {
            get;
            set;
        }

        void ShowMessageBoxDefaultTimeout(string strMsg);

        void ShowMessageBox(string strMsg, int? intAutoCloseTimeout = null);

        MessageBoxResult ShowMessageBox(string strMsg, MessageBoxButton enmButtons);

        MessageBoxResult ShowMessageBox(object xaml, MessageBoxButton enmButtons);

        void ShowLoadingWindow(string strMessage, bool blnShowProgressBar = true);

        void HideLoadingWindow();

        void RefreshWindowTitle();

        #region Load methods

        /// <summary>
        /// Load a view associated to aDomainObject in the specified region enmRegionName.
        /// <returns>Returns whether the view was loaded or not (if not, it was cancelled by the user)</returns>
        /// </summary>
        bool LoadViewFromViewModel(RegionName enmRegionName, AViewModel aDomainObject);


        /// <summary>
        /// Load a view from a view model T, in the specified region enmRegionName. The view model constructor called, is
        /// specified by the ConstructorParam parameters given. If no constructor exist for the specified arrConstructorArgs,
        /// an exception is thrown.
        /// </summary>
        bool LoadViewFromViewModel<T>(RegionName enmRegionName, params ConstructorParam[] arrConstructorArgs) where T : AViewModel;


        /// <summary>
        /// Loads view model in a new pop up window
        /// </summary>
        void LoadWindowViewFromViewModel<T>(bool blnModal = false, params ConstructorParam[] arrConstructorArgs) where T : AViewModel;


        /// <summary>
        /// Loads view for specified domain object in a new window.
        /// </summary>
        void LoadWindowViewFromViewModel(AViewModel aDomainObject, bool blnModal = false, string strWindowStyle = null);


        /// <summary>
        /// Loads a view for a specified domain object in a new window (the first time). If this method is called
        /// a second time and the window i still open, the window will switch view to the new one.
        /// </summary>
        void SwitchWindowViewFromViewModel(AViewModel aDomainObject);

        #endregion

        bool HasRegionUnsavedData(RegionName? region);

        void RefreshRegion(RegionName? region = null, RefreshOptions r = null);

        void InvalidateViews(RegionName? region = null);

        void ClearRegions(params RegionName[] regions);
    }
}
