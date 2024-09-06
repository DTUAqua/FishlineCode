using Babelfisk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FishLineMeasure.ViewModels.Lookups
{
    [DataContract]
    public class LookupItemViewModel : AViewModel
    {
        [DataMember(Name = "Id")]
        private string _id;

        [DataMember(Name = "Display")]
        private string _display;

        [DataMember(Name = "Code")]
        private string _code;

        [DataMember(Name = "Type")]
        private string _type;


        #region Properties


        public string Id
        {
            get { return _id; }
        }

        public string UIDisplay
        {
            get
            {
                return _display;
            }
        }


        public string Code
        {
            get
            {
                return _code;
            }
        }


        public string Type
        {
            get { return _type; }
        }


        public string LookupTypeHeader
        {
            get
            {
                return LookupsViewModel.GetLookupDisplayNameShort(Type);
            }
        }


        #endregion


        public LookupItemViewModel()
        {
            
        }


        public static LookupItemViewModel Create(ILookupEntity lookup)
        {
            var l = new LookupItemViewModel();
            l._id = lookup.Id;
            l._code = Lookups.LookupsViewModel.GetLookupCode(lookup);
            l._display = Lookups.LookupsViewModel.GetLookupUIDisplay(lookup);
            l._type = lookup.GetType().Name;

            return l;
        }


        public static LookupItemViewModel Create(string id, string code, string display, string type)
        {
            var l = new LookupItemViewModel();
            l._id = id;
            l._code = code;
            l._display = display;
            l._type = type;

            return l;
        }

        public static LookupItemViewModel Create(XElement xElm)
        {
            var l = new LookupItemViewModel();

           var xAttr = xElm.Attribute("type");

            if (xAttr != null && !string.IsNullOrWhiteSpace(xAttr.Value))
                l._type = xAttr.Value;

            xAttr = xElm.Attribute("id");

            if (xAttr != null && !string.IsNullOrWhiteSpace(xAttr.Value))
                l._id = xAttr.Value;

            xAttr = xElm.Attribute("code");

            if (xAttr != null && !string.IsNullOrWhiteSpace(xAttr.Value))
                l._code = xAttr.Value;

            xAttr = xElm.Attribute("display");

            if (xAttr != null && !string.IsNullOrWhiteSpace(xAttr.Value))
                l._display = xAttr.Value;

            return l;
        }


        public XElement ToXElement()
        {
            var xElm = new XElement("Lookup", 
                                 new XAttribute("type", Type ?? ""),
                                 new XAttribute("id", Id ?? ""),
                                 new XAttribute("code", Code ?? ""),
                                 new XAttribute("display", UIDisplay ?? "")
                                 );

            return xElm;
        }

    }
}
