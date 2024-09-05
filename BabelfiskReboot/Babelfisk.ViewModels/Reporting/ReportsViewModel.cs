using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.Reporting
{
    public class ReportsViewModel : AViewModel
    {
        private ReportsTreeViewModel _vmReportsTree;

        private ReportViewModel _vmSelectedReport;


        #region Properties


        public ReportsTreeViewModel ReportsTree
        {
            get { return _vmReportsTree; }
            set
            {
                _vmReportsTree = value;
                RaisePropertyChanged(() => ReportsTree);
            }
        }


     


        #endregion


        public ReportsViewModel()
        {
            WindowTitle = "Rapporter";
            WindowWidth = 850;
            WindowHeight = 500;

            _vmReportsTree = new ReportsTreeViewModel();
        }


        public void Initialize()
        {
            var tInitializeTree = _vmReportsTree.InitializeAsync();

        }


        public override void FireClosed(object sender, EventArgs e)
        {
            base.FireClosed(sender, e);

            _vmReportsTree.SelectedItem = null;
        }
    }
}
