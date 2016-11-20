using System;
using System.Linq;
using NBi.Core.Model;
using NBi.Core.Structure;
using NBi.Core.Report;
using NBi.Xml.Constraints;
using NBi.Xml.Items;
using NBi.Xml.Systems;
using System.Collections.Generic;
using NBi.Xml.Items.Filters;
using NBi.Core.Structure.Relational.PostFilters;

namespace NBi.NUnit.Builder
{
    abstract class AbstractStructureBuilder : AbstractTestCaseBuilder
    {
        protected StructureXml SystemUnderTestXml { get; set; }

        public AbstractStructureBuilder()
        {
        }

        

        protected override void BaseSetup(AbstractSystemUnderTestXml sutXml, AbstractConstraintXml ctrXml)
        {
            if (!(sutXml is StructureXml))
                throw new ArgumentException("System-under-test must be a 'StructureXml'");

            SystemUnderTestXml = (StructureXml)sutXml;
        }

        protected override void BaseBuild()
        {
            SystemUnderTest = InstantiateSystemUnderTest(SystemUnderTestXml);
        }

        protected virtual object InstantiateSystemUnderTest(StructureXml sutXml)
        {
            if (sutXml.Item is DatabaseModelItemXml)
                return InstantiateCommand((DatabaseModelItemXml)sutXml.Item);
            else if (sutXml.Item is ReportingModelItemXml)
                return InstantiateCommand((ReportingModelItemXml)sutXml.Item);
            else 
                throw new ArgumentOutOfRangeException();

            
        }

        protected virtual IModelDiscoveryCommand InstantiateCommand(DatabaseModelItemXml item)
        {
            var provider = new StructureDiscoveryFactoryProvider();
            var factory = provider.Instantiate(item.GetConnectionString());

            var modelFilterBuilder = new FilterObjectDatabaseModelBuilder();
            modelFilterBuilder.Setup(item);
            modelFilterBuilder.Build();

            var target = modelFilterBuilder.GetTarget();
            var filters = modelFilterBuilder.GetFilters();

            var command = factory.Instantiate(target, Core.Structure.TargetType.Object, filters);
            return command;
        }


        protected virtual IModelDiscoveryCommand InstantiateCommand(ReportingModelItemXml item)
        {
            var provider = new ReportingModelDiscoveryFactoryProvider();
            var factory = provider.Instantiate(item.ConnectionString);

            var modelFilterBuilder = new FilterObjectReportModelBuilder();
            modelFilterBuilder.Setup(item);
            modelFilterBuilder.Build();

            var target = modelFilterBuilder.GetTarget();
            var filters = modelFilterBuilder.GetFilters();

            var command = factory.Instantiate(target, Core.Report.TargetType.Object, filters);
            return command;
        }

    }
}
