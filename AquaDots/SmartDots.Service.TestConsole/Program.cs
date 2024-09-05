using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartDots.Service.Entities;

namespace SmartDots.Service.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            PostAuthentication();
        }


        private static void PostAuthentication()
        {
            try
            {
                var req = new RESTRequest("http://localhost:56115/SmartDotsService.svc/");

                var ua = new UserAuthentication() { DtoAuthenticationMethod = AuthenticationMethod.Basic, Username = "Mads", Password = "test" };
                var str = JsonConvert.SerializeObject(ua);
                var res = req.Post<WebApiResult>("authenticate", str, false);
                res.Wait();
                res = req.Get<WebApiResult>("getsettings?token=whatever");
                res.Wait();

                var resObj = res.Result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
