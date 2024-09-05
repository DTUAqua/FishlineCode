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
    public partial class AnimalFile: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int animalFileId
        {
            get { return _animalFileId; }
            set
            {
                if (_animalFileId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'animalFileId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _animalFileId = value;
                    OnPropertyChanged("animalFileId");
                }
            }
        }
        private int _animalFileId;
    
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
        public string filePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    OnPropertyChanged("filePath");
                }
            }
        }
        private string _filePath;
    
        [DataMember]
        public string fileType
        {
            get { return _fileType; }
            set
            {
                if (_fileType != value)
                {
                    _fileType = value;
                    OnPropertyChanged("fileType");
                }
            }
        }
        private string _fileType;
    
        [DataMember]
        public bool autoAdded
        {
            get { return _autoAdded; }
            set
            {
                if (_autoAdded != value)
                {
                    _autoAdded = value;
                    OnPropertyChanged("autoAdded");
                }
            }
        }
        private bool _autoAdded;

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
    
            if (previousValue != null && previousValue.AnimalFiles.Contains(this))
            {
                previousValue.AnimalFiles.Remove(this);
            }
    
            if (Animal != null)
            {
                if (!Animal.AnimalFiles.Contains(this))
                {
                    Animal.AnimalFiles.Add(this);
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
