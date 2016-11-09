using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using NBi.Xml.Items;
using NBi.Xml.Settings;

namespace NBi.Xml.Systems
{
    public abstract class AbstractSystemUnderTestXml
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        private DefaultXml _default;
        [XmlIgnore()]
        public virtual DefaultXml Default
        {
            get { return _default; }
            set
            {
                _default = value;
                if (Item != null)
                    Item.Default = value;
            }
        }
        private SettingsXml settings;
        [XmlIgnore()]
        public virtual SettingsXml Settings
        {
            get { return settings; }
            set
            {
                settings = value;
                if (Item != null)
                    Item.Settings = value;
            }
        }

        [XmlIgnore()]
        public abstract ModelItemXml Item { get; set; }

        public AbstractSystemUnderTestXml()
        {
            Default = new DefaultXml();
            Settings = new SettingsXml();
        }

        internal virtual Dictionary<string, string> GetRegexMatch()
        {
            return new Dictionary<string, string>();
        }

        public abstract ICollection<string> GetAutoCategories();
    }
}
