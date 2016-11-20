using NBi.Core.Structure.Olap;
using NBi.Core.Structure.Relational;
using NBi.Core.Structure.Tabular;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Reflection;

namespace NBi.Core.Report
{
    public class ReportingModelDiscoveryFactoryProvider
    {

        public ReportingModelDiscoveryFactoryProvider()
        {

        }

        public IReportingModelDiscoveryFactory Instantiate(string connectionString)
        {
            var directory = AssemblyDirectory;
            var filename = string.Format("NBi.Core.{0}.dll", "SqlServer2014");
            var filepath = string.Format("{0}\\{1}", directory, filename);
            if (!File.Exists(filepath))
                throw new InvalidOperationException(string.Format("Can't find the dll for version '{0}' in '{1}'. NBi was expecting to find a dll named '{2}'.", "SqlServer2014", directory, filename));

            var interfaceName = typeof(IReportingModelDiscoveryFactory).Name;

            var assembly = Assembly.LoadFrom(filepath);
            var types = assembly.GetTypes()
                            .Where(m => m.IsClass && m.GetInterface(interfaceName) != null);

            if (types.Count() == 0)
                throw new InvalidOperationException(string.Format("Can't find a class implementing '{0}' in '{1}'.", interfaceName, assembly.FullName));
            if (types.Count() > 1)
                throw new InvalidOperationException(string.Format("Found more than one class implementing '{0}' in '{1}'.", interfaceName, assembly.FullName));

            var connectionFactory = new ConnectionFactory();
            var connection = connectionFactory.Get(connectionString);

            var factoryType = types.ElementAt(0);
            var ctor = factoryType.GetConstructor(new Type[] { typeof(IDbConnection) });
            var factory = (IReportingModelDiscoveryFactory)ctor.Invoke(new object[] { connection });

            return factory;
        }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
