using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.Entities
{
    [DataContract]
    [KnownType(typeof(Folder))]
    [KnownType(typeof(AnalysisParameter))]
    [KnownType(typeof(List<AnalysisParameter>))]
    public class Analysis
    {
        [DataMember(IsRequired = false)]
        public Guid Id { get; set; }

        [DataMember(IsRequired = false)]
        public int Number { get; set; }

        [DataMember(IsRequired = false)]
        public Folder Folder { get; set; }

        [DataMember(IsRequired = false)]
        public List<AnalysisParameter> AnalysisParameters { get; set; }

        [DataMember(IsRequired = false)]
        public string HeaderInfo { get; set; }

        [DataMember(IsRequired = false)]
        public bool UserCanPin { get; set; }

        [DataMember(IsRequired = false)]
        public bool ShowEdgeColumn { get; set; }

        [DataMember(IsRequired = false)]
        public bool ShowNucleusColumn { get; set; }
    }
}
