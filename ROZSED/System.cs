using System.Diagnostics;
using System.Reflection;

namespace ROZSED.Std
{
    public static class Sys
    {
        /// <summary>
        /// Executes a system command from inside csharp.
        /// </summary>
        /// <param name="command">A dos type command like "copy ..."</param>
        /// <param name ="timeout">How long to wait for command comletion. Implicit is indefinitely (-1).</param>
        public static int Run(string command, int timeout = -1, bool noWin = false)
        {
            ProcessStartInfo ProcessInfo;
            Process Process;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/C " + command);
            ProcessInfo.CreateNoWindow = noWin;
            ProcessInfo.UseShellExecute = false;
            Process = Process.Start(ProcessInfo);
            if (timeout < 0)
                Process.WaitForExit();
            else
                Process.WaitForExit(timeout);
            var ExitCode = Process.ExitCode;
            Process.Close();
            return ExitCode;
        }
        public static string AssemblyName()
        {
            return Assembly.GetEntryAssembly().GetName().Name;
        }
    }
}
