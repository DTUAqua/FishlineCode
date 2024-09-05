using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;

namespace SmartDots.Service
{
	public class NewtonsoftJsonContentTypeMapper : WebContentTypeMapper
	{
		public override WebContentFormat GetMessageFormatForContentType(string contentType)
		{
			/*if(contentType != null)
            {
                if (contentType.StartsWith("text/plain", StringComparison.InvariantCultureIgnoreCase))
                {
                    return WebContentFormat.Json;
                }
                else if (contentType.StartsWith("application/json", StringComparison.InvariantCultureIgnoreCase))
                {
                    return WebContentFormat.Json;
                }
                if (contentType.StartsWith("json", StringComparison.InvariantCultureIgnoreCase))
                {
                    return WebContentFormat.Json;
                }
                else if (contentType.StartsWith("application/xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    return WebContentFormat.Xml;
                }
                else
                    return WebContentFormat.Json;
            }

			return WebContentFormat.Json;*/
            return WebContentFormat.Raw;
		}
	}
}