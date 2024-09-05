using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel.Security;
using Babelfisk.BusinessLogic.BabelfiskService;

namespace Babelfisk.BusinessLogic.Reporting
{
    internal class ServiceReportingClient : ReportingClient, IDataClient
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
