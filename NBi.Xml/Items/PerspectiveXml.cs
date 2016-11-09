using NBi.Xml.Items.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NBi.Xml.Items
{

    public class PerspectiveXml : DatabaseModelItemXml, IOwnerFilter
    {
        [XmlAttribute("owner")]
        public string Owner { get; set; }

        [XmlIgnore]
        public override string TypeName
        {
            get { return "perspective"; }
        }

        public override ICollection<string> GetAutoCategories()
        {
            var values = new List<string>();
            values.Add(string.Format("Perspective '{0}'", Caption));
            values.Add("Perspectives");
            return values;
        }
    }
}
