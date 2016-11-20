using NBi.Core.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.SqlServer.ReportingService.Database.PostFilters;

namespace NBi.Core.SqlServer.ReportingService.Database.Builders
{
    class ParameterDiscoveryCommandBuilder : ReportingModelDiscoveryCommandBuilder
    {
        protected override string BuildCommandText()
        {
            return ReadQueryFromResource("ListParameter");
        }

        protected override IEnumerable<ICommandFilter> BuildCaptionFilters(IEnumerable<CaptionFilter> filters)
        {
           
            yield return new CommandFilter(string.Format("left([path],LEN([path]) - charindex('/',reverse([path]),1))='{0}'"
                                                            , filters.Single(f => f.Target == Target.Path).Caption
                                                            ));
            yield return new CommandFilter(string.Format("[name]='{0}'"
                                                            , filters.Single(f => f.Target == Target.Report).Caption
                                                            ));

            var filter = filters.SingleOrDefault(f => f.Target == Target.Parameter);
            if (filter != null)
                yield return new ParameterCaptionFilter(filter.Caption);
        }

    }
}
