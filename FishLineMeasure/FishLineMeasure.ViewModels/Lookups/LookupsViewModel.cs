using FishLineMeasure.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using Babelfisk.Entities;
using FishLineMeasure.BusinessLogic.Lookups;

namespace FishLineMeasure.ViewModels.Lookups
{
    public class LookupsViewModel : AViewModel
    {
        private DelegateCommand _cmdCancelSynchronization;

        private int _transferredCount;

        private int _toTransferCount;

        private double _transferredPercentage;

        private bool _cancelSync = false;


        #region Properties

        // Henter TransferredCount / ToTransferCount lookups

        public int TransferredCount
        {
            get { return _transferredCount; }
            set
            {
                _transferredCount = value;
                RaisePropertyChanged(nameof(TransferredCount));
                //CloseUponDone();
            }
        }

        private void CloseUponDone()
        {
            if (_transferredCount == _toTransferCount)
            {
                DispatchMessageBox("Færdig med at hente længdefordelinger");
                this.Close();
            }
        }

        public int ToTransferCount
        {
            get { return _toTransferCount; }
            set
            {
                _toTransferCount = value;
                RaisePropertyChanged(nameof(ToTransferCount));
            }
        }

        public double TransferredPercentage
        {
            get { return _transferredPercentage; }
            set
            {
                _transferredPercentage = value;
                RaisePropertyChanged(nameof(TransferredPercentage));
            }
        }



        #endregion


        public LookupsViewModel()
        {
            WindowHeight = 150;
            WindowWidth = 500;
        }

        public Task SyncLookupsAsync()
        {
            IsLoading = true;
            List<Type> types = GetLookupTypesList;
            ToTransferCount = types == null ? 0 : types.Count;

            return Task.Run(() => SyncLookups()).ContinueWith(t => new Action(() =>
           {
                IsLoading = false;
           }).Dispatch());
        }


        public static List<Type> GetLookupTypesList
        {
            get
            {
                return new List<Type>()
                {
                      GetLookupType(LookupType.Species),
                      GetLookupType(LookupType.Sex),
                      GetLookupType(LookupType.LandingCategory),
                      GetLookupType(LookupType.SizeSortingEU),
                      GetLookupType(LookupType.Ovigorous)
                };
            }
        }


        public static string GetLookupUIDisplay(ILookupEntity lookup)
        {
            if (lookup is Babelfisk.Entities.Sprattus.L_LandingCategory)
                return string.Format("{0} - {1}", (lookup as Babelfisk.Entities.Sprattus.L_LandingCategory).landingCategory, (lookup as Babelfisk.Entities.Sprattus.L_LandingCategory).description ?? "");
            else if(lookup is Babelfisk.Entities.Sprattus.L_SexCode)
                return string.Format("{0} - {1}", (lookup as Babelfisk.Entities.Sprattus.L_SexCode).sexCode, (lookup as Babelfisk.Entities.Sprattus.L_SexCode).description ?? "");
            else if (lookup is L_Ovigorous)
                return string.Format("{0} - {1}", (lookup as L_Ovigorous).OvigorousCode, (lookup as L_Ovigorous).Description ?? "");


            return lookup.UIDisplay;

        }

        public static string GetLookupCode(ILookupEntity lookup)
        {
            if (lookup is Babelfisk.Entities.Sprattus.L_LandingCategory)
                return string.Format("{0}", (lookup as Babelfisk.Entities.Sprattus.L_LandingCategory).landingCategory);
            else if (lookup is Babelfisk.Entities.Sprattus.L_SexCode)
                return string.Format("{0}", (lookup as Babelfisk.Entities.Sprattus.L_SexCode).sexCode);
            else if (lookup is Babelfisk.Entities.Sprattus.L_Species)
                return string.Format("{0}", (lookup as Babelfisk.Entities.Sprattus.L_Species).speciesCode);
            else if (lookup is Babelfisk.Entities.Sprattus.L_SizeSortingEU)
                return string.Format("{0}", (lookup as Babelfisk.Entities.Sprattus.L_SizeSortingEU).sizeSortingEU);
            else if(lookup is L_Ovigorous)
                return string.Format("{0}", (lookup as L_Ovigorous).OvigorousCode);

            return lookup.Id;

        }


