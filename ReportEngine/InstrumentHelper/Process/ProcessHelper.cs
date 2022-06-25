using System.Diagnostics;

namespace ReportEngine.InstrumentHelper.Process
{
    public class ProcessHelper
    {
        public static ProcessResult ExecuteProcess(string fileName, string arguments)
        {
            var instrumentProcess = new System.Diagnostics.Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = arguments,
                    UseShellExecute = false,
                }
            };

            instrumentProcess.Start();
            var output = instrumentProcess.StandardOutput.ReadToEnd();
            var error = instrumentProcess.StandardError.ReadToEnd();
            instrumentProcess.WaitForExit();

            return new ProcessResult
            {
                Error = error,
                Output = output
            };
        }
    }
}