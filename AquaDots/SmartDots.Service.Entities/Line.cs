using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.Entities
{
    [DataContract]
    public class Line
    {
        [DataMember(IsRequired = false)]
        public Guid Id { get; set; }

        [DataMember(IsRequired = false)]
        public Guid AnnotationId { get; set; }

        [DataMember(IsRequired = false)]
        public int X1 { get; set; }

        [DataMember(IsRequired = false)]
        public int Y1 { get; set; }

        [DataMember(IsRequired = false)]
        public int X2 { get; set; }

        [DataMember(IsRequired = false)]
        public int Y2 { get; set; }

        [DataMember(IsRequired = false)]
        public int Width { get; set; }

        [DataMember(IsRequired = false)]
        public string Color { get; set; }

        [DataMember(IsRequired = false)]
        public int LineIndex { get; set; }
    }
}
