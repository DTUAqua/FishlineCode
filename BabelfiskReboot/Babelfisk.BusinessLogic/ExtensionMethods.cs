using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.ServiceModel.Security;
using System.Linq.Expressions;
using System.Reflection;
using System.IO;
using System.Data.Objects;
using Anchor.Core;
using System.Collections;
using Babelfisk.Entities;

namespace Babelfisk.BusinessLogic
{
    public static class ExtensionMethods
    {
        static readonly IList<char> InvalidFileNameChars = Path.GetInvalidFileNameChars();

      

        public static BabelfiskService.OfflineClient SupplyCredentials(this BabelfiskService.OfflineClient srv)
        {
            srv.ClientCredentials.UserName.UserName = SecurityManager.CurrentUser.UserName;
            srv.ClientCredentials.UserName.Password = SecurityManager.CurrentUser.Password.ToString();
            srv.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            srv.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);

            return srv;
        }


      


        /// <summary>
        /// Waits for the task to complete, unwrapping any exceptions.
        /// </summary>
        /// <param name="task">The task. May not be <c>null</c>.</param>
        public static void WaitAndUnwrapException(this Task task)
        {
            Contract.Requires(task != null);
            Contract.Ensures(task.IsCompleted);
            Contract.EnsuresOnThrow<Exception>(task.IsCompleted);

            try
            {
                task.Wait();
                Contract.Assume(task.IsCompleted);
            }
            catch (AggregateException ex)
            {
                Exception e = ex.InnerException;

                throw e ?? ex;
            }
        }


        public static bool IsHVN(this string strTripType)
        {
            if (strTripType == null)
                return false;

           return  strTripType.ToLower().Equals("hvn");
        }

        public static bool IsVID(this string strTripType)
        {
            if (strTripType == null)
                return false;

            return strTripType.ToLower().Equals("vid");
        }

        public static bool IsSEA(this string strTripType)
        {
            if (strTripType == null)
                return false;

            return strTripType.ToLower().Equals("søs");
        }

        public static bool IsREK(this string strTripType)
        {
            if (strTripType == null)
                return false;

            return strTripType.ToLower().StartsWith("rek");
        }

        public static bool IsREKHVN(this string strTripType)
        {
            if (strTripType == null)
                return false;

            return strTripType.ToLower().Equals("rekhvn");
        }

        public static bool IsREKOMR(this string strTripType)
        {
            if (strTripType == null)
                return false;

            return strTripType.ToLower().Equals("rekomr");
        }

        public static bool IsREKTBD(this string strTripType)
        {
            if (strTripType == null)
                return false;

            return strTripType.ToLower().Equals("rektbd");
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static string ExtractPropertyName<T>(this Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("NotMemberAccessExpression", "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("Member is not a property", "propertyExpression");
            }

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
            {
                throw new ArgumentException("Static properties are not supported.", "propertyExpression");
            }

            return memberExpression.Member.Name;
        }


        public static string ToSafeFileName(this string str)
        {
            var validFilename = new string(str.Select(ch => InvalidFileNameChars.Contains(ch) ? Convert.ToChar(InvalidFileNameChars.IndexOf(ch) + 65) : ch).ToArray());
            return validFilename;
        }



        public static void OfflineInclude(this object obj, params string[] arrInclude)
        {
            if (arrInclude == null || arrInclude.Length == 0)
                return;

            Type objType = obj.GetType();

            LookupDataVersioning ldv = new LookupDataVersioning();
            var lookupMan = new LookupManager();

            foreach (string strInclude in arrInclude)
            {
                if (!strInclude.StartsWith("L_"))
                    continue;

                string strCodeColumn = strInclude.Replace("L_", "");

                var navProp = objType.GetField("_" + strInclude, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                var prop = objType.GetField(strCodeColumn.OfflineMapIncludeField(), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

                if (navProp == null)
                    throw new ApplicationException(String.Format("Property '{0}' does not exist on object '{0}'.", strInclude, objType.Name));

                if (prop == null)
                    throw new ApplicationException(String.Format("Property '{0}' does not exist on object '{0}'.", strCodeColumn.OfflineMapIncludeField(), objType.Name));

                var lst = lookupMan.GetLookups(navProp.FieldType, ldv);

                var objId = prop.GetValue(obj);

                //ignore navigation property if there is none assigned
                if (objId == null)
                    continue; 

                string strId = objId.ToString();
                var val = lst.Where(x => x.Id == strId).FirstOrDefault();

                if (val == null)
                    throw new ApplicationException(String.Format("Property '{0}' was null for object '{0}'.", strCodeColumn, objType.Name));

                navProp.SetValue(obj, val);
            }
        }



        private static string OfflineMapIncludeField(this string strCodeColumn)
        {
            switch (strCodeColumn)
            {
                case "Species":
                    return "_speciesCode";

                default:
                    return "_" + strCodeColumn;
            }
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
        /// Clone an object without cloning a specific property with name strProperty
        /// </summary>
        public static T OmitClone<T>(this T obj, string strProperty) 
        {
            lock (obj)
            {
                object objOldPropertyValue = null;

                if (strProperty != null)
                {
                    //Get old property value and clear it.
                    objOldPropertyValue = obj.GetNavigationProperty(strProperty);
                    obj.AssignNavigationPropertyWithoutChanges(strProperty, null);
                }

                //Clone object
                var st = obj.Clone();

                //Reassign old proprty value
                if (strProperty != null && objOldPropertyValue != null)
                {
                    obj.AssignNavigationPropertyWithoutChanges(strProperty, objOldPropertyValue);
                    st.AssignNavigationPropertyWithoutChanges(strProperty, objOldPropertyValue);
                }

                return st;
            }
        }


        public static string DumpObjectGraph(object o, string name = "", int depth = 3)
        {
            try
            {
                var leafprefix = (string.IsNullOrWhiteSpace(name) ? name : name + " = ");

                if (null == o) return leafprefix + "null";

                var t = o.GetType();
                if (depth-- < 1 || t == typeof(string) || t.IsValueType)
                    return leafprefix + o;

                var sb = new StringBuilder();
                var enumerable = o as IEnumerable;
                if (enumerable != null)
                {
                    name = (name ?? "").TrimEnd('[', ']') + '[';
                    var elements = enumerable.Cast<object>().Select(e => DumpObjectGraph(e, "", depth)).ToList();
                    var arrayInOneLine = elements.Count + "] = {" + string.Join(",", elements) + '}';
                    if (!arrayInOneLine.Contains(Environment.NewLine)) // Single line?
                        return name + arrayInOneLine;
                    var i = 0;
                    foreach (var element in elements)
                    {
                        var lineheader = name + i++ + ']';
                        sb.Append(lineheader).AppendLine(element.Replace(Environment.NewLine, Environment.NewLine + lineheader));
                    }
                    return sb.ToString();
                }
                foreach (var f in t.GetFields())
                    sb.AppendLine(DumpObjectGraph(f.GetValue(o), name + '.' + f.Name, depth));
                foreach (var p in t.GetProperties())
                    sb.AppendLine(DumpObjectGraph(p.GetValue(o, null), name + '.' + p.Name, depth));
                if (sb.Length == 0) return leafprefix + o;
                return sb.ToString().TrimEnd();
            }
            catch
            {
                return name + "???";
            }
        }
    }
}
