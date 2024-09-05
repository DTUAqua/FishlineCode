using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.BusinessLogic
{
    public interface IDataClient
    {
        void Abort();

        void Close();

        IDataClient SupplyCredentials();
    }
}
