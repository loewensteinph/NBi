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
            List<string> folderBlacklist = new List<string>();
            folderBlacklist.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            folderBlacklist.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            folderBlacklist.Add(Environment.GetFolderPath(Environment.SpecialFolder.Windows));

            foreach (var excludeFolder in folderBlacklist)
            {
                if (folder.Contains(excludeFolder) || !Directory.Exists(folder))
                    return;
            }

            string[] drives = Directory.GetLogicalDrives();

            foreach (var drive in drives)
            {
                if(folder.Equals(drive, StringComparison.OrdinalIgnoreCase))
                    return;
            }          

            if (drives.Contains(folder))
                return;

            Directory.Delete(folder,true);
        }
    }
}
