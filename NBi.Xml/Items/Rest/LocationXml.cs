using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using NBi.Core;
using NBi.Core.Report;
using NBi.Xml.Settings;
using NBi.Xml.Constraints;
using System.ComponentModel;
using NBi.Core.Rest;

namespace NBi.Xml.Items.Rest
{
    public class LocationXml
    {
        [DefaultValue("")]
        [XmlAttribute("base-address")]
        public string BaseAddress { get; set; }

        [DefaultValue(ContentType.Json)]
        [XmlAttribute("content-type")]
        public ContentType ContentType { get; set; }

        public LocationXml()
        {
            ContentType = ContentType.Json;
        }

        private static LocationXml blank;
        public static LocationXml Blank
        {
            get
            {
                if (blank == null)
                {
                    blank = new LocationXml()
                    {
                        BaseAddress = string.Empty,
                        ContentType = ContentType.Json
                    };
                }
                return blank;
            }
        }
    }
}
