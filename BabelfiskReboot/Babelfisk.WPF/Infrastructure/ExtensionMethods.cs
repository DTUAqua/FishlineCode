using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;

namespace Babelfisk.WPF
{
    public static class ExtensionMethods
    {
        public static TContainer GetContainerFromIndex<TContainer>(this ItemsControl itemsControl, int index) where TContainer : DependencyObject
        {
            return (TContainer)
              itemsControl.ItemContainerGenerator.ContainerFromIndex(index);
        }

        public static bool IsEditing(this DataGrid dataGrid)
        {
            return dataGrid.GetEditingRow() != null;
        }

        public static bool IsCellEditing(this DataGrid dataGrid, ref DataGridCell cell)
        {
            cell = GetCurrentCell(dataGrid);
            return cell == null ? false : cell.IsEditing;
        }

        public static DataGridRow GetEditingRow(this DataGrid dataGrid)
        {
            var sIndex = dataGrid.SelectedIndex;
            if (sIndex >= 0)
            {
                var selected = dataGrid.GetContainerFromIndex<DataGridRow>(sIndex);
                if (selected != null && selected.IsEditing) return selected;
            }

            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                if (i == sIndex) continue;
                var item = dataGrid.GetContainerFromIndex<DataGridRow>(i);
                if (item == null)
                    continue;
                if (item.IsEditing) return item;
            }

            return null;
        }


        public static DataGridCell GetCurrentCell(DataGrid SourceDataGrid)
        {
            if (SourceDataGrid.CurrentCell == null)
                return null;

            var RowContainer = SourceDataGrid.ItemContainerGenerator.ContainerFromItem(SourceDataGrid.CurrentCell.Item);
            if (RowContainer == null)
                return null;

            var RowPresenter = GetVisualChild<System.Windows.Controls.Primitives.DataGridCellsPresenter>(RowContainer);
            if (RowPresenter == null)
                return null;

            int columnIndex = SourceDataGrid.Columns.IndexOf(SourceDataGrid.CurrentCell.Column);

            DataGridCell Cell = null;
            DependencyObject Container = null;

            if (columnIndex > -1)
            {
                Container = RowPresenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                //var Container = RowPresenter.ItemContainerGenerator.ContainerFromItem(SourceDataGrid.CurrentCell.Item); //This statement failed on .net 4.0 only machines (the ones without 4.5) - used above statement instead.
                Cell = Container as DataGridCell;
            }

            // Try to get the cell if null, because maybe the cell is virtualized
            if (Cell == null && Container != null)
            {
                SourceDataGrid.ScrollIntoView(RowContainer, SourceDataGrid.CurrentCell.Column);
                Container = RowPresenter.ItemContainerGenerator.ContainerFromItem(SourceDataGrid.CurrentCell.Item);
                Cell = Container as DataGridCell;
            }

            return Cell;
        }


        public static TRet GetVisualChild<TRet>(DependencyObject Target) where TRet : DependencyObject
        {
            if (Target == null)
                return null;

            for (int ChildIndex = 0; ChildIndex < VisualTreeHelper.GetChildrenCount(Target); ChildIndex++)
            {
                var Child = VisualTreeHelper.GetChild(Target, ChildIndex);

                if (Child != null && Child is TRet)
                    return (TRet)Child;
                else
                {
                    TRet childOfChild = GetVisualChild<TRet>(Child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }

        public static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }


        public static void Kill(this Map map)
        {
            try
            {
                TypeDescriptor.Refresh(map);

                map.Dispose();

                var configType = typeof(Microsoft.Maps.MapControl.WPF.Core.MapConfiguration);

                var configuration = configType.GetField("configuration", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);

                var requestQueue = configuration.GetFieldValue("requestQueue");

                var values = (System.Collections.IEnumerable)requestQueue.GetPropertyValue("Values");

                foreach (System.Collections.IEnumerable requests in values)
                    foreach (var request in requests.OfType<object>().ToList())
                    {
                        var target = request.GetPropertyValue("Callback").GetPropertyValue("Target");

                        if (target == map)
                            requests.ExecuteMethod("Remove", request);
                        else if (target is DependencyObject)
                        {
                            var d = (DependencyObject)target;

                            if (d.HasParentOf(map))
                                requests.ExecuteMethod("Remove", request);
                        }
                    }
            }
            catch { }
        }

        private static Object GetFieldValue(this Object obj, String fieldName)
        {
            var type = obj.GetType();

            return type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(obj);
        }

        private static Object GetPropertyValue(this Object obj, String fieldName)
        {
            var type = obj.GetType();

            return type.GetProperty(fieldName, BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | BindingFlags.Instance).GetValue(obj, new object[] { });
        }

        private static Object ExecuteMethod(this Object obj, String methodName, params object[] parameters)
        {
            var type = obj.GetType();

            return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(obj, parameters);
        }

        private static Boolean HasParentOf(this DependencyObject obj, DependencyObject parent)
        {
            if (obj == null)
                return false;

            if (obj == parent)
                return true;

            return VisualTreeHelper.GetParent(obj).HasParentOf(parent);
        }
    }
}
