using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using Babelfisk.FileSynchronizer.Classes;
using Anchor.Core;
using System.Threading;

namespace Babelfisk.FileSynchronizer
{
    class Program
    {
        static readonly Mutex _Mutex = new Mutex(true, "FishLine.FileSynchronizer");

        public static int ChunkSize
        {
            get
            {
                int size = 1000;

                try
                {
                    int tmp = 0;
                    if (ConfigurationManager.AppSettings["ChunkSize"] != null && ConfigurationManager.AppSettings["ChunkSize"].ToString().TryParseInt32(out tmp))
                        size = tmp;
                }
                catch { }

                return size;
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("------ Initializing FishLine image file synchronizer -------");
            DataManager datMan = new DataManager();

            Console.WriteLine("");
            Console.WriteLine(" - Checking if application is already running.");

            try
            {
                if (_Mutex.WaitOne(TimeSpan.Zero, true))
                {
                     SynchronizeOtolithImages(datMan);
                }
                else
                    DataManager.LogInfo(" - Application is already running, shutting down.");
            }
            catch (Exception ex)
            {
                DataManager.LogError(ex);
            }
        }


        private static void SynchronizeOtolithImages(DataManager datMan)
        {
            //1) Retrieve animal ids from database that already have files attached

            Console.WriteLine("");
            Console.WriteLine(" - Retrieving animals with image files from DB.");
            //Retrieve list of animals with already added animal files. If NULL is returned, something went wrong.
            var dicAnimalsWithFiles = datMan.GetAnimalsWithAnimalFiles();

            if (dicAnimalsWithFiles == null)
            {
                DataManager.LogError("Retrieving animals with image files from FishLine database failed.");
                return;
            }

            Console.WriteLine(" - Found {0} animals with image files.", dicAnimalsWithFiles.Count);
            Console.WriteLine("");

            List<string> lstExtensions = new List<string>();

            //Retrieve file types (extensions) to search for.
            var strExtensions = ConfigurationManager.AppSettings["ImageExtensionsToInclude"] == null ? "" : ConfigurationManager.AppSettings["ImageExtensionsToInclude"].ToString();
            var arrExtensions = strExtensions.Split(',');
            lstExtensions = arrExtensions.Select(x => x.Trim()).ToList();

            string reg = "_ai([0-9]+).({0})";

            if (ConfigurationManager.AppSettings["FileNameGrabAnimalIdRegEx"] != null)
                reg = ConfigurationManager.AppSettings["FileNameGrabAnimalIdRegEx"].ToString();

            //Create regex for extracting id from file name.
            string strRegex = reg.Contains("{0}") ? string.Format(reg, lstExtensions.Count == 0 ? "" : string.Join("|", lstExtensions)) : reg;

            Console.WriteLine(" - Using following regex to extract animal id from filenames: {0}.", strRegex);

            Regex regIds = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);


            string strFileLocation = ConfigurationManager.AppSettings["OtolithImagesBaseFolderPathServer"] == null ? null : ConfigurationManager.AppSettings["OtolithImagesBaseFolderPathServer"].ToString();

            if (strFileLocation == null)
            {
                DataManager.LogError("Images base folder was not found in config file.");
                return;
            }
   
            
            //2) Find all image files on network-drive with file name containing an animal id.

            Dictionary<int, List<DBResultAnimal>> dicFilesWithAnimalIds = new Dictionary<int, List<DBResultAnimal>>();
            Anchor.Core.Network.NetworkShareAccessor na = null;
            try
            {
                Console.WriteLine(" - Connecting to server containing image files.");
                string otolithImagesServerName = ConfigurationManager.AppSettings["OtolithImagesServerName"] == null ? null : ConfigurationManager.AppSettings["OtolithImagesServerName"].ToString();
                string otolithImagesServerDomainOrComputerName = ConfigurationManager.AppSettings["otolithImagesServerDomainOrComputerName"] == null ? null : ConfigurationManager.AppSettings["otolithImagesServerDomainOrComputerName"].ToString();
                string otolithImagesServerUser = ConfigurationManager.AppSettings["OtolithImagesServerUser"] == null ? null : ConfigurationManager.AppSettings["OtolithImagesServerUser"].ToString();
                string otolithImagesServerPassword = ConfigurationManager.AppSettings["OtolithImagesServerPassword"] == null ? null : ConfigurationManager.AppSettings["OtolithImagesServerPassword"].ToString();

                if (!string.IsNullOrEmpty(otolithImagesServerName) && !string.IsNullOrEmpty(otolithImagesServerUser) && !string.IsNullOrEmpty(otolithImagesServerPassword))
                {
                    //Make sure to disconnect, so it is possible to connect again.
                    try
                    {
                        string strUNC = otolithImagesServerName;
                        if (!strUNC.StartsWith(@"\\"))
                            strUNC = @"\\" + strUNC;

                        Anchor.Core.Network.NetworkShareAccessor.DisconnectFromShare(strUNC);
                    }
                    catch { }

                    //Connect to server.
                    na = Anchor.Core.Network.NetworkShareAccessor.Access(otolithImagesServerName, otolithImagesServerDomainOrComputerName, otolithImagesServerUser, otolithImagesServerPassword);
                }

                Console.WriteLine(" - Searching for image files with animal ids.");

                var allFiles = Directory.EnumerateFiles(strFileLocation, "*.*", SearchOption.AllDirectories)
                                        .Where(x => EndsWith(x, lstExtensions));
                int filesCount = 0;
                foreach(var f in allFiles)
                {
                    filesCount++;
                    var m = regIds.Match(f);

                    if (m.Success && m.Groups.Count > 1)
                    {
                        string strId = m.Groups[1].Value;
                        int intId = 0;

                        if (strId.TryParseInt32(out intId))
                        {
                            DBResultAnimal dbRes = new DBResultAnimal(intId, GetPath(f, strFileLocation), true);

                            if (dicFilesWithAnimalIds.ContainsKey(intId))
                                dicFilesWithAnimalIds[intId].Add(dbRes);
                            else
                                dicFilesWithAnimalIds.Add(intId, new List<DBResultAnimal>() { dbRes });
                        }
                    }
                }

                Console.WriteLine(" - Found {0} animal ids in {1} image files.", dicFilesWithAnimalIds.Count, filesCount);
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                DataManager.LogError(e);
                return;
            }
            finally
            {
                if(na != null)
                {
                    try
                    {
                        na.Dispose();
                    }
                    catch{}
                }
            }


            if (dicFilesWithAnimalIds == null || dicFilesWithAnimalIds.Count == 0)
            {
                DataManager.LogError("No image files found at destination, cancelling search.");
                return;
            }

            
            //3) Synchronize found files with existing ones.
            Console.WriteLine(" - Synchronizing information.");
            List<int> lstChangedAnimalIds = SynchronizeImages(datMan, dicAnimalsWithFiles, dicFilesWithAnimalIds);


            //4) Add cruises with changed animal ids to queue to be transferred to DW.
            if (lstChangedAnimalIds.Count > 0)
            {
                Console.WriteLine("");
                Console.WriteLine(" - Setting affected cruises to be (re)transferred to DW.");
                datMan.InsertDWCruisesToTransfer(lstChangedAnimalIds, ChunkSize);
            }
        }



