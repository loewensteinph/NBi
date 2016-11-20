using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.Model;

namespace NBi.Core.Report
{
    public class CommandDescription : ICommandDescription
    {
        protected readonly Target target;
        protected readonly IEnumerable<IFilter> filters;

        public Enum Target
        {
            get { return target; }
        }

        public IEnumerable<IFilter> Filters
        {
            get { return filters; }
        }

        public CommandDescription(Target target, IEnumerable<IFilter> filters)
        {
            this.target = target;
            this.filters = filters;
        }

        public virtual IEnumerable<ICaptionFilter> GetCaptionNonTargetFilter()
        {
            return filters.Where(f => f is ICaptionFilter).Cast<ICaptionFilter>().Where(f => f.UntypedTarget.CompareTo(target)!=0);
        }
    }
}
