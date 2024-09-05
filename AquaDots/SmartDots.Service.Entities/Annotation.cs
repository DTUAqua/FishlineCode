using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.Entities
{
    [DataContract]
    [KnownType(typeof(Dot))]
    [KnownType(typeof(List<Dot>))]
    [KnownType(typeof(Line))]
    [KnownType(typeof(List<Line>))]
    public class Annotation
    {
        [DataMember(IsRequired = false)]
        public Guid Id { get; set; }

        [DataMember(IsRequired = false)]
        public Guid? ParameterId { get; set; }

        [DataMember(IsRequired = false)]
        public Guid FileId { get; set; }

        [DataMember(IsRequired = false)]
        public Guid? QualityId { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime DateCreation { get; set; }

        [DataMember(IsRequired = false)]
        public Guid? LabTechnicianId { get; set; }

        [DataMember(IsRequired = false)]
        public string LabTechnician { get; set; }

        [DataMember(IsRequired = false)]
        public int Result { get; set; }

        [DataMember(IsRequired = false)]
        public bool IsApproved { get; set; }

        [DataMember(IsRequired = false)]
        public bool IsReadOnly { get; set; }

        [DataMember(IsRequired = false)]
        public bool IsFixed { get; set; }

        [DataMember(IsRequired = false)]
        public string Comment { get; set; }

        [DataMember(IsRequired = false)]
        public List<Dot> Dots { get; set; }

        [DataMember(IsRequired = false)]
        public List<Line> Lines { get; set; }

        [DataMember(IsRequired = false)]
        // New on this API version
        public string Edge { get; set; }

        [DataMember(IsRequired = false)]
        public string Nucleus { get; set; }
    }
}
