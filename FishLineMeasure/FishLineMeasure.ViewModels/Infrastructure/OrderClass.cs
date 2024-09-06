using Babelfisk.Entities;
using FishLineMeasure.ViewModels.CustomControls;
using FishLineMeasure.ViewModels.Lookups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace FishLineMeasure.ViewModels.Infrastructure
{
    [DataContract]
    public class OrderClass : AViewModel
    {

        private List<Lookups.LookupItemViewModel> _lstLookups;

        [DataMember]
        public List<Lookups.LookupItemViewModel> Lookups
        {
            get { return _lstLookups; }
            set
            {
                _lstLookups = value;
                RaisePropertyChanged(() => Lookups);
            }
        }

        public string GroupString
        {
            get
            {
                if (Lookups == null || Lookups.Count == 0)
                    return "";

                
                return string.Join(", ", Lookups.Select(x => x.Code ?? "N/A"));
            }
        }

        public string GroupStringWithHeaders
        {
            get
            {
                if (Lookups == null || Lookups.Count == 0)
                    return "";


                return string.Join(", ", Lookups.Select(x => string.Format("{0}: {1}", x.LookupTypeHeader ?? "N/A", x.Code ?? "N/A")));
            }
        }

        public OrderClass()
        {
            Lookups = new List<Lookups.LookupItemViewModel>();
        }

        public OrderClass(List<Lookups.LookupItemViewModel> lstLookups)
        {
            Lookups = lstLookups;
        }


        public string GetLookupCode(LookupType type)
        {
            string res = null;

            var typeString = LookupsViewModel.GetLookupType(type).Name;
            var l = Lookups.Where(x => x.Type != null && x.Type.Equals(typeString, System.StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if(l != null)
                res = l.Code;

            return res;
        }


        public static OrderClass Create(XElement xElm)
        {
            var res = new OrderClass();

            foreach(var l in xElm.Elements())
            {
                var li = LookupItemViewModel.Create(l);
                res.Lookups.Add(li);
            }

            return res;
        }


        public XElement ToXElement()
        {
            var xElm = new XElement("Lookups");

            foreach(var l in Lookups)
                xElm.Add(l.ToXElement());

            return xElm;
        }
    }
    
}
