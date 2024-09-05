using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.BusinessLogic.Offline
{
    [Serializable]
    public class OfflineDictionaryItem
    {
        private int _intId;
        private string _strName;
        private ObjectState _entityState;
        private Type _entityType;

        public int Id
        {
            get { return _intId; }
            set { _intId = value; }
        }

        public string Name
        {
            get { return _strName; }
            set { _strName = value; }
        }

        public ObjectState EntityState
        {
            get { return _entityState; }
            set { _entityState = value; }
        }

        public Type EntityType
        {
            get { return _entityType; }
            set { _entityType = value; }
        }


        public static OfflineDictionaryItem Create(int intId, string strName, ObjectState objState, Type entityType)
        {
            var odi = new OfflineDictionaryItem();
            odi.Id = intId;
            odi.Name = strName;
            odi.EntityState = objState;
            odi.EntityType = entityType;

            return odi;
        }

    }
}
