using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using Anchor.Core;

namespace FishLineMeasure.BusinessLogic.Settings
{
    [DataContract(Name="SettingsBaseObject", IsReference=true)]
    [Serializable]
    public class SettingsBaseObject<T> : INotifyPropertyChanged where T : new()
    {
        internal static T Deserialize(string str)
        {
            T os = new T();

            if (str == null)
                return os;

            byte[] arr = Convert.FromBase64String(str);

            try
            {
                os = arr.ToObjectDataContract<T>();
            }
            catch { }

            return os;
        }


        internal static string Serialize<T>(T obj, IEnumerable<Type> knownTypes = null)
        {
            try
            {
                var types = knownTypes ?? new List<Type>();
                byte[] arr = obj.ToByteArrayDataContract(types);
                string str = Convert.ToBase64String(arr);

                return str;
            }
            catch { }

            return "";
        }


        internal string Serialize()
        {
            try
            {
                byte[] arr = this.ToByteArrayDataContract();
                string str = Convert.ToBase64String(arr);

                return str;
            }
            catch { }

            return "";
        }



        #region INotifyPropertyChanged interface


        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string strPropertyName)
        {
            if (PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(strPropertyName);
                PropertyChanged(this, e);
            }
        }


        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property that has a new value</typeparam>
        /// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Method used to raise an event")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Cannot change the signature")]
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = propertyExpression.ExtractPropertyName();
            this.RaisePropertyChanged(propertyName);
        }

        protected string PropertyNameToString<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = propertyExpression.ExtractPropertyName();
            return propertyName;
        }


        #endregion
    }
}
