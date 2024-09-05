using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.Entities
{
    [DataContract]
    public class Folder
    {
        [DataMember(IsRequired=false)]
        public Guid Id { get; set; }

        [DataMember(IsRequired = false)]
        public string Path { get; set; }
    }
}
