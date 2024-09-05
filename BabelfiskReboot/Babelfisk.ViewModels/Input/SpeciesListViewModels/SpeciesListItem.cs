using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Anchor.Core;
using Babelfisk.Entities.Sprattus;
using Anchor.Core.Converters;

namespace Babelfisk.ViewModels.Input
{
    public abstract class SpeciesListItem : AViewModel
    {
        protected SpeciesList _speciesListEntity;

        private int _intIndex;

        protected bool _blnNew = false;

        protected SpeciesListViewModel _speciesListVM;

        private string _strLastErrorProperty = null;
        private string _strLastErrorMsg = null;

        private bool _blnIsSelected;

        private static InvCultureStringToDecimalConverter _decConverter = new InvCultureStringToDecimalConverter();


        #region Properties


        public bool IsNew
        {
            get { return _blnNew; }
            set
            {
                _blnNew = value;
                RaisePropertyChanged(() => IsNew);
            }
        }

        public int Index
        {
            get { return _intIndex; }
            set
            {
                _intIndex = value;
                RaisePropertyChanged(() => Index);
            }
        }


        public bool IsSelected
        {
            get { return _blnIsSelected; }
            set
            {
                _blnIsSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }


        public override bool HasUnsavedData
        {
            get
            {
                return (_speciesListEntity != null && 
                       (_speciesListEntity.ChangeTracker.State != ObjectState.Unchanged && !(_speciesListEntity.ChangeTracker.State == ObjectState.Added && !IsDirty))) 
                       || IsDirty;
            }
        }


        public SpeciesList SpeciesListEntity
        {
            get { return _speciesListEntity; }
            private set
            {
                _speciesListEntity = value;
                RaisePropertyChanged(() => SpeciesListEntity);
            }
        }


        #region Shared species list properties (between HVN, VID, and SØS)

        public string Species
        {
            get { return _speciesListEntity.speciesCode; }
            set
            {
                _speciesListEntity.speciesCode = value;
                IsDirty = true;
                RaisePropertyChanged(() => Species);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string LandingCategory
        {
            get { return _speciesListEntity.landingCategory; }
            set
            {
                _speciesListEntity.landingCategory = value;
                IsDirty = true;
                UpdateTreatmentFromLandingCategory();
                RaisePropertyChanged(() => LandingCategory);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string Sex
        {
            get { return _speciesListEntity.sexCode; }
            set
            {
                _speciesListEntity.sexCode = value;
                IsDirty = true;
                RaisePropertyChanged(() => Sex);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string Treatment
        {
            get { return _speciesListEntity.treatment; }
            set
            {
                _speciesListEntity.treatment = value;
                IsDirty = true;
                RaisePropertyChanged(() => Treatment);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public bool? Ovigorous
        {
            get { return string.IsNullOrEmpty(_speciesListEntity.ovigorous) ? false : string.Equals(_speciesListEntity.ovigorous, "ja", StringComparison.InvariantCultureIgnoreCase) ? new Nullable<bool>(true) : null; }
            set
            {
                _speciesListEntity.ovigorous = value.HasValue ? (value.Value ? "ja" : null) : "nej";
                IsDirty = true;
                RaisePropertyChanged(() => Ovigorous);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }

        public int? SizeSorting
        {
            get { return _speciesListEntity.sizeSortingEU; }
            set
            {
                _speciesListEntity.sizeSortingEU = value;
                IsDirty = true;
                RaisePropertyChanged(() => SizeSorting);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string SizeSortingDFU
        {
            get { return _speciesListEntity.sizeSortingDFU; }
            set
            {
                _speciesListEntity.sizeSortingDFU = value;
                IsDirty = true;
                RaisePropertyChanged(() => SizeSortingDFU);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public int? Number
        {
            get { return _speciesListEntity.number; }
            set
            {
                _speciesListEntity.number = value;
                IsDirty = true;
                RaisePropertyChanged(() => Number);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public decimal? SubSampleWeightStep0Dec
        {
            get { return GetSubsample(0) == null ? null : GetSubsample(0).subSampleWeight; }
        }

        public string SubSampleWeightStep0
        {
            get { return GetSubsample(0) == null ? null : GetWeight(GetSubsample(0)); }
            set
            {
                var s = GetSubsample(0);

                if (s != null)
                    SetWeight(s, value);
                else
                {
                    if (value != null)
                    {
                        s = new SubSample() { stepNum = 0, representative = "ja" };
                        SetWeight(s, value);
                        AddSubSample(s);
                    }
                }

                IsDirty = true;
                RaisePropertyChanged(() => SubSampleWeightStep0);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public decimal? SubSampleWeightStep1Dec
        {
            get { return GetSubsample(1) == null ? null : GetSubsample(1).subSampleWeight; }
        }

        public string SubSampleWeightStep1
        {
            get { return GetSubsample(1) == null ? null : GetWeight(GetSubsample(1)); }
            set
            {
                var s = GetSubsample(1);

                if (s != null)
                    SetWeight(s, value);
                else
                {
                    if (value != null)
                    {
                        s = new SubSample() { stepNum = 1, representative = "ja" };
                        SetWeight(s, value);
                        AddSubSample(s);
                    }
                }

                IsDirty = true;
                RaisePropertyChanged(() => SubSampleWeightStep1);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public decimal? SubSampleWeightStep2Dec
        {
            get { return GetSubsample(2) == null ? null : GetSubsample(2).subSampleWeight; }
        }

        public string SubSampleWeightStep2
        {
            get { return GetSubsample(2) == null ? null : GetWeight(GetSubsample(2)); }
            set
            {
                var s = GetSubsample(2);

                if (s != null)
                    SetWeight(s, value);
                else
                {
                    if (value != null)
                    {
                        s = new SubSample() { stepNum = 2, representative = "ja" };
                        SetWeight(s, value);
                        AddSubSample(s);
                    }
                }

                IsDirty = true;
                RaisePropertyChanged(() => SubSampleWeightStep2);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }

        public decimal? SubSampleWeightNotRepDec
        {
            get { return GetSubsample(0, false) == null ? null : GetSubsample(0, false).subSampleWeight; }
        }

        public string SubSampleWeightNotRep
        {
            get { return GetSubsample(0, false) == null ? null : GetWeight(GetSubsample(0, false)); }
            set
            {
                var s = GetSubsample(0, false);

                if (s != null)
                    SetWeight(s,value);
                else
                {
                    if (value != null)
                    {
                        s = new SubSample() { stepNum = 0, representative = "nej" };
                        SetWeight(s, value);
                        AddSubSample(s);
                    }
                }

                IsDirty = true;
                RaisePropertyChanged(() => SubSampleWeightNotRep);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public decimal? SubSampleWeightBMSNotRepDec
        {
            get { return _speciesListEntity == null ? null : _speciesListEntity.bmsNonRep; }
        }


        public string SubSampleWeightBMSNotRep
        {
            get
            {
                object objVal = _decConverter.Convert(_speciesListEntity != null ? _speciesListEntity.bmsNonRep : null, null, "4", null);
                return objVal == null ? null : objVal.ToString();
            }
            set
            {
                var s = GetSubsample(0, false);

                if (_speciesListEntity != null)
                {
                    object objVal = _decConverter.ConvertBack(value, null, "4", null);
                    _speciesListEntity.bmsNonRep = objVal == null ? null : new Nullable<decimal>((decimal)objVal);
                }

                IsDirty = true;
                RaisePropertyChanged(() => SubSampleWeightBMSNotRep);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public virtual string LE
        {
            get 
            {
                string str = "";

                if (SpeciesListEntity == null)
                    return str;

                if (SpeciesListEntity.IsLAVPresent && SpeciesListEntity.IsSFPresent)
                    return "L,E";
                else if (SpeciesListEntity.IsLAVPresent)
                    return "L";
                else if (SpeciesListEntity.IsSFPresent)
                    return "E";

                return str;
            }
        }


        public string Remark
        {
            get { return _speciesListEntity.remark; }
            set
            {
                _speciesListEntity.remark = value;
                IsDirty = true;
                RaisePropertyChanged(() => Remark);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public virtual bool IsWeightsSpecified
        {
            get
            {
                return SubSampleWeightStep0 != null || SubSampleWeightStep1 != null || SubSampleWeightStep2 != null;
            }
        }


        private bool IsUnique
        {
            get
            {
                if (_speciesListEntity == null)
                    return true;

                var itms = _speciesListVM.Items.Where(x => (x != this) &&
                                                     (!string.IsNullOrEmpty(Species) ? Species.Equals(x.Species) : x.Species == null) &&
                                                     (!string.IsNullOrEmpty(LandingCategory) ? LandingCategory.Equals(x.LandingCategory) : x.LandingCategory == null) &&
                                                     (SizeSorting.HasValue ? SizeSorting.Equals(x.SizeSorting) : x.SizeSorting == null) &&
                                                     (!string.IsNullOrEmpty(SizeSortingDFU) ? SizeSortingDFU.Equals(x.SizeSortingDFU) : x.SizeSortingDFU == null) &&
                                                     (!string.IsNullOrEmpty(Sex) ? Sex.Equals(x.Sex) : x.Sex == null) &&
                                                     (_speciesListEntity.applicationId == (x._speciesListEntity != null ? x._speciesListEntity.applicationId : null)) && 
                                                     (Ovigorous == x.Ovigorous));

                bool blnUnique = itms.Count() == 0;

                return blnUnique;
            }
        }


        public string ErrorProperty
        {
            get { return _strLastErrorProperty; }
            set
            {
                _strLastErrorProperty = value;
                RaisePropertyChanged(() => ErrorProperty);
                RaisePropertyChanged(() => LastError);
            }
        }


        public string LastError
        {
            get { return _strLastErrorMsg; }
        }


        public bool CanEditOffline
        {
            get { return !BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline || IsNew || (IsLoading || (_speciesListEntity != null && _speciesListEntity.OfflineState == ObjectState.Added)); }
        }

        #endregion


        #endregion



        public SpeciesListItem(SpeciesListViewModel slvm, SpeciesList sl)
        {
            _speciesListVM = slvm;

            _speciesListEntity = sl;

            if (_speciesListEntity == null)
            {
                _speciesListEntity = new SpeciesList();
                _blnNew = true;
            }

            //Make sure item is not dirty after initialized.
            IsDirty = false;
        }


        private void UpdateTreatmentFromLandingCategory()
        {
           /* if (_speciesListEntity == null)
                return;

            if (Treatment == null)
                Treatment = "UR";*/
            /*if (_speciesListEntity == null || string.IsNullOrWhiteSpace(_speciesListEntity.landingCategory))
                return;

            switch (_speciesListEntity.landingCategory.ToLower())
            {
                case "kon":
                    Treatment = "RH";
                    break;

                case "ind":
                case "dis":
                    Treatment = "UR";
                    break;
            }*/
            
        }


        /// <summary>
        /// Create a new species list item of type T
        /// </summary>
        public static SpeciesListItem New<T>(SpeciesListViewModel slvm, int intSampleId, int intIndex) 
            where T : SpeciesListItem
        {
            ConstructorInfo c = typeof(T).GetConstructor(new Type[] { typeof(SpeciesListViewModel), typeof(SpeciesList) });
            var speciesItem = c.Invoke(new object[] { slvm, null }) as SpeciesListItem;
            speciesItem.Index = intIndex;
            speciesItem.SpeciesListEntity.sampleId = intSampleId;
            speciesItem.Treatment = "UR";
            speciesItem.IsDirty = false;

            speciesItem.AfterCreated(slvm);

            return speciesItem;
        }


        /// <summary>
        /// Method is called after a new species list item has been created
        /// </summary>
        protected virtual void AfterCreated(SpeciesListViewModel slvm) { }



        public SubSample GetSubsample(int intStep, bool blnRep = true)
        {
            return _speciesListEntity.SubSample.Where(x => x.stepNum == intStep && x.representative.Equals((blnRep ? "ja" : "nej"), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }


        public void AddSubSample(SubSample s)
        {
            if (GetSubsample(s.stepNum, s.representative == null ? true : s.representative.Equals("ja", StringComparison.InvariantCultureIgnoreCase)) != null)
                throw new ApplicationException("SubSample already exists in collection");

            _speciesListEntity.SubSample.Add(s);

            if (_speciesListEntity.ChangeTracker.State == ObjectState.Unchanged)
                _speciesListEntity.MarkAsModified();
        }


        #region Validation methods

        public override void ValidateAllProperties(bool blnRefreshProperties = false, bool raisePropertyChangedOnProperties = true)
        {
            _strLastErrorMsg = ErrorProperty = null;

            base.ValidateAllProperties(true, raisePropertyChangedOnProperties);
        }


        /// <summary>
        /// Hide base method ValidateField for any inherited classes of this class (they should override ValidateListItem instead).
        /// </summary>
        protected override string ValidateField(string strFieldName)
        {
            string strError = null;
            SubSample ss0, ss1, ss2;

            if (_blnValidate && IsPropertyUsed(strFieldName))
            {
                switch (strFieldName)
                {
                    case "Species":
                        if (string.IsNullOrEmpty(Species))
                            strError = "Angiv venligst en art.";

                        if (!IsUnique)
                        {
                            strError = "En række med samme Art";
                            if (this is SpeciesListHVNItem || this is SpeciesListSEAItem)
                                strError += ", Landingskategori, Sortering";

                            if (this is SpeciesListVIDItem || this is SpeciesListSEAItem)
                                strError += ", Opdeling, Rogn";

                            strError += " og Køn eksisterer allerede.";
                        }
                        break;

                    case "LandingCategory":
                        //Not mandatory
                        break;

                    case "Treatment":
                        if (string.IsNullOrEmpty(Treatment))
                            strError = "Angiv venligst en behandling.";
                        break;

                    case "Sex":
                        //Not mandatory
                        if (Ovigorous.HasValue && Ovigorous.Value && (Sex == null || !Sex.Equals("f", StringComparison.InvariantCultureIgnoreCase)))
                            strError = "Hvis fisken indeholder rogn, skal køn sættes til 'Hunkøn'.";
                        break;

                    case "SubSampleWeightStep1":
                        ss0 = GetSubsample(0);
                        ss1 = GetSubsample(1);
                        if (!IsWeightValid(ss0, ss1))
                            strError = "Vægten på trin 1 må ikke være større end eller lig med vægten på trin 0";
                        break;

                    case "SubSampleWeightStep2":
                        ss0 = GetSubsample(0);
                        ss1 = GetSubsample(1);
                        ss2 = GetSubsample(2);
                        if (!IsWeightValid(ss0, ss1) || !IsWeightValid(ss1, ss2))
                            strError = "Vægten på trin 2 må ikke være større end eller lig med vægten på trin 0 eller trin 1";
                        break; 

                    default:
                        strError = ValidateListItem(strFieldName);
                        break;
                }

                //Save the property name of the property raising the error
                if (strError != null)
                {
                    _strLastErrorMsg = strError;
                    ErrorProperty = strFieldName;
                }
            }
            //If this is a normal error check, return the same error as found during the manual error check if the
            //property being checked is the same as the property checked during the manual error checking.
            else if (_strLastErrorProperty == strFieldName)
            {
                return _strLastErrorMsg;
            }

            return strError;
        }


        private bool IsWeightValid(SubSample ssHigh, SubSample ssLow)
        {
            if (ssHigh == null || ssLow == null || !ssHigh.subSampleWeight.HasValue || !ssLow.subSampleWeight.HasValue)
                return true;

            return ssLow.subSampleWeight.Value < ssHigh.subSampleWeight.Value;
        }


        private bool IsValid(decimal? weightHigh, decimal? weightLow)
        {
            if(!weightHigh.HasValue || !weightLow.HasValue)
                return true;

            return weightLow.Value < weightHigh.Value;
        }

        protected virtual bool IsPropertyUsed(string strProperty)
        {
            return true;
        }


        protected virtual string ValidateListItem(string strPropertyName)
        {
            return null;
        }

        #endregion


        private string GetWeight(SubSample ss)
        {
            if (ss.sumAnimalWeights.HasValue && ss.sumAnimalWeights.Value)
                return "x";
            else if (ss.subSampleWeight.HasValue)
            {
                object objVal = _decConverter.Convert(ss.subSampleWeight.Value, null, "4", null);
                return objVal == null ? null : objVal.ToString();
            }
            else
                return null;
        }


        private void SetWeight(SubSample ss, string strWeight)
        {
            if (!string.IsNullOrWhiteSpace(strWeight))
            {
                if (strWeight.Equals("x", StringComparison.InvariantCultureIgnoreCase))
                {
                    ss.subSampleWeight = null;
                    ss.sumAnimalWeights = true;
                }
                else
                {
                    object objVal = _decConverter.ConvertBack(strWeight, null, "4", null);
                    ss.subSampleWeight = objVal == null ? null : new Nullable<decimal>((decimal)objVal);
                    ss.sumAnimalWeights = null;
                }
            }
            else
            {
                ss.subSampleWeight = null;
                ss.sumAnimalWeights = null;
            }
        }


        public virtual void BeforeSave()
        {

        }
    }
}
