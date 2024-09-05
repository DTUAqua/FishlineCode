using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Babelfisk.Entities;
using System.Transactions;
using System.Data;
using Anchor.Core;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Asprosys.Security.AccessControl;

namespace Babelfisk.Service
{
    public partial class BabelfiskService : IReporting
    {
        public static string GetReportDetailsDirectory
        {
            get
            {
                string strDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReportDetails");

                try
                {
                    if (!Directory.Exists(strDirectory))
                        Directory.CreateDirectory(strDirectory);
                }
                catch(Exception e)
                {
                    Anchor.Core.Loggers.Logger.LogError(e);
                }

                return strDirectory;
            }
        }


        public static string GetReportFilesDirectory
        {
            get
            {
                string strDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReportFiles");

                try
                {
                    if (!Directory.Exists(strDirectory))
                        Directory.CreateDirectory(strDirectory);
                }
                catch (Exception e)
                {
                    Anchor.Core.Loggers.Logger.LogError(e);
                }

                return strDirectory;
            }
        }


        public byte[] GetReportTreeNodes()
        {
            byte[] arr = null;

            List<IObjectWithChangeTracker> lst = new List<IObjectWithChangeTracker>();

            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                //Select all nodes out to make sure all of the navigation properties are populated.
                var nodes = ctx.ReportingTreeNode
                               .Include("ParentTreeNode")
                               .Include("ChildTreeNodes")
                               .Include("Reports")
                               .ToList();

                var reports = ctx.Report
                                 .Where(x => x.ReportingTreeNodes.Count == 0)
                                 .ToList();

                //Only return root nodes.
                nodes = nodes.Where(x => x.ParentTreeNode == null).ToList();

                var comparer = new Anchor.Core.Comparers.StringNumberComparer();
                
                //Sort root nodes and root reports
                nodes.Sort(x => x.name, comparer);
                reports.Sort(x => x.name, comparer);

                //Sort rest of the hierarchy
                Sort(nodes, comparer);

                //Add result
                lst.AddRange(nodes);
                lst.AddRange(reports);

                arr = lst.ToByteArrayNetDataContract();
                arr = arr.Compress();
            }

            return arr;
        }


        private void Sort(IEnumerable<ReportingTreeNode> rtnList, IComparer<string> comparer)
        {
            foreach (var rtn in rtnList)
            {
                rtn.StopTracking();
                {
                    rtn.ChildTreeNodes.Sort(x => x.name, comparer);
                    rtn.Reports.Sort(x => x.name, comparer);

                    Sort(rtn.ChildTreeNodes, comparer);
                }
                rtn.StartTracking();
            }
        }


        private void DeleteAllChildren(SprattusContainer ctx, ReportingTreeNode tn)
        {
            var lst = tn.ChildTreeNodes.ToList();
            foreach (var t in lst)
                DeleteAllChildren(ctx, t);

            ctx.DeleteObject(tn);
        }


        public DatabaseOperationResult SaveReportingTreeNode(ref ReportingTreeNode node)
        {
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    using (TransactionScope scope = new TransactionScope())
                    {
                        ReportingTreeNode repNewEdit = null;
                        int intNodeId = node.reportingTreeNodeId;

                        //If species list is deleted, delete it and all its children.
                        if (node.ChangeTracker.State == ObjectState.Deleted)
                        {
                            //First receive all reporting nodes (so all ChildTreeNodes and their children etc, on the node selected out below, will be populated automatically - that's how EF works)
                            var all = ctx.ReportingTreeNode.ToList();
                            var enode = ctx.ReportingTreeNode.Where(x => x.reportingTreeNodeId == intNodeId).FirstOrDefault();
                            DeleteAllChildren(ctx, enode);
                        }
                        else //Else apply changes to it
                        {
                            if (node.ChangeTracker.State == ObjectState.Added)
                                repNewEdit = new ReportingTreeNode();
                            else
                            {
                                repNewEdit = ctx.ReportingTreeNode.Where(x => x.reportingTreeNodeId == intNodeId).FirstOrDefault();
                                if (repNewEdit == null)
                                    repNewEdit = new ReportingTreeNode();
                            }

                            node.CopyEntityValueTypesTo(repNewEdit);
                            ctx.ReportingTreeNode.ApplyChanges(repNewEdit);
                        }

                        ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                        scope.Complete();

                        node = repNewEdit;
                    }
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveReportingTreeNode with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveReportingTreeNode with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveReportingTreeNode with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }

