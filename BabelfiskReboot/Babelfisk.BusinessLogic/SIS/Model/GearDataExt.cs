using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.BusinessLogic.SIS.Model
{
    public partial class GearData
    {
        public string GearDataDetails
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(String.Format("Slæb: {0}", haulNo));
                sb.AppendLine(String.Format("Station: {0}", gearNo));
                sb.AppendLine(String.Format("Redskab: {0}", gearCode));
                sb.AppendLine(String.Format("Status: {0}", status));
                sb.AppendLine(String.Format("Fisketid: {0}", gearDur));
                sb.AppendLine("---------------------------------------");
                sb.AppendLine(String.Format("Start-tid: {0}", timeStart));
                sb.AppendLine(String.Format("Start-breddegrad: {0}", posLatStart));
                sb.AppendLine(String.Format("Start-længdegrad: {0}", posLonStart));
                sb.AppendLine(String.Format("Start-square: {0}", squareStart));
                sb.AppendLine("---------------------------------------");
                sb.AppendLine(String.Format("Slut-tid: {0}", timeStop));
                sb.AppendLine(String.Format("Slut-breddegrad: {0}", posLatStop));
                sb.AppendLine(String.Format("Slut-længdegrad: {0}", posLonStop));
                sb.AppendLine(String.Format("Slut-square: {0}", squareStop));
                
                return sb.ToString().TrimEnd('\r', '\n');
            }
        }
    }
}
