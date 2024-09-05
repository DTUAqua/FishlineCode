
using System;
using System.Windows;
using Babelfisk.ViewModels;

namespace Babelfisk.WPF.Infrastructure.Behaviors
{
    /// <summary>
    /// Defines a wrapper for the <see cref="Window"/> class that implements the <see cref="IWindow"/> interface.
    /// </summary>
    public class WindowWrapper : IWindow
    {
        private readonly Window window;

        /// <summary>
        /// Initializes a new instance of <see cref="WindowWrapper"/>.
        /// </summary>
        public WindowWrapper(DependencyObject doOwner)
        {
            this.window = new Window();
            this.window.BorderThickness = new Thickness(1);
            this.window.BorderBrush = System.Windows.Media.Brushes.Gray;
            if (doOwner is Window)
            {
                this.window.Owner = doOwner as Window;
                this.window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
        }

        /// <summary>
        /// Ocurrs when the <see cref="Window"/> is closed.
        /// </summary>
        public event EventHandler Closed
        {
            add { this.window.Closed += value; }
            remove { this.window.Closed -= value; }
        }

        /// <summary>
        /// Gets or Sets the content for the <see cref="Window"/>.
        /// </summary>
        public object Content
        {
            get { return this.window.Content; }
            set 
            { 
                this.window.Content = value;
                this.window.DataContext = value == null ? null : (value as FrameworkElement).DataContext;

                if (this.window.DataContext != null && typeof(IDomainObjectWindow).IsAssignableFrom(this.window.DataContext.GetType()))
                {
                    Action<IDomainObjectWindow> vHandler = null;

                    vHandler = new Action<IDomainObjectWindow>(delegate(IDomainObjectWindow iWin)
                    {
                        Close();
                    });

                    EventHandler closedEvt = null;
                    closedEvt = (sender, e) =>
                    {
                        try
                        {
                            this.window.Closed -= closedEvt;
                            ((IDomainObjectWindow)this.window.DataContext).RequestClose -= vHandler;
                        }
                        catch { }
                    };

                    this.window.Closed += closedEvt;


                    ((IDomainObjectWindow)this.window.DataContext).RequestClose += vHandler;
                }
            }
        }

        protected void WindowWrapper_RequestClose(IDomainObjectWindow iDomainObjectWindow)
        {
           
        }

        /// <summary>
        /// Gets or Sets the <see cref="Window.Owner"/> control of the <see cref="Window"/>.
        /// </summary>
        public object Owner
        {
            get { return this.window.Owner; }
            set { this.window.Owner = value as Window; }
        }

        /// <summary>
        /// Gets or Sets the <see cref="FrameworkElement.Style"/> to apply to the <see cref="Window"/>.
        /// </summary>
        public Style Style
        {
            get { return this.window.Style; }
            set { this.window.Style = value; }
        }

        /// <summary>
        /// Opens the <see cref="Window"/>.
        /// </summary>
        public void Show()
        {
            this.window.Show();
        }


        public void ShowDialog()
        {
            this.window.ShowDialog();
        }


        public Window Window
        {
            get { return window; }
        }

        /// <summary>
        /// Closes the <see cref="Window"/>.
        /// </summary>
        public void Close()
        {
            this.window.Close();
        }
    }
}