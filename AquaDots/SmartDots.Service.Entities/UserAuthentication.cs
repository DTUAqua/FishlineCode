using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.Entities
{
    [DataContract]
    public class UserAuthentication
    {
        [DataMember(IsRequired=false)]
        public string Username { get; set; }

        [DataMember(IsRequired = false)]
        public string Password { get; set; }

        [DataMember(IsRequired = false)]
        public AuthenticationMethod DtoAuthenticationMethod { get; set; }
    }
}