        private static string GetPath(string strFullPath, string strServerFolder)
        {
            //Return full network path to image at the moment
            return strFullPath;
            //return Regex.Replace(strFullPath, Regex.Escape(strServerFolder), "", RegexOptions.IgnoreCase);
        }



        private static List<int> SynchronizeImages(DataManager datMan, Dictionary<int, List<DBResultAnimal>> dicDatabase, Dictionary<int, List<DBResultAnimal>> dicNetworkLocation)
        {
            List<int> lstChangedAnimalIds = new List<int>();

            List<DBResultAnimal> lstAdd = new List<DBResultAnimal>();
            List<DBResultAnimal> lstRemove = new List<DBResultAnimal>();

            foreach (var itm in dicNetworkLocation)
            {
                //If animalId does not exist in dicDatabase, all of the found images should be added directly.
                if (!dicDatabase.ContainsKey(itm.Key))
                {
                    lstAdd.AddRange(itm.Value);
                }
                else //Else merge image files by removing ones not existing anymore and adding new ones.
                {
                    var lstExistingFiles = dicDatabase[itm.Key];

                    //Add new ones
                    foreach (var a in itm.Value)
                    {
                        if (!lstExistingFiles.Where(x => x.FilePath.Equals(a.FilePath, StringComparison.InvariantCultureIgnoreCase)).Any())
                            lstAdd.Add(a);
                    }

                    //Remove the ones not there.
                    foreach (var a in lstExistingFiles)
                    {
                        if (!itm.Value.Where(x => x.FilePath.Equals(a.FilePath, StringComparison.InvariantCultureIgnoreCase)).Any())
                            lstRemove.Add(a);
                    }
                }
            }

            //Remove all animal files that are not present in any of the network files.
            foreach (var itm in dicDatabase)
            {
                if (!dicNetworkLocation.ContainsKey(itm.Key))
                    lstRemove.AddRange(itm.Value);
            }

            Console.WriteLine(" - Updating database. Removing {0} recrods, adding {1} records.", lstRemove.Count, lstAdd.Count);

            //Perform database updates
            if (lstAdd.Count > 0 || lstRemove.Count > 0)
            {
                bool blnSuccess = UpdateDatabase(datMan, lstAdd, lstRemove);
            }

            //Get a distinct list of all the animal ids.
            if(lstAdd.Count > 0)
                lstChangedAnimalIds.AddRange(lstAdd.Select(x => x.AnimalId));

            if (lstRemove.Count > 0)
                lstChangedAnimalIds.AddRange(lstRemove.Select(x => x.AnimalId));

            lstChangedAnimalIds = lstChangedAnimalIds.Distinct().ToList();

            return lstChangedAnimalIds;

        }


        private static bool UpdateDatabase(DataManager datMan, List<DBResultAnimal> lstAdd, List<DBResultAnimal> lstRemove)
        {
            bool blnSuccess = datMan.DeleteAnimalFiles(lstRemove, ChunkSize);

            if (!blnSuccess)
                return false;

            blnSuccess = datMan.InsertAnimalFiles(lstAdd);

            return blnSuccess;
        }


        private static bool EndsWith(string strFile, List<string> lstExtensions = null)
        {
            if (lstExtensions == null || lstExtensions.Count == 0)
                return true;

            for (int i = 0; i < lstExtensions.Count; i++)
                if (strFile.EndsWith(lstExtensions[i], StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }
    }
}
