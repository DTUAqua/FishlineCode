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
    /// <summary>
    /// SmartsDots WPF app only calls one endpoint so only one interface can exist with all the methods
    /// </summary>
    [ServiceContract]
    public interface IAquaDotsService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/ping", BodyStyle = WebMessageBodyStyle.Bare)]
        string Ping(string value);



        #region Security methods


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/authenticate", BodyStyle = WebMessageBodyStyle.Bare)]
        WebApiResult Authenticate(UserAuthentication usr);


        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/getsettings", BodyStyle = WebMessageBodyStyle.Bare)]
        WebApiResult GetSettings();


        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/getreadabilityqualities", BodyStyle = WebMessageBodyStyle.Bare)]
        WebApiResult GetReadabilityQualities();


        #endregion


        #region Data


        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/getanalysesdynamic", BodyStyle = WebMessageBodyStyle.Bare)]
        WebApiResult GetAnalysesDynamic();


        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/getanalysis", BodyStyle = WebMessageBodyStyle.Bare)]
        WebApiResult GetAnalysis();


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/getfiles", BodyStyle = WebMessageBodyStyle.Bare)]
        WebApiResult GetFiles(List<string> imageNames);


        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/getfilewithsampleandannotations", BodyStyle = WebMessageBodyStyle.Bare)]
        WebApiResult GetFileWithSampleAndAnnotations();


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/updatefile", BodyStyle = WebMessageBodyStyle.Bare)]
        WebApiResult UpdateFile(Entities.File file);


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/addannotation", BodyStyle = WebMessageBodyStyle.Bare)]
        WebApiResult AddAnnotation(Annotation annotation);


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/updateannotations", BodyStyle = WebMessageBodyStyle.Bare)]
        WebApiResult UpdateAnnotations(List<Annotation> annotations);
      

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/deleteannotations", BodyStyle = WebMessageBodyStyle.Bare)]
        WebApiResult DeleteAnnotations(List<Guid> ids);




        #endregion
    }
}
