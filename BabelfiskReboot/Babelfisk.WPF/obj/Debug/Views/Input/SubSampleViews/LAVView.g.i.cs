﻿#pragma checksum "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "4CFBD5F2A3ABF5C9EFDCBD7E75C835674A1870D8854EF1E37B4FCC5C72FFEB8D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Anchor.Core.Common;
using Anchor.Core.Controls;
using Anchor.Core.Controls.Behaviors;
using Anchor.Core.Controls.Sliders;
using Babelfisk.BusinessLogic.Settings;
using Babelfisk.Entities.Sprattus;
using Babelfisk.WPF.Converters;
using Babelfisk.WPF.Infrastructure;
using Babelfisk.WPF.Infrastructure.Behaviors;
using Babelfisk.WPF.Infrastructure.DataGrid;
using Babelfisk.WPF.Views.Input;
using Microsoft.Windows.Themes;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Babelfisk.WPF.Views.Input {
    
    
    /// <summary>
    /// LAVView
    /// </summary>
    public partial class LAVView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 18 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Babelfisk.WPF.Views.Input.LAVView uCtrl;
        
        #line default
        #line hidden
        
        
        #line 214 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid dpGrid;
        
        #line default
        #line hidden
        
        
        #line 226 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Anchor.Core.Controls.AncDataGrid dataGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Babelfisk.WPF;component/views/input/subsampleviews/lavview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.uCtrl = ((Babelfisk.WPF.Views.Input.LAVView)(target));
            return;
            case 2:
            this.dpGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.dataGrid = ((Anchor.Core.Controls.AncDataGrid)(target));
            
            #line 234 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            this.dataGrid.RowEditEnding += new System.EventHandler<System.Windows.Controls.DataGridRowEditEndingEventArgs>(this.dataGrid_RowEditEnding);
            
            #line default
            #line hidden
            
            #line 235 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            this.dataGrid.Loaded += new System.Windows.RoutedEventHandler(this.dataGrid_Loaded);
            
            #line default
            #line hidden
            
            #line 235 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            this.dataGrid.DataContextChanged += new System.Windows.DependencyPropertyChangedEventHandler(this.dataGrid_DataContextChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            System.Windows.EventSetter eventSetter;
            switch (connectionId)
            {
            case 4:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.PreviewMouseDownEvent;
            
            #line 273 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            eventSetter.Handler = new System.Windows.Input.MouseButtonEventHandler(this.Cell_PreviewMouseDown);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 5:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.PreviewKeyDownEvent;
            
            #line 298 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            eventSetter.Handler = new System.Windows.Input.KeyEventHandler(this.PreviewKeyDown_Handler);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.PreviewKeyUpEvent;
            
            #line 299 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            eventSetter.Handler = new System.Windows.Input.KeyEventHandler(this.PreviewKeyUp_Handler);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 6:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.KeyDownEvent;
            
            #line 327 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            eventSetter.Handler = new System.Windows.Input.KeyEventHandler(this.KeyDownHandler);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 7:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.PreviewKeyDownEvent;
            
            #line 359 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            eventSetter.Handler = new System.Windows.Input.KeyEventHandler(this.PreviewKeyDown_Handler);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.PreviewKeyUpEvent;
            
            #line 360 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            eventSetter.Handler = new System.Windows.Input.KeyEventHandler(this.PreviewKeyUp_Handler);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 8:
            
            #line 405 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 9:
            
            #line 424 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 10:
            
            #line 442 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 11:
            
            #line 461 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((Anchor.Core.Controls.FilteredComboBox)(target)).Initialized += new System.EventHandler(this.FilteredComboBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 12:
            
            #line 481 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((Anchor.Core.Controls.FilteredComboBox)(target)).Initialized += new System.EventHandler(this.FilteredComboBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 13:
            
            #line 502 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 14:
            
            #line 524 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 15:
            
            #line 544 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 16:
            
            #line 564 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 17:
            
            #line 584 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 18:
            
            #line 604 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 19:
            
            #line 624 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 20:
            
            #line 644 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 21:
            
            #line 664 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 22:
            
            #line 684 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 23:
            
            #line 704 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 24:
            
            #line 724 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 25:
            
            #line 744 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 26:
            
            #line 764 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 27:
            
            #line 784 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 28:
            
            #line 804 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((System.Windows.Controls.TextBox)(target)).Initialized += new System.EventHandler(this.TextBox_Initialized);
            
            #line default
            #line hidden
            break;
            case 29:
            
            #line 824 "..\..\..\..\..\Views\Input\SubSampleViews\LAVView.xaml"
            ((Anchor.Core.Controls.DropDownTextBox)(target)).Initialized += new System.EventHandler(this.DropDownTextBox_Initialized);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

