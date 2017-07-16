using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NBi.Core;
using NBi.Xml.Items;

namespace NBi.Xml.Decoration.Command
{
    public abstract class DecorationCommandXml : IDecorationCommand
    {

        [XmlIgnore()]
        public virtual Settings.SettingsXml Settings { get; set; }

        [XmlElement("variable")]
        public List<QueryTemplateVariableXml> Variables { get; set; }

        public virtual List<QueryTemplateVariableXml> GetVariables()
        {
            var list = Variables;
            foreach (var variable in Variables)
                if (!Variables.Exists(p => p.Name == variable.Name))
                    list.Add(variable);

            return list;
        }
    }
}
