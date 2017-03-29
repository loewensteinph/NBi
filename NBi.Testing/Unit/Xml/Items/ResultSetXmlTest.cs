using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NBi.Xml;
using NUnit.Framework;
using NBi.Core.Rest;
using NBi.Xml.Constraints;
using NBi.Xml.Items.ResultSet;
using System.Data;

namespace NBi.Testing.Unit.Xml.Items
{
    [TestFixture]
    public class ResultSetXmlTest
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
                                           .GetManifestResourceStream("NBi.Testing.Unit.Xml.Resources.ResultSetXmlTestSuite.xml"))
            using (StreamReader reader = new StreamReader(stream))
            {
                manager.Read(reader);
            }
            //manager.ApplyDefaultSettings();
            return manager.TestSuite;
        }

        [Test]
        public void Deserialize_ResultSetWithRowsCell_CorrectlyDeserialized()
        {
            int testNr = 0;

            // Create an instance of the XmlSerializer specifying type and namespace.
            TestSuiteXml ts = DeserializeSample();

            Assert.That(ts.Tests[testNr].Constraints[0], Is.TypeOf<EqualToXml>());
            Assert.That(((EqualToXml)ts.Tests[testNr].Constraints[0]).ResultSet, Is.TypeOf<ResultSetXml>());
            var resultSet = ((EqualToXml)ts.Tests[testNr].Constraints[0]).ResultSet;

            Assert.That(resultSet, Is.Not.Null);
            Assert.That(resultSet.Rows, Has.Count.EqualTo(3));
            Assert.That(resultSet.Rows[0].Cells, Has.Count.EqualTo(2));
            Assert.That(resultSet.Rows[0].Cells[0].Value, Is.EqualTo("1"));
            Assert.That(resultSet.Rows[0].Cells[1].Value, Is.EqualTo("alpha"));
            Assert.That(resultSet.Rows[1].Cells[0].Value, Is.EqualTo("2"));
            Assert.That(resultSet.Rows[1].Cells[1].Value, Is.EqualTo("beta"));
            Assert.That(resultSet.Rows[2].Cells[0].Value, Is.EqualTo("3"));
            Assert.That(resultSet.Rows[2].Cells[1].Value, Is.EqualTo("gamma"));
        }

        [Test]
        public void Deserialize_ResultSetWithSubTable_CorrectlyDeserialized()
        {
            int testNr = 1;

            // Create an instance of the XmlSerializer specifying type and namespace.
            TestSuiteXml ts = DeserializeSample();

            Assert.That(ts.Tests[testNr].Constraints[0], Is.TypeOf<EqualToXml>());
            Assert.That(((EqualToXml)ts.Tests[testNr].Constraints[0]).ResultSet, Is.TypeOf<ResultSetXml>());
            var resultSet = ((EqualToXml)ts.Tests[testNr].Constraints[0]).ResultSet;

            Assert.That(resultSet, Is.Not.Null);
            Assert.That(resultSet.Rows, Has.Count.EqualTo(1));
            Assert.That(resultSet.Rows[0].Cells, Has.Count.EqualTo(5));
            Assert.That(resultSet.Rows[0].Cells[0].Value, Is.EqualTo("2015-06-27"));
            Assert.That(resultSet.Rows[0].Cells[1].Value, Is.EqualTo("ordinary"));
            Assert.That(resultSet.Rows[0].Cells[2].Value, Is.EqualTo("12"));
            Assert.That(resultSet.Rows[0].Cells[3].Rows, Has.Count.EqualTo(2));
            Assert.That(resultSet.Rows[0].Cells[3].Rows[0].Cells[0].Value, Is.EqualTo("(empty)"));
            Assert.That(resultSet.Rows[0].Cells[3].Rows[0].Cells[1].Value, Is.EqualTo("green"));
            Assert.That(resultSet.Rows[0].Cells[3].Rows[1].Cells[0].Value, Is.EqualTo("Saint Cyril of Alexandria, bishop and doctor"));
            Assert.That(resultSet.Rows[0].Cells[3].Rows[1].Cells[1].Value, Is.EqualTo("white"));
            Assert.That(resultSet.Rows[0].Cells[4].Value, Is.EqualTo("saturday"));
        }

        [Test]
        public void Deserialize_ResultSetWithArray_CorrectlyDeserialized()
        {
            int testNr = 2;

            // Create an instance of the XmlSerializer specifying type and namespace.
            TestSuiteXml ts = DeserializeSample();

            Assert.That(ts.Tests[testNr].Constraints[0], Is.TypeOf<EqualToXml>());
            Assert.That(((EqualToXml)ts.Tests[testNr].Constraints[0]).ResultSet, Is.TypeOf<ResultSetXml>());
            var resultSet = ((EqualToXml)ts.Tests[testNr].Constraints[0]).ResultSet;

            Assert.That(resultSet, Is.Not.Null);
            Assert.That(resultSet.Rows, Has.Count.EqualTo(1));
            Assert.That(resultSet.Rows[0].Cells, Has.Count.EqualTo(2));
            Assert.That(resultSet.Rows[0].Cells[0].Value, Is.EqualTo("1"));
            Assert.That(resultSet.Rows[0].Cells[1].Rows, Has.Count.EqualTo(1));
            Assert.That(resultSet.Rows[0].Cells[1].Rows[0].Cells[0].Value, Is.EqualTo("JWDdDeGHry0"));
            Assert.That(resultSet.Rows[0].Cells[1].Rows[0].Cells[1].Rows, Has.Count.EqualTo(1));
            Assert.That(resultSet.Rows[0].Cells[1].Rows[0].Cells[1].Rows[0].Cells[0].Value, Is.EqualTo("Educational Complex Onwards 1995-2008"));
            Assert.That(resultSet.Rows[0].Cells[1].Rows[0].Cells[1].Rows[0].Cells[1].Values, Has.Member("Mike Kelley"));
            Assert.That(resultSet.Rows[0].Cells[1].Rows[0].Cells[1].Rows[0].Cells[1].Values, Has.Member("Anne Pontégnie"));
        }

        [Test]
        public void Deserialize_ResultSetWithArraySingleValue_CorrectlyDeserialized()
        {
            int testNr = 3;

            // Create an instance of the XmlSerializer specifying type and namespace.
            TestSuiteXml ts = DeserializeSample();

            Assert.That(ts.Tests[testNr].Constraints[0], Is.TypeOf<EqualToXml>());
            Assert.That(((EqualToXml)ts.Tests[testNr].Constraints[0]).ResultSet, Is.TypeOf<ResultSetXml>());
            var resultSet = ((EqualToXml)ts.Tests[testNr].Constraints[0]).ResultSet;

            Assert.That(resultSet, Is.Not.Null);
            Assert.That(resultSet.Rows, Has.Count.EqualTo(1));
            Assert.That(resultSet.Rows[0].Cells, Has.Count.EqualTo(2));
            Assert.That(resultSet.Rows[0].Cells[0].Value, Is.EqualTo("1"));
            Assert.That(resultSet.Rows[0].Cells[1].Rows, Has.Count.EqualTo(1));
            Assert.That(resultSet.Rows[0].Cells[1].Rows[0].Cells[0].Value, Is.EqualTo("511pTu1Klc4"));
            Assert.That(resultSet.Rows[0].Cells[1].Rows[0].Cells[1].Rows, Has.Count.EqualTo(1));
            Assert.That(resultSet.Rows[0].Cells[1].Rows[0].Cells[1].Rows[0].Cells[0].Value, Is.EqualTo("Harry Potter 1 and the Philosopher's Stone"));
            Assert.That(resultSet.Rows[0].Cells[1].Rows[0].Cells[1].Rows[0].Cells[1].Values, Has.Member("J. K. Rowling"));
            Assert.That(resultSet.Rows[0].Cells[1].Rows[0].Cells[1].Rows[0].Cells[1].Values.Count(), Is.EqualTo(1));
            Assert.That(resultSet.Rows[0].Cells[1].Rows[0].Cells[1].Rows[0].Cells[1].Value, Is.EqualTo("J. K. Rowling"));
        }

    }
}
