using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.Query;

namespace NBi.Core.Batch
{
    public interface IBatchRunCommand : IBatchCommand
    {
        string FullPath { get; }
        string InlineQuery { get; set; }
        IEnumerable<IQueryTemplateVariable> Variables { get; set; }
    }
}
