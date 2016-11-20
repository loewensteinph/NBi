using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Model
{
    public interface IModelDiscoveryCommand
    {
        ICommandDescription Description { get; }
        IEnumerable<string> Execute();

    }
}
