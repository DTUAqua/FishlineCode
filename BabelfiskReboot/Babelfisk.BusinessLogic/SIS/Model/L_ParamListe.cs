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
    [KnownType(typeof(ShipData))]
    [KnownType(typeof(TrawlData))]
    [KnownType(typeof(L_SampleType))]
    public partial class L_ParamListe
    {
        #region Primitive Properties
        [DataMember]
        public virtual int paramID
        {
            get;
            set;
        }
        [DataMember]
        public virtual string paramCode
        {
            get;
            set;
        }
        [DataMember]
        public virtual string description
        {
            get;
            set;
        }
        [DataMember]
        public virtual string unit
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
        
    
        [DataMember]
        public virtual ICollection<ShipData> ShipData
        {
            get
            {
                if (_shipData == null)
                {
                    var newCollection = new FixupCollection<ShipData>();
                    newCollection.CollectionChanged += FixupShipData;
                    _shipData = newCollection;
                }
                return _shipData;
            }
            set
            {
                if (!ReferenceEquals(_shipData, value))
                {
                    var previousValue = _shipData as FixupCollection<ShipData>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupShipData;
                    }
                    _shipData = value;
                    var newValue = value as FixupCollection<ShipData>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupShipData;
                    }
                }
            }
        }
        private ICollection<ShipData> _shipData;
        
    
        [DataMember]
        public virtual ICollection<TrawlData> TrawlData
        {
            get
            {
                if (_trawlData == null)
                {
                    var newCollection = new FixupCollection<TrawlData>();
                    newCollection.CollectionChanged += FixupTrawlData;
                    _trawlData = newCollection;
                }
                return _trawlData;
            }
            set
            {
                if (!ReferenceEquals(_trawlData, value))
                {
                    var previousValue = _trawlData as FixupCollection<TrawlData>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTrawlData;
                    }
                    _trawlData = value;
                    var newValue = value as FixupCollection<TrawlData>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTrawlData;
                    }
                }
            }
        }
        private ICollection<TrawlData> _trawlData;
        
    
        [DataMember]
        public virtual ICollection<L_SampleType> L_SampleType
        {
            get
            {
                if (_l_SampleType == null)
                {
                    var newCollection = new FixupCollection<L_SampleType>();
                    newCollection.CollectionChanged += FixupL_SampleType;
                    _l_SampleType = newCollection;
                }
                return _l_SampleType;
            }
            set
            {
                if (!ReferenceEquals(_l_SampleType, value))
                {
                    var previousValue = _l_SampleType as FixupCollection<L_SampleType>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupL_SampleType;
                    }
                    _l_SampleType = value;
                    var newValue = value as FixupCollection<L_SampleType>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupL_SampleType;
                    }
                }
            }
        }
        private ICollection<L_SampleType> _l_SampleType;

        #endregion
        #region Association Fixup
    
        private void FixupShipData(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ShipData item in e.NewItems)
                {
                    item.L_ParamListe = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ShipData item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_ParamListe, this))
                    {
                        item.L_ParamListe = null;
                    }
                }
            }
        }
    
        private void FixupTrawlData(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TrawlData item in e.NewItems)
                {
                    item.L_ParamListe = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TrawlData item in e.OldItems)
                {
                    if (ReferenceEquals(item.L_ParamListe, this))
                    {
                        item.L_ParamListe = null;
                    }
                }
            }
        }
    
        private void FixupL_SampleType(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (L_SampleType item in e.NewItems)
                {
                    if (!item.L_ParamListe.Contains(this))
                    {
                        item.L_ParamListe.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (L_SampleType item in e.OldItems)
                {
                    if (item.L_ParamListe.Contains(this))
                    {
                        item.L_ParamListe.Remove(this);
                    }
                }
            }
        }

        #endregion
    }
}
