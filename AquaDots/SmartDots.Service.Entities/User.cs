using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.Entities
{
    [DataContract]
    public class User
    {
        [DataMember(IsRequired=false)]
        public Guid Id { get; set; }

        [DataMember(IsRequired = false)]
        public string AccountName { get; set; }

        [DataMember(IsRequired = false)]
        public string Token { get; set; }
    }
}
