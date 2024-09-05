using Babelfisk.Entities.Sprattus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
using System.IO;

namespace Babelfisk.WPF.Views.SmartDots
{
    /// <summary>
    /// Interaction logic for OtolithImagesPreview.xaml
    /// </summary>
    public partial class OtolithImagesPreview : UserControl, INotifyPropertyChanged, IDisposable
    {
        public enum SourceBindingType
        {
            SDFiles,
            FilePath,
            FilePaths
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private List<SDFileItem> _lstFiles;

        private bool _isLoading = false;


        //One of below 3 propertyies can be used to preview the files.

        #region SDFiles Property

        /// <summary>
        /// SelectedItem member property used for tooltip.
        /// </summary>
        public static readonly DependencyProperty SDFilesProperty = DependencyProperty.Register("SDFiles", typeof(IList<SDFile>), typeof(OtolithImagesPreview), new UIPropertyMetadata(null, SDFilesChanged));


        [DefaultValue(null)]
        public IList<SDFile> SDFiles
        {
            get
            {
                return this.GetValue(SDFilesProperty) as IList<SDFile>;
            }
            set
            {
                this.SetValue(SDFilesProperty, value);
            }
        }


        public static void SDFilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var view = d as OtolithImagesPreview;

                if (view != null)
                {
                    view.Initialize();
                }
            }
            catch { }
        }


        #endregion


        #region FilePath Property


