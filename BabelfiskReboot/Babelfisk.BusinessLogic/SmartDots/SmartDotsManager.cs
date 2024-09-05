using Babelfisk.Entities.Sprattus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anchor.Core;
using Babelfisk.Entities;
using Babelfisk.Entities.FileEntities;
using Babelfisk.Warehouse.Model;

namespace Babelfisk.BusinessLogic.SmartDots
{
    public class SmartDotsManager
    {
        #region SDEvent methods


        /// <summary>
        /// Retrieve all SDEvent entities
        /// </summary>
        public List<SDEvent> GetSDEvents()
        {
            //var paths = GetAllFilePaths();
            //var lstrt = GetFileInformationFromAnimalIds(new string[] { "34353" });
            //var  strt = GetFileInformationFromAnimalIds(new string[] { "8353205" });
            //var ggte = GetSelectionAnimals("SIL", new int[] { 2021 }, new string[] { "4b" }).Reverse<SelectionAnimal>().ToList();
            //var gseg = GetSelectionAnimals(new int[] { 8352606, 923, 92345, 1234 });
            //var fn = GetFileInformationFromFileNames(new string[] { "8353205_ALA_RLX_XX.jpg", "8353205_ALA_RLX_XX.jpg", "8352611_ALA_RLX_XX.jpg" });

            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as BabelfiskService.ISmartDots).GetSDEventsCompressed();

                sv.Close();

                if (arr == null)
                    return null;

                var darr = arr.Decompress();
                var lst = darr.ToObjectDataContract<List<SDEvent>>();

                return lst;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Add, Edit or Delete SDEvent. 
        /// </summary>
        public DatabaseOperationResult SaveSDEvent(ref SDEvent evt)
        {
            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as BabelfiskService.ISmartDots).SaveSDEvent(ref evt);

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


        #endregion


        #region SDSample methods


        /// <summary>
        /// Retrieve all SDSample entities from an event id.
        /// </summary>
        public List<SDSample> GetSDSamples(int eventId)
        {
            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as BabelfiskService.ISmartDots).GetSDSamplesCompressed(eventId);

                sv.Close();

                if (arr == null)
                    return null;

                var darr = arr.Decompress();
                var lst = darr.ToObjectDataContract<List<SDSample>>();

                return lst;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Retrieve all SDSample entities with supplied includes from an event id.
        /// </summary>
        public List<SDSample> GetSDSamplesWithIncludes(int eventId, params string[] includes)
        {
            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as BabelfiskService.ISmartDots).GetSDSamplesWithIncludesCompressed(eventId, includes);

                sv.Close();

                if (arr == null)
                    return null;

                var darr = arr.Decompress();
                var lst = darr.ToObjectDataContract<List<SDSample>>();

                return lst;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Add, Edit or Delete SDSamples.
        /// Add: New SDSamples with it's whole hierarchy of SDFiles, SDAnnotations, SDFiles, and SDPoints can be saved.
        /// Delete: Existing SDSamples with their whole hierarchy of SDFiles, SDAnnotations, SDFiles, and SDPoints can be deleted.
        /// Edit: Only the value type details of SDSamples will be edited (not the full object hierarchy). Meaning if a new SDFile has been added or removed, this won't currently be updated.
        /// </summary>
        public DatabaseOperationResult SaveSDSamples(IEnumerable<SDSample> samples)
        {
            if (samples == null || !samples.Any())
                return DatabaseOperationResult.CreateSuccessResult();


            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as BabelfiskService.ISmartDots).SaveSDSamples(samples.ToArray());

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


        public DatabaseOperationResult MoveOrCopySamplesToEvent(List<int> sampleIds, int toEventId, bool copy)
        {
            if (sampleIds == null || !sampleIds.Any())
                return DatabaseOperationResult.CreateSuccessResult();


            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as BabelfiskService.ISmartDots).MoveOrCopySamplesToEvent(sampleIds.ToArray(), toEventId, copy);

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



        #endregion

