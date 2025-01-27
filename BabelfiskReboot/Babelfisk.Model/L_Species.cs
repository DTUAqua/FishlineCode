//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Babelfisk.Entities.Sprattus
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(Est_MethodStep))]
    [KnownType(typeof(L_TreatmentFactorGroup))]
    [KnownType(typeof(R_TargetSpecies))]
    [KnownType(typeof(SpeciesList))]
    [KnownType(typeof(R_StockSpeciesArea))]
    [KnownType(typeof(R_SDReader))]
    [KnownType(typeof(SDSample))]
    [KnownType(typeof(L_LengthMeasureType))]
    public partial class L_Species: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int L_speciesId
        {
            get { return _l_speciesId; }
            set
            {
                if (_l_speciesId != value)
                {
                    _l_speciesId = value;
                    OnPropertyChanged("L_speciesId");
                }
            }
        }
        private int _l_speciesId;
    
        [DataMember]
        public string speciesCode
        {
            get { return _speciesCode; }
            set
            {
                if (_speciesCode != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'speciesCode' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _speciesCode = value;
                    OnPropertyChanged("speciesCode");
                }
            }
        }
        private string _speciesCode;
    
        [DataMember]
        public string dkName
        {
            get { return _dkName; }
            set
            {
                if (_dkName != value)
                {
                    _dkName = value;
                    OnPropertyChanged("dkName");
                }
            }
        }
        private string _dkName;
    
        [DataMember]
        public string ukName
        {
            get { return _ukName; }
            set
            {
                if (_ukName != value)
                {
                    _ukName = value;
                    OnPropertyChanged("ukName");
                }
            }
        }
        private string _ukName;
    
        [DataMember]
        public string nodc
        {
            get { return _nodc; }
            set
            {
                if (_nodc != value)
                {
                    _nodc = value;
                    OnPropertyChanged("nodc");
                }
            }
        }
        private string _nodc;
    
        [DataMember]
        public string latin
        {
            get { return _latin; }
            set
            {
                if (_latin != value)
                {
                    _latin = value;
                    OnPropertyChanged("latin");
                }
            }
        }
        private string _latin;
    
        [DataMember]
        public string treatmentFactorGroup
        {
            get { return _treatmentFactorGroup; }
            set
            {
                if (_treatmentFactorGroup != value)
                {
                    ChangeTracker.RecordOriginalValue("treatmentFactorGroup", _treatmentFactorGroup);
                    if (!IsDeserializing)
                    {
                        if (L_TreatmentFactorGroup != null && L_TreatmentFactorGroup.treatmentFactorGroup != value)
                        {
                            L_TreatmentFactorGroup = null;
                        }
                    }
                    _treatmentFactorGroup = value;
                    OnPropertyChanged("treatmentFactorGroup");
                }
            }
        }
        private string _treatmentFactorGroup;
    
        [DataMember]
        public string dfuFisk_Code
        {
            get { return _dfuFisk_Code; }
            set
            {
                if (_dfuFisk_Code != value)
                {
                    _dfuFisk_Code = value;
                    OnPropertyChanged("dfuFisk_Code");
                }
            }
        }
        private string _dfuFisk_Code;
    
        [DataMember]
        public string tsn
        {
            get { return _tsn; }
            set
            {
                if (_tsn != value)
                {
                    _tsn = value;
                    OnPropertyChanged("tsn");
                }
            }
        }
        private string _tsn;
    
        [DataMember]
        public Nullable<int> aphiaID
        {
            get { return _aphiaID; }
            set
            {
                if (_aphiaID != value)
                {
                    _aphiaID = value;
                    OnPropertyChanged("aphiaID");
                }
            }
        }
        private Nullable<int> _aphiaID;
    
        [DataMember]
        public Nullable<int> standardLengthMeasureTypeId
        {
            get { return _standardLengthMeasureTypeId; }
            set
            {
                if (_standardLengthMeasureTypeId != value)
                {
                    ChangeTracker.RecordOriginalValue("standardLengthMeasureTypeId", _standardLengthMeasureTypeId);
                    if (!IsDeserializing)
                    {
                        if (L_StandardLengthMeasureType != null && L_StandardLengthMeasureType.L_lengthMeasureTypeId != value)
                        {
                            L_StandardLengthMeasureType = null;
                        }
                    }
                    _standardLengthMeasureTypeId = value;
                    OnPropertyChanged("standardLengthMeasureTypeId");
                }
            }
        }
        private Nullable<int> _standardLengthMeasureTypeId;
    
        [DataMember]
        public Nullable<int> lengthMin
        {
            get { return _lengthMin; }
            set
            {
                if (_lengthMin != value)
                {
                    _lengthMin = value;
                    OnPropertyChanged("lengthMin");
                }
            }
        }
        private Nullable<int> _lengthMin;
    
        [DataMember]
        public Nullable<int> lengthMax
        {
            get { return _lengthMax; }
            set
            {
                if (_lengthMax != value)
                {
                    _lengthMax = value;
                    OnPropertyChanged("lengthMax");
                }
            }
        }
        private Nullable<int> _lengthMax;
    
        [DataMember]
        public Nullable<int> ageMin
        {
            get { return _ageMin; }
            set
            {
                if (_ageMin != value)
                {
                    _ageMin = value;
                    OnPropertyChanged("ageMin");
                }
            }
        }
        private Nullable<int> _ageMin;
    
        [DataMember]
        public Nullable<int> ageMax
        {
            get { return _ageMax; }
            set
            {
                if (_ageMax != value)
                {
                    _ageMax = value;
                    OnPropertyChanged("ageMax");
                }
            }
        }
        private Nullable<int> _ageMax;
    
        [DataMember]
        public Nullable<int> weightMin
        {
            get { return _weightMin; }
            set
            {
                if (_weightMin != value)
                {
                    _weightMin = value;
                    OnPropertyChanged("weightMin");
                }
            }
        }
        private Nullable<int> _weightMin;
    
        [DataMember]
        public Nullable<int> weightMax
        {
            get { return _weightMax; }
            set
            {
                if (_weightMax != value)
                {
                    _weightMax = value;
                    OnPropertyChanged("weightMax");
                }
            }
        }
        private Nullable<int> _weightMax;
    
        [DataMember]
        public string speciesFAO
        {
            get { return _speciesFAO; }
            set
            {
                if (_speciesFAO != value)
                {
                    _speciesFAO = value;
                    OnPropertyChanged("speciesFAO");
                }
            }
        }
        private string _speciesFAO;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<Est_MethodStep> Est_MethodStep
        {
            get
            {
                if (_est_MethodStep == null)
                {
                    _est_MethodStep = new TrackableCollection<Est_MethodStep>();
                    _est_MethodStep.CollectionChanged += FixupEst_MethodStep;
                }
                return _est_MethodStep;
            }
            set
            {
                if (!ReferenceEquals(_est_MethodStep, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_est_MethodStep != null)
                    {
                        _est_MethodStep.CollectionChanged -= FixupEst_MethodStep;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (Est_MethodStep item in _est_MethodStep)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _est_MethodStep = value;
                    if (_est_MethodStep != null)
                    {
                        _est_MethodStep.CollectionChanged += FixupEst_MethodStep;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (Est_MethodStep item in _est_MethodStep)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Est_MethodStep");
                }
            }
        }
        private TrackableCollection<Est_MethodStep> _est_MethodStep;
    
        [DataMember]
        public L_TreatmentFactorGroup L_TreatmentFactorGroup
        {
            get { return _l_TreatmentFactorGroup; }
            set
            {
                if (!ReferenceEquals(_l_TreatmentFactorGroup, value))
                {
                    var previousValue = _l_TreatmentFactorGroup;
                    _l_TreatmentFactorGroup = value;
                    FixupL_TreatmentFactorGroup(previousValue);
                    OnNavigationPropertyChanged("L_TreatmentFactorGroup");
                }
            }
        }
        private L_TreatmentFactorGroup _l_TreatmentFactorGroup;
    
        [DataMember]
        public TrackableCollection<R_TargetSpecies> R_TargetSpecies
        {
            get
            {
                if (_r_TargetSpecies == null)
                {
                    _r_TargetSpecies = new TrackableCollection<R_TargetSpecies>();
                    _r_TargetSpecies.CollectionChanged += FixupR_TargetSpecies;
                }
                return _r_TargetSpecies;
            }
            set
            {
                if (!ReferenceEquals(_r_TargetSpecies, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_r_TargetSpecies != null)
                    {
                        _r_TargetSpecies.CollectionChanged -= FixupR_TargetSpecies;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (R_TargetSpecies item in _r_TargetSpecies)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _r_TargetSpecies = value;
                    if (_r_TargetSpecies != null)
                    {
                        _r_TargetSpecies.CollectionChanged += FixupR_TargetSpecies;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (R_TargetSpecies item in _r_TargetSpecies)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("R_TargetSpecies");
                }
            }
        }
        private TrackableCollection<R_TargetSpecies> _r_TargetSpecies;
    
        [DataMember]
        public TrackableCollection<R_TargetSpecies> R_TargetSpecies1
        {
            get
            {
                if (_r_TargetSpecies1 == null)
                {
                    _r_TargetSpecies1 = new TrackableCollection<R_TargetSpecies>();
                    _r_TargetSpecies1.CollectionChanged += FixupR_TargetSpecies1;
                }
                return _r_TargetSpecies1;
            }
            set
            {
                if (!ReferenceEquals(_r_TargetSpecies1, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_r_TargetSpecies1 != null)
                    {
                        _r_TargetSpecies1.CollectionChanged -= FixupR_TargetSpecies1;
                    }
                    _r_TargetSpecies1 = value;
                    if (_r_TargetSpecies1 != null)
                    {
                        _r_TargetSpecies1.CollectionChanged += FixupR_TargetSpecies1;
                    }
                    OnNavigationPropertyChanged("R_TargetSpecies1");
                }
            }
        }
        private TrackableCollection<R_TargetSpecies> _r_TargetSpecies1;
    
        [DataMember]
        public TrackableCollection<SpeciesList> SpeciesList
        {
            get
            {
                if (_speciesList == null)
                {
                    _speciesList = new TrackableCollection<SpeciesList>();
                    _speciesList.CollectionChanged += FixupSpeciesList;
                }
                return _speciesList;
            }
            set
            {
                if (!ReferenceEquals(_speciesList, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_speciesList != null)
                    {
                        _speciesList.CollectionChanged -= FixupSpeciesList;
                    }
                    _speciesList = value;
                    if (_speciesList != null)
                    {
                        _speciesList.CollectionChanged += FixupSpeciesList;
                    }
                    OnNavigationPropertyChanged("SpeciesList");
                }
            }
        }
        private TrackableCollection<SpeciesList> _speciesList;
    
        [DataMember]
        public TrackableCollection<R_StockSpeciesArea> R_StockSpeciesArea
        {
            get
            {
                if (_r_StockSpeciesArea == null)
                {
                    _r_StockSpeciesArea = new TrackableCollection<R_StockSpeciesArea>();
                    _r_StockSpeciesArea.CollectionChanged += FixupR_StockSpeciesArea;
                }
                return _r_StockSpeciesArea;
            }
            set
            {
                if (!ReferenceEquals(_r_StockSpeciesArea, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_r_StockSpeciesArea != null)
                    {
                        _r_StockSpeciesArea.CollectionChanged -= FixupR_StockSpeciesArea;
                    }
                    _r_StockSpeciesArea = value;
                    if (_r_StockSpeciesArea != null)
                    {
                        _r_StockSpeciesArea.CollectionChanged += FixupR_StockSpeciesArea;
                    }
                    OnNavigationPropertyChanged("R_StockSpeciesArea");
                }
            }
        }
        private TrackableCollection<R_StockSpeciesArea> _r_StockSpeciesArea;
    
        [DataMember]
        public TrackableCollection<R_SDReader> R_SDReader
        {
            get
            {
                if (_r_SDReader == null)
                {
                    _r_SDReader = new TrackableCollection<R_SDReader>();
                    _r_SDReader.CollectionChanged += FixupR_SDReader;
                }
                return _r_SDReader;
            }
            set
            {
                if (!ReferenceEquals(_r_SDReader, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_r_SDReader != null)
                    {
                        _r_SDReader.CollectionChanged -= FixupR_SDReader;
                    }
                    _r_SDReader = value;
                    if (_r_SDReader != null)
                    {
                        _r_SDReader.CollectionChanged += FixupR_SDReader;
                    }
                    OnNavigationPropertyChanged("R_SDReader");
                }
            }
        }
        private TrackableCollection<R_SDReader> _r_SDReader;
    
        [DataMember]
        public TrackableCollection<SDSample> SDSample
        {
            get
            {
                if (_sDSample == null)
                {
                    _sDSample = new TrackableCollection<SDSample>();
                    _sDSample.CollectionChanged += FixupSDSample;
                }
                return _sDSample;
            }
            set
            {
                if (!ReferenceEquals(_sDSample, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_sDSample != null)
                    {
                        _sDSample.CollectionChanged -= FixupSDSample;
                    }
                    _sDSample = value;
                    if (_sDSample != null)
                    {
                        _sDSample.CollectionChanged += FixupSDSample;
                    }
                    OnNavigationPropertyChanged("SDSample");
                }
            }
        }
        private TrackableCollection<SDSample> _sDSample;
    
        [DataMember]
        public L_LengthMeasureType L_StandardLengthMeasureType
        {
            get { return _l_StandardLengthMeasureType; }
            set
            {
                if (!ReferenceEquals(_l_StandardLengthMeasureType, value))
                {
                    var previousValue = _l_StandardLengthMeasureType;
                    _l_StandardLengthMeasureType = value;
                    FixupL_StandardLengthMeasureType(previousValue);
                    OnNavigationPropertyChanged("L_StandardLengthMeasureType");
                }
            }
        }
        private L_LengthMeasureType _l_StandardLengthMeasureType;

        #endregion

        #region ChangeTracking
    
        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
            {
                ChangeTracker.State = ObjectState.Modified;
            }
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        protected virtual void OnNavigationPropertyChanged(String propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged{ add { _propertyChanged += value; } remove { _propertyChanged -= value; } }
        private event PropertyChangedEventHandler _propertyChanged;
        private ObjectChangeTracker _changeTracker;
    
        [DataMember]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if (_changeTracker == null)
                {
                    _changeTracker = new ObjectChangeTracker();
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
                return _changeTracker;
            }
            set
            {
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging -= HandleObjectStateChanging;
                }
                _changeTracker = value;
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
            }
        }
    
        private void HandleObjectStateChanging(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                ClearNavigationProperties();
            }
        }
    
        protected bool IsDeserializing { get; private set; }
    
        [OnDeserializing]
        public void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
        }
    
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            ChangeTracker.ChangeTrackingEnabled = true;
        }
    
        // This entity type is the dependent end in at least one association that performs cascade deletes.
        // This event handler will process notifications that occur when the principal end is deleted.
        internal void HandleCascadeDelete(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                this.MarkAsDeleted();
            }
        }
    
        protected virtual void ClearNavigationProperties()
        {
            Est_MethodStep.Clear();
            L_TreatmentFactorGroup = null;
            R_TargetSpecies.Clear();
            R_TargetSpecies1.Clear();
            SpeciesList.Clear();
            R_StockSpeciesArea.Clear();
            R_SDReader.Clear();
            SDSample.Clear();
            L_StandardLengthMeasureType = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupL_TreatmentFactorGroup(L_TreatmentFactorGroup previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.L_Species.Contains(this))
            {
                previousValue.L_Species.Remove(this);
            }
    
            if (L_TreatmentFactorGroup != null)
            {
                if (!L_TreatmentFactorGroup.L_Species.Contains(this))
                {
                    L_TreatmentFactorGroup.L_Species.Add(this);
                }
    
                treatmentFactorGroup = L_TreatmentFactorGroup.treatmentFactorGroup;
            }
            else if (!skipKeys)
            {
                treatmentFactorGroup = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_TreatmentFactorGroup")
                    && (ChangeTracker.OriginalValues["L_TreatmentFactorGroup"] == L_TreatmentFactorGroup))
                {
                    ChangeTracker.OriginalValues.Remove("L_TreatmentFactorGroup");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_TreatmentFactorGroup", previousValue);
                }
                if (L_TreatmentFactorGroup != null && !L_TreatmentFactorGroup.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_TreatmentFactorGroup.StartTracking();
                }
            }
        }
    
        private void FixupL_StandardLengthMeasureType(L_LengthMeasureType previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (L_StandardLengthMeasureType != null)
            {
                standardLengthMeasureTypeId = L_StandardLengthMeasureType.L_lengthMeasureTypeId;
            }
    
            else if (!skipKeys)
            {
                standardLengthMeasureTypeId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_StandardLengthMeasureType")
                    && (ChangeTracker.OriginalValues["L_StandardLengthMeasureType"] == L_StandardLengthMeasureType))
                {
                    ChangeTracker.OriginalValues.Remove("L_StandardLengthMeasureType");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_StandardLengthMeasureType", previousValue);
                }
                if (L_StandardLengthMeasureType != null && !L_StandardLengthMeasureType.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_StandardLengthMeasureType.StartTracking();
                }
            }
        }
    
        private void FixupEst_MethodStep(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Est_MethodStep item in e.NewItems)
                {
                    item.L_Species = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Est_MethodStep", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Est_MethodStep item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_Species, this))
                    {
                        item.L_Species = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Est_MethodStep", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Remove the previous dependent from the event listener.
                    ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                }
            }
        }
    
        private void FixupR_TargetSpecies(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (R_TargetSpecies item in e.NewItems)
                {
                    item.L_Species = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("R_TargetSpecies", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (R_TargetSpecies item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_Species, this))
                    {
                        item.L_Species = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("R_TargetSpecies", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Remove the previous dependent from the event listener.
                    ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                }
            }
        }
    
        private void FixupR_TargetSpecies1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (R_TargetSpecies item in e.NewItems)
                {
                    item.L_Species1 = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("R_TargetSpecies1", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (R_TargetSpecies item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_Species1, this))
                    {
                        item.L_Species1 = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("R_TargetSpecies1", item);
                    }
                }
            }
        }
    
        private void FixupSpeciesList(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (SpeciesList item in e.NewItems)
                {
                    item.L_Species = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("SpeciesList", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SpeciesList item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_Species, this))
                    {
                        item.L_Species = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("SpeciesList", item);
                    }
                }
            }
        }
    
        private void FixupR_StockSpeciesArea(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (R_StockSpeciesArea item in e.NewItems)
                {
                    item.L_Species = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("R_StockSpeciesArea", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (R_StockSpeciesArea item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_Species, this))
                    {
                        item.L_Species = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("R_StockSpeciesArea", item);
                    }
                }
            }
        }
    
        private void FixupR_SDReader(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (R_SDReader item in e.NewItems)
                {
                    item.L_Species = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("R_SDReader", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (R_SDReader item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_Species, this))
                    {
                        item.L_Species = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("R_SDReader", item);
                    }
                }
            }
        }
    
        private void FixupSDSample(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (SDSample item in e.NewItems)
                {
                    item.L_Species = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("SDSample", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SDSample item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_Species, this))
                    {
                        item.L_Species = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("SDSample", item);
                    }
                }
            }
        }

        #endregion

    }
}
