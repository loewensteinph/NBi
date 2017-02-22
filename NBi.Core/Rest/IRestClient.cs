using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Rest
{
    public interface IRestClient
    {
        string Download(string uri, IDictionary<string, string> parameters);
        DataSet Parse(string content);
    }
}