        public static string GetLookupDisplayName(Type t)
        {
            switch(t.Name)
            {
                case "L_LandingCategory":
                      return "Landingskategori";

                case "L_SexCode":
                    return "Køn";

                case "L_Species":
                    return "Art";

                case "L_SizeSortingEU":
                    return "Sortering";

                case "L_SizeSortingDFU":
                    return "Opdeling";

                case "L_Application":
                    return "Anvendelse";

                case "L_Ovigorous":
                    return "Rogn";
            }

            return "";
            
        }


        public static string GetLookupDisplayNameShort(Type t)
        {
            if (t == null)
                return "";

            return GetLookupDisplayNameShort(t.Name);
        }

        public static string GetLookupDisplayNameShort(string type)
        {
            switch (type)
            {
                case "L_LandingCategory":
                    return "Kat.";

                case "L_SexCode":
                    return "Køn";

                case "L_Species":
                    return "Art";

                case "L_SizeSortingEU":
                    return "Sort.";

                case "L_SizeSortingDFU":
                    return "Opd.";

                case "L_Application":
                    return "Anv.";

                case "L_Ovigorous":
                    return "Rogn";
            }

            return "";
        }


        public static Type GetLookupType(LookupType enm)
        {
            switch (enm)
            {
                case LookupType.LandingCategory:
                    return typeof(Babelfisk.Entities.Sprattus.L_LandingCategory);

                case LookupType.Sex:
                    return typeof(Babelfisk.Entities.Sprattus.L_SexCode);

                case LookupType.Species:
                    return typeof(Babelfisk.Entities.Sprattus.L_Species);

                case LookupType.SizeSortingEU:
                    return typeof(Babelfisk.Entities.Sprattus.L_SizeSortingEU);

                case LookupType.Ovigorous:
                    return typeof(L_Ovigorous);
            }

            throw new ApplicationException("Lookup type is undefined.");
        }

        public bool SyncLookups()
        {
            var om = new BusinessLogic.LookupManager();

            LookupDataVersioning ldv = new LookupDataVersioning();

            //Type lookupType = typeof(Babelfisk.Entities.ILookupEntity);
            //var types = lookupType.Assembly.GetTypes().Where(t => lookupType.IsAssignableFrom(t) && !t.IsInterface).ToList();

            List<Type> types = GetLookupTypesList;

            //Set initial transfer states
            ReportProgress(0, types.Count, 0);

            for (int i = 0; i < types.Count; i++)
            {
                System.Threading.Thread.Sleep(50);
                try
                {
                    if(!typeof(ILocalLookupEntity).IsAssignableFrom(types[i]))
                        om.SaveLookupsToDisk(types[i], ldv);
                }
                catch(Exception e)
                {
                    //TODO: Handle if lookup could not be synced.
                    LogError(e, string.Format("Failed to save lookup '{0}'", types[i].Name));
                }

                //Update UI
                ReportProgress(i + 1, types.Count, (100.0 / (double)types.Count) * (i + 1));

                if (_cancelSync)
                    return false;
            }

            System.Threading.Thread.Sleep(500);

            return true;
        }


      


        private void ReportProgress(int intTransferedCount, int intToTransfer, double dblTransferredPercentage)
        {
            new Action(() =>
            {
                TransferredCount = intTransferedCount;

                if (ToTransferCount != intToTransfer)
                    ToTransferCount = intToTransfer;

                TransferredPercentage = dblTransferredPercentage;
            }).DispatchInvoke();
        }


        #region Cancel Synchronization Command


        public DelegateCommand CancelSynchronizationCommand
        {
            get { return _cmdCancelSynchronization ?? (_cmdCancelSynchronization = new DelegateCommand(CancelSynchronization)); }
        }


        public void CancelSynchronization()
        {
            _cancelSync = true;
        }


        #endregion

    }


}
