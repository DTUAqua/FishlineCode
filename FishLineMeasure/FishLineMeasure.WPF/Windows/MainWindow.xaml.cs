using FishLineMeasure.ViewModels;
using FishLineMeasure.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using Anchor.Core;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Practices.Prism.Regions;

namespace FishLineMeasure.WPF.Windows
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


        public AViewModel ViewModel
        {
            get { return this.DataContext as ViewModels.Windows.MainWindowViewModel; }
        }

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.Closing += MainWindow_Closing;
            this.SizeChanged += MainWindow_SizeChanged;
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
                    this.Title = string.Format("FishLine - Measure{0}{1}", GetSizeToTitle(), GetAddressToTitle());
                }).Dispatch();
            });
        }

        private string GetSizeToTitle()
        {
            return string.Format(" - {0:0}x{1:0}", this.ActualWidth.ToString("0"), this.ActualHeight.ToString("0"));
        }

        private string GetAddressToTitle()
        {
            /*var strEndpoint = new Babelfisk.BusinessLogic.DataRetrievalManager().GetConnectedEndPoint();

            try
            {
                if (strEndpoint != "Offline")
                {
                    Uri host = new Uri(strEndpoint);
                    strEndpoint = host.Host;
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return string.Format(" - {0}", strEndpoint);
            */
            return "";
        }


        private bool CheckForUnsavedData()
        {
            bool blnUnsavedData = AppRegionManager.HasRegionUnsavedData(RegionName.MainRegion);

            if (blnUnsavedData)
            {
                var res = AppRegionManager.ShowMessageBox("Der er ændringer som ikke er gemt, er du sikker på du vil fortsætte?", System.Windows.MessageBoxButton.YesNo);

                if (res == System.Windows.MessageBoxResult.No)
                    return true;
            }

            return false;
        }


        protected void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CheckForUnsavedData())
            {
                e.Cancel = true;
                return;
            }

            if (ViewModel != null)
                ViewModel.FireClosing(sender, e);

            if (e.Cancel)
                return;

            FishLineMeasure.BusinessLogic.Settings.Settings.Instance.Save();
        }


        protected void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            AViewModel.StaticPreviewKeyDown(sender, e);
        }


        protected void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as AViewModel).AppRegionManager = AppRegionManager;

            (this.DataContext as MainWindowViewModel).Initialize();

            UpdateTitle();
        }

    }

}
