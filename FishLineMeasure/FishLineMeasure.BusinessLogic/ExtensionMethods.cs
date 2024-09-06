using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.BusinessLogic
{
    public static class ExtensionMethods
    {


        public static void SupplyCredentials(this FishLineService.DataRetrievalClient c)
        {
            c.ClientCredentials.UserName.UserName = Settings.Settings.CurrentUser.UserName;
            c.ClientCredentials.UserName.Password = Settings.Settings.CurrentUser.Password;
            c.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            c.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);
        }


        public static void SupplyCredentials(this FishLineService.OfflineClient c)
        {
            c.ClientCredentials.UserName.UserName = Settings.Settings.CurrentUser.UserName;
            c.ClientCredentials.UserName.Password = Settings.Settings.CurrentUser.Password;
            c.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            c.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);
        }
    }
}
