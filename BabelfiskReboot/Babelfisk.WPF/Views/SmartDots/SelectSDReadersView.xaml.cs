using Anchor.Core;
using Anchor.Core.Comparers;
using Babelfisk.ViewModels.SmartDots;
using Babelfisk.WPF.Infrastructure.DataGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Babelfisk.WPF.Views.SmartDots
{
    /// <summary>
    /// Interaction logic for SelectSDReadersView.xaml
    /// </summary>
    public partial class SelectSDReadersView : UserControl
    {
        private Key? _keyLastPressedKey;

        public SelectSDReadersViewModel ViewModel
        {
            get { return this.DataContext as SelectSDReadersViewModel; }
        }

        public SelectSDReadersView()
        {
            InitializeComponent();
        }

        private void selectAllCheckBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        protected void KeyDownHandler(object sender, KeyEventArgs e)
        {
            var isNumber = e.Key.IsNumber(true);
            var isLetter = e.Key.IsLetterOrSpace();

            _keyLastPressedKey = e.Key;
            if (e.IsDown && (e.Key == Key.Return || isNumber || isLetter))
            {
                DataGridCell cell = null;
                //Enter datagrid editing mode when hitting enter on a cell
                if (!dataGrid.IsEditing())
                {
                    dataGrid.BeginEdit();
                    e.Handled = true; //Don't move focus to next row
                }
                else if (!dataGrid.IsCellEditing(ref cell) && cell != null)
                {
                    cell.IsEditing = true;
                    if (e.Key != Key.Return)
                        e.Handled = true;
                }
            }
            else if (e.IsDown && e.Key == Key.Escape)
            {
                if (dataGrid.IsEditing())
                {
                    dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                }
            }
        }

        private void TextBox_Initialized(object sender, EventArgs e)
        {
            dataGrid.TextBox_Initialized(sender, e, _keyLastPressedKey);
        }


        private void FilteredComboBox_Initialized(object sender, EventArgs e)
        {
            dataGrid.FilteredComboBox_Initialized(sender, e, _keyLastPressedKey);

        }

        private void CheckBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void CheckBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
            var vm = (sender as CheckBox).DataContext as SDReaderItem;
            vm.IsSelected = !vm.IsSelected;
            e.Handled = true;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                foreach (var item in e.AddedItems.OfType<SDReaderItem>())
                {
                    //System.Diagnostics.Debug.WriteLine("** Item {0} is added to selection.", item.SelectionAnimal.AnimalId);

                    if (!item.IsSelected)
                    {
                        // if bound data item still isn't selected, fix this
                        item.IsSelected = true;
                    }
                }
            }
            if (e.RemovedItems != null)
            {
                foreach (var item in e.RemovedItems.OfType<SDReaderItem>())
                {
                    //System.Diagnostics.Debug.WriteLine("** Item {0} is removed from selection.", item.SelectionAnimal.AnimalId);

                    if (item.IsSelected)
                    {
                        // if bound data item still is selected, fix this
                        item.IsSelected = false;
                    }
                }
            }

            e.Handled = true;
        }

    }
}
