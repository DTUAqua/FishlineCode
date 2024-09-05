using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Babelfisk.ViewModels.Map;
using Anchor.Core;
using System.ComponentModel;


namespace Babelfisk.WPF.Views.Map
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl, IDisposable
    {
        public IMapViewModel ViewModel
        {
            get { return this.DataContext as IMapViewModel; }
        }

        public MapView()
        {
            InitializeComponent();

            this.DataContextChanged += MapView_DataContextChanged;

            if (DesignerProperties.GetIsInDesignMode(this))
                this.Visibility = System.Windows.Visibility.Collapsed;
         
        }

        protected void MapView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IMapViewModel mvm = null;
            if (e.NewValue != null && (mvm = e.NewValue as IMapViewModel) != null)
            {
                if (mvm.IsWindow)
                {
                    //Clear width and height bindings so the map will fill out the whole window.
                    BindingOperations.ClearBinding(mapContent, TextBox.WidthProperty);
                    BindingOperations.ClearBinding(mapContent, TextBox.HeightProperty);
                }
            }
        }

        private void ClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            new Action(() =>
            {
                try
                {
                   // ToolTipService.
                    Bitmap bmp = GetBrowserScreenshot();
                    var bmpSource = bmp.ToBitmapSource();

                    Clipboard.SetImage(bmpSource);
                }
                catch (Exception ex)
                {
                    if (ViewModel != null)
                        ViewModel.AppRegionManager.ShowMessageBox("En uventet fejl opstod. " + ex.Message);
                }
            }).Dispatch();
        }

        private void ScreenShotButton_Click(object sender, RoutedEventArgs e)
        {
            new Action(() =>
            {
                try
                {
                    Bitmap bmp = GetBrowserScreenshot();

                    Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                    sfd.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|GIF (*.gif)|*.gif|Bitmap (*.bmp)|*.bmp|All Files|*.*";
                    bool? blnRes = sfd.ShowDialog(Application.Current.MainWindow);

                    if (blnRes.HasValue && blnRes.Value)
                        bmp.Save(sfd.FileName);
                }
                catch (Exception ex)
                {
                    if (ViewModel != null)
                        ViewModel.AppRegionManager.ShowMessageBox("En uventet fejl opstod. " + ex.Message);
                }
            }).Dispatch();
        }


        private void Print_Click(object sender, RoutedEventArgs e)
        {
            new Action(() =>
            {
                try
                {
                    Bitmap bmp = GetBrowserScreenshot();
                    var bmpSource = bmp.ToBitmapSource();

                    PrintDialog dlg = new PrintDialog();
                    bool? result = dlg.ShowDialog();

                    if (result.HasValue && result.Value)
                    {
                        var img = new System.Windows.Controls.Image();
                        img.Source = bmpSource;

                        img.Measure(new System.Windows.Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight));
                        img.Arrange(new Rect(new System.Windows.Point(0, 0), img.DesiredSize));

                        dlg.PrintVisual(img, "Map");
                    }
                }
                catch (Exception ex)
                {
                    if (ViewModel != null)
                        ViewModel.AppRegionManager.ShowMessageBox("En uventet fejl opstod. " + ex.Message);
                }
            }).Dispatch();
        }


        private Bitmap GetBrowserScreenshot()
        {
            var topLeftCorner = mapBrowser.PointToScreen(new System.Windows.Point(0, 0));
            var topLeftGdiPoint = new System.Drawing.Point((int)topLeftCorner.X, (int)topLeftCorner.Y);
            var size = new System.Drawing.Size((int)mapBrowser.ActualWidth, (int)mapBrowser.ActualHeight);

            Bitmap screenShot = new Bitmap((int)mapBrowser.ActualWidth, (int)mapBrowser.ActualHeight);

            using (var graphics = Graphics.FromImage(screenShot))
            {
                graphics.CopyFromScreen(topLeftGdiPoint, new System.Drawing.Point(),
                     size, CopyPixelOperation.SourceCopy);
            }

            return screenShot;
        }

        public void Dispose()
        {
            try
            {
                mapBrowser.Dispose();

                this.DataContext = null;
            }
            catch { }
        }
    }
}
