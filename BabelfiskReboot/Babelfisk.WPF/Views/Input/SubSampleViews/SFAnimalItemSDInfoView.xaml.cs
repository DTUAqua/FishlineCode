using Babelfisk.ViewModels.Input;
using System;
using System.Collections.Generic;
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
using Babelfisk.BusinessLogic;
using Babelfisk.Entities.Sprattus;
using System.IO;

namespace Babelfisk.WPF.Views.Input.SubSampleViews
{
    /// <summary>
    /// Interaction logic for SFAnimalItemSDInfoView.xaml
    /// </summary>
    public partial class SFAnimalItemSDInfoView : UserControl, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isLoading = false;

        private bool _isLoadingImage = false;

        private bool _hasAnnotationInfo = false;

        private string _message;

        private string _eventReader = null;
        private string _imagePath = null;
        private string _imageName = null;
        private string _eventName = null;
        private string _eventId = null;
        private string _imageError = null;

        private bool _showReaderInfo = false;


        private BitmapImage _image;


        #region AnimalItem Property

        /// <summary>
        /// SelectedItem member property used for tooltip.
        /// </summary>
        public static readonly DependencyProperty AnimalItemProperty = DependencyProperty.Register("AnimalItem", typeof(AnimalItem), typeof(SFAnimalItemSDInfoView), new UIPropertyMetadata(null, AnimalItemChanged));


        [DefaultValue(null)]
        public AnimalItem AnimalItem
        {
            get
            {
                return this.GetValue(AnimalItemProperty) as AnimalItem;
            }
            set
            {
                this.SetValue(AnimalItemProperty, value);
            }
        }


        public static void AnimalItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var view = d as SFAnimalItemSDInfoView;

