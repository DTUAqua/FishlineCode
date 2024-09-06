using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Security;

namespace FishLineMeasure.BusinessLogic
{
    internal class ServiceLookupClient : FishLineService.LookupClient
    {
        public ServiceLookupClient SupplyCredentials()
        {
            this.ClientCredentials.UserName.UserName = "no";
            this.ClientCredentials.UserName.Password = "ayqbhtBDIILz28oUb+qVbAnEtlZhiaW/nO4+mr2OTKezXyZHxCzPdYJdkWC3wSRFjkxEo/5G27TGVZKN8kss6A==";
            this.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            this.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);

            return this;
        }
    }
}
