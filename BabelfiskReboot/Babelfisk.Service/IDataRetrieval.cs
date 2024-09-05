using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.Service
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    interface IDataRetrieval
    {
        [OperationContract]
        List<L_Year> GetTreeViewYears();

        [OperationContract]
        int GetTreeViewTripCount(int intCruiseId, string strTripType);

        [OperationContract]
        Dictionary<int, int> GetTreeViewTripCounts(int[] CruiseIds, string strTripType);

        [OperationContract]
        List<Cruise> GetTreeViewCruises(int intYear, string strTripType);

        [OperationContract]
        List<Trip> GetTreeViewTrips(int intCruiseId);

        [OperationContract]
        List<Sample> GetTreeViewSamples(int intTripId);

        [OperationContract]
        List<SpeciesList> GetSpeciesLists(int intSampleId);

        [OperationContract]
        List<Animal> GetAnimals(int intSubSampleId);

        [OperationContract]
        List<Age> GetAges(int intSubSampleId);

        [OperationContract]
        List<AnimalFile> GetAnimalFiles(int intSubSampleId);

        [OperationContract]
        List<AnimalInfo> GetAnimalInfos(int intSubSampleId);

        [OperationContract]
        List<R_AnimalInfoReference> GetAnimalInfoReferences(int intSubSampleId);

        [OperationContract]
        byte[] GetDataForRaising(List<int> lstCruiseIds, List<int> lstTripIds, List<int> lstSampleIds);


        [OperationContract]
        Cruise GetCruise(int year, string cruiseName);

        [OperationContract]
        byte[] GetStationsForDataImport(int cruiseId, string trip, List<string> stations);
    }
}
