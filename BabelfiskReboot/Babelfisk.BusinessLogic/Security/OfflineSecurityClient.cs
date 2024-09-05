using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.BusinessLogic.BabelfiskService;
using Babelfisk.Entities.SprattusSecurity;

namespace Babelfisk.BusinessLogic.Security
{
    internal class OfflineSecurityClient : IDataClient, ISecurity
    {
        private string _strUserName;

        private string _strPassword;


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


        public void SupplyCredentials(string strUserName, string strPassword)
        {
            _strUserName = strUserName;
            _strPassword = strPassword;
        }


        public Entities.SprattusSecurity.Users LogonUser()
        {
            if(!Settings.Settings.Instance.OfflineUsers.IsInitialized || Settings.Settings.Instance.OfflineUsers.Users == null)
                throw new ApplicationException("Applikationen er i offlinetilstand, men offline-brugerlisten kunne ikke lokaliseres på computeren og du kan dermed ikke logge ind.");

            Users usr;

            string strPwdHash = Entities.Hash.ComputeHash((_strPassword ?? ""));
            if ((usr = Settings.Settings.Instance.OfflineUsers.Users.Where(x => x.UserName.Equals(_strUserName) && x.Password.Equals(_strPassword)).FirstOrDefault()) == null &&
                (usr = Settings.Settings.Instance.OfflineUsers.Users.Where(x => x.UserName.Equals(_strUserName) && x.Password.Equals(strPwdHash)).FirstOrDefault()) == null)
                throw new System.ServiceModel.Security.MessageSecurityException("Username or password is incorrect.");

            return usr;
        }

        public Entities.SprattusSecurity.Users GetUser(string strUserName)
        {
            throw new NotImplementedException();
        }

        public Entities.SprattusSecurity.Users[] GetUsers()
        {
            throw new NotImplementedException();
        }

        public Entities.SprattusSecurity.Users GetUserById(int intId)
        {
            throw new NotImplementedException();
        }

        public Entities.SprattusSecurity.Role[] GetRoles()
        {
            throw new NotImplementedException();
        }

        public Entities.SprattusSecurity.FishLineTasks[] GetTasks()
        {
            throw new NotImplementedException();
        }

        public bool CanDeleteUser(Entities.SprattusSecurity.Users user)
        {
            throw new NotImplementedException();
        }

        public Entities.DatabaseOperationResult UpdateUser(Entities.SprattusSecurity.Users user)
        {
            throw new NotImplementedException();
        }

        public Entities.DatabaseOperationResult UpdateRole(ref Role r)
        {
            throw new NotImplementedException();
        }

    }
}
