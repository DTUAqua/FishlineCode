using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.SprattusSecurity;
using Babelfisk.BusinessLogic.BabelfiskService;
using System.ServiceModel;

namespace Babelfisk.BusinessLogic
{
    public class SecurityManager
    {
        private static Users _user = null;

        public static bool IsLoggedOn
        {
            get { return _user != null; }
        }

        public static Users CurrentUser
        {
            get { return _user; }
            private set
            {
                _user = value;
            }
        }



        /// <summary>
        /// Retrieve user by name
        /// </summary>
        public Users LogIn(string strName, string strPassword, ref string strFaultCode)
        {
            var ser = DataClientFactory.CreateSecurityClient();

            try
            {
                strPassword = strPassword ?? "";
                string strHash = Entities.Hash.ComputeHash(strPassword);
                string token = string.Format("{0}¤¤-¤¤{1}", strHash, strPassword);

                if (ser is BabelfiskService.SecurityClient)
                {
                    (ser as BabelfiskService.SecurityClient).ClientCredentials.UserName.UserName = strName;
                    (ser as BabelfiskService.SecurityClient).ClientCredentials.UserName.Password = token;
                    (ser as BabelfiskService.SecurityClient).ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None; 
                }
                else if (ser is Security.OfflineSecurityClient)
                    (ser as Security.OfflineSecurityClient).SupplyCredentials(strName, strPassword);

                CurrentUser = (ser as ISecurity).LogonUser();

                if (CurrentUser == null)
                {
                    ser.Close();
                    return null;
                }

                ser.Close();

                return CurrentUser;
            }
            catch (System.ServiceModel.EndpointNotFoundException ep)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(ep);
                throw new ApplicationException(ep.Message);
            }
            catch (System.ServiceModel.Security.MessageSecurityException mse)
            {
                ser.Abort();

                if (mse != null && mse.InnerException != null && (mse.InnerException is FaultException) && (mse.InnerException as FaultException).Code != null)
                    strFaultCode = (mse.InnerException as FaultException).Code.Name;

                //throw new Exception(ms.Message + ", " + (ms.InnerException == null ? "No Inner exception" : ms.InnerException.Message) + (ms.InnerException.InnerException == null ? "No Inner exception" : ms.InnerException.InnerException.Message));
                return null;
            }
            catch (Exception e)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Retrieve user by name
        /// </summary>
        public Users GetUser(string strName)
        {
            var ser = DataClientFactory.CreateSecurityClient();

            try
            {
                ser.SupplyCredentials();

                var ent = (ser as ISecurity).GetUser(strName);
                ser.Close();

                return ent;
            }
            catch (Exception e)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public List<Users> GetUsers()
        {
            var ser = DataClientFactory.CreateSecurityClient();

            try
            {
                ser.SupplyCredentials();

                var ent = (ser as ISecurity).GetUsers().ToList();
                ser.Close();

                return ent;
            }
            catch (Exception e)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Retrieve user by id
        /// </summary>
        public Users GetUserById(int id)
        {
            var ser = DataClientFactory.CreateSecurityClient();

            try
            {
                ser.SupplyCredentials();

                var ent = (ser as ISecurity).GetUserById(id);
                ser.Close();

                return ent;
            }
            catch (Exception e)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public void Logout()
        {
            CurrentUser = null;
        }


        public List<Role> GetRoles()
        {
            var ser = DataClientFactory.CreateSecurityClient();

            try
            {
                ser.SupplyCredentials();

                var ent = (ser as ISecurity).GetRoles().ToList();
                ser.Close();

                return ent;
            }
            catch (Exception e)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public List<FishLineTasks> GetTasks()
        {
            var ser = DataClientFactory.CreateSecurityClient();

            try
            {
                ser.SupplyCredentials();

                var ent = (ser as ISecurity).GetTasks().ToList();
                ser.Close();

                return ent;
            }
            catch (Exception e)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public Entities.DatabaseOperationResult SaveChanges(Users user)
        {
            var ser = DataClientFactory.CreateSecurityClient();

            try
            {
                ser.SupplyCredentials();

                Entities.DatabaseOperationResult dor = (ser as ISecurity).UpdateUser(user);

                ser.Close();

                return dor;
            }
            catch (Exception e)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public bool CanDeleteUser(Users user)
        {
            var ser = DataClientFactory.CreateSecurityClient();

            try
            {
                ser.SupplyCredentials();

                bool dor = (ser as ISecurity).CanDeleteUser(user);

                ser.Close();

                return dor;
            }
            catch (Exception e)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }



        public Entities.DatabaseOperationResult DeleteUser(Users user)
        {
            var ser = DataClientFactory.CreateSecurityClient();

            try
            {
                ser.SupplyCredentials();

                user.MarkAsDeleted();
                Entities.DatabaseOperationResult dor = (ser as ISecurity).UpdateUser(user);

                ser.Close();

                return dor;
            }
            catch (Exception e)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }

        }


        public Entities.DatabaseOperationResult UpdateRole(ref Role r)
        {
            var ser = DataClientFactory.CreateSecurityClient();

            try
            {
                ser.SupplyCredentials();

                Entities.DatabaseOperationResult dor = (ser as ISecurity).UpdateRole(ref r);

                ser.Close();

                return dor;
            }
            catch (Exception e)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }

    }
}
