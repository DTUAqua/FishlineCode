using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Babelfisk.Entities.SprattusSecurity;
using System.Linq.Expressions;
using Anchor.Core;
using System.Windows.Input;
using Babelfisk.BusinessLogic;

namespace Babelfisk.ViewModels
{
    public class AViewModel : NotificationObject, IDataErrorInfo, IDomainObjectWindow, IDisposable
    {
        public static string WarningPrefix = "WARNING¤¤";

        public event Action<object, System.ComponentModel.CancelEventArgs, AViewModel> Closing;
        public event Action<object, AViewModel> Closed;

        public event Action<AViewModel> OnError;
        public event Action<AViewModel, string> OnScrollTo;
        public event Action<AViewModel, string> OnUIMessage;
        public event Action<AViewModel, string, object> OnUIMessageParameter;
        public event Func<string, object> OnUIReturnMessage; 

        private static event Action<System.Windows.Input.KeyEventArgs> OnPreviewKeyDown;

        protected bool _blnValidate = false;

        protected bool _blnIsDirty;


        /// <summary>
        /// Stores the DFU person entity of the person logged in to FishLine.
        /// </summary>
        protected static Entities.Sprattus.DFUPerson _dfuPersonLogin;


        /// <summary>
        /// Reference to the AnalyzerRegionManager.
        /// </summary>
        protected static IAppRegionManager _appRegionManager;


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


        public static Anchor.Core.Language.Translater Translater = null;

        private static bool _blnLanguagePropertySet = false;

        protected bool _blnIsDisposed = false;


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

        public static Entities.Sprattus.DFUPerson DFUPersonLogin
        {
            get { return _dfuPersonLogin; }
            set
            {
                _dfuPersonLogin = value;
            }
        }


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
                Error = ValidateField(columnName);

                return Error;
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


        protected virtual void PushUIMessageParameter(string message, object param)
        {
            if (OnUIMessageParameter != null)
                OnUIMessageParameter(this, message, param);
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
                Anchor.Core.Loggers.Logger.LogError(e);
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


        public virtual void ValidateAllProperties(bool blnRefreshAllProperties = false, bool raisePropertyChangedOnProperties = true)
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
                    if(raisePropertyChangedOnProperties)
                        RaisePropertyChanged(prop.Name);

                    if (HasErrors)
                        break;
                }

