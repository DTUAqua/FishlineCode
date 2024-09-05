using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.Entities
{
    [DataContract]
    public class Dot
    {
        [DataMember(IsRequired = false)]
        public Guid Id { get; set; }

        [DataMember(IsRequired = false)]
        public Guid AnnotationId { get; set; }

        [DataMember(IsRequired = false)]
        public int X { get; set; }

        [DataMember(IsRequired = false)]
        public int Y { get; set; }

        [DataMember(IsRequired = false)]
        public int Width { get; set; }

        [DataMember(IsRequired = false)]
        public int DotIndex { get; set; }

        [DataMember(IsRequired = false)]
        public string Color { get; set; }

        [DataMember(IsRequired = false)]
        public string DotShape { get; set; }

        [DataMember(IsRequired = false)]
        public string DotType { get; set; }
    }
}
