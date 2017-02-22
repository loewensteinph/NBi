using NBi.Core.Rest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Testing.Unit.Core.Rest
{
    [TestFixture()]
    public class JsonRestClientTest
    {
        [Test]
        public void Parse_Object_Successful()
        {
            var json = "{ \"name\":\"John\", \"age\":31, \"city\":\"New York\" }";
            var client = new JsonRestClient(null);

            var ds = client.Parse(json);
            Assert.That(ds.Tables[0].Columns, Has.Count.EqualTo(3));
            Assert.That(ds.Tables[0].Columns[0].ColumnName, Is.EqualTo("name"));
            Assert.That(ds.Tables[0].Columns[1].ColumnName, Is.EqualTo("age"));
            Assert.That(ds.Tables[0].Columns[2].ColumnName, Is.EqualTo("city"));
            Assert.That(ds.Tables[0].Rows, Has.Count.EqualTo(1));
        }

        [Test]
        public void Parse_DataTableAsObject_Successful()
        {
            var json = "{\"employees\":[{ \"firstName\":\"John\", \"lastName\":\"Doe\" },{ \"firstName\":\"Anna\", \"lastName\":\"Smith\" },{ \"firstName\":\"Peter\", \"lastName\":\"Jones\" }]}";
            var client = new JsonRestClient(null);

            var ds = client.Parse(json);
            Assert.That(ds.Tables[0].Columns, Has.Count.EqualTo(2));
            Assert.That(ds.Tables[0].Columns[0].ColumnName, Is.EqualTo("firstName"));
            Assert.That(ds.Tables[0].Columns[1].ColumnName, Is.EqualTo("lastName"));
            Assert.That(ds.Tables[0].Rows, Has.Count.EqualTo(3));
        }

        [Test]
        public void Parse_ObjectWithDataTable_Successful()
        {
            var json = "{\"company\":\"My Company\", \"employees\":[{ \"firstName\":\"John\", \"lastName\":\"Doe\" },{ \"firstName\":\"Anna\", \"lastName\":\"Smith\" },{ \"firstName\":\"Peter\", \"lastName\":\"Jones\" }]}";
            var client = new JsonRestClient(null);

            var ds = client.Parse(json);

            Assert.That(ds.Tables[0].Columns, Has.Count.EqualTo(2));
            Assert.That(ds.Tables[0].Columns[0].ColumnName, Is.EqualTo("company"));
            Assert.That(ds.Tables[0].Columns[1].ColumnName, Is.EqualTo("employees"));

            Assert.That(ds.Tables[0].Rows[0].ItemArray[1], Is.TypeOf<DataTable>());
            var dataTable = ds.Tables[0].Rows[0].ItemArray[1] as DataTable;
            Assert.That(dataTable.Columns, Has.Count.EqualTo(2));
            Assert.That(dataTable.Columns[0].ColumnName, Is.EqualTo("firstName"));
            Assert.That(dataTable.Columns[1].ColumnName, Is.EqualTo("lastName"));
            Assert.That(dataTable.Rows, Has.Count.EqualTo(3));
        }

        [Test]
        [Ignore]
        public void Parse_Array_Successful()
        {
            var json = "{\"employees\":[ \"John\", \"Anna\", \"Peter\" ]}";
            var client = new JsonRestClient(null);

            var ds = client.Parse(json);

            Assert.That(ds.Tables[0].Columns, Has.Count.EqualTo(2));
            Assert.That(ds.Tables[0].Columns[0].ColumnName, Is.EqualTo("company"));
            Assert.That(ds.Tables[0].Columns[1].ColumnName, Is.EqualTo("employees"));

            Assert.That(ds.Tables[0].Rows[0].ItemArray[1], Is.TypeOf<DataTable>());
            var dataTable = ds.Tables[0].Rows[0].ItemArray[1] as DataTable;
            Assert.That(dataTable.Columns, Has.Count.EqualTo(2));
            Assert.That(dataTable.Columns[0].ColumnName, Is.EqualTo("firstName"));
            Assert.That(dataTable.Columns[1].ColumnName, Is.EqualTo("lastName"));
            Assert.That(dataTable.Rows, Has.Count.EqualTo(3));
        }
        
    }
}
