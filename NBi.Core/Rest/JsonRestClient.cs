using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Rest
{
    public class JsonRestClient : IRestClient
    {
        private readonly WebClient client;
        public JsonRestClient(WebClient client)
        {
            this.client = client;
        }

        public string Download(string uri, IDictionary<string, string> parameters)
        {
            client.QueryString = parameters.Aggregate(new NameValueCollection(),
                (seed, current) =>
                {
                    seed.Add(current.Key, current.Value);
                    return seed;
                });
            return client.DownloadString(uri);
        }

        public DataSet Parse(string content)
        {
            //If json is an object and not an array then transform it to array
            var token = JToken.Parse(content);
            if (token is JObject && token.Count()>1)
                content = new StringBuilder(content).Append("]}").Insert(0, "{ \"object\": [").ToString();

            //desrialize to Dataset
            var dataset = JsonConvert.DeserializeObject<DataSet>(content);
            return dataset;
        }
    }
}
