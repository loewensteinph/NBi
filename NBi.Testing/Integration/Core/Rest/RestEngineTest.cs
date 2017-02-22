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
        public void Execute_ValidCallForGithub_ExpectedResult()
        {
            var factory = new RestClientFactory();
            var client = factory.Instantiate(ContentType.Json, "https://api.github.com/", CredentialsType.Anonymous);
            var engine = new RestEngine(client);

            var parameters = new Dictionary<string, string>();
            parameters.Add("username", "Seddryck");

            var dataset = engine.Execute("users/$username$", parameters);

            Assert.That(dataset, Is.Not.Null);
            Assert.That(dataset.Tables, Has.Count.EqualTo(1));
            Assert.That(dataset.Tables[0].Rows, Has.Count.EqualTo(1));
            Assert.That(dataset.Tables[0].Columns, Has.Count.EqualTo(30));
            Assert.That(dataset.Tables[0].Rows[0].ItemArray[28], Is.EqualTo(new DateTime(2013,9,13,21,6,29)));
        }

        [Test]
        public void Execute_ValidCallForGithub2_ExpectedResult()
        {
            var factory = new RestClientFactory();
            var client = factory.Instantiate(ContentType.Json, "https://api.github.com/", CredentialsType.Anonymous);
            var engine = new RestEngine(client);

            var parameters = new Dictionary<string, string>();
            parameters.Add("username", "Seddryck");
            var dataset = engine.Execute("users/$username$/repos", parameters);

            Assert.That(dataset, Is.Not.Null);
            Assert.That(dataset.Tables, Has.Count.EqualTo(1));
            Assert.That(dataset.Tables[0].Rows, Has.Count.EqualTo(1));
        }
    }
}
