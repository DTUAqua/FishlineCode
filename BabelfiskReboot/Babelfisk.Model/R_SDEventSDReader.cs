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
    [KnownType(typeof(R_SDReader))]
    [KnownType(typeof(SDEvent))]
    public partial class R_SDEventSDReader: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int sdEventId
        {
            get { return _sdEventId; }
            set
            {
                if (_sdEventId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'sdEventId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (SDEvent != null && SDEvent.sdEventId != value)
                        {
                            SDEvent = null;
                        }
                    }
                    _sdEventId = value;
                    OnPropertyChanged("sdEventId");
                }
            }
        }
        private int _sdEventId;
    
        [DataMember]
        public int sdReaderId
        {
            get { return _sdReaderId; }
            set
            {
                if (_sdReaderId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'sdReaderId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (SDReader != null && SDReader.r_SDReaderId != value)
                        {
                            SDReader = null;
                        }
                    }
                    _sdReaderId = value;
                    OnPropertyChanged("sdReaderId");
                }
            }
        }
        private int _sdReaderId;
    
        [DataMember]
        public bool primaryReader
        {
            get { return _primaryReader; }
            set
            {
                if (_primaryReader != value)
                {
                    _primaryReader = value;
                    OnPropertyChanged("primaryReader");
                }
            }
        }
        private bool _primaryReader;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public R_SDReader SDReader
        {
            get { return _sDReader; }
            set
            {
                if (!ReferenceEquals(_sDReader, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (sdReaderId != value.r_SDReaderId)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _sDReader;
                    _sDReader = value;
                    FixupSDReader(previousValue);
                    OnNavigationPropertyChanged("SDReader");
                }
            }
        }
        private R_SDReader _sDReader;
    
        [DataMember]
        public SDEvent SDEvent
        {
            get { return _sDEvent; }
            set
            {
                if (!ReferenceEquals(_sDEvent, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (sdEventId != value.sdEventId)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _sDEvent;
                    _sDEvent = value;
                    FixupSDEvent(previousValue);
                    OnNavigationPropertyChanged("SDEvent");
                }
            }
        }
        private SDEvent _sDEvent;

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
            SDReader = null;
            SDEvent = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupSDReader(R_SDReader previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_SDEventSDReader.Contains(this))
            {
                previousValue.R_SDEventSDReader.Remove(this);
            }
    
            if (SDReader != null)
            {
                if (!SDReader.R_SDEventSDReader.Contains(this))
                {
                    SDReader.R_SDEventSDReader.Add(this);
                }
    
                sdReaderId = SDReader.r_SDReaderId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("SDReader")
                    && (ChangeTracker.OriginalValues["SDReader"] == SDReader))
                {
                    ChangeTracker.OriginalValues.Remove("SDReader");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("SDReader", previousValue);
                }
                if (SDReader != null && !SDReader.ChangeTracker.ChangeTrackingEnabled)
                {
                    SDReader.StartTracking();
                }
            }
        }
    
        private void FixupSDEvent(SDEvent previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.SDReaders.Contains(this))
            {
                previousValue.SDReaders.Remove(this);
            }
    
            if (SDEvent != null)
            {
                if (!SDEvent.SDReaders.Contains(this))
                {
                    SDEvent.SDReaders.Add(this);
                }
    
                sdEventId = SDEvent.sdEventId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("SDEvent")
                    && (ChangeTracker.OriginalValues["SDEvent"] == SDEvent))
                {
                    ChangeTracker.OriginalValues.Remove("SDEvent");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("SDEvent", previousValue);
                }
                if (SDEvent != null && !SDEvent.ChangeTracker.ChangeTrackingEnabled)
                {
                    SDEvent.StartTracking();
                }
            }
        }

        #endregion

    }
}
