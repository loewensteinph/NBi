using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NBi.Core.Query;

namespace NBi.Core.FolderManipulation
{
    public class FolderDeleteCommand : IDecorationCommandImplementation
    {
        private readonly string _folderPath;

        public FolderDeleteCommand(IFolderDeleteCommand command)
        {
            this._folderPath = command.Path;
        }

        public void Execute()
        {
            Execute(_folderPath);
        }

        internal void Execute(string folder)
        {
            if (!Directory.Exists(folder))
                return;
            Directory.Delete(folder,true);
        }
    }
}
