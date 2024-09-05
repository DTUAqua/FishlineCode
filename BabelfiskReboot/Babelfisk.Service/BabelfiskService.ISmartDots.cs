using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Babelfisk.Entities;
using System.Data;
using System.Transactions;
using System.Dynamic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Babelfisk.Entities.FileEntities;
using System.Security.Cryptography;


namespace Babelfisk.Service
{
    public partial class BabelfiskService : ISmartDots
    {
      

        public byte[] GetSDEventsCompressed()
        {
            var lst = GetSDEvents();

            if (lst == null)
                return null;

            var ba = lst.ToByteArrayDataContract();
            var cba = ba.Compress();

            return cba;
        }


        public List<SDEvent> GetSDEvents()
        {
            try
            {
                //SynchronizeStockAreaSpecies();
                using (var ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    var sl = (from e in ctx.SDEvent
                                           .Include("L_SDEventType")
                                           .Include("L_SDPurpose")
                                           .Include("L_Species")
                                           .Include("L_DFUAreas")
                                           .Include("L_SDSampleType")
                                           .Include("SDReaders")
                                           .Include("SDReaders.SDReader")
                                           .Include("SDReaders.SDReader.DFUPerson")
                              let sc = e.SDSample.Count()
                              select new { Event = e, SamplesCount = sc, 
                                           EventType = e.L_SDEventType,
                                           Purpose = e.L_SDPurpose, 
                                           Species = e.L_Species, 
                                           Areas = e.L_DFUAreas, 
                                           SampleType = e.L_SDSampleType, 
                                           SDReaders = e.SDReaders, 
                                           R_SDReaders = e.SDReaders.Select(x => x.SDReader), 
                                           SDReaderPersons = e.SDReaders.Select(x => x.SDReader.DFUPerson),
                                           SDReaderStocks = e.SDReaders.Select(x => x.SDReader.L_Stock),
                                           SDReaderExperience = e.SDReaders.Select(x => x.SDReader.L_SDReaderExperience),
                                           SDReaderPreparationMethod = e.SDReaders.Select(x => x.SDReader.L_SDPreparationMethod)
                              }
                              );

                    List<SDEvent> lst = new List<SDEvent>();
                    foreach (var e in sl)
                    {
                        e.Event.ChangeTracker.ChangeTrackingEnabled = false;
                        e.Event.SamplesCount = e.SamplesCount;
                        e.Event.L_DFUAreas = e.Areas;
                        e.Event.SDReaders = e.SDReaders;
                        e.Event.ChangeTracker.ChangeTrackingEnabled = true;
                        e.Event.AcceptChanges();
                        lst.Add(e.Event);
                    }

                    return lst;
                }
            }
            catch (Exception e)
            {
                try { Anchor.Core.Loggers.Logger.LogError(e); }
                catch { }
            }

            return null;
        }


