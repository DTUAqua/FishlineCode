using SmartDots.Service.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using Babelfisk.Service;
using FishLine = Babelfisk.Entities.Sprattus;
using Anchor.Core;

namespace SmartDots.Service
{
    public partial class AquaDotsService
    {
        public static string OtolithImagesWebPath
        {
            get
            {
                string path = System.Configuration.ConfigurationManager.AppSettings["OtolithImagesWebPath"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["OtolithImagesWebPath"].ToString();

                return path;
            }
        }


        public static int L_SDReaderExperienceAdvancedDatabaseId
        {
            get
            {
                int res = 1;
                try
                {
                    string sId = System.Configuration.ConfigurationManager.AppSettings["L_SDReaderExperienceAdvancedDbId"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["L_SDReaderExperienceAdvancedDbId"].ToString();
                    if (!sId.TryParseInt32(out res))
                        res = 1;
                }
                catch(Exception e)
                {
                    Anchor.Core.Loggers.Logger.LogError(e);
                }

                return res;
            }
        }


        /// <summary>
        /// Get list of data/rows (events) for main grid after login to SmartDots.
        /// </summary>
        public WebApiResult GetAnalysesDynamic()
        {
            try
            {
                var req = HttpContext.Current.Request.Params["token"];

                if (!ValidateToken(req))
                    return new WebApiResult() { ErrorMessage = _tokenInvalidErrorMessage };

                List<ExpandoObject> lstEvents = new List<ExpandoObject>();
                using (var ctx = new FishLine.SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    var q = (from e in ctx.SDEvent
                                           .Include("L_SDEventType")
                                           .Include("L_SDPurpose")
                                           .Include("L_Species")
                                           .Include("L_DFUAreas")
                                           .Include("L_SDSampleType")
                             select new { Event = e, 
                                          SampleCount = e.SDSample.Count(),
                                          ImageCount = e.SDSample.SelectMany(x => x.SDFile).Count(),
                                          ReadersCount = e.SDReaders.Count(),
                                          EventType = e.L_SDEventType,
                                          Purpose = e.L_SDPurpose,
                                          Species = e.L_Species,
                                          Areas = e.L_DFUAreas,
                                          SampleType = e.L_SDSampleType
                                        });

                   // var query = ((System.Data.Objects.ObjectQuery)q).ToTraceString();
                    var lst = q.ToList();

                    foreach (var e in lst)
                    {
                        dynamic eo = new ExpandoObject();
                        var evt = e.Event;

                        eo.ID = evt.sdEventGuid;
                        eo.EventID = evt.sdEventId;
                        eo.NameOfEvent = evt.name;
                        eo.Species = evt.L_Species == null ? "" : evt.L_Species.UIDisplay;
                        eo.Year = evt.year == null ? "" : evt.year.Value.ToString();
                        eo.Areas = (e.Areas == null || e.Areas.Count == 0) ? "" : string.Join(", ", e.Areas.Select(x => x.DFUArea));
                        eo.StartDate = evt.startDate == null ? "" : evt.startDate.Value.ToString("dd-MM-yyyy");
                        eo.EndDate = evt.endDate == null ? "" : evt.endDate.Value.ToString("dd-MM-yyyy");
                        //eo.Purpose = evt.L_SDPurpose == null ? "" : evt.L_SDPurpose.description;
                        eo.EventType = evt.L_SDEventType == null ? "" : evt.L_SDEventType.description;
                        eo.SampleType = evt.L_SDSampleType == null ? "" : evt.L_SDSampleType.description;
                        eo.Closed = evt.closed ? "Closed" : "Open";
                        eo.AgeReaders = e.ReadersCount;
                        eo.SampleCount = e.SampleCount;
                        eo.ImageCount = e.ImageCount;
                        eo.CreatedUserName = evt.createdByUserName;

                        lstEvents.Add(eo);
                    }
                }

                

                return new WebApiResult() { Result = lstEvents };
            }
            catch (Exception e)
            {
                return new WebApiResult() { ErrorMessage = _unknownErrorMessage + e.Message };
            }

            /* 
            dynamic eo = new ExpandoObject();
            eo.ID = "D798E78C-C2B2-44C0-82FE-8E86709387F1";  //GUID Event id
            eo.Purpose = "Purpose";
            eo.EventID = "100"; //Event id
            eo.NameOfEvent = "Event 1";
            eo.Species = "COD";
            eo.StartDate = "10-12-2021";
            eo.EndDate = "11-12-2021";
            eo.OrganizerEmail = "test@test.dk";
            eo.Institute = "DTU Aqua";
            eo.EventType = "EventType";
            eo.Year = "2021";
            eo.ExtraColumn = "Something";
            eo.Closed = "Event is open"; //"Event is closed", "Event is public"
            eo.UserProgress = "Ongoing"; //Finished, NA
            eo.Role = "Role";
            eo.Progress = "Progress";

            lstEvents.Add(eo);

            eo = new ExpandoObject();
            eo.ID = "{21F258C4-742F-4B9D-AA62-666E40493CFA}";  //GUID Event id
            eo.Purpose = "Purpose2";
            eo.EventID = "101"; //Event id
            eo.NameOfEvent = "Event 2";
            eo.Species = "COD";
            eo.StartDate = "11-12-2021";
            eo.EndDate = "13-12-2021";
            eo.OrganizerEmail = "test@test.dk";
            eo.Institute = "DTU Aqua";
            eo.EventType = "EventType";
            eo.Year = "2021";
            eo.Closed = "Event is open"; //"Event is closed", "Event is public"
            eo.UserProgress = "Finished"; //Finished, NA
            eo.Role = "Role";
            eo.Progress = "Progress";

            lstEvents.Add(eo);
            */

            //  string res = Newtonsoft.Json.JsonConvert.SerializeObject(lstEvents);

            /* var stream1 = new MemoryStream();
             var dcj = new DataContractJsonSerializer(typeof(WebApiResult));
             dcj.WriteObject(stream1, lstEvents);
             stream1.Position = 0;
             var sr = new StreamReader(stream1);
             var s = sr.ReadToEnd();
             stream1.Close();*/
        }


        /// <summary>
        /// Return an analysis object after selecting an event in SmartDots
        /// </summary>
        public WebApiResult GetAnalysis()
        {
            try
            {
                var req = HttpContext.Current.Request.Params["token"];
                string idEventGuidString = HttpContext.Current.Request.Params["id"];
                Guid eventGuid = new Guid(idEventGuidString);

                int userId = -1, dfuPersonId = -1;
                string userName = null;
                if (!ValidateToken(req, ref userName, ref userId, ref dfuPersonId))
                    return new WebApiResult() { ErrorMessage = _tokenInvalidErrorMessage };

                var usr = GetUserFromId(userId);

                if (usr == null)
                    return new WebApiResult() { ErrorMessage = "Token is invalid (user)." };

                bool isADAdmin = usr != null && usr.HasEditSDAnnotationsTask;

                FishLine.SDEvent evt = null;
                using (var ctx = new FishLine.SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    evt = (from e in ctx.SDEvent
                                           .Include("L_SDEventType")
                                           .Include("L_SDPurpose")
                                           .Include("L_Species")
                                           .Include("L_DFUAreas")
                                           .Include("L_SDSampleType")
                                           .Include("SDReaders")
                                           .Include("SDReaders.SDReader")
                                           .Include("SDReaders.SDReader.DFUPerson")
                           where e.sdEventGuid == eventGuid
                           select e).FirstOrDefault();
                }

                if (evt == null)
                    return new WebApiResult() { ErrorMessage = "Event not found." };

                Analysis analysis = new Analysis();
                analysis.AnalysisParameters = new List<AnalysisParameter>();

                analysis.Id = eventGuid;
                analysis.Number = evt.sdEventId;
                analysis.HeaderInfo = "User " + usr.UserName + ", ICES  - " + "EventType: " + evt.L_SDEventType.description + ", NameOfEvent: " + evt.name;
                analysis.ShowEdgeColumn = true;
               

                if (userName != null)
                {
                    int advancedId = L_SDReaderExperienceAdvancedDatabaseId;
                    bool isAdvancedUser = evt != null && evt.SDReaders != null && evt.SDReaders.Where(x => x.SDReader != null && x.SDReader.DFUPerson != null && x.SDReader.sdReaderExperienceId == advancedId && x.SDReader.DFUPerson.initials != null && x.SDReader.DFUPerson.initials.Equals(userName, StringComparison.InvariantCultureIgnoreCase)).Any();
                    bool isEventCreator = dfuPersonId == evt.createdById;
                    analysis.UserCanPin = isEventCreator || isAdvancedUser || isADAdmin; //Edit other users annotations should also be able to pin
                }
                                      

                Folder f = new Folder();
                f.Id = Guid.NewGuid();
                //f.Path = "C:\\temp\\" + "2021" + "\\" + "101";
                f.Path = OtolithImagesWebPath;
                analysis.Folder = f;

                /* AnalysisParameter p = new AnalysisParameter();
                 p.Id = Guid.NewGuid();
                 p.Code = "OWR";
                 p.Description = "Age, method: Otolith Winter Rings";*/

                var analysisParameters = GetAnalysisParameters();

                if (analysisParameters != null && analysisParameters.Count > 0)
                    analysis.AnalysisParameters.AddRange(analysisParameters);

                return new WebApiResult() { Result = analysis };
            }
            catch (Exception e)
            {
                return new WebApiResult() { ErrorMessage = _unknownErrorMessage + e.Message };
            }
        }


        private List<AnalysisParameter> GetAnalysisParameters()
        {
            try
            {
                var req = HttpContext.Current.Request.Params["token"];

                if (!ValidateToken(req))
                    return new List<AnalysisParameter>();

                List<Babelfisk.Entities.Sprattus.L_SDAnalysisParameter> lst = null;
                using (var ctx = new Babelfisk.Entities.Sprattus.SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    lst = ctx.L_SDAnalysisParameter.OrderBy(x => x.analysisParameter).ToList();
                }

                List<AnalysisParameter> lstParams = new List<AnalysisParameter>();
                if (lst != null && lst.Count > 0)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        var itm = lst[i];

                        lstParams.Add(new AnalysisParameter()
                        {
                            Id = IntToGuid(itm.L_sdAnalysisParameterId),
                            Code = itm.analysisParameter,
                            Description = itm.description,
                        });
                    }
                }

                return lstParams;
            }
            catch (Exception e)
            {
                return new List<AnalysisParameter>();
            }
        }


