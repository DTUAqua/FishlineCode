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
    [KnownType(typeof(Est_Strata))]
    [KnownType(typeof(SpeciesList))]
    public partial class L_DFUBase_Category: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int L_dfuBase_CategoryId
        {
            get { return _l_dfuBase_CategoryId; }
            set
            {
                if (_l_dfuBase_CategoryId != value)
                {
                    _l_dfuBase_CategoryId = value;
                    OnPropertyChanged("L_dfuBase_CategoryId");
                }
            }
        }
        private int _l_dfuBase_CategoryId;
    
        [DataMember]
        public string dfuBase_Category
        {
            get { return _dfuBase_Category; }
            set
            {
                if (_dfuBase_Category != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'dfuBase_Category' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _dfuBase_Category = value;
                    OnPropertyChanged("dfuBase_Category");
                }
            }
        }
        private string _dfuBase_Category;
    
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
        public TrackableCollection<Est_Strata> Est_Strata
        {
            get
            {
                if (_est_Strata == null)
                {
                    _est_Strata = new TrackableCollection<Est_Strata>();
                    _est_Strata.CollectionChanged += FixupEst_Strata;
                }
                return _est_Strata;
            }
            set
            {
                if (!ReferenceEquals(_est_Strata, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_est_Strata != null)
                    {
                        _est_Strata.CollectionChanged -= FixupEst_Strata;
                    }
                    _est_Strata = value;
                    if (_est_Strata != null)
                    {
                        _est_Strata.CollectionChanged += FixupEst_Strata;
                    }
                    OnNavigationPropertyChanged("Est_Strata");
                }
            }
        }
        private TrackableCollection<Est_Strata> _est_Strata;
    
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
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (SpeciesList item in _speciesList)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _speciesList = value;
                    if (_speciesList != null)
                    {
                        _speciesList.CollectionChanged += FixupSpeciesList;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (SpeciesList item in _speciesList)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("SpeciesList");
                }
            }
        }
        private TrackableCollection<SpeciesList> _speciesList;

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
            Est_MethodStep.Clear();
            Est_Strata.Clear();
            SpeciesList.Clear();
        }

        #endregion

        #region Association Fixup
    
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
                    item.L_DFUBase_Category = this;
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
                    if (ReferenceEquals(item.L_DFUBase_Category, this))
                    {
                        item.L_DFUBase_Category = null;
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
    
        private void FixupEst_Strata(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Est_Strata item in e.NewItems)
                {
                    item.L_DFUBase_Category = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Est_Strata", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Est_Strata item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_DFUBase_Category, this))
                    {
                        item.L_DFUBase_Category = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Est_Strata", item);
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
                    item.L_DFUBase_Category = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("SpeciesList", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SpeciesList item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_DFUBase_Category, this))
                    {
                        item.L_DFUBase_Category = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("SpeciesList", item);
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
