using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Anchor.Core;
using Babelfisk.Entities.SprattusSecurity;
using FishLineMeasure.ViewModels.Windows;
using System.Runtime.Serialization;

namespace FishLineMeasure.ViewModels
{
    [DataContract]
    public class AViewModel : NotificationObject, IDataErrorInfo, IDomainObjectWindow, IDisposable
    {
        public static string WarningPrefix = "WARNING¤¤";

        public event Action<object, System.ComponentModel.CancelEventArgs, AViewModel> Closing;
        public event Action<object, AViewModel> Closed;

        public event Action<AViewModel> OnError;
        public event Action<AViewModel, string> OnScrollTo;
        public event Action<AViewModel, string> OnUIMessage;
        public event Func<string, object> OnUIReturnMessage;

        private static event Action<System.Windows.Input.KeyEventArgs> OnPreviewKeyDown;

        protected bool _blnValidate = false;

        protected bool _blnIsDirty;

        /// <summary>
        /// Reference to the AnalyzerRegionManager.
        /// </summary>
        protected static IAppRegionManager _appRegionManager;

        protected static MainWindowViewModel _vmMain;


        /// <summary>
        /// Reference to error string.
        /// </summary>
        [NonSerialized]
        private string _strError = null;


        /// <summary>
        /// Is object currently loading
        /// </summary>
        [NonSerialized]
        private bool _blnIsLoading = false;


        [NonSerialized]
        protected static string _strDefaultLoadingMessage = "Arbejder, vent venligst...";
        [NonSerialized]
        private string _strLoadingMessage = _strDefaultLoadingMessage;


        /// <summary>
        /// Command for closing a dialog window, if view model is opened in such a view.
        /// </summary>
        private DelegateCommand _cmdCloseWindow;

        protected bool _blnIsDisposed = false;


        public MainWindowViewModel Main
        {
            get { return _vmMain; }
        }

        static AViewModel()
        {
        }

        public AViewModel()
        {
        }

        ~AViewModel()
        {
            DeRegisterToKeyDown();
        }


        #region Properties

        /// <summary>
        /// Get/Set boolean flag indicating whether internal object data is current being loaded.
        /// </summary>
        public virtual bool IsLoading
        {
            get { return _blnIsLoading; }
            set
            {
                _blnIsLoading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }



        public virtual string LoadingMessage
        {
            get { return _strLoadingMessage; }
            set
            {
                _strLoadingMessage = value;
                RaisePropertyChanged(() => LoadingMessage);
            }
        }


        protected void AdjustWindowWidthHeightToScreen()
        {
            var screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 0.95;
            var screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 0.95;

            if (WindowWidth > screenWidth)
                WindowWidth = screenWidth;

            if (WindowHeight > screenHeight)
                WindowHeight = screenHeight;
        }


        /// <summary>
        /// Get errors (if any). If null is returned, no errors exist. The error property is typically
        /// changed during UI validation.
        /// </summary>
        public virtual string Error
        {
            get { return _strError; }
            protected set
            {
                _strError = value;

                if (OnError != null && !string.IsNullOrEmpty(value))
                    OnError(this);

                RaisePropertyChanged(() => Error);
                RaisePropertyChanged(() => HasErrors);
                RaisePropertyChanged(() => GetUIErrorMessage);
                RaisePropertyChanged(() => GetUIWarningMessage);
            }
        }


        public virtual string GetUIWarningMessage
        {
            get { return HasWarningsOnly ? _strError.Replace(WarningPrefix, "") : null; }
        }

        public virtual string GetUIErrorMessage
        {
            get { return HasErrors ? _strError.Replace(WarningPrefix, "") : null; }
        }


        /// <summary>
        /// Indexer overload validating a specific object property. If null is returned, property is valid.
        /// </summary>
        public virtual string this[string columnName]
        {
            get
            {
                var error = ValidateField(columnName);

                return error;
            }
        }

        public virtual bool HasWarningsOnly
        {
            get { return !String.IsNullOrEmpty(Error) && Error.Contains(WarningPrefix); }
        }


        /// <summary>
        /// Get a value specifying wether object has validation errors.
        /// </summary>
        public virtual bool HasErrors
        {
            get { return !String.IsNullOrEmpty(Error) && !Error.Contains(WarningPrefix); }
        }


        //[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Method used to raise an event")]
        //[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Cannot change the signature")]
        protected virtual void ScrollTo<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            this.ScrollTo(propertyName);
        }

