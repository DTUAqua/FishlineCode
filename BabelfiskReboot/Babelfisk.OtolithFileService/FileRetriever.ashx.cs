using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Babelfisk.OtolithFileService
{
    /// <summary>
    /// Summary description for FileRetriever
    /// </summary>
    public class FileRetriever : IHttpHandler
    {
        private static Babelfisk.OtolithFileService.OtolithFileService _os = new Babelfisk.OtolithFileService.OtolithFileService();

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var path = context.Request.PathInfo;

                if (!string.IsNullOrWhiteSpace(path))
                {
                    path = path.Replace("/", "\\");
                    var res = _os.GetFileBytes(path);

                    context.Response.ContentType = "image";

                    byte[] arr = null;
                    if (res != null && (arr = res.Data as byte[]) != null)
                    {
                        context.Response.BinaryWrite(arr);
                        return;
                    }
                }
            }
            catch(Exception e)
            {
              
            }

            context.Response.Write(null);
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}