using System.Xml.Serialization;
using NBi.Core.ResultSet;
using System.Collections.Generic;
using System.Linq;

namespace NBi.Xml.Items.ResultSet
{
    public class ItemXml
    {
        [XmlText]
        public string Value { get; set; }
        
        public override string ToString()
        {
            return Value;
        }
    }
}
