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
            StringBuilder outputBuilder = new StringBuilder();
            ProcessStartInfo processStartInfo;
            System.Diagnostics.Process process;

            if (string.IsNullOrEmpty(argument))
                Console.WriteLine("Starting process: {0} without argument.", fullPath);
            else
                Console.WriteLine("Starting process: {0} with arguments \"{1}\".", fullPath, argument);
            var result = false;

            processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = fullPath;
            processStartInfo.Arguments = argument;
            processStartInfo.RedirectStandardOutput = true;

            processStartInfo.UseShellExecute = silent;


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
                    if (exePath != null) processStartInfo.FileName = exePath;
                }
            }

            process = new System.Diagnostics.Process();
            process.StartInfo = processStartInfo;
            // enable raising events because Process does not raise events by default
            process.EnableRaisingEvents = true;
            // attach the event handler for OutputDataReceived before starting the process
            process.OutputDataReceived += new DataReceivedEventHandler
            (
                delegate (object sender, DataReceivedEventArgs e)
                {
                    // append the new data to the data already read-in
                    outputBuilder.Append(Environment.NewLine);
                    outputBuilder.Append(e.Data);
                }
            );


            process.Start();
            process.BeginOutputReadLine();
            if (timeOut > 0)
            {
                Console.WriteLine("Waiting the end of the process with timeout.");
                process.WaitForExit(timeOut);
            }
            else
            {
                Console.WriteLine("Waiting the end of the process.");
                process.WaitForExit();
            }
            result = process.ExitCode == 0;
            process.CancelOutputRead();

            string output = outputBuilder.ToString();
            Console.WriteLine(string.Format("Process StdOut: {0} ", output));

            if (!result)
                throw new InvalidProgramException();
        }
    }
}
