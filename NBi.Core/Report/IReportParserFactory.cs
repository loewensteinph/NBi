using NBi.Core.Report.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Report
{
    public interface IReportParserFactory
    {
        IReportParser Get(IReport request);
    }
}
