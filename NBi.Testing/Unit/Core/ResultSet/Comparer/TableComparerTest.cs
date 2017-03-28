using NBi.Core.ResultSet;
using NBi.Core.ResultSet.Comparer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Testing.Unit.Core.ResultSet.Comparer
{
    [TestFixture]
    class TableComparerTest
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
        public void Compare_NullVsNull_True()
        {
            var comparer = new TableComparer(null);
            Assert.That(comparer.Compare(null, null).AreEqual, Is.True);
        }

        [Test]
        public void Compare_NullVsEmptyDataTable_True()
        {
            var comparer = new TableComparer(null);
            Assert.That(comparer.Compare(null, new DataTable()).AreEqual, Is.True);
        }

        [Test]
        public void IsValid_NullVsTableWithoutRows_True()
        {
            var comparer = new TableComparer(null);
            var table = new DataTable();
            table.Columns.Add(new DataColumn());
            Assert.That(comparer.Compare(null, table).AreEqual, Is.True);
        }

        [Test]
        public void IsValid_NullVsTableWithRows_False()
        {
            var comparer = new TableComparer(null);
            var table = new DataTable();
            table.Columns.Add(new DataColumn());
            var row = table.NewRow();
            row.ItemArray = new[] { "Hello World" };
            table.Rows.Add(row);
            table.AcceptChanges();
            Assert.That(comparer.Compare(null, table).AreEqual, Is.False);
        }

        [Test]
        public void Compare_EmptyDataTableVsTableWithoutRows_True()
        {
            var comparer = new TableComparer(null);
            var table = new DataTable();
            table.Columns.Add(new DataColumn());
            Assert.That(comparer.Compare(table, new DataTable()).AreEqual, Is.True);
        }

        [Test]
        public void IsValid_TableVsTable_False()
        {
            var colDef = new Column() { Name = "alpha", Role = ColumnRole.Key, Type = ColumnType.Numeric };
            var settings = new SettingsResultSetComparisonByName(Enumerable.Repeat(colDef, 1));
            var comparer = new TableComparer(settings);

            var table1 = new DataTable();
            table1.Columns.Add(new DataColumn("alpha"));
            var row1 = table1.NewRow();
            row1.ItemArray = new[] { "157" };
            table1.Rows.Add(row1);
            table1.AcceptChanges();

            var table2 = new DataTable();
            table2.Columns.Add(new DataColumn("alpha"));
            var row2 = table2.NewRow();
            row2.ItemArray = new[] { "201" };
            table2.Rows.Add(row2);
            table2.AcceptChanges();

            Assert.That(comparer.Compare(table1, table2).AreEqual, Is.False);
        }

        [Test]
        public void IsValid_TableVsTableWithMoreRows_False()
        {
            var colDef = new Column() { Name = "alpha", Role = ColumnRole.Key, Type = ColumnType.Numeric };
            var settings = new SettingsResultSetComparisonByName(Enumerable.Repeat(colDef, 1));
            var comparer = new TableComparer(settings);

            var table1 = new DataTable();
            table1.Columns.Add(new DataColumn("alpha"));
            table1.Columns.Add(new DataColumn());
            var row1 = table1.NewRow();
            row1.ItemArray = new[] { "157" };
            table1.Rows.Add(row1);
            table1.AcceptChanges();

            var table2 = new DataTable();
            table2.Columns.Add(new DataColumn("alpha"));
            var row2 = table2.NewRow();
            row2.ItemArray = new[] { "157" };
            table2.Rows.Add(row2);
            var row3 = table2.NewRow();
            row3.ItemArray = new[] { "201" };
            table2.Rows.Add(row3);
            table2.AcceptChanges();

            Assert.That(comparer.Compare(table1, table2).AreEqual, Is.False);
        }

        [Test]
        public void IsValid_TableVsTableWithSameRows_True()
        {
            var colDef = new Column() { Name = "alpha", Role = ColumnRole.Key, Type = ColumnType.Numeric };
            var settings = new SettingsResultSetComparisonByName(Enumerable.Repeat(colDef, 1));
            var comparer = new TableComparer(settings);

            var table1 = new DataTable();
            table1.Columns.Add(new DataColumn("alpha"));
            table1.Columns.Add(new DataColumn());
            var row1 = table1.NewRow();
            row1.ItemArray = new[] { "157" };
            table1.Rows.Add(row1);
            table1.AcceptChanges();

            var table2 = new DataTable();
            table2.Columns.Add(new DataColumn("alpha"));
            var row2 = table2.NewRow();
            row2.ItemArray = new[] { "157" };
            table2.Rows.Add(row2);
            table2.AcceptChanges();

            Assert.That(comparer.Compare(table1, table2).AreEqual, Is.True);
        }

    }
}
