using Babelfisk.Entities;
using Babelfisk.Entities.FileEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Babelfisk.OtolithFileService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IOtolithFileService
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        ServiceResult GetFileInformationFromAnimalIds(List<string> animalIds, string[] subFolders);

        [OperationContract]
        ServiceResult GetFileInformationFromFileNames(List<string> fileNames, string[] subFolders);

        [OperationContract]
        ServiceResult GetFileBytes(string relativeImagePath);

        [OperationContract]
        Dictionary<string, Dictionary<string, OtolithFileInformation>> GetFileInformationDictionaryFromAnimalIds(List<string> animalIds, string[] subFolders, ref string error);

        [OperationContract]
        ServiceResult GetAllFilePaths(string[] subFolders);

        [OperationContract]
        ServiceResult GetFolderContentCompressed(string subFolderPath, FileSystemType type);
    }

}
