using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.Entities
{
    [DataContract]
    public class ReadabilityQuality
    {
        [DataMember(IsRequired = false)]
        public Guid Id { get; set; }

        [DataMember(IsRequired = false)]
        public string Code { get; set; }

        [DataMember(IsRequired = false)]
        public string Description { get; set; }

        [DataMember(IsRequired = false)]
        public string Color { get; set; }
    }
}
