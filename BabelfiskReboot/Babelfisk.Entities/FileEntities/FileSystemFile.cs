using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities.FileEntities
{
    [DataContract]
    [ProtoBuf.ProtoContract]
    public class FileSystemFile : FileSystemItem
    {
        public override FileSystemType FileSystemType
        {
            get { return FileSystemType.File; }
        }

        public FileSystemFile() : base() { }

        public FileSystemFile(string path)
          : base(path)
        {

        }


        protected override string GetNameFromPath(string path)
        {
            if (path == null)
                return null;

            return System.IO.Path.GetFileName(path);
        }
    }
}
