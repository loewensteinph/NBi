using System;
using System.Linq;
using Moq;
using NBi.Core.Etl;
using NBi.Core.SqlServer.IntegrationService;
using NUnit.Framework;
using NBi.Core.Report;
using NBi.Core.Report.Request;
using NBi.Core.SqlServer.ReportingService;

namespace NBi.Testing.Unit.Core.SqlServer.IntegrationService
{
    [TestFixture]
    public class SsrsReportParserFactoryTest
    {
        #region SetUp & TearDown
        //Called only at instance creation
        [TestFixtureSetUp]
        public void SetupMethods()
        {

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
        }

        //Called after each test
        [TearDown]
        public void TearDownTest()
        {
        }
        #endregion

        [Test]
        public void Get_FileRequest_FileParser()
        {
            var request = Mock.Of<IReport>(r => r.Path=="C:\\Etl" && r.Name=="mySample.rdl" && r.Source==string.Empty);
            
            var factory = new SsrsReportParserFactory();

            Assert.IsInstanceOf<FileParser>(factory.Get(request));
        }

        [Test]
        public void Get_FileRequestWithoutExtension_FileParser()
        {
            var request = Mock.Of<IReport>(r => r.Path == "C:\\Etl" && r.Name == "mySample" && r.Source == string.Empty);

            var factory = new SsrsReportParserFactory();
            var parser = factory.Get(request);

            Assert.That(parser.ReportName == "mySample.rdl");
        }


        [Test]
        public void Get_DatabaseRequest_DatabaseParser()
        {
            var request = Mock.Of<IReport>(r => r.Path == "/Etl" && r.Name == "mySample" && r.Source == "database=myDB;Server=myServer;Integrated Security=SSPI");

            var factory = new SsrsReportParserFactory();

            Assert.IsInstanceOf<DatabaseParser>(factory.Get(request));
        }
        
    }
}
