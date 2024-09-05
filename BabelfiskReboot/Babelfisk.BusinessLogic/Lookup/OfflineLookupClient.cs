using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Babelfisk.Entities;
using System.Linq.Expressions;

namespace Babelfisk.BusinessLogic.Lookup
{
    internal class OfflineLookupClient : IDataClient, BabelfiskService.ILookup
    {
        #region IDataClient

        public void Abort()
        {
           
        }

        public void Close()
        {
        
        }

        public IDataClient SupplyCredentials()
        {
            return this;
        }

        #endregion 



        #region ILookup interface


        public byte[] GetLookups(string strEntityType, string[] includes)
        {
            //This method is not needed, Lookupmanager handles this internally.
            throw new NotImplementedException();
        }


        /// <summary>
        /// Save local lookups. This only works for a set of lookups.
        /// </summary>
        /// <param name="lstLookups"></param>
        /// <returns></returns>
        public Entities.DatabaseOperationResult SaveLookups(ref object[] lstLookups)
        {
            var res =  Entities.DatabaseOperationResult.CreateSuccessResult();

            if(lstLookups == null || lstLookups.Length == 0)
                return res;

            var strLookupType = lstLookups.First().GetType().Name;

            LookupDataVersioning ldv = new LookupDataVersioning();
            var lstExistingLookups = ldv.GetLocalLookups(strLookupType);

            for(int i = 0; i < lstLookups.Length; i++)
            {
                var lookup = lstLookups[i] as OfflineEntity;
                switch (strLookupType)
                {
                    case "DFUPerson":
                        var dfuPerson = lookup as DFUPerson;
                        ApplyOfflineLookup(ref dfuPerson, ref lstExistingLookups, () => dfuPerson.dfuPersonId);
                        break;

                    case "Person":
                         var person = lookup as Person;
                         ApplyOfflineLookup(ref person, ref lstExistingLookups, () => person.personId);
                        break;

                    case "L_Species":
                        var species = lookup as L_Species;
                        ApplyOfflineLookup(ref species, ref lstExistingLookups, () => species.L_speciesId);
                        break;

                    case "L_Platform":
                        var platform = lookup as L_Platform;
                        ApplyOfflineLookup(ref platform, ref lstExistingLookups, () => platform.L_platformId);
                        break;

                    case "L_GearType":
                        var geartype = lookup as L_GearType;
                        ApplyOfflineLookup(ref geartype, ref lstExistingLookups, () => geartype.L_gearTypeId);
                        break;

                    default:
                        return res;
                }
            }

            //Save offline dictionary
            Offline.OfflineDictionary.Instance.Save();

            ldv.SetLocalLookups(strLookupType, lstExistingLookups);
        
            return Entities.DatabaseOperationResult.CreateSuccessResult();
        }


        private void ApplyOfflineLookup<T>(ref T lookup, ref List<ILookupEntity> lstExistingLookups, Expression<Func<int>> lookupId) where T : OfflineEntity, ILookupEntity, new()
        {
            IObjectWithChangeTracker iLookup = lookup as IObjectWithChangeTracker;
            T dLookupNewEdit;
            string strIdProperty = lookupId.ExtractPropertyName();
            int objId = (int)lookup.GetNavigationProperty(strIdProperty);

            dLookupNewEdit = lstExistingLookups.OfType<T>().Where(x => x.GetNavigationProperty(strIdProperty).Equals(objId)).FirstOrDefault();

            if (iLookup.ChangeTracker.State == ObjectState.Added || dLookupNewEdit == null)
            {
                dLookupNewEdit = new T();
                lstExistingLookups.Add(dLookupNewEdit as ILookupEntity);
                int intNewId = Offline.OfflineDictionary.Instance.CreateNewId();
                dLookupNewEdit.AssignNavigationPropertyWithoutChanges(strIdProperty, intNewId);
                objId = intNewId;

                //Update id of referenced entity
                lookup.AssignNavigationPropertyWithoutChanges(strIdProperty, intNewId);
            }

            //Handle deleted entity
            if (iLookup.ChangeTracker.State == ObjectState.Deleted && dLookupNewEdit != null)
            {
                lstExistingLookups.Remove(dLookupNewEdit as ILookupEntity);
                Offline.OfflineDictionary.Instance.RemoveChangedLookup(objId);
            }
            else
            {
                lookup.CopyEntityValueTypesTo(dLookupNewEdit, strIdProperty);
                UpdateEntityOfflineState(dLookupNewEdit);
                Offline.OfflineDictionary.Instance.SetLookupChanged(dLookupNewEdit, objId);
            }

            iLookup.AcceptChanges();
            (dLookupNewEdit as IObjectWithChangeTracker).AcceptChanges();
        }


        private void UpdateEntityOfflineState(OfflineEntity oe)
        {
            var ioe = oe as IObjectWithChangeTracker;

            //If the state is already set, don't do anything
            if (oe.OfflineState != ObjectState.Unchanged && oe.OfflineState != 0)
                return;

            //If no changes are recorded, do nothing.
            if (ioe.ChangeTracker.State == ObjectState.Unchanged)
                return;

            oe.OfflineState = ioe.ChangeTracker.State;
        }


        public Entities.Record[] GetDependencies(object objLookup)
        {
           return new Entities.Record[] {};
        }

        public Entities.Sprattus.L_DFUArea[] GetRectangleAreas()
        {
            var lm = new LookupManager();

            var lstAreas = lm.GetLookups(typeof(L_DFUArea)).OfType<L_DFUArea>();
            var lstICES_DFU_Rel = lm.GetLookups(typeof(ICES_DFU_Relation_FF)).OfType<ICES_DFU_Relation_FF>();

            var arr = (from a in lstICES_DFU_Rel.Select(x => x.Area_20_21Upper.ToUpper())
                       join area in lstAreas on a equals area.DFUArea.ToUpper()
                       select area).Distinct().ToArray();

            return arr;
        }

        public Entities.DataVersioning GetLookupVersions()
        {
            return Entities.DataVersioning.CreateEmpty();
        }

        #endregion


    }
}