        #region Animal methods
            /// <summary>
            /// Retrieve a distinct list of cruise years in descending order.
            /// </summary>
            public List<int> GetCruiseYears()
        {
            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as BabelfiskService.ISmartDots).GetCruiseYears();

                sv.Close();

                return (arr ?? new int[] { }).ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Retrieve animals that the user can select from for adding to an event. 
        /// A species code and optionally a list of years is needed. If no years are supplied, data for all years are returned.
        /// </summary>
        public List<SelectionAnimal> GetSelectionAnimals(string speciesCode, int[] years, string[] areaCodes, bool includeAnimalJpgNames = true, string[] relativeImagePaths = null)
        {
            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as BabelfiskService.ISmartDots).GetSelectionAnimalsCompressed(speciesCode, years, areaCodes, includeAnimalJpgNames, relativeImagePaths);

                sv.Close();

                if (arr == null)
                    return null;

                var darr = arr.Decompress();
                var lst = darr.ToObjectProtoBuf<List<SelectionAnimal>>();

                return lst;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }



        /// <summary>
        /// Retrieve selection animals with specific id's for adding to an event. 
        /// </summary>
        public List<SelectionAnimal> GetSelectionAnimals(int[] animalIds, bool includeAnimalJpgNames = true, string[] relativeImagePaths = null)
        {
            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as BabelfiskService.ISmartDots).GetSelectionAnimalsByIdCompressed(animalIds, includeAnimalJpgNames, relativeImagePaths);

                sv.Close();

                if (arr == null)
                    return null;

                var darr = arr.Decompress();
                var lst = darr.ToObjectProtoBuf<List<SelectionAnimal>>();

