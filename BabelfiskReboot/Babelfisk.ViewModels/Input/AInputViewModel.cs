using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Babelfisk.BusinessLogic.DataInput;
using Babelfisk.Entities.Sprattus;
using Microsoft.Practices.Prism.Commands;

namespace Babelfisk.ViewModels.Input
{
    public abstract class AInputViewModel : AViewModel
    {
        /// <summary>
        /// Event fired when a trip has been successfully saved (only).
        /// </summary>
        public event Action<AInputViewModel> OnSaveSucceeded;


        /// <summary>
        /// Button commands
        /// </summary>
        protected DelegateCommand _cmdSave;
        protected DelegateCommand _cmdClose;


        private bool _blnIsEdit;

        protected bool _blnValidate = false;

        private Map.IMapViewModel _mapViewModel;


        #region Properties

        public bool IsEdit
        {
            get { return _blnIsEdit; }
            protected set
            {
                _blnIsEdit = value;
                RaisePropertyChanged(() => IsEdit);
            }
        }


        public Map.IMapViewModel MapViewModel
        {
            get { return _mapViewModel; }
            set
            {
                _mapViewModel = value;
                RaisePropertyChanged(() => MapViewModel);
            }
        }


        public virtual bool CanEditOffline
        {
            get
            {
                return !BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline;
            }
        }


        #endregion


        protected AInputViewModel()
        {
            MapViewModel = new Map.MapViewModelBingControl();
        }

        protected void RaiseSaveSucceeded()
        {
            if (OnSaveSucceeded != null)
                OnSaveSucceeded(this);
        }


        protected void InitializeMap(int? intId, TreeNodeLevel idType)
        {
            if (!intId.HasValue)
                return;

            try
            {
                var man = new DataInputManager();
                List<MapPoint> lstPoints = null;

                switch (idType)
                {
                    case TreeNodeLevel.Cruise:
                        lstPoints = man.GetMapPositionsFromCruiseId(intId.Value);
                        break;

                    case TreeNodeLevel.Trip:
                        lstPoints = man.GetMapPositionsFromTripId(intId.Value);
                        break;

                    case TreeNodeLevel.Sample:
                        lstPoints = man.GetMapPositionsFromSampleId(intId.Value);
                        break;
                }

                if (lstPoints == null)
                    return;

                if (MapViewModel != null)
                {
                    MapViewModel.IsHidden = false;
                    MapViewModel.Points = lstPoints;
                    MapViewModel.RefreshAsync();
                }
            }
            catch (Exception e)
            {
                if (e is TimeoutException || e is EndpointNotFoundException)
                {
                    //Have the map show the "No internet connection" message.
                    MapViewModel.IsHidden = false;
                    MapViewModel.ShowWebBrowser = false;
                }
                else
                    DispatchMessageBox("En uventet fejl opstod under hentning af kort-data. " + e.Message);
            }
        }


        public ViewModels.Lookup.LookupManagerViewModel GetLookupManagerViewModel(string strLookupType)
        {
            ViewModels.Lookup.LookupManagerViewModel lm = null;

            switch (strLookupType)
            {
                case "L_LandingCategory":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_LandingCategory));
                    break;

                case "L_FisheryType":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_FisheryType));
                    break;

                case "L_GearQuality":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_GearQuality));
                    break;

                case "L_SelectionDevice":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SelectionDevice));
                    break;

                case "L_SelectionDeviceSource":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SelectionDeviceSource));
                    break;

                case "L_Harbour":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_Harbour));
                    break;

                case "L_Species":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_Species));
                    break;

                case "L_StatisticalRectangle":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_StatisticalRectangle));
                    break;

                case "L_DFUArea":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_DFUArea));
                    break;

                case "ICES_DFU_Relation_FF":
                    lm = new Lookup.LookupManagerViewModel(typeof(ICES_DFU_Relation_FF));
                    break;

                case "L_SpeciesRegistration":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SpeciesRegistration));
                    break;

                case "L_CatchRegistration":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_CatchRegistration));
                    break;

                case "L_Platform":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_Platform));
                    break;

                case "L_GearType":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_GearType));
                    break;

                case "L_SampleType":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SampleType));
                    break;

                case "L_SamplingType":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SamplingType));
                    break;

                case "L_SamplingMethod":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SamplingMethod));
                    break;

                case "DFUPerson":
                    lm = new Lookup.LookupManagerViewModel(typeof(DFUPerson));
                    break;

                case "L_HaulType":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_HaulType));
                    break;

                case "L_ThermoCline":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_ThermoCline));
                    break;

                case "L_TimeZone":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_TimeZone));
                    break;

                case "Person":
                    lm = new Lookup.LookupManagerViewModel(typeof(Person));
                    break;

                case "L_Bottomtype":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_Bottomtype));
                    break;

                case "L_SDPurpose":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SDPurpose));
                    break;

                case "L_SDEventType":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SDEventType));
                    break;

                case "L_SDSampleType":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SDSampleType));
                    break;

                case "L_SDLightType":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SDLightType));
                    break;

                case "L_SDOtolithDescription":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SDOtolithDescription));
                    break;

                case "L_SDPreparationMethod":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SDPreparationMethod));
                    break;

                case "L_Stock":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_Stock));
                    break;

                case "L_SexCode":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_SexCode));
                    break;

                case "Maturity":
                    lm = new Lookup.LookupManagerViewModel(typeof(Maturity));
                    break;

                case "L_OtolithReadingRemark":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_OtolithReadingRemark));
                    break;

                case "L_EdgeStructure":
                    lm = new Lookup.LookupManagerViewModel(typeof(L_EdgeStructure));
                    break;

            }

            return lm;
        }


        public bool HasUserViewLookupRights(bool blnDisplayErrorMessage = true)
        {
            if (User == null || !User.HasViewLookupsTask)
            {
                if(blnDisplayErrorMessage)
                    AppRegionManager.ShowMessageBox("Du har ikke rettigheder til at se eller ændre kodelisten.");

                return false;
            }

            return true;
        }


        public override void Dispose()
        {
            try
            {
                base.Dispose();

                if (MapViewModel != null)
                    MapViewModel.Dispose();
            }
            catch { }
        }


        #region Save Command

        public DelegateCommand SaveCommand
        {
            get { return _cmdSave ?? (_cmdSave = new DelegateCommand(() => ValidateAndSaveAsync())); }
        }


        /// <summary>
        /// Validate and save changes to database.
        /// </summary>
        protected virtual void ValidateAndSaveAsync()
        {
        }

        #endregion


        #region Close Command

        public DelegateCommand CloseCommand
        {
            get { return _cmdClose ?? (_cmdClose = new DelegateCommand(() => CloseViewModel())); }
        }


        protected virtual void CloseViewModel()
        {
        }

        #endregion
    }
}
