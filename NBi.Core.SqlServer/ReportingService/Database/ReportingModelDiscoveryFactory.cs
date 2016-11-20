using NBi.Core.SqlServer.ReportingService;
using NBi.Core.SqlServer.ReportingService.Database.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.Report;
using NBi.Core.Model;

namespace NBi.Core.SqlServer.ReportingService.Database
{
    class ReportingModelDiscoveryFactory : IReportingModelDiscoveryFactory
    {
        private readonly IDbConnection connection;
        public ReportingModelDiscoveryFactory(IDbConnection connection)
        {
            this.connection = connection as IDbConnection;
        }

        public IModelDiscoveryCommand Instantiate(Target target, TargetType type, IEnumerable<IFilter> filters)
        {
            var builder = InstantiateBuilder(target);
            builder.Build(filters);

            var cmd = connection.CreateCommand();
            cmd.CommandText = builder.GetCommandText();
            var postFilters = builder.GetPostFilters();

            var description = new CommandDescription(target, filters);

            ReportingModelCommand command = null;
            command = new ReportingModelCommand(cmd, postFilters, description);

            return command;
        }


        protected virtual IDiscoveryCommandBuilder InstantiateBuilder(Target target)
        {
            switch (target)
            {
                case Target.Parameter:
                    return new ParameterDiscoveryCommandBuilder();
                default:
                    throw new ArgumentOutOfRangeException(string.Format("The value '{0}' is not supported when instantiating with 'RelationalStructureDiscoveryFactory'.", target));
            }
        }
        
    }
}
