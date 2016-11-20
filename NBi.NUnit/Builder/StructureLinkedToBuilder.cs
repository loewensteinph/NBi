using System;
using System.Linq;
using NBi.Core.Model;
using NBi.NUnit.Structure;
using NBi.Xml.Constraints;
using NBi.Xml.Items;
using NBi.Xml.Systems;
using NBi.Core.Structure;
using System.Collections.Generic;
using NBi.Xml.Items.Filters;

namespace NBi.NUnit.Builder
{
    class StructureLinkedToBuilder : AbstractStructureBuilder
    {
        protected LinkedToXml ConstraintXml { get; set; }

        public StructureLinkedToBuilder() : base()
        {
        }

        

        protected override void SpecificSetup(AbstractSystemUnderTestXml sutXml, AbstractConstraintXml ctrXml)
        {
            if (!(ctrXml is LinkedToXml))
                throw new ArgumentException("Constraint must be a 'LinkedToXml'");

            ConstraintXml = (LinkedToXml)ctrXml;
        }

        protected override void SpecificBuild()
        {
            Constraint = InstantiateConstraint(ConstraintXml);
        }

        protected NBiConstraint InstantiateConstraint(LinkedToXml ctrXml)
        {
            if (!(ctrXml.Item is IModelSingleItemXml && ctrXml.Item is DatabaseModelItemXml))
                throw new ArgumentOutOfRangeException();

            var ctr = new LinkedToConstraint(((IModelSingleItemXml)ctrXml.Item).Caption);
            return ctr;
        }

        protected override IModelDiscoveryCommand InstantiateCommand(DatabaseModelItemXml item)
        {
            var provider = new StructureDiscoveryFactoryProvider();
            var factory = provider.Instantiate(item.GetConnectionString());

            var modelFilterBuilder = new FilterRelationDatabaseModelBuilder();
            modelFilterBuilder.Setup(item);
            modelFilterBuilder.Build();

            var target = modelFilterBuilder.GetTarget();
            var filters = modelFilterBuilder.GetFilters();

            var command = factory.Instantiate(target, TargetType.Relation, filters);
            return command;
        }


    }
}
