using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.Entities
{
    [DataContract(IsReference = true)]
    public class LavSFTransferItem
    {
        Animal _animal;
        List<Age> _lstAges;
        List<AnimalFile> _lstAnimalFiles;
        AnimalInfo _animalInfo;
        List<R_AnimalInfoReference> _lstAnimalInfoReferences;

        private int _intSubSampleId;


        [DataMember]
        public Animal Animal
        {
            get { return _animal; }
            set {  _animal = value; }
        }


        [DataMember]
        public List<Age> Ages
        {
            get { return _lstAges; }
            set { _lstAges = value; }
        }


        [DataMember]
        public AnimalInfo AnimalInfo
        {
            get { return _animalInfo; }
            set { _animalInfo = value; }
        }

        [DataMember]
        public int SubSampleId
        {
            get { return _intSubSampleId; }
            set { _intSubSampleId = value; }
        }



        [DataMember]
        public List<R_AnimalInfoReference> AnimalInfoReferences
        {
            get { return _lstAnimalInfoReferences; }
            set { _lstAnimalInfoReferences = value; }
        }


        [DataMember]
        public List<AnimalFile> AnimalFiles
        {
            get { return _lstAnimalFiles; }
            set { _lstAnimalFiles = value; }
        }


        public LavSFTransferItem()
        {
            Ages = new List<Age>();
            AnimalFiles = new List<AnimalFile>();
            AnimalInfoReferences = new List<R_AnimalInfoReference>();
        }
    }
}
