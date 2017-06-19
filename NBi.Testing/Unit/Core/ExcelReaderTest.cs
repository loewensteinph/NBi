using NBi.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Testing.Unit.Core
{
    public class ExcelReaderTest
    {
        [Test]
        [Category("ExcelReader")]
        [TestCase("1,5")]
        [TestCase("1.5")]
        public void ParseString_Positive(string item)
        {
            ExcelDefinition excelDefinition = new ExcelDefinition();
            var reader = new ExcelReader(excelDefinition);
            var value = reader.ParseString(item);
            Assert.That(value, Is.EqualTo(typeof(Decimal)));
        }
        [Test]
        [Category("ExcelReader")]
        [TestCase("01.01.1980")]
        [TestCase("2003-04-08 09:13:38")]
        [TestCase("2016-04-30 00:44:20.530")]
        [TestCase("2009-06-15T13:45:30.0000000Z")]
        public void ParseStringDate_Positive(string item)
        {
            ExcelDefinition excelDefinition = new ExcelDefinition();
            var reader = new ExcelReader(excelDefinition);
            var value = reader.ParseString(item);
            Assert.That(value, Is.EqualTo(typeof(DateTime)));
        }

        [Test]
        [Category("ExcelReader")]
        [TestCase("936DA01F-9ABD-4D9D-80C7-02AF85C822A8")]
        [TestCase("{936DA01F-9ABD-4D9D-80C7-02AF85C822A8}")]
        public void IsGuid_Positive(string item)
        {
            ExcelDefinition excelDefinition = new ExcelDefinition();
            var reader = new ExcelReader(excelDefinition);
            var value = reader.IsGuid(item);
            Assert.That(value, Is.EqualTo(true));
        }
        [Test]
        [Category("ExcelReader")]
        [TestCase("936DA01F-9ABD-4D9D-80C7-02AF85C822A")]
        [TestCase("{936DA01F-9ABD-4D9D-80C7-02AF85C822A8")]
        public void IsGuid_Negative(string item)
        {
            ExcelDefinition excelDefinition = new ExcelDefinition();
            var reader = new ExcelReader(excelDefinition);
            var value = reader.IsGuid(item);
            Assert.That(value, Is.EqualTo(false));
        }

        [Test]
        [Category("ExcelReader")]
        [TestCase("Test1")]

        public void DataTypes(string item)
        {
            ExcelDefinition excelDefinition = new ExcelDefinition();
            excelDefinition.SheetName = item;
            var reader = new ExcelReader(excelDefinition);

            var path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"..\..\Unit\Core\Excel\DataTableTest.xlsx");
            DataTable dt = reader.Read(path,true);
            Assert.That(dt.Columns[0].DataType, Is.EqualTo(typeof(Int64)));
            Assert.That(dt.Columns[1].DataType, Is.EqualTo(typeof(Int64)));
            Assert.That(dt.Columns[2].DataType, Is.EqualTo(typeof(Decimal)));
            Assert.That(dt.Columns[3].DataType, Is.EqualTo(typeof(DateTime)));
            Assert.That(dt.Columns[4].DataType, Is.EqualTo(typeof(Guid)));
            Assert.That(dt.Columns[5].DataType, Is.EqualTo(typeof(String)));
        }
    }
}