                return lst;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }



        /// <summary>
        /// Retrieve a list of file information object for supplied animal ids. If no ids are supplied, an empty list is returned.
        /// </summary>
        public List<OtolithFileInformation> GetFileInformationFromAnimalIds(IEnumerable<string> animalIds, string[] imageFolderPaths)
        {
            if (animalIds == null || !animalIds.Any())
                return new List<OtolithFileInformation>();

            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var sr = (sv as BabelfiskService.ISmartDots).GetFileInformationFromAnimalIds(animalIds.ToArray(), imageFolderPaths);

                sv.Close();

                if (sr == null)
                    throw new ApplicationException("Unabled to call GetFileInformationFromAnimalIds service method.");

                if(sr.Result != DatabaseOperationStatus.Successful)
                    throw new ApplicationException(sr.Message);

                var arr = sr.Data as byte[];

                if (arr == null)
                    return new List<OtolithFileInformation>();

                var darr = arr.Decompress();
                var lst = darr.ToObjectProtoBuf<List<OtolithFileInformation>>();

                return lst;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Retrieve a list of file information object for supplied FileNames. If no ids are supplied, an empty list is returned.
        /// </summary>
        public Dictionary<string, List<OtolithFileInformation>> GetFileInformationFromFileNames(IEnumerable<string> fileNames, string[] imageFolderPaths)
        {
            if (fileNames == null || !fileNames.Any())
                return new Dictionary<string, List<OtolithFileInformation>>();

            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var sr = (sv as BabelfiskService.ISmartDots).GetFileInformationFromFileNames(fileNames.ToArray(), imageFolderPaths);

                sv.Close();

                if (sr == null)
                    throw new ApplicationException("Unabled to call GetFileInformationFromAnimalIds service method.");

                if (sr.Result != DatabaseOperationStatus.Successful)
                    throw new ApplicationException(sr.Message);

                var arr = sr.Data as byte[];

                if (arr == null)
                    return new Dictionary<string, List<OtolithFileInformation>>();

                var darr = arr.Decompress();
                var lst = darr.ToObjectProtoBuf<List<OtolithFileInformation>>();
                var dic = new Dictionary<string, List<OtolithFileInformation>>();
             
                if (lst != null && lst.Count > 0)
                {
                    foreach (var f in lst)
                    {
                        var fileNameLower = f.FileNameLowerCase;

                        if (!dic.ContainsKey(fileNameLower))
                            dic.Add(fileNameLower, new List<OtolithFileInformation>());

                        dic[fileNameLower].Add(f);
                    }
                }

                return dic;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Gets either the folders or files or both from a folder given by the relative path (folderRelativePath). 
        /// If type == FileSystemType.File, only files will be returned.
        /// If type == FileSystemType.Directory, only directories will be returned.
        /// If type == FileSystemType.File | FileSystemType.Directory, files and directories will be returned.
        /// </summary>
        public List<FileSystemItem> GetFolderContent(string folderRelativePath, FileSystemType type)
        {
            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var sr = (sv as BabelfiskService.ISmartDots).GetFolderContentCompressed(folderRelativePath, type);

                sv.Close();

                if (sr == null)
                    throw new ApplicationException("Unabled to call GetFolderContentCompressed service method.");

                if (sr.Result != DatabaseOperationStatus.Successful)
                    throw new ApplicationException(sr.Message);

                var arr = sr.Data as byte[];

                if (arr == null)
                    return new List<FileSystemItem>();

                var darr = arr.Decompress();
                var lst = darr.ToObjectProtoBuf<List<FileSystemItem>>();
               
                return lst;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Retrieve a list of file information object for supplied animal ids. If no ids are supplied, an empty list is returned.
        /// </summary>
        public string[] GetAllFilePaths(string[] serverImageFolders)
        {
            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var sr = (sv as BabelfiskService.ISmartDots).GetAllFilesPaths(serverImageFolders);

                sv.Close();

                if (sr == null)
                    throw new ApplicationException("No files were returned.");

                if (sr.Result != DatabaseOperationStatus.Successful)
                    throw new ApplicationException(sr.Message);

                var arr = sr.Data as byte[];

                if (arr == null)
                    return new string[] { };

                var darr = arr.Decompress();
                var lst = darr.ToObjectProtoBuf<string[]>();

                return lst;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Retrieve an otolith file from the server.
        /// </summary>
        /// <param name="relativeImagePath">The relative path to the image</param>
        public ServiceResult GetFileBytes(string relativeImagePath)
        {
            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var sr = (sv as BabelfiskService.ISmartDots).GetFileBytes(relativeImagePath);

                sv.Close();

                return sr;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Update FishLine animal/age records with the ages supplied in the input parameter list.
        /// </summary>
        public DatabaseOperationResult CopyAgesToFishLine(List<SDAnimalAgeItem> animalIdsAndAges)
        {
            if (animalIdsAndAges == null || animalIdsAndAges.Count == 0)
                return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "NothingToUpdate", "Please supply at least one animal to update.");

            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var sr = (sv as BabelfiskService.ISmartDots).CopyAgesToFishLine(animalIdsAndAges.ToArray());

                sv.Close();

                return sr;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        #endregion


        /// <summary>
        /// Retrieve SDAnnotation from its id.
        /// </summary>
        public SDAnnotation GetSDAnnotation(int intAnnotationId)
        {
            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                SDAnnotation anno = (sv as BabelfiskService.ISmartDots).GetSDAnnotation(intAnnotationId);

                sv.Close();

                return anno;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }



        /// <summary>
        /// Retrieve a list of R_SDReaderStatistics entities.
        /// </summary>
        public Dictionary<int, R_SDReaderStatistics> GetSDReaderStatistics()
        {
           
            var sv = DataClientFactory.CreateFishLineSmartDotsClient();

            try
            {
                sv.SupplyCredentials();

                var lst = (sv as BabelfiskService.ISmartDots).GetSDReaderStatistics();

                sv.Close();

                return lst == null ? new Dictionary<int, R_SDReaderStatistics>() : lst.DistinctBy(x => x.R_SDReaderId).ToDictionary(x => x.R_SDReaderId);
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return new Dictionary<int, R_SDReaderStatistics>();
        }

    }
}
