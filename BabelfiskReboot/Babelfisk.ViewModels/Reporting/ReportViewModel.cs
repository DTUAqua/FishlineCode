using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.ViewModels.Reporting.ReportExecuteModels;

namespace Babelfisk.ViewModels.Reporting
{
    public class ReportViewModel : AViewModel
    {
        private Entities.Sprattus.Report _entReport;

        private ReportExecuteViewModel _vmExecute;

        public static string UserNameTag = "<username>";


        #region Properties


        public Entities.Sprattus.Report ReportEntity
        {
            get { return _entReport; }
        }


        public ReportExecuteViewModel ReportExecuteVM
        {
            get { return _vmExecute; }
            set
            {
                _vmExecute = value;
                RaisePropertyChanged(() => ReportExecuteVM);
            }
        }


        #endregion


        public ReportViewModel(Entities.Sprattus.Report report)
        {
            _entReport = report;

            Initialize();
        }


        private void Initialize()
        {
            switch (_entReport.type)
            {
                case "RScript":
                    ReportExecuteVM = new RScriptReportExecuteViewModel(this);
                    break;

                case "Document":
                    ReportExecuteVM = new DocumentReportExecuteViewModel(this);
                    break;
            }
        }


    }
}
