using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities
{
    [DataContract]
    public class R_SDReaderStatistics
    {
        [DataMember]
        public int R_SDReaderId
        {
            get;
            set;
        }

        [DataMember]
        public int NumberOfReadings
        {
            get;
            set;
        }

    }
}
