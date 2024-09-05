using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Babelfisk.ViewModels;
using Microsoft.Practices.Prism.Regions;
using Anchor.Core.Language;
using Anchor.Core;
using System.Threading;

namespace Babelfisk.WPF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export]
    public partial class MainWindow : Window
    {
        private IRegionManager _regionManager;

        private IAppRegionManager _appRegionManager;


        #region Properties

        [Import(AllowRecomposition = false)]
        public IRegionManager RegionManager
        {
            get { return _regionManager; }
            set
            {
                _regionManager = value;
            }
        }

        
        [Export("AppRegionManager")]
        public IAppRegionManager AppRegionManager
        {
            get
            {
                if (RegionManager == null)
                    return null;

                if (_appRegionManager == null)
                    _appRegionManager = new AppRegionManager(RegionManager, this);

                return _appRegionManager;
            }
        }


        [Export("CompositionContainer")]
        public CompositionContainer CompositionContainer
        {
            get;
            set;
        }

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.Closing += MainWindow_Closing;
            this.SizeChanged += MainWindow_SizeChanged;

            try
            {
                //Specific date pattern used for DatePicker control.
                var cult = (CultureInfo)System.Globalization.CultureInfo.InvariantCulture.Clone();
                cult.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
                cult.DateTimeFormat.DateSeparator = "-";

                Thread.CurrentThread.CurrentUICulture = cult;
                Thread.CurrentThread.CurrentCulture = cult;
            }
            catch { }

            Anchor.Core.Language.TranslaterFactory.Instance.LanguageFolder = BusinessLogic.Settings.Settings.Instance.LanguageFolder;

            //.ListItemHighlightForegroundBrush
            //var color =  SystemColors.MenuTextColor;
            //var cListItemHighlightForegroundBrush = String.Format("#{0}{1}{2}{3}", color.A.ToString("X2"), color.R.ToString("X2"), color.G.ToString("X2"), color.B.ToString("X2"));
        }


        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTitle();
        }


        public void UpdateTitle()
        {
            Task.Factory.StartNew(() =>
            {
                new Action(() =>
                {
                    this.Title = string.Format("Fiskeline{0}{1}", GetSizeToTitle(), GetAddressToTitle());
                }).Dispatch();
            });
        }

        private string GetSizeToTitle()
        {
            return string.Format(" - {0:0}x{1:0}", this.ActualWidth.ToString("0"), this.ActualHeight.ToString("0"));
        }

        private string GetAddressToTitle()
        {
            var strEndpoint = new Babelfisk.BusinessLogic.DataRetrievalManager().GetConnectedEndPoint();

            try
            {
                if (strEndpoint != "Offline")
                {
                    Uri host = new Uri(strEndpoint);
                    strEndpoint = host.Host;
                }
            }
            catch(Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return string.Format(" - {0}", strEndpoint);
        }


        private bool CheckForUnsavedData()
        {
            bool blnUnsavedData = AppRegionManager.HasRegionUnsavedData(RegionName.MainRegion);

            if (blnUnsavedData)
            {
                var res = AppRegionManager.ShowMessageBox(AViewModel.Translater.Translate("Warning", "3"), System.Windows.MessageBoxButton.YesNo);

                if (res == System.Windows.MessageBoxResult.No)
                    return true;
            }

            return false;
        }


        protected void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(CheckForUnsavedData())
            {
                e.Cancel = true;
                return;
            }

            Babelfisk.BusinessLogic.Settings.Settings.Instance.Save();
        }


        protected void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            AViewModel.StaticPreviewKeyDown(sender, e);
        }


        protected  void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as AViewModel).AppRegionManager = AppRegionManager;

            (this.DataContext as Babelfisk.ViewModels.Windows.MainWindowViewModel).InitializeAsync();

            UpdateTitle();
        }
        
    }
}
