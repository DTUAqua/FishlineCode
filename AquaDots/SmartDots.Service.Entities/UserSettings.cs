using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.Entities
{
    [DataContract]
    public class UserSettings
    {
        [DataMember(IsRequired = false)]
        public bool CanAttachDetachSample { get; set; }

        [DataMember(IsRequired = false)]
        public bool CanBrowseFolder { get; set; }

        [DataMember(IsRequired = false)]
        public bool UseSampleStatus { get; set; }

        [DataMember(IsRequired = false)]
        public bool CanApproveAnnotation { get; set; }

        [DataMember(IsRequired = false)]
        public bool RequireAqForApproval { get; set; }

        [DataMember(IsRequired = false)]
        public bool RequireAq1ForApproval { get; set; }

        [DataMember(IsRequired = false)]
        public bool RequireParamForApproval { get; set; }

        [DataMember(IsRequired = false)]
        public bool AutoMeasureScale { get; set; }

        [DataMember(IsRequired = false)]
        public bool ScanFolder { get; set; }

        [DataMember(IsRequired = false)]
        public bool AnnotateWithoutSample { get; set; }

        [DataMember(IsRequired = false)]
        public bool OpenSocket { get; set; }

        [DataMember(IsRequired = false)]
        public string EventAlias { get; set; }

        [DataMember(IsRequired = false)]
        public string SampleAlias { get; set; }

        [DataMember(IsRequired = false)]
        public bool CanMarkEventAsCompleted { get; set; }

        [DataMember(IsRequired = false)]
        public bool AllowMultipleApprovements { get; set; }

        [DataMember(IsRequired = false)]
        public bool IgnoreMultiUserColor { get; set; }

        [DataMember(IsRequired = false)]
        public float MinRequiredVersion { get; set; }
    }
}
