using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NBi.Core.Rest
{
    public enum CredentialsType
    {
        [XmlEnum("anonymous")]
        Anonymous = 0,
        [XmlEnum("current-user")]
        CurrentUser = 1
    }
}
