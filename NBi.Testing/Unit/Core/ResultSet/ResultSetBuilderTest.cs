#region Using directives
using System.Collections.Generic;
using System.Linq;
using NBi.Xml.Items.ResultSet;
using NUnit.Framework;
using NBiRs = NBi.Core.ResultSet;
using System.Data;

#endregion

namespace NBi.Testing.Unit.Core.ResultSet
{
    [TestFixture]
    public class ResultSetBuilderTest
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
        public void Build_OneRow_OneRow()
        {
            var row = new RowXml();
            row._cells.Add(new CellXml { Value = "1" });
            row._cells.Add(new CellXml { Value = "2" });

            var builder = new NBiRs.ResultSetBuilder();
            var rs = builder.Build(Enumerable.Repeat(row as NBiRs.IRow, 1).ToList());

            Assert.That(rs.Rows, Has.Count.EqualTo(1));
            Assert.That(rs.Columns, Has.Count.EqualTo(2));
            
        }

        [Test]
        public void Build_OneRowWithSubTable_OneRow()
        {
            var row = new RowXml();
            row._cells.Add(new CellXml { Value = "1" });
            var subCell1a = new CellXml { Value = "1a" };
            var subCell1b = new CellXml { Value = "1b" };
            var subCell1c = new CellXml { Value = "1c" };
            var subRow1 = new RowXml();
            subRow1._cells.Add(subCell1a);
            subRow1._cells.Add(subCell1b);
            subRow1._cells.Add(subCell1c);

            var subCell2a = new CellXml { Value = "2a" };
            var subCell2b = new CellXml { Value = "2b" };
            var subCell2c = new CellXml { Value = "2c" };
            var subRow2 = new RowXml();
            subRow2._cells.Add(subCell2a);
            subRow2._cells.Add(subCell2b);
            subRow2._cells.Add(subCell2c);

            var cell2 = new CellXml();
            cell2._rows.Add(subRow1);
            cell2._rows.Add(subRow2);
            row._cells.Add(cell2);

            var builder = new NBiRs.ResultSetBuilder();
            var rs = builder.Build(Enumerable.Repeat(row as NBiRs.IRow, 1).ToList());

            Assert.That(rs.Rows, Has.Count.EqualTo(1));
            Assert.That(rs.Columns, Has.Count.EqualTo(2));

            Assert.That(rs.Rows[0].ItemArray[1], Is.InstanceOf<DataTable>());
            var subTable = rs.Rows[0].ItemArray[1] as DataTable;
            Assert.That(subTable.Rows, Has.Count.EqualTo(2));
            Assert.That(subTable.Columns, Has.Count.EqualTo(3));

        }

    }
}