        /// <summary>
        /// Get the available files.
        /// </summary>
        public WebApiResult GetFiles(List<string> imageNames)
        {
            try
            {
                var req = HttpContext.Current.Request.Params["token"];
                var idEventGuidString = HttpContext.Current.Request.Params["analysisid"];
                Guid eventGuid = new Guid(idEventGuidString);

                int userId = -1, dfuPersonId = -1;
                string userName = null;
                if (!ValidateToken(req, ref userName, ref userId, ref dfuPersonId))
                    return new WebApiResult() { ErrorMessage = _tokenInvalidErrorMessage };

                var user = GetUserFromId(userId);

                if (user == null)
                    return new WebApiResult() { ErrorMessage = "Token is invalid (user)." };

                List<FishLine.SDSample> lstSamples = null;
                FishLine.SDEvent evt = null;
                using (var ctx = new FishLine.SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    evt = (from e in ctx.SDEvent
                               // .Include("L_Species")
                               // .Include("L_SDSampleType")
                           where e.sdEventGuid == eventGuid
                           select e).FirstOrDefault();

                    if (evt == null)
                        return new WebApiResult() { ErrorMessage = "Event not found." };

                    int id = evt.sdEventId;

                    lstSamples = (from s in ctx.SDSample
                                                .Include("L_DFUArea")
                                                .Include("L_Stock")
                                                .Include("Maturity")
                                                .Include("L_SDPreparationMethod")
                                               // .Include("L_SDOtolithDescription")
                                               .Include("SDFile")
                                               .Include("SDFile.SDAnnotation")
                                      //.Include("SDFile.SDAnnotation.SDLine")
                                      //.Include("SDFile.SDAnnotation.SDPoint")
                                  where s.sdEventId == id
                                  select s
                                 ).ToList();
                }

                List<Entities.File> lst = new List<Entities.File>();

                foreach (var s in lstSamples)
                {
                    if (s.SDFile == null || s.SDFile.Count == 0)
                        continue;

                    foreach (var f in s.SDFile)
                    {
                        var mSample = new Sample();
                        mSample.Id = s.sdSampleGuid;

                        mSample.StatusCode = "To do";
                        mSample.StatusColor = "#cc0000";
                        mSample.StatusRank = 1;

                        var mFile = new Entities.File();
                        mFile.ID = f.sdFileGuid;
                        mFile.SampleID = s.sdSampleGuid;
                        mFile.Sample = mSample;
                        mFile.Scale = f.scale.HasValue ? (decimal)f.scale.Value : 0;
                        mFile.SampleNumber = s.animalId;

                        bool hasEditAnnotationsRights = user.HasTask(Babelfisk.Entities.SecurityTask.EditSDAnnotations);

                        mFile.AnnotationCount = f.SDAnnotation == null ? 0 : f.SDAnnotation.Where(x => ShouldShowAnnotation(x, dfuPersonId, !evt.closed, hasEditAnnotationsRights) && (!x.isFixed.HasValue || !x.isFixed.Value)).Count();
                        mFile.Annotations = new List<Annotation>(); //Don't load the annotation here, since they are not used before a file is clicked in the SmartDots.
                        mFile.Filename = f.fileName;
                        if (!string.IsNullOrWhiteSpace(f.path))
                            mFile.Filename = System.IO.Path.Combine(f.path, f.fileName);
                        mFile.DisplayName = f.displayName;
                        mFile.IsReadOnly = evt.closed;

                        //Add any additional columns here if wanted.
                        //var res = new Dictionary<string, string>();
                        mFile.SampleProperties = GetFilesColumns(s, evt);

                        if (mFile.AnnotationCount > 0)
                        {
                            if (f.SDAnnotation.Where(x => x.isApproved.HasValue && x.isApproved.Value).Any())
                            {
                                mSample.StatusCode = "Done";
                                mSample.StatusColor = "#00b300";
                                mSample.StatusRank = 3;
                            }
                            else
                            {
                                mSample.StatusCode = "Work in progress";
                                mSample.StatusColor = "#ff8000";
                                mSample.StatusRank = 2;
                            }
                        }

                        lst.Add(mFile);
                    }
                }

                var cmp = new Anchor.Core.Comparers.StringNumberComparer();
                lst = lst.OrderBy(x => x.SampleNumber, cmp).ToList();

                return new WebApiResult() { Result = lst };
            }
            catch (Exception e)
            {
                return new WebApiResult() { ErrorMessage = _unknownErrorMessage + e.Message };
            }

        }


       

