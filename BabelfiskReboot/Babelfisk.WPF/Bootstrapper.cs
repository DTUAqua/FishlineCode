using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Babelfisk.ViewModels;
using Babelfisk.WPF.Windows;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Markup;
using Babelfisk.ViewModels.Windows;
using System.Reflection;
using System.Windows.Interop;
using System.Windows.Threading;

using Anchor.Core;

namespace Babelfisk.WPF
{
    public class Bootstrapper : MefBootstrapper
    {
        private CompositionContainer _CompContainer;

        private string _strApplicationData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Anchor.Core.Loggers.BaseLogger.AssemblyName + @"\");


        public string LanguageFolder
        {
            get
            {
                return Path.Combine(_strApplicationData, "Languages");
            }
        }

        /*
        public Bootstrapper()
        {
            LoadLanguageSettings();
        }*/
    
        /*
        private void LoadLanguageSettings()
        {
            CopyLanguageFiles();

            var curCulture = CultureInfo.CurrentCulture;

            Anchor.Core.Language.TranslaterFactory.Instance.LanguageFolder = LanguageFolder; //System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Xml\\Languages");

            Anchor.Core.Language.Translater t;
            try
            {
                //For now, hardcode current culture to Danish.
                curCulture = new CultureInfo("da-DK");
                t = Anchor.Core.Language.TranslaterFactory.Instance.ApplyTranslater(curCulture);
            }
            catch
            {
                curCulture = new CultureInfo("da-DK");
                t = Anchor.Core.Language.TranslaterFactory.Instance.ApplyTranslater(curCulture);
            }

            //System.Threading.Thread.CurrentThread.CurrentCulture = curCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = curCulture;
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                                                               XmlLanguage.GetLanguage(curCulture.Name)));

            AViewModel.Translater = t;
        }*/


       /* private void CopyLanguageFiles()
        {
            if (!Directory.Exists(_strApplicationData))
                Directory.CreateDirectory(_strApplicationData);

            if (!Directory.Exists(LanguageFolder))
                Directory.CreateDirectory(LanguageFolder);

            List<string> _lst = new List<string>();
            _lst.Add("da-DK.xml");

            foreach (string sLanguage in _lst)
            {
                string strLocalPath = Path.Combine(LanguageFolder, sLanguage);

                Stream stmLanguage = Application.GetResourceStream(new Uri("pack://application:,,,/Xml/Languages/" + sLanguage)).Stream;

                FileInfo fi = new FileInfo(strLocalPath);
                if (fi.Exists && fi.Length == stmLanguage.Length)
                    continue;

                fi = null;
                using (FileStream fs = new FileStream(strLocalPath, FileMode.Create, FileAccess.Write))
                    stmLanguage.CopyTo(fs);
            }
        }*/


        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();


            //Add current assembly as one of the catalogs (so MEF pattern types in this assembly can be used).
            //We should probably add a DirectoryCatelog later looking for other assemblies in a directory implementing
            //the MEF pattern.
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(AViewModel).Assembly));
        }


        protected override IModuleCatalog CreateModuleCatalog()
        {
            var modCatalog = base.CreateModuleCatalog();

            //We might in the future need to create a module catalog from xaml.
            //var modCatalog = Microsoft.Practices.Prism.Modularity.ModuleCatalog.CreateFromXaml(new Uri(ModuleCatalogUri, UriKind.Relative));

            return modCatalog;
        }


        /// <summary>
        /// Create the CompositionContainer based on the AggregateCatalog (this is required in order for 
        /// the viewmodel in Analyzer.ViewModels to be bound to the view automatically.
        /// </summary>
        protected override CompositionContainer CreateContainer()
        {
            _CompContainer = new CompositionContainer(this.AggregateCatalog);

            return _CompContainer;
        }





        protected override Microsoft.Practices.Prism.Regions.RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var regionAdapterMappings = Container.GetExport<RegionAdapterMappings>();
            //var regionFactory = Container.GetExport<IRegionBehaviorFactory>();

            if (regionAdapterMappings != null)
            {
               //regionAdapterMappings.Value.RegisterMapping(typeof(DockableContent), this.Container.GetExportedValue<DockableContentRegionAdapter>());
                // regionAdapterMappings.Value.RegisterMapping(typeof(ContentControl), this.Container.GetExportedValue<KFish.WPF.Infrastructure.Regions.ContentControlRegionAdapter>());
            }

            return base.ConfigureRegionAdapterMappings();
        }


        /// <summary>
        /// Initialize main window and save the CompositionContainer in it as well (if needed later).
        /// </summary>
        protected override void InitializeShell()
        {
            base.InitializeShell();

            //Initialize the main window
            Application.Current.MainWindow = this.Shell as Window;
            (Application.Current.MainWindow as MainWindow).CompositionContainer = _CompContainer;
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Create main window shell.
        /// </summary>
        protected override DependencyObject CreateShell()
        {
            DependencyObject dObject = this.Container.GetExportedValue<MainWindow>();
            (dObject as MainWindow).DataContext = new MainWindowViewModel() { };
            (dObject as MainWindow).InvalidateVisual();
            return dObject;
        }
    }
}
