using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities;
using Anchor.Core;
using Babelfisk.Entities.SprattusSecurity;
using Babelfisk.Entities.Sprattus;

namespace FishLineMeasure.BusinessLogic
{
    public class LookupManager
    {

        public List<ILookupEntity> GetLookups(Type t, LookupDataVersioning ldv = null, params string[] includes) //where T : class
        {
            List<ILookupEntity> lst = new List<ILookupEntity>();

            if (ldv == null)
                ldv = new LookupDataVersioning();

           /* if (ldv.IsLocalLookupsExpired(t.Name))
            {
                var ser = new ServiceLookupClient();
                try
                {
                    ser.SupplyCredentials();
                    var arr = ser.GetLookups(t.AssemblyQualifiedName, includes);
                    ser.Close();

                    if (arr != null)
                    {
                        arr = arr.Decompress();
                        lst = arr.ToObjectDataContract<List<ILookupEntity>>(new Type[] { t });
                    }
                }
                catch (Exception e)
                {
                    ser.Abort();
                    Anchor.Core.Loggers.Logger.LogError(e);
                    throw e;
                }

                if (lst != null)
                    ldv.SetLocalLookups(t.Name, lst);
            }
            else */
            {
                lst = ldv.GetLocalLookups(t.Name, includes);
            }

            return lst;
        }


        public void SaveLookupsToDisk(Type t, LookupDataVersioning ldv = null, params string[] includes) //where T : class
        {
            List<ILookupEntity> lst = new List<ILookupEntity>();

            if (ldv == null)
                ldv = new LookupDataVersioning();

            if (ldv.IsLocalLookupsExpired(t.Name))
            {
                var ser = new ServiceLookupClient();
                try
                {
                    ser.SupplyCredentials();
                    var arr = ser.GetLookups(t.AssemblyQualifiedName, includes);
                    ser.Close();

                    if (arr != null)
                    {
                        arr = arr.Decompress();
                        lst = arr.ToObjectDataContract<List<ILookupEntity>>(new Type[] { t });
                    }
                }
                catch (Exception e)
                {
                    ser.Abort();
                    throw e;
                }

                if (lst != null)
                    ldv.SetLocalLookups(t.Name, lst);
            }
        }



        /// <summary>
        /// Get a list of dependencies for the supplied lookup.
        /// </summary>
        public List<Record> GetDependencies(ILookupEntity lookup)
        {
            var srv = new ServiceLookupClient();
            srv.SupplyCredentials();

            try
            {
                var res = (srv as FishLineService.ILookup).GetDependencies(lookup);

                srv.Close();
                return res.ToList();
            }
            catch (Exception e)
            {
                srv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }

    }
}
