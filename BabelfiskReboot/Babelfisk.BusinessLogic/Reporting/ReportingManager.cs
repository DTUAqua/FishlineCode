using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.BusinessLogic.BabelfiskService;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Babelfisk.Entities;

namespace Babelfisk.BusinessLogic.Reporting
{
    public class ReportingManager
    {

        public List<IObjectWithChangeTracker> GetReportTreeNodes()
        {
            var sv = DataClientFactory.CreateReportingClient();

            List<IObjectWithChangeTracker> lst = new List<IObjectWithChangeTracker>();
            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IReporting).GetReportTreeNodes();

                sv.Close();

                if (arr != null)
                {
                    arr = arr.Decompress();
                    lst = arr.ToObjectNetDataContract<List<IObjectWithChangeTracker>>();
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }

            return lst;
        }


        public string GetReportDetails(int intReportId)
        {
            var sv = DataClientFactory.CreateReportingClient();

            string str = null;

            try
            {
                sv.SupplyCredentials();

                str = (sv as IReporting).GetReportDetails(intReportId);

                sv.Close();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }

            return str;
        }


        public List<ReportFile> GetReportFiles(int intReportId)
        {
            var sv = DataClientFactory.CreateReportingClient();

            List<ReportFile> lst = null;

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IReporting).GetReportFiles(intReportId);

                if (arr != null)
                    lst = arr.ToList();

                sv.Close();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }

            return lst;
        }



        /// <summary>
        /// Save a ReportingTreeNode entity to database (whether it is new, edited or deleted).
        /// </summary>
        public DatabaseOperationResult SaveReportingTreeNode(ref ReportingTreeNode node)
        {
            var sv = DataClientFactory.CreateReportingClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IReporting).SaveReportingTreeNode(ref node);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }



        /// <summary>
        /// Save a Report entity to database (whether it is new, edited or deleted).
        /// </summary>
        public DatabaseOperationResult SaveReport(ref Report node)
        {
            var sv = DataClientFactory.CreateReportingClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IReporting).SaveReport(ref node);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }



        public List<ParameterListItem> ExecuteSQLStatement(string strQuery)
        {
            var sv = DataClientFactory.CreateReportingClient();

            List<ParameterListItem> lst = null;
            try
            {
                sv.SupplyCredentials();

                var res = (sv as IReporting).ExecuteSQLStatement(strQuery);

                sv.Close();

                if (res.Result == DatabaseOperationStatus.Successful && (res.Data as byte[]) != null)
                {
                    byte[] arr = (byte[])res.Data;

                    arr = arr.Decompress();
                    lst = arr.ToObjectNetDataContract<List<ParameterListItem>>();
                }
                else
                {
                    //Show fail message in UI.
                    if(res.Result == DatabaseOperationStatus.ValidationError)
                        throw new ApplicationException("SQL scriptet må ikke indeholde DELETE eller UPDATE udtryk.");
                    else
                        throw new ApplicationException(res.Message);
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }

            return lst;
        }



        public ServiceResult ExecuteRScript(string strScript, int intReportId, List<ReportFile> lstSupportFiles, out ReportFile[] arrAdditionalFiles, ReportResultType rep = ReportResultType.PDF)
        {
            arrAdditionalFiles = null;

            var sv = DataClientFactory.CreateReportingClient();

            try
            {
                sv.SupplyCredentials();
               
                var res = (sv as IReporting).ExecuteRScript(strScript, rep.ToString(), intReportId, lstSupportFiles == null ? new ReportFile[]{} : lstSupportFiles.ToArray(), ref arrAdditionalFiles);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }
        }

    }
}
