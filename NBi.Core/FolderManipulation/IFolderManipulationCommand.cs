using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBi.Core.Query;

namespace NBi.Core.FolderManipulation
{
    public interface IFolderManipulationCommand : IDecorationCommand
    {
        string Path { get; }
    }
}
