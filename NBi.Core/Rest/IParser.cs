using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Rest
{
    public interface IParser
    {
        DataSet Parse(string content);
    }
}
