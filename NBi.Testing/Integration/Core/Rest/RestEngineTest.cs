using NBi.Core.Rest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Testing.Integration.Core.Rest
{
    [TestFixture()]
    public class RestEngineTest
    {
        [Test]
        public void Execute_ValidCallForObject_ExpectedResult()
        {
            var factory = new RestClientFactory();
            var client = factory.Instantiate(ContentType.Json, "http://calapi.inadiutorium.cz/api/v0/en/", CredentialsType.Anonymous);
            var engine = new RestEngine(client);

            var parameters = new Dictionary<string, string>();
            parameters.Add("calendar", "default");
            parameters.Add("year", "2015");
            parameters.Add("month", "6");
            parameters.Add("day", "27");

            var dataset = engine.Execute("calendars/$calendar$/$year$/$month$/$day$", parameters);

            Assert.That(dataset, Is.Not.Null);
            Assert.That(dataset.Tables, Has.Count.EqualTo(1));
            Assert.That(dataset.Tables[0].Rows, Has.Count.EqualTo(1));
            Assert.That(dataset.Tables[0].Rows[0].ItemArray[0], Is.EqualTo("2015-06-27"));
            Assert.That(dataset.Tables[0].Rows[0].ItemArray[1], Is.EqualTo("ordinary"));
            Assert.That(dataset.Tables[0].Rows[0].ItemArray[2], Is.EqualTo(12));
            Assert.That(dataset.Tables[0].Rows[0].ItemArray[3], Is.TypeOf<DataTable>());
            Assert.That(dataset.Tables[0].Rows[0].ItemArray[4], Is.EqualTo("saturday"));

            var subTable = dataset.Tables[0].Rows[0].ItemArray[3] as DataTable;
            Assert.That(subTable.Rows, Has.Count.EqualTo(2));
            Assert.That(subTable.Rows[0].ItemArray[0], Is.EqualTo(string.Empty));
            Assert.That(subTable.Rows[1].ItemArray[0], Is.EqualTo("Saint Cyril of Alexandria, bishop and doctor"));
            Assert.That(subTable.Rows[0].ItemArray[1], Is.EqualTo("green"));
            Assert.That(subTable.Rows[1].ItemArray[1], Is.EqualTo("white"));
        }

        [Test]
        public void Execute_ValidCallForGoogleBooks_ExpectedResult()
        {
            var factory = new RestClientFactory();
            var client = factory.Instantiate(ContentType.Json, "https://www.googleapis.com/books/v1/", CredentialsType.Anonymous);
            var engine = new RestEngine(client);

            var parameters = new Dictionary<string, string>();
            parameters.Add("isbn", "0747532699");

            var dataset = engine.Execute("volumes?q=isbn:$isbn$", parameters);

            Assert.That(dataset, Is.Not.Null);
            Assert.That(dataset.Tables, Has.Count.EqualTo(1));
            Assert.That(dataset.Tables[0].Rows, Has.Count.EqualTo(1));
            Assert.That(dataset.Tables[0].Columns, Has.Count.EqualTo(3));

            Assert.That(dataset.Tables[0].Rows[0].ItemArray[2], Is.TypeOf<DataTable>());
            var items = dataset.Tables[0].Rows[0].ItemArray[2] as DataTable;

            Assert.That(items.Rows[0].ItemArray[4], Is.TypeOf<DataTable>());
            var volumeInfo = items.Rows[0].ItemArray[4] as DataTable;

            Assert.That(volumeInfo.Rows[0].ItemArray[1], Is.InstanceOf<IEnumerable<object>>());
            var authors = volumeInfo.Rows[0].ItemArray[1] as IEnumerable<object>;
            Assert.That(authors, Has.Count.EqualTo(1));

            Assert.That(volumeInfo.Rows[0].ItemArray[7], Is.TypeOf<int>());
        }

        [Test]
        public void Execute_ValidCallForGoogleBooks2_ExpectedResult()
        {
            var factory = new RestClientFactory();
            var client = factory.Instantiate(ContentType.Json, "https://www.googleapis.com/books/v1/", CredentialsType.Anonymous);
            var engine = new RestEngine(client);

            var parameters = new Dictionary<string, string>();
            parameters.Add("q", "isbn:3905829800");

            var dataset = engine.Execute("volumes", parameters);

            Assert.That(dataset, Is.Not.Null);
            Assert.That(dataset.Tables, Has.Count.EqualTo(1));
            Assert.That(dataset.Tables[0].Rows, Has.Count.EqualTo(1));
            Assert.That(dataset.Tables[0].Columns, Has.Count.EqualTo(3));

            Assert.That(dataset.Tables[0].Rows[0]["items"], Is.TypeOf<DataTable>());
            var items = dataset.Tables[0].Rows[0]["items"] as DataTable;

            Assert.That(items.Rows[0]["volumeInfo"], Is.TypeOf<DataTable>());
            var volumeInfo = items.Rows[0]["volumeInfo"] as DataTable;

            Assert.That(volumeInfo.Rows[0]["authors"], Is.InstanceOf<IEnumerable<object>>());
            var authors = volumeInfo.Rows[0]["authors"] as IEnumerable<object>;
            Assert.That(authors, Has.Count.EqualTo(2));

            Assert.That(volumeInfo.Rows[0]["pageCount"], Is.TypeOf<int>());
        }
    }
}
