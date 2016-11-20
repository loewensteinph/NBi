using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.Model;

namespace NBi.Core.Report
{
    public interface IReportingModelDiscoveryFactory
    {
        IModelDiscoveryCommand Instantiate(Target target, TargetType type, IEnumerable<IFilter> filters);
    }
}
