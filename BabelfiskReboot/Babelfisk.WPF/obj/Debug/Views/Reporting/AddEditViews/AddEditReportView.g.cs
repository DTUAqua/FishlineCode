﻿#pragma checksum "..\..\..\..\..\Views\Reporting\AddEditViews\AddEditReportView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "521680A0BA38AFC99A86FFDBEEF91D51B65AB20D61B3FFB3E7D5361DC468FF9D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Babelfisk.WPF.Views.Reporting.AddEditViews;
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


namespace Babelfisk.WPF.Views.Reporting.AddEditViews {
    
    
    /// <summary>
    /// AddEditReportView
    /// </summary>
    public partial class AddEditReportView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 51 "..\..\..\..\..\Views\Reporting\AddEditViews\AddEditReportView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbName;
        
        #line default
        #line hidden
        
        
        #line 193 "..\..\..\..\..\Views\Reporting\AddEditViews\AddEditReportView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer itemScrollViewer;
        
        #line default
        #line hidden
        
        
        #line 280 "..\..\..\..\..\Views\Reporting\AddEditViews\AddEditReportView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnBrowse;
        
        #line default
        #line hidden
        
        
        #line 299 "..\..\..\..\..\Views\Reporting\AddEditViews\AddEditReportView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRestriction;
        
        #line default
        #line hidden
        
        
        #line 356 "..\..\..\..\..\Views\Reporting\AddEditViews\AddEditReportView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel dpErrorMessage;
        
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
            System.Uri resourceLocater = new System.Uri("/Babelfisk.WPF;component/views/reporting/addeditviews/addeditreportview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\Reporting\AddEditViews\AddEditReportView.xaml"
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
            this.tbName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.itemScrollViewer = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 3:
            this.btnBrowse = ((System.Windows.Controls.Button)(target));
            return;
            case 4:
            this.btnRestriction = ((System.Windows.Controls.Button)(target));
            
            #line 299 "..\..\..\..\..\Views\Reporting\AddEditViews\AddEditReportView.xaml"
            this.btnRestriction.Click += new System.Windows.RoutedEventHandler(this.btnRestriction_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.dpErrorMessage = ((System.Windows.Controls.DockPanel)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

