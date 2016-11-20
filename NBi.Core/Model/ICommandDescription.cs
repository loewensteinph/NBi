using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Model
{
    public interface ICommandDescription
    {
        Enum Target { get; }
        IEnumerable<IFilter> Filters { get; }
        IEnumerable<ICaptionFilter> GetCaptionNonTargetFilter();
        
    }
}
