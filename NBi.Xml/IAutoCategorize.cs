using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Xml
{
    public interface IAutoCategorize
    {
        ICollection<string> GetAutoCategories();

        string TypeName { get; }

        Dictionary<string, string> GetRegexMatch();
    }
}
