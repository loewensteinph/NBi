using NBi.Xml.Items.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NBi.Xml.Items
{
    public class ReportParametersXml : ReportingModelItemXml
    {

        [XmlAttribute("report")]
        public string Report { get; set; }

        [XmlIgnore]
        protected virtual string ParentPath { get { return string.Format("[{0}]", Report); } }
        

    }
}