        protected virtual void ScrollTo(string str)
        {
            if (OnScrollTo != null)
                OnScrollTo(this, str);
        }


        protected virtual void PushUIMessage(string message)
        {
            if (OnUIMessage != null)
                OnUIMessage(this, message);
        }


        protected virtual T PushUIReturnMessage<T>(string msg)
        {
            object obj = null;

            try
            {
                var e = OnUIReturnMessage;
                if (e != null)
                    obj = e(msg);

                if (obj == null)
                    return default(T);

                return (T)obj;
            }
            catch (Exception e)
            {
                LogError(e);
            }

            return default(T);
        }




        /// <summary>
        /// When a view model is created using MEF, AnalyzerRegionManager will be instantiated
        /// automatically. It will therefore not be available in the constructor, if this is needed,
        /// create a constructor taking the IAnalyzerRegionManager in as argument and apply the [ImportingConstructor]
        /// attribute to it.
        /// This property sets the protected static instance, so the RegionManager is only set once for all the 
        /// view models.
        /// </summary>
        [Import("AppRegionManager")]
        public IAppRegionManager AppRegionManager
        {
            get { return _appRegionManager; }
            set
            {
                if (_appRegionManager == null)
                    _appRegionManager = value;
            }
        }



        public Dispatcher Dispatcher
        {
            get
            {
                //If view model is used in an application, return current dispatcher
                if (Application.Current != null)
                    return Application.Current.Dispatcher;
                else //If not used in an application instance (e.g. for unit testing) return a new dispatcher instance
                    return Dispatcher.CurrentDispatcher;
            }
        }


        public virtual bool IsDirty
        {
            get { return _blnIsDirty; }
            set
            {
                _blnIsDirty = value;
                RaisePropertyChanged(() => IsDirty);
            }
        }


        /// <summary>
        /// Run through all control instance properties and calls OnPropertyChanged on them.
        /// </summary>
        public virtual void RefreshAllNotifiableProperties()
        {
            PropertyInfo[] arrPropInfo = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (arrPropInfo != null && arrPropInfo.Length > 0)
                RaisePropertyChanged(arrPropInfo.Select(x => x.Name).ToArray());
        }


        public virtual void ValidateAllProperties(bool blnRefreshAllProperties = false)
        {
            PropertyInfo[] arrPropInfo = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            Error = null;
            _blnValidate = true;
            if (arrPropInfo != null && arrPropInfo.Length > 0)
            {
                foreach (var prop in arrPropInfo)
                {
                    Error = ValidateField(prop.Name);

                    // if(Error != null)
                    RaisePropertyChanged(prop.Name);

                    if (HasErrors)
                        break;
                }

                if (blnRefreshAllProperties)
                    new Action(() =>
                    {
                        RefreshAllNotifiableProperties();
                    }).Dispatch();
            }
            _blnValidate = false;
        }


  
        public bool IsDisposed
        {
            get { return _blnIsDisposed; }
        }


        public BusinessLogic.Settings.Settings AppSettings
        {
            get { return BusinessLogic.Settings.Settings.Instance; }
        }


        #endregion


        #region Window specific properties


        private string _strWindowTitle = "";
        private double _dblWindowWidth = Double.NaN;
        private double _dblWindowHeight = Double.NaN;

        private double _dblMinWindowWidth = Double.NaN;
        private double _dblMinWindowHeight = Double.NaN;

        private bool _blnIsCloseable = true;

        /// <summary>
        /// Get/Set window title.
        /// </summary>
        public string WindowTitle
        {
            get { return _strWindowTitle; }
            set
            {
                _strWindowTitle = value;
                RaisePropertyChanged(() => WindowTitle);
            }
        }


        /// <summary>
        /// Get/Set window width.
        /// </summary>
        public double WindowWidth
        {
            get { return _dblWindowWidth; }
            set
            {
                _dblWindowWidth = value;
                RaisePropertyChanged(() => WindowWidth);
            }
        }

        public double MinWindowWidth
        {
            get { return _dblMinWindowWidth; }
            set
            {
                _dblMinWindowWidth = value;
                RaisePropertyChanged(() => MinWindowWidth); 
            }
        }

        /// <summary>
        /// Get/Set window height
        /// </summary>
        public double WindowHeight
        {
            get { return _dblWindowHeight; }
            set
            {
                _dblWindowHeight = value;
                RaisePropertyChanged(() => WindowHeight);
            }
        }

