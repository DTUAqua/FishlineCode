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

namespace Babelfisk.Warehouse.Model
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(Animal))]
    public partial class R_AnimalPictureReference: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int R_animalPictureReferenceId
        {
            get { return _r_animalPictureReferenceId; }
            set
            {
                if (_r_animalPictureReferenceId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'R_animalPictureReferenceId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _r_animalPictureReferenceId = value;
                    OnPropertyChanged("R_animalPictureReferenceId");
                }
            }
        }
        private int _r_animalPictureReferenceId;
    
        [DataMember]
        public int animalId
        {
            get { return _animalId; }
            set
            {
                if (_animalId != value)
                {
                    ChangeTracker.RecordOriginalValue("animalId", _animalId);
                    if (!IsDeserializing)
                    {
                        if (Animal != null && Animal.animalId != value)
                        {
                            Animal = null;
                        }
                    }
                    _animalId = value;
                    OnPropertyChanged("animalId");
                }
            }
        }
        private int _animalId;
    
        [DataMember]
        public string pictureReference
        {
            get { return _pictureReference; }
            set
            {
                if (_pictureReference != value)
                {
                    _pictureReference = value;
                    OnPropertyChanged("pictureReference");
                }
            }
        }
        private string _pictureReference;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public Animal Animal
        {
            get { return _animal; }
            set
            {
                if (!ReferenceEquals(_animal, value))
                {
                    var previousValue = _animal;
                    _animal = value;
                    FixupAnimal(previousValue);
                    OnNavigationPropertyChanged("Animal");
                }
            }
        }
        private Animal _animal;

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
            Animal = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupAnimal(Animal previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_AnimalPictureReference.Contains(this))
            {
                previousValue.R_AnimalPictureReference.Remove(this);
            }
    
            if (Animal != null)
            {
                if (!Animal.R_AnimalPictureReference.Contains(this))
                {
                    Animal.R_AnimalPictureReference.Add(this);
                }
    
                animalId = Animal.animalId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Animal")
                    && (ChangeTracker.OriginalValues["Animal"] == Animal))
                {
                    ChangeTracker.OriginalValues.Remove("Animal");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Animal", previousValue);
                }
                if (Animal != null && !Animal.ChangeTracker.ChangeTrackingEnabled)
                {
                    Animal.StartTracking();
                }
            }
        }

        #endregion

    }
}
