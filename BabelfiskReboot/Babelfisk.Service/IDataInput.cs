using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.Service
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    [ServiceKnownType(typeof(MapPoint))]
    public interface IDataInput
    {
        [OperationContract]
        List<L_Year> GetYears();

        [OperationContract]
        List<string> GetCruiseNames();

        [OperationContract]
        Cruise CreateAndGetCruise(int cruiseYear, string cruiseName);

        [OperationContract]
        Cruise GetCruiseFromId(int intCruiseId);

        [OperationContract]
        Trip GetTripFromId(int intTripId, string[] arrIncludes);

        [OperationContract]
        Person GetLatestPersonFromPlatformId(int intPlatformId);

        [OperationContract]
        Sample GetSampleFromId(int intSampleId);

        [OperationContract]
        Sample GetLatestSample(int intTripId);

        [OperationContract]
        List<Sample> GetSamplesFromTripId(int intTripId);

        [OperationContract]
        SpeciesList GetSpeciesListFromId(int intSpeciesListId, string[] arrIncludes);

        [OperationContract]
        SubSample GetSubSampleFromId(int intSubSampleId);

        [OperationContract]
        DatabaseOperationResult SaveCruise(ref Cruise c);

        [OperationContract]
        DatabaseOperationResult SaveTrip(ref Trip t);

        [OperationContract]
        DatabaseOperationResult SaveSample(ref Sample s);

        [OperationContract]
        DatabaseOperationResult SaveHVN(ref Trip t, ref Sample s);

        [OperationContract]
        List<L_StatisticalRectangle> GetStatisticalRectangleFromArea(string strAreaCode);

        [OperationContract]
        List<L_SelectionDevice> GetSelectionDevicesFromGearType(string strGearType);

        [OperationContract]
        List<MapPoint> GetMapPositionsFromCruiseId(int intCruiseId);

        [OperationContract]
        List<MapPoint> GetMapPositionsFromTripId(int intTripId);

        [OperationContract]
        List<MapPoint> GetMapPositionsFromSampleId(int intSampleId);

        [OperationContract]
        DatabaseOperationResult SaveSpeciesListItems(ref List<SpeciesList> speciesListItems);

        [OperationContract]
        DatabaseOperationResult SaveLavSFItems(SpeciesList sl, List<LavSFTransferItem> lstItems);

        [OperationContract]
        DatabaseOperationResult SaveCruiseToDataWarehouse(byte[] arrCruise, byte[] arrMessages, bool blnDeleteCruiseBeforeInsert, ref List<Babelfisk.Warehouse.DWMessage> lstMessagesNew);

        [OperationContract]
        DatabaseOperationResult DeleteCruise(int intCruiseId);

        [OperationContract]
        DatabaseOperationResult DeleteTrip(int intTripId);

        [OperationContract]
        DatabaseOperationResult DeleteSample(int intSampleId);
        
        [OperationContract]
        DatabaseOperationResult RunFileSynchronizer();

    }
}