                if (view != null)
                {
                    view.Initialize();
                }
            }
            catch { }
        }


        #endregion


        #region Properties

        public string EventReader
        {
            get { return _eventReader; }
            set
            {
                _eventReader = value;
                RaisePropertyChanged(() => EventReader);
            }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                RaisePropertyChanged(() => ImagePath);
            }
        }


        public string ImageName
        {
            get { return _imageName; }
            set
            {
                _imageName = value;
                RaisePropertyChanged(() => ImageName);
            }
        }


        public string EventName
        {
            get { return _eventName; }
            set
            {
                _eventName = value;
                RaisePropertyChanged(() => EventName);
            }
        }


        public string EventIdString
        {
            get { return _eventId; }
            set
            {
                _eventId = value;
                RaisePropertyChanged(() => EventIdString);
            }
        }


        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged(() => Message);
                RaisePropertyChanged(() => HasMessage);
            }
        }


        public bool HasMessage
        {
            get { return !string.IsNullOrWhiteSpace(_message); }
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


        public bool IsLoadingImage
        {
            get { return _isLoadingImage; }
            set
            {
                _isLoadingImage = value;
                RaisePropertyChanged(() => IsLoadingImage);
            }
        }


        public string ImageError
        {
            get { return _imageError; }
            set
            {
                _imageError = value;
                RaisePropertyChanged(() => ImageError);
                RaisePropertyChanged(() => HasImageError);
            }
        }


        public bool HasImageError
        {
            get { return !string.IsNullOrWhiteSpace(_imageError); }
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


        public bool HasImage
        {
            get { return Image != null; }
        }


        public bool ShowReaderInfo
        {
            get { return _showReaderInfo; }
            set
            {
                _showReaderInfo = value;
                RaisePropertyChanged(() => ShowReaderInfo);
            }
        }

        public bool HasAnnotationInfo
        {
            get { return _hasAnnotationInfo; }
            set
            {
                _hasAnnotationInfo = value;
                RaisePropertyChanged(() => HasAnnotationInfo);
            }
        }


        #endregion


        public SFAnimalItemSDInfoView()
        {
            InitializeComponent();
        }


        private void Initialize()
        {
            try
            {
                IsLoading = true;
                var ani = AnimalItem;
                Task.Factory.StartNew(() => LoadData(ani)).ContinueWith(t =>
                {
                    new Action(() =>
                    {
                        IsLoading = false;
                        IsLoadingImage = false;
                    }).Dispatch();
                });
            }
            catch { }
        }


        private void LoadData(AnimalItem animalItem)
        {
            try
            {
                string message = null;

                if (animalItem == null || !animalItem.IsAgedByAquaDots || animalItem.SFAgeEntity == null)
                    message = ViewModels.AViewModel.Translate("SFAnimalItemSDInfoView", "AgeNotAssignedFromSD"); // "The age has not been assigned from SmartDots.";

                if (message != null)
                {
                    new Action(() =>
                    {
                        Message = message;
                    }).Dispatch();
                    return;
                }

                var age = animalItem.SFAgeEntity;

                message = ViewModels.AViewModel.Translate("SFAnimalItemSDInfoView", "AgeAssignedFromSD");  //"The age has been assigned from SmartDots.";
                string readerName = null;

                //Get DFU Person
                if (age.sdAgeReadId.HasValue)
                {
                    int userReadId = age.sdAgeReadId.Value;
                    var manLookup = new LookupManager();
                    var lv = new LookupDataVersioning();
                    var dfuPerson = manLookup.GetLookups(typeof(DFUPerson), lv).OfType<DFUPerson>().Where(x => x.dfuPersonId == userReadId).FirstOrDefault();
                    if (dfuPerson != null)
                        readerName = string.Format("{0} - {1}", dfuPerson.initials, dfuPerson.name ?? "");
                }

                string imagePath = null;
                string imageName = null;
                string eventName = null;
                string eventId = null;
                bool hasAnnotationInfo = age.sdAnnotationId.HasValue;

                if (hasAnnotationInfo)
                {
                    var man = new BusinessLogic.SmartDots.SmartDotsManager();
                    var sdAnno = man.GetSDAnnotation(age.sdAnnotationId.Value);

                    if (sdAnno != null)
                    {
                        var f = sdAnno.SDFile;
                        if (f != null)
                        {
                            imageName = f.fileName;
                            imagePath = System.IO.Path.Combine(f.path, f.fileName);

                            if (f.SDSample != null && f.SDSample.SDEvent != null)
                            {
                                eventName = f.SDSample.SDEvent.name;
                                eventId = f.SDSample.SDEvent.sdEventId.ToString();
                            }
                        }
                    }
                }
                else
                {
                    message = ViewModels.AViewModel.Translate("SFAnimalItemSDInfoView", "AgeAssignedFromSDNoAnnotation");  //"The age has been assigned from SmartDots but the annotation itself has since been deleted.";
                }

                //Show animal details before loading the image.
                new Action(() =>
                {
                    Message = message;
                    EventReader = readerName;
                    ImagePath = imagePath;
                    ImageName = imageName;
                    EventName = eventName;
                    EventIdString = eventId;
                    HasAnnotationInfo = hasAnnotationInfo;
                    ShowReaderInfo = true;
                    IsLoading = false;
                    IsLoadingImage = true;
                }).Dispatch();

                if (imagePath != null)
                {
                    string error = null;
                    byte[] arrImage = SmartDots.OtolithImagesPreview.GetImage(imagePath, ref error);

                    new Action(() =>
                    {
                        try
                        {
                            ImageError = error;

                            if (arrImage != null && arrImage.Length > 0)
                            {
                                MemoryStream ms = new MemoryStream(arrImage);
                                var img = SmartDots.OtolithImagesPreview.ConvertToBitmapImage(ms);

                                Image = img;
                            }
                        }
                        catch { }
                        finally
                        {
                            IsLoading = false;
                            IsLoadingImage = false;
                        }
                    }).Dispatch();
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
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


        public static void LoadPreviewWindow(object animalItem)
        {
            try
            {
                Window w = new Window();
                w.WindowStyle = WindowStyle.ToolWindow;
                w.Owner = Application.Current.MainWindow;
                w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                // w.Title = Babelfisk.ViewModels.AViewModel.Translate("Common", "ImagesParan");
                //  w.SizeToContent = SizeToContent.Height;

                w.Width = 700;
                w.Height = 600;

                var v = new SFAnimalItemSDInfoView();
                v.DataContext = animalItem;

                Binding bi = new Binding("");
                bi.Source = animalItem;

                v.SetBinding(SFAnimalItemSDInfoView.AnimalItemProperty, bi);

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
                // if (Image != null && Image.StreamSource != null)
                //     Image.StreamSource.Dispose();

                // Image = null;
            }
            catch { }
        }

    }
}
