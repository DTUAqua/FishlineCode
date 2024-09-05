using Babelfisk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Anchor.Core;
using System.Text.RegularExpressions;
using System.IO;
using Babelfisk.Entities.FileEntities;

namespace Babelfisk.OtolithFileService
{
    public class OtolithFileService : IOtolithFileService
    {
        private static bool _disconnectUNCBeforeConnect = true;


        #region Properties


        public static string OtolithImagesRootPath
        {
            get
            {
                string path = System.Configuration.ConfigurationManager.AppSettings["OtolithImagesRootPath"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["OtolithImagesRootPath"].ToString();

                return path;
            }
        }


        public static string OtolithImagesServerName
        {
            get
            {
                string path = System.Configuration.ConfigurationManager.AppSettings["OtolithImagesServerName"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["OtolithImagesServerName"].ToString();

                return path;
            }
        }


        public static string OtolithImagesServerDomainOrComputerName
        {
            get
            {
                string path = System.Configuration.ConfigurationManager.AppSettings["OtolithImagesServerDomainOrComputerName"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["OtolithImagesServerDomainOrComputerName"].ToString();

                return path;
            }
        }


        public static string OtolithImagesServerUser
        {
            get
            {
                string path = System.Configuration.ConfigurationManager.AppSettings["OtolithImagesServerUser"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["OtolithImagesServerUser"].ToString();

                return path;
            }
        }


        public static string OtolithImagesServerPassword
        {
            get
            {
                string path = System.Configuration.ConfigurationManager.AppSettings["OtolithImagesServerPassword"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["OtolithImagesServerPassword"].ToString();

                return path;
            }
        }


        public static bool OtolithImagesServerConnectToUNCPath
        {
            get
            {
                string path = System.Configuration.ConfigurationManager.AppSettings["OtolithImagesServerConnectToUNCPath"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["OtolithImagesServerConnectToUNCPath"].ToString();

                bool c = false;
                if (path == null || !path.TryParseBool(out c))
                    return false;

                return c;
            }
        }


        public static string[] OtolithImagesExtensions
        {
            get
            {
                string path = System.Configuration.ConfigurationManager.AppSettings["OtolithImagesExtensions"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["OtolithImagesExtensions"].ToString();

                if (path != null)
                {
                    var arr = path.Split('|');
                    return arr;
                }

                return null;
            }
        }

        public static string[] OtolithImagesAdditionalFileExtensions
        {
            get
            {
                string path = System.Configuration.ConfigurationManager.AppSettings["OtolithImagesAdditionalFileExtensions"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["OtolithImagesAdditionalFileExtensions"].ToString();

                if (path != null)
                {
                    var arr = path.Split('|');
                    return arr;
                }

                return null;
            }
        }


        private static bool? _searchSubfoldersForChildren = null;


        public static bool SearchSubfoldersForChildren
        {
            get
            {
                if (_searchSubfoldersForChildren == null)
                {
                    try
                    {
                        string val = System.Configuration.ConfigurationManager.AppSettings["SearchSubfoldersForChildren"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["SearchSubfoldersForChildren"].ToString();
                        _searchSubfoldersForChildren = val != null && val.Equals("True", StringComparison.CurrentCultureIgnoreCase);
                    }
                    catch { _searchSubfoldersForChildren = false; }
                }

                return _searchSubfoldersForChildren.Value;
            }
        }


        #endregion



        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }


        public ServiceResult GetFileBytes(string relativeImagePath)
        {

            Anchor.Core.Network.NetworkShareAccessor na = null;

            try
            {
                string error = null;

                var imagesRootPath = OtolithImagesRootPath;

                if (string.IsNullOrWhiteSpace(imagesRootPath))
                {
                    error = "The otolith images path is not configured on the server.";
                    return null;
                }

                //Access network location (if required, if it's not required, the method handles it).
                na = AccessNetworkLocation();

                if (!Directory.Exists(imagesRootPath))
                {
                    error = "The otolith images path could not be reached.";
                    return null;
                }
                
                if(!string.IsNullOrWhiteSpace(error))
                    return new ServiceResult(DatabaseOperationStatus.UnexpectedException, error);

                if (relativeImagePath.StartsWith("\\"))
                    relativeImagePath = relativeImagePath.Substring(1);

                var path = Path.Combine(OtolithImagesRootPath, relativeImagePath);

                if(!File.Exists(path))
                    return new ServiceResult(DatabaseOperationStatus.UnexpectedException, "File was not found.");

                var bytes = path.LoadFileBytesFromPath(FileShare.ReadWrite);

                var sr = new ServiceResult(DatabaseOperationStatus.Successful);
                sr.Data = bytes;
                return sr;
            }
            catch (Exception e)
            {
                return new ServiceResult(DatabaseOperationStatus.UnexpectedException, e.Message + (e.InnerException != null ? (". " + (e.InnerException.Message ?? "")) : ""));
            }
            finally
            {
                SafelyDisposeNetworkShareAccessor(na);
            }
        }


        public ServiceResult GetFileInformationFromAnimalIds(List<string> animalIds, string[] subFolders)
        {
            try
            {
                string error = "";
                var dic = GetFileInformationDictionaryFromAnimalIds(animalIds, subFolders, ref error);

                if (dic == null)
                    return new ServiceResult(DatabaseOperationStatus.UnexpectedException, error ?? "");

                var lst = dic.Values.SelectMany(x => x.Values).Where(x => !string.IsNullOrWhiteSpace(x.FileName)).ToList();

                var ba = lst.ToByteArrayProtoBuf();
                var res = ba.Compress();

                var sr = new ServiceResult(DatabaseOperationStatus.Successful);
                sr.Data = res;
                return sr;
            }
            catch (Exception e)
            {
                return new ServiceResult(DatabaseOperationStatus.UnexpectedException, e.Message + (e.InnerException != null ? (". " + (e.InnerException.Message ?? "")) : ""));
            }
        }


        public ServiceResult GetFileInformationFromFileNames(List<string> fileNames, string[] subFolders)
        {
            try
            {
                string error = "";
                var dic = GetFileInformationDictionaryFromImageNames(fileNames, subFolders, ref error);

                if (dic == null)
                    return new ServiceResult(DatabaseOperationStatus.UnexpectedException, error ?? "");

                var lst = dic.Values.SelectMany(x => x.Values).Where(x => !string.IsNullOrWhiteSpace(x.FileName)).ToList();

                var ba = lst.ToByteArrayProtoBuf();
                var res = ba.Compress();

                var sr = new ServiceResult(DatabaseOperationStatus.Successful);
                sr.Data = res;
                return sr;
            }
            catch (Exception e)
            {
                return new ServiceResult(DatabaseOperationStatus.UnexpectedException, e.Message + (e.InnerException != null ? (". " + (e.InnerException.Message ?? "")) : ""));
            }
        }


        public ServiceResult GetAllFilePaths(string[] subFolders)
        {
            string error = "";
            var imagesRootPath = OtolithImagesRootPath;

            if (string.IsNullOrWhiteSpace(imagesRootPath))
            {
                error = "The otolith images path is not configured on the server.";
                return null;
            }

            //Access network location (if required, if it's not required, the method handles it).
            Anchor.Core.Network.NetworkShareAccessor na = AccessNetworkLocation();

            if (!Directory.Exists(imagesRootPath))
            {
                error = "The otolith images path could not be reached.";
                return null;
            }

            try
            {
                List<string> lstExtensions = new List<string>();
                var oix = OtolithImagesExtensions;
                if (oix != null && oix.Length > 0)
                    lstExtensions.AddRange(oix);

                if (lstExtensions.Count == 0)
                {
                    error = "Supported images extensions is not configured in the web.config on the server.";
                    return null;
                }

                var etxArray = lstExtensions.ToArray();

                string[] allFiles = GetFilePaths(imagesRootPath, etxArray, subFolders);
                /*string[] allFiles = Directory.EnumerateFiles(imagesRootPath, "*.*", SearchOption.AllDirectories)
                                             .Where(x => EndsWith(x, etxArray))
                                             .Select(x => x.Replace(imagesRootPath, "", StringComparison.InvariantCultureIgnoreCase))
                                             .ToArray();*/

                byte[] files = allFiles.ToByteArrayProtoBuf();
                byte[] arrFilesCompressed = files.Compress();

                return new ServiceResult(DatabaseOperationStatus.Successful) { Data = arrFilesCompressed };
            }
            catch (Exception e)
            {
                return new ServiceResult(DatabaseOperationStatus.UnexpectedException, e.Message + (e.InnerException != null ? (". " + (e.InnerException.Message ?? "")) : ""));
            }
            finally
            {
                SafelyDisposeNetworkShareAccessor(na);
            }

        }


        private string[] GetImageExtensions()
        {
            List<string> lstExtensions = new List<string>();
            var oix = OtolithImagesExtensions;
            if (oix != null && oix.Length > 0)
                lstExtensions.AddRange(oix);

            var oiafx = OtolithImagesAdditionalFileExtensions;
            if (oiafx != null && oiafx.Length > 0)
                lstExtensions.AddRange(oiafx);

            return lstExtensions.ToArray();
        }



        //Create regex definition.
        private static string regDefinition = string.Format(@"^\\*(.+\\)*(([0-9]*)_*.+)\.(.+)$");

        /// <summary>
        /// Returns a Dictionary<filenameWithExtension, Dictionary<RelativeDirectoryPath, OtolithFileInformation>>.
        /// If null is returned error has a value.
        /// </summary>
        public Dictionary<string, Dictionary<string, OtolithFileInformation>> GetFileInformationDictionaryFromImageNames(List<string> fileNames, string[] subFolders, ref string error)
        {
            var imagesRootPath = OtolithImagesRootPath;

            if (string.IsNullOrWhiteSpace(imagesRootPath))
            {
                error = "The otolith images path is not configured on the server.";
                return null;
            }

            //Access network location (if required, if it's not required, the method handles it).
            Anchor.Core.Network.NetworkShareAccessor na = AccessNetworkLocation();

            if (!Directory.Exists(imagesRootPath))
            {
                error = "The otolith images path could not be reached.";
                return null;
            }


            try
            {
                var etxArray = GetImageExtensions();

                if (etxArray == null || etxArray.Length == 0)
                {
                    error = "Supported images extensions is not configured in the web.config on the server.";
                    return null;
                }

                string[] allFiles = GetFilePaths(imagesRootPath, etxArray, subFolders);
                /* string[] allFiles = Directory.EnumerateFiles(imagesRootPath, "*.*", SearchOption.AllDirectories)
                                             .Where(x => EndsWith(x, etxArray))
                                             .Select(x => x.Replace(imagesRootPath, "", StringComparison.InvariantCultureIgnoreCase))
                                             .ToArray(); */

                //Instantiate regex
                Regex reg = new Regex(regDefinition, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

                var hsFileNames = fileNames.Select(x => x.ToLower()).Distinct().ToHashSet();

                var hsImageExtension = OtolithImagesExtensions.Select(x => x.ToLower()).Distinct().ToHashSet();


                Dictionary<string, Dictionary<string, OtolithFileInformation>> dic = new Dictionary<string, Dictionary<string, OtolithFileInformation>>();

                for (int i = 0; i < allFiles.Length; i++)
                {
                    var strFilePath = allFiles[i];

                    var m = reg.Match(strFilePath);

                    if (!m.Success || m.Groups.Count < 5)
                        continue;

                    string fileNameWithoutExtensions = m.Groups[2].Value;
                    string fileExtension = (m.Groups[4].Value ?? "").Trim();    //3 letter, without the "."
                    string fileName = string.Format("{0}.{1}", fileNameWithoutExtensions, fileExtension);
                    string fileNameWithFirstDotExtension = GetFileNameWithFirstDotExtension(fileName);

                    if (string.IsNullOrWhiteSpace(fileNameWithoutExtensions) || !hsFileNames.Contains(fileNameWithFirstDotExtension.ToLower()))
                        continue;

                    string animalId = m.Groups[3].Value ?? "";
                    string relativeDirectoryPath = m.Groups[1].Value;
                    
                   
                   
                    bool isJpg = hsImageExtension.Contains(fileExtension.ToLower());

                    Dictionary<string, OtolithFileInformation> dicJpgs = null;
                    OtolithFileInformation fBase = null;
                    string dicJpgsKey = null;

                    if (dic.ContainsKey(fileNameWithFirstDotExtension))
                    {
                        dicJpgs = dic[fileNameWithFirstDotExtension];

                        //Check for whether parent to the relativeDirectoryPath or a path in the same folder as relativeDirectoryPath exists, an assign the additional image file to that.
                        dicJpgsKey = dicJpgs.Keys.Where(x => relativeDirectoryPath.Contains(x, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        if (dicJpgsKey != null)
                            fBase = dicJpgs[dicJpgsKey];
                        else
                        {
                            dicJpgsKey = relativeDirectoryPath;
                            fBase = new OtolithFileInformation(animalId, null, null);
                            dicJpgs.Add(dicJpgsKey, fBase);
                        }
                    }
                    else
                    {
                        fBase = new OtolithFileInformation(animalId, null, null);
                        dicJpgs = new Dictionary<string, OtolithFileInformation>();
                        dicJpgsKey = relativeDirectoryPath;
                        dicJpgs.Add(dicJpgsKey, fBase);
                        dic.Add(fileNameWithFirstDotExtension, dicJpgs);
                    }

                    if (isJpg)
                    {
                        fBase.FileName = fileName;
                        fBase.RelativeDirectoryPath = relativeDirectoryPath;
                    }
                    else
                    {
                        OtolithFileInformation fi = new OtolithFileInformation(animalId, fileName, relativeDirectoryPath);
                        fBase.AddAdditionalFile(fi);
                    }


                    dicJpgs[dicJpgsKey] = fBase;

                    //Make sure the dictionary is updated with any changes.
                    dic[fileNameWithFirstDotExtension] = dicJpgs;
                }

                return dic;
            }
            catch (Exception e)
            {
                error = e.Message + (e.InnerException != null ? (". " + (e.InnerException.Message ?? "")) : "");
            }
            finally
            {
                SafelyDisposeNetworkShareAccessor(na);
            }

            return null;
        }


        /// <summary>
        /// Returns a Dictionary<animalids, Dictionary<filenameWithoutExtension, OtolithFileInformation>>.
        /// If null is returned error has a value.
        /// </summary>
        public Dictionary<string, Dictionary<string, OtolithFileInformation>> GetFileInformationDictionaryFromAnimalIds(List<string> animalIds, string[] subFolders, ref string error)
        {
            var imagesRootPath = OtolithImagesRootPath;

            if (string.IsNullOrWhiteSpace(imagesRootPath))
            {
                error = "The otolith images path is not configured on the server.";
                return null;
            }

            //Access network location (if required, if it's not required, the method handles it).
            Anchor.Core.Network.NetworkShareAccessor na = AccessNetworkLocation();
            
            try
            {
                if (!Directory.Exists(imagesRootPath))
                {
                    error = "The otolith images path could not be reached.";
                    return null;
                }

                var etxArray = GetImageExtensions();

                if (etxArray == null || etxArray.Length == 0)
                {
                    error = "Supported images extensions is not configured in the web.config on the server.";
                    return null;
                }

                string[] allFiles = GetFilePaths(imagesRootPath, etxArray, subFolders);
                /*string[] allFiles = Directory.EnumerateFiles(imagesRootPath, "*.*", SearchOption.AllDirectories)
                                             .Where(x => EndsWith(x, etxArray))
                                             .Select(x => x.Replace(imagesRootPath, "", StringComparison.InvariantCultureIgnoreCase))
                                             .ToArray();*/

                //Convert all paths found to a single semicolon seperated string, so regex expressions can run on it. 
                //string allPaths = string.Join(Environment.NewLine, allFiles.Select(x => x.Replace(imagesRootPath, "", StringComparison.InvariantCultureIgnoreCase)));

                //Join all ids as an regex or.
                //string idsForReg = string.Join("|", animalIds);


                //Instantiate regex
                Regex reg = new Regex(regDefinition, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

                var hsAnimalIds = animalIds.Distinct().ToHashSet();
                var hsImageExtension = OtolithImagesExtensions.Select(x => x.ToLower().Trim()).Distinct().ToHashSet();


                Dictionary<string, Dictionary<string, OtolithFileInformation>> dic = new Dictionary<string, Dictionary<string, OtolithFileInformation>>();

                for (int i = 0; i < allFiles.Length; i++)
                {
                    var strFilePath = allFiles[i];

                    var m = reg.Match(strFilePath);

                    if (!m.Success || m.Groups.Count < 5)
                        continue;

                    string animalId = m.Groups[3].Value ?? "";

                    if (string.IsNullOrWhiteSpace(animalId) || !hsAnimalIds.Contains(animalId))
                        continue;

                    string relativeDirectoryPath = m.Groups[1].Value;
                    string fileExtension = (m.Groups[4].Value ?? "").Trim();    //3 letter, without the "."
                    string fileNameWithoutExtensions = m.Groups[2].Value;
                    string fileNameWithoutExtensionStopAtFirstDot = GetFileNameWithoutExtensionStopAtFirstDot(fileNameWithoutExtensions);
                    string fileName = string.Format("{0}.{1}", fileNameWithoutExtensions, fileExtension);
                    bool isJpg = hsImageExtension.Contains(fileExtension.ToLower().Trim());

                    Dictionary<string, OtolithFileInformation> dicJpgs = null;
                    OtolithFileInformation fBase = null;
                    if (dic.ContainsKey(animalId))
                    {
                        dicJpgs = dic[animalId];

                        if (dicJpgs.ContainsKey(fileNameWithoutExtensionStopAtFirstDot))
                            fBase = dicJpgs[fileNameWithoutExtensionStopAtFirstDot];
                        else
                        {
                            fBase = new OtolithFileInformation(animalId, null, null);
                            dicJpgs.Add(fileNameWithoutExtensionStopAtFirstDot, fBase);
                        }
                    }
                    else
                    {
                        fBase = new OtolithFileInformation(animalId, null, null);
                        dicJpgs = new Dictionary<string, OtolithFileInformation>();
                        dicJpgs.Add(fileNameWithoutExtensionStopAtFirstDot, fBase);
                        dic.Add(animalId, dicJpgs);
                    }

                    if (isJpg)
                    {
                        fBase.FileName = fileName;
                        fBase.RelativeDirectoryPath = relativeDirectoryPath;
                    }
                    else
                    {
                        OtolithFileInformation fi = new OtolithFileInformation(animalId, fileName, relativeDirectoryPath);
                        fBase.AddAdditionalFile(fi);
                    }

                    dicJpgs[fileNameWithoutExtensionStopAtFirstDot] = fBase;

                    //Make sure the dictionary is updated with any changes.
                    dic[animalId] = dicJpgs;
                }

                return dic;
            }
            catch (Exception e)
            {
                error = e.Message + (e.InnerException != null ? (". " + (e.InnerException.Message ?? "")) : "");
            }
            finally
            {
                SafelyDisposeNetworkShareAccessor(na);
            }

            return null;
        }

      

        private Anchor.Core.Network.NetworkShareAccessor AccessNetworkLocation(int trialNo = 1)
        {
            if (OtolithImagesServerConnectToUNCPath)
            {
                string otolithImagesServerName = OtolithImagesServerName;
                string otolithImagesServerDomainOrComputerName = OtolithImagesServerDomainOrComputerName;
                string otolithImagesServerUser = OtolithImagesServerUser;
                string otolithImagesServerPassword = OtolithImagesServerPassword;

                if (!string.IsNullOrEmpty(otolithImagesServerName) && !string.IsNullOrEmpty(otolithImagesServerUser) && !string.IsNullOrEmpty(otolithImagesServerPassword))
                {
                    //Make sure to disconnect, so it is possible to connect again. But only do it once for each session, therefore the static disconnectUNCBeforeConnect check.
                    if (_disconnectUNCBeforeConnect)
                    {
                        try
                        {
                            string strUNC = otolithImagesServerName;
                            if (!strUNC.StartsWith(@"\\"))
                                strUNC = @"\\" + strUNC;

                            Anchor.Core.Network.NetworkShareAccessor.DisconnectFromShare(strUNC, false);
                        }
                        catch { }
                        _disconnectUNCBeforeConnect = false;
                    }

                    //Connect to server.
                    var res = Anchor.Core.Network.NetworkShareAccessor.Access(otolithImagesServerName, otolithImagesServerDomainOrComputerName, otolithImagesServerUser, otolithImagesServerPassword);

                    if (res == null && trialNo > 0)
                       return AccessNetworkLocation(trialNo - 1); //Try again one more time.
                    
                    return res;
                }
            }

            return null;
        }


        private void SafelyDisposeNetworkShareAccessor(Anchor.Core.Network.NetworkShareAccessor na)
        {
            if (na != null)
            {
                try
                {
                    na.Dispose();
                }
                catch { }
            }
        }


        /// <summary>
        /// With a filename of 8169670_ALA_RLX_B0.jpg.cal.xml, return 8169670_ALA_RLX_B0
        /// </summary>
        private string GetFileNameWithoutExtensionStopAtFirstDot(string st)
        {
            if (st == null)
                return st;

            int index = st.IndexOf('.');
            if (index > 0)
            {
                return st.Substring(0, index);
            }

            return st;
        }

        /// <summary>
        /// With a filename of 8169670_ALA_RLX_B0.jpg.cal.xml, return 8169670_ALA_RLX_B0.jpg
        /// </summary>
        private string GetFileNameWithFirstDotExtension(string st)
        {
            if (st == null)
                return st;

            int index = st.IndexOf('.');

            if (index > 0 && index + 1 < st.Length)
            {

                int i = st.IndexOf('.', index + 1);

                if (i > 0)
                    return st.Substring(0, i);

                return st;
            }

            return st;
        }


        private static bool EndsWith(string strFile, string[] lstExtensions = null)
        {
            if (lstExtensions == null || lstExtensions.Length == 0)
                return true;

            for (int i = 0; i < lstExtensions.Length; i++)
                if (!string.IsNullOrWhiteSpace(lstExtensions[i]) && strFile.EndsWith(lstExtensions[i], StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }


        /// <summary>
        /// Gets either the folders or files or both from a folder given by the relative path (subFolderPath). 
        /// If type == FileSystemType.File, only files will be returned.
        /// If type == FileSystemType.Directory, only directories will be returned.
        /// If type == FileSystemType.File | FileSystemType.Directory, files and directories will be returned.
        /// </summary>
        public ServiceResult GetFolderContentCompressed(string subFolderPath, FileSystemType type)
        {
            try
            {
                string error = "";
                var lst = GetFolderContent(subFolderPath, type, ref error);

                if (lst == null || !string.IsNullOrWhiteSpace(error))
                    return new ServiceResult(DatabaseOperationStatus.UnexpectedException, error ?? "");

                var ba = lst.ToByteArrayProtoBuf();
                var res = ba.Compress();

                var sr = new ServiceResult(DatabaseOperationStatus.Successful);
                sr.Data = res;
                return sr;
            }
            catch (Exception e)
            {
                return new ServiceResult(DatabaseOperationStatus.UnexpectedException, e.Message + (e.InnerException != null ? (". " + (e.InnerException.Message ?? "")) : ""));
            }
        }


        public bool HasSubDirectories(string path)
        {
            try
            {
                

                //return FastDirectoryEnumerator.EnumerateDirectories(path).Where(x => !x.Attributes.HasFlag(FileAttributes.Hidden)).Any();

                //return FastDirectoryEnumerator.EnumerateFilesAndDirectories(path).Where(x => !x.Attributes.HasFlag(FileAttributes.Hidden)).Any();

                if(SearchSubfoldersForChildren)
                    return Directory.EnumerateDirectories(path).Where(x => !new DirectoryInfo(x).Attributes.HasFlag(FileAttributes.Hidden)).Any();
                else
                //It's very slow to check the subfolders, since a lot of them has 3000 images but no folders. So it loops through all 3000 until it founds out there were no folders there.
                    return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Gets either the folders or files or both from a folder given by the relative path (subFolderPath). 
        /// If type == FileSystemType.File, only files will be returned.
        /// If type == FileSystemType.Directory, only directories will be returned.
        /// If type == FileSystemType.File | FileSystemType.Directory, files and directories will be returned.
        /// </summary>
        public List<FileSystemItem> GetFolderContent(string subFolderPath, FileSystemType type, ref string error)
        {
            List<FileSystemItem> lstItems = new List<FileSystemItem>();

            var imagesRootPath = OtolithImagesRootPath;

            if (string.IsNullOrWhiteSpace(imagesRootPath))
            {
                error = "The otolith images path is not configured on the server.";
                return null;
            }

            var etxArray = GetImageExtensions();

            if (etxArray == null || etxArray.Length == 0)
            {
                error = "Supported images extensions is not configured in the web.config on the server.";
                return null;
            }

            //Access network location (if required, if it's not required, the method handles it).
            Anchor.Core.Network.NetworkShareAccessor na = AccessNetworkLocation();

            try
            {
                if (!Directory.Exists(imagesRootPath))
                {
                    error = "The otolith images path could not be reached.";
                    return null;
                }

                string path = imagesRootPath;

                if(!string.IsNullOrWhiteSpace(subFolderPath) && subFolderPath != @"\")
                    path = System.IO.Path.Combine(imagesRootPath, subFolderPath);

                //If the directory does not exist, skip it. 
                if (!Directory.Exists(path))
                {
                    error = "Folder path do not exist.";
                    return null;
                }

                DirectoryInfo dirInfo = new DirectoryInfo(path);

                //Check if directories should be returned.
                if ((type & FileSystemType.Directory) == FileSystemType.Directory)
                {
                   // var dirs = FastDirectoryEnumerator.EnumerateDirectories(path).ToArray();
                    //var dirsTest = Directory.GetDirectories(path);
                   
                    var dirs = dirInfo.GetDirectories();
                    
                    //Get directories
                    if (dirs != null && dirs.Length > 0)
                    {
                        foreach (var d in dirs)
                        {
                            if (d.Attributes.HasFlag(FileAttributes.Hidden))
                                continue;

                            var fullPath = d.FullName;
                            var relPath = fullPath.Replace(imagesRootPath, "", StringComparison.InvariantCultureIgnoreCase);
                            while (relPath.StartsWith(@"\"))
                                relPath = relPath.Substring(1);

                            var hasSubs = HasSubDirectories(fullPath);
                            var di = FileSystemItem.Create(relPath, FileSystemType.Directory) as FileSystemDirectory;
                            di.HasSubdirectories = hasSubs;
                            lstItems.Add(di);
                        }
                    }
                }

                //Check if files should be returned.
                if ((type & FileSystemType.File) == FileSystemType.File)
                {
                    var files = dirInfo.GetFiles();
                    //var files = FastDirectoryEnumerator.EnumerateFiles(path).ToArray();

                    //Get files
                    if (files != null && files.Length > 0)
                    {
                        foreach (var f in files)
                        {
                            var fullPath = f.FullName;

                            if (!EndsWith(fullPath, etxArray))
                                continue;

                            var relPath = fullPath.Replace(imagesRootPath, "", StringComparison.InvariantCultureIgnoreCase);

                            var di = FileSystemItem.Create(relPath, FileSystemType.File);
                            lstItems.Add(di);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                error = e.Message;
                return null;
            }
            finally
            {
                SafelyDisposeNetworkShareAccessor(na);
            }

            return lstItems;
        }



        /// <summary>
        /// Get file paths from the imagesRootPath if subfolders = null, otherwise from all the subfolders. Only returns the files with the extensions given in extArray.
        /// </summary>
        /// <param name="imagesRootPath">The main directory (root directory)</param>
        /// <param name="extArray">Array of file extensions to return (without the ".", just jpg, bmp, etc.)</param>
        /// <param name="subfolders">Array of subfolders (relative paths from root directory)</param>
        /// <returns></returns>
        private string[] GetFilePaths(string imagesRootPath, string[] extArray, string[] subfolders)
        {
            string[] arrFiles = null;

            if (string.IsNullOrWhiteSpace(imagesRootPath))
                return arrFiles;

            if (subfolders == null || subfolders.Length == 0)
            {
                arrFiles = Directory.EnumerateFiles(imagesRootPath, "*.*", SearchOption.AllDirectories)
                                                .Where(x => EndsWith(x, extArray))
                                                .Select(x => x.Replace(imagesRootPath, "", StringComparison.InvariantCultureIgnoreCase))
                                                .ToArray();
            }
            else
            {
                HashSet<string> hsAlreadyAdded = new HashSet<string>();
                List<string> lstFiles = new List<string>();
                foreach (var s in subfolders)
                {
                    try
                    {
                        //If the folder is null or white space, skip it.
                        if (string.IsNullOrWhiteSpace(s))
                            continue;

                        var lowerS = s.ToLower();

                        //If the folder has already been enumerated, skip it and continue to next.
                        if (hsAlreadyAdded.Contains(lowerS))
                            continue;

                        string path = System.IO.Path.Combine(imagesRootPath, s);

                        //If the directory does not exist, skip it. 
                        if (!Directory.Exists(path))
                            continue;

                        var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                                             .Where(x => EndsWith(x, extArray))
                                             .Select(x => x.Replace(imagesRootPath, "", StringComparison.InvariantCultureIgnoreCase));

                        //Add any files found. AddRange supports an empty enumeration, which is why there is no count/length check beforehand. 
                        lstFiles.AddRange(files);

                        hsAlreadyAdded.Add(lowerS);
                    }
                    catch { }
                }

                arrFiles = lstFiles.ToArray();
            }


            return arrFiles;
        }



        /// <summary>
        /// Returns a Dictionary<animalids, Dictionary<filenameWithoutExtension, OtolithFileInformation>>.
        /// If null is returned error has a value.
        /// </summary>
        /*public Dictionary<string, Dictionary<string, OtolithFileInformation>> GetFileInformationDictionaryFromAnimalIds(List<string> animalIds, ref string error)
        {
            var imagesRootPath = OtolithImagesRootPath;

            if (string.IsNullOrWhiteSpace(imagesRootPath))
            {
                error = "The otolith images path is not configured on the server.";
                return null;
            }

            if (!Directory.Exists(imagesRootPath))
            {
                error = "The otolith images path could not be reached.";
                return null;
            }


            try
            {
                var allFiles = Directory.GetFiles(imagesRootPath, "*.*", SearchOption.AllDirectories);

                //Convert all paths found to a single semicolon seperated string, so regex expressions can run on it. 
                string allPaths = string.Join(Environment.NewLine, allFiles.Select(x => x.Replace(imagesRootPath, "", StringComparison.InvariantCultureIgnoreCase)));

                //Join all ids as an regex or.
                string idsForReg = string.Join("|", animalIds);

                //Create regex definition.
                string regDefinition = string.Format(@"^\\+(.+\\)*(({0})_.+)\.(.+)$", idsForReg);

                //Instantiate regex
                Regex reg = new Regex(regDefinition, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

                //find matches.
                var ms = reg.Matches(allPaths);

                if (ms != null)
                {
                    Dictionary<string, Dictionary<string, OtolithFileInformation>> dic = new Dictionary<string, Dictionary<string, OtolithFileInformation>>();

                    //Loop through all matches and build up return list.
                    foreach (Match m in ms)
                    {
                        if (!m.Success || m.Groups.Count < 5)
                            continue;

                        string relativeDirectoryPath = m.Groups[1].Value;
                        string animalId = m.Groups[3].Value ?? "";
                        string fileExtension = (m.Groups[4].Value ?? "").Trim();    //3 letter, without the "."
                        string fileNameWithoutExtension = m.Groups[2].Value;
                        string fileName = string.Format("{0}.{1}", fileNameWithoutExtension, fileExtension);
                        bool isJpg = fileExtension.Equals("jpg", StringComparison.InvariantCultureIgnoreCase);

                        Dictionary<string, OtolithFileInformation> dicJpgs = null;
                        OtolithFileInformation fBase = null;
                        if (dic.ContainsKey(animalId))
                        {
                            dicJpgs = dic[animalId];

                            if (dicJpgs.ContainsKey(fileNameWithoutExtension))
                                fBase = dicJpgs[fileNameWithoutExtension];
                            else
                            {
                                fBase = new OtolithFileInformation(animalId, null, null);
                                dicJpgs.Add(fileNameWithoutExtension, fBase);
                            }
                        }
                        else
                        {
                            fBase = new OtolithFileInformation(animalId, null, null);
                            dicJpgs = new Dictionary<string, OtolithFileInformation>();
                            dicJpgs.Add(fileNameWithoutExtension, fBase);
                            dic.Add(animalId, dicJpgs);
                        }

                        if (isJpg)
                        {
                            fBase.FileName = fileName;
                            fBase.RelativeDirectoryPath = relativeDirectoryPath;
                        }
                        else
                        {
                            OtolithFileInformation fi = new OtolithFileInformation(animalId, fileName, relativeDirectoryPath);
                            fBase.AddAdditionalFile(fi);
                        }

                        dicJpgs[fileNameWithoutExtension] = fBase;

                        //Make sure the dictionary is updated with any changes.
                        dic[animalId] = dicJpgs;
                    }

                    return dic;
                }
                else
                    return new Dictionary<string, Dictionary<string, OtolithFileInformation>>();
            }
            catch (Exception e)
            {
                error = e.Message + (e.InnerException != null ? (". " + (e.InnerException.Message ?? "")) : "");
            }

            return null;
        }*/

    }
}
