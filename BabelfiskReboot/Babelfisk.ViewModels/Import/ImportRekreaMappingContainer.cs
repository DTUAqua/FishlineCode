using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Babelfisk.ViewModels.Import
{
    public class ImportRekreaMappingContainer
    {
        public enum RekreaMappingType
        {
            L_TripTypeCode,
            DFUPersonInitials,
            CruiseName
        }

        private XElement _xeMap;

        public ImportRekreaMappingContainer(XElement xeMap)
        {
            _xeMap = xeMap;

            _xeMap = new XElement("Mappings",
                                  new XElement("LookupMaps",
                                               //Trip type mapping
                                               new XElement("LookupMap", new XAttribute("lookupName", "L_TripTypeCode"),
                                                           new XElement("LookupMapping", new XAttribute("from", "Havn"), new XAttribute("to", "REKHVN")),
                                                           new XElement("LookupMapping", new XAttribute("from", "Turbåd"), new XAttribute("to", "REKTBD")),
                                                           new XElement("LookupMapping", new XAttribute("from", "Område"), new XAttribute("to", "REKOMR"))
                                                           ),

                                               new XElement("LookupMap", new XAttribute("lookupName", "DFUPersonInitials"),
                                                           new XElement("LookupMapping", new XAttribute("from", "adh"), new XAttribute("to", "adh"))
                                                           ),

                                               new XElement("LookupMap", new XAttribute("lookupName", "CruiseName"),
                                                           new XElement("LookupMapping", new XAttribute("from", "Laks"), new XAttribute("to", "REKREA laks")),
                                                           new XElement("LookupMapping", new XAttribute("from", "Torsk"), new XAttribute("to", "REKREA torsk"))
                                                           )
                                              )
                                 );
        }


        /// <summary>
        /// Search for fromValue in the given lookup type.
        /// </summary>
        public string Map(string fromValue, RekreaMappingType enm)
        {
            if (_xeMap == null)
                return null;

            var type = enm.ToString();

            var xeMappings = _xeMap.Element("Mappings");
            var xeLookupMaps = _xeMap.Element("LookupMaps");
                
            var xeLookupMap = (from elm in xeLookupMaps.Elements()
                               let aln = elm.Attribute("lookupName")
                               where aln != null && aln.Value != null && aln.Value.Equals(type, StringComparison.InvariantCultureIgnoreCase)
                               select elm).FirstOrDefault();

            if (xeLookupMap == null)
                return null;

            var lookupMapping = (from elm in xeLookupMap.Elements()
                                 let aFrom = elm.Attribute("from")
                                 where aFrom.Value != null && aFrom.Value.Equals(fromValue, StringComparison.InvariantCultureIgnoreCase)
                                 select elm).FirstOrDefault();

            if (lookupMapping != null && lookupMapping.Attribute("to") != null)
                return lookupMapping.Attribute("to").Value;

            return null;
        }

    }
}
