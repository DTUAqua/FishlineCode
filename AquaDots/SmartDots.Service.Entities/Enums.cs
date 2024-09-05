using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.Entities
{
    [DataContract]
    public enum AuthenticationMethod 
    { 
        [EnumMember]
        Windows,
        [EnumMember]
        Basic 
    }
}
