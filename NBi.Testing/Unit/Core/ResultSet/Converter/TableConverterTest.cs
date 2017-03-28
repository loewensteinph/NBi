using System;
using System.Linq;
using NBi.Core.ResultSet.Comparer;
using NUnit.Framework;
using NBi.Core.ResultSet.Converter;
using System.Data;

namespace NBi.Testing.Unit.Core.ResultSet.Converter
{
    [TestFixture]
    public class TableConverterTest
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

        //Valid tables
        [Test]
        public void IsValid_Null_True()
        {
            Assert.That(new TableConverter().IsValid(null), Is.True);
        }

        [Test]
        public void IsValid_EmptyTable_True()
        {
            
            Assert.That(new TableConverter().IsValid(new DataTable()), Is.True);
        }

        [Test]
        public void IsValid_TableWithoutRows_True()
        {
            var table = new DataTable();
            table.Columns.Add(new DataColumn());
            Assert.That(new TableConverter().IsValid(table), Is.True);
        }

        [Test]
        public void IsValid_TableWithRows_True()
        {
            var table = new DataTable();
            table.Columns.Add(new DataColumn());
            var row = table.NewRow();
            row.ItemArray = new[] { "Hello World" };
            table.Rows.Add(row);
            table.AcceptChanges();
            Assert.That(new TableConverter().IsValid(table), Is.True);
        }

        [Test]
        public void IsValid_string_False()
        {
            
            Assert.That(new TableConverter().IsValid("Hello World"), Is.False);
        }

        [Test]
        public void IsValid_Object_False()
        {

            Assert.That(new TableConverter().IsValid(new object()), Is.False);
        }

        [Test]
        public void IsValid_Array_False()
        {

            Assert.That(new TableConverter().IsValid(new object[] { }), Is.False);
        }

    }
}
