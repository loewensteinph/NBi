using NBi.Core.Report;
using NBi.Core.SqlServer.ReportingService;
using NBi.Core.SqlServer.ReportingService.Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NBi.Testing.Unit.Core.ReportingModel
{
    public class ReportingModelDiscoveryFactoryProviderTest
    {
        private class FakeReportingModelDiscoveryFactoryProvider : ReportingModelDiscoveryFactoryProvider
        {
            private readonly string result;

            public FakeReportingModelDiscoveryFactoryProvider(string result)
                : base()
            {
                this.result = result;
            }
        }

        [Test]
        public void Instantiate_SqlConnection_GetDatabaseReportingModelDiscoveryFactory()
        {
            var connectionString = ConnectionStringReader.GetSqlClient();
            
            var provider = new ReportingModelDiscoveryFactoryProvider();
            var factory = provider.Instantiate(connectionString);
            Assert.That(factory, Is.TypeOf<ReportingModelDiscoveryFactory>());
        }
        
    }
}
