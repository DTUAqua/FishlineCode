using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.Entities
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(Babelfisk.Entities.LavSFTransferItem))]
    public class OfflineEntity
    {
        private ObjectState _offlineState = ObjectState.Unchanged;

        private int? _intOfflineId;

        private OverwritingMethod? _overwritingMethod;

        private IObjectWithChangeTracker _offlineComparisonEntity;

        private List<object> _lstOfflineDeletedEntities;

        [DataMember]
        public ObjectState OfflineState
        {
            get { return _offlineState; }
            set { _offlineState = value; }
        }

        [DataMember]
        public int? OfflineId
        {
            get { return _intOfflineId; }
            set { _intOfflineId = value; }
        }

        [DataMember]
        public OverwritingMethod? OverwritingMethod
        {
            get { return _overwritingMethod; }
            set { _overwritingMethod = value; }
        }

         [DataMember]
        public IObjectWithChangeTracker OfflineComparisonEntity
        {
            get { return _offlineComparisonEntity; }
            set { _offlineComparisonEntity = value; }
        }

         [DataMember]
         public List<object> OfflineDeletedEntities
         {
             get 
             {
                 if (_lstOfflineDeletedEntities == null)
                     _lstOfflineDeletedEntities = new List<object>();

                 return _lstOfflineDeletedEntities; 
             }
             set { _lstOfflineDeletedEntities = value; }
         }


         public bool CanDelete
         {
             get
             {
                 if ((this as IObjectWithChangeTracker) != null)
                 {
                     if(((this as IObjectWithChangeTracker).ChangeTracker.State == ObjectState.Modified && _offlineState == ObjectState.Unchanged) ||
                        (this as IObjectWithChangeTracker).ChangeTracker.State == ObjectState.Unchanged && _offlineState == ObjectState.Unchanged)
                     {
                         return false;
                     }
                 }

                 return true;
             }
         }
    }
}
