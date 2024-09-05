using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SmartDots.Service.TestConsole
{
    public class RESTRequest
    {
        private string _server;

        private string _anonymousToken = "{9A31042A-E72C-4790-9950-398002AA4704}";

        public int RequestTimeout = 30000;

        public int ResponseTimeout = 30000;




        public RESTRequest(string serverURI)
        {
            _server = serverURI;
        }



        /// <summary>
        /// Make a json POST request, returning object T as result (deserialized from Json).
        /// </summary>
        public async Task<T> Get<T>(string method)
        {
            var res = await Get(method);

            return DesrializeContentJson<T>(res);
        }



        public async Task<string> Get(string method, bool unwrapResult = true)
        {
            string res = null;

            try
            {
                var request = HttpWebRequest.Create(Path.Combine(_server, method));
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers["AppToken"] = _anonymousToken;
               // request.Headers["Token"] = "";

                HttpWebResponse response = await request.GetResponseAsync().WithTimeout(ResponseTimeout) as HttpWebResponse;


                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        res = reader.ReadToEnd();

                        if(unwrapResult)
                            res = Unwrap(res);
                    }
                }
                else
                {
                    if (response != null)
                        return response.StatusCode.ToString();
                }
            }
            catch (Exception e)
            {
                //Do not log on get requests, since there could be many, if the phone has no internet connection.
                //AppLogger.LogError(e, "RESTRequest->Get()");
            }

            return res;
        }


        /// <summary>
        /// Convert array of parameters (name value pairs to Json).
        /// "{ "ParamName1" : "Value1", "ParamName2" : "Value2" }"
        /// </summary>
        private string ParametersToJson(params Tuple<string, string>[] parameters)
        {
            string strRes = null;

            strRes = string.Format("{{{0}}}", string.Join(",", parameters.Select(x => string.Format("\"{0}\":{1}", x.Item1, x.Item2))));

            return strRes;
        }


        /// <summary>
        /// Make a json POST request, returning object T as result (deserialized from Json).
        /// </summary>
        public async Task<T> Post<T>(string method, params Tuple<string, string>[] parametersNameValue)
        {
            if (parametersNameValue == null)
                throw new Exception("parametersNameValue was NULL. Please supply at least 1 parameter for POST request");

            var res = await Post(method, ParametersToJson(parametersNameValue));

            return DesrializeContentJson<T>(res);
        }


        public async Task<byte[]> PostRaw(string method, params Tuple<string, string>[] parametersNameValue)
        {
            if (parametersNameValue == null)
                throw new Exception("parametersNameValue was NULL. Please supply at least 1 parameter for POST request");

            var res = await PostRaw(method, ParametersToJson(parametersNameValue));

            return res;
        }


        /// <summary>
        /// Make a json POST request, returning object T as result (deserialized from Json).
        /// </summary>
        public async Task<T> Post<T>(string method, string serializedDataString, bool unwrapResult = true)
        {
            var res = await Post(method, serializedDataString, unwrapResult);

            return DesrializeContentJson<T>(res);
        }


        /// <summary>
        /// Make a json POST request, returning a json string as result.
        /// </summary>
        public async Task<string> Post(string method, string serializedDataString, bool unwrapResult = true)
        {
            string res = null;

            try
            {
                byte[] arr = null;
                arr = Encoding.UTF8.GetBytes(serializedDataString);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Path.Combine(_server, method));

                request.ContentType = "application/json";
                request.Headers["AppToken"] = _anonymousToken;

               
                //    request.Headers["Token"] = AppSettings.Instance.SecurityToken;

                request.Method = "POST";
                var stream = await request.GetRequestStreamAsync().WithTimeout(RequestTimeout);

                if (stream == null)
                    return res;

                stream.Write(arr, 0, arr.Length);
                stream.Flush();

                try
                {
                    stream.Dispose();
                }
                catch { }
                /*using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(serializedDataString);
                    writer.Flush();
                    writer.Dispose();
                }*/

                var response = await request.GetResponseAsync().WithTimeout(ResponseTimeout);

                if (response == null)
                    return res;

                var respStream = response.GetResponseStream();

                using (StreamReader sr = new StreamReader(respStream))
                {
                    res = sr.ReadToEnd();
                }

                if(unwrapResult)
                    res = Unwrap(res);
            }
            catch (Exception e)
            {
                //Do not log on post requests, since there could be many, if the phone has no internet connection.
                //AppLogger.LogError(e, "RESTRequest->Post()");
            }

            return res;
        }


        public async Task<byte[]> PostRaw(string method, string serializedDataString)
        {
            byte[] arrRes = null;

            try
            {
                byte[] arr = null;
                arr = Encoding.UTF8.GetBytes(serializedDataString);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Path.Combine(_server, method));

                request.ContentType = "application/json";
                request.Headers["AppToken"] = _anonymousToken;

                //    request.Headers["Token"] = AppSettings.Instance.SecurityToken;

                request.Method = "POST";
                var stream = await request.GetRequestStreamAsync().WithTimeout(RequestTimeout);

                if (stream == null)
                    return arrRes;

                stream.Write(arr, 0, arr.Length);
                stream.Flush();

                try
                {
                    stream.Dispose();
                }
                catch { }
                /*using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(serializedDataString);
                    writer.Flush();
                    writer.Dispose();
                }*/

                var response = await request.GetResponseAsync().WithTimeout(ResponseTimeout);

                if (response == null)
                    return arrRes;

                var respStream = response.GetResponseStream();

                MemoryStream ms = new MemoryStream();
                respStream.CopyTo(ms);

                arrRes = ms.ToArray();
            }
            catch (Exception e)
            {
                //Do not log on post requests, since there could be many, if the phone has no internet connection.
                //AppLogger.LogError(e, "RESTRequest->PostRaw()");
            }

            return arrRes;
        }



        private string Unwrap(string res)
        {
            var r = res;

            if (res != null)
            {
                var jres = DesrializeContentJson(res) as JObject;
                if (jres.Count > 0)
                {
                    var first = jres.First;
                    if (first != null)
                        r = first.First.ToString();
                }
            }

            return r;
        }


        #region Serialization methods


        public T DeserializeContent<T>(String stringToDeserialize) where T : class
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(stringToDeserialize));

            return ser.ReadObject(stream) as T;
        }


        public T DesrializeContentJson<T>(String stringToDeserialize)
        {
            try
            {
                if (stringToDeserialize == null)
                    return default(T);

                return JsonConvert.DeserializeObject<T>(stringToDeserialize);
            }
            catch { }

            return default(T);
        }


        public object DesrializeContentJson(String stringToDeserialize)
        {
            return JsonConvert.DeserializeObject(stringToDeserialize);
        }


        public string SerializeContent(Object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize);
        }

        public string SerializeContentDataContract<T>(T objectToSerialize, params Type[] knownTypes)
        {
            DataContractJsonSerializer ser = null;

            if (knownTypes.Length == 0)
                ser = new DataContractJsonSerializer(typeof(T));
            else
                ser = new DataContractJsonSerializer(typeof(T), knownTypes);

            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, objectToSerialize);

            var arr = ms.ToArray();
            var res = Encoding.UTF8.GetString(arr, 0, arr.Length);
            return res;
        }


        #endregion
    }
}

