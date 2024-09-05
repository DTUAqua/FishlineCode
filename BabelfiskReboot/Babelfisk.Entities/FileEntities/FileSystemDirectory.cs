using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities.FileEntities
{
    [DataContract]
    [ProtoBuf.ProtoContract]
    public class FileSystemDirectory : FileSystemItem
    {
        private bool _hasSubdirectories;

        public override FileSystemType FileSystemType
        {
            get { return FileSystemType.Directory; }
        }


        [ProtoBuf.ProtoMember(1)]
        public bool HasSubdirectories
        {
            get { return _hasSubdirectories; }
            set
            {
                _hasSubdirectories = value;
            }
        }



        public FileSystemDirectory(): base() { }

        public FileSystemDirectory(string path)
            : base(path)
        {

        }

        protected override string GetNameFromPath(string path)
        {
            if (path == null)
                return null;

            while (path.EndsWith(@"\"))
                path = path.Substring(0, path.Length - 1);

            return System.IO.Path.GetFileName(path);
        }
    }
}
