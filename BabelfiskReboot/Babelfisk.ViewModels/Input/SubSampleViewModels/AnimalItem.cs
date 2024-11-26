using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Babelfisk.Entities;
using System.Diagnostics;
using Microsoft.Practices.Prism.Commands;

namespace Babelfisk.ViewModels.Input
{
    public class AnimalItem : AViewModel
    {
        private Animal _animalEntity;

        private List<Age> _lstAges;

        private List<AnimalFile> _lstAnimalFiles;

        private AnimalInfo _animalInfo;

        private List<R_AnimalInfoReference> _lstR_References;

        private List<L_Reference> _lstReferences;

        private int _intIndex;

        private bool _blnNew = false;

        private bool _blnIsSelected;

        private ALavSFViewModel _parent;

        private string _strLastErrorProperty = null;
        private string _strLastErrorMsg = null;

        private bool _blnIsAnimalFilesPopupOpen;


        #region Properties

        public override bool HasUnsavedData
        {
            get
            {
                return (_animalEntity != null &&
                       (_animalEntity.ChangeTracker.State != ObjectState.Unchanged && !(_animalEntity.ChangeTracker.State == ObjectState.Added && !IsDirty))) ||
                       (_lstAges != null && _lstAges.Where(x => x.ChangeTracker.State != ObjectState.Unchanged && !IsAgeNew(x)).Count() > 0) ||
                       (_lstAnimalFiles != null && _lstAnimalFiles.Where(x => x.ChangeTracker.State != ObjectState.Unchanged).Count() > 0) ||
                       (_lstReferences != null && _lstReferences.Where(x => x.IsDirty).Count() > 0) ||
                       IsDirty;
            }
        }


        private bool IsAgeNew(Age a)
        {
            return a.ChangeTracker.State == ObjectState.Added && 
                   a.age1 == null && 
                   a.otolithReadingRemarkId == null && 
                   a.hatchMonthReadabilityId == null &&
                   a.otolithWeight == null && 
                   a.hatchMonth == null && 
                   a.edgeStructure == null && 
                   (a.genetics == null || (a.genetics.HasValue && a.genetics.Value == false) &&
                   a.visualStockId == null &&
                   a.geneticStockId == null);
        }

        
        public bool IsDeleted
        {
            get { return _animalEntity != null && _animalEntity.ChangeTracker.State == ObjectState.Deleted; }
        }


