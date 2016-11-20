using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.SqlServer.ReportingService.Database.PostFilters
{
    class ParameterCaptionFilter : ICommandFilter, IValueFilter, IPostCommandFilter
    {
        private string value;
        public string Value { get { return value; } }

        public ParameterCaptionFilter(string caption)
        {
            this.value = caption;
        }

        public bool Evaluate(object row)
        {
            if (row is ReportingModelRow)
                return Evaluate((ReportingModelRow)row);

            throw new ArgumentException();
        }

        protected bool Evaluate(ReportingModelRow row)
        {
            return row.Caption==Value;
        }
    }
}
