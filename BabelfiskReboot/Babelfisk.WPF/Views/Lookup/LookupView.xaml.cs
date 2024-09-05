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
using Babelfisk.Entities;
using Babelfisk.ViewModels;
using Babelfisk.ViewModels.Lookup;

namespace Babelfisk.WPF.Views.Lookup
{
    /// <summary>
    /// Interaction logic for LookupView.xaml
    /// </summary>
    public partial class LookupView : UserControl
    {
        public LookupView()
        {
            InitializeComponent();
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void grid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            AViewModel avm = this.DataContext as AViewModel;
            var lt = e.Column.Header as Babelfisk.ViewModels.Lookup.LookupColumn;
            ILookupEntity vm = (e.Row.Item as ILookupEntity);

            //Cancel cell-edit if column is readonly and the lookup is not new.
            if (lt != null && lt.IsReadOnly && vm.ChangeTracker.State != Entities.Sprattus.ObjectState.Added)
            {
                e.Cancel = true;
                return;
            }

            if (avm != null)
                avm.IsDirty = true;
        }

       
        private void GridCellKeyDown(object sender, KeyEventArgs e)
        {
            DataGridCell dgc = sender as DataGridCell;

            if (dgc != null && !dgc.IsEditing && (e.Key != Key.Right && e.Key != Key.Left && e.Key != Key.Up && e.Key != Key.Down))
            {
                grid.BeginEdit();
            }
        }


        private void gridChild_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var lt = e.Column.Header as Babelfisk.ViewModels.Lookup.LookupColumn;
            ILookupEntity vm = (e.Row.Item as ILookupEntity);

            //Cancel cell-edit if column is readonly and the lookup is not new.
            if (lt.IsReadOnly && vm.ChangeTracker.State != Entities.Sprattus.ObjectState.Added)
            {
                e.Cancel = true;
                return;
            }

            AViewModel avm = this.DataContext as AViewModel;
            if (avm != null)
            {
                avm.IsDirty = true;

                //Remember to set dirty flag on child lookup as well.
                if (avm is LookupViewModel)
                    (avm as LookupViewModel).ChildLookup.IsDirty = true;
            }
        }


        private void GridChildCellKeyDown(object sender, KeyEventArgs e)
        {
            DataGridCell dgc = sender as DataGridCell;

            if (dgc != null && !dgc.IsEditing)
            {
                grid.BeginEdit();
            }
        }


        private void OnRowMouseDown(object sender, MouseEventArgs e)
        {
            //Below makes sure the lookup is selected, even if the DataGridRow is disabled
            LookupViewModel avm = this.DataContext as LookupViewModel;
            var row = sender as DataGridRow;

            if (avm != null && row != null)
            {
                var itm = row.Item;

                if (avm.SelectedLookup != itm)
                    avm.SelectedLookup = itm as ILookupEntity;
            }
        }
    }
}