        public DatabaseOperationResult SaveSDEvent(ref SDEvent evt)
        {
            try
            {
                using (var ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    using (TransactionScope scope = new TransactionScope())
                    {
                        bool deleted = evt.ChangeTracker.State == ObjectState.Deleted;
                        //Add new
                        if (evt.ChangeTracker.State == ObjectState.Added)
                        {
                            var sdEvt = new SDEvent();
                            evt.CopyEntityValueTypesTo(sdEvt, "L_SDEventType", "L_SDPurpose", "L_Species", "L_DFUArea", "L_SDSampleType", "sdEventId");

                            if (evt.L_DFUAreas != null && evt.L_DFUAreas.Count > 0)
                            {
                                var lst = evt.L_DFUAreas.Select(x => x.DFUArea).ToList();
                                var lstAdd = ctx.L_DFUArea.Where(x => lst.Contains(x.DFUArea)).ToList();
                                if (lstAdd.Count > 0)
                                    sdEvt.L_DFUAreas.AddRange(lstAdd);
                            }

                            //Add any added age readers.
                            if(evt.SDReaders != null && evt.SDReaders.Count > 0)
                            {
                               foreach(var reader in evt.SDReaders)
                                {
                                    var r_reader = new R_SDEventSDReader();
                                    reader.CopyEntityValueTypesTo(r_reader, () => r_reader.sdEventId);
                                    sdEvt.SDReaders.Add(r_reader);
                                }
                            }

                            ctx.SDEvent.ApplyChanges(sdEvt);
                            evt = sdEvt;
                        }
                        else //Delete or update eixsting
                        {
                            var id = evt.sdEventId;

                            if (evt.ChangeTracker.State == ObjectState.Deleted)
                            {
                                var sdEvt = ctx.SDEvent
                                               .Include("L_DFUAreas")
                                               .Include("SDReaders")
                                               .Include("SDSample")
                                               .Include("SDSample.SDFile")
                                               .Include("SDSample.SDFile.SDAnnotation")
                                               .Include("SDSample.SDFile.SDAnnotation.SDLine")
                                               .Include("SDSample.SDFile.SDAnnotation.SDPoint")
                                               .Where(x => x.sdEventId == id).FirstOrDefault();

                                //If it exists in DB, delete it. It should be sufficient to include all the related entities, since their FK does not allow them not being connected to a parent, so they should get deleted, when the event is.
                                if (sdEvt != null)
                                {
                                    sdEvt.L_DFUAreas.Clear();

                                    //Make sure reader relations are deleted as well.
                                    foreach (var sdr in sdEvt.SDReaders.ToList())
                                        ctx.DeleteObject(sdr);

                                    ctx.DeleteObject(sdEvt);
                                }
                            }
                            else
                            {
                                var sdEvt = ctx.SDEvent.Include("L_DFUAreas")
                                                       .Include("SDReaders")
                                                       .Where(x => x.sdEventId == id).FirstOrDefault();

                                //If it for some reason did not exist, add it.
                                if (sdEvt == null)
                                    sdEvt = new SDEvent();

                                evt.CopyEntityValueTypesTo(sdEvt, "L_SDEventType", "L_SDPurpose", "L_Species", "L_DFUAreas", "L_SDSampleType", "sdEventId", "SDReaders");

                                SynchronizeSDEventAreas(ctx, evt, sdEvt);

                                SynchronizeSDEventReaders(ctx, evt, sdEvt);

                                ctx.SDEvent.ApplyChanges(sdEvt);
                                evt = sdEvt;
                            }

                        }

                        ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                        evt.AcceptChanges();

                        //Make sure navigation properties are loaded on return.
                        if (!deleted)
                        {
                            ctx.LoadProperty(evt, "L_SDEventType");
                            ctx.LoadProperty(evt, "L_SDPurpose");
                            ctx.LoadProperty(evt, "L_Species");
                            ctx.LoadProperty(evt, "L_DFUAreas");
                            ctx.LoadProperty(evt, "L_SDSampleType");
                            ctx.LoadProperty(evt, "SDReaders");
                            foreach(var rel in evt.SDReaders)
                            {
                                ctx.LoadProperty(rel, "SDReader");
                                if (rel.SDReader != null)
                                {
                                    var r = rel.SDReader;
                                    ctx.LoadProperty(r, "DFUPerson");
                                    ctx.LoadProperty(r, "L_Stock");
                                    ctx.LoadProperty(r, "L_SDReaderExperience");
                                    ctx.LoadProperty(r, "L_SDPreparationMethod");
                                    ctx.LoadProperty(r, "L_Species");
                                }
                            }
                        }

                        scope.Complete();
                    }


                }

                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveSDEvent with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveSDEvent with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveSDEvent with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }


        private void SynchronizeSDEventReaders(SprattusContainer ctx, SDEvent evtIncoming, SDEvent evtDatabase)
        {
            //Remove readers that are not in the incoming entity.
            var readerToAdd = evtIncoming.SDReaders.ToList();
            var readersToRemove = evtDatabase.SDReaders.Where(x => !readerToAdd.Any(y => y.sdReaderId == x.sdReaderId)).ToList();
            foreach (var a in readersToRemove)
            {
                ctx.DeleteObject(a);
               // evtDatabase.SDReaders.Remove(a);
            }

            //Get new readers that are not already added.
            var lst = readerToAdd.Where(x => !evtDatabase.SDReaders.Any(y => y.sdReaderId == x.sdReaderId)).ToList();

            //If there are any new to add, add them.
            if (lst.Count > 0)
            {
                foreach (var r_toAdd in lst)
                {
                    var r_new = new R_SDEventSDReader();
                    r_toAdd.CopyEntityValueTypesTo(r_new, () => r_new.sdEventId);
                    evtDatabase.SDReaders.Add(r_new);
                }
            }

            var lstModified = readerToAdd.Where(x => x.ChangeTracker.State == ObjectState.Modified).ToList();

            if(lstModified.Count > 0)
            {
                foreach(var updateFrom in lstModified)
                {
                    var existing = evtDatabase.SDReaders.Where(x => x.sdReaderId == updateFrom.sdReaderId).FirstOrDefault();
                    if(existing != null)
                        updateFrom.CopyEntityValueTypesTo(existing, () => existing.sdEventId);
                }
            }
        }


        private void SynchronizeSDEventAreas(SprattusContainer ctx, SDEvent evtIncoming, SDEvent evtDatabase)
        {
            //Remove areas that are not in the incoming entity.
            var areasToAdd = evtIncoming.L_DFUAreas.ToList();
            var areasToRemove = evtDatabase.L_DFUAreas.Where(x => !areasToAdd.Any(y => y.L_DFUAreaId == x.L_DFUAreaId)).ToList();
            foreach (var a in areasToRemove)
                evtDatabase.L_DFUAreas.Remove(a);

            //Get new areas that are not already added.
            var lst = areasToAdd.Where(x => !evtDatabase.L_DFUAreas.Any(y => y.L_DFUAreaId == x.L_DFUAreaId)).Select(x => x.L_DFUAreaId).ToList();

            //If there are any to add, get them fresh from the DB and add them.
            if (lst.Count > 0)
            {
                var lstAdd = ctx.L_DFUArea.Where(x => lst.Contains(x.L_DFUAreaId)).ToList();
                if (lstAdd.Count > 0)
                    evtDatabase.L_DFUAreas.AddRange(lstAdd);
            }
        }



        public byte[] GetSDSamplesCompressed(int eventId)
        {
            var lst = GetSDSamples(eventId);

            if (lst == null)
                return null;

            var ba = lst.ToByteArrayDataContract();
            var cba = ba.Compress();

            return cba;
        }


        /// <summary>
        /// Get all samples from an event or only the ones from the sampleIds list, if it is supplied.
        /// </summary>
        public List<SDSample> GetSDSamples(int eventId, List<int> sampleIds = null)
        {
            try
            {
                int evtId = eventId;
                using (var ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);


                    var sl = (from s in ctx.SDSample
                                           .Include("SDFile")
                                           .Include("SDFile.SDAnnotation")
                                           .Include("SDFile.SDAnnotation.SDLine")
                                           .Include("SDFile.SDAnnotation.SDPoint")
                                           .Include("SDFile.SDAnnotation.L_OtolithReadingRemark")
                                           .Include("L_DFUArea")
                                           .Include("L_MaturityIndexMethod")
                                           .Include("L_SexCode")
                                           .Include("L_StatisticalRectangle")
                                           .Include("L_Species")
                                           .Include("L_Stock")
                                           .Include("Maturity")
                                           .Include("L_EdgeStructure")
                                           .Include("L_OtolithReadingRemark")
                                           .Include("L_SDLightType")
                                           .Include("L_SDOtolithDescription")
                                           .Include("L_SDPreparationMethod")
                             
                              select s);

                    //Only return samples with ids the supplied sampleIds list (if it is supplied)
                    if (sampleIds != null && sampleIds.Count > 0)
                    {
                        var lstIds = sampleIds.ToList();
                        sl = sl.Where(s => lstIds.Contains(s.sdSampleId));
                    }
                    else //Other get samples from supplied event id
                    {
                        sl = sl.Where(s => s.sdEventId == evtId);
                    }

                    var lst = sl.ToList();

                    return lst;
                }
            }
            catch (Exception e)
            {
                try { Anchor.Core.Loggers.Logger.LogError(e); }
                catch { }
            }

            return null;
        }


