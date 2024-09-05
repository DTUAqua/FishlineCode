using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.Service
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface ISmartDots
    {
        [OperationContract]
        DatabaseOperationResult SaveSDEvent(ref SDEvent evt);

        [OperationContract]
        byte[] GetSDEventsCompressed();

        [OperationContract]
        List<int> GetCruiseYears();

        [OperationContract]
        byte[] GetSelectionAnimalsCompressed(string speciesCode, int[] years, string[] areas, bool includeAnimalJpgFiles, string[] relativeFolderPaths);

        [OperationContract]
        byte[] GetSelectionAnimalsByIdCompressed(int[] animalIds, bool includeAnimalJpgFiles, string[] relativeFolderPaths);

        [OperationContract]
        byte[] GetSDSamplesCompressed(int eventId);

        [OperationContract]
        byte[] GetSDSamplesWithIncludesCompressed(int eventId, string[] include);

        [OperationContract]
        DatabaseOperationResult MoveOrCopySamplesToEvent(List<int> sampleIds, int toEventId, bool copy);

        [OperationContract]
        ServiceResult GetFileInformationFromAnimalIds(List<string> animalIds, string[] relativeFolderPaths);

        [OperationContract]
        ServiceResult GetFileInformationFromFileNames(List<string> fileNames, string[] relativeFolderPaths);

        [OperationContract]
        ServiceResult GetFileBytes(string relativeImagePath);

        [OperationContract]
        ServiceResult GetAllFilesPaths(string[] relativeFolderPaths);

        [OperationContract]
        ServiceResult GetFolderContentCompressed(string folderRelativePath, FileSystemType type);



        [OperationContract]
        DatabaseOperationResult SaveSDSamples(SDSample[] samples);

      

        [OperationContract]
        DatabaseOperationResult CopyAgesToFishLine(List<SDAnimalAgeItem> animalIdsAndAges);

        [OperationContract]
        SDAnnotation GetSDAnnotation(int sdAnnotationId);

        [OperationContract]
        List<R_SDReaderStatistics> GetSDReaderStatistics();
    }

}
