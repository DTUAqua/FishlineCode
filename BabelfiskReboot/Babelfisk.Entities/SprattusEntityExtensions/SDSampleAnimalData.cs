using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Anchor.Core;

namespace Babelfisk.Entities.Sprattus
{
    [DataContract]
    [ProtoBuf.ProtoContract]
    public class SDSampleAnimalData
    {
        [DataMember] [ProtoBuf.ProtoMember(1)] public int AnimalId { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(2)] public DateTime? CatchDate { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(3)] public string AreaCode { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(4)] public string StatisticalRectangle { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(5)] public string Latitude { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(6)] public string Longitude { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(7)] public string SexCode { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(8)] public int? LengthMM { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(9)] public Decimal? WeightG { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(10)] public string MaturityIndexMethod { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(11)] public int? MaturityId { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(12)] public int? MaturityIndex { get; set; }

    }
}
