using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels
{
    /// <summary>
    /// Class used for defining a constructor parameter (the type and the value of the parameter). This class
    /// can be used in the process of initializing a class using reflection (obtaining a class constructor
    /// with some specific constructor signature).
    /// </summary>
    public class ConstructorParam
    {
        /// <summary>
        /// Return constructor parameter type
        /// </summary>
        public Type Type;

        /// <summary>
        /// Return constructor parameter value
        /// </summary>
        public Object Value;



        /// <summary>
        /// Force a private constructor so the static Create method is the only option of obtaining a 
        /// class instance.
        /// </summary>
        private ConstructorParam() { }


        /// <summary>
        /// Create a ConstructorParam object from an arbitrary object. The returned class contains
        /// the parameter value and its type.
        /// </summary>
        public static ConstructorParam Create<T>(T value)
        {
            return new ConstructorParam() { Type = typeof(T), Value = value };
        }
    }
}
