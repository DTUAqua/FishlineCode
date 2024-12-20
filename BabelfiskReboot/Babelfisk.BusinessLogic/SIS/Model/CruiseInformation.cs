//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace Babelfisk.BusinessLogic.SIS.Model
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(L_ShipInformation))]
    [KnownType(typeof(GearData))]
    [KnownType(typeof(SampleLog))]
    [KnownType(typeof(TrawlLog))]
    public partial class CruiseInformation
    {
        #region Primitive Properties
        [DataMember]
        public virtual int cruiseID
        {
            get;
            set;
        }
        [DataMember]
        public virtual string shipCode
        {
            get { return _shipCode; }
            set
            {
                if (_shipCode != value)
                {
                    if (L_ShipInformation != null && L_ShipInformation.shipCode != value)
                    {
                        L_ShipInformation = null;
                    }
                    _shipCode = value;
                }
            }
        }
        private string _shipCode;
        [DataMember]
        public virtual int cruiseYear
        {
            get;
            set;
        }
        [DataMember]
        public virtual int cruiseNo
        {
            get;
            set;
        }
        [DataMember]
        public virtual Nullable<short> cruiseDays
        {
            get;
            set;
        }
        [DataMember]
        public virtual string cruiseName
        {
            get;
            set;
        }
        [DataMember]
        public virtual string projectType
        {
            get;
            set;
        }
        [DataMember]
        public virtual string cruiseLeader
        {
            get;
            set;
        }
        [DataMember]
        public virtual string assistCruiseLeader
        {
            get;
            set;
        }
        [DataMember]
        public virtual string captain
        {
            get;
            set;
        }
        [DataMember]
        public virtual string technician
        {
            get;
            set;
        }
        [DataMember]
        public virtual string cruiseArea
        {
            get;
            set;
        }
        [DataMember]
        public virtual Nullable<System.DateTime> startDate
        {
            get;
            set;
        }
        [DataMember]
        public virtual Nullable<System.DateTime> endDate
        {
            get;
            set;
        }
        [DataMember]
        public virtual string institution
        {
            get;
            set;
        }
        [DataMember]
        public virtual string institute
        {
            get;
            set;
        }
        [DataMember]
        public virtual string projectArea
        {
            get;
            set;
        }
        [DataMember]
        public virtual string projectNo
        {
            get;
            set;
        }
        [DataMember]
        public virtual string remarks
        {
            get;
            set;
        }
        [DataMember]
        public virtual bool status
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
        
    
        [DataMember]
        public virtual L_ShipInformation L_ShipInformation
        {
            get { return _l_ShipInformation; }
            set
            {
                if (!ReferenceEquals(_l_ShipInformation, value))
                {
                    var previousValue = _l_ShipInformation;
                    _l_ShipInformation = value;
                    FixupL_ShipInformation(previousValue);
                }
            }
        }
        private L_ShipInformation _l_ShipInformation;
        
    
        [DataMember]
        public virtual ICollection<GearData> GearData
        {
            get
            {
                if (_gearData == null)
                {
                    var newCollection = new FixupCollection<GearData>();
                    newCollection.CollectionChanged += FixupGearData;
                    _gearData = newCollection;
                }
                return _gearData;
            }
            set
            {
                if (!ReferenceEquals(_gearData, value))
                {
                    var previousValue = _gearData as FixupCollection<GearData>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupGearData;
                    }
                    _gearData = value;
                    var newValue = value as FixupCollection<GearData>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupGearData;
                    }
                }
            }
        }
        private ICollection<GearData> _gearData;
        
    
        [DataMember]
        public virtual ICollection<SampleLog> SampleLog
        {
            get
            {
                if (_sampleLog == null)
                {
                    var newCollection = new FixupCollection<SampleLog>();
                    newCollection.CollectionChanged += FixupSampleLog;
                    _sampleLog = newCollection;
                }
                return _sampleLog;
            }
            set
            {
                if (!ReferenceEquals(_sampleLog, value))
                {
                    var previousValue = _sampleLog as FixupCollection<SampleLog>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSampleLog;
                    }
                    _sampleLog = value;
                    var newValue = value as FixupCollection<SampleLog>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSampleLog;
                    }
                }
            }
        }
        private ICollection<SampleLog> _sampleLog;
        
    
        [DataMember]
        public virtual ICollection<TrawlLog> TrawlLog
        {
            get
            {
                if (_trawlLog == null)
                {
                    var newCollection = new FixupCollection<TrawlLog>();
                    newCollection.CollectionChanged += FixupTrawlLog;
                    _trawlLog = newCollection;
                }
                return _trawlLog;
            }
            set
            {
                if (!ReferenceEquals(_trawlLog, value))
                {
                    var previousValue = _trawlLog as FixupCollection<TrawlLog>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTrawlLog;
                    }
                    _trawlLog = value;
                    var newValue = value as FixupCollection<TrawlLog>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTrawlLog;
                    }
                }
            }
        }
        private ICollection<TrawlLog> _trawlLog;

        #endregion

        #region Association Fixup
    
        private void FixupL_ShipInformation(L_ShipInformation previousValue)
        {
            if (previousValue != null && previousValue.CruiseInformation.Contains(this))
            {
                previousValue.CruiseInformation.Remove(this);
            }
    
            if (L_ShipInformation != null)
            {
                if (!L_ShipInformation.CruiseInformation.Contains(this))
                {
                    L_ShipInformation.CruiseInformation.Add(this);
                }
                if (shipCode != L_ShipInformation.shipCode)
                {
                    shipCode = L_ShipInformation.shipCode;
                }
            }
        }
    
        private void FixupGearData(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GearData item in e.NewItems)
                {
                    item.CruiseInformation = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GearData item in e.OldItems)
                {
                    if (ReferenceEquals(item.CruiseInformation, this))
                    {
                        item.CruiseInformation = null;
                    }
                }
            }
        }
    
        private void FixupSampleLog(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SampleLog item in e.NewItems)
                {
                    item.CruiseInformation = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SampleLog item in e.OldItems)
                {
                    if (ReferenceEquals(item.CruiseInformation, this))
                    {
                        item.CruiseInformation = null;
                    }
                }
            }
        }
    
        private void FixupTrawlLog(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TrawlLog item in e.NewItems)
                {
                    item.CruiseInformation = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TrawlLog item in e.OldItems)
                {
                    if (ReferenceEquals(item.CruiseInformation, this))
                    {
                        item.CruiseInformation = null;
                    }
                }
            }
        }

        #endregion

    }
}
