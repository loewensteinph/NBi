using System.IO;
using System.Reflection;
using NBi.Xml;
using NBi.Xml.Items;
using NBi.Xml.Systems;
using NUnit.Framework;
using NBi.Xml.Constraints;

namespace NBi.Testing.Unit.Xml.Items.ResultSet
{
    [TestFixture]
    public class ResultSetXmlTest
    {
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
            return manager.TestSuite;
        }
        
        [Test]
        public void Deserialize_ResultSet_NotEmptyOtherEmpty()
        {
            int testNr = 0;
            
            // Create an instance of the XmlSerializer specifying type and namespace.
            TestSuiteXml ts = DeserializeSample();

            // Check the properties of the object.
            Assert.That(ts.Tests[testNr].Constraints[0], Is.TypeOf<EqualToXml>());
            var equalTo = ts.Tests[testNr].Constraints[0] as EqualToXml;

            Assert.That(equalTo.ResultSet, Is.Not.Null);
            Assert.That(equalTo.Query, Is.Null.Or.Empty);
            Assert.That(equalTo.XmlSource, Is.Null.Or.Empty);
        }

        [Test]
        public void Deserialize_OneCell_ValueReturned()
        {
            int testNr = 0;

            // Create an instance of the XmlSerializer specifying type and namespace.
            TestSuiteXml ts = DeserializeSample();

            // Check the properties of the object.
            Assert.That(ts.Tests[testNr].Constraints[0], Is.TypeOf<EqualToXml>());
            var equalTo = ts.Tests[testNr].Constraints[0] as EqualToXml;

            Assert.That(equalTo.ResultSet.Rows, Has.Count.EqualTo(1));
            Assert.That(equalTo.ResultSet.Rows[0].Cells, Has.Count.EqualTo(1));
            Assert.That(equalTo.ResultSet.Rows[0].Cells[0].Value, Is.EqualTo("Document Control"));
        }
        


    }
}
