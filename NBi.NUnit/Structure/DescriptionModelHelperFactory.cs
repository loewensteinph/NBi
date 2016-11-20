using NBi.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.NUnit.Structure
{
    class DescriptionModelHelperFactory
    {
        public IDescriptionModelHelper Get(IEnumerable<ICaptionFilter> filters, Enum target)
        {
            if (filters.All(f => f is Core.Report.CaptionFilter) && target is Core.Report.Target)
                return new DescriptionReportModelHelper(filters.Cast<Core.Report.CaptionFilter>(), (Core.Report.Target)target);
            else if (filters.All(f => f is Core.Structure.CaptionFilter))
                return new DescriptionDatabaseModelHelper(filters.Cast<Core.Structure.CaptionFilter>(), (Core.Structure.Target)target);
            else
                throw new ArgumentOutOfRangeException();
        }
    }
}
