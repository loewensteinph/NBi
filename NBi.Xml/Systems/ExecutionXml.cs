using System.Collections.Generic;
using System.Xml.Serialization;
using NBi.Xml.Items;
using NBi.Xml.Settings;
using NBi.Xml.Constraints;

namespace NBi.Xml.Systems
{
    public class ExecutionXml : AbstractSystemUnderTestXml, IReferenceFriendly
    {       
        [XmlElement(Type = typeof(QueryXml), ElementName = "query"),
        XmlElement(Type = typeof(AssemblyXml), ElementName = "assembly"),
        XmlElement(Type = typeof(ReportXml), ElementName = "report"),
        XmlElement(Type = typeof(EtlXml), ElementName = "etl"),
        ]
        public override ModelItemXml Item { get; set; }

        public ExecutableXml BaseItem
        {
            get
            {
                return (ExecutableXml)BaseItem;
            }
        }

        internal override Dictionary<string, string> GetRegexMatch()
        {
            var dico = base.GetRegexMatch();
            return dico;
        }

        public override ICollection<string> GetAutoCategories()
        {
            return new string[] { "Execution" };
        }

        public void AssignReferences(IEnumerable<ReferenceXml> references)
        {
            if (Item is IReferenceFriendly)
                ((IReferenceFriendly)Item).AssignReferences(references);
        }
    }
}
