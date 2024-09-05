using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Babelfisk.BusinessLogic.Settings
{
    /// <summary>
    /// Class implements functionality for storing objects (settings) under a given name to either
    /// memory or persistant storage. If objects are stored in memory, they are reset the next time the application
    /// runs - if they are stored in persistant storage, they are saved for the next time the application runs. 
    /// </summary>
    [Serializable]
    public class SettingsContainer
    {
        /// <summary>
        /// Container identifier.
        /// </summary>
        protected  string _strIdentifier;


        /// <summary>
        /// Dictionary of memory stored objects.
        /// </summary>
        protected Dictionary<string, object> m_dicMemory;

        /// <summary>
        /// Dictionary of persitant stored objects.
        /// </summary>
        protected Dictionary<string, object> m_dicStored;


        /// <summary>
        /// Settings container identifier.
        /// </summary>
        public virtual string Identifier
        {
            get { return _strIdentifier; }
        }


        /// <summary>
        /// Construct SettingsContainer under a given identifier.
        /// </summary>
        public SettingsContainer(string strIdentifier)
        {
            _strIdentifier = strIdentifier;

            m_dicMemory = new Dictionary<string, object>();
            m_dicStored = new Dictionary<string, object>();
        }


        /// <summary>
        /// Retrieve stored parameter from either memory or persistant storage (where else it exists). If it exists
        /// both places, the memory object is returned.
        /// </summary>
        public object this[string strParameter]
        {
            get { return GetValue(strParameter); }
        }


        /// <summary>
        /// Retrieve stored parameter from either memory or persistant storage (where else it exists). If it exists
        /// both places, the memory object is returned.
        /// </summary>
        public object GetValue(string strParameter)
        {
            if (m_dicMemory.ContainsKey(strParameter))
                return m_dicMemory[strParameter];

            if (m_dicStored.ContainsKey(strParameter))
                return m_dicStored[strParameter];

            return null;
        }


        /// <summary>
        /// Assign an object to memory storage under a given parameter name.
        /// </summary>
        public void SetMemoryValue(string strParameter, object objValue)
        {
            if (m_dicMemory.ContainsKey(strParameter))
            {
                m_dicMemory[strParameter] = objValue;
                return;
            }

            if (m_dicStored.ContainsKey(strParameter))
                throw new ArgumentException(string.Format("Key '{0}' is already in use in the Stored dictionary.", strParameter));

            m_dicMemory.Add(strParameter, objValue);
        }


        /// <summary>
        /// Assign an object to persistant storage under a given parameter name.
        /// </summary>
        protected virtual void SetStoredValue(string strParameter, XElement value)
        {
            SetStoredValue(strParameter, value.Value);
        }


        /// <summary>
        /// Assign an object to persistant storage under a given parameter name.
        /// </summary>
        public virtual void SetStoredValue(string strParameter, object objValue, bool blnRemoveOnValueNull = false)
        {
            if (m_dicStored.ContainsKey(strParameter))
            {
                if (blnRemoveOnValueNull && objValue == null)
                    m_dicStored.Remove(strParameter);
                else
                    m_dicStored[strParameter] = objValue;
                return;
            }

            if (m_dicMemory.ContainsKey(strParameter))
                throw new ArgumentException(string.Format("Key '{0}' is already in use in the Memory dictionary.", strParameter));

            m_dicStored.Add(strParameter, objValue);
        }



        /// <summary>
        /// Save persistant data to an XElement for disk storage.
        /// </summary>
        public virtual XElement Save()
        {
            if (string.IsNullOrEmpty(Identifier))
                return null;

            return new XElement(_strIdentifier, from d in m_dicStored
                                                select new XElement(d.Key, d.Value));
        }


        /// <summary>
        /// Loads the settings container from a XElement (representing persistet storage).
        /// </summary>
        public virtual void Load(XElement xeSettings)
        {
            m_dicMemory = new Dictionary<string, object>();
            m_dicStored = new Dictionary<string, object>();

            if (xeSettings == null)
                return;

            foreach (var xe in xeSettings.Elements())
                SetStoredValue(xe.Name.LocalName, xe);
        }
    }
}
