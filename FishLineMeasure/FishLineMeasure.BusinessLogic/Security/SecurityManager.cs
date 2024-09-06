using Babelfisk.Entities;
using Babelfisk.Entities.SprattusSecurity;
using FishLineMeasure.BusinessLogic.FishLineService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.BusinessLogic.Security
{
    public class SecurityManager
    {
       
        public static bool IsLoggedOn
        {
            get { return Settings.Settings.CurrentUser != null; }
        }

      



        /// <summary>
        /// Retrieve user by name
        /// </summary>
        public Users LogIn(string strName, string strPassword, ref string strFaultCode)
        {
            var ser = new FishLineService.SecurityClient();

            try
            {
                Settings.Settings.CurrentUser = null;

                strPassword = strPassword ?? "";
                string strHash = Hash.ComputeHash(strPassword);
                string token = string.Format("{0}¤¤-¤¤{1}", strHash, strPassword);

                ser.ClientCredentials.UserName.UserName = strName;
                ser.ClientCredentials.UserName.Password = token;
                ser.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;

                Settings.Settings.CurrentUser = (ser as ISecurity).LogonUser();

                if (Settings.Settings.CurrentUser == null)
                {
                    ser.Close();
                    return null;
                }

                ser.Close();

                return Settings.Settings.CurrentUser;
            }
            catch (System.ServiceModel.EndpointNotFoundException ep)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(ep);
                throw ep;
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

    }
}
