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
    [KnownType(typeof(L_UsabilityParam))]
    [KnownType(typeof(L_UsabilityParamGrp))]
    public partial class R_UsabilityParamUsabilityGrp: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int R_UsabilityParamUsabilityGrpId
        {
            get { return _r_UsabilityParamUsabilityGrpId; }
            set
            {
                if (_r_UsabilityParamUsabilityGrpId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'R_UsabilityParamUsabilityGrpId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _r_UsabilityParamUsabilityGrpId = value;
                    OnPropertyChanged("R_UsabilityParamUsabilityGrpId");
                }
            }
        }
        private int _r_UsabilityParamUsabilityGrpId;
    
        [DataMember]
        public int usabilityParamId
        {
            get { return _usabilityParamId; }
            set
            {
                if (_usabilityParamId != value)
                {
                    ChangeTracker.RecordOriginalValue("usabilityParamId", _usabilityParamId);
                    if (!IsDeserializing)
                    {
                        if (L_UsabilityParam != null && L_UsabilityParam.usabilityParamId != value)
                        {
                            L_UsabilityParam = null;
                        }
                    }
                    _usabilityParamId = value;
                    OnPropertyChanged("usabilityParamId");
                }
            }
        }
        private int _usabilityParamId;
    
        [DataMember]
        public int usabilityParamGrpId
        {
            get { return _usabilityParamGrpId; }
            set
            {
                if (_usabilityParamGrpId != value)
                {
                    ChangeTracker.RecordOriginalValue("usabilityParamGrpId", _usabilityParamGrpId);
                    if (!IsDeserializing)
                    {
                        if (L_UsabilityParamGrp != null && L_UsabilityParamGrp.usabilityParamGrpId != value)
                        {
                            L_UsabilityParamGrp = null;
                        }
                    }
                    _usabilityParamGrpId = value;
                    OnPropertyChanged("usabilityParamGrpId");
                }
            }
        }
        private int _usabilityParamGrpId;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public L_UsabilityParam L_UsabilityParam
        {
            get { return _l_UsabilityParam; }
            set
            {
                if (!ReferenceEquals(_l_UsabilityParam, value))
                {
                    var previousValue = _l_UsabilityParam;
                    _l_UsabilityParam = value;
                    FixupL_UsabilityParam(previousValue);
                    OnNavigationPropertyChanged("L_UsabilityParam");
                }
            }
        }
        private L_UsabilityParam _l_UsabilityParam;
    
        [DataMember]
        public L_UsabilityParamGrp L_UsabilityParamGrp
        {
            get { return _l_UsabilityParamGrp; }
            set
            {
                if (!ReferenceEquals(_l_UsabilityParamGrp, value))
                {
                    var previousValue = _l_UsabilityParamGrp;
                    _l_UsabilityParamGrp = value;
                    FixupL_UsabilityParamGrp(previousValue);
                    OnNavigationPropertyChanged("L_UsabilityParamGrp");
                }
            }
        }
        private L_UsabilityParamGrp _l_UsabilityParamGrp;

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
            L_UsabilityParam = null;
            L_UsabilityParamGrp = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupL_UsabilityParam(L_UsabilityParam previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_UsabilityParamUsabilityGrp.Contains(this))
            {
                previousValue.R_UsabilityParamUsabilityGrp.Remove(this);
            }
    
            if (L_UsabilityParam != null)
            {
                if (!L_UsabilityParam.R_UsabilityParamUsabilityGrp.Contains(this))
                {
                    L_UsabilityParam.R_UsabilityParamUsabilityGrp.Add(this);
                }
    
                usabilityParamId = L_UsabilityParam.usabilityParamId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_UsabilityParam")
                    && (ChangeTracker.OriginalValues["L_UsabilityParam"] == L_UsabilityParam))
                {
                    ChangeTracker.OriginalValues.Remove("L_UsabilityParam");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_UsabilityParam", previousValue);
                }
                if (L_UsabilityParam != null && !L_UsabilityParam.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_UsabilityParam.StartTracking();
                }
            }
        }
    
        private void FixupL_UsabilityParamGrp(L_UsabilityParamGrp previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_UsabilityParamUsabilityGrp.Contains(this))
            {
                previousValue.R_UsabilityParamUsabilityGrp.Remove(this);
            }
    
            if (L_UsabilityParamGrp != null)
            {
                if (!L_UsabilityParamGrp.R_UsabilityParamUsabilityGrp.Contains(this))
                {
                    L_UsabilityParamGrp.R_UsabilityParamUsabilityGrp.Add(this);
                }
    
                usabilityParamGrpId = L_UsabilityParamGrp.usabilityParamGrpId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_UsabilityParamGrp")
                    && (ChangeTracker.OriginalValues["L_UsabilityParamGrp"] == L_UsabilityParamGrp))
                {
                    ChangeTracker.OriginalValues.Remove("L_UsabilityParamGrp");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_UsabilityParamGrp", previousValue);
                }
                if (L_UsabilityParamGrp != null && !L_UsabilityParamGrp.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_UsabilityParamGrp.StartTracking();
                }
            }
        }

        #endregion

    }
}
