using NBi.Core.Report.Request;
using NBi.Core.Report.Result;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NBi.Core.Report
{
    public interface IReportParser
    {
        ReportCommand ExtractQuery(string dataSetName);
        IEnumerable<ReportParameter> ExtractParameters();

        string ReportName { get; }
    }
}
