using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Babelfisk.WPF.Windows
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        /// <summary>
        /// Identifies the <see cref="Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(LoadingWindow), new PropertyMetadata(""));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        public static readonly DependencyProperty ProgressBarVisibilityProperty = DependencyProperty.Register("ProgressBarVisibility", typeof(Visibility), typeof(LoadingWindow), new PropertyMetadata(Visibility.Visible));

        public Visibility ProgressBarVisibility
        {
            get { return (Visibility)GetValue(ProgressBarVisibilityProperty); }
            set { SetValue(ProgressBarVisibilityProperty, value); }
        }



        public LoadingWindow()
        {
            InitializeComponent();
        }


        public static LoadingWindow Show(Window wOwner, string strMessage, bool blnShowProgressBar)
        {
            LoadingWindow msgWindow = new LoadingWindow();
            msgWindow.Owner = wOwner;

            if (!blnShowProgressBar)
                msgWindow.ProgressBarVisibility = Visibility.Collapsed;

            msgWindow.Header = strMessage;
            msgWindow.Show();

            return msgWindow;
        }
    }
}
