using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Rest
{
    public class RestEngine 
    {
        private readonly IRestClient client;

        public RestEngine(IRestClient client)
        {
            this.client = client;
        }

        public virtual DataSet Execute(string uri, IDictionary<string, string> parameters)
        {
            var builder = new UriParametersBuilder();
            builder.Setup(uri, parameters);
            builder.Build();
            var content = client.Download(builder.GetUri(), builder.GetRemainingParameters());
            var dataset = client.Parse(content);
            
            return dataset;
        }

       
    }
}
