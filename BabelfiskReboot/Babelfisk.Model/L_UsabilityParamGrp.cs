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
    [KnownType(typeof(R_UsabilityParamUsabilityGrp))]
    public partial class L_UsabilityParamGrp: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int usabilityParamGrpId
        {
            get { return _usabilityParamGrpId; }
            set
            {
                if (_usabilityParamGrpId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'usabilityParamGrpId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _usabilityParamGrpId = value;
                    OnPropertyChanged("usabilityParamGrpId");
                }
            }
        }
        private int _usabilityParamGrpId;
    
        [DataMember]
        public string usabilityGrp
        {
            get { return _usabilityGrp; }
            set
            {
                if (_usabilityGrp != value)
                {
                    _usabilityGrp = value;
                    OnPropertyChanged("usabilityGrp");
                }
            }
        }
        private string _usabilityGrp;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<R_UsabilityParamUsabilityGrp> R_UsabilityParamUsabilityGrp
        {
            get
            {
                if (_r_UsabilityParamUsabilityGrp == null)
                {
                    _r_UsabilityParamUsabilityGrp = new TrackableCollection<R_UsabilityParamUsabilityGrp>();
                    _r_UsabilityParamUsabilityGrp.CollectionChanged += FixupR_UsabilityParamUsabilityGrp;
                }
                return _r_UsabilityParamUsabilityGrp;
            }
            set
            {
                if (!ReferenceEquals(_r_UsabilityParamUsabilityGrp, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_r_UsabilityParamUsabilityGrp != null)
                    {
                        _r_UsabilityParamUsabilityGrp.CollectionChanged -= FixupR_UsabilityParamUsabilityGrp;
                    }
                    _r_UsabilityParamUsabilityGrp = value;
                    if (_r_UsabilityParamUsabilityGrp != null)
                    {
                        _r_UsabilityParamUsabilityGrp.CollectionChanged += FixupR_UsabilityParamUsabilityGrp;
                    }
                    OnNavigationPropertyChanged("R_UsabilityParamUsabilityGrp");
                }
            }
        }
        private TrackableCollection<R_UsabilityParamUsabilityGrp> _r_UsabilityParamUsabilityGrp;

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
            R_UsabilityParamUsabilityGrp.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupR_UsabilityParamUsabilityGrp(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (R_UsabilityParamUsabilityGrp item in e.NewItems)
                {
                    item.L_UsabilityParamGrp = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("R_UsabilityParamUsabilityGrp", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (R_UsabilityParamUsabilityGrp item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_UsabilityParamGrp, this))
                    {
                        item.L_UsabilityParamGrp = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("R_UsabilityParamUsabilityGrp", item);
                    }
                }
            }
        }

        #endregion

    }
}
