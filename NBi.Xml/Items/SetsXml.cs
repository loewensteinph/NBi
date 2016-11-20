using NBi.Xml.Items.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NBi.Xml.Items
{
    public class SetsXml : DatabaseModelItemXml, IPerspectiveFilter, IModelCollectionItemXml
    {
        [XmlAttribute("perspective")]
        public string Perspective { get; set; }
        
        public override string TypeName
        {
            get { return "sets"; }
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
            values.Add("Sets");
            return values;
        }
    }
}
