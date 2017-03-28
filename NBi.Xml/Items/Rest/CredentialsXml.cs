using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using NBi.Core;
using NBi.Core.Report;
using NBi.Xml.Settings;
using NBi.Xml.Constraints;
using NBi.Core.Rest;
using System.ComponentModel;

namespace NBi.Xml.Items.Rest
{
    public class CredentialsXml
    {
        [DefaultValue(CredentialsType.Anonymous)]
        [XmlAttribute("type")]
        public CredentialsType Type { get; set; }

        public CredentialsXml()
        {
            Type = CredentialsType.Anonymous;
        }

        private static CredentialsXml blank;
        public static CredentialsXml Blank
        {
            get
            {
                if (blank == null)
                {
                    blank = new CredentialsXml()
                    {
                        Type = CredentialsType.Anonymous
                    };
                }
                return blank;
            }
        }
    }
}
