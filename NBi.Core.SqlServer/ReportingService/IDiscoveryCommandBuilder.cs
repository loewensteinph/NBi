using NBi.Core.SqlServer.ReportingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.Model;

namespace NBi.Core.SqlServer.ReportingService
{
    public interface IDiscoveryCommandBuilder
    {
        void Build(IEnumerable<IFilter> filters);
        string GetCommandText();
        IEnumerable<IPostCommandFilter> GetPostFilters();
    }
}
