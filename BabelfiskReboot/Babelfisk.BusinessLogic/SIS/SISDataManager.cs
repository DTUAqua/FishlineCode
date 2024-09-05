using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.BusinessLogic.SIS.Model;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.BusinessLogic.SIS
{
    public class SISDataManager
    {
        private string _strConnectionString;


        /// <summary>
        /// Create an instance of SISDataManager.
        /// </summary>
        /// <param name="strConnectionString"></param>
        public SISDataManager(string strConnectionString)
        {
            _strConnectionString = strConnectionString;
        }


        /// <summary>
        /// Retrieve a list of all cruise years in DanaDB
        /// </summary>
        public List<int> GetCruiseYears()
        {
            List<int> lst = new List<int>();

            try
            {
                using (SIS.Model.DanaDBContext ctx = new Model.DanaDBContext(_strConnectionString))
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    var y = ctx.CruiseInformation.Select(x => x.cruiseYear).Distinct();

                    lst = y.ToList();
                }
            }
            catch (Exception e)
            {
                //Log exception
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return lst;
        }


        /// <summary>
        /// Retrieve a list of CruiseInformation records for a given year.
        /// </summary>
        public List<CruiseInformation> GetCruiseInformations(int intYear)
        {
            List<CruiseInformation> lst = new List<CruiseInformation>();

            try
            {
                using (SIS.Model.DanaDBContext ctx = new Model.DanaDBContext(_strConnectionString))
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    var q = ctx.CruiseInformation.Where(x => x.cruiseYear == intYear);

                    lst = q.ToList();
                }
            }
            catch (Exception e)
            {
                //Log exception
                Anchor.Core.Loggers.Logger.LogError(e, String.Format("SISDataManager.GetCruiseInformations({0})", intYear));
            }

            return lst;
        }


        /// <summary>
        /// Retrieve a list of GearData records for a specific cruise id.
        /// </summary>
        public List<GearData> GetGearData(int intCruiseInformationId)
        {
            List<GearData> lst = new List<GearData>();

            try
            {
                using (SIS.Model.DanaDBContext ctx = new Model.DanaDBContext(_strConnectionString))
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    var q = ctx.GearData.Where(x => x.cruiseID == intCruiseInformationId);

                    lst = q.ToList();
                }
            }
            catch (Exception e)
            {
                //Log exception
                Anchor.Core.Loggers.Logger.LogError(e, String.Format("SISDataManager.GetGearData({0})", intCruiseInformationId));
            }

            return lst;
        }



        public Sample GetStation(int intGearDataId)
        {
            Sample s = null;

            string strSQL = String.Format(GetSISStationSQL, intGearDataId);

            try
            {
                using (SIS.Model.DanaDBContext ctx = new Model.DanaDBContext(_strConnectionString))
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    var res = ctx.ExecuteStoreQuery<Sample>(strSQL);

                    s = res.FirstOrDefault();

                    //Set default values
                    if (s != null)
                    {
                        s.@virtual = "nej";
                        s.sampleType = "A";
                        s.gearQuality = "V";

                        s.remark = string.IsNullOrWhiteSpace(s.remark) ? "" : s.remark + ". ";
                        s.remark += "Stationen er overført fra SIS.";

                        decimal? decBottomSpeedMean = CalculateShipDataAvg(ctx, intGearDataId, "speedBot");

                        if (decBottomSpeedMean.HasValue)
                            s.haulSpeedBot = decBottomSpeedMean;

                        decimal? decDepthMean = CalculateShipDataAvg(ctx, intGearDataId, "botDep");

                        if (decDepthMean.HasValue)
                            s.depthAvg = decDepthMean;

                        //It is the right scanmar values used for calculating the Avg below for shovelDist and netOpening. However
                        //the values vary to much during the haul, that it is better to use what they entered on the bridge (which goes into gearData)
                       /* decimal? decShovalDistance = CalculateTrawlDataAvg(ctx, intGearDataId, "DST");

                        if (decShovalDistance.HasValue)
                            s.shovelDist = Math.Round(decShovalDistance.Value, 1);

                        decimal? decNetOpening = CalculateTrawlDataAvg(ctx, intGearDataId, "TS-O");

                        if (decNetOpening.HasValue)
                            s.netOpening = Math.Round(decNetOpening.Value, 1);*/
                    }
                }
            }
            catch (Exception e)
            {
                //Log exception
                Anchor.Core.Loggers.Logger.LogError(e, String.Format("SISDataManager.GetStation({0})", intGearDataId));
            }

            return s;
        }


        /// <summary>
        /// Calculate average on ship data with param code strParamCode.
        /// </summary>
        private decimal? CalculateShipDataAvg(SIS.Model.DanaDBContext ctx, int intGearDataId, string strParamCode = "speedBot")
        {
            decimal? decMean = null;

            try
            {
                var gearData = ctx.GearData.Where(gd => gd.gearDataID == intGearDataId).FirstOrDefault();

                //Don't calculate bottom speed mean if geardata was not found or timespan is null.
                if (gearData == null || !gearData.timeStart.HasValue || !gearData.timeStop.HasValue)
                    return decMean;

                int intCruiseId = gearData.cruiseID;

                var q = from sl in ctx.SampleLog
                        join sd in ctx.ShipData on sl.sampleID equals sd.sampleID
                        where sl.cruiseID == intCruiseId && sl.logTime >= gearData.timeStart && sl.logTime <= gearData.timeStop &&
                              sd.ParamCode == strParamCode && sd.Result.HasValue
                        select sd.Result;

                float? res = q.Average();

                if(res.HasValue)
                    decMean = (decimal)Math.Round(res.Value, 1);
            }
            catch (Exception e)
            {
                //Log exception
                Anchor.Core.Loggers.Logger.LogError(e, String.Format("SISDataManager.CalculateShipDataAvg({0})", intGearDataId));
            }

            return decMean;
        }


        /// <summary>
        /// Calculate average on TrawlData with param code strParamCode.
        /// </summary>
        private decimal? CalculateTrawlDataAvg(SIS.Model.DanaDBContext ctx, int intGearDataId, string strParamCode)
        {
            decimal? decMean = null;

            try
            {
                var gearData = ctx.GearData.Where(gd => gd.gearDataID == intGearDataId).FirstOrDefault();

                //Don't calculate bottom speed mean if geardata was not found or timespan is null.
                if (gearData == null || !gearData.timeStart.HasValue || !gearData.timeStop.HasValue)
                    return decMean;

                int intCruiseId = gearData.cruiseID;

                var q = from tl in ctx.TrawlLog
                        join td in ctx.TrawlData on tl.sampleID equals td.sampleID
                        where tl.cruiseID == intCruiseId && tl.logTime >= gearData.timeStart && tl.logTime <= gearData.timeStop &&
                              td.paramCode == strParamCode && td.result.HasValue
                        select td.result;

                float? res = q.Average();

                if (res.HasValue)
                    decMean = (decimal)res.Value;
            }
            catch (Exception e)
            {
                //Log exception
                Anchor.Core.Loggers.Logger.LogError(e, String.Format("SISDataManager.CalculateShipDataAvg({0})", intGearDataId));
            }

            return decMean;
        }


        private string GetSISStationSQL
        {
            get
            {
                string str = @"
                                SELECT 
                                    gd.timeStart As 'dateGearStart', 
                                    gd.timeStop As 'dateGearEnd',
	                                ((LEFT(gd.posLatStart, (LEN(gd.posLatStart)-1)))+' '+(RIGHT(gd.posLatStart, 1))) As 'latPosStartText', 
                                    ((LEFT(gd.posLonStart, (LEN(gd.posLonStart)-1)))+' '+(RIGHT(gd.posLonStart, 1))) As 'lonPosStartText', 
                                    ((LEFT(gd.posLatStop, (LEN(gd.posLatStop)-1)))+' '+(RIGHT(gd.posLatStop, 1))) As 'latPosEndText', 
                                    ((LEFT(gd.posLonStop, (LEN(gd.posLonStop)-1)))+' '+(RIGHT(gd.posLonStop, 1))) As 'lonPosEndText',
		                            CAST(((gd.botDepStart+gd.botDepStop)/2) AS numeric(5,1)) As 'depthAvg', 
                                    CAST(CAST(gd.windDir AS numeric) AS SmallInt) as 'windDirection', 
                                    CAST(CAST(gd.windSpeed AS numeric) As Int) as 'windSpeed',
		                            CAST((gd.speedBotStart+gd.speedBotStop)/2 AS numeric(3,1)) AS 'haulSpeedBot', 
                                    CAST(CAST(gd.wireLength AS numeric) AS int) as 'wireLength',
                                    CAST(gearDur AS Int) As 'fishingtime',
                                    CAST(((courseStart+courseStop)/2) AS SmallInt) As 'haulDirection',
		                            CAST(gd.trawlSlack AS numeric(5,1)) as 'shovelDist', 
                                    CAST(gd.gearDepth AS numeric(5,1)) as 'depthAveGear', 
                                    CAST(gd.trawlGap AS numeric(3,1)) as 'netOpening',
                                    CAST(CAST(gd.remarks AS varchar) AS nvarchar(450)) as 'remark'
	                            FROM danadb.dbo.CruiseInformation ci 
                                INNER JOIN danadb.dbo.GearData gd ON ci.cruiseID = gd.cruiseID 
                                INNER JOIN danadb.dbo.L_GearCodeList gcl ON gd.gearCode = gcl.gearCode
	                            WHERE (gd.gearDataId = {0})
                              ";

                return str;
            }
        }
    }
}
