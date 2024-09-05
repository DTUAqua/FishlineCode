using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities
{
    [DataContract]
    public class SDAnimalAgeItem
    {
        [DataMember]
        public int AnimalId
        {
            get;
            set;
        }


        [DataMember]
        public int SDSampleId
        {
            get;
            set;
        }


        [DataMember]
        public int Age
        {
            get;
            set;
        }


        [DataMember]
        public bool ShouldAssignAge
        {
            get;
            set;
        }


        [DataMember]
        public int? SDAnnotationId
        {
            get;
            set;
        }

        [DataMember]
        public int? OtolithReadingRemarkId
        {
            get;
            set;
        }

        [DataMember]
        public string EdgeStructureCode
        {
            get;
            set;
        }

        [DataMember]
        public int? DFUPersonReaderId
        {
            get;
            set;
        }


        /*[DataMember]
        public string DFUPersonReaderInitials
        {
            get;
            set;
        }*/
    }
}
