using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities;
using Anchor.Core;
using Babelfisk.Entities.SprattusSecurity;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.BusinessLogic
{
    public class LookupManager
    {

        public void UpdateLookup(Type t, LookupDataVersioning ldv, params string[] includes)
        {
            List<ILookupEntity> lst = new List<ILookupEntity>();
            var ser = new Babelfisk.BusinessLogic.Lookup.ServiceLookupClient();
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


        public List<ILookupEntity> GetLookups(Type t, LookupDataVersioning ldv = null, params string[] includes) //where T : class
        {
            List<ILookupEntity> lst = new List<ILookupEntity>();

            if (ldv == null)
                ldv = new LookupDataVersioning();

            if (!Settings.Settings.Instance.OfflineStatus.IsOffline && ldv.IsLocalLookupsExpired(t.Name))
            {
                var ser = new Babelfisk.BusinessLogic.Lookup.ServiceLookupClient();
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
            else
            {
                lst = ldv.GetLocalLookups(t.Name, includes);
            }

            return lst;
        }


        public DatabaseOperationResult SaveLookups(ref List<ILookupEntity> lstLookups)
        {
            var srv = DataClientFactory.CreateLookupClient();
            srv.SupplyCredentials();

            try
            {
                var arrLookups = lstLookups.ToArray<object>();
                var res = (srv as BabelfiskService.ILookup).SaveLookups(ref arrLookups);

                srv.Close();

                lstLookups = arrLookups.OfType<ILookupEntity>().ToList();
                return res;
            }
            catch (Exception e)
            {
                srv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Get a list of dependencies for the supplied lookup.
        /// </summary>
        public List<Record> GetDependencies(ILookupEntity lookup)
        {
            var srv = DataClientFactory.CreateLookupClient();
            srv.SupplyCredentials();

            try
            {
                var res = (srv as BabelfiskService.ILookup).GetDependencies(lookup);

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


        /// <summary>
        /// Get a list of areas from assigned statistical rectangles.
        /// </summary>
        public List<L_DFUArea> GetRectangleAreas()
        {
            var srv = DataClientFactory.CreateLookupClient();
            srv.SupplyCredentials();

            try
            {
                var res = (srv as BabelfiskService.ILookup).GetRectangleAreas();

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
