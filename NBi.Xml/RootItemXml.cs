using NBi.Xml.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NBi.Xml
{
    public abstract class RootItemXml
    {
        [XmlIgnore()]
        public virtual DefaultXml Default { get; set; }
        [XmlIgnore()]
        public virtual SettingsXml Settings { get; set; }

        public RootItemXml()
        {
            Default = new DefaultXml();
            Settings = new SettingsXml();
        }
    }
}
