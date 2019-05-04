using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Narik.Common.Infrastructure.Extension
{
    public static  class XElementHelper
    {
        public static T ToObject<T>(this XElement element) where T:class
        {
            StringReader reader = new StringReader(element.ToString());
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            return  (T)xmlSerializer.Deserialize(reader);
        }

        public static List<T> ToListOfObject<T>(this XElement element) where T : class
        {
            return (from xElement in element.Elements()
                    select new StringReader(xElement.ToString()) into reader
                    let xmlSerializer = new XmlSerializer(typeof (T)) select (T) xmlSerializer.Deserialize(reader))
                    .ToList();
        }
    }
}
