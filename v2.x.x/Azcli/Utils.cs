using System;
using System.Diagnostics;
using System.IO;

namespace Azurlane
{
    internal class Utils
    {
        internal static void eLogger(string message, Exception exception)
        {
            pDebugf(message);

            if (!File.Exists(PathMgr.Local("Logs.txt")))
                File.WriteAllText(PathMgr.Local("Logs.txt"), string.Empty);

            using (var streamWriter = new StreamWriter(PathMgr.Local("Logs.txt"), true))
            {
                streamWriter.WriteLine("=== START =================================================================================");
                streamWriter.WriteLine(message);
                streamWriter.WriteLine($"Date: {DateTime.Now.ToString()}");
                streamWriter.WriteLine($"Exception Message: {exception.Message}");
                streamWriter.WriteLine($"Exception StackTrace: {exception.StackTrace}");
                streamWriter.WriteLine("=== END ===================================================================================");
                streamWriter.WriteLine();
            }
        }

        internal static void NewCommand(string argument)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = $"/c {argument}";
                process.StartInfo.WorkingDirectory = PathMgr.Thirdparty();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.WaitForExit();
            }
        }

        internal static void pDebugf(string message) => Console.Write(string.Format("[{0}][DEBUG]> {1}", DateTime.Now.ToString("HH:mm:ss"), message));

        internal static void pDebugln(string message) => Console.WriteLine(string.Format("[{0}][DEBUG]> {1}", DateTime.Now.ToString("HH:mm:ss"), message));

        internal static void pInfof(string message) => Console.Write(string.Format("[{0}][INFO]> {1}", DateTime.Now.ToString("HH:mm:ss"), message));

        internal static void pInfoln(string message) => Console.WriteLine(string.Format("[{0}][INFO]> {1}", DateTime.Now.ToString("HH:mm:ss"), message));
    }
}