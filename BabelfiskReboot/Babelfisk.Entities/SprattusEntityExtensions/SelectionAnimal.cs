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
    public class SelectionAnimal
    {
        [DataMember] [ProtoBuf.ProtoMember(1)] public int AnimalId { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(2)] public int CruiseYear { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(3)] public string Cruise { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(4)] public string Trip { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(5)] public string TripType { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(6)] public string Station { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(7)] public DateTime? GearStartDate { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(8)] public string AreaCode { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(9)] public string StatisticalRectangle { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(10)] public string Latitude { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(11)] public string Longitude { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(12)] public string SexCode { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(13)] public int? Age { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(14)] public string EdgeStructureCode { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(15)] public string OtolithReadingRemarkCode { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(16)] public int? MaturityIndex { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(17)] public string MaturityIndexMethod { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(18)] public int? LengthMM { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(19)] public Decimal? WeightG { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(20)] public string AnimalRemark { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(21)] public string SpeciesCode { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(22)] public string SpeciesListRemark { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(23)] public string AnimalImageFileNamesString { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(24)] public string StockCode { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(25)] public int? StockId { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(26)] public string PreperationMethod { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(27)] public string LightType { get; set; }
        [DataMember] [ProtoBuf.ProtoMember(28)] public string OtolithDescription { get; set; }


        /// <summary>
        /// This should return the description of the Maturity lookup and should be assigned in memory from MaturityIndex and MaturityIndexMethod.
        /// </summary>
        public string MaturityDescription
        {
            get;
            set;
        }


        /// <summary>
        /// Get the quarter of the GearStartDate
        /// </summary>
        public int? Quarter 
        {  
            get { return GearStartDate.HasValue ? new Nullable<int>(GearStartDate.Value.GetQuarter()) : null; } 
        }


        /// <summary>
        /// Gets a list of animal images names.
        /// </summary>
        public List<string> AnimalImageFileNames
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AnimalImageFileNamesString))
                    return null;

                var arr = AnimalImageFileNamesString.Split(';');

                return arr == null ? null : arr.ToList();
            }
            set
            {
                AnimalImageFileNamesString = (value == null || value.Count == 0) ? null : string.Join(";", value);
            }
        }

        /// <summary>
        /// Whether or not the selection animal has any images.
        /// </summary>
        public bool HasAnimalImages
        {
            get { return !string.IsNullOrWhiteSpace(AnimalImageFileNamesString); }
        }
       


        public SelectionAnimal Clone()
        {
            try
            {
                byte[] arr = this.ToByteArrayProtoBuf();
                var sa = arr.ToObjectProtoBuf<SelectionAnimal>();

                return sa;
            }
            catch
            {
                return null;
            }
        }
    }
}
