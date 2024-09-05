using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.TestConsole
{
    public static class ExtensionMethods
    {
        public async static Task<T> WithTimeout<T>(this Task<T> task, int millisecondsDuration)
        {
            var retTask = await Task.WhenAny(task, Task.Delay(millisecondsDuration)).ConfigureAwait(false);

            if (retTask is Task<T> && !retTask.IsFaulted)
                return task.Result;

            return default(T);
        }

        public static T ToObjectDataContract<T>(this byte[] arr, IEnumerable<Type> knownTypes)
        {
            var dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(T), knownTypes);
            var ms = new MemoryStream(arr);
            ms.Position = 0;
            object o = dcs.ReadObject(ms);
            ms.Dispose();

            var t = (T)o;
            return t;
        }


        /// <summary>
        /// Serialize an object of type T to a byte[] (using a DataContractSerializer)
        /// </summary>
        public static byte[] ToByteArrayDataContract<T>(this T source, IEnumerable<Type> knownTypes)
        {
            var dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(T), knownTypes);
            byte[] arr = null;
            using (var ms = new System.IO.MemoryStream())
            {
                dcs.WriteObject(ms, source);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                arr = ms.ToArray();
            }

            return arr;
        }

    }
}
