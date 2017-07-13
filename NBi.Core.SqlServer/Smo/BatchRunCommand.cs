using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.Batch;
using NBi.Core.Query;

namespace NBi.Core.SqlServer.Smo
{
    class BatchRunCommand : IDecorationCommandImplementation
    {
        private readonly string connectionString;
        private readonly string fullPath;
        private readonly string query;
        private IEnumerable<IQueryTemplateVariable> Variables;

        public BatchRunCommand(IBatchRunCommand command, SqlConnection connection)
        {
            this.connectionString = connection.ConnectionString;
            this.fullPath = command.FullPath;
            this.query = command.InlineQuery;
            this.Variables = command.Variables;
        }       

        public void Execute()
        {
            IEnumerable<IQueryTemplateVariable> variables = null;

            if (Variables != null && Variables.Any())
            {
                variables = Variables;
            }

            var commandBuilder = new CommandBuilder();
            var cmd = commandBuilder.Build(connectionString, query, variables);

            if (!File.Exists(fullPath) && cmd == null)
                throw new ExternalDependencyNotFoundException(fullPath);

            var script = string.Empty;
            if (File.Exists(fullPath))
            {
                script = File.ReadAllText(fullPath);
                Trace.WriteLineIf(NBiTraceSwitch.TraceVerbose, script);
            }
            if (!File.Exists(fullPath) && cmd != null)
            {
                script = cmd.CommandText;
                Trace.WriteLineIf(NBiTraceSwitch.TraceVerbose, script);
            }

            var server = new Server();
            server.ConnectionContext.ConnectionString = connectionString;
            server.ConnectionContext.ExecuteNonQuery(script);
        }
    }
}
