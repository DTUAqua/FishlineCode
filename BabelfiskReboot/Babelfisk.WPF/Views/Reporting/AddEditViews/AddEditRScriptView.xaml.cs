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
using Babelfisk.ViewModels;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Search;
using Anchor.Core;

namespace Babelfisk.WPF.Views.Reporting.AddEditViews
{
    /// <summary>
    /// Interaction logic for AddEditRScriptView.xaml
    /// </summary>
    public partial class AddEditRScriptView : UserControl
    {
        FoldingManager _fm;


        public AViewModel ViewModel
        {
            get { return this.DataContext as AViewModel; }
        }

        public AddEditRScriptView()
        {
            InitializeComponent();

            this.Loaded += AddEditRScriptView_Loaded;
            this.DataContextChanged += View_DataContextChanged;
            this.Unloaded += View_Unloaded;

            SetupAvalonEdit();
        }


        /// <summary>
        /// Deregister OnScrollTo when species list is unloaded.
        /// </summary>
        protected void View_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.OnScrollTo -= ViewModel_OnScrollTo;
        }

        protected void View_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is AViewModel)
                (e.OldValue as AViewModel).OnScrollTo -= ViewModel_OnScrollTo;

            (e.NewValue as AViewModel).OnScrollTo -= ViewModel_OnScrollTo;
            (e.NewValue as AViewModel).OnScrollTo += ViewModel_OnScrollTo;
        }



        protected void ViewModel_OnScrollTo(ViewModels.AViewModel obj, string strTo)
        {
            switch (strTo)
            {
                case "ResultStart":
                    tbResult.ScrollToHome();

                    break;

                case "ResultEnd":
                    tbResult.ScrollToEnd();
                    break;
            }
        }

        void AddEditRScriptView_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(avQuery);
            avQuery.Focus();
        }



        private void SetupAvalonEdit()
        {
            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Babelfisk.WPF.Resources.TextDefinitions.r.xshd"))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    avQuery.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                }
            }

            avQuery.TextChanged += avQuery_TextChanged;
        }

        protected void avQuery_TextChanged(object sender, EventArgs e)
        {
            ApplyFolding();
        }


        private void ApplyFolding()
        {
            try
            {
                if (avQuery.Document == null || avQuery.Document.Text == null)
                    return;

                if (_fm == null)
                    _fm = FoldingManager.Install(avQuery.TextArea);

                List<NewFolding> lst = new List<NewFolding>();

                if (avQuery.Document != null && avQuery.Document.Text != null)
                {
                    bool blnFound = false;
                    int intStart = 0;
                    foreach (var v in AllIndexesOf(avQuery.Document.Text, "```"))
                    {
                        if (blnFound == false)
                        {
                            intStart = v;
                            blnFound = true;
                        }
                        else
                        {
                            blnFound = false;
                            lst.Add(new NewFolding(intStart, v));
                            intStart = 0;
                        }
                    }

                }

                _fm.UpdateFoldings(lst, -1);
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        public IEnumerable<int> AllIndexesOf( string str, string searchstring)
        {
            int minIndex = str.IndexOf(searchstring);
            while (minIndex != -1)
            {
                int intNew = -1;
                if ((intNew = str.IndexOf('\n', minIndex)) != -1)
                    minIndex = intNew-1;

                yield return minIndex;

                if (minIndex + searchstring.Length > str.Length)
                    minIndex = -1;
                else
                {
                    try
                    {
                        minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
                    }
                    catch 
                    {
                        minIndex = -1;
                    }
                }
            }
        }

        private void avQuery_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.F))
            {
                avQuery.TextArea.DefaultInputHandler.NestedInputHandlers.Add(new ICSharpCode.AvalonEdit.Search.SearchInputHandler(avQuery.TextArea));
            }
        }


        private void txtClipboard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    var tb = (sender as TextBlock);

                    if (tb != null && tb.Text != null)
                    {
                        string strText = tb.Text;
                        int intIndex = 0;
                        if ((intIndex = strText.IndexOf('<')) > 0)
                            strText = strText.Substring(0, intIndex);

                        strText.CopyToClipboard();
                    }
                }
            }
            catch { }
        }
    }
}
