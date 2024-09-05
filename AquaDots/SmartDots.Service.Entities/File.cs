using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.Entities
{
    [DataContract]
    [KnownType(typeof(Annotation))]
    [KnownType(typeof(List<Annotation>))]
    [KnownType(typeof(Sample))]
    [KnownType(typeof(Dictionary<string, string>))]
    public class File
    {
        [DataMember(IsRequired = false)]
        public Guid ID { get; set; }

        [DataMember(IsRequired = false)]
        public string Filename { get; set; }

        [DataMember(IsRequired = false)]
        public string DisplayName { get; set; }

        [DataMember(IsRequired = false)]
        public string SampleNumber { get; set; }

        [DataMember(IsRequired = false)]
        public Guid? SampleID { get; set; }

        [DataMember(IsRequired = false)]
        public int AnnotationCount { get; set; }

        [DataMember(IsRequired = false)]
        public bool IsReadOnly { get; set; }

        [DataMember(IsRequired = false)]
        public virtual List<Annotation> Annotations { get; set; }

        [DataMember(IsRequired = false)]
        public virtual Sample Sample { get; set; }

        [DataMember(IsRequired = false)]
        public decimal? Scale { get; set; }

        [DataMember(IsRequired = false)]
        public bool? CanApprove { get; set; } // only prevents approval when false

        [DataMember(IsRequired = false)]
        public Dictionary<string, string> SampleProperties { get; set; }
    }
}
