using NBi.Xml.Items;
using NBi.Xml.Items.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.Model;
using NBi.Core.Report;

namespace NBi.NUnit.Builder
{
    class FilterObjectReportModelBuilder
    {
        protected bool isSetup;
        protected bool isBuild;
        protected ReportingModelItemXml item;
        private IEnumerable<IFilter> filters;
        private Target target;

        public void Setup(ReportingModelItemXml model)
        {
            isBuild = false;
            this.item = model;
            isSetup = true;
        }

        public void Build()
        {
            if (!isSetup)
                throw new InvalidOperationException();

            target = BuildTarget();
            filters = BuildFilters();

            isBuild = true;
        }

        public IEnumerable<IFilter> GetFilters()
        {
            return filters;
        }

        public Target GetTarget()
        {
            return target;
        }

        protected virtual IEnumerable<IFilter> BuildFilters()
        {
            if (item is ReportParameterXml)
            {
                yield return new CaptionFilter(Target.Path, ((ReportParameterXml)item).Path);
                yield return new CaptionFilter(Target.Report, ((ReportParameterXml)item).Report);
                yield return new CaptionFilter(Target.Parameter, ((ReportParameterXml)item).Caption);
            }
            else if (item is ReportParametersXml)
            {
                yield return new CaptionFilter(Target.Path, ((ReportParametersXml)item).Path);
                yield return new CaptionFilter(Target.Report, ((ReportParametersXml)item).Report);
            }
        }

        protected virtual Target BuildTarget()
        {
            if (!isSetup)
                throw new InvalidOperationException();

            if (item is ReportParameterXml || item is ReportParametersXml)
                return Target.Parameter;
            else
                throw new ArgumentException(item.GetType().Name);
        }
    }
}
