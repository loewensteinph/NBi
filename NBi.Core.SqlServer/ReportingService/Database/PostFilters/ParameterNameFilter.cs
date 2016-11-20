using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.SqlServer.ReportingService.Database.PostFilters
{
    public class ParameterNameFilter : ICommandFilter, IValueFilter
    {
        private string value;
        public string Value { get { return value; } }

        public ParameterNameFilter(string caption)
        {
            this.value = caption;
        }
    }
}
