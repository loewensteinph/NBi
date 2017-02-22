using NBi.Core.Rest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Testing.Unit.Core.Rest
{
    [TestFixture()]
    public class RestClientFactoryTest
    {
        [Test]
        public void Instantiate_Json_JsonClient()
        {
            var factory = new RestClientFactory();
            var client = factory.Instantiate(ContentType.Json, "http://calapi.inadiutorium.cz/api/v0/en/", CredentialsType.Anonymous);

            Assert.That(client, Is.TypeOf<JsonRestClient>());
        }
        
    }
}
