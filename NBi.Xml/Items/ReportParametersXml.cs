using NBi.Xml.Items.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NBi.Xml.Items
{
    public class ReportParametersXml : AbstractItem
    {

        [XmlAttribute("path")]
        public string Path { get; set; }

        [XmlAttribute("report")]
        public string Report { get; set; }

        [XmlIgnore]
        protected virtual string ParentPath { get { return string.Format("[{0}]", Report); } }
        

        [XmlIgnore]
        public override string TypeName
        {
            get { return "report parameters"; }
        }
        
        internal override ICollection<string> GetAutoCategories()
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
