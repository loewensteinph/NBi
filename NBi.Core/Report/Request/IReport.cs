using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBi.Core.Report.Request
{
    public interface IReport
    {
        string Source { get; }
        string Path { get; }
        string Name { get; }
        string Version { get; }
    }
}
