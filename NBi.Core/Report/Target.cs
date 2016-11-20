using System;
using System.Linq;
using System.Xml.Serialization;

namespace NBi.Core.Report
{
    public enum Target
    {
        Undefined = 0,
        Path,
        Report,
        Parameter
    }
}