        public double MinWindowHeight
        {
            get { return _dblMinWindowHeight; }
            set
            {
                _dblMinWindowHeight = value;
                RaisePropertyChanged(() => WindowHeight);
            }
        }

        public bool IsCloseable
        {
            get { return _blnIsCloseable; }
            set
            {
                _blnIsCloseable = value;
                RaisePropertyChanged(() => IsCloseable);
            }
        }



        #region Close Window Command

        public DelegateCommand CloseWindowCommand
        {
            get
            {
                if (_cmdCloseWindow == null)
                    _cmdCloseWindow = new DelegateCommand(() => Close());

                return _cmdCloseWindow;
            }
        }

        public virtual void Close()
        {
            if (RequestClose != null)
                RequestClose(this);
        }

        #endregion


        #region IDomainObjectWindow Members

        public event Action<IDomainObjectWindow> RequestClose;

        #endregion


        #endregion


        #region Validation methods


        /// <summary>
        /// If validation is needed on a view model, the method below should be overriden and properties cased (switch).
        /// Returning null means no error(s) were found.
        /// </summary>
        protected virtual string ValidateField(string strFieldName)
        {
            return null;
        }


        /// <summary>
        /// Resets any errors.
        /// </summary>
        public virtual void ResetErrors()
        {
            Error = null;
        }


        #endregion


        public Users User
        {
            get { return AppRegionManager == null ? null : AppRegionManager.User; }
        }



        public virtual bool HasUnsavedData
        {
            get { return false; }
        }


        /// <summary>
        /// Method for refreshing the view model. This method should be overriden by inherited view models if the view models
        /// contains data that can be refreshed.
        /// </summary>
        public virtual void Refresh(RefreshOptions r = null)
        {

        }


        /// <summary>
        /// Returns whether the AViewModel type is represented in the types array.
        /// If the array is null or empty, true is returned.
        /// </summary>
        protected bool ShouldRefresh(RefreshOptions r)
        {
            if (r.Omit.Length > 0 && r.Omit.Where(x => x == this).Count() > 0)
                return false;

            if (r.Types.Length == 0)
                return true;

            return r.Types.Where(x => x == this.GetType()).Count() > 0;
        }


        public virtual void WindowCloseButtonClicked()
        {

        }



        public virtual void FireClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Closing != null)
                Closing(sender, e, this);
        }

        public virtual void FireClosed(object sender, EventArgs e)
        {
            if (Closed != null)
                Closed(sender, this);
        }



        protected void DispatchMessageBox(string strMsg, int? intTimeout = null)
        {
            new Action(() =>
            {
                AppRegionManager.ShowMessageBox(strMsg, intTimeout);
            }).Dispatch();
        }



        #region Key events

        public void RegisterToKeyDown()
        {
            OnPreviewKeyDown += AViewModel_OnPreviewKeyDown;
        }


        public void DeRegisterToKeyDown()
        {
            OnPreviewKeyDown -= AViewModel_OnPreviewKeyDown;
        }


        /// <summary>
        /// Override this method in viewmodel to get global key downs
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void GlobelPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {

        }


        public static void StaticPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (OnPreviewKeyDown != null)
                OnPreviewKeyDown(e);
        }

        protected void AViewModel_OnPreviewKeyDown(System.Windows.Input.KeyEventArgs obj)
        {
            GlobelPreviewKeyDown(obj);
        }

        #endregion



        protected void ResetLoadingMessage()
        {
            LoadingMessage = _strDefaultLoadingMessage;
        }


        public virtual void Dispose()
        {
            _blnIsDisposed = true;
            DeRegisterToKeyDown();
        }


        public static void LogError(Exception e, string msg = null)
        {
            try
            {
                Anchor.Core.Loggers.Logger.LogError(e, msg);

                System.Diagnostics.Debug.WriteLine("Exception: {0} {1} {3}", e.Message, e.InnerException != null ? (", " + (e.InnerException.Message ?? "")) : "", msg ?? "");
            }
            catch { }
        }

        public static void LogError(string msg)
        {
            try
            {
                Anchor.Core.Loggers.Logger.Log( Anchor.Core.Loggers.LogType.Error, msg);

                System.Diagnostics.Debug.WriteLine("Error: {0}", msg ?? "");
            }
            catch { }
        }

        public static void LogInfo(string msg)
        {
            try
            {
                Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Info, msg);

                System.Diagnostics.Debug.WriteLine("Info: {0}", msg ?? "");
            }
            catch { }
        }
    }
}

