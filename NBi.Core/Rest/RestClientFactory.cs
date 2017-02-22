using Antlr4.StringTemplate;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NBi.Core.Rest
{
    class RestClientFactory
    {
        public IRestClient Instantiate(ContentType contentType, string baseAddress, CredentialsType credentialsType)
        {
            var webClient = new WebClient();

            if (!string.IsNullOrEmpty(baseAddress))
                webClient.BaseAddress = baseAddress;

            switch (credentialsType)
            {
                case CredentialsType.Anonymous:
                    webClient.Credentials = null;
                    webClient.UseDefaultCredentials = false;
                    break;
                case CredentialsType.CurrentUser:
                    webClient.UseDefaultCredentials = true;
                    break;
                default:
                    break;
            }

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            webClient.Headers.Add("User-Agent", "NBi");
            WebRequest.DefaultWebProxy = null;
            webClient.Proxy = null;

            switch (contentType)
            {
                case ContentType.Json:
                    return new JsonRestClient(webClient);
                default:
                    throw new ArgumentOutOfRangeException(nameof(contentType));
            }
        }
        
    }
}
