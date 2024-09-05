using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.Entities
{
    [DataContract]
    public struct SyncDatabaseOperationResult
    {
        private DatabaseOperationResult _datRes;

        private IObjectWithChangeTracker _entity;

        [DataMember]
        public DatabaseOperationResult DatabaseOperationResult
        {
            get { return _datRes; }
            set { _datRes = value; }
        }

        [DataMember]
        public IObjectWithChangeTracker ExistingEntity
        {
            get { return _entity; }
            set { _entity = value; }
        }


        public SyncDatabaseOperationResult(DatabaseOperationResult datRes, IObjectWithChangeTracker entity = null)
        {
            _datRes = datRes;
            _entity = entity;
        }


        public static SyncDatabaseOperationResult CreateSuccessResult()
        {
            return new SyncDatabaseOperationResult(new DatabaseOperationResult(DatabaseOperationStatus.Successful));
        }
    }
}
