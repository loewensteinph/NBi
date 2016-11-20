using NBi.Xml.Items.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NBi.Xml.Items
{
    public class ColumnsXml : DimensionsXml, ITableFilter
    {
        
        [XmlAttribute("table")]
        public string Table { get; set; }

        [XmlIgnore]
        protected virtual string ParentPath { get { return string.Format("[{0}]", Table); } }
        
        [XmlIgnore]
        public override string TypeName
        {
            get { return "tables"; }
        }

        public override Dictionary<string, string> GetRegexMatch()
        {
            var dico = base.GetRegexMatch();
            dico.Add("sut:table", Table);
            return dico;
        }

        public override ICollection<string> GetAutoCategories()
        {
            var values = new List<string>();
            if (!string.IsNullOrEmpty(Perspective))
                values.Add(string.Format("Perspective '{0}'", Perspective));
            if (!string.IsNullOrEmpty(Table))
                values.Add(string.Format("Table '{0}'", Table));
            values.Add("Columns");
            return values;
        }
    }
}
