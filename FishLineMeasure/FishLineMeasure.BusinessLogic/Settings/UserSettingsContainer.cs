using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace FishLineMeasure.BusinessLogic.Settings
{
    public class UserSettingsContainer : SettingsContainer
    {
        private Settings _settings;


        private string UserPath
        {
            get 
            { 
                string strUserPath = Path.Combine(_settings.SettingsPath, _strIdentifier); 

                if(!Directory.Exists(strUserPath))
                    Directory.CreateDirectory(strUserPath);

                return strUserPath;
            }
        }


        public UserSettingsContainer(string strIdentifier, Settings set)
            : base(strIdentifier)
        {
            _settings = set;
        }


        public override XElement Save()
        {
            if (string.IsNullOrEmpty(Identifier))
                return null;

            XElement xElmSettings =  new XElement("User", new XAttribute("name", _strIdentifier), 
                                                          from d in m_dicStored
                                                          select new XElement(d.Key, d.Value));

            return xElmSettings;
        }


        protected override void SetStoredValue(string strParameter, XElement elm)
        {
            base.SetStoredValue(strParameter, elm);
        }
    }
}
