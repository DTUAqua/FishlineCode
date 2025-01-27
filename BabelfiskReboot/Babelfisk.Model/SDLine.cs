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
    [KnownType(typeof(DFUPerson))]
    public partial class SDLine: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int sdLineId
        {
            get { return _sdLineId; }
            set
            {
                if (_sdLineId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'sdLineId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _sdLineId = value;
                    OnPropertyChanged("sdLineId");
                }
            }
        }
        private int _sdLineId;
    
        [DataMember]
        public System.Guid sdLineGuid
        {
            get { return _sdLineGuid; }
            set
            {
                if (_sdLineGuid != value)
                {
                    _sdLineGuid = value;
                    OnPropertyChanged("sdLineGuid");
                }
            }
        }
        private System.Guid _sdLineGuid;
    
        [DataMember]
        public int sdAnnotationId
        {
            get { return _sdAnnotationId; }
            set
            {
                if (_sdAnnotationId != value)
                {
                    ChangeTracker.RecordOriginalValue("sdAnnotationId", _sdAnnotationId);
                    if (!IsDeserializing)
                    {
                        if (SDAnnotation != null && SDAnnotation.sdAnnotationId != value)
                        {
                            SDAnnotation = null;
                        }
                    }
                    _sdAnnotationId = value;
                    OnPropertyChanged("sdAnnotationId");
                }
            }
        }
        private int _sdAnnotationId;
    
        [DataMember]
        public Nullable<int> createdById
        {
            get { return _createdById; }
            set
            {
                if (_createdById != value)
                {
                    ChangeTracker.RecordOriginalValue("createdById", _createdById);
                    if (!IsDeserializing)
                    {
                        if (DFUPerson != null && DFUPerson.dfuPersonId != value)
                        {
                            DFUPerson = null;
                        }
                    }
                    _createdById = value;
                    OnPropertyChanged("createdById");
                }
            }
        }
        private Nullable<int> _createdById;
    
        [DataMember]
        public string createdByUserName
        {
            get { return _createdByUserName; }
            set
            {
                if (_createdByUserName != value)
                {
                    _createdByUserName = value;
                    OnPropertyChanged("createdByUserName");
                }
            }
        }
        private string _createdByUserName;
    
        [DataMember]
        public Nullable<System.DateTime> createdTime
        {
            get { return _createdTime; }
            set
            {
                if (_createdTime != value)
                {
                    _createdTime = value;
                    OnPropertyChanged("createdTime");
                }
            }
        }
        private Nullable<System.DateTime> _createdTime;
    
        [DataMember]
        public string color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged("color");
                }
            }
        }
        private string _color;
    
        [DataMember]
        public Nullable<int> lineIndex
        {
            get { return _lineIndex; }
            set
            {
                if (_lineIndex != value)
                {
                    _lineIndex = value;
                    OnPropertyChanged("lineIndex");
                }
            }
        }
        private Nullable<int> _lineIndex;
    
        [DataMember]
        public Nullable<int> width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnPropertyChanged("width");
                }
            }
        }
        private Nullable<int> _width;
    
        [DataMember]
        public Nullable<int> X1
        {
            get { return _x1; }
            set
            {
                if (_x1 != value)
                {
                    _x1 = value;
                    OnPropertyChanged("X1");
                }
            }
        }
        private Nullable<int> _x1;
    
        [DataMember]
        public Nullable<int> X2
        {
            get { return _x2; }
            set
            {
                if (_x2 != value)
                {
                    _x2 = value;
                    OnPropertyChanged("X2");
                }
            }
        }
        private Nullable<int> _x2;
    
        [DataMember]
        public Nullable<int> Y1
        {
            get { return _y1; }
            set
            {
                if (_y1 != value)
                {
                    _y1 = value;
                    OnPropertyChanged("Y1");
                }
            }
        }
        private Nullable<int> _y1;
    
        [DataMember]
        public Nullable<int> Y2
        {
            get { return _y2; }
            set
            {
                if (_y2 != value)
                {
                    _y2 = value;
                    OnPropertyChanged("Y2");
                }
            }
        }
        private Nullable<int> _y2;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public SDAnnotation SDAnnotation
        {
            get { return _sDAnnotation; }
            set
            {
                if (!ReferenceEquals(_sDAnnotation, value))
                {
                    var previousValue = _sDAnnotation;
                    _sDAnnotation = value;
                    FixupSDAnnotation(previousValue);
                    OnNavigationPropertyChanged("SDAnnotation");
                }
            }
        }
        private SDAnnotation _sDAnnotation;
    
        [DataMember]
        public DFUPerson DFUPerson
        {
            get { return _dFUPerson; }
            set
            {
                if (!ReferenceEquals(_dFUPerson, value))
                {
                    var previousValue = _dFUPerson;
                    _dFUPerson = value;
                    FixupDFUPerson(previousValue);
                    OnNavigationPropertyChanged("DFUPerson");
                }
            }
        }
        private DFUPerson _dFUPerson;

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
            SDAnnotation = null;
            DFUPerson = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupSDAnnotation(SDAnnotation previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.SDLine.Contains(this))
            {
                previousValue.SDLine.Remove(this);
            }
    
            if (SDAnnotation != null)
            {
                if (!SDAnnotation.SDLine.Contains(this))
                {
                    SDAnnotation.SDLine.Add(this);
                }
    
                sdAnnotationId = SDAnnotation.sdAnnotationId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("SDAnnotation")
                    && (ChangeTracker.OriginalValues["SDAnnotation"] == SDAnnotation))
                {
                    ChangeTracker.OriginalValues.Remove("SDAnnotation");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("SDAnnotation", previousValue);
                }
                if (SDAnnotation != null && !SDAnnotation.ChangeTracker.ChangeTrackingEnabled)
                {
                    SDAnnotation.StartTracking();
                }
            }
        }
    
        private void FixupDFUPerson(DFUPerson previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.SDLine.Contains(this))
            {
                previousValue.SDLine.Remove(this);
            }
    
            if (DFUPerson != null)
            {
                if (!DFUPerson.SDLine.Contains(this))
                {
                    DFUPerson.SDLine.Add(this);
                }
    
                createdById = DFUPerson.dfuPersonId;
            }
            else if (!skipKeys)
            {
                createdById = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("DFUPerson")
                    && (ChangeTracker.OriginalValues["DFUPerson"] == DFUPerson))
                {
                    ChangeTracker.OriginalValues.Remove("DFUPerson");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("DFUPerson", previousValue);
                }
                if (DFUPerson != null && !DFUPerson.ChangeTracker.ChangeTrackingEnabled)
                {
                    DFUPerson.StartTracking();
                }
            }
        }

        #endregion

    }
}
