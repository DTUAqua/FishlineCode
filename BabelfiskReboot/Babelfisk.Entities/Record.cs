using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities
{
    /// <summary>
    /// Class used within the CanDeleteLookup method.
    /// </summary>
    [DataContract(IsReference = true)]
    public class Record
    {
        [DataMember]
        public string Result { get; set; }
    }
}
