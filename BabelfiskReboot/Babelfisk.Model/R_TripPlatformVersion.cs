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
    [KnownType(typeof(L_PlatformVersion))]
    [KnownType(typeof(Trip))]
    public partial class R_TripPlatformVersion: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int R_TripPlatformVersionId
        {
            get { return _r_TripPlatformVersionId; }
            set
            {
                if (_r_TripPlatformVersionId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'R_TripPlatformVersionId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _r_TripPlatformVersionId = value;
                    OnPropertyChanged("R_TripPlatformVersionId");
                }
            }
        }
        private int _r_TripPlatformVersionId;
    
        [DataMember]
        public int tripId
        {
            get { return _tripId; }
            set
            {
                if (_tripId != value)
                {
                    ChangeTracker.RecordOriginalValue("tripId", _tripId);
                    if (!IsDeserializing)
                    {
                        if (Trip != null && Trip.tripId != value)
                        {
                            Trip = null;
                        }
                    }
                    _tripId = value;
                    OnPropertyChanged("tripId");
                }
            }
        }
        private int _tripId;
    
        [DataMember]
        public int platformVersionId
        {
            get { return _platformVersionId; }
            set
            {
                if (_platformVersionId != value)
                {
                    ChangeTracker.RecordOriginalValue("platformVersionId", _platformVersionId);
                    if (!IsDeserializing)
                    {
                        if (L_PlatformVersion != null && L_PlatformVersion.platformVersionId != value)
                        {
                            L_PlatformVersion = null;
                        }
                    }
                    _platformVersionId = value;
                    OnPropertyChanged("platformVersionId");
                }
            }
        }
        private int _platformVersionId;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public L_PlatformVersion L_PlatformVersion
        {
            get { return _l_PlatformVersion; }
            set
            {
                if (!ReferenceEquals(_l_PlatformVersion, value))
                {
                    var previousValue = _l_PlatformVersion;
                    _l_PlatformVersion = value;
                    FixupL_PlatformVersion(previousValue);
                    OnNavigationPropertyChanged("L_PlatformVersion");
                }
            }
        }
        private L_PlatformVersion _l_PlatformVersion;
    
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
            L_PlatformVersion = null;
            Trip = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupL_PlatformVersion(L_PlatformVersion previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_TripPlatformVersion.Contains(this))
            {
                previousValue.R_TripPlatformVersion.Remove(this);
            }
    
            if (L_PlatformVersion != null)
            {
                if (!L_PlatformVersion.R_TripPlatformVersion.Contains(this))
                {
                    L_PlatformVersion.R_TripPlatformVersion.Add(this);
                }
    
                platformVersionId = L_PlatformVersion.platformVersionId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_PlatformVersion")
                    && (ChangeTracker.OriginalValues["L_PlatformVersion"] == L_PlatformVersion))
                {
                    ChangeTracker.OriginalValues.Remove("L_PlatformVersion");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_PlatformVersion", previousValue);
                }
                if (L_PlatformVersion != null && !L_PlatformVersion.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_PlatformVersion.StartTracking();
                }
            }
        }
    
        private void FixupTrip(Trip previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_TripPlatformVersion.Contains(this))
            {
                previousValue.R_TripPlatformVersion.Remove(this);
            }
    
            if (Trip != null)
            {
                if (!Trip.R_TripPlatformVersion.Contains(this))
                {
                    Trip.R_TripPlatformVersion.Add(this);
                }
    
                tripId = Trip.tripId;
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

        #endregion

    }
}
