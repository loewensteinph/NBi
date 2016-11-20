using NBi.Core.Structure;
using NBi.Core.Structure.Relational.PostFilters;
using NBi.Xml.Items;
using NBi.Xml.Items.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.Model;

namespace NBi.NUnit.Builder
{
    class FilterRelationDatabaseModelBuilder : FilterObjectDatabaseModelBuilder
    {

        protected override Target BuildTarget()
        {

            if (item is MeasureGroupXml)
                return Target.Dimensions;
            if (item is DimensionXml)
                return Target.MeasureGroups;
            else
                throw new ArgumentException(item.GetType().Name);
        }

        protected override IEnumerable<IFilter> BuildFilters()
        {
            if (item is IPerspectiveFilter)
                yield return new CaptionFilter(Target.Perspectives, ((IPerspectiveFilter)item).Perspective);

            var itselfTarget = Target.Dimensions;
            if (item is MeasureGroupXml)
                itselfTarget = Target.MeasureGroups;

            yield return new CaptionFilter(itselfTarget, ((IModelSingleItemXml)item).Caption);
        }
    }
}
