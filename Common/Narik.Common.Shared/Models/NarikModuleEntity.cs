using System.Xml.Serialization;
using Narik.Common.Shared.Interfaces;

namespace Narik.Common.Shared.Models
{
    [XmlRoot("SaturnModule")]
    public class NarikModuleEntity:INarikModuleEntity
    {

        [XmlAttribute("Version")]
        public string Version { get; set; }


        [XmlAttribute("Key")]
        public string Key { get; set; }
      

        [XmlAttribute("InitOrder")]
        public int InitOrder { get; set; }

        [XmlAttribute("MenuOrder")]
        public int MenuOrder { get; set; }

        public int Id { get; set; }
    }
}
