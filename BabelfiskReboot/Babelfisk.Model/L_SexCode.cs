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
    [KnownType(typeof(Animal))]
    [KnownType(typeof(SpeciesList))]
    public partial class L_SexCode: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int L_sexCodeId
        {
            get { return _l_sexCodeId; }
            set
            {
                if (_l_sexCodeId != value)
                {
                    _l_sexCodeId = value;
                    OnPropertyChanged("L_sexCodeId");
                }
            }
        }
        private int _l_sexCodeId;
    
        [DataMember]
        public string sexCode
        {
            get { return _sexCode; }
            set
            {
                if (_sexCode != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'sexCode' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _sexCode = value;
                    OnPropertyChanged("sexCode");
                }
            }
        }
        private string _sexCode;
    
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
        public TrackableCollection<Animal> Animal
        {
            get
            {
                if (_animal == null)
                {
                    _animal = new TrackableCollection<Animal>();
                    _animal.CollectionChanged += FixupAnimal;
                }
                return _animal;
            }
            set
            {
                if (!ReferenceEquals(_animal, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_animal != null)
                    {
                        _animal.CollectionChanged -= FixupAnimal;
                    }
                    _animal = value;
                    if (_animal != null)
                    {
                        _animal.CollectionChanged += FixupAnimal;
                    }
                    OnNavigationPropertyChanged("Animal");
                }
            }
        }
        private TrackableCollection<Animal> _animal;
    
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
                    }
                    _speciesList = value;
                    if (_speciesList != null)
                    {
                        _speciesList.CollectionChanged += FixupSpeciesList;
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
            Animal.Clear();
            SpeciesList.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupAnimal(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Animal item in e.NewItems)
                {
                    item.L_SexCode = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Animal", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Animal item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_SexCode, this))
                    {
                        item.L_SexCode = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Animal", item);
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
                    item.L_SexCode = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("SpeciesList", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SpeciesList item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_SexCode, this))
                    {
                        item.L_SexCode = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("SpeciesList", item);
                    }
                }
            }
        }

        #endregion

    }
}
