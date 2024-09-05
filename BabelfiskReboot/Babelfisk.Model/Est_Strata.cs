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
    [KnownType(typeof(Cruise))]
    [KnownType(typeof(DFUPerson))]
    [KnownType(typeof(Est_MethodStep))]
    [KnownType(typeof(L_DFUBase_Category))]
    [KnownType(typeof(L_YesNo))]
    [KnownType(typeof(Sample))]
    [KnownType(typeof(Trip))]
    public partial class Est_Strata: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int Est_StrataId
        {
            get { return _est_StrataId; }
            set
            {
                if (_est_StrataId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'Est_StrataId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _est_StrataId = value;
                    OnPropertyChanged("Est_StrataId");
                }
            }
        }
        private int _est_StrataId;
    
        [DataMember]
        public string name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("name");
                }
            }
        }
        private string _name;
    
        [DataMember]
        public int cruiseId
        {
            get { return _cruiseId; }
            set
            {
                if (_cruiseId != value)
                {
                    ChangeTracker.RecordOriginalValue("cruiseId", _cruiseId);
                    if (!IsDeserializing)
                    {
                        if (Cruise != null && Cruise.cruiseId != value)
                        {
                            Cruise = null;
                        }
                    }
                    _cruiseId = value;
                    OnPropertyChanged("cruiseId");
                }
            }
        }
        private int _cruiseId;
    
        [DataMember]
        public Nullable<int> TripId
        {
            get { return _tripId; }
            set
            {
                if (_tripId != value)
                {
                    ChangeTracker.RecordOriginalValue("TripId", _tripId);
                    if (!IsDeserializing)
                    {
                        if (Trip != null && Trip.tripId != value)
                        {
                            Trip = null;
                        }
                    }
                    _tripId = value;
                    OnPropertyChanged("TripId");
                }
            }
        }
        private Nullable<int> _tripId;
    
        [DataMember]
        public Nullable<int> SampleId
        {
            get { return _sampleId; }
            set
            {
                if (_sampleId != value)
                {
                    ChangeTracker.RecordOriginalValue("SampleId", _sampleId);
                    if (!IsDeserializing)
                    {
                        if (Sample != null && Sample.sampleId != value)
                        {
                            Sample = null;
                        }
                    }
                    _sampleId = value;
                    OnPropertyChanged("SampleId");
                }
            }
        }
        private Nullable<int> _sampleId;
    
        [DataMember]
        public string dfubase_category
        {
            get { return _dfubase_category; }
            set
            {
                if (_dfubase_category != value)
                {
                    ChangeTracker.RecordOriginalValue("dfubase_category", _dfubase_category);
                    if (!IsDeserializing)
                    {
                        if (L_DFUBase_Category != null && L_DFUBase_Category.dfuBase_Category != value)
                        {
                            L_DFUBase_Category = null;
                        }
                    }
                    _dfubase_category = value;
                    OnPropertyChanged("dfubase_category");
                }
            }
        }
        private string _dfubase_category;
    
        [DataMember]
        public Nullable<int> sizeSortingEU
        {
            get { return _sizeSortingEU; }
            set
            {
                if (_sizeSortingEU != value)
                {
                    _sizeSortingEU = value;
                    OnPropertyChanged("sizeSortingEU");
                }
            }
        }
        private Nullable<int> _sizeSortingEU;
    
        [DataMember]
        public string representative
        {
            get { return _representative; }
            set
            {
                if (_representative != value)
                {
                    ChangeTracker.RecordOriginalValue("representative", _representative);
                    if (!IsDeserializing)
                    {
                        if (L_YesNo != null && L_YesNo.YesNo != value)
                        {
                            L_YesNo = null;
                        }
                    }
                    _representative = value;
                    OnPropertyChanged("representative");
                }
            }
        }
        private string _representative;
    
        [DataMember]
        public string statisticalRectangle
        {
            get { return _statisticalRectangle; }
            set
            {
                if (_statisticalRectangle != value)
                {
                    _statisticalRectangle = value;
                    OnPropertyChanged("statisticalRectangle");
                }
            }
        }
        private string _statisticalRectangle;
    
        [DataMember]
        public string disabled
        {
            get { return _disabled; }
            set
            {
                if (_disabled != value)
                {
                    ChangeTracker.RecordOriginalValue("disabled", _disabled);
                    if (!IsDeserializing)
                    {
                        if (L_YesNo1 != null && L_YesNo1.YesNo != value)
                        {
                            L_YesNo1 = null;
                        }
                    }
                    _disabled = value;
                    OnPropertyChanged("disabled");
                }
            }
        }
        private string _disabled;
    
        [DataMember]
        public int datahandlerId
        {
            get { return _datahandlerId; }
            set
            {
                if (_datahandlerId != value)
                {
                    ChangeTracker.RecordOriginalValue("datahandlerId", _datahandlerId);
                    if (!IsDeserializing)
                    {
                        if (DFUPerson != null && DFUPerson.dfuPersonId != value)
                        {
                            DFUPerson = null;
                        }
                    }
                    _datahandlerId = value;
                    OnPropertyChanged("datahandlerId");
                }
            }
        }
        private int _datahandlerId;
    
        [DataMember]
        public int delete_CruiseId
        {
            get { return _delete_CruiseId; }
            set
            {
                if (_delete_CruiseId != value)
                {
                    ChangeTracker.RecordOriginalValue("delete_CruiseId", _delete_CruiseId);
                    if (!IsDeserializing)
                    {
                        if (Cruise1 != null && Cruise1.cruiseId != value)
                        {
                            Cruise1 = null;
                        }
                    }
                    _delete_CruiseId = value;
                    OnPropertyChanged("delete_CruiseId");
                }
            }
        }
        private int _delete_CruiseId;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public Cruise Cruise
        {
            get { return _cruise; }
            set
            {
                if (!ReferenceEquals(_cruise, value))
                {
                    var previousValue = _cruise;
                    _cruise = value;
                    FixupCruise(previousValue);
                    OnNavigationPropertyChanged("Cruise");
                }
            }
        }
        private Cruise _cruise;
    
        [DataMember]
        public Cruise Cruise1
        {
            get { return _cruise1; }
            set
            {
                if (!ReferenceEquals(_cruise1, value))
                {
                    var previousValue = _cruise1;
                    _cruise1 = value;
                    FixupCruise1(previousValue);
                    OnNavigationPropertyChanged("Cruise1");
                }
            }
        }
        private Cruise _cruise1;
    
        [DataMember]
        public DFUPerson DFUPerson
        {
            get { return _dFUPerson; }
            set
            {
                if (!ReferenceEquals(_dFUPerson, value))
                {
                    var previousValue = _dFUPerson;
                    _dFUPerson = value;
                    FixupDFUPerson(previousValue);
                    OnNavigationPropertyChanged("DFUPerson");
                }
            }
        }
        private DFUPerson _dFUPerson;
    
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
                    }
                    _est_MethodStep = value;
                    if (_est_MethodStep != null)
                    {
                        _est_MethodStep.CollectionChanged += FixupEst_MethodStep;
                    }
                    OnNavigationPropertyChanged("Est_MethodStep");
                }
            }
        }
        private TrackableCollection<Est_MethodStep> _est_MethodStep;
    
        [DataMember]
        public L_DFUBase_Category L_DFUBase_Category
        {
            get { return _l_DFUBase_Category; }
            set
            {
                if (!ReferenceEquals(_l_DFUBase_Category, value))
                {
                    var previousValue = _l_DFUBase_Category;
                    _l_DFUBase_Category = value;
                    FixupL_DFUBase_Category(previousValue);
                    OnNavigationPropertyChanged("L_DFUBase_Category");
                }
            }
        }
        private L_DFUBase_Category _l_DFUBase_Category;
    
        [DataMember]
        public L_YesNo L_YesNo
        {
            get { return _l_YesNo; }
            set
            {
                if (!ReferenceEquals(_l_YesNo, value))
                {
                    var previousValue = _l_YesNo;
                    _l_YesNo = value;
                    FixupL_YesNo(previousValue);
                    OnNavigationPropertyChanged("L_YesNo");
                }
            }
        }
        private L_YesNo _l_YesNo;
    
        [DataMember]
        public L_YesNo L_YesNo1
        {
            get { return _l_YesNo1; }
            set
            {
                if (!ReferenceEquals(_l_YesNo1, value))
                {
                    var previousValue = _l_YesNo1;
                    _l_YesNo1 = value;
                    FixupL_YesNo1(previousValue);
                    OnNavigationPropertyChanged("L_YesNo1");
                }
            }
        }
        private L_YesNo _l_YesNo1;
    
        [DataMember]
        public Sample Sample
        {
            get { return _sample; }
            set
            {
                if (!ReferenceEquals(_sample, value))
                {
                    var previousValue = _sample;
                    _sample = value;
                    FixupSample(previousValue);
                    OnNavigationPropertyChanged("Sample");
                }
            }
        }
        private Sample _sample;
    
        [DataMember]
        public Trip Trip
        {
            get { return _trip; }
            set
            {
                if (!ReferenceEquals(_trip, value))
                {
                    var previousValue = _trip;
                    _trip = value;
                    FixupTrip(previousValue);
                    OnNavigationPropertyChanged("Trip");
                }
            }
        }
        private Trip _trip;

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
            Cruise = null;
            Cruise1 = null;
            DFUPerson = null;
            Est_MethodStep.Clear();
            L_DFUBase_Category = null;
            L_YesNo = null;
            L_YesNo1 = null;
            Sample = null;
            Trip = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupCruise(Cruise previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_Strata.Contains(this))
            {
                previousValue.Est_Strata.Remove(this);
            }
    
            if (Cruise != null)
            {
                if (!Cruise.Est_Strata.Contains(this))
                {
                    Cruise.Est_Strata.Add(this);
                }
    
                cruiseId = Cruise.cruiseId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Cruise")
                    && (ChangeTracker.OriginalValues["Cruise"] == Cruise))
                {
                    ChangeTracker.OriginalValues.Remove("Cruise");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Cruise", previousValue);
                }
                if (Cruise != null && !Cruise.ChangeTracker.ChangeTrackingEnabled)
                {
                    Cruise.StartTracking();
                }
            }
        }
    
        private void FixupCruise1(Cruise previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_Strata1.Contains(this))
            {
                previousValue.Est_Strata1.Remove(this);
            }
    
            if (Cruise1 != null)
            {
                if (!Cruise1.Est_Strata1.Contains(this))
                {
                    Cruise1.Est_Strata1.Add(this);
                }
    
                delete_CruiseId = Cruise1.cruiseId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Cruise1")
                    && (ChangeTracker.OriginalValues["Cruise1"] == Cruise1))
                {
                    ChangeTracker.OriginalValues.Remove("Cruise1");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Cruise1", previousValue);
                }
                if (Cruise1 != null && !Cruise1.ChangeTracker.ChangeTrackingEnabled)
                {
                    Cruise1.StartTracking();
                }
            }
        }
    
        private void FixupDFUPerson(DFUPerson previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_Strata.Contains(this))
            {
                previousValue.Est_Strata.Remove(this);
            }
    
            if (DFUPerson != null)
            {
                if (!DFUPerson.Est_Strata.Contains(this))
                {
                    DFUPerson.Est_Strata.Add(this);
                }
    
                datahandlerId = DFUPerson.dfuPersonId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("DFUPerson")
                    && (ChangeTracker.OriginalValues["DFUPerson"] == DFUPerson))
                {
                    ChangeTracker.OriginalValues.Remove("DFUPerson");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("DFUPerson", previousValue);
                }
                if (DFUPerson != null && !DFUPerson.ChangeTracker.ChangeTrackingEnabled)
                {
                    DFUPerson.StartTracking();
                }
            }
        }
    
        private void FixupL_DFUBase_Category(L_DFUBase_Category previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_Strata.Contains(this))
            {
                previousValue.Est_Strata.Remove(this);
            }
    
            if (L_DFUBase_Category != null)
            {
                if (!L_DFUBase_Category.Est_Strata.Contains(this))
                {
                    L_DFUBase_Category.Est_Strata.Add(this);
                }
    
                dfubase_category = L_DFUBase_Category.dfuBase_Category;
            }
            else if (!skipKeys)
            {
                dfubase_category = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_DFUBase_Category")
                    && (ChangeTracker.OriginalValues["L_DFUBase_Category"] == L_DFUBase_Category))
                {
                    ChangeTracker.OriginalValues.Remove("L_DFUBase_Category");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_DFUBase_Category", previousValue);
                }
                if (L_DFUBase_Category != null && !L_DFUBase_Category.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_DFUBase_Category.StartTracking();
                }
            }
        }
    
        private void FixupL_YesNo(L_YesNo previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_Strata.Contains(this))
            {
                previousValue.Est_Strata.Remove(this);
            }
    
            if (L_YesNo != null)
            {
                if (!L_YesNo.Est_Strata.Contains(this))
                {
                    L_YesNo.Est_Strata.Add(this);
                }
    
                representative = L_YesNo.YesNo;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_YesNo")
                    && (ChangeTracker.OriginalValues["L_YesNo"] == L_YesNo))
                {
                    ChangeTracker.OriginalValues.Remove("L_YesNo");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_YesNo", previousValue);
                }
                if (L_YesNo != null && !L_YesNo.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_YesNo.StartTracking();
                }
            }
        }
    
        private void FixupL_YesNo1(L_YesNo previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_Strata1.Contains(this))
            {
                previousValue.Est_Strata1.Remove(this);
            }
    
            if (L_YesNo1 != null)
            {
                if (!L_YesNo1.Est_Strata1.Contains(this))
                {
                    L_YesNo1.Est_Strata1.Add(this);
                }
    
                disabled = L_YesNo1.YesNo;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_YesNo1")
                    && (ChangeTracker.OriginalValues["L_YesNo1"] == L_YesNo1))
                {
                    ChangeTracker.OriginalValues.Remove("L_YesNo1");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_YesNo1", previousValue);
                }
                if (L_YesNo1 != null && !L_YesNo1.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_YesNo1.StartTracking();
                }
            }
        }
    
        private void FixupSample(Sample previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_Strata.Contains(this))
            {
                previousValue.Est_Strata.Remove(this);
            }
    
            if (Sample != null)
            {
                if (!Sample.Est_Strata.Contains(this))
                {
                    Sample.Est_Strata.Add(this);
                }
    
                SampleId = Sample.sampleId;
            }
            else if (!skipKeys)
            {
                SampleId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Sample")
                    && (ChangeTracker.OriginalValues["Sample"] == Sample))
                {
                    ChangeTracker.OriginalValues.Remove("Sample");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Sample", previousValue);
                }
                if (Sample != null && !Sample.ChangeTracker.ChangeTrackingEnabled)
                {
                    Sample.StartTracking();
                }
            }
        }
    
        private void FixupTrip(Trip previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_Strata.Contains(this))
            {
                previousValue.Est_Strata.Remove(this);
            }
    
            if (Trip != null)
            {
                if (!Trip.Est_Strata.Contains(this))
                {
                    Trip.Est_Strata.Add(this);
                }
    
                TripId = Trip.tripId;
            }
            else if (!skipKeys)
            {
                TripId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Trip")
                    && (ChangeTracker.OriginalValues["Trip"] == Trip))
                {
                    ChangeTracker.OriginalValues.Remove("Trip");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Trip", previousValue);
                }
                if (Trip != null && !Trip.ChangeTracker.ChangeTrackingEnabled)
                {
                    Trip.StartTracking();
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
                    item.Est_Strata = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Est_MethodStep", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Est_MethodStep item in e.OldItems)
                {
                    if (ReferenceEquals(item.Est_Strata, this))
                    {
                        item.Est_Strata = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Est_MethodStep", item);
                    }
                }
            }
        }

        #endregion

    }
}
