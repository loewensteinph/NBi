using NBi.Xml.Items.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NBi.Xml.Items
{
    public class DimensionXml : DatabaseModelItemXml, IPerspectiveFilter, IModelSingleItemXml
    {

        [XmlAttribute("caption")]
        public string Caption { get; set; }

        [XmlAttribute("perspective")]
        public string Perspective { get; set; }

        [XmlIgnore]
        protected virtual string Path { get { return string.Format("[{0}]", Caption); } }

        public override string TypeName
        {
            get { return "dimension"; }
        }

        public override Dictionary<string, string> GetRegexMatch()
        {
            var dico = base.GetRegexMatch();
            dico.Add("sut:perspective", Perspective);
            return dico;
        }

        public override ICollection<string> GetAutoCategories()
        {
            var values = new List<string>();
            if (!string.IsNullOrEmpty(Perspective))
                values.Add(string.Format("Perspective '{0}'", Perspective));
            values.Add(string.Format("Dimension '{0}'", Caption));
            values.Add("Dimensions");
            return values;
        }
    }
}
