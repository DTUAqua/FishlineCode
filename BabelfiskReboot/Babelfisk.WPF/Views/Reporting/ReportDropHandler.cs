using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Anchor.Core.Controls.DragDrop;
using Anchor.Core.Controls.DragDrop.Utilities;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;
using Babelfisk.ViewModels.Reporting;

namespace Babelfisk.WPF.Views.Reporting
{
    public class ReportDropHandler : DependencyObject, IDropTarget
    {
        public static readonly DependencyProperty TreeViewModelProperty = DependencyProperty.Register("TreeViewModel", typeof(ReportsTreeViewModel), typeof(ReportDropHandler), new UIPropertyMetadata(null, TreeViewModelChangedCallback));
        public ReportsTreeViewModel TreeViewModel
        {
            get
            {
                return (ReportsTreeViewModel)GetValue(TreeViewModelProperty);
            }
            set
            {
                SetValue(TreeViewModelProperty, value);
            }
        }

        public static void TreeViewModelChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public ReportDropHandler()
        {

        }


        /// <summary>
        /// Updates the current drag state.
        /// </summary>
        /// <param name="dropInfo">Information about the drag.</param>
        /// <remarks>
        /// To allow a drop at the current drag position, the <see cref="DropInfo.Effects" /> property on
        /// <paramref name="dropInfo" /> should be set to a value other than <see cref="DragDropEffects.None" />
        /// and <see cref="DropInfo.Data" /> should be set to a non-null value.
        /// </remarks>
        public virtual void DragOver(IDropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                // when source is the same as the target set the move effect otherwise set the copy effect
                var moveData = dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget
                               || !dropInfo.KeyStates.HasFlag(dropInfo.DragInfo.DragDropCopyKeyState)
                               || dropInfo.DragInfo.VisualSourceItem is TabItem
                               || dropInfo.DragInfo.VisualSourceItem is TreeViewItem
                               || dropInfo.DragInfo.VisualSourceItem is MenuItem
                               || dropInfo.DragInfo.VisualSourceItem is ListBoxItem;
                dropInfo.Effects = moveData ? DragDropEffects.Move : DragDropEffects.Copy;
                var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter) && dropInfo.VisualTargetItem is TreeViewItem;
                dropInfo.DropTargetAdorner = isTreeViewItem ? DropTargetAdorners.Highlight : DropTargetAdorners.Insert;
            }
        }

        /// <summary>
        /// Performs a drop.
        /// </summary>
        /// <param name="dropInfo">Information about the drop.</param>
        public virtual void Drop(IDropInfo dropInfo)
        {
            if (dropInfo == null 
                || dropInfo.DragInfo == null 
                || (dropInfo.TargetItem != null && !(dropInfo.TargetItem is ReportingTreeNode))
                || !(dropInfo.Data is INodeItem))
            {
                return;
            }

            var target = dropInfo.TargetItem as ReportingTreeNode;
            var source = dropInfo.Data as INodeItem;

            var vm = new ViewModels.Reporting.AddEditModels.MoveTreeNodeViewModel(source, target);
            vm.OnSaved += vm_OnSaved;
            ViewModels.Reporting.AddEditModels.MoveTreeNodeViewModel.ShowDialog(vm);
        }

        void vm_OnSaved(ViewModels.Reporting.AddEditModels.MoveTreeNodeViewModel obj)
        {
            obj.OnSaved -= vm_OnSaved;

            if (TreeViewModel != null)
                TreeViewModel.RefreshTreeAsync(obj.Source);
        }

        /// <summary>
        /// Test the specified drop information for the right data.
        /// </summary>
        /// <param name="dropInfo">The drop information.</param>
        public static bool CanAcceptData(IDropInfo dropInfo)
        {
            if (dropInfo == null || dropInfo.DragInfo == null)
                return false;

            if (!dropInfo.IsSameDragDropContextAsSource)
                return false;

            if (dropInfo.TargetItem is Report)
                return false;

            int intTargetId = dropInfo.TargetItem == null ? -1 : (dropInfo.TargetItem as ReportingTreeNode).reportingTreeNodeId;
            int intSourceParentId = -1;
            if(dropInfo.Data is Report && (dropInfo.Data as Report).ReportingTreeNodes.Count > 0)
                intSourceParentId =  (dropInfo.Data as Report).ReportingTreeNodes.FirstOrDefault().reportingTreeNodeId;
            else if(dropInfo.Data is ReportingTreeNode && (dropInfo.Data as ReportingTreeNode).parentTreeNodeId.HasValue)
                intSourceParentId = (dropInfo.Data as ReportingTreeNode).parentTreeNodeId.Value;

            if (intSourceParentId == intTargetId)
                return false;

            // do not drop on itself
            var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter)
                                 && dropInfo.VisualTargetItem is TreeViewItem;
            if (isTreeViewItem && dropInfo.VisualTargetItem == dropInfo.DragInfo.VisualSourceItem)
            {
                return false;
            }

            if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
            {
                var targetList = dropInfo.TargetCollection.TryGetList();
                return targetList != null;
            }
            //      else if (dropInfo.DragInfo.SourceCollection is ItemCollection) {
            //        return false;
            //      }
            else if (dropInfo.TargetCollection == null)
            {
                return false;
            }
            else
            {
                if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data))
                {
                    var isChildOf = IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
                    return !isChildOf;
                }
                else
                {
                    return false;
                }
            }
        }

        public static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable && !(data is string))
            {
                return (IEnumerable)data;
            }
            else
            {
                return Enumerable.Repeat(data, 1);
            }
        }

        protected static bool IsChildOf(UIElement targetItem, UIElement sourceItem)
        {
            var parent = ItemsControl.ItemsControlFromItemContainer(targetItem);

            while (parent != null)
            {
                if (parent == sourceItem)
                {
                    return true;
                }

                parent = ItemsControl.ItemsControlFromItemContainer(parent);
            }

            return false;
        }

        protected static bool TestCompatibleTypes(IEnumerable target, object data)
        {
            TypeFilter filter = (t, o) =>
            {
                return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            };

            var enumerableInterfaces = target.GetType().FindInterfaces(filter, null);
            var enumerableTypes = from i in enumerableInterfaces select i.GetGenericArguments().Single();

            if (enumerableTypes.Count() > 0)
            {
                var dataType = TypeUtilities.GetCommonBaseClass(ExtractData(data));
                return enumerableTypes.Any(t => t.IsAssignableFrom(dataType));
            }
            else
            {
                return target is IList;
            }
        }
    }
}
