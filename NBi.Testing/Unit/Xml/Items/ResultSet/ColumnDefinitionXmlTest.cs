using System.IO;
using System.Reflection;
using NBi.Xml;
using NBi.Xml.Items;
using NBi.Xml.Systems;
using NUnit.Framework;
using NBi.Xml.Constraints;
using System.Collections.Generic;
using NBi.Core.ResultSet;

namespace NBi.Testing.Unit.Xml.Items.ResultSet
{
    [TestFixture]
    public class ColumnDefinitionTestXml
    {
        protected TestSuiteXml DeserializeSample()
        {
            // Declare an object variable of the type to be deserialized.
            var manager = new XmlManager();

            // A Stream is needed to read the XML document.
            using (Stream stream = Assembly.GetExecutingAssembly()
                                           .GetManifestResourceStream("NBi.Testing.Unit.Xml.Resources.ColumnDefinitionXmlTestSuite.xml"))
            using (StreamReader reader = new StreamReader(stream))
            {
                manager.Read(reader);
            }
            return manager.TestSuite;
        }
        
        [Test]
        public void Deserialize_Columns_CorrectType()
        {
            int testNr = 0;
            
            // Create an instance of the XmlSerializer specifying type and namespace.
            TestSuiteXml ts = DeserializeSample();

            // Check the properties of the object.
            Assert.That(ts.Tests[testNr].Constraints[0], Is.TypeOf<EqualToXml>());
            var equalTo = ts.Tests[testNr].Constraints[0] as EqualToXml;

            Assert.That(equalTo.ColumnsDef, Is.Not.Null);
            Assert.That(equalTo.ColumnsDef, Has.Count.EqualTo(5));

            Assert.That(equalTo.ColumnsDef[0].Type, Is.EqualTo(ColumnType.Numeric));
            Assert.That(equalTo.ColumnsDef[1].Type, Is.EqualTo(ColumnType.Text));
            Assert.That(equalTo.ColumnsDef[2].Type, Is.EqualTo(ColumnType.DateTime));
            Assert.That(equalTo.ColumnsDef[3].Type, Is.EqualTo(ColumnType.Boolean));
            Assert.That(equalTo.ColumnsDef[4].Type, Is.EqualTo(ColumnType.Table));
        }
        

    }
}
