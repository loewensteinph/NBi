using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using NBi.Xml.Items;

namespace NBi.Xml.Systems
{
    public class DataTypeXml: AbstractSystemUnderTestXml
    {
        [XmlElement(Type = typeof(MeasureXml), ElementName = "measure"),
        XmlElement(Type = typeof(PropertyXml), ElementName = "property"),
        XmlElement(Type = typeof(ColumnXml), ElementName = "column"),
        ]
        public override ModelItemXml Item { get; set; }

        internal override Dictionary<string, string> GetRegexMatch()
        {
            if (Item is IAutoCategorize)
                return ((IAutoCategorize)Item).GetRegexMatch();
            else
                return new Dictionary<string, string>();
        }

        public override ICollection<string> GetAutoCategories()
        {
            ICollection<string> values = new List<string>();
            if (Item is IAutoCategorize)
                values = ((IAutoCategorize)Item).GetAutoCategories();
            
            values.Add("Data-type");
            return values;
        }
    }
}
