using NBi.Xml.Items.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NBi.Xml.Items
{
    public class ReportParameterXml : ReportingModelItemXml, IModelSingleItemXml
    {
        [XmlAttribute("report")]
        public string Report { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("caption")]
        public string Caption { get; set; }

        [XmlIgnore]
        public override string TypeName
        {
            get { return "report's parameter"; }
        }

        public override Dictionary<string, string> GetRegexMatch()
        {
            var dico = new Dictionary<string, string>();
            dico.Add("sut:caption", Caption);
            dico.Add("sut:typeName", TypeName);
            return dico;
        }

        public override ICollection<string> GetAutoCategories()
        {
            var values = new List<string>();
            if (!string.IsNullOrEmpty(Path))
                values.Add(string.Format("Report's path '{0}'", Path));
            if (!string.IsNullOrEmpty(Report))
                values.Add(string.Format("Report '{0}'", Report));
            values.Add("Report parameters");
            return values;
        }
    }
}
