using System.Diagnostics;

namespace CSScriptInstaller {
    public static class ProcessHelper {
        public static int Execute(string executable, string args = "", string workingDirectory = "") {
            var pi = new ProcessStartInfo(executable, args) {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            if (!string.IsNullOrWhiteSpace(workingDirectory)) {
                pi.WorkingDirectory = workingDirectory;
            }

            var process = Process.Start(pi);

            if (process == null) {
                return -1;
            }

            process.WaitForExit();
            return process.ExitCode;
        }
    }
}
