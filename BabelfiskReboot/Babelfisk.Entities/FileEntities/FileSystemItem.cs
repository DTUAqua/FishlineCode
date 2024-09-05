using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities.FileEntities
{
    [DataContract]
    [KnownType(typeof(FileSystemDirectory))]
    [KnownType(typeof(FileSystemFile))]
    [ProtoContract]
    [ProtoInclude(10, typeof(FileSystemDirectory))]
    [ProtoInclude(20, typeof(FileSystemFile))]
    public abstract class FileSystemItem
    {
        [DataMember]
        [ProtoMember(1)]
        protected string _fullPath;

        private string _name;

        public virtual FileSystemType FileSystemType
        {
            get;
        }


        public string Path
        {
            get 
            { 
                return _fullPath;
            }
        }


        public string Name
        {
            get
            {
                if (_name == null)
                    _name = GetNameFromPath(Path);

                return _name;
            }
        }


        public FileSystemItem() { }

        public FileSystemItem(string path)
        {
            _fullPath = path;
        }


        public static FileSystemItem Create(string path, FileSystemType itm)
        {
            FileSystemItem fi = null;
            switch(itm)
            {
                case FileSystemType.Directory:
                    fi = new FileSystemDirectory(path);
                    break;

                case FileSystemType.File:
                    fi = new FileSystemFile(path);
                    break;
            }

            return fi;
        }


        protected abstract string GetNameFromPath(string path);
    }
}
