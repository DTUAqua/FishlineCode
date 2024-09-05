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
    [KnownType(typeof(Est_Method))]
    [KnownType(typeof(Est_Strata))]
    [KnownType(typeof(L_DFUBase_Category))]
    [KnownType(typeof(L_Species))]
    [KnownType(typeof(L_YesNo))]
    public partial class Est_MethodStep: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int Est_MethodStepId
        {
            get { return _est_MethodStepId; }
            set
            {
                if (_est_MethodStepId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'Est_MethodStepId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _est_MethodStepId = value;
                    OnPropertyChanged("Est_MethodStepId");
                }
            }
        }
        private int _est_MethodStepId;
    
        [DataMember]
        public int Est_MethodId
        {
            get { return _est_MethodId; }
            set
            {
                if (_est_MethodId != value)
                {
                    ChangeTracker.RecordOriginalValue("Est_MethodId", _est_MethodId);
                    if (!IsDeserializing)
                    {
                        if (Est_Method != null && Est_Method.Est_MethodId != value)
                        {
                            Est_Method = null;
                        }
                    }
                    _est_MethodId = value;
                    OnPropertyChanged("Est_MethodId");
                }
            }
        }
        private int _est_MethodId;
    
        [DataMember]
        public int stepNumber
        {
            get { return _stepNumber; }
            set
            {
                if (_stepNumber != value)
                {
                    _stepNumber = value;
                    OnPropertyChanged("stepNumber");
                }
            }
        }
        private int _stepNumber;
    
        [DataMember]
        public Nullable<int> Est_StrataId
        {
            get { return _est_StrataId; }
            set
            {
                if (_est_StrataId != value)
                {
                    ChangeTracker.RecordOriginalValue("Est_StrataId", _est_StrataId);
                    if (!IsDeserializing)
                    {
                        if (Est_Strata != null && Est_Strata.Est_StrataId != value)
                        {
                            Est_Strata = null;
                        }
                    }
                    _est_StrataId = value;
                    OnPropertyChanged("Est_StrataId");
                }
            }
        }
        private Nullable<int> _est_StrataId;
    
        [DataMember]
        public string overwrites
        {
            get { return _overwrites; }
            set
            {
                if (_overwrites != value)
                {
                    ChangeTracker.RecordOriginalValue("overwrites", _overwrites);
                    if (!IsDeserializing)
                    {
                        if (L_YesNo != null && L_YesNo.YesNo != value)
                        {
                            L_YesNo = null;
                        }
                    }
                    _overwrites = value;
                    OnPropertyChanged("overwrites");
                }
            }
        }
        private string _overwrites;
    
        [DataMember]
        public string SpeciesCode
        {
            get { return _speciesCode; }
            set
            {
                if (_speciesCode != value)
                {
                    ChangeTracker.RecordOriginalValue("SpeciesCode", _speciesCode);
                    if (!IsDeserializing)
                    {
                        if (L_Species != null && L_Species.speciesCode != value)
                        {
                            L_Species = null;
                        }
                    }
                    _speciesCode = value;
                    OnPropertyChanged("SpeciesCode");
                }
            }
        }
        private string _speciesCode;
    
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
        public Nullable<int> sampleId
        {
            get { return _sampleId; }
            set
            {
                if (_sampleId != value)
                {
                    _sampleId = value;
                    OnPropertyChanged("sampleId");
                }
            }
        }
        private Nullable<int> _sampleId;
    
        [DataMember]
        public Nullable<int> tripId
        {
            get { return _tripId; }
            set
            {
                if (_tripId != value)
                {
                    _tripId = value;
                    OnPropertyChanged("tripId");
                }
            }
        }
        private Nullable<int> _tripId;
    
        [DataMember]
        public int datahandlerId
        {
            get { return _datahandlerId; }
            set
            {
                if (_datahandlerId != value)
                {
                    _datahandlerId = value;
                    OnPropertyChanged("datahandlerId");
                }
            }
        }
        private int _datahandlerId;
    
        [DataMember]
        public string disabled
        {
            get { return _disabled; }
            set
            {
                if (_disabled != value)
                {
                    _disabled = value;
                    OnPropertyChanged("disabled");
                }
            }
        }
        private string _disabled;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public Est_Method Est_Method
        {
            get { return _est_Method; }
            set
            {
                if (!ReferenceEquals(_est_Method, value))
                {
                    var previousValue = _est_Method;
                    _est_Method = value;
                    FixupEst_Method(previousValue);
                    OnNavigationPropertyChanged("Est_Method");
                }
            }
        }
        private Est_Method _est_Method;
    
        [DataMember]
        public Est_Strata Est_Strata
        {
            get { return _est_Strata; }
            set
            {
                if (!ReferenceEquals(_est_Strata, value))
                {
                    var previousValue = _est_Strata;
                    _est_Strata = value;
                    FixupEst_Strata(previousValue);
                    OnNavigationPropertyChanged("Est_Strata");
                }
            }
        }
        private Est_Strata _est_Strata;
    
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
        public L_Species L_Species
        {
            get { return _l_Species; }
            set
            {
                if (!ReferenceEquals(_l_Species, value))
                {
                    var previousValue = _l_Species;
                    _l_Species = value;
                    FixupL_Species(previousValue);
                    OnNavigationPropertyChanged("L_Species");
                }
            }
        }
        private L_Species _l_Species;
    
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
            Est_Method = null;
            Est_Strata = null;
            L_DFUBase_Category = null;
            L_Species = null;
            L_YesNo = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupEst_Method(Est_Method previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_MethodStep.Contains(this))
            {
                previousValue.Est_MethodStep.Remove(this);
            }
    
            if (Est_Method != null)
            {
                if (!Est_Method.Est_MethodStep.Contains(this))
                {
                    Est_Method.Est_MethodStep.Add(this);
                }
    
                Est_MethodId = Est_Method.Est_MethodId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Est_Method")
                    && (ChangeTracker.OriginalValues["Est_Method"] == Est_Method))
                {
                    ChangeTracker.OriginalValues.Remove("Est_Method");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Est_Method", previousValue);
                }
                if (Est_Method != null && !Est_Method.ChangeTracker.ChangeTrackingEnabled)
                {
                    Est_Method.StartTracking();
                }
            }
        }
    
        private void FixupEst_Strata(Est_Strata previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_MethodStep.Contains(this))
            {
                previousValue.Est_MethodStep.Remove(this);
            }
    
            if (Est_Strata != null)
            {
                if (!Est_Strata.Est_MethodStep.Contains(this))
                {
                    Est_Strata.Est_MethodStep.Add(this);
                }
    
                Est_StrataId = Est_Strata.Est_StrataId;
            }
            else if (!skipKeys)
            {
                Est_StrataId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Est_Strata")
                    && (ChangeTracker.OriginalValues["Est_Strata"] == Est_Strata))
                {
                    ChangeTracker.OriginalValues.Remove("Est_Strata");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Est_Strata", previousValue);
                }
                if (Est_Strata != null && !Est_Strata.ChangeTracker.ChangeTrackingEnabled)
                {
                    Est_Strata.StartTracking();
                }
            }
        }
    
        private void FixupL_DFUBase_Category(L_DFUBase_Category previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_MethodStep.Contains(this))
            {
                previousValue.Est_MethodStep.Remove(this);
            }
    
            if (L_DFUBase_Category != null)
            {
                if (!L_DFUBase_Category.Est_MethodStep.Contains(this))
                {
                    L_DFUBase_Category.Est_MethodStep.Add(this);
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
    
        private void FixupL_Species(L_Species previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_MethodStep.Contains(this))
            {
                previousValue.Est_MethodStep.Remove(this);
            }
    
            if (L_Species != null)
            {
                if (!L_Species.Est_MethodStep.Contains(this))
                {
                    L_Species.Est_MethodStep.Add(this);
                }
    
                SpeciesCode = L_Species.speciesCode;
            }
            else if (!skipKeys)
            {
                SpeciesCode = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_Species")
                    && (ChangeTracker.OriginalValues["L_Species"] == L_Species))
                {
                    ChangeTracker.OriginalValues.Remove("L_Species");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_Species", previousValue);
                }
                if (L_Species != null && !L_Species.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_Species.StartTracking();
                }
            }
        }
    
        private void FixupL_YesNo(L_YesNo previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Est_MethodStep.Contains(this))
            {
                previousValue.Est_MethodStep.Remove(this);
            }
    
            if (L_YesNo != null)
            {
                if (!L_YesNo.Est_MethodStep.Contains(this))
                {
                    L_YesNo.Est_MethodStep.Add(this);
                }
    
                overwrites = L_YesNo.YesNo;
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

        #endregion

    }
}
