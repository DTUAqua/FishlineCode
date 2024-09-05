using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.Entities
{
    [DataContract]
    public class Sample
    {
        [DataMember(IsRequired=false)]
        public Guid Id { get; set; }

        [DataMember(IsRequired = false)]
        public string StatusCode { get; set; }

        [DataMember(IsRequired = false)]
        public string StatusColor { get; set; }

        [DataMember(IsRequired = false)]
        public int StatusRank { get; set; }

        [DataMember(IsRequired = false)]
        public Dictionary<string, string> DisplayedProperties { get; set; }
    }
}
