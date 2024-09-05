using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Anchor.Core;

namespace Babelfisk.BusinessLogic.Settings
{
    [DataContract(IsReference=true)]
    public class ServerNames : SettingsBaseObject<ServerNames>
    {
        [IgnoreDataMember]
        private List<ServerName> _lstServerNames = new List<ServerName>();


        private ServerName _selectedServerName;

        [DataMember]
        public ServerName SelectedServerName
        {
            get { return _selectedServerName; }
            set
            {
                _selectedServerName = value;
                RaisePropertyChanged(() => SelectedServerName);
            }
        }


        [DataMember]
        public List<ServerName> Names
        {
            get { return _lstServerNames; }
            private set { _lstServerNames = value; }
        }


        public ServerNames()
        {
            _lstServerNames.Add(new ServerName { DisplayName = "Dana", Name = @"da-sis.dana.local", DatabaseName = "DanaDB", User = "sa", Password = "dfudfu" });
            _lstServerNames.Add(new ServerName { DisplayName = "Land",  Name = @"aqua-sisland3.win.dtu.dk", DatabaseName = "DanaDB", User = "sa", Password = "dfudfu" });
            _lstServerNames.Add(new ServerName { DisplayName = "Debug", Name = @"CH-PCB-MDU02\SQL2005", DatabaseName = "DanaDB", User = "sa", Password = "dfudfu" });

            _selectedServerName = _lstServerNames[0];
        }

        [OnDeserialized]
        private void OnDeserializing(StreamingContext context)
        {
            try
            {
                if (_selectedServerName != null && _lstServerNames != null)
                {
                    SelectedServerName = _lstServerNames.Where(x => x.DisplayName.Equals(SelectedServerName.DisplayName)).FirstOrDefault();
                }
            }
            catch { }
        }

    }


    [DataContract]
    public class ServerName
    {
       
        public string _strName;

        private string _strDisplayName;

        [DataMember]
        public string DatabaseName;
        [DataMember]
        public string User;
        [DataMember]
        public string Password;

         [DataMember]
        public string Name
        {
            get { return _strName; }
            set { _strName = value; }
        }

         [DataMember]
         public string DisplayName
         {
             get { return _strDisplayName; }
             set { _strDisplayName = value; }
         }


         public string LongDisplayName
         {
             get { return String.Format("{0} ({1})", DisplayName, Name); }
         }

        [IgnoreDataMember]
        public string ConnectionString
        {
            get
            {
                return "user id=" + User + ";password=" + Password + ";initial catalog=" + DatabaseName + ";data source=" + Name + ";";
            }
        }

        [IgnoreDataMember]
        public string EFConnectionString
        {
            get
            {
                string str = "metadata=res://*/SIS.Model.DanaDB.csdl|res://*/SIS.Model.DanaDB.ssdl|res://*/SIS.Model.DanaDB.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source={0};initial catalog={1};user id={2};password={3};multipleactiveresultsets=True;App=EntityFramework&quot;";
                str = str.Replace("&quot;", "\"");
                str = string.Format(str, Name, DatabaseName, User, Password);

                return str;
            }
        }
    }
   
}
