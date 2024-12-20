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
    [KnownType(typeof(L_NavigationSystem))]
    [KnownType(typeof(L_Platform))]
    [KnownType(typeof(R_TripPlatformVersion))]
    public partial class L_PlatformVersion: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int platformVersionId
        {
            get { return _platformVersionId; }
            set
            {
                if (_platformVersionId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'platformVersionId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _platformVersionId = value;
                    OnPropertyChanged("platformVersionId");
                }
            }
        }
        private int _platformVersionId;
    
        [DataMember]
        public string platform
        {
            get { return _platform; }
            set
            {
                if (_platform != value)
                {
                    ChangeTracker.RecordOriginalValue("platform", _platform);
                    if (!IsDeserializing)
                    {
                        if (L_Platform != null && L_Platform.platform != value)
                        {
                            L_Platform = null;
                        }
                    }
                    _platform = value;
                    OnPropertyChanged("platform");
                }
            }
        }
        private string _platform;
    
        [DataMember]
        public int version
        {
            get { return _version; }
            set
            {
                if (_version != value)
                {
                    _version = value;
                    OnPropertyChanged("version");
                }
            }
        }
        private int _version;
    
        [DataMember]
        public Nullable<int> revisionYear
        {
            get { return _revisionYear; }
            set
            {
                if (_revisionYear != value)
                {
                    _revisionYear = value;
                    OnPropertyChanged("revisionYear");
                }
            }
        }
        private Nullable<int> _revisionYear;
    
        [DataMember]
        public string navigationSystem
        {
            get { return _navigationSystem; }
            set
            {
                if (_navigationSystem != value)
                {
                    ChangeTracker.RecordOriginalValue("navigationSystem", _navigationSystem);
                    if (!IsDeserializing)
                    {
                        if (L_NavigationSystem != null && L_NavigationSystem.navigationSystem != value)
                        {
                            L_NavigationSystem = null;
                        }
                    }
                    _navigationSystem = value;
                    OnPropertyChanged("navigationSystem");
                }
            }
        }
        private string _navigationSystem;
    
        [DataMember]
        public Nullable<int> registerTons
        {
            get { return _registerTons; }
            set
            {
                if (_registerTons != value)
                {
                    _registerTons = value;
                    OnPropertyChanged("registerTons");
                }
            }
        }
        private Nullable<int> _registerTons;
    
        [DataMember]
        public Nullable<decimal> length
        {
            get { return _length; }
            set
            {
                if (_length != value)
                {
                    _length = value;
                    OnPropertyChanged("length");
                }
            }
        }
        private Nullable<decimal> _length;
    
        [DataMember]
        public Nullable<int> power
        {
            get { return _power; }
            set
            {
                if (_power != value)
                {
                    _power = value;
                    OnPropertyChanged("power");
                }
            }
        }
        private Nullable<int> _power;
    
        [DataMember]
        public Nullable<int> crew
        {
            get { return _crew; }
            set
            {
                if (_crew != value)
                {
                    _crew = value;
                    OnPropertyChanged("crew");
                }
            }
        }
        private Nullable<int> _crew;
    
        [DataMember]
        public string radiosignal
        {
            get { return _radiosignal; }
            set
            {
                if (_radiosignal != value)
                {
                    _radiosignal = value;
                    OnPropertyChanged("radiosignal");
                }
            }
        }
        private string _radiosignal;
    
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

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public L_NavigationSystem L_NavigationSystem
        {
            get { return _l_NavigationSystem; }
            set
            {
                if (!ReferenceEquals(_l_NavigationSystem, value))
                {
                    var previousValue = _l_NavigationSystem;
                    _l_NavigationSystem = value;
                    FixupL_NavigationSystem(previousValue);
                    OnNavigationPropertyChanged("L_NavigationSystem");
                }
            }
        }
        private L_NavigationSystem _l_NavigationSystem;
    
        [DataMember]
        public L_Platform L_Platform
        {
            get { return _l_Platform; }
            set
            {
                if (!ReferenceEquals(_l_Platform, value))
                {
                    var previousValue = _l_Platform;
                    _l_Platform = value;
                    FixupL_Platform(previousValue);
                    OnNavigationPropertyChanged("L_Platform");
                }
            }
        }
        private L_Platform _l_Platform;
    
        [DataMember]
        public TrackableCollection<R_TripPlatformVersion> R_TripPlatformVersion
        {
            get
            {
                if (_r_TripPlatformVersion == null)
                {
                    _r_TripPlatformVersion = new TrackableCollection<R_TripPlatformVersion>();
                    _r_TripPlatformVersion.CollectionChanged += FixupR_TripPlatformVersion;
                }
                return _r_TripPlatformVersion;
            }
            set
            {
                if (!ReferenceEquals(_r_TripPlatformVersion, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_r_TripPlatformVersion != null)
                    {
                        _r_TripPlatformVersion.CollectionChanged -= FixupR_TripPlatformVersion;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (R_TripPlatformVersion item in _r_TripPlatformVersion)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _r_TripPlatformVersion = value;
                    if (_r_TripPlatformVersion != null)
                    {
                        _r_TripPlatformVersion.CollectionChanged += FixupR_TripPlatformVersion;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (R_TripPlatformVersion item in _r_TripPlatformVersion)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("R_TripPlatformVersion");
                }
            }
        }
        private TrackableCollection<R_TripPlatformVersion> _r_TripPlatformVersion;

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
            L_NavigationSystem = null;
            L_Platform = null;
            R_TripPlatformVersion.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupL_NavigationSystem(L_NavigationSystem previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.L_PlatformVersion.Contains(this))
            {
                previousValue.L_PlatformVersion.Remove(this);
            }
    
            if (L_NavigationSystem != null)
            {
                if (!L_NavigationSystem.L_PlatformVersion.Contains(this))
                {
                    L_NavigationSystem.L_PlatformVersion.Add(this);
                }
    
                navigationSystem = L_NavigationSystem.navigationSystem;
            }
            else if (!skipKeys)
            {
                navigationSystem = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_NavigationSystem")
                    && (ChangeTracker.OriginalValues["L_NavigationSystem"] == L_NavigationSystem))
                {
                    ChangeTracker.OriginalValues.Remove("L_NavigationSystem");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_NavigationSystem", previousValue);
                }
                if (L_NavigationSystem != null && !L_NavigationSystem.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_NavigationSystem.StartTracking();
                }
            }
        }
    
        private void FixupL_Platform(L_Platform previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.L_PlatformVersion.Contains(this))
            {
                previousValue.L_PlatformVersion.Remove(this);
            }
    
            if (L_Platform != null)
            {
                if (!L_Platform.L_PlatformVersion.Contains(this))
                {
                    L_Platform.L_PlatformVersion.Add(this);
                }
    
                platform = L_Platform.platform;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_Platform")
                    && (ChangeTracker.OriginalValues["L_Platform"] == L_Platform))
                {
                    ChangeTracker.OriginalValues.Remove("L_Platform");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_Platform", previousValue);
                }
                if (L_Platform != null && !L_Platform.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_Platform.StartTracking();
                }
            }
        }
    
        private void FixupR_TripPlatformVersion(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (R_TripPlatformVersion item in e.NewItems)
                {
                    item.L_PlatformVersion = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("R_TripPlatformVersion", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (R_TripPlatformVersion item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_PlatformVersion, this))
                    {
                        item.L_PlatformVersion = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("R_TripPlatformVersion", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Remove the previous dependent from the event listener.
                    ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                }
            }
        }

        #endregion

    }
}
