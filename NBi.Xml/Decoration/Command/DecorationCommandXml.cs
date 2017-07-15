using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NBi.Core;
using NBi.Xml.Items;
using NBi.Xml.Settings;

namespace NBi.Xml.Decoration.Command
{
    public abstract class DecorationCommandXml : IDecorationCommand
    {
        [XmlIgnore()]
        public virtual Settings.SettingsXml Settings { get; set; }
    }
}
