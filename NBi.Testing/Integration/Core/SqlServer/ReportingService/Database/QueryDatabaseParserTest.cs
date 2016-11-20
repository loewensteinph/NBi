using System;
using System.Diagnostics;
using System.Linq;
using NBi.Core.Report;
using NUnit.Framework;
using System.Data;
using NBi.Core.Report.Request;
using NBi.Core.SqlServer.ReportingService.Database;

namespace NBi.Testing.Integration.Core.SqlServer.ReportingService
{
    [TestFixture]
    [Category("ReportServerDB")]
    public class QueryDatabaseParserTest
    {

        private static bool isSqlServerStarted = false;

        #region SetUp & TearDown
        //Called only at instance creation
        [TestFixtureSetUp]
        public void SetupMethods()
        {
            isSqlServerStarted = CheckIfSqlServerStarted();
        }

        //Called only at instance destruction
        [TestFixtureTearDown]
        public void TearDownMethods()
        {
        }

        //Called before each test
        [SetUp]
        public void SetupTest()
        {
            if (!isSqlServerStarted)
                Assert.Ignore("SQL Server not started.");
        }

        //Called after each test
        [TearDown]
        public void TearDownTest()
        {
        }

        private bool CheckIfSqlServerStarted()
        {
            var pname = System.Diagnostics.Process.GetProcesses().Where(p => p.ProcessName.Contains("sqlservr"));
            return pname.Count() > 0;
        }
        #endregion

        [Test]
        public void ExtractQuery_ExistingReportAndDataSet_CorrectQueryReturned()
        {
            var parser = new QueryDatabaseParser(ConnectionStringReader.GetReportServerDatabase(), "/AdventureWorks Sample Reports/", "Currency_List");
            var query = parser.ExtractQuery("Currency");

            Assert.That(query.Text, 
                Is.StringContaining("SELECT").And
                .StringContaining("[CurrencyAlternateKey]").And
                .StringContaining("[DimCurrency]"));
            Assert.That(query.CommandType, Is.EqualTo(CommandType.Text));
        }

        [Test]
        public void ExtractQuery_NonExistingDataSetOneExisting_CorrectExceptionReturned()
        {
            var parser = new QueryDatabaseParser(ConnectionStringReader.GetReportServerDatabase(), "/AdventureWorks Sample Reports/", "Currency_List");
            var ex = Assert.Throws<ArgumentException>(()=> parser.ExtractQuery("Non Existing"));
            Assert.That(ex.Message, Is.StringContaining("'Currency_List'"));
        }

        [Test]
        public void ExtractQuery_NonExistingDataSetMoreThanOneExisting_CorrectExceptionReturned()
        {
            var parser = new QueryDatabaseParser(ConnectionStringReader.GetReportServerDatabase(), "/AdventureWorks Sample Reports/", "Currency_List");
            var ex = Assert.Throws<ArgumentException>(() => parser.ExtractQuery("Non Existing"));
            Assert.That(ex.Message, Is.StringContaining("DataSet1").And.StringContaining("DataSet2"));
        }

        [Test]
        public void ExtractQuery_NonExistingReport_CorrectExceptionReturned()
        {
            var parser = new QueryDatabaseParser(ConnectionStringReader.GetReportServerDatabase(), "/AdventureWorks Sample Reports/", "Non Existing");
            var ex = Assert.Throws<ArgumentException>(() => parser.ExtractQuery("DataSet1"));

            Assert.That(ex.Message, Is.StringContaining("No report found"));
        }

        public void ExtractQuery_SharedDataSet_CorrectQuery()
        {
            var parser = new QueryDatabaseParser(ConnectionStringReader.GetReportServerDatabase(), "/AdventureWorks Sample Reports/", "Employee_Sales_Summary");
            var query = parser.ExtractQuery("SalesEmployees2008R2");

            Assert.That(query.Text,
                Is.StringContaining("SELECT"));
            Assert.That(query.CommandType, Is.EqualTo(CommandType.Text));

        }

        public void ExtractQuery_NonExistingSharedDataSet_CorrectQuery()
        {
            var parser = new QueryDatabaseParser(ConnectionStringReader.GetReportServerDatabase(), "/AdventureWorks Sample Reports/", "Employee_Sales_Summary");
            var ex = Assert.Throws<ArgumentException>(() => parser.ExtractQuery("Non Existing"));
            Assert.That(ex.Message, Is.StringContaining("Quota").And.StringContaining("2008R2"));
        }
    }
}
