using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.BusinessLogic.SIS.Model
{
    public partial class CruiseInformation
    {
        public string CruiseInformationDetails
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(String.Format("Togtnummer: {0}", cruiseNo));
                sb.AppendLine(String.Format("Togtnavn: {0}", cruiseName));
                sb.AppendLine(String.Format("Togtdage: {0}", cruiseDays));
                sb.AppendLine(String.Format("Togtleder: {0}", cruiseLeader));
                sb.AppendLine(String.Format("Kaptajn: {0}", captain));
                sb.AppendLine(String.Format("Tekniker: {0}", technician));
                sb.AppendLine(String.Format("Togtområde: {0}", cruiseArea));
                sb.AppendLine(String.Format("Startdato: {0}", startDate));
                sb.AppendLine(String.Format("Slutdato: {0}", endDate));
                sb.AppendLine(String.Format("Institution: {0}", institution));
                sb.AppendLine(String.Format("Institut: {0}", institute));

                return sb.ToString().TrimEnd('\r', '\n');
            }
        }
    }
}