        private string GetSampleOrigin(FishLine.SDSample s)
        {
            List<string> lst = new List<string>();;

            if (!string.IsNullOrWhiteSpace(s.cruise))
                lst.Add(s.cruise);

            if (!string.IsNullOrWhiteSpace(s.trip))
                lst.Add(s.trip);

            if (!string.IsNullOrWhiteSpace(s.station))
                lst.Add(s.station);

            return string.Join(" - ", lst);
        }


        /// <summary>
        /// Returns s file and associated samples and annotations for editing in smartdots.
        /// </summary>
        public WebApiResult GetFileWithSampleAndAnnotations()
        {
            try
            {
                var req = HttpContext.Current.Request.Params["token"];
                var sdFileGuidIdString = HttpContext.Current.Request.Params["id"];
                var fileGuidId = new Guid(sdFileGuidIdString);
                var getAnnotations = HttpContext.Current.Request.Params["withAnnotations"];
                var getSample = HttpContext.Current.Request.Params["withSample"];

                int userId = -1, dfuPersonId = -1;
                string userName = null;
                if (!ValidateToken(req, ref userName, ref userId, ref dfuPersonId))
                    return new WebApiResult() { ErrorMessage = _tokenInvalidErrorMessage };


                var user = GetUserFromId(userId);

                if (user == null)
                    return new WebApiResult() { ErrorMessage = "Token is invalid (user)." };

                FishLine.SDFile sdFile = null;

                using (var ctx = new FishLine.SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    sdFile = (from s in ctx.SDFile
                                               .Include("SDSample")
                                               .Include("SDSample.L_DFUArea")
                                               .Include("SDSample.L_Stock")
                                               .Include("SDSample.Maturity")
                                               .Include("SDSample.L_SDPreparationMethod")
                                               .Include("SDSample.L_SDOtolithDescription")
                                               .Include("SDSample.L_SDLightType")
                                               .Include("SDSample.SDEvent")
                                               .Include("SDSample.SDEvent.L_Species")
                                               .Include("SDSample.SDEvent.L_SDSampleType")
                                               .Include("SDSample.SDEvent.SDReaders")
                                               .Include("SDSample.SDEvent.SDReaders.SDReader")
                                               .Include("SDSample.SDEvent.SDReaders.SDReader.DFUPerson")
                                               .Include("SDAnnotation")
                                               .Include("SDAnnotation.SDLine")
                                               .Include("SDAnnotation.SDPoint")
                              where s.sdFileGuid == fileGuidId
                              select s
                                 ).FirstOrDefault();                   
                }

                if (sdFile == null)
                    return new WebApiResult() { ErrorMessage = "File not found." };

                FishLine.SDSample sdSample = sdFile.SDSample;

                if (sdSample == null)
                    return new WebApiResult() { ErrorMessage = "Sample information not found." };

                FishLine.SDEvent sdEvent = sdSample.SDEvent;

                if (sdEvent == null)
                    return new WebApiResult() { ErrorMessage = "Event information not found." };

                var mFile = new Entities.File();
                mFile.ID = sdFile.sdFileGuid;

                mFile.SampleID = sdSample.sdSampleGuid;
                mFile.Scale = sdFile.scale.HasValue ? (decimal?)sdFile.scale.Value : null;
                mFile.SampleNumber = sdSample.animalId;
                mFile.Annotations = new List<Annotation>(); //Don't load the annotation here, since they are not used before a file is clicked in the SmartDots.
                mFile.Filename = sdFile.fileName;
                if (!string.IsNullOrWhiteSpace(sdFile.path))
                    mFile.Filename = System.IO.Path.Combine(sdFile.path, sdFile.fileName);
                mFile.DisplayName = sdFile.displayName;

                bool canAnnotate = false;
                bool isAdvancedUser = false;
                bool isAgeReader = false;
                bool isEventCreator = false;
                bool isYearlyReadingEvent = false;
                bool isADAdmin = user != null && user.HasEditSDAnnotationsTask;

                if (sdEvent != null)
                    isYearlyReadingEvent = sdEvent.IsYearlyReadingEventType;

                if (userName != null)
                {
                    int advancedId = L_SDReaderExperienceAdvancedDatabaseId;

                    if(sdEvent != null && sdEvent.SDReaders != null)
                    {
                        //Find the reader for the logged in user.
                        var usr = sdEvent.SDReaders.Where(x => x.SDReader != null && x.SDReader.dfuPersonId == dfuPersonId).FirstOrDefault();

                        canAnnotate = usr != null;

                        //If the reader was found, set some flags concerning that (advaced reader or not)
                        if (usr != null)
                        {
                            isAgeReader = true;
                            if(usr.SDReader.sdReaderExperienceId == advancedId)
                                isAdvancedUser = true;
                        }
                    }
                   
                    isEventCreator = dfuPersonId == sdEvent.createdById;

                    //Can approve file if the user is AquaDots admin, is event creator, or avanced user in yearly reading events, or basic user in Sammenlæsning and Reference events.
                    mFile.CanApprove = isADAdmin || isEventCreator || (isAdvancedUser && isYearlyReadingEvent) || (isAgeReader && !isYearlyReadingEvent);
                }

                mFile.IsReadOnly = sdEvent.closed || (!canAnnotate && !isADAdmin);

                

                //var res = new Dictionary<string, string>();
                //res.Add("Annotations", mFile.AnnotationCount.ToString());
                //res.Add("Scale (px/mm)", mFile.Scale.HasValue ? mFile.Scale.Value.ToString() : "0.0");
                mFile.SampleProperties = GetFilesColumns(sdFile.SDSample, sdFile.SDSample.SDEvent);

                var mSample = new Sample();
                mSample.Id = sdSample.sdSampleGuid;
                mFile.Sample = mSample;

               

                mSample.DisplayedProperties = GetSampleProperties(sdEvent, sdSample);

                bool isEventClosed = sdEvent.closed;
                bool hasEditAnnotationsRights = user.HasEditSDAnnotationsTask;

                //Add annotations and points/lines.
                if (getAnnotations == "True" && sdFile.SDAnnotation != null && sdFile.SDAnnotation.Count > 0)
                {
                    mFile.Annotations = new List<Annotation>();

                    foreach (var a in sdFile.SDAnnotation)
                    {
                        //If the event is open, only show the users own event. If the user has edit annotations rights, add all annotations. If annotation is fixed, make sure to add it, but in readonly mode, if it's another users.
                        if (!ShouldShowAnnotation(a, dfuPersonId, !isEventClosed, hasEditAnnotationsRights))
                            continue;

                        bool isUserAnnotation = a.createdById == dfuPersonId;
                        bool isFixed = a.isFixed != null && a.isFixed.Value;

                        var mAnnotation = new Annotation();
                        mAnnotation.Id = a.sdAnnotationGuid;
                        mAnnotation.LabTechnician = a.createdByUserName;
                        mAnnotation.IsApproved = a.isApproved ?? false;
                        mAnnotation.IsFixed = a.isFixed ?? false;
                        mAnnotation.IsReadOnly = isEventClosed || (a.isReadOnly ?? false) || (!isUserAnnotation && isFixed && !hasEditAnnotationsRights); //Make readonly if event is closed, if it is readonly, or if it is fixed but not created by the user and the user doesn't have edit annotatoin rights.
                        mAnnotation.FileId = mFile.ID;
                        mAnnotation.Result = a.SDPoint != null ? a.SDPoint.Count : 0;
                        mAnnotation.DateCreation = a.createdTime.HasValue ? a.createdTime.Value.ToLocalTime() : DateTime.Now;
                        mAnnotation.Comment = a.comment;
                        mAnnotation.QualityId = a.otolithReadingRemarkId.HasValue ? new Nullable<Guid>(IntToGuid(a.otolithReadingRemarkId.Value)) : null;
                        mAnnotation.ParameterId = a.sdAnalysisParameterId.HasValue ? new Nullable<Guid>(IntToGuid(a.sdAnalysisParameterId.Value)) : null;
                        mAnnotation.Edge = GetSDEdgeStructureCodeFromFishLineCode(a.edgeStructure);

                        mAnnotation.Lines = new List<Line>();
                        if (a.SDLine != null && a.SDLine.Count > 0)
                        {
                            foreach (var l in a.SDLine)
                            {
                                var line = new Line();
                                line.Id = l.sdLineGuid;
                                line.AnnotationId = mAnnotation.Id;
                                line.Color = l.color;
                                line.X1 = l.X1.HasValue ? l.X1.Value : 0;
                                line.X2 = l.X2.HasValue ? l.X2.Value : 0;
                                line.Y1 = l.Y1.HasValue ? l.Y1.Value : 0;
                                line.Y2 = l.Y2.HasValue ? l.Y2.Value : 0;
                                line.LineIndex = l.lineIndex.HasValue ? l.lineIndex.Value : 0;
                                line.Width = l.width.HasValue ? l.width.Value : 1;
                                mAnnotation.Lines.Add(line);
                            }
                        }

                        mAnnotation.Dots = new List<Dot>();
                        if (a.SDPoint != null && a.SDPoint.Count > 0)
                        {

                            foreach (var p in a.SDPoint)
                            {
                                var dot = new Dot();
                                dot.Id = p.sdPointGuid;
                                dot.AnnotationId = mAnnotation.Id;
                                dot.Color = p.color;
                                dot.DotShape = p.shape;
                                dot.DotType = p.pointType;
                                dot.X = p.X.HasValue ? p.X.Value : 0;
                                dot.Y = p.Y.HasValue ? p.Y.Value : 0;
                                dot.DotIndex = p.pointIndex.HasValue ? p.pointIndex.Value : 0;
                                dot.Width = p.width.HasValue ? p.width.Value : 1;
                                mAnnotation.Dots.Add(dot);
                            }
                        }

                        mFile.Annotations.Add(mAnnotation);
                    }
                }

                mFile.AnnotationCount = mFile.Annotations == null ? 0 : mFile.Annotations.Where(x => !x.IsFixed).Count();

                mSample.StatusCode = "To do";
                mSample.StatusColor = "#cc0000";
                mSample.StatusRank = 1;

                if (mFile.AnnotationCount > 0)
                {
                    if (mFile.Annotations.Where(x => x.IsApproved).Any())
                    {
                        mSample.StatusCode = "Done";
                        mSample.StatusColor = "#00b300";
                        mSample.StatusRank = 3;
                    }
                    else
                    {
                        mSample.StatusCode = "Work in progress";
                        mSample.StatusColor = "#ff8000";
                        mSample.StatusRank = 2;
                    }
                }

                return new WebApiResult() { Result = mFile };
            }
            catch(Exception e)
            {
                return new WebApiResult() { ErrorMessage = _unknownErrorMessage + e.Message };
            }
        }


