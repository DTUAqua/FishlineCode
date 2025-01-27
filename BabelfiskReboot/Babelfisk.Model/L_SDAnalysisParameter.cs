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
    [KnownType(typeof(SDAnnotation))]
    public partial class L_SDAnalysisParameter: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int L_sdAnalysisParameterId
        {
            get { return _l_sdAnalysisParameterId; }
            set
            {
                if (_l_sdAnalysisParameterId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'L_sdAnalysisParameterId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _l_sdAnalysisParameterId = value;
                    OnPropertyChanged("L_sdAnalysisParameterId");
                }
            }
        }
        private int _l_sdAnalysisParameterId;
    
        [DataMember]
        public string analysisParameter
        {
            get { return _analysisParameter; }
            set
            {
                if (_analysisParameter != value)
                {
                    _analysisParameter = value;
                    OnPropertyChanged("analysisParameter");
                }
            }
        }
        private string _analysisParameter;
    
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
        public TrackableCollection<SDAnnotation> SDAnnotation
        {
            get
            {
                if (_sDAnnotation == null)
                {
                    _sDAnnotation = new TrackableCollection<SDAnnotation>();
                    _sDAnnotation.CollectionChanged += FixupSDAnnotation;
                }
                return _sDAnnotation;
            }
            set
            {
                if (!ReferenceEquals(_sDAnnotation, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_sDAnnotation != null)
                    {
                        _sDAnnotation.CollectionChanged -= FixupSDAnnotation;
                    }
                    _sDAnnotation = value;
                    if (_sDAnnotation != null)
                    {
                        _sDAnnotation.CollectionChanged += FixupSDAnnotation;
                    }
                    OnNavigationPropertyChanged("SDAnnotation");
                }
            }
        }
        private TrackableCollection<SDAnnotation> _sDAnnotation;

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
            SDAnnotation.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupSDAnnotation(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (SDAnnotation item in e.NewItems)
                {
                    item.L_SDAnalysisParameter = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("SDAnnotation", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SDAnnotation item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_SDAnalysisParameter, this))
                    {
                        item.L_SDAnalysisParameter = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("SDAnnotation", item);
                    }
                }
            }
        }

        #endregion

    }
}
