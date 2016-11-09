using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NBi.Core.Query;
using NBi.Xml.Constraints;
using NBi.Xml.Items;
using NBi.Xml.Systems;

namespace NBi.NUnit.Builder
{
    abstract class AbstractExecutionBuilder : AbstractTestCaseBuilder
    {
        protected ExecutionXml SystemUnderTestXml { get; set; }

        protected override void BaseSetup(AbstractSystemUnderTestXml sutXml, AbstractConstraintXml ctrXml)
        {
            if (!(sutXml is ExecutionXml))
                throw new ArgumentException("System-under-test must be a 'ExecutionXml'");

            SystemUnderTestXml = (ExecutionXml)sutXml;
        }

        protected override void BaseBuild()
        {
            SystemUnderTest = InstantiateSystemUnderTest(SystemUnderTestXml);
        }

        protected virtual IDbCommand InstantiateSystemUnderTest(ExecutionXml executionXml)
        {
            var commandBuilder = new CommandBuilder();

            var item = executionXml.Item as QueryableXml;
            if (item == null)
                throw new ArgumentException();

            var connectionString = item.GetConnectionString();
            var commandText = item.GetQuery();

            IEnumerable<IQueryParameter> parameters=null;
            IEnumerable<IQueryTemplateVariable> variables = null;
            if (executionXml.Item is QueryXml)
            { 
                parameters = ((QueryXml)executionXml.Item).GetParameters();
                variables = ((QueryXml)executionXml.Item).GetVariables();
            }
            if (executionXml.Item is ReportXml)
            {
                parameters = ((ReportXml)executionXml.Item).GetParameters();
            }
            var cmd = commandBuilder.Build(connectionString, commandText, parameters, variables);
            cmd.CommandTimeout = (int) Math.Ceiling((executionXml.Item as QueryableXml).Timeout/1000.0);

            if (executionXml.Item is ReportXml)
            {
                cmd.CommandType = ((ReportXml)executionXml.Item).GetCommandType();
            }

            return cmd;
        }


    }
}