        private bool ShouldShowAnnotation(FishLine.SDAnnotation a, int? dfuPersonId, bool isEventOpen, bool hasEditAnnotationsRights)
        {
            bool isUserAnnotation = a.createdById == dfuPersonId;
            bool isFixed = a.isFixed != null && a.isFixed.Value;

            //If the event is open, only show the users own event. If the user has edit annotations rights, add all annotations. If annotation is fixed, make sure to add it, but in readonly mode, if it's another users.
            if (!isUserAnnotation && !hasEditAnnotationsRights && isEventOpen && !isFixed)
                return false;

            return true;
        }


        private Dictionary<string, string> GetSampleProperties(FishLine.SDEvent e, FishLine.SDSample s)
        {
            var res = new Dictionary<string, string>();

            res.Add("Animal id", s.animalId);
            res.Add("Species", (e.speciesCode ?? "") + (e.L_Species == null || string.IsNullOrWhiteSpace(e.L_Species.dkName) ? "" : (" - " + e.L_Species.dkName)));
            res.Add("Catch date", s.catchDate.HasValue ? s.catchDate.Value.ToString("dd-MM-yyyy") : "-");
            res.Add("Length (mm)", s.fishLengthMM.HasValue ? s.fishLengthMM.Value.ToString() : "-");
            res.Add("Weight (g)", s.fishWeightG.HasValue ? s.fishWeightG.Value.ToString("0.###") : "-");
            res.Add("Area (Aqua, ICES)", s.L_DFUArea != null ? string.Format("{0}, {1}", s.L_DFUArea.DFUArea ?? "", s.L_DFUArea.areaICES ?? "") : "-");
            res.Add("Stock", s.L_Stock != null ? (s.L_Stock.stockCode ?? "-") : "-");
            res.Add("Statistical rectangle", s.statisticalRectangle ?? "-");
            res.Add("Sex", s.sexCode ?? "-");
            res.Add("Sample origin", GetSampleOrigin(s));
            res.Add("Sample type", e.L_SDSampleType != null ? e.L_SDSampleType.description : "-");
            res.Add("Maturity index method", s.maturityIndexMethod ?? "-");
            res.Add("Maturity", s.Maturity != null ? (s.Maturity.description ?? "-") : "-");
            res.Add("Preparation method", s.L_SDPreparationMethod != null ? (s.L_SDPreparationMethod.preparationMethod ?? "") + " - " + (s.L_SDPreparationMethod.ukDescription ?? "") : "-");
            res.Add("Otolith description", s.L_SDOtolithDescription != null ? (s.L_SDOtolithDescription.otolithDescription ?? "") + " - " + (s.L_SDOtolithDescription.ukDescription ?? "") : "-");
            res.Add("Light type", s.L_SDLightType != null ? (s.L_SDLightType.lightType ?? "") + " - " + (s.L_SDLightType.ukDescription ?? "") : "-");
            res.Add("Comments", string.IsNullOrWhiteSpace(s.comments) ? "-" : s.comments);

            return res;
        }


