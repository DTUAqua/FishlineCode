using System;
using System.Collections.Generic;
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
using Babelfisk.ViewModels.Reporting.ReportExecuteModels;
using Anchor.Core;

namespace Babelfisk.WPF.Views.Reporting.ReportExecuteViews
{
    /// <summary>
    /// Interaction logic for SelectParametersView.xaml
    /// </summary>
    public partial class SelectParametersView : UserControl
    {
        public SelectParametersViewModel ViewModel
        {
            get { return this.DataContext as SelectParametersViewModel; }
        }

        public SelectParametersView()
        {
            InitializeComponent();

            this.Loaded += SelectParametersView_Loaded;
        }

        void SelectParametersView_Loaded(object sender, RoutedEventArgs e)
        {
            new Action(() =>
            {
              //  if (ViewModel != null && !ViewModel.IsInTestMode)
              //      rowScript.Height = new GridLength(Double.NaN, GridUnitType.Auto);

                var w = Window.GetWindow(this);
                w.SizeToContent = SizeToContent.Manual;
                w.Height = w.ActualHeight;
            }).Dispatch();
        }

      /*  private void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            new Action(() =>
            {
                if (sender is ItemsControl && (sender as ItemsControl).Items.Count > 0)
                {
                    var itemsControl = sender as ItemsControl;
                    var contentPresenter = (ContentPresenter)itemsControl.ItemContainerGenerator.ContainerFromIndex(0);

                    var ctrl = contentPresenter.ContentTemplate.FindName("ctrlInput", contentPresenter) as Control;

                    if (ctrl != null)
                    {
                        ctrl.Focus();
                       // Keyboard.Focus(ctrl);
                    }
                }
            }).Dispatch();
        }*/


        private void tbInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.FireParameterChanged((sender as FrameworkElement).Tag as ParameterViewModel);
        }


        private void cbxInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.FireParameterChanged((sender as FrameworkElement).Tag as ParameterViewModel);
        }

        private void mcbxInput_OnOpenChanged(Anchor.Core.Controls.DropDownListBox obj)
        {
            if (ViewModel != null && !obj.IsOpen)
                ViewModel.FireParameterChanged(obj.Tag as ParameterViewModel);
        }

        
    }
}
