using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;

namespace Babelfisk.BusinessLogic.SmartDots
{
    internal class FishLineSmartDotsClient : BabelfiskService.SmartDotsClient, IDataClient
    {

        public IDataClient SupplyCredentials()
        {
            this.ClientCredentials.UserName.UserName = SecurityManager.CurrentUser.UserName;
            this.ClientCredentials.UserName.Password = SecurityManager.CurrentUser.Password.ToString();
            this.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            this.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);

            return this;
        }

    }
}
