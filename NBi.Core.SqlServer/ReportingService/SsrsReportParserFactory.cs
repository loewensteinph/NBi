using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBi.Core.Etl;
using NBi.Core.Report;
using NBi.Core.Report.Request;

namespace NBi.Core.SqlServer.ReportingService
{
    public class SsrsReportParserFactory : IReportParserFactory
    {
        public IReportParser Get(IReport request)
        {
            if (string.IsNullOrWhiteSpace(request.Source))
                return new DiskFile.QueryFileParser(request.Path, request.Name.EndsWith(".rdl") ? request.Name : request.Name + ".rdl");
            else
                return new Database.QueryDatabaseParser(request.Source, request.Path, request.Name);
        }
    }
}
