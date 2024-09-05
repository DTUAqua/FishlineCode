using Anchor.Core;
using Babelfisk.Entities.Sprattus;
using Babelfisk.ViewModels.SmartDots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddEditSDEventView.xaml
    /// </summary>
    public partial class AddEditSDEventView : UserControl
    {
        private Key? _keyLastPressedKey;

        public AddEditSDEventView()
        {
            InitializeComponent();
        }

        #region input constrain to numbers only
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void PreviewTextInputNumbers(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = !IsTextAllowed(e.Text);
        }

        #endregion

        private void CheckBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void CheckBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
            var vm = (sender as CheckBox).DataContext as R_SDEventSDReader;
            vm.primaryReader = !vm.primaryReader;
            e.Handled = true;
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


    }
}
