using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace FishLineMeasure.WPF.Windows
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window, INotifyPropertyChanged
    {
        private DispatcherTimer m_timer;


        #region Properties


        /// <summary>
        /// Identifies the <see cref="Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MessageWindow), new PropertyMetadata(""));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }


        /// <summary>
        /// Identifies the <see cref="Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageBoxButtonProperty = DependencyProperty.Register("MessageBoxButton", typeof(System.Windows.MessageBoxButton), typeof(MessageWindow), new PropertyMetadata(System.Windows.MessageBoxButton.OK));

        public System.Windows.MessageBoxButton MessageBoxButton
        {
            get { return (System.Windows.MessageBoxButton)GetValue(MessageBoxButtonProperty); }
            set { SetValue(MessageBoxButtonProperty, value); }
        }


        /// <summary>
        /// Identifies the <see cref="Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register("Seconds", typeof(int), typeof(MessageWindow), new PropertyMetadata(0));

        public int Seconds
        {
            get { return (int)GetValue(SecondsProperty); }
            set
            {
                SetValue(SecondsProperty, value);
                OnPropertyChanged("IsTimerRunning");
            }
        }


        public bool IsTimerRunning
        {
            get
            {
                return Seconds > 0;
            }
        }


        public MessageBoxResult MessageBoxResult
        {
            get;
            set;
        }


        #endregion

      
        protected MessageWindow()
        {
            InitializeComponent();
        }


        #region Timer methods


        public void StartTimer(int intSeconds)
        {
            m_timer = new DispatcherTimer(DispatcherPriority.Background);
            m_timer.Tick += new EventHandler(m_timer_Tick);

            Seconds = intSeconds;
            m_timer.Interval = new TimeSpan(0, 0, 1);
            m_timer.IsEnabled = true;
            m_timer.Start();
        }


        protected void m_timer_Tick(object sender, EventArgs e)
        {
            Seconds = Seconds - 1;

            if (Seconds == 0)
            {
                StopTimer();
                this.Close();
            }
        }


        private void StopTimer()
        {
            try
            {
                if (m_timer != null)
                {
                    m_timer.Stop();
                    m_timer.IsEnabled = false;
                    m_timer = null;
                }
            }
            catch { }
        }


        #endregion



        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            StopTimer();

            base.OnClosing(e);

            //Make sure owner windows is in focus on close (and put it in a try-catch block to ensure this will never kill the app). 
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (Owner != null)
                        Owner.Activate();
                }
                catch { }
            }));
        }

        private static MessageWindow _msgWindow;

        private static void CheckAndCloseOpenWindow()
        {
            if (_msgWindow != null)
            {
                try
                {
                    _msgWindow.Close();
                }
                catch { }
            }
        }

        public static MessageBoxResult Show(Window wOwner, string strMessage, MessageBoxButton enmButtons)
        {
            CheckAndCloseOpenWindow();

            _msgWindow = new MessageWindow();
            _msgWindow.MessageBoxButton = enmButtons;
           // _msgWindow.Owner = wOwner;
            _msgWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _msgWindow.Message = strMessage;
            _msgWindow.ShowDialog();

            return _msgWindow.MessageBoxResult;
        }

        public static MessageBoxResult Show(Window wOwner, object xaml, MessageBoxButton enmButtons)
        {
            CheckAndCloseOpenWindow();

            _msgWindow = new MessageWindow();
            _msgWindow.MessageBoxButton = enmButtons;
            _msgWindow.Owner = wOwner;

            ContentControl c = new ContentControl();
           
            _msgWindow.spContent.Children.Clear();
            _msgWindow.spContent.Children.Add(c);
            c.Content = xaml;
            _msgWindow.ShowDialog();

            return _msgWindow.MessageBoxResult;
        }



        public static void Show(Window wOwner, string strMessage, int? intSecondsTimeout = null)
        {
            CheckAndCloseOpenWindow();

            _msgWindow = new MessageWindow();
            _msgWindow.MessageBoxButton = MessageBoxButton.OK;
            _msgWindow.Owner = wOwner;

            _msgWindow.Message = strMessage;
            if (intSecondsTimeout != null)
                _msgWindow.StartTimer(intSecondsTimeout.Value);
            _msgWindow.Show();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxResult = System.Windows.MessageBoxResult.OK;
            this.Close();
        }


        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxResult = System.Windows.MessageBoxResult.Yes;
            this.Close();
        }


        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxResult = System.Windows.MessageBoxResult.No;
            this.Close();
        }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxResult = System.Windows.MessageBoxResult.OK;
            this.Close();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxResult = System.Windows.MessageBoxResult.Cancel;
            this.Close();
        }



        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }
}