        private string GetReportFilesPath(int intReportId)
        {
            string strDir = GetReportFilesDirectory;

            return Path.Combine(strDir, string.Format("Report{0}", intReportId));
        }

        private bool DeleteReportFiles(int intReportId)
        {
            bool blnResult = false;

            var strDir = GetReportFilesPath(intReportId);

             try
            {
                if (Directory.Exists(strDir))
                    Directory.Delete(strDir, true);

                blnResult = true;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return blnResult;
        }


        private string GetReportPath(int intReportId)
        {
            string strDir = GetReportDetailsDirectory;

            return Path.Combine(strDir, string.Format("Report{0}.xml", intReportId));
        }


        /// <summary>
        /// Delete details for a report.
        /// </summary>
        private bool DeleteReportDetails(int intReportId)
        {
            bool blnResult = false;

            string strPath = GetReportPath(intReportId);

            try
            {
                if (File.Exists(strPath))
                    File.Delete(strPath);

                blnResult = true;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return blnResult;
        }


        /// <summary>
        /// Retrieve all files for report with id intReportId
        /// </summary>
        public List<ReportFile> GetReportFiles(int intReportId)
        {
            List<ReportFile> lst = new List<ReportFile>();

            string strDir = GetReportFilesPath(intReportId);

            try
            {
                if (Directory.Exists(strDir))
                {
                    foreach (var strPath in Directory.GetFiles(strDir))
                    {
                        byte[] arr = null;
                        using (FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            arr = new byte[(int)fs.Length];
                            fs.Read(arr, 0, arr.Length);
                        }

                        if (arr != null)
                        {
                            var rf = new ReportFile() { FileName = Path.GetFileName(strPath), DecompressedData = arr };
                            rf.CompressData(true);
                            lst.Add(rf);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return lst;
        }


        /// <summary>
        /// Retrieve report details as an xml formatted string.
        /// </summary>
        public string GetReportDetails(int intReportId)
        {
            string strDetails = null;

            string strPath = GetReportPath(intReportId);

            try
            {
                if (File.Exists(strPath))
                {
                    using (FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                        {
                            strDetails = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
                

            return strDetails;
        }



        /// <summary>
        /// Save report details from an xml formatted string.
        /// </summary>
        private string SaveReportDetails(int intReportId, string strContent)
        {
            string strDetails = null;

            string strPath = GetReportPath(intReportId);

            try
            {
                using (FileStream fs = new FileStream(strPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(strContent);
                    }
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return strDetails;
        }


        /// <summary>
        /// Save report files from a list of reportfile objects.
        /// </summary>
        private void SaveReportFiles(int intReportId, List<ReportFile> lstFiles)
        {
            string strDir = GetReportFilesPath(intReportId);

            try
            {
                DeleteReportFiles(intReportId);

                if (!Directory.Exists(strDir))
                    Directory.CreateDirectory(strDir);

                foreach (var file in lstFiles)
                {
                    if (file.FileName == null)
                        continue;

                    if (file.DecompressedData == null)
                        file.DecompressData();

                    if (file.DecompressedData == null || file.DecompressedData.Length == 0)
                        continue;

                    string strPath = Path.Combine(strDir, file.FileName);
                    using (FileStream fs = new FileStream(strPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        fs.Write(file.DecompressedData, 0, file.DecompressedData.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        public DatabaseOperationResult SaveReport(ref Report node)
        {
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    using (TransactionScope scope = new TransactionScope())
                    {
                        Report repNewEdit = null;
                        int intNodeId = node.reportId;
                        bool blnIsDeleted = node.ChangeTracker.State == ObjectState.Deleted;
                        string strReportDetails = node.Details;
                        List<ReportFile> lstFiles = node.Files;

                        //If species list is deleted, delete it and all its children.
                        if (blnIsDeleted)
                        {
                            //First receive all reporting nodes (so all ChildTreeNodes and their children etc, on the node selected out below, will be populated automatically - that's how EF works)
                            var enode = ctx.Report.Include("ReportingTreeNodes").Where(x => x.reportId == intNodeId).FirstOrDefault();

                            ctx.DeleteObject(enode);

                            DeleteReportDetails(intNodeId);
                            DeleteReportFiles(intNodeId);
                        }
                        else //Else apply changes to it
                        {
                            if (node.ChangeTracker.State == ObjectState.Added)
                                repNewEdit = new Report();
                            else
                            {
                                repNewEdit = ctx.Report.Include("ReportingTreeNodes").Where(x => x.reportId == intNodeId).FirstOrDefault();
                                if (repNewEdit == null)
                                    repNewEdit = new Report();

                                if(repNewEdit.ReportingTreeNodes.Count > 0)
                                {
                                    repNewEdit.ReportingTreeNodes.Clear();
                                    ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                                }
                            }

                            node.CopyEntityValueTypesTo(repNewEdit);

                            foreach (var r in node.ReportingTreeNodes)
                            {
                                var rn = ctx.ReportingTreeNode.Where(x => x.reportingTreeNodeId == r.reportingTreeNodeId).FirstOrDefault();
                                if (rn != null)
                                    repNewEdit.ReportingTreeNodes.Add(rn);
                            }

                            ctx.Report.ApplyChanges(repNewEdit);
                        }

                        ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                        scope.Complete();

                        node = repNewEdit;

                        if(node != null)
                            intNodeId = node.reportId;

                        if (!blnIsDeleted && strReportDetails != null)
                            SaveReportDetails(intNodeId, strReportDetails);

                        if (!blnIsDeleted && lstFiles != null)
                        {
                            SaveReportFiles(intNodeId, lstFiles);
                        }
                    }
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveReport with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveReport with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveReport with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }



        public ServiceResult ExecuteSQLStatement(string strQuery)
        {
            byte[] arr = null;

            if (strQuery == null || strQuery.Contains("DELETE", StringComparison.InvariantCultureIgnoreCase) || strQuery.Contains("UPDATE", StringComparison.InvariantCultureIgnoreCase))
                return new ServiceResult(DatabaseOperationStatus.ValidationError, "Query must be defined and not contain DELETE or UPDATE statements.");

            SqlDataReader rdr = null;
            try
            {
                List<ParameterListItem> lst = new List<ParameterListItem>();

                using (var ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    var cmd = new SqlCommand(strQuery);
                    cmd.Connection = (ctx.Connection as System.Data.EntityClient.EntityConnection).StoreConnection as SqlConnection;

                    rdr = cmd.ExecuteReader();

                    
                    if (rdr.FieldCount > 1)
                    {
                        while (rdr.Read())
                        {
                            object obj1 = rdr.GetValue(0);
                            object obj2 = rdr.GetValue(1);
                            ParameterListItem p = new ParameterListItem(obj1 == null ? null : obj1.ToString(), obj2 == null ? null : obj2.ToString());

                            lst.Add(p);
                        }
                    }

                        //Select all nodes out to make sure all of the navigation properties are populated.
                  //  var res = ctx.ExecuteStoreQuery<QueryListItem>(strQuery);
                    //lst = res.ToList();

                    arr = lst.ToByteArrayNetDataContract();
                    arr = arr.Compress();
                }
            }
            catch (Exception e)
            {
                return new ServiceResult(DatabaseOperationStatus.UnexpectedException, e.InnerException != null ? e.InnerException.Message : e.Message);
            }
            finally
            {
                if (rdr != null)
                    rdr.Close();
            }

            return new ServiceResult(DatabaseOperationStatus.Successful, null, arr);

        }



        public ServiceResult ExecuteRScript(string script, string reportResultType, int intReportId, List<ReportFile> lstSupportFiles, ref List<ReportFile> lstAdditionalOutput)
        {
            //return new ServiceResult(DatabaseOperationStatus.UnexpectedException, "Log dir: " + Anchor.Core.Loggers.Logger.ApplicationDataPath);
            var rep = ReportResultType.PDF;

            Enum.TryParse<ReportResultType>(reportResultType, true, out rep);

            string strRScriptServerPath = System.Configuration.ConfigurationManager.AppSettings["RScriptPath"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["RScriptPath"].ToString();  //@"C:\Program Files\R\R-3.2.0\bin\Rscript.exe";
          
            if(!File.Exists(strRScriptServerPath))
                return new ServiceResult(DatabaseOperationStatus.UnexpectedException, string.Format("Could not locate RScript.exe on server, searched: '{0}'.", strRScriptServerPath));

            string strPandocPath = System.Configuration.ConfigurationManager.AppSettings["PandocPath"] == null ? "Sys.getenv('RSTUDIO_PANDOC')" : System.Configuration.ConfigurationManager.AppSettings["PandocPath"].ToString(); //@"C:/Program Files/RStudio/bin/pandoc";

            string strMikTexPath = System.Configuration.ConfigurationManager.AppSettings["PDFLatex"] == null ? "" : "Sys.setenv(PATH=paste(Sys.getenv('PATH')," + System.Configuration.ConfigurationManager.AppSettings["PDFLatex"].ToString() + ",sep=';')); ";

            string strSessionDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ("Sessions"), DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")) + "/"; // + @"C:/Mads/Test/" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + "/";

            strSessionDirectory = strSessionDirectory.Replace(@"\", "/");

            byte[] arrFile = null;

            try
            {
                Directory.CreateDirectory(strSessionDirectory);
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                return new ServiceResult(DatabaseOperationStatus.UnexpectedException, string.Format("Could not create session directory: '{0}'. {1}", strSessionDirectory, e.InnerException != null ? e.InnerException.Message : e.Message));
            }

            try
            {
                string strFishLineConString = System.Configuration.ConfigurationManager.AppSettings["CustomFishLineConnectionString"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["CustomFishLineConnectionString"].ToString();
                string strFishLineDWConString = System.Configuration.ConfigurationManager.AppSettings["CustomFishLineDWConnectionString"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["CustomFishLineDWConnectionString"].ToString();
                string strFishLineSecurityConString = System.Configuration.ConfigurationManager.AppSettings["CustomFishLineSecurityConnectionString"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["CustomFishLineSecurityConnectionString"].ToString();
                string strDFADConString = System.Configuration.ConfigurationManager.AppSettings["CustomDFADConnectionString"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["CustomDFADConnectionString"].ToString();
                string strDanaDBConString = System.Configuration.ConfigurationManager.AppSettings["CustomDanaDBConnectionString"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["CustomDanaDBConnectionString"].ToString();
                string strDanaDBDWConString = System.Configuration.ConfigurationManager.AppSettings["CustomDanaDBDWConnectionString"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["CustomDanaDBDWConnectionString"].ToString();


                if (!string.IsNullOrWhiteSpace(strFishLineConString))
                {
                    script = ReplaceCaseInsensitive(script, "odbcDriverConnect(\"FishLine\")", string.Format("odbcDriverConnect(\"{0}\")", strFishLineConString));
                    script = ReplaceCaseInsensitive(script, "odbcConnect(\"FishLine\")", string.Format("odbcDriverConnect(\"{0}\")", strFishLineConString));
                }

                if (!string.IsNullOrWhiteSpace(strFishLineDWConString))
                {
                    script = ReplaceCaseInsensitive(script, "odbcDriverConnect(\"FishLineDW\")", string.Format("odbcDriverConnect(\"{0}\")", strFishLineDWConString));
                    script = ReplaceCaseInsensitive(script, "odbcConnect(\"FishLineDW\")", string.Format("odbcDriverConnect(\"{0}\")", strFishLineDWConString));
                }

                if (!string.IsNullOrWhiteSpace(strFishLineSecurityConString))
                {
                    script = ReplaceCaseInsensitive(script, "odbcDriverConnect(\"FishLineSecurity\")", string.Format("odbcDriverConnect(\"{0}\")", strFishLineSecurityConString));
                    script = ReplaceCaseInsensitive(script, "odbcConnect(\"FishLineSecurity\")", string.Format("odbcDriverConnect(\"{0}\")", strFishLineSecurityConString));
                }

                if (!string.IsNullOrWhiteSpace(strDFADConString))
                {
                    script = ReplaceCaseInsensitive(script, "odbcDriverConnect(\"DFAD\")", string.Format("odbcDriverConnect(\"{0}\")", strDFADConString));
                    script = ReplaceCaseInsensitive(script, "odbcConnect(\"DFAD\")", string.Format("odbcDriverConnect(\"{0}\")", strDFADConString));
                }

                if (!string.IsNullOrWhiteSpace(strDanaDBConString))
                {
                    script = ReplaceCaseInsensitive(script, "odbcDriverConnect(\"DanaDB\")", string.Format("odbcDriverConnect(\"{0}\")", strDanaDBConString));
                    script = ReplaceCaseInsensitive(script, "odbcConnect(\"DanaDB\")", string.Format("odbcDriverConnect(\"{0}\")", strDanaDBConString));
                }

                if (!string.IsNullOrWhiteSpace(strDanaDBDWConString))
                {
                    script = ReplaceCaseInsensitive(script, "odbcDriverConnect(\"DanaDBDW\")", string.Format("odbcDriverConnect(\"{0}\")", strDanaDBDWConString));
                    script = ReplaceCaseInsensitive(script, "odbcConnect(\"DanaDBDW\")", string.Format("odbcDriverConnect(\"{0}\")", strDanaDBDWConString));
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e, "Could not replace connection strings.");
            }


            try
            {
                string dfadPathKey = System.Configuration.ConfigurationManager.AppSettings["DFADReplaceKey"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["DFADReplaceKey"].ToString();

                string dfadFolderPath = System.Configuration.ConfigurationManager.AppSettings["DFADFolderPath"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["DFADFolderPath"].ToString();

                if (!string.IsNullOrWhiteSpace(dfadPathKey) && !string.IsNullOrWhiteSpace(dfadFolderPath))
                {
                    script = ReplaceCaseInsensitive(script, dfadPathKey, dfadFolderPath);
                }

            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e, "Could not replace DFAD path strings.");
            }

            List<string> lstReportFiles = new List<string>();
            //Copy help files to session directory
            try
            {
                var repFilesPath = GetReportFilesPath(intReportId);

                string[] arrHelpFiles = null;
                if (repFilesPath != null && Directory.Exists(repFilesPath) && (arrHelpFiles = Directory.GetFiles(repFilesPath)) != null && arrHelpFiles.Length > 0)
                {
                    foreach (var f in arrHelpFiles)
                    {
                        string strFileNameWithExt = Path.GetFileName(f);
                        var newLocation = Path.Combine(strSessionDirectory, strFileNameWithExt);
                        lstReportFiles.Add(strFileNameWithExt);
                        try
                        {
                            if (!File.Exists(newLocation))
                                f.CopyFileToLocation(Path.Combine(strSessionDirectory, strFileNameWithExt));
                        }
                        catch (Exception e)
                        {
                            Anchor.Core.Loggers.Logger.LogError(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            //Copy support files to session directory
            try
            {
                if (lstSupportFiles != null && lstSupportFiles.Count > 0)
                {
                    foreach (var f in lstSupportFiles)
                    {
                        string strFileNameWithExt = f.FileName;
                        var newLocation = Path.Combine(strSessionDirectory, strFileNameWithExt);
                        lstReportFiles.Add(strFileNameWithExt);
                        try
                        {
                            if (!File.Exists(newLocation))
                            {
                                f.DecompressData(true);
                                if(f.DecompressedData != null && f.DecompressedData.Length > 0)
                                    f.DecompressedData.SaveByteToFile(newLocation, true);
                            }
                        }
                        catch (Exception e)
                        {
                            Anchor.Core.Loggers.Logger.LogError(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            string strScriptPath = strSessionDirectory + @"rapport.Rmd";

            try
            {
                File.WriteAllText(strScriptPath, script);
            }
            catch (Exception e)
            {
                try
                {
                    Directory.Delete(strSessionDirectory, true);
                }
                catch { }

                Anchor.Core.Loggers.Logger.LogError(e);
                return new ServiceResult(DatabaseOperationStatus.UnexpectedException, string.Format("Could not write script file: '{0}'. {1}", strScriptPath, e.InnerException != null ? e.InnerException.Message : e.Message));
            }

  
            string strReportName = @"rapport.pdf";
            string strOutputFile = strSessionDirectory + strReportName;
            string strEncoding = "UTF-8";

            string strCommand = string.Format("{0}", strRScriptServerPath);
            string strParameters = string.Format("-e \"Sys.setenv(RSTUDIO_PANDOC={0}); {1} rmarkdown::render('{2}', encoding = '{3}', {4}output_file = '{5}')\"", strPandocPath, strMikTexPath, strScriptPath, strEncoding, rep == ReportResultType.PDF ? "" : "'word_document', ", strOutputFile);

            string strResult = null;
            bool blnSuccess = false;

            string strUserName = null, strPassword = null, strDomain = null;
            try
            {
                strUserName = System.Configuration.ConfigurationManager.AppSettings["ProcessUserName"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["ProcessUserName"].ToString();
                strPassword = System.Configuration.ConfigurationManager.AppSettings["ProcessPassword"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["ProcessPassword"].ToString();
                strDomain = System.Configuration.ConfigurationManager.AppSettings["ProcessDomain"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["ProcessDomain"].ToString();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            try
            {
                blnSuccess = RunFromCmd(strCommand, strParameters, strUserName, strPassword, strDomain, out strResult);

                if(blnSuccess)
                {
                    if (File.Exists(strOutputFile))
                    {
                        arrFile = File.ReadAllBytes(strOutputFile);

                        //Compress report
                        if(arrFile.Length > 0)
                            arrFile = arrFile.Compress();

                        lstAdditionalOutput = AppendAdditionalFiles(strSessionDirectory, strReportName, lstReportFiles);
                    }
                }
            }
            catch(Exception e)
            {
                return new ServiceResult(DatabaseOperationStatus.UnexpectedException, e.InnerException != null ? e.InnerException.Message : e.Message, strResult, null);
            }
            finally
            {

                try
                {
                    Directory.Delete(strSessionDirectory, true);
                }
                catch { }
            }

            return new ServiceResult(blnSuccess ? DatabaseOperationStatus.Successful : DatabaseOperationStatus.ValidationError, strResult, arrFile);
        }

        private static string ReplaceCaseInsensitive(string input, string search, string replacement)
        {
            string result = Regex.Replace(
                input,
                Regex.Escape(search),
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );
            return result;
        }


        private List<ReportFile> AppendAdditionalFiles(string strSessionDirectory, string strReportName, List<string> lstReportFiles)
        {
            var lstAdditionalOutput = new List<ReportFile>();

            string fileExtensions = System.Configuration.ConfigurationManager.AppSettings["AdditionalReportFileExtensions"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["AdditionalReportFileExtensions"].ToString();

            if (fileExtensions.Length > 0)
            {
                string strFilesToSkip = string.Join(",", lstReportFiles);

                //Find all additional files in session directory to transfer back.
                var files = Directory.GetFiles(strSessionDirectory);
                foreach (var file in files)
                {
                    string strFileName = Path.GetFileName(file);

                    //If file is the rapport pdf file, skip it
                    if (strFileName.Equals(strReportName, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    //Skip any report help files.
                    if (strFilesToSkip.Contains(strFileName, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    string ext = Path.GetExtension(strFileName);

                    //If file is not of the type to be returned, skip it
                    if (string.IsNullOrWhiteSpace(ext) || !fileExtensions.Contains(ext, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    byte[] arr = null;
                    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        arr = new byte[(int)fs.Length];
                        fs.Read(arr, 0, arr.Length);
                    }

                    if (arr != null)
                    {
                        var rf = new ReportFile() { FileName = strFileName, DecompressedData = arr };
                        rf.CompressData(true);
                        lstAdditionalOutput.Add(rf);
                    }
                }
            }

            return lstAdditionalOutput;

        }


        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
        private static bool RunFromCmd(string rScriptExecutablePath, string args, string strUserName, string strPassword, string strDomain, out string result)
        {
            result = string.Empty;
            bool blnSuccess = false;

            try
            {

                var info = new ProcessStartInfo();
                info.FileName = rScriptExecutablePath;
               // info.WorkingDirectory = Path.GetDirectoryName(rScriptExecutablePath);
                info.Arguments = args;

               // info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;
                info.ErrorDialog = false;
                info.StandardOutputEncoding = Encoding.Default;
                info.StandardErrorEncoding = Encoding.Default;

                if (!string.IsNullOrWhiteSpace(strUserName))
                    info.UserName = strUserName;

                if (!string.IsNullOrWhiteSpace(strPassword))
                    info.Password = GetSecureString(strPassword);

                if (!string.IsNullOrWhiteSpace(strDomain))
                    info.Domain = strDomain;

                info.Verb = "runas";

                if (!string.IsNullOrWhiteSpace(strUserName))
                {
                    try
                    {
                        IntPtr hWinSta = GetProcessWindowStation();
                        WindowStationSecurity ws = new WindowStationSecurity(hWinSta, System.Security.AccessControl.AccessControlSections.Access);
                        ws.AddAccessRule(new WindowStationAccessRule(strUserName, WindowStationRights.AllAccess, System.Security.AccessControl.AccessControlType.Allow));
                        ws.AcceptChanges();

                        IntPtr hDesk = GetThreadDesktop(GetCurrentThreadId());
                        DesktopSecurity ds = new DesktopSecurity(hDesk, System.Security.AccessControl.AccessControlSections.Access);
                        ds.AddAccessRule(new DesktopAccessRule(strUserName, DesktopRights.AllAccess, System.Security.AccessControl.AccessControlType.Allow));

                        ds.AcceptChanges();
                    }
                    catch (Exception ex)
                    {
                        Anchor.Core.Loggers.Logger.LogError(ex, "Setting security rights failed during r-script execution.");
                    }

                }
                
                using (var proc = new Process())
                {
                    proc.StartInfo = info;
                    proc.Start();

                    string output = "", error = "";

                    //Prevent deadlocks by reading StandardOutput and StandardError stream at the samme time in two threads, since if only one is read at a time, and the other ones buffer is full, a deadlock occurs.
                    var tOut = new Task(() =>
                    {
                        output = proc.StandardOutput.ReadToEnd();
                    });

                    var tError = new Task(() =>
                    {
                        error = proc.StandardError.ReadToEnd();
                    });

                    tOut.Start();
                    tError.Start();

                    tOut.Wait(10 * 60 * 1000);
                    tError.Wait(10 * 60 * 1000);

                    proc.WaitForExit(10 * 60 * 1000);

                    result = output;

                    if (result == null)
                        result = "";
                    //Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Info, "Script executed. " + (error != null ? ("Error: " + error) : "No error") + " | " + (result != null ? ("info: " + result) : "No info"));

                    if (error != null && !error.Contains("Output created:", StringComparison.InvariantCultureIgnoreCase))
                        result += error;
                    
                    int intExitcode = proc.ExitCode;

                    if (intExitcode == 0 && error.Contains("Output created:", StringComparison.InvariantCultureIgnoreCase))
                        blnSuccess = true;

                    proc.Close();
                }

                return blnSuccess;
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex, "Failed to execute R-Script");
                throw new Exception("R Script failed: " + result, ex);
            }
        }


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetProcessWindowStation();


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetThreadDesktop(int dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentThreadId();


        public static SecureString GetSecureString(string str)
        {
            SecureString secureString = new SecureString();
            foreach (char ch in str)
            {
                secureString.AppendChar(ch);
            }
            secureString.MakeReadOnly();
            return secureString;
        }
       
    }
}