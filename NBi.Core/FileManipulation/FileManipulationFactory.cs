using System;
using System.Data.SqlClient;
using System.Linq;
using NBi.Core.FileManipulation;

namespace NBi.Core.DataManipulation
{
    public class FileManipulationFactory
    {
        public IDecorationCommandImplementation Get(IFileManipulationCommand command)
        {

            if (command is IFileDeleteCommand)
                return new FileDeleteCommand(command as IFileDeleteCommand);
            if (command is IFileCopyCommand)
                return new FileCopyCommand(command as IFileCopyCommand);

            throw new ArgumentException();
        }
    }
}
