using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.Entities
{
    public interface ILookupEntity : IObjectWithChangeTracker
    {
        string Id
        {
            get;
        }

        string FilterValue
        {
            get;
        }

        string DefaultSortValue
        {
            get;
        }

        string UIDisplay
        {
            get;
        }


        string GetErrors(List<ILookupEntity> lst);

        void BeforeSave();

        void NewLookupCreated();
    }
}
