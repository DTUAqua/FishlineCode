using FishLineMeasure.ViewModels.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Infrastructure
{
    [DataContract]
    [KnownType(typeof(OrderClass))]
    [KnownType(typeof(AViewModel))]
    [KnownType(typeof(LookupItemViewModel))]
    public class OrderClassGroup : AViewModel
    {
        private string _name;

        private List<OrderClass> _orderClasses;

        private bool _isEditable;


        #region Properties


        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }


        [DataMember]
        public List<OrderClass> OrderClasses
        {
            get { return _orderClasses; }
            set
            {
                _orderClasses = value;
                RaisePropertyChanged(nameof(OrderClasses));
            }
        }

        [DataMember]
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                RaisePropertyChanged(nameof(IsEditable));
            }
        }


        public OrderClassGroup()
        {
            _orderClasses = new List<OrderClass>();
            _isEditable = false;
        }


        #endregion

    }
}
