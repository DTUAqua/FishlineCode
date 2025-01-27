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
    [KnownType(typeof(L_Species))]
    [KnownType(typeof(Sample))]
    public partial class R_TargetSpecies: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int TargetSpeciesId
        {
            get { return _targetSpeciesId; }
            set
            {
                if (_targetSpeciesId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'TargetSpeciesId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _targetSpeciesId = value;
                    OnPropertyChanged("TargetSpeciesId");
                }
            }
        }
        private int _targetSpeciesId;
    
        [DataMember]
        public int sampleId
        {
            get { return _sampleId; }
            set
            {
                if (_sampleId != value)
                {
                    ChangeTracker.RecordOriginalValue("sampleId", _sampleId);
                    if (!IsDeserializing)
                    {
                        if (Sample != null && Sample.sampleId != value)
                        {
                            Sample = null;
                        }
                    }
                    _sampleId = value;
                    OnPropertyChanged("sampleId");
                }
            }
        }
        private int _sampleId;
    
        [DataMember]
        public string speciesCode
        {
            get { return _speciesCode; }
            set
            {
                if (_speciesCode != value)
                {
                    ChangeTracker.RecordOriginalValue("speciesCode", _speciesCode);
                    if (!IsDeserializing)
                    {
                        if (L_Species != null && L_Species.speciesCode != value)
                        {
                            L_Species = null;
                        }
                        if (L_Species1 != null && L_Species1.speciesCode != value)
                        {
                            L_Species1 = null;
                        }
                    }
                    _speciesCode = value;
                    OnPropertyChanged("speciesCode");
                }
            }
        }
        private string _speciesCode;

        #endregion

        #region Navigation Properties
    
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
        public L_Species L_Species1
        {
            get { return _l_Species1; }
            set
            {
                if (!ReferenceEquals(_l_Species1, value))
                {
                    var previousValue = _l_Species1;
                    _l_Species1 = value;
                    FixupL_Species1(previousValue);
                    OnNavigationPropertyChanged("L_Species1");
                }
            }
        }
        private L_Species _l_Species1;
    
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
            L_Species = null;
            L_Species1 = null;
            Sample = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupL_Species(L_Species previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_TargetSpecies.Contains(this))
            {
                previousValue.R_TargetSpecies.Remove(this);
            }
    
            if (L_Species != null)
            {
                if (!L_Species.R_TargetSpecies.Contains(this))
                {
                    L_Species.R_TargetSpecies.Add(this);
                }
    
                speciesCode = L_Species.speciesCode;
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
    
        private void FixupL_Species1(L_Species previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_TargetSpecies1.Contains(this))
            {
                previousValue.R_TargetSpecies1.Remove(this);
            }
    
            if (L_Species1 != null)
            {
                if (!L_Species1.R_TargetSpecies1.Contains(this))
                {
                    L_Species1.R_TargetSpecies1.Add(this);
                }
    
                speciesCode = L_Species1.speciesCode;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_Species1")
                    && (ChangeTracker.OriginalValues["L_Species1"] == L_Species1))
                {
                    ChangeTracker.OriginalValues.Remove("L_Species1");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_Species1", previousValue);
                }
                if (L_Species1 != null && !L_Species1.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_Species1.StartTracking();
                }
            }
        }
    
        private void FixupSample(Sample previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_TargetSpecies.Contains(this))
            {
                previousValue.R_TargetSpecies.Remove(this);
            }
    
            if (Sample != null)
            {
                if (!Sample.R_TargetSpecies.Contains(this))
                {
                    Sample.R_TargetSpecies.Add(this);
                }
    
                sampleId = Sample.sampleId;
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

        #endregion

    }
}
