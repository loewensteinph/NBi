using Antlr4.StringTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NBi.Core.Rest
{
    class UriParametersBuilder
    {
        private bool isSetup;
        private bool isBuild;

        private string originalUri;
        private IDictionary<string, string> originalParameters;

        private string finalUri;
        private IDictionary<string, string> finalParameters;

        public void Setup(string uri, IDictionary<string, string> parameters)
        {
            originalUri = uri;
            originalParameters = parameters;
            isSetup = true;
            isBuild = false;
        }

        public void Build()
        {
            if (!isSetup)
                throw new InvalidOperationException();

            var regex = new Regex(@"\$(.*?)\$");
            var neededParams = regex.Matches(originalUri);

            var missingParams = new List<string>();
            foreach (Match neededParam in neededParams)
            {
                var value = neededParam.Value.Replace("$", string.Empty);
                if (!originalParameters.Keys.Contains(value))
                    missingParams.Add(value);
            }

            if (missingParams.Count() > 0)
                throw new ArgumentException(
                    string.Format("The uri of the rest-api is expecting the following parameter{0} '{1}' but the call doesn't define a value for {2}"
                    , missingParams.Count() > 1 ? "s" : string.Empty
                    , string.Join("', '", missingParams.ToArray())
                    , missingParams.Count() > 1 ? "them" : "it"));

            var template = new Template(originalUri, '$', '$');
            foreach (var parameter in originalParameters)
                template.Add(parameter.Key, parameter.Value);

            finalUri = template.Render();

            finalParameters = new Dictionary<string, string>(originalParameters);
            foreach (Match neededParam in neededParams)
            {
                var value = neededParam.Value.Replace("$", string.Empty);
                finalParameters.Remove(value);
            }

            isBuild = true;
        }

        public string GetUri()
        {
            if (!isBuild)
                throw new InvalidOperationException();

            return finalUri;
        }

        public IDictionary<string, string> GetRemainingParameters()
        {
            if (!isBuild)
                throw new InvalidOperationException();

            return finalParameters;
        }
    }
}
