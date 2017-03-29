using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBi.Core.ResultSet
{
    public interface ICell
    {
        string Value { get; set; }
        IEnumerable<string> Values { get; set; }
        IList<IRow> Rows { get; }
        string ColumnName { get; set; }
    }
}
