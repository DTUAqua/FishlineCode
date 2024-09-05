using SmartDots.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SmartDots.Service
{
    public partial class AquaDotsService : IAquaDotsService
    {
        public string Ping(string value)
        {
            return string.Format("You entered: {0}", value);
        }


       
    }
}
