using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Babelfisk.WPF.Infrastructure.Behaviors.DataGrid
{
    public class DataGridColumnsBehavior
    {
        public static readonly DependencyProperty BindableColumnsHeaderStyleProperty = DependencyProperty.RegisterAttached("BindableColumnsHeaderStyle", typeof(Style), typeof(DataGridColumnsBehavior), new UIPropertyMetadata(null));

        public static readonly DependencyProperty BindableColumnsProperty = DependencyProperty.RegisterAttached("BindableColumns", typeof(ObservableCollection<DataGridColumn>), typeof(DataGridColumnsBehavior), new UIPropertyMetadata(null, BindableColumnsPropertyChanged));

        public static readonly DependencyProperty StaticColumnsCountProperty = DependencyProperty.RegisterAttached("StaticColumnsCount", typeof(int), typeof(DataGridColumnsBehavior), new UIPropertyMetadata(0));
        

        private static void BindableColumnsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            System.Windows.Controls.DataGrid dataGrid = source as System.Windows.Controls.DataGrid;
            ObservableCollection<DataGridColumn> columns = e.NewValue as ObservableCollection<DataGridColumn>;

            int iCount = (int)source.GetValue(StaticColumnsCountProperty);

            while (dataGrid.Columns.Count > iCount)
                dataGrid.Columns.RemoveAt(dataGrid.Columns.Count - 1);

          //  dataGrid.Columns.Clear();
            if (columns == null)
            {
                return;
            }

            Style s = source.GetValue(BindableColumnsHeaderStyleProperty) as Style;

            foreach (DataGridColumn column in columns)
            {
                if (s != null)
                    column.HeaderStyle = s;

                dataGrid.Columns.Add(column);
            }
            columns.CollectionChanged += (sender, e2) =>
            {
                NotifyCollectionChangedEventArgs ne = e2 as NotifyCollectionChangedEventArgs;
                if (ne.Action == NotifyCollectionChangedAction.Reset)
                {
                    dataGrid.Columns.Clear();

                    if(ne.NewItems != null)
                        foreach (DataGridColumn column in ne.NewItems)
                        {
                            if (s != null)
                                column.HeaderStyle = s;

                            dataGrid.Columns.Add(column);
                        }
                }
                else if (ne.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (DataGridColumn column in ne.NewItems)
                    {
                        if (s != null)
                            column.HeaderStyle = s;

                        dataGrid.Columns.Add(column);
                    }
                }
                else if (ne.Action == NotifyCollectionChangedAction.Move)
                {
                    dataGrid.Columns.Move(ne.OldStartingIndex, ne.NewStartingIndex);
                }
                else if (ne.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (DataGridColumn column in ne.OldItems)
                    {
                        dataGrid.Columns.Remove(column);
                    }
                }
                else if (ne.Action == NotifyCollectionChangedAction.Replace)
                {
                    var vColumn = ne.NewItems[0] as DataGridColumn;

                    if (s != null)
                        vColumn.HeaderStyle = s;

                    dataGrid.Columns[ne.NewStartingIndex] = vColumn;
                }
            };
        }


        public static void SetStaticColumnsCount(DependencyObject element, int value)
        {
            element.SetValue(StaticColumnsCountProperty, value);
        }

        public static int GetStaticColumnsCount(DependencyObject element)
        {
            return (int)element.GetValue(StaticColumnsCountProperty);
        }


        public static void SetBindableColumnsHeaderStyle(DependencyObject element, Style value)
        {
            element.SetValue(BindableColumnsHeaderStyleProperty, value);
        }

        public static Style GetBindableColumnsHeaderStyle(DependencyObject element)
        {
            return (Style)element.GetValue(BindableColumnsHeaderStyleProperty);
        }



        public static void SetBindableColumns(DependencyObject element, ObservableCollection<DataGridColumn> value)
        {
            element.SetValue(BindableColumnsProperty, value);
        }

        public static ObservableCollection<DataGridColumn> GetBindableColumns(DependencyObject element)
        {
            return (ObservableCollection<DataGridColumn>)element.GetValue(BindableColumnsProperty);
        }



        public static void SetIsReadOnly(DependencyObject obj, bool isReadOnly)
        {
            obj.SetValue(IsReadOnlyProperty, isReadOnly);
        }

        public static bool GetIsReadOnly(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsReadOnlyProperty);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.RegisterAttached("IsReadOnly", typeof(bool), typeof(DataGridColumnsBehavior), new UIPropertyMetadata(true, Callback));

        private static void Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataGridTextColumn)d).IsReadOnly = (bool)e.NewValue;
        }
    }
}
