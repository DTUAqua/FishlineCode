using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;

namespace SmartDots.Service
{
    public class CustomContentTypeMapper : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            /*switch (contentType.ToLowerInvariant())
            {
                case "text/plain":
                case "application/json":
                    return WebContentFormat.Json;
                case "application/xml":
                    return WebContentFormat.Xml;
                default:
                    return WebContentFormat.Default;
            }*/

            return WebContentFormat.Json;
        }
    }
}