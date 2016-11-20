using System;
using System.Linq;
using NBi.Core.DataType;
using NBi.Xml.Constraints;
using NBi.Xml.Items;
using NBi.Xml.Systems;
using System.Collections.Generic;
using NBi.Xml.Items.Filters;

namespace NBi.NUnit.Builder
{
    abstract class AbstractDataTypeBuilder : AbstractTestCaseBuilder
    {
        protected DataTypeXml SystemUnderTestXml { get; set; }
        protected readonly DataTypeDiscoveryFactoryProvider discoveryProvider;

        public AbstractDataTypeBuilder()
        {
            discoveryProvider = new DataTypeDiscoveryFactoryProvider();
        }

        internal AbstractDataTypeBuilder(DataTypeDiscoveryFactoryProvider discoveryProvider)
        {
            this.discoveryProvider = discoveryProvider;
        }

        protected override void BaseSetup(AbstractSystemUnderTestXml sutXml, AbstractConstraintXml ctrXml)
        {
            if (!(sutXml is DataTypeXml))
                throw new ArgumentException("System-under-test must be a 'DataTypeXml'");

            SystemUnderTestXml = (DataTypeXml)sutXml;
        }

        protected override void BaseBuild()
        {
            SystemUnderTest = InstantiateSystemUnderTest(SystemUnderTestXml);
        }

        protected virtual object InstantiateSystemUnderTest(DataTypeXml sutXml)
        {
            if (sutXml.Item is DatabaseModelItemXml)
                return InstantiateCommand((DatabaseModelItemXml)sutXml.Item);
            else
                throw new ArgumentException();
        }

        protected virtual IDataTypeDiscoveryCommand InstantiateCommand(DatabaseModelItemXml item)
        {
            var factory = discoveryProvider.Instantiate(item.GetConnectionString());

            var target = BuildTarget(item);
            var filters = BuildFilters(item);

            var command = factory.Instantiate(target, filters);
            return command;
        }

        protected virtual IEnumerable<CaptionFilter> BuildFilters(DatabaseModelItemXml item)
        {
            if (item is IPerspectiveFilter)
                yield return new CaptionFilter(Target.Perspectives, ((IPerspectiveFilter)item).Perspective);
            if (item is IDimensionFilter)
                yield return new CaptionFilter(Target.Dimensions, ((IDimensionFilter)item).Dimension);
            if (item is IHierarchyFilter)
                yield return new CaptionFilter(Target.Hierarchies, ((IHierarchyFilter)item).Hierarchy);
            if (item is ILevelFilter)
                yield return new CaptionFilter(Target.Levels, ((ILevelFilter)item).Level);
            if (item is IMeasureGroupFilter && !(string.IsNullOrEmpty(((IMeasureGroupFilter)item).MeasureGroup)))
                yield return new CaptionFilter(Target.MeasureGroups, ((IMeasureGroupFilter)item).MeasureGroup);
            //if (item is ISchemaFilter)
            //    yield return new CaptionFilter(Target.Schemas, ((ISchemaFilter)item).Schema);
            if (item is ITableFilter)
                yield return new CaptionFilter(Target.Tables, ((ITableFilter)item).Table);

            if (item is IModelSingleItemXml)
            {
                var itselfTarget = BuildTarget(item);
                yield return new CaptionFilter(itselfTarget, ((IModelSingleItemXml)item).Caption);
            }
        }

        protected virtual Target BuildTarget(ModelItemXml item)
        {
            if (item is MeasureXml)
                return Target.Measures;
            if (item is ColumnXml)
                return Target.Columns;
            if (item is PropertyXml)
                return Target.Properties;
           
            throw new ArgumentException(item.GetType().Name);
        }

    }
}
