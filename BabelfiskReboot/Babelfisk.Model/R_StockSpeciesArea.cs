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
    [KnownType(typeof(L_DFUArea))]
    [KnownType(typeof(L_Species))]
    [KnownType(typeof(L_StatisticalRectangle))]
    [KnownType(typeof(L_Stock))]
    public partial class R_StockSpeciesArea: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int r_StockSpeciesAreaId
        {
            get { return _r_StockSpeciesAreaId; }
            set
            {
                if (_r_StockSpeciesAreaId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'r_StockSpeciesAreaId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _r_StockSpeciesAreaId = value;
                    OnPropertyChanged("r_StockSpeciesAreaId");
                }
            }
        }
        private int _r_StockSpeciesAreaId;
    
        [DataMember]
        public int L_stockId
        {
            get { return _l_stockId; }
            set
            {
                if (_l_stockId != value)
                {
                    ChangeTracker.RecordOriginalValue("L_stockId", _l_stockId);
                    if (!IsDeserializing)
                    {
                        if (L_Stock != null && L_Stock.L_stockId != value)
                        {
                            L_Stock = null;
                        }
                    }
                    _l_stockId = value;
                    OnPropertyChanged("L_stockId");
                }
            }
        }
        private int _l_stockId;
    
        [DataMember]
        public string DFUArea
        {
            get { return _dFUArea; }
            set
            {
                if (_dFUArea != value)
                {
                    ChangeTracker.RecordOriginalValue("DFUArea", _dFUArea);
                    if (!IsDeserializing)
                    {
                        if (L_DFUArea != null && L_DFUArea.DFUArea != value)
                        {
                            L_DFUArea = null;
                        }
                    }
                    _dFUArea = value;
                    OnPropertyChanged("DFUArea");
                }
            }
        }
        private string _dFUArea;
    
        [DataMember]
        public string speciesCode
        {
            get { return _speciesCode; }
            set
            {
                if (_speciesCode != value)
                {
                    ChangeTracker.RecordOriginalValue("speciesCode", _speciesCode);
                    if (!IsDeserializing)
                    {
                        if (L_Species != null && L_Species.speciesCode != value)
                        {
                            L_Species = null;
                        }
                    }
                    _speciesCode = value;
                    OnPropertyChanged("speciesCode");
                }
            }
        }
        private string _speciesCode;
    
        [DataMember]
        public string statisticalRectangle
        {
            get { return _statisticalRectangle; }
            set
            {
                if (_statisticalRectangle != value)
                {
                    ChangeTracker.RecordOriginalValue("statisticalRectangle", _statisticalRectangle);
                    if (!IsDeserializing)
                    {
                        if (L_StatisticalRectangle != null && L_StatisticalRectangle.statisticalRectangle != value)
                        {
                            L_StatisticalRectangle = null;
                        }
                    }
                    _statisticalRectangle = value;
                    OnPropertyChanged("statisticalRectangle");
                }
            }
        }
        private string _statisticalRectangle;
    
        [DataMember]
        public Nullable<int> quarter
        {
            get { return _quarter; }
            set
            {
                if (_quarter != value)
                {
                    _quarter = value;
                    OnPropertyChanged("quarter");
                }
            }
        }
        private Nullable<int> _quarter;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public L_DFUArea L_DFUArea
        {
            get { return _l_DFUArea; }
            set
            {
                if (!ReferenceEquals(_l_DFUArea, value))
                {
                    var previousValue = _l_DFUArea;
                    _l_DFUArea = value;
                    FixupL_DFUArea(previousValue);
                    OnNavigationPropertyChanged("L_DFUArea");
                }
            }
        }
        private L_DFUArea _l_DFUArea;
    
        [DataMember]
        public L_Species L_Species
        {
            get { return _l_Species; }
            set
            {
                if (!ReferenceEquals(_l_Species, value))
                {
                    var previousValue = _l_Species;
                    _l_Species = value;
                    FixupL_Species(previousValue);
                    OnNavigationPropertyChanged("L_Species");
                }
            }
        }
        private L_Species _l_Species;
    
        [DataMember]
        public L_StatisticalRectangle L_StatisticalRectangle
        {
            get { return _l_StatisticalRectangle; }
            set
            {
                if (!ReferenceEquals(_l_StatisticalRectangle, value))
                {
                    var previousValue = _l_StatisticalRectangle;
                    _l_StatisticalRectangle = value;
                    FixupL_StatisticalRectangle(previousValue);
                    OnNavigationPropertyChanged("L_StatisticalRectangle");
                }
            }
        }
        private L_StatisticalRectangle _l_StatisticalRectangle;
    
        [DataMember]
        public L_Stock L_Stock
        {
            get { return _l_Stock; }
            set
            {
                if (!ReferenceEquals(_l_Stock, value))
                {
                    var previousValue = _l_Stock;
                    _l_Stock = value;
                    FixupL_Stock(previousValue);
                    OnNavigationPropertyChanged("L_Stock");
                }
            }
        }
        private L_Stock _l_Stock;

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
            L_DFUArea = null;
            L_Species = null;
            L_StatisticalRectangle = null;
            L_Stock = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupL_DFUArea(L_DFUArea previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_StockSpeciesArea.Contains(this))
            {
                previousValue.R_StockSpeciesArea.Remove(this);
            }
    
            if (L_DFUArea != null)
            {
                if (!L_DFUArea.R_StockSpeciesArea.Contains(this))
                {
                    L_DFUArea.R_StockSpeciesArea.Add(this);
                }
    
                DFUArea = L_DFUArea.DFUArea;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_DFUArea")
                    && (ChangeTracker.OriginalValues["L_DFUArea"] == L_DFUArea))
                {
                    ChangeTracker.OriginalValues.Remove("L_DFUArea");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_DFUArea", previousValue);
                }
                if (L_DFUArea != null && !L_DFUArea.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_DFUArea.StartTracking();
                }
            }
        }
    
        private void FixupL_Species(L_Species previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_StockSpeciesArea.Contains(this))
            {
                previousValue.R_StockSpeciesArea.Remove(this);
            }
    
            if (L_Species != null)
            {
                if (!L_Species.R_StockSpeciesArea.Contains(this))
                {
                    L_Species.R_StockSpeciesArea.Add(this);
                }
    
                speciesCode = L_Species.speciesCode;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_Species")
                    && (ChangeTracker.OriginalValues["L_Species"] == L_Species))
                {
                    ChangeTracker.OriginalValues.Remove("L_Species");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_Species", previousValue);
                }
                if (L_Species != null && !L_Species.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_Species.StartTracking();
                }
            }
        }
    
        private void FixupL_StatisticalRectangle(L_StatisticalRectangle previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_StockSpeciesArea.Contains(this))
            {
                previousValue.R_StockSpeciesArea.Remove(this);
            }
    
            if (L_StatisticalRectangle != null)
            {
                if (!L_StatisticalRectangle.R_StockSpeciesArea.Contains(this))
                {
                    L_StatisticalRectangle.R_StockSpeciesArea.Add(this);
                }
    
                statisticalRectangle = L_StatisticalRectangle.statisticalRectangle;
            }
            else if (!skipKeys)
            {
                statisticalRectangle = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_StatisticalRectangle")
                    && (ChangeTracker.OriginalValues["L_StatisticalRectangle"] == L_StatisticalRectangle))
                {
                    ChangeTracker.OriginalValues.Remove("L_StatisticalRectangle");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_StatisticalRectangle", previousValue);
                }
                if (L_StatisticalRectangle != null && !L_StatisticalRectangle.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_StatisticalRectangle.StartTracking();
                }
            }
        }
    
        private void FixupL_Stock(L_Stock previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.R_StockSpeciesArea.Contains(this))
            {
                previousValue.R_StockSpeciesArea.Remove(this);
            }
    
            if (L_Stock != null)
            {
                if (!L_Stock.R_StockSpeciesArea.Contains(this))
                {
                    L_Stock.R_StockSpeciesArea.Add(this);
                }
    
                L_stockId = L_Stock.L_stockId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("L_Stock")
                    && (ChangeTracker.OriginalValues["L_Stock"] == L_Stock))
                {
                    ChangeTracker.OriginalValues.Remove("L_Stock");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("L_Stock", previousValue);
                }
                if (L_Stock != null && !L_Stock.ChangeTracker.ChangeTrackingEnabled)
                {
                    L_Stock.StartTracking();
                }
            }
        }

        #endregion

    }
}