        /// <summary>
        /// Get the extra columns needed for Files table.
        /// </summary>
        private Dictionary<string, string> GetFilesColumns(FishLine.SDSample sampl, FishLine.SDEvent evt)
        {
            var res = new Dictionary<string, string>();

            try
            {
                List<Babelfisk.Entities.SDFilesExtraColumn> lst = null;

                if (evt != null && (lst = evt.SDFileExtraColumns) != null && lst.Count > 0)
                {
                    foreach (var c in lst)
                    {
                        switch (c)
                        {
                            case Babelfisk.Entities.SDFilesExtraColumn.CatchDate:
                                res.Add("Catch date", sampl.catchDate.HasValue ? sampl.catchDate.Value.ToString("dd-MM-yyyy") : "");
                                break;

                            case Babelfisk.Entities.SDFilesExtraColumn.Length:
                                res.Add("Length (mm)", sampl.fishLengthMM.HasValue ? sampl.fishLengthMM.Value.ToString() : "");
                                break;

                            case Babelfisk.Entities.SDFilesExtraColumn.Area:
                                res.Add("Area (Aqua, ICES)", sampl.L_DFUArea != null ? string.Format("{0}, {1}", sampl.L_DFUArea.DFUArea ?? "", sampl.L_DFUArea.areaICES ?? "") : "");
                                break;

                            case Babelfisk.Entities.SDFilesExtraColumn.Stock:
                                res.Add("Stock", sampl.L_Stock != null ? (sampl.L_Stock.stockCode ?? "") : "");
                                break;

                            case Babelfisk.Entities.SDFilesExtraColumn.StatisticalRectangle:
                                res.Add("Statistical rectangle", sampl.statisticalRectangle ?? "");
                                break;

                            case Babelfisk.Entities.SDFilesExtraColumn.Sex:
                                res.Add("Sex", sampl.sexCode ?? "");
                                break;

                            case Babelfisk.Entities.SDFilesExtraColumn.SampleOrigin:
                                res.Add("Sample origin", GetSampleOrigin(sampl));
                                break;

                            case Babelfisk.Entities.SDFilesExtraColumn.Maturity:
                                res.Add("Maturity", sampl.Maturity != null ? (sampl.Maturity.description ?? "") : "");
                                break;

                            case Babelfisk.Entities.SDFilesExtraColumn.PreperationMethod:
                                res.Add("Preparation method", sampl.L_SDPreparationMethod != null ? (sampl.L_SDPreparationMethod.preparationMethod ?? "") + " - " + (sampl.L_SDPreparationMethod.ukDescription ?? "") : "");
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return res;
        }



        /// <summary>
        /// Update file with scale
        /// </summary>
        public WebApiResult UpdateFile(Entities.File file)
        {
            try
            {
                var req = HttpContext.Current.Request.Params["token"];

                if (!ValidateToken(req))
                    return new WebApiResult() { ErrorMessage = _tokenInvalidErrorMessage };

                FishLine.SDFile sdFile = null;

                using (var ctx = new FishLine.SprattusContainer())
                {
                    sdFile = ctx.SDFile.Where(x => x.sdFileGuid == file.ID).FirstOrDefault();

                    if(sdFile != null)
                    {
                        var dblScale = DecimalToDouble(file.Scale);
                        if (sdFile.scale != DecimalToDouble(file.Scale))
                            sdFile.scale = dblScale;

                        FishLine.SelfTrackingEntitiesContextExtensions.ApplyChanges(ctx.SDFile, sdFile);

                        ctx.SaveChangesAndHandleOptimisticConcurrency(Babelfisk.Entities.OverwritingMethod.ClientWins);
                    }
                }

                if (sdFile == null)
                    return new WebApiResult() { ErrorMessage = "Failed to update file information." };
            }
            catch (Exception e)
            {
                return new WebApiResult() { ErrorMessage =  _unknownErrorMessage + e.Message };
            }

            return new WebApiResult() { Result = true };
        }


        private double? DecimalToDouble(Decimal? dec)
        {
            if (dec == null)
                return null;

            return (double)dec.Value;
        }



        private static List<Annotation> _lstAnnotations = new List<Annotation>();



        /// <summary>
        /// Add annotation
        /// </summary>
        public WebApiResult AddAnnotation(Annotation annotation)
        {
            try
            {
                var req = HttpContext.Current.Request.Params["token"];

                string userName = null;
                int userId = -1, dfuPersonId = -1;
                if (!ValidateToken(req, ref userName, ref userId, ref dfuPersonId))
                    return new WebApiResult() { ErrorMessage = _tokenInvalidErrorMessage };

                Guid fileId = annotation.FileId;

                FishLine.SDFile sdFile = null;

                using (var ctx = new FishLine.SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    sdFile = ctx.SDFile.Where(x => x.sdFileGuid == fileId).FirstOrDefault();

                    if(sdFile != null)
                    {
                        var annoNew = new FishLine.SDAnnotation();
                        annoNew.sdFileId = sdFile.sdFileId;
                        annoNew.sdAnnotationGuid = annotation.Id;
                        annoNew.isFixed = annotation.IsFixed;
                        annoNew.isReadOnly = annotation.IsReadOnly;
                        annoNew.isApproved = annotation.IsApproved;
                        annoNew.createdById = dfuPersonId;
                        annoNew.createdByUserName = userName;
                        annoNew.createdTime = annotation.DateCreation.ToUniversalTime();
                        annoNew.modifiedTime = annoNew.createdTime;
                        annoNew.otolithReadingRemarkId = annotation.QualityId.HasValue ? GuidToInt(annotation.QualityId.Value) : null;
                        annoNew.sdAnalysisParameterId = annotation.ParameterId.HasValue ? GuidToInt(annotation.ParameterId.Value) : null;
                        annoNew.comment = annotation.Comment;
                        annoNew.age = annotation.Result;

                        SyncSDLines(ctx, userName, dfuPersonId, annoNew, annotation);
                        SyncSDDots(ctx, userName, dfuPersonId, annoNew, annotation);

                        ctx.SDAnnotation.AddObject(annoNew);

                        ctx.SaveChangesAndHandleOptimisticConcurrency(Babelfisk.Entities.OverwritingMethod.ClientWins);
                    }
                }

                if (sdFile == null)
                    return new WebApiResult() { ErrorMessage = "Failed to add new annotation to database." };
            }
            catch (Exception e)
            {
                return new WebApiResult() { ErrorMessage = _unknownErrorMessage + e.Message };
            }

            return new WebApiResult() { Result = true };
        }


        /// <summary>
        /// Retrieve edgestructure code from SmartDots edgestructure code.
        /// </summary>
        private static string GetFishLineEdgeStructureCodeFromSDCode(List<FishLine.L_EdgeStructure> lstFFEdgesStructures, string SDEdgeStructureCode)
        {
            if (lstFFEdgesStructures == null || string.IsNullOrWhiteSpace(SDEdgeStructureCode) || SDEdgeStructureCode.Equals("NA", StringComparison.InvariantCultureIgnoreCase))
                return null;

            FishLine.L_EdgeStructure es = null;
            switch (SDEdgeStructureCode.ToLower().Trim())
            {
                case "opaque":
                    if ((es = lstFFEdgesStructures.Where(x => x.edgeStructure.Equals("O", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()) != null)
                        return es.edgeStructure;
                    break;

                case "translucent":
                    if ((es = lstFFEdgesStructures.Where(x => x.edgeStructure.Equals("T", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()) != null)
                        return es.edgeStructure;
                    break;
            }

            return null;
        }

        private static string GetSDEdgeStructureCodeFromFishLineCode(string fishlineEdgestructureCode)
        {
            if (string.IsNullOrWhiteSpace(fishlineEdgestructureCode))
                return null;

            switch(fishlineEdgestructureCode.ToLower().Trim())
            {
                case "o":
                    return "Opaque";

                case "t":
                    return "Translucent";
            }

            return null;
        }


        /// <summary>
        /// Update annotations
        /// </summary>
        public WebApiResult UpdateAnnotations(List<Annotation> annotations)
        {
            try
            {
                var req = HttpContext.Current.Request.Params["token"];

                string userName = null;
                int userId = -1, dfuPersonId = -1;
                if (!ValidateToken(req, ref userName, ref userId, ref dfuPersonId))
                    return new WebApiResult() { ErrorMessage = _tokenInvalidErrorMessage };

                bool updated = false;

                using (var ctx = new FishLine.SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    var lstApprovedAnnotationsWithNoAQScore = annotations.Where(x => x.IsApproved && !x.QualityId.HasValue).ToList();

                    //If one or more approved annotation does not have a quality id, return error. 
                    if(lstApprovedAnnotationsWithNoAQScore.Count > 0)
                    {
                        return new WebApiResult() { ErrorMessage = "Please make sure all approved annotations have AQ scores before saving them." };
                    }
                    
                    var otolithReadingRemarkLookups = ctx.L_OtolithReadingRemark.ToList();
                    
                    Dictionary<string, FishLine.DFUPerson> dicDFUPersons = null;                
                    var animalIdsAndAges = new List<Babelfisk.Entities.SDAnimalAgeItem>();
                    foreach (var annotation in annotations)
                    {
                        var edgeStructures = ctx.L_EdgeStructure.ToList();

                        var sdAnno = ctx.SDAnnotation
                                        .Include("SDLine")
                                        .Include("SDPoint")
                                        .Where(x => x.sdAnnotationGuid == annotation.Id).FirstOrDefault();

                        var evt = ctx.SDAnnotation.Where(x => x.sdAnnotationGuid == annotation.Id).Select(x => new { SDEvent = x.SDFile.SDSample.SDEvent, SDSample = x.SDFile.SDSample, SDFile = x.SDFile }).FirstOrDefault();

                        
                        if (sdAnno != null && evt != null && evt.SDEvent != null)
                        {
                            sdAnno.isFixed = annotation.IsFixed;
                            sdAnno.isReadOnly = annotation.IsReadOnly;
                            sdAnno.isApproved = annotation.IsApproved;

                            //If either createByUserName or createdById is missing, assign them.
                            if (!string.IsNullOrWhiteSpace(annotation.LabTechnician) && (!sdAnno.createdById.HasValue || string.IsNullOrWhiteSpace(sdAnno.createdByUserName)))
                            {
                                if (dicDFUPersons == null)
                                {
                                    dicDFUPersons = ctx.DFUPerson.Where(x => x.initials != null)
                                                                 .ToList()
                                                                 .DistinctBy(x => x.initials.ToLower())
                                                                 .ToDictionary(x => x.initials.ToLower());
                                }

                                if (dicDFUPersons.ContainsKey(annotation.LabTechnician.ToLower()))
                                {
                                    sdAnno.createdById = dicDFUPersons[annotation.LabTechnician.ToLower()].dfuPersonId;
                                    sdAnno.createdByUserName = dicDFUPersons[annotation.LabTechnician.ToLower()].initials;
                                }
                                else
                                    sdAnno.createdByUserName = annotation.LabTechnician;
                            }

                            sdAnno.modifiedTime = DateTime.UtcNow;
                            sdAnno.otolithReadingRemarkId = annotation.QualityId.HasValue ? GuidToInt(annotation.QualityId.Value) : null;
                            sdAnno.sdAnalysisParameterId = annotation.ParameterId.HasValue ? GuidToInt(annotation.ParameterId.Value) : null;
                            sdAnno.comment = annotation.Comment;
                            sdAnno.age = annotation.Result;
                            sdAnno.edgeStructure = GetFishLineEdgeStructureCodeFromSDCode(edgeStructures, annotation.Edge);

                            SyncSDLines(ctx, userName, dfuPersonId, sdAnno, annotation);
                            SyncSDDots(ctx, userName, dfuPersonId, sdAnno, annotation);

                            FishLine.SelfTrackingEntitiesContextExtensions.ApplyChanges(ctx.SDAnnotation, sdAnno);

                            var currentOtolithReadingRemark = (otolithReadingRemarkLookups != null && sdAnno.otolithReadingRemarkId != null) ? otolithReadingRemarkLookups.Where(x => x.Id == sdAnno.otolithReadingRemarkId.ToString()).FirstOrDefault() : null;

                            //Add animals to update FishLine with.
                            int animalId = 0;
                            if(evt.SDEvent.IsYearlyReadingEventType && evt.SDSample != null && annotation.Result >= 0 && evt.SDSample.animalId.TryParseInt32(out animalId) && annotation.IsApproved)
                            {
                                var itm = new Babelfisk.Entities.SDAnimalAgeItem();
                                itm.AnimalId = animalId;
                                itm.SDSampleId = evt.SDSample.sdSampleId;

                                if (currentOtolithReadingRemark != null && currentOtolithReadingRemark.transAgeFromAquaDotsToFishLine)
                                    itm.ShouldAssignAge = true;

                                itm.Age = annotation.Result;
                                itm.OtolithReadingRemarkId = sdAnno.otolithReadingRemarkId;
                                itm.EdgeStructureCode = sdAnno.edgeStructure;
                                itm.SDAnnotationId = sdAnno.sdAnnotationId;
                                itm.DFUPersonReaderId = dfuPersonId;
                                animalIdsAndAges.Add(itm);
                            }
                        }
                    }

                    ctx.SaveChangesAndHandleOptimisticConcurrency(Babelfisk.Entities.OverwritingMethod.ClientWins);
                    updated = true;

                    //Update ages in FishLine as well.
                    if(animalIdsAndAges.Count > 0)
                    {
                        var bs = new BabelfiskService();
                        var res = bs.CopyAgesToFishLine(animalIdsAndAges);

                        if(res.DatabaseOperationStatus != Babelfisk.Entities.DatabaseOperationStatus.Successful)
                            return new WebApiResult() { ErrorMessage = "The annotation(s) was saved, but an error occurred when updating the age(s) in FishLine. " + (res.UIMessage ?? res.Message) };
                        else
                        {
                            int skippedDueToLavRepCount = 0;
                            int tmp;
                            if (res.Properties != null && res.Properties.ContainsKey("SkippedDueToLavRepCount") && res.Properties["SkippedDueToLavRepCount"].TryParseInt32(out tmp))
                                skippedDueToLavRepCount = tmp;

                            //If any animals were skipped, make sure to notify why.
                            if (skippedDueToLavRepCount > 0)
                            {
                                string msg = string.Format("The annotation(s) was saved successfully but the age was not transferred to FishLine for {0} of the approved annotation(s), since it/they are not single fish animal(s) in FishLine (but LAV rep).", skippedDueToLavRepCount);
                               // return new WebApiResult() { ErrorMessage = msg };
                               //Dont show error message, since it will make the UI not update with the saved annotations.
                            }
                        }
                    }
                }

                if (!updated)
                    return new WebApiResult() { ErrorMessage = "Failed to update annotations" };


            }
            catch (Exception e)
            {
                return new WebApiResult() { ErrorMessage = _unknownErrorMessage + e.Message };
            }

            return new WebApiResult() { Result = true };
        }


       


        private void SyncSDLines(FishLine.SprattusContainer ctx, string userName, int? dfuPersonId, FishLine.SDAnnotation sdAnno, Annotation anno)
        {
            //Remove lines that are not in the incoming entity.
            var linesToAdd = (anno.Lines ?? new List<Line>()).ToList();
            var linesToRemove = sdAnno.SDLine.Where(x => !linesToAdd.Any(y => y.Id == x.sdLineGuid)).ToList();
            foreach (var a in linesToRemove)
            {
                ctx.DeleteObject(a);
                sdAnno.SDLine.Remove(a);
            }

            //If there are any to add, get them fresh from the DB and add them.
            if (linesToAdd.Count > 0)
            {
                foreach(var l in linesToAdd)
                {
                    var lEdit = sdAnno.SDLine.Where(y => y.sdLineGuid == l.Id).FirstOrDefault();

                    //If line does not exist, create a new one. Otherwise just update the existing one.
                    if (lEdit == null)
                    {
                        lEdit = new FishLine.SDLine();
                        sdAnno.SDLine.Add(lEdit);
                    }

                    //Make sure to check for changes, so nothing is updated, if there are no changes.
                    if(lEdit.sdLineGuid != l.Id)
                        lEdit.sdLineGuid = l.Id;

                    if (lEdit.color != l.Color)
                        lEdit.color = l.Color;

                    if(!lEdit.createdTime.HasValue)
                        lEdit.createdTime = DateTime.UtcNow;

                    if (string.IsNullOrWhiteSpace(lEdit.createdByUserName))
                        lEdit.createdByUserName = userName;

                    if (!lEdit.createdById.HasValue)
                        lEdit.createdById = dfuPersonId;

                    if (lEdit.lineIndex != l.LineIndex)
                        lEdit.lineIndex = l.LineIndex;

                    if (lEdit.width != l.Width)
                        lEdit.width = l.Width;

                    if (lEdit.X1 != l.X1)
                        lEdit.X1 = l.X1;

                    if (lEdit.X2 != l.X2)
                        lEdit.X2 = l.X2;

                    if (lEdit.Y1 != l.Y1)
                        lEdit.Y1 = l.Y1;

                    if (lEdit.Y2 != l.Y2)
                        lEdit.Y2 = l.Y2;
                }
            }
        }


        private void SyncSDDots(FishLine.SprattusContainer ctx, string userName, int? dfuPersonId, FishLine.SDAnnotation sdAnno, Annotation anno)
        {
            //Remove dots that are not in the incoming entity.
            var dotsToAdd = (anno.Dots ?? new List<Dot>()).ToList();
            var dotsToRemove = sdAnno.SDPoint.Where(x => !dotsToAdd.Any(y => y.Id == x.sdPointGuid)).ToList();
            foreach (var a in dotsToRemove)
            {
                ctx.DeleteObject(a);
                sdAnno.SDPoint.Remove(a);
            }

            //Get new areas that are not already added.
            var lstAdd = dotsToAdd.Where(x => !sdAnno.SDPoint.Any(y => y.sdPointGuid == x.Id)).ToList();

            //If there are any to add, get them fresh from the DB and add them.
            if (dotsToAdd.Count > 0)
            {
                foreach (var p in dotsToAdd)
                {
                    var pEdit = sdAnno.SDPoint.Where(y => y.sdPointGuid == p.Id).FirstOrDefault();

                    //If point does not exist, create a new one. Otherwise just update the existing one.
                    if (pEdit == null)
                    {
                        pEdit = new FishLine.SDPoint();
                        sdAnno.SDPoint.Add(pEdit);
                    }

                    //Make sure to check for changes, so nothing is updated, if there are no changes.
                    if (pEdit.sdPointGuid != p.Id)
                        pEdit.sdPointGuid = p.Id;

                    if (pEdit.color != p.Color)
                        pEdit.color = p.Color;

                    if (!pEdit.createdTime.HasValue)
                        pEdit.createdTime = DateTime.UtcNow;

                    if (string.IsNullOrWhiteSpace(pEdit.createdByUserName))
                        pEdit.createdByUserName = userName;

                    if (!pEdit.createdById.HasValue)
                        pEdit.createdById = dfuPersonId;

                    if (pEdit.pointIndex != p.DotIndex)
                        pEdit.pointIndex = p.DotIndex;

                    if (pEdit.pointType != p.DotType)
                        pEdit.pointType = p.DotType;

                    if (pEdit.width != p.Width)
                        pEdit.width = p.Width;

                    if (pEdit.shape != p.DotShape)
                        pEdit.shape = p.DotShape;

                    if (pEdit.X != p.X)
                        pEdit.X = p.X;

                    if (pEdit.Y != p.Y)
                        pEdit.Y = p.Y;
                }
            }
        }


        /// <summary>
        /// Delete annotations
        /// </summary>
        public WebApiResult DeleteAnnotations(List<Guid> ids)
        {
            try
            {
                var req = HttpContext.Current.Request.Params["token"];

                if (!ValidateToken(req))
                    return new WebApiResult() { ErrorMessage = _tokenInvalidErrorMessage };

                bool updated = false;
                using (var ctx = new FishLine.SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    foreach (var aguid in ids)
                    {
                        var sdAnno = ctx.SDAnnotation
                                        .Include("SDLine")
                                        .Include("SDPoint")
                                         .Include("FishLineAge")
                                        .Where(x => x.sdAnnotationGuid == aguid).FirstOrDefault();

                        if (sdAnno != null)
                        {
                            foreach (var l in sdAnno.SDLine.ToList())
                                ctx.DeleteObject(l);

                            foreach (var p in sdAnno.SDPoint.ToList())
                                ctx.DeleteObject(p);

                            ctx.DeleteObject(sdAnno);
                        }
                    }

                    ctx.SaveChangesAndHandleOptimisticConcurrency(Babelfisk.Entities.OverwritingMethod.ClientWins);
                    updated = true;
                }

                if (!updated)
                    return new WebApiResult() { ErrorMessage = "Failed to delete annotations" };
            }
            catch (Exception e)
            {
                return new WebApiResult() { ErrorMessage = _unknownErrorMessage + e.Message };
            }

            return new WebApiResult() { Result = true };
        }
    }
}