        public bool IsNew
        {
            get { return _blnNew; }
            set
            {
                _blnNew = value;
                RaisePropertyChanged(() => IsNew);
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


        public int Index
        {
            get { return _intIndex; }
            set
            {
                _intIndex = value;
                RaisePropertyChanged(() => Index);
            }
        }


        public AnimalInfo AnimalInfoEntity
        {
            get { return _animalInfo; }
        }


        public Animal AnimalEntity
        {
            get { return _animalEntity; }
            private set
            {
                _animalEntity = value;
                RaisePropertyChanged(() => AnimalEntity);
            }
        }


        public AnimalItem AnimalAgeUI
        {
            get { return this; }
            set
            {
                RaisePropertyChanged(() => AnimalAgeUI);
            }
        }


        public string AnimalId
        {
            get
            {
                if (_animalEntity == null || _animalEntity.ChangeTracker.State == ObjectState.Added || _animalEntity.animalId <= 0)
                    return "-";

                string strAnimalString = _animalEntity.animalId.ToString();

               // while (strAnimalString.Length < 9)
               //     strAnimalString = "0" + strAnimalString;

                return strAnimalString;
            }
        }


        public int? IndividNum
        {
            get { return _animalEntity.individNum; }
            set
            {
                _animalEntity.individNum = value;

                if (!IsNew)
                    IsDirty = true;

                RaisePropertyChanged(() => IndividNum);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        private int? _intLength;

        public int? Length
        {
            get { return _intLength/*_animalEntity.length*/; }
            set
            {
                // _animalEntity.length = value;
                _intLength = value;
                IsDirty = true;
                RaisePropertyChanged(() => Length);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public int Number
        {
            get { return _animalEntity.number; }
            set
            {
                _animalEntity.number = value;
                IsDirty = true;
                RaisePropertyChanged(() => Number);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public decimal? WeightInGrams
        {
            get { return _animalEntity.weight.HasValue ? new Nullable<decimal>(_animalEntity.weight.Value * 1000) : null; }
            set
            {
                if (value.HasValue)
                    _animalEntity.weight = value.Value / 1000;
                else
                    _animalEntity.weight = null;

                IsDirty = true;
                RaisePropertyChanged(() => Weight);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }

        public decimal? Weight
        {
            get { return _animalEntity.weight; }
            set
            {
                _animalEntity.weight = value;
                IsDirty = true;
                RaisePropertyChanged(() => Weight);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string Sex
        {
            get { return _animalEntity.sexCode; }
            set
            {
                _animalEntity.sexCode = value;
                IsDirty = true;
                RaisePropertyChanged(() => Sex);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string BroodingPhase
        {
            get { return _animalEntity.broodingPhase; }
            set
            {
                _animalEntity.broodingPhase = value;
                IsDirty = true;

                RaisePropertyChanged(() => BroodingPhase);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public int? MaturityId
        {
            get { return _animalInfo.maturityId; }
            set
            {
                _animalInfo.maturityId = value;
                IsDirty = true;
                RaisePropertyChanged(() => MaturityId);
                RaisePropertyChanged(() => Maturity);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string Maturity
        {
            get
            {
                if (!_animalInfo.maturityId.HasValue)
                    return null;

                var m = _parent.Parent.MaturityList.Where(x => x.maturityId == _animalInfo.maturityId.Value).FirstOrDefault();
                return m == null ? null : m.maturityIndex.ToString();
            }
        }


        public bool? OtolithFinScale
        {
            get { return _animalEntity.otolithFinScale; }
            set
            {
                _animalEntity.otolithFinScale = value;
                IsDirty = true;
                RaisePropertyChanged(() => OtolithFinScale);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public int? OtolithReadingRemarkId
        {
            get { return GetFirstAge() == null || _lstAges.First().ChangeTracker.State == ObjectState.Deleted ? null : GetFirstAge().otolithReadingRemarkId; }
            set
            {

                if (_lstAges.Count == 0)
                    _lstAges.Add(new Age());
                Age a = _lstAges.First();
                a.otolithReadingRemarkId = value;
                a.number = 1; //Set number = 1 for all single fish ages.
                HandleSingleFishAgeDeletion();

                IsDirty = true;
                RaisePropertyChanged(() => OtolithReadingRemarkId);
                RaisePropertyChanged(() => OtolithReadingRemark);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string OtolithReadingRemark
        {
            get
            {
                var a = GetFirstAge();
                if (a == null || !a.otolithReadingRemarkId.HasValue)
                    return null;

                var m = _parent.Parent.OtolithReadingRemarks.Where(x => x.L_OtolithReadingRemarkID == a.otolithReadingRemarkId.Value).FirstOrDefault();
                return m == null ? null : m.otolithReadingRemark;
            }
        }


        public int? HatchMonthReadabilityId
        {
            get { return GetFirstAge() == null || _lstAges.First().ChangeTracker.State == ObjectState.Deleted ? null : GetFirstAge().hatchMonthReadabilityId; }
            set
            {

                if (_lstAges.Count == 0)
                    _lstAges.Add(new Age());
                Age a = _lstAges.First();
                a.hatchMonthReadabilityId = value;
                a.number = 1; //Set number = 1 for all single fish ages.
                HandleSingleFishAgeDeletion();

                IsDirty = true;
                RaisePropertyChanged(() => HatchMonthReadabilityId);
                RaisePropertyChanged(() => HatchMonthRemark);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string HatchMonthRemark
        {
            get
            {
                var a = GetFirstAge();
                if (a == null || !a.hatchMonthReadabilityId.HasValue)
                    return null;

                var m = _parent.Parent.HatchMonthReadabilities.Where(x => x.L_HatchMonthReadabilityId == a.hatchMonthReadabilityId.Value).FirstOrDefault();
                return m == null ? null : m.hatchMonthRemark;
            }
        }



        public int? VisualStockId
        {
            get { return GetFirstAge() == null || _lstAges.First().ChangeTracker.State == ObjectState.Deleted ? null : GetFirstAge().visualStockId; }
            set
            {

                if (_lstAges.Count == 0)
                    _lstAges.Add(new Age());
                Age a = _lstAges.First();
                a.visualStockId = value;
                a.number = 1; //Set number = 1 for all single fish ages.
                HandleSingleFishAgeDeletion();

                IsDirty = true;
                RaisePropertyChanged(() => VisualStockId);
                RaisePropertyChanged(() => VisualStock);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string VisualStock
        {
            get
            {
                var a = GetFirstAge();
                if (a == null || !a.visualStockId.HasValue)
                    return null;

                var m = _parent.Parent.VisualStocks.Where(x => x.L_visualStockId == a.visualStockId.Value).FirstOrDefault();
                return m == null ? null : m.visualStock;
            }
        }



        public int? GeneticStockId
        {
            get { return GetFirstAge() == null || _lstAges.First().ChangeTracker.State == ObjectState.Deleted ? null : GetFirstAge().geneticStockId; }
            set
            {

                if (_lstAges.Count == 0)
                    _lstAges.Add(new Age());
                Age a = _lstAges.First();
                a.geneticStockId = value;
                a.number = 1; //Set number = 1 for all single fish ages.
                HandleSingleFishAgeDeletion();

                IsDirty = true;
                RaisePropertyChanged(() => GeneticStockId);
                RaisePropertyChanged(() => GeneticStock);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string GeneticStock
        {
            get
            {
                var a = GetFirstAge();
                if (a == null || !a.geneticStockId.HasValue)
                    return null;

                var m = _parent.Parent.GeneticStocks.Where(x => x.L_geneticStockId == a.geneticStockId.Value).FirstOrDefault();
                return m == null ? null : m.geneticStock;
            }
        }


        private List<FilterableKeyValuePair<bool>> _lstGenetics = new List<FilterableKeyValuePair<bool>>() 
        {
            new FilterableKeyValuePair<bool>("1 - Ja", true) { Tag = "Ja" },
            new FilterableKeyValuePair<bool>("2 - Nej", false) { Tag = "Nej" }
        };


        public List<FilterableKeyValuePair<bool>> Genetics
        {
            get { return _lstGenetics; }
        }


        public FilterableKeyValuePair<bool> SelectedGenetics
        {
            get
            {
                var a = GetFirstAge();

                if (a == null || _lstAges.First().ChangeTracker.State == ObjectState.Deleted || a.genetics == null)
                    return null;

                var g = _lstGenetics.Where(x => x.Value == a.genetics.Value).FirstOrDefault();
                return g;
            }
            set
            {
                if (_lstAges.Count == 0)
                    _lstAges.Add(new Age());
                Age a = _lstAges.First();
                a.genetics = value == null ? null : new Nullable<bool>(value.Value);
                a.number = 1;
                HandleSingleFishAgeDeletion();

                IsDirty = true;
                RaisePropertyChanged(() => SelectedGenetics);
                RaisePropertyChanged(() => SelectedGeneticsString);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string SelectedGeneticsString
        {
            get
            {
                var g = SelectedGenetics;
                return g == null ? "" : g.Tag as string;
            }
        }


        //Single fish age property. Don't delete age from database, if it is removed from UI, since EdgeStructure, Hatchmonth, OtolithReadingRemarkId is also set on Age.
        //TODO: It could be deleted if neither of the mentioned properties are set.
        public int? Age
        {
            get { return _lstAges.Count == 0 || _lstAges.First().ChangeTracker.State == ObjectState.Deleted ? new Nullable<int>() : _lstAges.First().age1; }
            set
            {
                if (_lstAges.Count == 0)
                    _lstAges.Add(new Age() { ageMeasureMethodId = 1 });

                Age a = _lstAges.First();

                a.number = 1; //Set number = 1 for all single fish ages.
                a.age1 = value;
                a.sdAgeInfoUpdated = false;
                a.sdAnnotationId = null; //Make sure reference to annotation is removed.
                a.sdAgeReadId = null;    //Also reset the age reader.
                HandleSingleFishAgeDeletion();

                IsDirty = true;
                RaisePropertyChanged(() => Age);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public bool IsRowEmptySF
        {
            get
            {
                return !Length.HasValue &&
                        !WeightInGrams.HasValue &&
                        string.IsNullOrWhiteSpace(Sex) &&
                        string.IsNullOrWhiteSpace(Maturity) &&
                        !Age.HasValue &&
                        string.IsNullOrWhiteSpace(OtolithReadingRemark) &&
                        string.IsNullOrWhiteSpace(HatchMonthRemark) &&
                        (SelectedGenetics == null || SelectedGenetics.Value == false) &&
                        string.IsNullOrWhiteSpace(EdgeStructure) &&
                        !HatchMonth.HasValue &&
                        string.IsNullOrWhiteSpace(Parasite) &&
                        !WeightGutted.HasValue &&
                        !WeightGonads.HasValue &&
                        !WeightLiver.HasValue &&
                        string.IsNullOrWhiteSpace(ReferencesString) &&
                        string.IsNullOrWhiteSpace(Remark) &&
                        string.IsNullOrWhiteSpace(VisualStock) &&
                        string.IsNullOrWhiteSpace(GeneticStock);
            }
        }


        #region LAV Ages

        public int? Age0
        {
            get { return GetAgeNumber(0); }
            set
            {
                SetAgeNumber(0, value);
                RaisePropertyChanged(() => Age0);
            }
        }

        public int? Age1
        {
            get { return GetAgeNumber(1); }
            set
            {
                SetAgeNumber(1, value);
                RaisePropertyChanged(() => Age1);
            }
        }

        public int? Age2
        {
            get { return GetAgeNumber(2); }
            set
            {
                SetAgeNumber(2, value);
                RaisePropertyChanged(() => Age2);
            }
        }

        public int? Age3
        {
            get { return GetAgeNumber(3); }
            set
            {
                SetAgeNumber(3, value);
                RaisePropertyChanged(() => Age3);
            }
        }

        public int? Age4
        {
            get { return GetAgeNumber(4); }
            set
            {
                SetAgeNumber(4, value);
                RaisePropertyChanged(() => Age4);
            }
        }

        public int? Age5
        {
            get { return GetAgeNumber(5); }
            set
            {
                SetAgeNumber(5, value);
                RaisePropertyChanged(() => Age5);
            }
        }

        public int? Age6
        {
            get { return GetAgeNumber(6); }
            set
            {
                SetAgeNumber(6, value);
                RaisePropertyChanged(() => Age6);
            }
        }

        public int? Age7
        {
            get { return GetAgeNumber(7); }
            set
            {
                SetAgeNumber(7, value);
                RaisePropertyChanged(() => Age7);
            }
        }

        public int? Age8
        {
            get { return GetAgeNumber(8); }
            set
            {
                SetAgeNumber(8, value);
                RaisePropertyChanged(() => Age8);
            }
        }


        public int? Age9
        {
            get { return GetAgeNumber(9); }
            set
            {
                SetAgeNumber(9, value);
                RaisePropertyChanged(() => Age9);
            }
        }


        public int? Age10
        {
            get { return GetAgeNumber(10); }
            set
            {
                SetAgeNumber(10, value);
                RaisePropertyChanged(() => Age10);
            }
        }

        public int? Age11
        {
            get { return GetAgeNumber(11); }
            set
            {
                SetAgeNumber(11, value);
                RaisePropertyChanged(() => Age11);
            }
        }

        public int? Age12
        {
            get { return GetAgeNumber(12); }
            set
            {
                SetAgeNumber(12, value);
                RaisePropertyChanged(() => Age12);
            }
        }

        public int? Age13
        {
            get { return GetAgeNumber(13); }
            set
            {
                SetAgeNumber(13, value);
                RaisePropertyChanged(() => Age13);
            }
        }

        public int? Age14
        {
            get { return GetAgeNumber(14); }
            set
            {
                SetAgeNumber(14, value);
                RaisePropertyChanged(() => Age14);
            }
        }

        public int? Age15
        {
            get { return GetAgeNumber(15); }
            set
            {
                SetAgeNumber(15, value);
                RaisePropertyChanged(() => Age15);
            }
        }

        public int AgeSum
        {
            get { return _lstAges == null || _lstAges.Count == 0 ? 0 : _lstAges.Where(x => x.OfflineState != ObjectState.Deleted && x.ChangeTracker.State != ObjectState.Deleted).Sum(x => x.number); }
        }

        #endregion


        /// <summary>
        /// Returns whether the age for the animal was aged by smartdots.
        /// </summary>
        public bool IsAgedByAquaDots
        {
            get
            {
                var age =  (_lstAges.Count == 0 || _lstAges.First().ChangeTracker.State == ObjectState.Deleted) ? null : _lstAges.First();

                if (age != null && age.sdAgeInfoUpdated.HasValue && age.sdAgeInfoUpdated.Value)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Returns the age (if any).
        /// </summary>
        public Age SFAgeEntity
        {
            get
            {
                var age = (_lstAges.Count == 0 || _lstAges.First().ChangeTracker.State == ObjectState.Deleted) ? null : _lstAges.First();

                return age;
            }
        }


        public string EdgeStructure
        {
            get { return _lstAges.Count == 0 || _lstAges.First().ChangeTracker.State == ObjectState.Deleted ? null : _lstAges.First().edgeStructure; }
            set
            {
                if (_lstAges.Count == 0)
                    _lstAges.Add(new Age());

                Age a = _lstAges.First();
                a.edgeStructure = value;
                a.number = 1; //Set number = 1 for all single fish ages.
                HandleSingleFishAgeDeletion();

                IsDirty = true;
                RaisePropertyChanged(() => EdgeStructure);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }



        public int? HatchMonth
        {
            get { return _lstAges.Count == 0 || _lstAges.First().ChangeTracker.State == ObjectState.Deleted ? new Nullable<int>() : _lstAges.First().hatchMonth; }
            set
            {
                if (_lstAges.Count == 0)
                    _lstAges.Add(new Age());
                Age a = _lstAges.First();
                a.hatchMonth = value;
                a.number = 1; //Set number = 1 for all single fish ages.
                HandleSingleFishAgeDeletion();

                IsDirty = true;
                RaisePropertyChanged(() => HatchMonth);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public int? ParasiteId
        {
            get { return _animalInfo.parasiteId; }
            set
            {
                _animalInfo.parasiteId = value;
                IsDirty = true;
                RaisePropertyChanged(() => ParasiteId);
                RaisePropertyChanged(() => Parasite);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string Parasite
        {
            get
            {
                if (!_animalInfo.parasiteId.HasValue)
                    return null;

                var m = _parent.Parent.Parasites.Where(x => x.L_parasiteId == _animalInfo.parasiteId.Value).FirstOrDefault();
                return m == null ? null : (m.num + (!string.IsNullOrWhiteSpace(m.description) ? " - " + m.description.ToString() : ""));
            }
        }


        public decimal? WeightGutted
        {
            get { return _animalInfo.weightGutted; }
            set
            {
                _animalInfo.weightGutted = value;
                IsDirty = true;
                RaisePropertyChanged(() => WeightGutted);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public decimal? WeightGonads
        {
            get { return _animalInfo.weightGonads; }
            set
            {
                _animalInfo.weightGonads = value;
                IsDirty = true;
                RaisePropertyChanged(() => WeightGonads);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public decimal? WeightLiver
        {
            get { return _animalInfo.weightLiver; }
            set
            {
                _animalInfo.weightLiver = value;
                IsDirty = true;
                RaisePropertyChanged(() => WeightLiver);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }

        public int? StomachStatusFirstEvaluationId
        {
            get { return _animalInfo.stomachStatusFirstEvaluationId; }
            set
            {
                _animalInfo.stomachStatusFirstEvaluationId = value;
                IsDirty = true;
                RaisePropertyChanged(() => StomachStatusFirstEvaluationId);
                RaisePropertyChanged(() => StomachStatusFirstEvaluation);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public string StomachStatusFirstEvaluation
        {
            get
            {
                if (!_animalInfo.stomachStatusFirstEvaluationId.HasValue)
                    return null;

                var m = _parent.Parent.StomachStatusFirstEvaluations.Where(x => x.L_StomachStatusId == _animalInfo.stomachStatusFirstEvaluationId.Value).FirstOrDefault();
                return m == null ? null : m.stomachStatus.ToString();
            }
        }


        public List<L_Reference> References
        {
            get { return _lstReferences; }
            set
            {
                _lstReferences = value;
                RaisePropertyChanged(() => References);
                RaisePropertyChanged(() => HasUnsavedData);
                RaisePropertyChanged(() => ReferencesString);
            }
        }


        public string ReferencesString
        {
            get
            {
                if (_lstReferences == null)
                    return null;

                var lst = _lstReferences.Where(x => x.IsChecked).Select(x => x.reference).ToList();

                RaisePropertyChanged(() => HasUnsavedData);
                return lst.Count == 0 ? "" : String.Join(", ", lst);
            }
        }


        public int AnimalFilesCount
        {
            get
            {
                if (_lstAnimalFiles == null)
                    return 0;

                int intCount = _lstAnimalFiles.Where(x => x.FileTypeEnum == AnimalFileType.OtolithImage).Count();

                RaisePropertyChanged(() => HasUnsavedData);
                return intCount;
            }
        }


        public List<AnimalFile> AnimalFiles
        {
            get { return _lstAnimalFiles; }
            set
            {
                _lstAnimalFiles = value;
                RaisePropertyChanged(() => AnimalFiles);
                RaisePropertyChanged(() => HasAnimalFiles);
            }
        }


        public bool HasAnimalFiles
        {
            get { return _lstAnimalFiles != null && _lstAnimalFiles.Count > 0; }
        }



        public bool IsAnimalFilesPopupOpen
        {
            get { return _blnIsAnimalFilesPopupOpen; }
            set
            {
                //Make sure popup cannot be shown if not animal files exist.
                if (value && !HasAnimalFiles)
                    return;

                _blnIsAnimalFilesPopupOpen = value;
                RaisePropertyChanged(() => IsAnimalFilesPopupOpen);
            }
        }


        public string AnimalFilesString
        {
            get
            {
                if (_lstAnimalFiles == null)
                    return "";

                var lst = _lstAnimalFiles.Where(x => x.FileTypeEnum == AnimalFileType.OtolithImage).Select(x => System.IO.Path.GetFileName(x.filePath)).ToList();

                RaisePropertyChanged(() => HasUnsavedData);
                return lst.Count == 0 ? "" : String.Join(", ", lst);
            }
        }


        public string Remark
        {
            get { return _animalEntity.remark; }
            set
            {
                _animalEntity.remark = value;
                IsDirty = true;
                RaisePropertyChanged(() => Remark);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public bool CanEditOffline
        {
            get { return !BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline || IsNew || (IsLoading || (_animalEntity != null && _animalEntity.OfflineState == ObjectState.Added)); }
        }

        #endregion



        /// <summary>
        /// Hide default constructor forcing paramterized constructor.
        /// </summary>
        private AnimalItem() { }

        private bool? _prevGenetics = null;

        public AnimalItem(ALavSFViewModel aVm, Animal a, List<Age> lstAges, List<AnimalFile> lstAnimalFiles, List<AnimalInfo> lstAnimalInfos, List<R_AnimalInfoReference> lstR_References)
        {
            _parent = aVm;
            _animalEntity = a;
            _lstAges = lstAges;
            _lstAnimalFiles = lstAnimalFiles;
            _lstR_References = lstR_References;

            if (_animalEntity == null)
            {
                _animalEntity = new Animal();
                _blnNew = true;
            }
            else
            {
                _intLength = a.length.HasValue ? Convert(a.length.Value, "mm", a.lengthMeasureUnit) : new Nullable<int>();
            }

            if (_lstAges == null)
                _lstAges = new List<Age>();

            if (_lstAnimalFiles == null)
                _lstAnimalFiles = new List<AnimalFile>();

            if (lstAnimalInfos != null && lstAnimalInfos.Count > 0)
            {
                if (lstAnimalInfos.Count > 1)
                    throw new ApplicationException("More than one AnimalInfo record was found to an Animal (which should not be possible). Please contact an administrator and report the problem.");

                _animalInfo = lstAnimalInfos.FirstOrDefault();
            }
            else
                _animalInfo = new AnimalInfo();

            if (_animalInfo.ChangeTracker.State != ObjectState.Added && lstR_References != null)
                _lstR_References = lstR_References.Where(x => x.animalInfoId == _animalInfo.animalInfoId).ToList();

            if (_lstR_References == null)
                _lstR_References = new List<R_AnimalInfoReference>();


            if (_parent.IsSFType )
            {
                //Default genetics to false ("No" genetics) for new rows. Old row, should be allowed NULL values.
                if (IsNew)
                {
                    if (_lstAges.Count == 0)
                        _lstAges.Add(new Age());

                    Age ag = _lstAges.First();
                    ag.genetics = false;
                    ag.number = 1; //Set number = 1 for all single fish ages.
                }

                if (_lstAges.Count > 0)
                    _prevGenetics = _lstAges.First().genetics;
            }

 
            PopulateReferences();
        }

       
        /// <summary>
        /// This method is only used on SingleFish forms. But it deletes the single Age record if all of its properties
        /// are null (because then it is not needed).
        /// </summary>
        private void HandleSingleFishAgeDeletion()
        {
            if (_lstAges.Count == 1)
            {
                var a = _lstAges.First();

                if (a.age1 == null && a.otolithReadingRemarkId == null && a.otolithWeight == null && a.hatchMonth == null && a.hatchMonthReadabilityId == null && a.edgeStructure == null && a.genetics == null && a.visualStockId == null && a.geneticStockId == null)
                {
                    if (a.ChangeTracker.State == ObjectState.Added)
                        _lstAges.Remove(a);
                    else
                        a.MarkAsDeleted();
                }
                else
                {
                    if (a.ChangeTracker.State == ObjectState.Unchanged || a.ChangeTracker.State == ObjectState.Deleted)
                        a.MarkAsModified();
                }

            }
        }

        private void PopulateReferences()
        {
            if (_parent == null || _parent.Parent == null || _parent.Parent.References == null)
            {
                _lstReferences = new List<L_Reference>();
                return;
            }

            var lstReferences = _parent.Parent.References.Clone();

            _lstR_References.ForEach(x =>
            {
                L_Reference lRef = null;
                if ((lRef = lstReferences.Where(y => y.L_referenceId == x.L_referenceId).FirstOrDefault()) != null)
                {
                    lRef.IsChecked = true;
                    lRef.IsDirty = false;
                }
            });

            new Action(() =>
            {
                References = lstReferences;
            }).Dispatch();
        }


        private Age GetFirstAge()
        {
            return _lstAges.Count == 0 ? null : _lstAges.FirstOrDefault();
        }


        /// <summary>
        /// Create a new species list item of type T
        /// </summary>
        public static AnimalItem New(ALavSFViewModel slvm, int intSubSampleId, int intIndex)
        {
            var aItem = new AnimalItem(slvm, null, null, null, null, null);
            aItem.Index = intIndex;
            aItem.AnimalEntity.subSampleId = intSubSampleId;

            return aItem;
        }


        /// <summary>
        /// Assign Age number (converter is used on UI to call this method)
        /// </summary>
        public void SetAgeNumber(int intAge, int? number)
        {
            //System.Diagnostics.Debug.WriteLine("SetAgeNumber. Age: " + intAge + ", Number: " + number);
            Age a = _lstAges.Where(x => x.age1.HasValue && x.age1.Value == intAge).FirstOrDefault();

            if (a == null && number.HasValue)
            {
                a = new Age();
                if (_animalEntity.ChangeTracker.State != ObjectState.Added)
                    a.animalId = _animalEntity.animalId;
                _lstAges.Add(a);
                a.age1 = intAge;
                a.number = number.Value;
                a.ageMeasureMethodId = 1; //Set default measure method (not used and there is only one)
            }
            else if (a != null)
            {
                if (number.HasValue)
                {
                    a.number = number.Value;

                    //age is actually deleted or unchaged, mark it as modified now.
                    if (a.ChangeTracker.State == ObjectState.Unchanged || a.ChangeTracker.State == ObjectState.Deleted)
                        a.MarkAsModified();
                }
                else
                {
                    //Number is set to null - either remove the age from the list or mark it as modified (depending on if it exists).
                    if (a.ChangeTracker.State == ObjectState.Added)
                        _lstAges.Remove(a);
                    else
                        a.MarkAsDeleted();
                }
            }

            RaisePropertyChanged(() => AnimalAgeUI);
            RaisePropertyChanged(() => HasUnsavedData);
        }


        /// <summary>
        /// Retrieve Age number (converter is used on UI to call this method)
        /// </summary>
        public int? GetAgeNumber(int intAge)
        {
            Age a = _lstAges.Where(x => x.age1.HasValue && x.age1.Value == intAge).FirstOrDefault();

            int? intNumber = null;

            if (a != null && a.ChangeTracker.State != ObjectState.Deleted)
                intNumber = a.number;

            //System.Diagnostics.Debug.WriteLine("GetAgeNumber. Age: " + intAge + ", Number: " + intNumber);
            return intNumber;
        }


        /// <summary>
        /// Mark animal item as deleted.
        /// </summary>
        public void MarkAsDeleted()
        {
            if (IsNew)
                return;

            _animalEntity.MarkAsDeleted();

            foreach (var a in _lstAges.ToList())
                if (a.ChangeTracker.State == ObjectState.Added)
                    _lstAges.Remove(a);
                else
                    a.MarkAsDeleted();

            foreach (var af in _lstAnimalFiles)
                if (af.ChangeTracker.State == ObjectState.Added)
                    _lstAnimalFiles.Remove(af);
                else
                    af.MarkAsDeleted();

            if (_animalEntity != null)
            {
                if (_animalEntity.ChangeTracker.State == ObjectState.Added)
                    _animalEntity = null;
                else
                    _animalEntity.MarkAsDeleted();
            }

            foreach (var r_r in _lstR_References.ToList())
                r_r.MarkAsDeleted();

            RaisePropertyChanged(() => HasUnsavedData);
        }



        public LavSFTransferItem GetUnsavedData()
        {
            if (!HasUnsavedData)
                return null;
            else
            {
                bool blnIsOffline = BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline;

                LavSFTransferItem ti = new LavSFTransferItem();

                //Handle saving of length
                if (_intLength.HasValue)
                {
                    int intLength = Convert(_intLength.Value, _parent.LengthMeasureUnit, "mm");

                    if (intLength != _animalEntity.length)
                        _animalEntity.length = intLength;
                }
                else if (_animalEntity.length.HasValue)
                    _animalEntity.length = null;



                ti.Animal = _animalEntity;
                ti.AnimalInfo = _animalInfo;

                //Add changed ages.
                if (_lstAges != null && _lstAges.Count > 0)
                    ti.Ages = _lstAges.Where(x => blnIsOffline || x.ChangeTracker.State != ObjectState.Unchanged).ToList();

                //Add changed animalfiles
                if (_lstAnimalFiles != null && _lstAnimalFiles.Count > 0)
                    ti.AnimalFiles = _lstAnimalFiles.Where(x => blnIsOffline || x.ChangeTracker.State != ObjectState.Unchanged).ToList();


                List<R_AnimalInfoReference> lstR_Ref = new List<R_AnimalInfoReference>();

                //If AnimalItem is to be deleted, just attach all R_AnimalInfoReferences, since these will be marked as deleted.
                if (IsDeleted)
                    lstR_Ref = _lstR_References.ToList();
                else
                {
                    //Synchronize list of R_AnimailInfoReference objects to user selection
                    foreach (var v in _lstReferences)
                    {
                        R_AnimalInfoReference r_ref = _lstR_References.Where(x => x.L_referenceId == v.L_referenceId).FirstOrDefault();

                        if (v.IsChecked)
                        {
                            //If r_ref is new, create it
                            if (r_ref == null)
                                r_ref = new R_AnimalInfoReference();

                            //if L_refrerence id is different, set it.
                            if (r_ref.L_referenceId != v.L_referenceId)
                                r_ref.L_referenceId = v.L_referenceId;
                        }
                        else
                        {
                            if (r_ref != null)
                                r_ref.MarkAsDeleted();
                        }

                        if (r_ref != null && r_ref.ChangeTracker.State != ObjectState.Unchanged)
                            lstR_Ref.Add(r_ref);
                    }
                }

                ti.AnimalInfoReferences = lstR_Ref;

                return ti;
            }
        }


        #region Validation methods

        public override void ValidateAllProperties(bool blnRefreshProperties = false, bool raisePropertyChangedOnProperties = true)
        {
         //   _strLastErrorProperty = _strLastErrorMsg = null;
            _dicLastErrorMessages.Clear();

            base.ValidateAllProperties(true, raisePropertyChangedOnProperties);
        }


        /// <summary>
        /// Hide base method ValidateField for any inherited classes of this class (they should override ValidateListItem instead).
        /// </summary>
        protected override string ValidateField(string strFieldName)
        {
            string strError = null;

            if (_blnValidate)
            {
                switch (strFieldName)
                {
                    case "HatchMonth":
                        if (HatchMonth.HasValue && (HatchMonth.Value < 1 || HatchMonth.Value > 12))
                            strError = "Angiv venligst en klækningsmåned mellem 1 og 12";

                        break;

                    case "Length":
                        if (!(IsNew && !IsDirty))
                        {
                            var vDoubles = _parent.Items.Where(x => x != this && (x.Length == Length &&
                                                                                  x.Sex == Sex &&
                                                                                  x.BroodingPhase == BroodingPhase &&
                                                                                  x.IndividNum == IndividNum
                                                                                 ));
                            if (vDoubles.Count() > 0)
                            {
                                if (_parent.IsLAVType)
                                    strError = string.Format("Der findes allerede en anden række med samme køn, rugefase og en længde på '{0}'", Length.HasValue ? Length.Value.ToString() : "");
                                else
                                    strError = string.Format("Der findes allerede en anden række med samme køn og en længde på '{0}'", Length.HasValue ? Length.Value.ToString() : "");
                            }

                            if (_parent.IsSFType)
                            {
                                if (IsRowEmptySF)
                                    strError = string.Format("Rækken er tom, udfyld eller slet den venligst.");
                            }
                        }

                        //Performn length min/max checks for "lav, rep" and both single fish tabels.
                        if (strError == null)
                            strError = GetLengthOutOfBoundsWarning();

                        break;

                    case "WeightGutted":
                        if(WeightGutted.HasValue && _parent.IsSFType && WeightGutted.Value <= 0)
                            strError = string.Format("Renset vægt (R-vægt) må ikke være mindre end eller lig med 0.");
                        break;

                    case "WeightGonads":
                        if (WeightGonads.HasValue && _parent.IsSFType && WeightGonads.Value <= 0)
                            strError = string.Format("Vægten af gonaderne (G-vægt) må ikke være mindre end eller lig med 0.");
                        break;

                    case "WeightLiver":
                        if (WeightLiver.HasValue && _parent.IsSFType && WeightLiver.Value <= 0)
                            strError = string.Format("Vægten af lever (L-vægt) må ikke være mindre end eller lig med 0.");
                        break;

                    case "WeightInGrams":
                        if (WeightInGrams.HasValue && _parent.IsSFType)
                        {
                            decimal dblSumWeightGrams = 0;

                            if (WeightLiver.HasValue && WeightLiver.Value > 0)
                                dblSumWeightGrams += WeightLiver.Value;

                            if (WeightGutted.HasValue && WeightGutted.Value > 0)
                                dblSumWeightGrams += WeightGutted.Value;

                            if (WeightGonads.HasValue && WeightGonads.Value > 0)
                                dblSumWeightGrams += WeightGonads.Value;

                            if (WeightInGrams.Value < dblSumWeightGrams)
                            {
                                strError = string.Format("Summen af vægtene for renset fisk (R-vægt), lever (L-vægt) og gonader (G-vægt) er større end 'Vægt (g)'");
                            }
                        }

                        //Performn weight min/max checks for "lav, rep" and both single fish tabels.
                        if (strError == null)
                            strError = GetWeightOutOfBoundsWarning();
                        break;

                    case "Age":
                        if (_parent.IsSFType && Age.HasValue)
                        {
                            var age = _lstAges.FirstOrDefault();

                            if (age != null && age.age1.HasValue)
                                strError = GetAgeOutOfBoundsWarning(age.age1.Value);
                        }
                        break;

                    case "SelectedGeneticsString":
                        var sg = SelectedGenetics;
                        //For new rows and for rows previously saved with a value for genetics, require genetics selected.
                        if (_parent.IsSFType && ((IsNew && sg == null) || (sg == null && _prevGenetics.HasValue)))
                        {
                            strError = "Vælg venligst genetik 'ja'/'nej' for rækken.";
                        }
                        break;
                }

                if(_parent.LavSFType.Equals("LAV", StringComparison.InvariantCultureIgnoreCase) && strError == null && strFieldName != null && strFieldName.StartsWith("Age"))
                {
                    var nr = strFieldName.Replace("Age", "");
                    int intAge = -1;

                    if (int.TryParse(nr, out intAge))
                        strError = GetAgeOutOfBoundsWarning(intAge);
                }

                //Save the property name of the property raising the error
                if (strError != null)
                {
                    if (_dicLastErrorMessages.ContainsKey(strFieldName))
                        _dicLastErrorMessages[strFieldName] = strError;
                    else
                        _dicLastErrorMessages.Add(strFieldName, strError);
                    //_strLastErrorProperty = strFieldName;
                    //_strLastErrorMsg = strError;
                }
            }
            //If this is a normal error check, return the same error as found during the manual error check if the
            //property being checked is the same as the property checked during the manual error checking.
            else if (_dicLastErrorMessages.ContainsKey(strFieldName) /*_strLastErrorProperty == strFieldName*/)
            {
                return _dicLastErrorMessages[strFieldName]; //_strLastErrorMsg;
            }

            return strError;
        }

        private Dictionary<string, string> _dicLastErrorMessages = new Dictionary<string, string>();


        public bool IsAnyAgesOutOfBounds()
        {
            try
            {
                if (_lstAges != null && _lstAges.Count > 0)
                {
                    foreach (var age in _lstAges)
                    {
                        if (age.age1.HasValue && GetAgeOutOfBoundsWarning(age.age1.Value) != null)
                            return true;
                    }
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return false;
        }


        private string GetAgeOutOfBoundsWarning(int intAge)
        {
            string strError = null;

            try
            {
                if (_parent != null && _parent.Parent != null && _parent.Parent.Species != null)
                {
                    var age = GetAgeNumber(intAge);

                    if (age.HasValue)
                    {
                        if (_parent.Parent.Species.ageMin.HasValue && age.Value > 0 && _parent.Parent.Species.ageMin.Value > intAge)
                        {
                            if(_parent.LavSFType.Equals("LAV", StringComparison.InvariantCultureIgnoreCase))
                                strError = string.Format("{0}Der er angivet aldre ({1} år) som er mindre end den forventede minimum alder ({2} år) for arten.", AViewModel.WarningPrefix, intAge, _parent.Parent.Species.ageMin.Value);
                            else
                                strError = string.Format("{0}Der er angivet en alder ({1} år) som er mindre end den forventede minimum alder ({2} år) for arten.", AViewModel.WarningPrefix, intAge, _parent.Parent.Species.ageMin.Value);
                        }
                        else if (_parent.Parent.Species.ageMax.HasValue && age.Value > 0 && intAge > _parent.Parent.Species.ageMax.Value)
                        {
                            if (_parent.LavSFType.Equals("LAV", StringComparison.InvariantCultureIgnoreCase))
                                strError = string.Format("{0}Der er angivet aldre ({1} år) som er større end den forventede maximum alder ({2} år) for arten.", AViewModel.WarningPrefix, intAge, _parent.Parent.Species.ageMax.Value);
                            else
                                strError = string.Format("{0}Der er angivet en alder ({1} år) som er større end den forventede maximum alder ({2} år) for arten.", AViewModel.WarningPrefix, intAge, _parent.Parent.Species.ageMax.Value);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            

            return strError;
        }
        

        /// <summary>
        /// Method returns a warning, if the weight on animal item is out of bounds of what is specified for the species.
        /// </summary>
        public string GetLengthOutOfBoundsWarning()
        {
            string strError = null;

            try
            {
                //Performn weight min/max checks for "lav, rep" and both single fish tabels.
                if (Length.HasValue && _parent != null && _parent.Parent != null && _parent.LengthMeasureUnit != null && _parent.Parent.Species != null)
                {
                    decimal length = Convert(Length.Value, _parent.LengthMeasureUnit, "mm");

                    if (_parent.Parent.Species.lengthMin.HasValue && _parent.Parent.Species.lengthMin.Value > length)
                        strError = string.Format("{0}Længden ({1}mm) er mindre end den forventede minimum længde ({2}mm) for arten.", AViewModel.WarningPrefix, length, _parent.Parent.Species.lengthMin.Value);
                    else if (_parent.Parent.Species.lengthMax.HasValue && length > _parent.Parent.Species.lengthMax.Value)
                        strError = string.Format("{0}Længden ({1}mm) er større end den forventede maximum længde ({2}mm) for arten.", AViewModel.WarningPrefix, length, _parent.Parent.Species.lengthMax.Value);
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return strError;
        }

        /// <summary>
        /// Method returns a warning, if the weight on animal item is out of bounds of what is specified for the species.
        /// </summary>
        public string GetWeightOutOfBoundsWarning()
        {
            string strError = null;

            //Performn weight min/max checks for "lav, rep" and both single fish tabels.
            if (WeightInGrams.HasValue && _parent != null && _parent.Parent != null && _parent.Parent.Species != null && _parent.Parent.TreatmentFactor.HasValue && Number > 0)
            {
                decimal decWeight = (WeightInGrams.Value * _parent.Parent.TreatmentFactor.Value) / Number;

                if (_parent.Parent.Species.weightMin.HasValue && _parent.Parent.Species.weightMin.Value > decWeight)
                    strError = string.Format("{0}Vægt ({1}g) er mindre end den forventede minimum vægt (UR) ({2}g) for arten.", AViewModel.WarningPrefix, decWeight.ToString("0.##"), _parent.Parent.Species.weightMin.Value.ToString("0.##"));
                else if (_parent.Parent.Species.weightMax.HasValue && decWeight > _parent.Parent.Species.weightMax.Value)
                    strError = string.Format("{0}Vægt ({1}g) er større end den forventede maximum vægt (UR) ({2}g) for arten.", AViewModel.WarningPrefix, decWeight.ToString("0.##"), _parent.Parent.Species.weightMax.Value.ToString("0.##"));
            }

            return strError;
        }


        public bool ShouldForceValidate()
        {
            if (_parent.LavSFType.Equals("SF", StringComparison.InvariantCultureIgnoreCase))
            {
                return IsRowEmptySF;
            }

            return false;
        }


        #endregion


        public static int Convert(int dblValue, string strFrom, string strTo)
        {
            if (strFrom.Equals("mm", StringComparison.InvariantCultureIgnoreCase))
            {
                if (strTo.Equals("mm", StringComparison.InvariantCultureIgnoreCase))
                    return dblValue;
                else if (strTo.Equals("cm", StringComparison.InvariantCultureIgnoreCase))
                    return dblValue / 10;
                else if (strTo.Equals("sc", StringComparison.InvariantCultureIgnoreCase))
                    return dblValue / 5;
            }
            else if (strFrom.Equals("cm", StringComparison.InvariantCultureIgnoreCase))
            {
                if (strTo.Equals("mm", StringComparison.InvariantCultureIgnoreCase))
                    return dblValue * 10;
                else if (strTo.Equals("cm", StringComparison.InvariantCultureIgnoreCase))
                    return dblValue;
                else if (strTo.Equals("sc", StringComparison.InvariantCultureIgnoreCase))
                    return dblValue * 2;
            }
            else if (strFrom.Equals("sc", StringComparison.InvariantCultureIgnoreCase))
            {
                if (strTo.Equals("mm", StringComparison.InvariantCultureIgnoreCase))
                    return dblValue * 5;
                else if (strTo.Equals("cm", StringComparison.InvariantCultureIgnoreCase))
                    return dblValue / 2;
                else if (strTo.Equals("sc", StringComparison.InvariantCultureIgnoreCase))
                    return dblValue;
            }

            throw new ApplicationException("Length measuring unit is not supported.");
        }


        public static void GoToPath(string strPath)
        {
            if (string.IsNullOrWhiteSpace(strPath))
                return;

            try
            {
                // string args = string.Format("/select, \"{0}\"", strPath);
                string args = "explorer.exe /select,\"Z:\\Projects\\Babelfisk\\OtolithImages\\Profile1.bmp\"";

                Process p = new Process();
              
                var pi = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = string.Format("/select,\"{0}\"", strPath),
                    UseShellExecute = true
                };

                p.StartInfo = pi;

                p.Start();

                p.WaitForExit();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);

                new Action(() =>
                {
                    if (e != null && e.Message != null && e.Message.Contains("The system cannot find the drive specified"))
                        _appRegionManager.ShowMessageBox("Computeren kunne ikke åbne/finde stien: " + strPath);
                    else
                        _appRegionManager.ShowMessageBox("En uventet fejl opstod. " + e.Message);
                }).Dispatch();
            }
        }
    }
}