                if (blnRefreshAllProperties)
                {
                    new Action(() =>
                    {
                        RefreshAllNotifiableProperties();
                    }).Dispatch();
                }
            }
            _blnValidate = false;
        }


        public TreeView.MainTreeViewModel MainTree
        {
            get { return Windows.MainWindowViewModel.TreeView; }
        }


        public bool IsDisposed
        {
            get { return _blnIsDisposed; }
        }


        public static bool IsLanguagePropertySet
        {
            get { return _blnLanguagePropertySet; }
            private set
            {
                _blnLanguagePropertySet = value;
            }
        }

        #endregion




        #region Window specific properties


        private string _strWindowTitle = "";
        private double _dblWindowWidth = Double.NaN;
        private double _dblWindowHeight = Double.NaN;
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

        private static bool _isLoadingDFUPerson = false;
        private static object _dfuPersonLoadingLock = new object();
        private static Task _loadingDFUPersonTask = null;

        public static Task InitializeDFUPersonFromLoggedInUserAsync()
        {
            lock (_dfuPersonLoadingLock)
            {
                if (_isLoadingDFUPerson)
                    return _loadingDFUPersonTask ?? GeneralExtensions.TaskFromResult(true);

                _isLoadingDFUPerson = true;

                _loadingDFUPersonTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var usr = _appRegionManager == null ? null : _appRegionManager.User;

                        if (usr == null)
                            return;

                        var manLookup = new LookupManager();
                        var lv = new LookupDataVersioning();

                    //Fetch DFUPerson object based on initials and the logged in user name.
                    var dfuPerson = manLookup.GetLookups(typeof(Entities.Sprattus.DFUPerson), lv)
                                                 .OfType<Entities.Sprattus.DFUPerson>()
                                                 .Where(x => x.initials != null && x.initials.Equals(usr.UserName, StringComparison.InvariantCultureIgnoreCase))
                                                 .FirstOrDefault();

                        DFUPersonLogin = dfuPerson;
                    }
                    catch { }
                    finally
                    {
                        lock (_dfuPersonLoadingLock)
                        {
                            _isLoadingDFUPerson = false;
                            _loadingDFUPersonTask = null;
                        }
                    }
                });
            }

            return _loadingDFUPersonTask;
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
            DispatchMessageBoxStatic(strMsg, intTimeout);
        }

        public static void DispatchMessageBoxStatic(string strMsg, int? intTimeout = null)
        {
            new Action(() =>
            {
                _appRegionManager.ShowMessageBox(strMsg, intTimeout);
            }).Dispatch();
        }


        protected void DispatchAccessDeniedMessageBox()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var result = AppRegionManager.ShowMessageBox(Translate("Warning", "6"), System.Windows.MessageBoxButton.OK);
            }));
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

            //Show/hide mouse cursor (this is in here for debug purposes, because users are experiencing that the cursor sometimes disappears. If that happens they should try below method)
            if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) &&
                System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftAlt)  &&
                System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift))
            {
                if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.C))
                {
                    //System.Windows.Input.Mouse.OverrideCursor = Cursors.Arrow;
                    System.Windows.Forms.Cursor.Show();
                }
              /*  else if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.H))
                {
                    //System.Windows.Input.Mouse.OverrideCursor = Cursors.None;
                    System.Windows.Forms.Cursor.Hide();
                }*/
            }
        }

        protected void AViewModel_OnPreviewKeyDown(System.Windows.Input.KeyEventArgs obj)
        {
            GlobelPreviewKeyDown(obj);
        }

        #endregion


        public static string Translate(string strSection, string strKey)
        {
            if (Translater == null)
                return "";

            return Translater.Translate(strSection, strKey);
        }

        public void LogInfo(string strMessage)
        {
            Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Info, strMessage);
        }

        public void LogError(string strMessage)
        {
            LogErrorStatic(strMessage);
        }

        public static void LogErrorStatic(string strMessage)
        {
            Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Error, strMessage);
        }

        public void LogError(Exception e, string strMessage = null)
        {
            LogErrorStatic(e, strMessage);
        }

        public static void LogErrorStatic(Exception e, string strMessage = null)
        {
            Anchor.Core.Loggers.Logger.LogError(e, strMessage);
        }


        public void LogAndDispatchUnexpectedErrorMessage(string message)
        {
            LogAndDispatchUnexpectedErrorMessageStatic(message);
        }

        public static void LogAndDispatchUnexpectedErrorMessageStatic(string message)
        {
            try
            {
                string msg = string.Format(Translate("Common", "Error"), message);
                LogErrorStatic(msg);
                DispatchMessageBoxStatic(msg);
            }
            catch { }
        }

        public void LogAndDispatchUnexpectedErrorMessage(Exception e)
        {
            LogAndDispatchUnexpectedErrorMessageStatic(e);
        }

        public static void LogAndDispatchUnexpectedErrorMessageStatic(Exception e)
        {
            try
            {
                LogErrorStatic(e);
                DispatchMessageBoxStatic(string.Format(Translate("Common", "Error"), e.Message));
            }
            catch { }
        }


        protected void ResetLoadingMessage()
        {
            LoadingMessage = _strDefaultLoadingMessage; 
        }


        public static Anchor.Core.Language.Translater ApplyLanguage(string strLanguage, bool blnApplyLanguageProperty = true)
        {
            Anchor.Core.Language.Translater t;
            CultureInfo curCulture = null;
            try
            {
                var lan = strLanguage;//BusinessLogic.Settings.Settings.Instance.Language; // "da-DK";
                curCulture = new CultureInfo(lan);
                t = Anchor.Core.Language.TranslaterFactory.Instance.ApplyTranslater(curCulture);
            }
            catch
            {
                curCulture = new CultureInfo("da-DK");
                t = Anchor.Core.Language.TranslaterFactory.Instance.ApplyTranslater(curCulture);
            }

            if (curCulture != null)
            {
                new Action(() =>
                {
                    try
                    {
                        curCulture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
                        curCulture.DateTimeFormat.DateSeparator = "-";

                        System.Threading.Thread.CurrentThread.CurrentCulture = curCulture;
                        System.Threading.Thread.CurrentThread.CurrentUICulture = curCulture;

                        if (!_blnLanguagePropertySet && blnApplyLanguageProperty)
                        {
                            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(curCulture.IetfLanguageTag)));
                            System.Windows.Documents.Run.LanguageProperty.OverrideMetadata(typeof(System.Windows.Documents.Run), new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(curCulture.IetfLanguageTag)));
                            IsLanguagePropertySet = true;
                        }

                        Translater = t;
                        BusinessLogic.Settings.Settings.Translater = t;
                        
                    }
                    catch (Exception e)
                    {
                        Anchor.Core.Loggers.Logger.LogError(e);
                    }
                }).Dispatch();
            }

            return t;
        }


        public virtual void Dispose()
        {
            _blnIsDisposed = true;
            DeRegisterToKeyDown();
        }
    }
}
