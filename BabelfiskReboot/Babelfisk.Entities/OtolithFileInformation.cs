using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities
{
    [DataContract]
    [ProtoBuf.ProtoContract]
    public class OtolithFileInformation
    {
        [DataMember]
        [ProtoBuf.ProtoMember(1)]
        /// <summary>
        /// Name of the otolith file with extension
        /// </summary>
        public string AnimalId
        {
            get;
            set;
        }


        [DataMember]
        [ProtoBuf.ProtoMember(2)]
        /// <summary>
        /// Name of the otolith file with extension
        /// </summary>
        public string FileName
        {
            get;
            set;
        }


        /// <summary>
        /// Get the extension of the image file name (.jpg).
        /// </summary>
        public string GetFileExtension
        {
            get { return string.IsNullOrWhiteSpace(FileName) ? null : System.IO.Path.GetExtension(FileName); }
        }


        /// <summary>
        /// Relative path from the server otolith images file root directory to the image file.
        /// </summary>
        [DataMember]
        [ProtoBuf.ProtoMember(3)]
        public string RelativeDirectoryPath
        {
            get;
            set;
        }


        /// <summary>
        /// List of auxiliary files that are not image files, but has the same animal id.
        /// </summary>
        [DataMember]
        [ProtoBuf.ProtoMember(4)]
        public List<OtolithFileInformation> AuxiliaryFiles
        {
            get;
            set;
        }


        public string FileNameLowerCase
        {
            get { return FileName == null ? null : FileName.ToLowerInvariant(); }
        }


        public OtolithFileInformation() { }


        public OtolithFileInformation(string animalId, string fileNameWithExtension, string relativeDirectoryPath)
        {
            AnimalId = animalId;
            FileName = fileNameWithExtension;
            RelativeDirectoryPath = relativeDirectoryPath;
        }


        public void AddAdditionalFile(OtolithFileInformation fi)
        {
            if (AuxiliaryFiles == null)
                AuxiliaryFiles = new List<OtolithFileInformation>();

            AuxiliaryFiles.Add(fi);
        }
    }
}