        public byte[] GetSDSamplesWithIncludesCompressed(int eventId, string[] includes)
        {
            var lst = GetSDSamplesWithIncludes(eventId, includes);

            if (lst == null)
                return null;

            var ba = lst.ToByteArrayDataContract();
            var cba = ba.Compress();

            return cba;
        }



        public List<SDSample> GetSDSamplesWithIncludes(int eventId, string[] includes)
        {
            try
            {
                int evtId = eventId;
                using (var ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    var query = ctx.SDSample.Where(s => s.sdEventId == evtId);

                    int i = 0;
                    while (includes != null && i < includes.Length)
                        query = ((dynamic)query).Include(includes[i++]);

                    var sl = query.ToList();

                    return sl;
                }
            }
            catch (Exception e)
            {
                try { Anchor.Core.Loggers.Logger.LogError(e); }
                catch { }
            }

            return null;
        }



        /// <summary>
        /// Return a distinct list of cruise years.
        /// </summary>
        public List<int> GetCruiseYears()
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var sl = ctx.Cruise.Select(c => c.year).Distinct().OrderByDescending(y => y).ToList();

                return sl;
            }
        }


        /// <summary>
        /// Get animals compressed from fishline to add to an event. The method needs a species code and an array of years. If all years are wanted, supple NULL. 
        /// </summary>
        public byte[] GetSelectionAnimalsCompressed(string speciesCode, int[] years, string[] areas, bool includeAnimalJpgFiles, string[] relativeFolderPaths)
        {
            /*
             LAVRep   = representative = "ja" && (individNum == NULL || individNum == 0)
             SFRep    = representative = "ja" && individNum != NULL && individNum > 0
             SFNotRep = representative = "nej" && individNum != NULL && individNum > 0

             Return SingleFish (SF) only. This can be done by checking the individNum > 0.
             */
            string strWhere = string.Format(" WHERE ani.individNum IS NOT NULL and ani.individNum > 0 AND sl.speciesCode = '{0}' ", speciesCode);

            if (years != null && years.Length > 0)
                strWhere += string.Format(" AND ani.Year IN ({0}) ", string.Join(", ", years));

            if (areas != null && areas.Length > 0)
                strWhere += string.Format(" AND ani.dfuArea IN ({0}) ", string.Join(", ", areas.Select(x => string.Format("'{0}'", x))));

            var lst = GetSelectionAnimals(strWhere, includeAnimalJpgFiles, relativeFolderPaths);

            if (lst == null)
                return null;

            var ba = lst.ToByteArrayProtoBuf();
            var cba = ba.Compress();

            return cba;
        }


        /// <summary>
        /// Get animals compressed from fishline to add to an event. The method needs a species code and an array of years. If all years are wanted, supple NULL. 
        /// </summary>
        public byte[] GetSelectionAnimalsByIdCompressed(int[] animalIds, bool includeAnimalJpgFiles, string[] relativeFolderPaths)
        {
            string strWhere = string.Format(" WHERE ani.animalId IN ({0}) ", string.Join(", ", animalIds));

            var lst = GetSelectionAnimals(strWhere, includeAnimalJpgFiles, relativeFolderPaths);

            if (lst == null)
                return null;

            var ba = lst.ToByteArrayProtoBuf();
            var cba = ba.Compress();

            return cba;
        }



        /// <summary>
        /// Get animals from fishline to add to an event. The method needs a species code and an array of years. If all years are wanted, supple NULL. 
        /// </summary>
        private List<SelectionAnimal> GetSelectionAnimals(string whereClause, bool includeAnimalJpgFiles, string[] relativeFolderPaths)
        {
            try
            {
                List<SelectionAnimal> lst = null;
                using (var ctx = new Warehouse.Model.DataWarehouseContext())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    string strCmd = @"SELECT ani.animalId as AnimalId,  
                                              ani.year as CruiseYear, 
                                              ani.cruise as Cruise, 
											  ani.trip as Trip,
                                              ani.tripType as TripType, 
											  ani.station as Station,
                                              ani.dateGearStart as GearStartDate, 
                                              ani.dfuArea as AreaCode,
                                              ani.statisticalRectangle as StatisticalRectangle,
											  s.latPosStartText as Latitude,
											  s.lonPosStartText as Longitude, 
											  ani.sexCode as SexCode,
											  age.age as Age, 
											  age.edgeStructure as EdgeStructureCode,
											  age.otolithReadingRemark as OtolithReadingRemarkCode,
											  ani.maturityIndex as MaturityIndex,
											  ani.maturityIndexMethod as MaturityIndexMethod,
                                              ani.length as LengthMM, 
                                              ani.weight * 1000 as WeightG, 
											  ani.remark as AnimalRemark,
                                              sl.speciesCode as SpeciesCode,
											  sl.remark as SpeciesListRemark
                                         FROM [dbo].Animal as ani 
                                         LEFT OUTER JOIN [dbo].Age as age ON age.animalId = ani.animalId 
                                         LEFT OUTER JOIN [dbo].SpeciesList as sl on sl.speciesListId = ani.speciesListId
                                         LEFT OUTER JOIN [dbo].Sample as s on s.sampleId = sl.sampleId
                                         ";

                    if (!string.IsNullOrWhiteSpace(whereClause))
                        strCmd += whereClause;

                    lst = ctx.ExecuteStoreQuery<SelectionAnimal>(strCmd).ToList();
                }

                if (lst == null || lst.Count == 0)
                    return lst;

                if (includeAnimalJpgFiles)
                {
                    var animalIds = lst.Select(x => x.AnimalId.ToString()).ToList();
                    string error = null;

                    //Get all animal files
                    var dicAnimalFiles = GetFileInformationDictionaryFromAnimalIds(animalIds, relativeFolderPaths, ref error);

                    var sdPrepMethod = GetLookupEntities(typeof(L_SDPreparationMethod), null).OfType<L_SDPreparationMethod>().DistinctBy(x => x.preparationMethod).ToDictionary(x => x.preparationMethod.ToUpperInvariant());
                    var sdLightType = GetLookupEntities(typeof(L_SDLightType), null).OfType<L_SDLightType>().DistinctBy(x => x.lightType).ToDictionary(x => x.lightType.ToUpperInvariant());
                    var sdOtolithDesc = GetLookupEntities(typeof(L_SDOtolithDescription), null).OfType<L_SDOtolithDescription>().DistinctBy(x => x.otolithDescription).ToDictionary(x => x.otolithDescription.ToUpperInvariant());

                    //Loop through animals and attach any found files.
                    foreach (var sa in lst.ToList())
                    {
                        var animalIdsString = sa.AnimalId.ToString();
                        if (dicAnimalFiles != null && dicAnimalFiles.ContainsKey(animalIdsString))
                        {
                            Dictionary<string, OtolithFileInformation> dicFi = dicAnimalFiles[animalIdsString];
                            if (dicFi != null && dicFi.Count > 0)
                            {
                                //Only get animal files that also have a jpeg.
                                var q = dicFi.Values.Where(x => !string.IsNullOrWhiteSpace(x.FileName)).Select(x => x.FileName);

                                //Concatenate all jpeg for the animal.
                                if (q.Any())
                                {
                                    if(q.Count() > 0)
                                    {
                                        Dictionary<string, SelectionAnimal> dic = new Dictionary<string, SelectionAnimal>();

                                        //Split image out in new selection animals depending on their preperation method, light type, and otolith description.
                                        foreach(var imgPath in q)
                                        {
                                            if (imgPath == null)
                                                continue;

                                            var ip = Path.GetFileNameWithoutExtension(imgPath);
                                            var arr = ip.Split('_');
                                            string key = "";

                                            //Check if prep method, light type and otolith descriptions actually exist as code in the db. If they don't exist, put them to null.
                                            string prepMethod = (arr.Length > 1 && sdPrepMethod.ContainsKey(arr[1].ToUpperInvariant())) ? arr[1].ToUpperInvariant() : null;
                                            string lightType = (arr.Length > 2 && sdLightType.ContainsKey(arr[2].ToUpperInvariant())) ? arr[2].ToUpperInvariant() : null;
                                            string otolithDesc = (arr.Length > 3 && sdOtolithDesc.ContainsKey(arr[3].ToUpperInvariant())) ? arr[3].ToUpperInvariant() : null;

                                            key += (prepMethod ?? "") + "-"; //Preperation method
                                            key += (lightType ?? "") + "-"; //Light type
                                            key += (otolithDesc ?? "") + "-"; //Otolith description.

                                            if (!dic.ContainsKey(key))
                                            {
                                                //The first item is already in the lst, therefore just update the animal with prep, light type, and oto descript.
                                                if (dic.Count == 0)
                                                {
                                                    sa.PreperationMethod = prepMethod;
                                                    sa.LightType = lightType;
                                                    sa.OtolithDescription = otolithDesc;
                                                    dic.Add(key, sa);
                                                }
                                                else
                                                {
                                                    var anii = sa.Clone();
                                                    anii.AnimalImageFileNames = null;
                                                    anii.PreperationMethod = prepMethod;
                                                    anii.LightType = lightType;
                                                    anii.OtolithDescription = otolithDesc;
                                                    lst.Add(anii);
                                                    dic.Add(key, anii);
                                                }
                                            }

                                            var ani = dic[key];
                                            var aniFiles = ani.AnimalImageFileNames;
                                            if (aniFiles == null)
                                                aniFiles = new List<string>();
                                            aniFiles.Add(imgPath);
                                            ani.AnimalImageFileNames = aniFiles;
                                            dic[key] = ani;
                                        }
                                    }
                                    else
                                        sa.AnimalImageFileNames = q.ToList();
                                }
                            }
                        }
                    }
                }

                var stockSpeciesArea = GetLookupEntities(typeof(R_StockSpeciesArea), new string[] { "L_Stock" }).OfType<R_StockSpeciesArea>().ToList();

                if (stockSpeciesArea != null && stockSpeciesArea.Count > 0)
                {
                    foreach (var a in lst)
                    {
                        //If stock was not there, see if it is available in the relationship table.
                        if (string.IsNullOrWhiteSpace(a.StockCode))
                        {
                            L_Stock stock = Babelfisk.Warehouse.EntityFactory.GetStock(stockSpeciesArea, a.SpeciesCode, a.AreaCode, a.StatisticalRectangle, a.Quarter);

                            if (stock != null)
                            {
                                a.StockCode = stock.stockCode;
                                a.StockId = stock.L_stockId;
                            }
                        }
                    }
                }

                return lst;
            }
            catch (Exception e)
            {
                try { Anchor.Core.Loggers.Logger.LogError(e); }
                catch { }
            }

            return null;
        }


        /// <summary>
        /// Create new SDSample hierarchy from existing SDSample
        /// </summary>
        private SDSample CreateNewSDSampleFromExisting(SDSample s)
        {
            var sdSample = new SDSample();
            s.CopyEntityValueTypesTo(sdSample, "SDEvent", "L_DFUArea", "L_MaturityIndexMethod", "L_SexCode", "L_StatisticalRectangle", "L_Stock", "Maturity", "SDFile", "L_EdgeStructure", "L_OtolithReadingRemark", "L_SDLightType", "L_SDOtolithDescription", "L_SDPreparationMethod");
            sdSample.sdSampleGuid = Guid.NewGuid();

            foreach (var f in s.SDFile)
            {
                var fNew = CreateNewSDFileFromExisting(f);
                fNew.sdFileGuid = Guid.NewGuid();
                sdSample.SDFile.Add(fNew);
            }

            return sdSample;
        }


        private SDFile CreateNewSDFileFromExisting(SDFile f)
        {
            var fNew = new SDFile();
            f.CopyEntityValueTypesTo(fNew, "SDAnnotation", "SDSample");

            foreach (var a in f.SDAnnotation)
            {
                var aNew = new SDAnnotation();
                a.CopyEntityValueTypesTo(aNew, "SDFile", "SDLine", "SDPoint");
                aNew.sdAnnotationGuid = Guid.NewGuid();

                foreach (var l in a.SDLine)
                {
                    var lNew = new SDLine();
                    l.CopyEntityValueTypesTo(lNew, "SDAnnotation");
                    lNew.sdLineGuid = Guid.NewGuid();
                    aNew.SDLine.Add(lNew);
                }

                foreach (var p in a.SDPoint)
                {
                    var pNew = new SDPoint();
                    p.CopyEntityValueTypesTo(pNew, "SDAnnotation");
                    pNew.sdPointGuid = Guid.NewGuid();
                    aNew.SDPoint.Add(pNew);
                }

                fNew.SDAnnotation.Add(aNew);
            }

            return fNew;
        }


        /// <summary>
        /// Returns a Dictionary<animalids, Dictionary<filenameWithoutExtension, OtolithFileInformation>>.
        /// If null is returned error has a value.
        /// </summary>
        public Dictionary<string, Dictionary<string, OtolithFileInformation>> GetFileInformationDictionaryFromAnimalIds(List<string> animalIds, string[] relativeFolderPaths, ref string error)
        {
            Dictionary<string, Dictionary<string, OtolithFileInformation>> res = null;

            OtolithFileService.OtolithFileServiceClient sv = null;
            try
            {
               sv = new OtolithFileService.OtolithFileServiceClient();

               res = sv.GetFileInformationDictionaryFromAnimalIds(animalIds.ToArray(), relativeFolderPaths, ref error);

               sv.Close();
            }
            catch(Exception e)
            {
                sv.Abort();
                error = "An unexpected error occurred when getting file information. " + (e.Message ?? "");
            }

            return res;
        }


        public ServiceResult GetFileBytes(string relativeImagePath)
        {
            OtolithFileService.OtolithFileServiceClient sv = null;
            try
            {
                sv = new OtolithFileService.OtolithFileServiceClient();

                var res = sv.GetFileBytes(relativeImagePath);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                var res = new ServiceResult(DatabaseOperationStatus.UnexpectedException, "An unexpected error occurred when getting file information. " + (e.Message ?? ""));
                return res;
            }
        }




        public ServiceResult GetFileInformationFromAnimalIds(List<string> animalIds, string[] relativeFolderPaths)
        { 
            OtolithFileService.OtolithFileServiceClient sv = null;
            try
            {
                sv = new OtolithFileService.OtolithFileServiceClient();

                var res = sv.GetFileInformationFromAnimalIds(animalIds.ToArray(), relativeFolderPaths);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                var res = new ServiceResult(DatabaseOperationStatus.UnexpectedException, "An unexpected error occurred when getting file information. " + (e.Message ?? ""));
                return res;
            }
        }


        public ServiceResult GetFileInformationFromFileNames(List<string> fileNames, string[] relativeFolderPaths)
        {
            OtolithFileService.OtolithFileServiceClient sv = null;
            try
            {
                sv = new OtolithFileService.OtolithFileServiceClient();

                var res = sv.GetFileInformationFromFileNames(fileNames.ToArray(), relativeFolderPaths);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                var res = new ServiceResult(DatabaseOperationStatus.UnexpectedException, "An unexpected error occurred when getting file information. " + (e.Message ?? ""));
                return res;
            }
        }


        public ServiceResult GetAllFilesPaths(string[] relativeFolderPaths)
        {
            OtolithFileService.OtolithFileServiceClient sv = null;
            try
            {
                sv = new OtolithFileService.OtolithFileServiceClient();

                var res = sv.GetAllFilePaths(relativeFolderPaths);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                var res = new ServiceResult(DatabaseOperationStatus.UnexpectedException, "An unexpected error occurred when getting file information. " + (e.Message ?? ""));
                return res;
            }
        }

       
        public ServiceResult GetFolderContentCompressed(string folderRelativePath, FileSystemType type)
        {
            OtolithFileService.OtolithFileServiceClient sv = null;
            try
            {
                sv = new OtolithFileService.OtolithFileServiceClient();

                var res = sv.GetFolderContentCompressed(folderRelativePath, type);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                var res = new ServiceResult(DatabaseOperationStatus.UnexpectedException, "An unexpected error occurred when getting folder content. " + (e.Message ?? ""));
                return res;
            }
        }


        /// <summary>
        /// Move or copy sample from one event to another.
        /// </summary>
        public DatabaseOperationResult MoveOrCopySamplesToEvent(List<int> sampleIds, int toEventId, bool copy)
        {
            try
            {
                //Move
                if (!copy)
                {
                    using (var ctx = new SprattusContainer())
                    {
                        ctx.Connection.Open();
                        ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                        using (TransactionScope scope = new TransactionScope())
                        {
                            List<SDSample> samplesToMove = new List<SDSample>();

                            //Get samples in chunks and add the to the samples list to move.
                            foreach (var sId in sampleIds.InChunks(500))
                            {
                                var lstIds = sId.ToList();
                                var samples = ctx.SDSample.Where(x => lstIds.Contains(x.sdSampleId)).ToList();
                                if (samples != null && samples.Count > 0)
                                    samplesToMove.AddRange(samples);
                            }

                            if (samplesToMove.Count > 0)
                            {
                                foreach (var s in samplesToMove)
                                {
                                    s.sdEventId = toEventId;
                                }
                            }

                            ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                            scope.Complete();
                        }
                    }
                }
                else //Copy
                {
                    //Copy samples in chunks of 50, to keep memory usage at a reasonable level.
                    foreach (var sId in sampleIds.InChunks(50))
                    {
                        var lstIds = sId.ToList();
                        var sdSamples = GetSDSamples(toEventId, lstIds);

                        using (var ctx = new SprattusContainer())
                        {
                            ctx.Connection.Open();
                            ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                            foreach (var s in sdSamples)
                            {
                                s.ChangeTracker.State = ObjectState.Added;
                                s.sdSampleId = 0;
                                s.sdEventId = toEventId;

                                var sdSample = CreateNewSDSampleFromExisting(s);
                                ctx.SDSample.ApplyChanges(sdSample);
                            }

                            ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                        }
                    }
                }
                       
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in MoveOrCopySamplesToEvent with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in MoveOrCopySamplesToEvent with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in MoveOrCopySamplesToEvent with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }


        public DatabaseOperationResult SaveSDSamples(SDSample[] samples)
        {
            try
            {
                using (var ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    using (TransactionScope scope = new TransactionScope())
                    {
                        foreach (var s in samples)
                        {
                            bool deleted = s.ChangeTracker.State == ObjectState.Deleted;
                            //Add new
                            if (s.ChangeTracker.State == ObjectState.Added)
                            {

                                var sdSampleDoublicatelst = ctx.SDSample.Include("SDFile")
                                                               .Include("SDFile.SDAnnotation")
                                                               .Include("SDFile.SDAnnotation.FishLineAge")
                                                               .Include("SDFile.SDAnnotation.SDLine")
                                                               .Include("SDFile.SDAnnotation.SDPoint")
                                                               .Where(x => x.animalId == s.animalId).ToList();

                                //If it did not exist, add it.

                                SDSample sdSampleDoublicate = null;
                                if(sdSampleDoublicatelst != null)
                                {
                                    sdSampleDoublicate = sdSampleDoublicatelst.FirstOrDefault(x => (x.SDFile == null || x.SDFile.Count == 0) && x.sdPreparationMethodId == null);
                                }

                                if (sdSampleDoublicatelst == null || sdSampleDoublicate == null )
                                {
                                    var sdSample = CreateNewSDSampleFromExisting(s);
                                    ctx.SDSample.ApplyChanges(sdSample);
                                }
                                else
                                {
                                    s.CopyEntityValueTypesTo(sdSampleDoublicate, "sdSampleId", "SDEvent", "L_DFUArea", "L_MaturityIndexMethod", "L_SexCode", "L_StatisticalRectangle", "L_Stock", "Maturity", "SDFile", "L_EdgeStructure", "L_OtolithReadingRemark", "L_SDLightType", "L_SDOtolithDescription", "L_SDPreparationMethod");
                                    var dres = SynchronizeSDFiles(ctx, sdSampleDoublicate, s);

                                    if (dres.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                                        return dres;

                                    ctx.SDSample.ApplyChanges(sdSampleDoublicate);
                                }
  
                            }
                            else //Delete or update eixsting
                            {
                                var id = s.sdSampleId;

                                if (s.ChangeTracker.State == ObjectState.Deleted)
                                {
                                    var sdSample = ctx.SDSample
                                                      .Include("SDFile")
                                                      .Include("SDFile.SDAnnotation")
                                                      .Include("SDFile.SDAnnotation.FishLineAge")
                                                      .Include("SDFile.SDAnnotation.SDLine")
                                                      .Include("SDFile.SDAnnotation.SDPoint")
                                                      .Where(x => x.sdSampleId == id).FirstOrDefault();

                                    //If it exists in DB, delete it.
                                    if (sdSample != null)
                                    {
                                        var ff = sdSample.SDFile.Where(x => x.SDAnnotation.Any()).FirstOrDefault();
                                        if (ff != null)
                                            return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "HASANNOTATIIONS", null, new Dictionary<string, string>() { { "AnimalId", sdSample.animalId }, { "FileName", ff.fileName } });

                                        foreach (var f in sdSample.SDFile.ToList())
                                        {
                                            DeleteSDFile(ctx, f);
                                        }

                                        ctx.DeleteObject(sdSample);
                                    }
                                }
                                else
                                {
                                    var sdSample = ctx.SDSample.Include("SDFile")
                                                               .Include("SDFile.SDAnnotation")
                                                               .Include("SDFile.SDAnnotation.FishLineAge")
                                                               .Include("SDFile.SDAnnotation.SDLine")
                                                               .Include("SDFile.SDAnnotation.SDPoint")
                                                               .Where(x => x.sdSampleId == id).FirstOrDefault();

                                    //If it for some reason did not exist, add it.
                                    if (sdSample == null)
                                        sdSample = new SDSample();


                                    s.CopyEntityValueTypesTo(sdSample, "SDEvent", "L_DFUArea", "L_MaturityIndexMethod", "L_SexCode", "L_StatisticalRectangle", "L_Stock", "Maturity", "SDFile", "L_EdgeStructure", "L_OtolithReadingRemark", "L_SDLightType", "L_SDOtolithDescription", "L_SDPreparationMethod");

                                    //s.CopyEntityValueTypesTo(sdSample, "L_SDEventType", "L_SDPurpose", "sdEventId");

                                    var dres = SynchronizeSDFiles(ctx, sdSample, s);

                                    if (dres.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                                        return dres;

                                    ctx.SDSample.ApplyChanges(sdSample);
                                }

                            }
                        }

                        ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                        scope.Complete();
                    }
                }

                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveSDSamples with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveSDSamples with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveSDSamples with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }


        private void DeleteSDFile(SprattusContainer ctx, SDFile f)
        {
            foreach (var a in f.SDAnnotation.ToList())
            {
                foreach (var p in a.SDPoint.ToList())
                    ctx.DeleteObject(p);

                foreach (var l in a.SDLine.ToList())
                    ctx.DeleteObject(l);

                ctx.DeleteObject(a);
            }

            ctx.DeleteObject(f);
        }



        private DatabaseOperationResult SynchronizeSDFiles(SprattusContainer ctx, SDSample sampleDB, SDSample sampleIn)
        {
            foreach (var f in sampleDB.SDFile.ToList())
            {
                var extFile = sampleIn.SDFile.Where(x => x.sdFileId == f.sdFileId).FirstOrDefault();

                //If should get deleted.
                if (extFile == null)
                {
                    //Don't allow to delete if annotations are found.
                    if (f.SDAnnotation.Any())
                        return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "HASANNOTATIIONS", null, new Dictionary<string, string>() { { "AnimalId", sampleDB.animalId }, { "FileName", f.fileName } });

                    DeleteSDFile(ctx, f);
                }
                //If modified.
                else if (extFile.ChangeTracker.State == ObjectState.Modified)
                {
                    extFile.CopyEntityValueTypesTo(f, "SDAnnotation", "SDSample");
                }
                else
                {
                    //Do nothing, the entity has not changed.
                }
            }

            foreach (var f in sampleIn.SDFile.ToList())
            {
                if (f.ChangeTracker.State == ObjectState.Added)
                {
                    var sdFile = CreateNewSDFileFromExisting(f);

                    sampleDB.SDFile.Add(sdFile);
                }
            }

            return DatabaseOperationResult.CreateSuccessResult();
        }


        /// <summary>
        /// Debug method for inserting a list of stock relations into the DB. 
        /// </summary>
        private void SynchronizeStockAreaSpecies()
        {
            string strDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ("Data"));
            string filePath = Path.Combine(strDirectory, "stock_relation_draft_2022-10-11.csv");

            var lstStocks = GetLookupEntities(typeof(L_Stock), null).OfType<L_Stock>().ToList();
            var lstSpecies = GetLookupEntities(typeof(L_Species), null).OfType<L_Species>().ToList();
            var lstArea = GetLookupEntities(typeof(L_DFUArea), null).OfType<L_DFUArea>().ToList();
            var lstRectangles = GetLookupEntities(typeof(L_StatisticalRectangle), null).OfType<L_StatisticalRectangle>().ToList();
            var lstStockSpeciesArea = GetLookupEntities(typeof(R_StockSpeciesArea), new string[] { "L_Stock" }).OfType<R_StockSpeciesArea>().ToList();

            var cont = filePath.LoadFileContentFromPath(FileShare.ReadWrite, System.Text.Encoding.Default);

            if(string.IsNullOrWhiteSpace(cont))
                    return;

            char seperator =',';

            int stockIndex = -1, areaIndex = -1, speciesIndex = -1, statIndex = -1, quarterIndex = -1;

            using (StringReader sr = new StringReader(cont))
            {
                var header = sr.ReadLine();

                var arrHeader = header.Split(seperator);

                for (int i = 0; i < arrHeader.Length; i++)
                {
                    var hv = arrHeader[i].Replace("\"", "");

                    switch (hv.ToLower())
                    {
                        case "stock":
                            stockIndex = i;
                            break;

                        case "area":
                            areaIndex = i;
                            break;

                        case "speciescode":
                        case "icescode":
                            speciesIndex = i;
                            break;

                        case "statisticalrectangle":
                            statIndex = i;
                            break;

                        case "quarter":
                            quarterIndex = i;
                            break;
                    }
                }

                string line = null;

                List<string> sb = new List<string>();

                List<R_StockSpeciesArea> lstAdd = new List<R_StockSpeciesArea>();
                int lineno = 0;
                using (var ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    while ((line = sr.ReadLine()) != null)
                    {
                        lineno++;
                        var lineh = line.Replace("\"", "");
                        var arrLine = lineh.Split(seperator);

                        var stock = arrLine[stockIndex].Trim();
                        var area = arrLine[areaIndex].Trim();
                        var species = arrLine[speciesIndex].Trim();
                        var statRect = arrLine[statIndex].Trim() == "NA" ? "" : arrLine[statIndex].Trim();
                        var quarter = arrLine[quarterIndex].Trim() == "NA" ? "" : arrLine[quarterIndex].Trim();

                        var dfuArea = lstArea.Where(x => !string.IsNullOrWhiteSpace(x.areaICES) && x.areaICES.Equals(area, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        var dfuSpecies = lstSpecies.Where(x => !string.IsNullOrWhiteSpace(x.icesCode) && x.icesCode.Equals(species, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        var dfuStock = lstStocks.Where(x => !string.IsNullOrWhiteSpace(x.stockCode) && x.stockCode.Equals(stock, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        var dfuStat = lstRectangles.Where(x => !string.IsNullOrWhiteSpace(x.statisticalRectangle) && x.statisticalRectangle.Equals(statRect, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();


                        if (statRect != "" && dfuStat == null)
                        {
                            sb.Add(string.Format("Line no: {0}. Statistical Rectangle not found in FishLine: {1}.", lineno, line));
                            continue;
                        }

                        if (dfuSpecies == null)
                        {
                            sb.Add(string.Format("Line no: {0}. Species not found in FishLine: {1}.", lineno, line));
                            continue;
                        }

                        if (dfuArea == null)
                        {
                            sb.Add(string.Format("Line no: {0}. Area not found in FishLine: {1}.", lineno, line));
                            continue;
                        }


                        if (dfuStock == null)
                        {
                            sb.Add(string.Format("Line no: {0}. Stock not found in FishLine: {1}.", lineno, line));
                            continue;
                        }

                        string dfuStatString = dfuStat == null ? null : dfuStat.statisticalRectangle;
                        int? quarterLocal = null;
                        int tmp = 0;
                        if (!string.IsNullOrWhiteSpace(quarter) && quarter.TryParseInt32(out tmp))
                            quarterLocal = tmp;

                        if (lstStockSpeciesArea.Where(x => x.L_stockId == dfuStock.L_stockId &&
                                                           x.DFUArea == dfuArea.DFUArea &&
                                                           x.speciesCode == dfuSpecies.speciesCode &&
                                                           (x.statisticalRectangle ?? "").Equals(dfuStatString ?? "", StringComparison.InvariantCultureIgnoreCase) &&
                                                           (x.quarter.HasValue ? x.quarter.Value : -1) == (quarterLocal.HasValue ? quarterLocal.Value : -1)).Any())
                        {
                           // sb.Add(string.Format("Line no: {0}. Duplicate row: {1}.", lineno, line));
                            continue;
                        }

                        var r = new R_StockSpeciesArea();
                        r.L_stockId = dfuStock.L_stockId;
                        r.DFUArea = dfuArea.DFUArea;
                        r.speciesCode = dfuSpecies.speciesCode;
                        r.statisticalRectangle = dfuStatString;
                        r.quarter = quarterLocal;

                        lstStockSpeciesArea.Add(r);
                        ctx.R_StockSpeciesArea.AddObject(r);
                    }
                    ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                }

                if(sb.Count > 0)
                {
                    var resFile = Path.Combine(strDirectory, "result.txt");
                    using (FileStream fs = new FileStream(resFile, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            foreach (var l in sb)
                                sw.WriteLine(l);
                        }
                    }
                }
            }

            
           

        }


        /// <summary>
        /// Update fishline Animal/Age records with supplied ages.
        /// </summary>
        public DatabaseOperationResult CopyAgesToFishLine(List<SDAnimalAgeItem> animalIdsAndAges)
        {
            try
            {
                if (animalIdsAndAges == null || animalIdsAndAges.Count == 0)
                    return DatabaseOperationResult.CreateSuccessResult();

                int skippedDueToNotSingleFish = 0;

                HashSet<int> subSampleIds = new HashSet<int>();
                Dictionary<string, DFUPerson> dicReaders = null;

                using (var ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    using (TransactionScope scope = new TransactionScope())
                    {
                        
                        foreach (var kv in animalIdsAndAges)
                        {
                            int animalId = kv.AnimalId;
                            var animal = ctx.Animal.Include("Age")
                                                   .Include("SubSample")
                                                    .Include("SubSample.SpeciesList")
                                                   .Where(x => x.animalId == animalId)
                                                   .FirstOrDefault();

                            //If animal is not a singlefish, dont update it.
                            if (animal.individNum == null || animal.individNum <= 0)
                            {
                                skippedDueToNotSingleFish++;
                                continue;
                                //return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "AbortOperation", "Aborting update operation. Animal with id {0} is not a single fish animal.");
                            }
                            
                            if (animal.Age.Count == 0)
                                animal.Age.Add(new Age() { ageMeasureMethodId = 1 });

                            Age a = animal.Age.First();

                            a.number = 1; //Set number = 1 for all single fish ages.
                            a.sdAgeInfoUpdated = true;
                           
                            //Only assign age if the otolith reading remark lookup says so.
                            if (kv.ShouldAssignAge)
                                a.age1 = kv.Age;

                            a.sdAgeReadId = kv.DFUPersonReaderId;

                            if(kv.OtolithReadingRemarkId.HasValue)
                                a.otolithReadingRemarkId = kv.OtolithReadingRemarkId.Value;

                            if (!string.IsNullOrWhiteSpace(kv.EdgeStructureCode))
                                a.edgeStructure = kv.EdgeStructureCode;

                            if(kv.SDAnnotationId.HasValue)
                                a.sdAnnotationId = kv.SDAnnotationId.Value;

                            //Update species lists age reader.
                            //Leave this out for now, since sdAgeReadId was added with the specific reader who updated the age. 
                            /* if (animal.SubSample != null && animal.SubSample.SpeciesList != null)
                             {
                                 if (kv.DFUPersonReaderId.HasValue)
                                     animal.SubSample.SpeciesList.ageReadId = kv.DFUPersonReaderId.Value;
                                 //If only initials are provided, find the DFUPerson id from that and assign it.
                                 //else if(!string.IsNullOrWhiteSpace(kv.DFUPersonReaderInitials))
                                 //{
                                 //    if(dicReaders == null)
                                 //        dicReaders = ctx.DFUPerson.ToList().Where(x => !string.IsNullOrWhiteSpace(x.initials)).DistinctBy(x => x.initials.ToLower()).ToDictionary(x => x.initials.ToLower());   
                                 //
                                 //     if(dicReaders != null && dicReaders.ContainsKey(kv.DFUPersonReaderInitials.ToLower()))
                                 //         animal.SubSample.SpeciesList.ageReadId = dicReaders[kv.DFUPersonReaderInitials.ToLower()].dfuPersonId;
                                 //}
                             }*/


                            //Make sure sample is updated with AQ score and edge structure of the animal, so consistency check wont find any discrepancies (after data has been moved to the warehouse).
                            int sampleId = kv.SDSampleId;
                            var sample = ctx.SDSample.Where(x => x.sdSampleId == sampleId).FirstOrDefault();
                            if (sample != null)
                            {
                                sample.otolithReadingRemarkId = a.otolithReadingRemarkId;
                                sample.edgeStructure = a.edgeStructure;
                            }

                            if (!subSampleIds.Contains(animal.subSampleId))
                                subSampleIds.Add(animal.subSampleId);
                        }

                        ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                        scope.Complete();
                    }

                    //Make sure the cruise is set for nightly transfer, since som ages have changes within the cruise.
                    if (subSampleIds.Count > 0)
                    {
                        foreach (var s in subSampleIds)
                        {
                            AddCruiseBySubSampleIdToDataWarehouseTransferQueue(ctx, s);
                        }
                    }
                }

                return new DatabaseOperationResult(DatabaseOperationStatus.Successful, null, null, new Dictionary<string, string>() { {"SkippedDueToLavRepCount", skippedDueToNotSingleFish.ToString() } });
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveSDSamples with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveSDSamples with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveSDSamples with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }



        public SDAnnotation GetSDAnnotation(int sdAnnotationId)
        {
            try
            {
                int annoId = sdAnnotationId;
                using (var ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    var an = (from a in ctx.SDAnnotation
                                           .Include("SDFile")
                                           .Include("SDFile.SDSample")
                                           .Include("SDFile.SDSample.SDEvent")
                              where a.sdAnnotationId == annoId
                              select a
                              ).FirstOrDefault();

                    return an;
                }
            }
            catch (Exception e)
            {
                try { Anchor.Core.Loggers.Logger.LogError(e); }
                catch { }
            }

            return null;
        }


        public List<R_SDReaderStatistics> GetSDReaderStatistics()
        {
            List<R_SDReaderStatistics> lst = null;
            try
            {

               
                using (var ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    string strCmd = @"  SELECT r.r_SDReaderId as R_SDReaderId, Count(a.sdAnnotationId) as NumberOfReadings
                                        FROM SDAnnotation a
                                        INNER JOIN SDFile f on f.sdfileId = a.sdFileId
                                        INNER JOIN SDSample s on s.sdSampleId = f.sdSampleId
                                        INNER JOIN SDEvent e on e.sdEventId = s.sdEventId
                                        INNER JOIN R_SDEventSDReader er on er.sdEventId = e.sdEventId
                                        INNER JOIN R_SDReader r on r.r_SDReaderId = er.sdReaderId
                                        INNER JOIN DFUPerson p on p.dfuPersonId = r.dfuPersonId
                                        WHERE r.dfuPersonId = a.createdById
                                        GROUP BY r.r_SDReaderId
                                     ";

                    lst = ctx.ExecuteStoreQuery<R_SDReaderStatistics>(strCmd).ToList();
                }
            }
            catch (Exception e)
            {
                try { Anchor.Core.Loggers.Logger.LogError(e); }
                catch { }
            }

            return lst;
        }


    }
}