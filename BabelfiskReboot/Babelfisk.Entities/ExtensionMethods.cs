using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using System.IO;

namespace Babelfisk.Entities
{
    public static class ExtensionMethods
    {
        public static void AssignNavigationPropertyWithoutChanges(this object obj, string strPropertyName, object objValue)
        {
            Type objType = obj.GetType();
            var navProp = objType.GetField("_" + strPropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

            if (navProp == null)
                throw new ApplicationException(String.Format("Property '{0}' does not exist on object '{0}'.", strPropertyName, objType.Name));

            navProp.SetValue(obj, objValue);
        }


        public static object GetNavigationProperty(this object obj, string strPropertyName)
        {
            Type objType = obj.GetType();
            var navProp = objType.GetField("_" + strPropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

            if (navProp == null)
                throw new ApplicationException(String.Format("Property '{0}' does not exist on object '{0}'.", strPropertyName, objType.Name));

            return navProp.GetValue(obj);
        }

        /// <summary>
        /// Handle and reset negative offline ids.
        /// </summary>
        public static void HandleOfflineId<T>(ref T entity, Expression<Func<int>> idProperty) where T : OfflineEntity, IObjectWithChangeTracker
        {
            string strEntityId = idProperty.ExtractPropertyName();

            int intOfflineId = (int)entity.GetNavigationProperty(strEntityId);

            //Reset offline id if it has been given a temporary id when offline.
            if (intOfflineId < 0)
            {
                entity.AssignNavigationPropertyWithoutChanges(strEntityId, 0);
                entity.OfflineId = intOfflineId;
            }

            //Assign correct changetracker state (so normal add methods can be used)
            if (entity.ChangeTracker.State == ObjectState.Unchanged && entity.ChangeTracker.State != entity.OfflineState)
                entity.ChangeTracker.State = entity.OfflineState;
        }


        public static byte[] ToByteArrayProtoBuf<T>(this T source)
        {
            byte[] arr = null;
            using (var ms = new System.IO.MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, source);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                arr = ms.ToArray();
            }

            return arr;
        }


        public static T ToObjectProtoBuf<T>(this byte[] arr)
        {
            var ms = new MemoryStream(arr);
            T o;
            try
            {
                ms.Position = 0;
                o = ProtoBuf.Serializer.Deserialize<T>(ms);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (ms != null)
                    ms.Dispose();
            }

            return o;

        }
    }
}
