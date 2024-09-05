using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.Service
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IReporting
    {
        [OperationContract]
        byte[] GetReportTreeNodes();

        [OperationContract]
        DatabaseOperationResult SaveReportingTreeNode(ref ReportingTreeNode node);

        [OperationContract]
        DatabaseOperationResult SaveReport(ref Report node);

        [OperationContract]
        ServiceResult ExecuteSQLStatement(string strQuery);

        [OperationContract]
        ServiceResult ExecuteRScript(string script, string reportResultType, int intReportId, List<ReportFile> lstSupportFiles, ref List<ReportFile> lstAdditionalOutput);

        [OperationContract]
        string GetReportDetails(int intReportId);

        [OperationContract]
        List<ReportFile> GetReportFiles(int intReportId);
    }
}
