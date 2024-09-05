using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities
{
    [DataContract(IsReference = true)]
    public class OfflineSelectionRecord
    {
        [DataMember]
        public int CruiseId { get; set; }

        [DataMember]
        public int? TripId { get; set; }

        [DataMember]
        public int Year { get; set; }

        [DataMember]
        public string CruiseName { get; set; }

        [DataMember]
        public string TripName { get; set; }

        [DataMember]
        public int? SampleId { get; set; }

        [DataMember]
        public string SampleName { get; set; }
    }
}
