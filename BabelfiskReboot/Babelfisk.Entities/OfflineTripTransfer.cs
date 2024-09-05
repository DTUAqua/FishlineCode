using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.Entities
{
    [DataContract(IsReference = true)]
    public class OfflineTripTransfer
    {
        [DataMember]
        public Cruise Cruise { get; set; }

        [DataMember]
        public Trip Trip { get; set; }

        [DataMember]
        public List<Sample> Samples { get; set; }
    }
}
