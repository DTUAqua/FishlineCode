using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Collections.Specialized;

namespace FishLineMeasure.WPF.Infrastructure.Regions
{
     [Export]
    public class ContentControlRegionAdapter : RegionAdapterBase<ContentControl>
    {
        public delegate void DockableContentHandler();

        [ImportingConstructor]
        public ContentControlRegionAdapter([Import] IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override IRegion CreateRegion()
        {
            return new Region();
        }

        protected override void Adapt(IRegion region, ContentControl regionTarget)
        {
            region.Views.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
            {
                OnViewsCollectionChanged(sender, e, region, regionTarget);
            };
        }

        private void OnViewsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e, IRegion region, ContentControl regionTarget)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        var newContent = item as ContentControl;

                       /* if (newContent == null)
                        {
                            newContent = new ContentControl();
                            newContent.Content = item;
                          
                            if ((item is FrameworkElement) && (item as FrameworkElement).DataContext is AViewModel)
                            {
                                newContent.Title = ((AViewModel)((FrameworkElement)item).DataContext).WindowTitle;
                                newContent.IsCloseable = ((AViewModel)((FrameworkElement)item).DataContext).IsCloseable;
                            }
                        }

                        if (newContent != null)
                        {
                            regionTarget.Items.Add(newContent);
                            newContent.InvalidateParents();
                            regionTarget.SelectedItem = newContent;
                        }

                        if(newContent.Content is FrameworkElement && (newContent.Content as FrameworkElement).DataContext is AViewModel)
                        {
                            AViewModel avm = ((AViewModel)(newContent.Content as FrameworkElement).DataContext);
                            newContent.IsCloseable = avm.IsCloseable;
                            newContent.Closing += (send, args) => avm.FireClosing(send, args);
                            newContent.Closed += (send, args) => avm.FireClosed(send, args);
                            avm.RequestClose += (av) =>
                            {
                                regionTarget.Items.Remove(newContent); 
                                newContent.InvalidateParents();
                            };
                        }*/
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    /*foreach (object item in e.OldItems)
                    {
                        regionTarget.Items.Remove(item);
                        var vItem = regionTarget.Items.OfType<DocumentContent>().Where(x => x.Content == item);
                        
                        if(vItem.Count() > 0)
                            regionTarget.Items.Remove(vItem.First());
                    }*/
                    break;
            }
        }

        void newContent_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
