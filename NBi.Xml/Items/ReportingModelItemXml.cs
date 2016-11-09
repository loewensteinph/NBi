using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NBi.Xml.Items
{
    public abstract class ReportingModelItemXml : ModelItemXml
    {
        [XmlAttribute("path")]
        public string Path { get; set; }

        [XmlIgnore]
        public virtual string TypeName
        {
            get { return "report parameters"; }
        }

        public virtual ICollection<string> GetAutoCategories()
        {
            var values = new List<string>();
            if (!string.IsNullOrEmpty(Path))
                values.Add(string.Format("Report's path '{0}'", Path));
            //if (!string.IsNullOrEmpty(Report))
            //    values.Add(string.Format("Report '{0}'", Report));
            values.Add("Report parameters");
            return values;
        }

        public virtual Dictionary<string, string> GetRegexMatch()
        {
            return new Dictionary<string, string>();
        }
    }
}
