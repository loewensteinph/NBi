using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.Report;

namespace NBi.Core.SqlServer.ReportingService.Database
{

    class ReportingModelCommand : ReportingModelDiscoveryCommand
    {
        public ReportingModelCommand(IDbCommand command, IEnumerable<IPostCommandFilter> postFilters, CommandDescription description)
            : base(command, postFilters, description)
        {
        }

        public override IEnumerable<string> Execute()
        {
            var values = new List<ReportingModelRow>();

            command.Connection.Open();
            var rdr = ExecuteReader(command);
            while (rdr.Read())
            {
                var row = BuildRow(rdr);
                var isValidRow = true;

                foreach (var postFilter in postFilters)
                    isValidRow &= postFilter.Evaluate(row);

                if (isValidRow)
                    values.Add(row);
            }
            command.Connection.Close();

            return values.Select(v => v.Caption);
        }

        protected virtual ReportingModelRow BuildRow(IDataReader rdr)
        {
            var row = new ReportingModelRow();
            row.Caption = rdr.GetString(0);
            return row;
        }

        protected IDataReader ExecuteReader(IDbCommand cmd)
        {
            Trace.WriteLineIf(NBiTraceSwitch.TraceInfo, cmd.CommandText);

            IDataReader rdr = null;
            try
            {
                rdr = cmd.ExecuteReader();
                return rdr;
            }
            catch (Exception ex)
            { throw new ConnectionException(ex, cmd.Connection.ConnectionString); }
        }
    }
}
