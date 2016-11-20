using System;
using System.Collections.Generic;
using System.Linq;
using NBi.Core.Report;

namespace NBi.NUnit.Structure
{
    internal class DescriptionReportModelHelper : IDescriptionModelHelper
    {
        private readonly IEnumerable<CaptionFilter> filters;
        private readonly Target target;

        public DescriptionReportModelHelper(IEnumerable<CaptionFilter> filters, Target target)
        {
            this.filters = filters;
            this.target = target;
        }

        public string GetFilterExpression()
        {
            var texts = new List<string>();
            foreach (var filter in filters)
            {
                var text = string.Empty;
                switch (filter.Target)
                {
                    case Target.Path:
                        text = "in path '{0}'";
                        break;
                    case Target.Report:
                        text = "in report '{0}'";
                        break;
                    default:
                        break;
                }
                if (text.Length > 0)
                {
                    text = string.Format(text, filter.Caption);
                    texts.Add(text);
                }
            }
            texts.Reverse();
            return string.Join(", ", texts.ToArray());
        }
        
        public string GetTargetExpression()
        {
            var text = string.Empty;
            switch (target)
            {
                case Target.Path:
                    text = "path";
                    break;
                case Target.Report:
                    text = "report";
                    break;
                case Target.Parameter:
                    text = "parameter";
                    break;
                default:
                    break;
            }

            return text;
        }
        
        public string GetTargetPluralExpression()
        {
            var text = string.Empty;
            switch (target)
            {
                case Target.Paths:
                    text = "paths";
                    break;
                case Target.Reports:
                    text = "reports";
                    break;
                case Target.Parameters:
                    text = "parameters";
                    break;
                default:
                    break;
            }

            return text;
        }

        public string GetNextTargetExpression()
        {
            var text = string.Empty;
            switch (target)
            {
                case Target.Paths:
                    text = "report";
                    break;
                case Target.Reports:
                    text = "parameters";
                    break;
                default:
                    break;
            }

            return text;
        }
        

        public string GetNextTargetPluralExpression()
        {
            var text = string.Empty;
            switch (target)
            {
                case Target.Paths:
                    text = "reports";
                    break;
                case Target.Reports:
                    text = "parameters";
                    break;
                default:
                    break;
            }

            return text;
        }

        public string GetNotExpression(bool not)
        {
            if (!not)
                return "not";
            else
                return "a";
        }
    }
}