        /// <summary>
        /// SelectedItem member property used for tooltip.
        /// </summary>
        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(OtolithImagesPreview), new UIPropertyMetadata(null, FilePathChanged));


        [DefaultValue(null)]
        public string FilePath
        {
            get
            {
                return this.GetValue(FilePathProperty) as string;
            }
            set
            {
                this.SetValue(FilePathProperty, value);
            }
        }


        public static void FilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as OtolithImagesPreview;

            if (view != null)
            {
                view.Initialize();
            }
        }


        #endregion


        #region FilePaths Property


        /// <summary>
        /// SelectedItem member property used for tooltip.
        /// </summary>
        public static readonly DependencyProperty FilePathsProperty = DependencyProperty.Register("FilePaths", typeof(IEnumerable<string>), typeof(OtolithImagesPreview), new UIPropertyMetadata(null, FilePathsChanged));


        [DefaultValue(null)]
        public IEnumerable<string> FilePaths
        {
            get
            {
                return this.GetValue(FilePathsProperty) as IEnumerable<string>;
            }
            set
            {
                this.SetValue(FilePathsProperty, value);
            }
        }


        public static void FilePathsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as OtolithImagesPreview;

            if (view != null)
            {
                view.Initialize();
            }
        }


        #endregion


        #region Properties


        public List<SDFileItem> FileItems
        {
            get { return _lstFiles; }
            set
            {
                if(_lstFiles != null && _lstFiles.Count > 0)
                    _lstFiles.ForEach(x => x.Dispose());

                _lstFiles = value;
                RaisePropertyChanged(() => FileItems);
                RaisePropertyChanged(() => HasFileItems);
            }
        }


        public bool HasFileItems
        {
            get { return _lstFiles != null && _lstFiles.Count > 0; }
        }


        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }


        #endregion


        public OtolithImagesPreview()
        {
            InitializeComponent();
        }


        private void Initialize()
        {
            try
            {
                List<SDFileItem> lst = new List<SDFileItem>();

                var sdFiles = SDFiles;
                if (sdFiles != null && sdFiles.Count > 0)
                {
                    foreach (var f in sdFiles)
                    {
                        var path = System.IO.Path.Combine(f.path, f.fileName);
                        var fi = new SDFileItem();
                        fi.Path = path;
                        fi.IsLoading = true;
                        lst.Add(fi);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(FilePath))
                {
                    var fi = new SDFileItem();
                    fi.Path = FilePath;
                    fi.IsLoading = true;
                    lst.Add(fi);
                }
                else if (FilePaths != null)
                {
                    foreach (var p in FilePaths)
                    {
                        var fi = new SDFileItem();
                        fi.Path = p;
                        fi.IsLoading = true;
                        lst.Add(fi);
                    }
                }

                FileItems = lst;

                Task.Factory.StartNew(LoadImages);
            }
            catch { }
        }


        private void LoadImages()
        {
            try
            {
                var fi = FileItems.ToList();
                if (fi == null || fi.Count == 0)
                    return;

                foreach (var f in fi)
                {
                    string error = null;
                    byte[] arrImage = GetImage(f.Path, ref error);
                    new Action(() =>
                    {
                        try
                        {
                            f.Error = error;

                            if (arrImage != null && arrImage.Length > 0)
                            {
                                MemoryStream ms = new MemoryStream(arrImage);
                                var img = ConvertToBitmapImage(ms);

                                f.Image = img;
                            }
                        }
                        catch { }
                        finally
                        {
                            f.IsLoading = false;
                        }
                    }).Dispatch();
                }
            }
            catch { }
        }


        public static byte[] GetImage(string path, ref string error)
        {
            var man = new BusinessLogic.SmartDots.SmartDotsManager();

            if(string.IsNullOrWhiteSpace(path))
            {
                error = "Image not found";
                return null;
            }
            Entities.ServiceResult res = null;

            try
            {
                res = man.GetFileBytes(path);
            }
            catch(Exception e)
            {
                error = e.Message;
                return null;
            }

            if(res == null)
            {
                error = "Image not found";
                return null;
            }

            if(res.Result != Entities.DatabaseOperationStatus.Successful)
            {
                error = res.Message;
                return null;
            }

            if(res.Data == null)
            {
                error = "Image content not found";
                return null;
            }

            byte[] arr = res.Data as byte[];

            if (arr == null)
            {
                error = "Image content not found";
                return null;
            }

            return arr;
        }


        public static BitmapImage ConvertToBitmapImage(Stream stream)
        {
            try
            {
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);
                var image = new BitmapImage();

                image.BeginInit();
                //image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
                return image;
            }
            catch { }
            return null;
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            try
            {
                var me = (propertyExpression.Body as MemberExpression);

                var evt = PropertyChanged;
                if (me != null && me.Member != null && !string.IsNullOrWhiteSpace(me.Member.Name) && evt != null)
                    evt(this, new PropertyChangedEventArgs(me.Member.Name));
            }
            catch { }
        }



        public static void LoadPreviewWindow(object DataContext, string itemPropertyName, SourceBindingType bindTo)
        {
            try
            {
                Window w = new Window();
                w.WindowStyle = WindowStyle.ToolWindow;
                w.Owner = Application.Current.MainWindow;
                w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                w.Title = Babelfisk.ViewModels.AViewModel.Translate("Common", "ImagesParan");
                //  w.SizeToContent = SizeToContent.Height;

                w.Width = 700;
                w.Height = 600;

                var v = new OtolithImagesPreview();
                v.DataContext = DataContext;

                Binding bi = new Binding(itemPropertyName);
                bi.Source = DataContext;

                switch (bindTo)
                {
                    case SourceBindingType.SDFiles:
                        v.SetBinding(OtolithImagesPreview.SDFilesProperty, bi);
                        break;
                    case SourceBindingType.FilePath:
                        v.SetBinding(OtolithImagesPreview.FilePathProperty, bi);
                        break;
                    case SourceBindingType.FilePaths:
                        v.SetBinding(OtolithImagesPreview.FilePathsProperty, bi);
                        break;
                }

                w.Content = v;


                EventHandler evtClosed = null;
                evtClosed = (obj, e) =>
                {
                    //Deregister events
                    try
                    {
                        if (evtClosed != null)
                            w.Closed -= evtClosed;
                    }
                    catch { }

                    try
                    {
                        if (v is IDisposable)
                            (v as IDisposable).Dispose();
                    }
                    catch { }
                };
                w.Closed += evtClosed;

                w.Show();

                //Make sure window is brought to front.
                Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(200);
                    new Action(() =>
                    {
                        try
                        {
                            w.Activate();
                            w.Focus();
                        }
                        catch { }
                    }).Dispatch();
                });
            }
            catch { }
        }

        public void Dispose()
        {
            try
            {
                //This also disposes all items, since the FileItems property handles that.
                FileItems = null;
            }
            catch { }
        }

       

    }


    public class SDFileItem : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isLoading = false;

        private BitmapImage _image;

        private string _errorMessage;


        public string Path
        {
            get;
            set;
        }


        public bool HasImage
        {
            get { return Image != null; }
        }


        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                RaisePropertyChanged(() => Image);
                RaisePropertyChanged(() => HasImage);
            }
        }


        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }


        public string Error
        {
            get {return _errorMessage;}
            set
            {
                _errorMessage = value;
                RaisePropertyChanged(() => Error);
                RaisePropertyChanged(() => HasError);
            }
        }


        public bool HasError
        {
            get { return !string.IsNullOrWhiteSpace(_errorMessage); }
        }




        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            try
            {
                var me = (propertyExpression.Body as MemberExpression);

                var evt = PropertyChanged;
                if (me != null && me.Member != null && !string.IsNullOrWhiteSpace(me.Member.Name) && evt != null)
                    evt(this, new PropertyChangedEventArgs(me.Member.Name));
            }
            catch { }
        }


        public void Dispose()
        {
            try
            {
                if (Image != null && Image.StreamSource != null)
                    Image.StreamSource.Dispose();

                Image = null;
            }
            catch { }
        }
    }
}
