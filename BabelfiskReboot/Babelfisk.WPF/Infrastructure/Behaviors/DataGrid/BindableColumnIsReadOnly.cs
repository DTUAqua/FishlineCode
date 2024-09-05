using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Babelfisk.WPF.Infrastructure.Behaviors.DataGrid
{
    public class BindableColumnIsReadOnly : Behavior<DataGridColumn>
    {
        /// <summary>
        /// The <see cref="Header" /> dependency property's name.
        /// </summary>
        public const string IsReadOnlyPropertyName = "IsReadOnly";

        /// <summary>
        /// Gets or sets the value of the <see cref="Header" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }
            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Header" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            IsReadOnlyPropertyName,
            typeof(bool),
            typeof(BindableColumnIsReadOnly),
            new PropertyMetadata(false, new PropertyChangedCallback(HandleHeaderBindingChanged)));

        private static void HandleHeaderBindingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var behave = obj as BindableColumnIsReadOnly;
            if (behave == null || behave.AssociatedObject == null)
                return;

            behave.AssociatedObject.Header = args.NewValue;
        }

        protected override void OnAttached()
        {
            if (this.AssociatedObject == null)
                return;

            this.AssociatedObject.IsReadOnly = this.IsReadOnly;

            base.OnAttached();
        }
    }

}
