using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Babelfisk.Entities.Sprattus;
using Babelfisk.Entities;
using Babelfisk.Entities.SprattusSecurity;
using System.Data.Objects;
using System.Data;
using System.Collections.ObjectModel;

namespace Babelfisk.Service
{
    public static class ExtensionMethods
    {
        public static void ApplyOverwritingMethod<T>(this SprattusContainer db, T etEntity, OverwritingMethod enmMethod)
            where T : Babelfisk.Entities.Sprattus.IObjectWithChangeTracker
        {
            if (etEntity == null || enmMethod == OverwritingMethod.None || etEntity.ChangeTracker.State == Babelfisk.Entities.Sprattus.ObjectState.Added)
                return;

            switch (enmMethod)
            {
                case OverwritingMethod.ClientWins:
                    db.Refresh(System.Data.Objects.RefreshMode.ClientWins, etEntity);
                    break;

                case OverwritingMethod.ServerWins:
                    db.Refresh(System.Data.Objects.RefreshMode.StoreWins, etEntity);
                    break;
            }
        }


        public static void ApplyOverwritingMethod<T>(this SprattusSecurityContainer db, T etEntity, OverwritingMethod enmMethod)
           where T : Babelfisk.Entities.SprattusSecurity.IObjectWithChangeTracker
        {
            if (etEntity == null || enmMethod == OverwritingMethod.None || etEntity.ChangeTracker.State == Babelfisk.Entities.SprattusSecurity.ObjectState.Added)
                return;

            switch (enmMethod)
            {
                case OverwritingMethod.ClientWins:
                    db.Refresh(System.Data.Objects.RefreshMode.ClientWins, etEntity);
                    break;

                case OverwritingMethod.ServerWins:
                    db.Refresh(System.Data.Objects.RefreshMode.StoreWins, etEntity);
                    break;
            }
        }



        public static void SaveChangesAndHandleOptimisticConcurrency(this SprattusSecurityContainer db, OverwritingMethod enmMethod)
        {
            do
            {
                try
                {
                    db.SaveChanges();
                    break;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    if (enmMethod == OverwritingMethod.None)
                        throw ex;

                    foreach (var item in ex.StateEntries)
                    {
                        db.Refresh(enmMethod == OverwritingMethod.ClientWins ? System.Data.Objects.RefreshMode.ClientWins : System.Data.Objects.RefreshMode.StoreWins, item.Entity);
                    }
                }
            }
            while (true);
        }

        public static void SaveChangesAndHandleOptimisticConcurrency(this SprattusContainer db, OverwritingMethod enmMethod)
        {
            do
            {
                try
                {
                    db.SaveChanges();
                    break;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    if (enmMethod == OverwritingMethod.None)
                        throw ex;

                    foreach (var item in ex.StateEntries)
                    {
                        db.Refresh(enmMethod == OverwritingMethod.ClientWins ? System.Data.Objects.RefreshMode.ClientWins : System.Data.Objects.RefreshMode.StoreWins, item.Entity);
                    }
                }
            }
            while (true);
        }


        public static void ApplyTransactionIsolationLevel(this ObjectContext ctx, System.Transactions.IsolationLevel isoLevel)
        {
            //Set default transaction level
            string str = "SERIALIZABLE";

            switch (isoLevel)
            {
                case System.Transactions.IsolationLevel.ReadUncommitted:
                    str = "READ UNCOMMITTED";
                    break;

                case System.Transactions.IsolationLevel.ReadCommitted:
                    str = "READ COMMITTED";
                    break;

                case System.Transactions.IsolationLevel.Serializable:
                    str = "SERIALIZABLE";
                    break;

                case System.Transactions.IsolationLevel.RepeatableRead:
                    str = "REPEATABLE READ";
                    break;

                case System.Transactions.IsolationLevel.Snapshot:
                    str = "SNAPSHOT";
                    break;
            }

            str = "SET TRANSACTION ISOLATION LEVEL " + str;

            ctx.ExecuteStoreCommand(str);
        }


        /// <summary>
        /// A generic method for including a many navigation properties in one call.
        /// </summary>
        public static ObjectQuery<T> Include<T>(this ObjectSet<T> source, string[] paths) where T : class
        {
            return (source as ObjectQuery<T>).Include<T>(paths);
        }

        /// <summary>
        /// A generic method for including a many navigation properties in one call.
        /// </summary>
        public static ObjectQuery<T> Include<T>(this ObjectQuery<T> source, string[] paths) where T : class
        {
            ObjectQuery<T> objs = source;

            if (paths == null && paths.Length == 0)
                return objs;

            if (paths != null && paths.Length > 0)
                for (int i = 0; i < paths.Length; i++)
                    objs = objs.Include(paths[i]);

            return objs;
        }

        /// <summary>
        /// Add a collection of items to an ObservableCollection
        /// </summary>
        public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> items)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var itm in items)
                source.Add(itm);
        }
    }
}