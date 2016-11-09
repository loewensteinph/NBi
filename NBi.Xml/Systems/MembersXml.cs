using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using NBi.Xml.Items;

namespace NBi.Xml.Systems
{
    public class MembersXml : AbstractSystemUnderTestXml
    {
        public MembersXml() : base()
        {
            Exclude = new ExcludeXml();
        }
        
        [XmlAttribute("children-of")]
        public string ChildrenOf { get; set; }

        [XmlElement(Type = typeof(DimensionXml), ElementName = "dimension"),
        XmlElement(Type = typeof(HierarchyXml), ElementName = "hierarchy"),
        XmlElement(Type = typeof(LevelXml), ElementName = "level"),
        XmlElement(Type = typeof(SetXml), ElementName = "set")
        ]
        public override ModelItemXml Item { get; set; }

        [XmlElement("exclude")]
        public ExcludeXml Exclude { get; set; }

        [XmlIgnore]
        public bool ExcludeSpecified
        {
            get
            {
                return !(
                            Exclude == null 
                            || (
                                    (Exclude.Items==null || Exclude.Items.Count == 0) 
                                    && (Exclude.Patterns==null || Exclude.Patterns.Count == 0)
                                )
                         );
            }
            set { return; }
        }

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

            values.Add("Members");
            return values;
        }
    }
}
