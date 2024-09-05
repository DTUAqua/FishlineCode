

using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;

namespace SmartDots.Service.Entities
{
    [DataContract]
    [KnownType(typeof(User))]
    [KnownType(typeof(UserAuthentication))]
    [KnownType(typeof(UserSettings))]
    [KnownType(typeof(ReadabilityQuality))]
    [KnownType(typeof(List<ReadabilityQuality>))]
    [KnownType(typeof(Analysis))]
    [KnownType(typeof(File))]
    [KnownType(typeof(List<File>))]
    public class WebApiResult
    {
        [DataMember(IsRequired=false)]
        public dynamic Result { get; set; }

        [DataMember(IsRequired=false)]
        public string ErrorMessage { get; set; }
    }
}
