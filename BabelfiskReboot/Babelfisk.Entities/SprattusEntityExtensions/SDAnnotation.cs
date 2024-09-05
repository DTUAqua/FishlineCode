using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class SDAnnotation
    {
        public string UIDisplayForAgeTransfer
        {
            get
            {
                return string.Format("Age: {0}, AQScore: {1}, Edge: {2}, User: {3}", age,
                                                                                     this.L_OtolithReadingRemark != null ? L_OtolithReadingRemark.otolithReadingRemark : "",
                                                                                     this.edgeStructure,
                                                                                     createdByUserName);
            }
        }
    }
}
