using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NBi.Core.Process
{
    class RunCommand : IDecorationCommandImplementation
    {
        private readonly string argument;
        private readonly string fullPath;
        private readonly int timeOut;
        private readonly bool silent;

        public RunCommand(IRunCommand command)
        {
            argument = command.Argument;
            fullPath = command.FullPath;
            timeOut = command.TimeOut;
            silent = command.Silent;
        }

        public void Execute()
        {
            if (string.IsNullOrEmpty(argument))
                Console.WriteLine("Starting process {0} without argument.", fullPath);
            else
                Console.WriteLine("Starting process {0} with arguments \"{1}\".", fullPath, argument);
            var result = false;
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = fullPath;
            startInfo.Arguments = argument;

            startInfo.UseShellExecute = silent;

            if (fullPath.Contains("%"))
            {
                var enviromentPath = Environment.GetEnvironmentVariable("PATH");
                var file = Path.GetFileName(fullPath);
                if (enviromentPath != null)
                {
                    var paths = enviromentPath.Split(';');
                    var exePath = paths
                        .Select(x => Path.Combine(x, file))
                        .FirstOrDefault(File.Exists);
                    if (exePath != null) startInfo.FileName = exePath;
                }
            }

            using (var exeProcess = System.Diagnostics.Process.Start(startInfo))
            {
                if (timeOut != 0)
                {
                    Console.WriteLine("Waiting the end of the process.");
                    if (exeProcess != null)
                    {
                        exeProcess.WaitForExit(timeOut);
                        if (exeProcess.HasExited)
                        {
                            Console.WriteLine(exeProcess.ExitCode == 0
                                ? "Process has been successfully executed."
                                : "Process has failed.");
                            result = exeProcess.ExitCode == 0;
                        }
                        else
                            Console.WriteLine("Process has been interrupted before the end of its execution.");
                    }
                }
                else
                {
                    Console.WriteLine("Not waiting the end of the process.");
                    result = true;
                }
            }

            if (!result)
                throw new InvalidProgramException();

        }
    }
}
