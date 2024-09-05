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
    [KnownType(typeof(Sample))]
    public partial class L_SpeciesRegistration: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int speciesRegistrationId
        {
            get { return _speciesRegistrationId; }
            set
            {
                if (_speciesRegistrationId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'speciesRegistrationId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _speciesRegistrationId = value;
                    OnPropertyChanged("speciesRegistrationId");
                }
            }
        }
        private int _speciesRegistrationId;
    
        [DataMember]
        public string speciesRegistration
        {
            get { return _speciesRegistration; }
            set
            {
                if (_speciesRegistration != value)
                {
                    _speciesRegistration = value;
                    OnPropertyChanged("speciesRegistration");
                }
            }
        }
        private string _speciesRegistration;
    
        [DataMember]
        public string description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged("description");
                }
            }
        }
        private string _description;
    
        [DataMember]
        public Nullable<int> num
        {
            get { return _num; }
            set
            {
                if (_num != value)
                {
                    _num = value;
                    OnPropertyChanged("num");
                }
            }
        }
        private Nullable<int> _num;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<Sample> Sample
        {
            get
            {
                if (_sample == null)
                {
                    _sample = new TrackableCollection<Sample>();
                    _sample.CollectionChanged += FixupSample;
                }
                return _sample;
            }
            set
            {
                if (!ReferenceEquals(_sample, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_sample != null)
                    {
                        _sample.CollectionChanged -= FixupSample;
                    }
                    _sample = value;
                    if (_sample != null)
                    {
                        _sample.CollectionChanged += FixupSample;
                    }
                    OnNavigationPropertyChanged("Sample");
                }
            }
        }
        private TrackableCollection<Sample> _sample;

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
    
        protected virtual void ClearNavigationProperties()
        {
            Sample.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupSample(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Sample item in e.NewItems)
                {
                    item.L_SpeciesRegistration = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Sample", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Sample item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_SpeciesRegistration, this))
                    {
                        item.L_SpeciesRegistration = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Sample", item);
                    }
                }
            }
        }

        #endregion

    }
}
