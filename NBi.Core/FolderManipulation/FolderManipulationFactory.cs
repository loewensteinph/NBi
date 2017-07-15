using System;
using System.Data.SqlClient;
using System.Linq;
using NBi.Core.FolderManipulation;

namespace NBi.Core.FolderManipulation
{
    public class FolderManipulationFactory
    {
        public IDecorationCommandImplementation Get(IFolderManipulationCommand command)
        {

            if (command is IFolderDeleteCommand)
                return new FolderDeleteCommand(command as IFolderDeleteCommand);

            throw new ArgumentException();
        }
    }
}
