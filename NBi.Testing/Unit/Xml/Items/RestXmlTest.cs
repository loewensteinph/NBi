using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NBi.Xml;
using NBi.Xml.Items;
using NBi.Xml.Systems;
using NUnit.Framework;
using NBi.Xml.Items.Rest;
using NBi.Core.Rest;

namespace NBi.Testing.Unit.Xml.Items
{
    [TestFixture]
    public class RestXmlTest
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

        protected TestSuiteXml DeserializeSample()
        {
            // Declare an object variable of the type to be deserialized.
            var manager = new XmlManager();

            // A Stream is needed to read the XML document.
            using (Stream stream = Assembly.GetExecutingAssembly()
                                           .GetManifestResourceStream("NBi.Testing.Unit.Xml.Resources.RestXmlTestSuite.xml"))
            using (StreamReader reader = new StreamReader(stream))
            {
                manager.Read(reader);
            }
            //manager.ApplyDefaultSettings();
            return manager.TestSuite;
        }

        [Test]
        public void Deserialize_RestApiWithAttributesSpecified_CorrectlyDeserialized()
        {
            int testNr = 0;

            // Create an instance of the XmlSerializer specifying type and namespace.
            TestSuiteXml ts = DeserializeSample();

            Assert.That(ts.Tests[testNr].Systems[0], Is.TypeOf<ExecutionXml>());
            Assert.That(((ExecutionXml)ts.Tests[testNr].Systems[0]).BaseItem, Is.TypeOf<RestXml>());
            var rest = (RestXml)((ExecutionXml)ts.Tests[testNr].Systems[0]).BaseItem;

            Assert.That(rest, Is.Not.Null);
            Assert.That(rest.Path, Is.EqualTo("calendars/default/2015/6/27"));
            Assert.That(rest.Location.BaseAddress, Is.EqualTo(@"http://calapi.inadiutorium.cz/api/v0/en/"));
            Assert.That(rest.Location.ContentType, Is.EqualTo(ContentType.Json));
            Assert.That(rest.Credentials.Type, Is.EqualTo(CredentialsType.Anonymous));
        }

        [Test]
        public void Deserialize_RestApiWithDefaults_CorrectlyDeserialized()
        {
            int testNr = 1;

            // Create an instance of the XmlSerializer specifying type and namespace.
            TestSuiteXml ts = DeserializeSample();

            Assert.That(ts.Tests[testNr].Systems[0], Is.TypeOf<ExecutionXml>());
            Assert.That(((ExecutionXml)ts.Tests[testNr].Systems[0]).BaseItem, Is.TypeOf<RestXml>());
            var rest = (RestXml)((ExecutionXml)ts.Tests[testNr].Systems[0]).BaseItem;

            Assert.That(rest, Is.Not.Null);
            Assert.That(rest.Path, Is.EqualTo(@"http://calapi.inadiutorium.cz/api/v0/en/calendars/default/2015/6/27"));
            Assert.That(rest.Location.BaseAddress, Is.Null.Or.Empty);
            Assert.That(rest.Location.ContentType, Is.EqualTo(ContentType.Json));
            Assert.That(rest.Credentials.Type, Is.EqualTo(CredentialsType.Anonymous));
        }

        [Test]
        public void Deserialize_RestApiWithParameters_CorrectlyDeserialized()
        {
            int testNr = 2;

            // Create an instance of the XmlSerializer specifying type and namespace.
            TestSuiteXml ts = DeserializeSample();

            Assert.That(ts.Tests[testNr].Systems[0], Is.TypeOf<ExecutionXml>());
            Assert.That(((ExecutionXml)ts.Tests[testNr].Systems[0]).BaseItem, Is.TypeOf<RestXml>());
            var rest = (RestXml)((ExecutionXml)ts.Tests[testNr].Systems[0]).BaseItem;

            Assert.That(rest, Is.Not.Null);
            Assert.That(rest.Parameters, Is.Not.Null);
            Assert.That(rest.Parameters, Has.Count.EqualTo(4));
            Assert.That(List.Map(rest.Parameters).Property("Name"), Is.EqualTo(new[] {"calendar", "year", "month", "day" }));
            Assert.That(List.Map(rest.Parameters).Property("StringValue"), Is.EqualTo(new[] { "default", "2015", "6", "27" }));
        }
    }
}
