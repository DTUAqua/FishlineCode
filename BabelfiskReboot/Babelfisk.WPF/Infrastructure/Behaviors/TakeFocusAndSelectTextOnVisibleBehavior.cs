using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Babelfisk.WPF.Infrastructure.Behaviors
{
    public class TakeFocusAndSelectTextOnVisibleBehavior : TriggerAction<TextBox>
    {
        protected override void Invoke(object parameter)
        {
            Dispatcher.BeginInvoke(
                DispatcherPriority.Loaded,
                new Action(() =>
                {
                    AssociatedObject.Focus();
                    AssociatedObject.SelectAll();
                }));
        }
    }
}
