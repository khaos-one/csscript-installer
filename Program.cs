using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Principal;

namespace CSScriptInstaller {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("CSScript Installer v1.0\n" +
                              "By Egor khaos Zelensky\n" +
                              "-----------------------\n");

            if (!UacHelper.IsInRole(WindowsBuiltInRole.Administrator)) {
                Error("Installation program requires administration rights.");
            }

            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            Console.Write($"Unpacking archive into `{tempFolder}`...");

            using (var rstream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CSScriptInstaller.Content.zip")) {
                if (rstream == null) {
                    Directory.Delete(tempFolder, true);
                    Error("Installation archive is corrupt.");
                }

                using (var zip = new ZipArchive(rstream)) {
                    zip.ExtractToDirectory(tempFolder);
                    Console.WriteLine("done.");
                }
            }

            var programfiles = Environment.GetEnvironmentVariable("PROGRAMFILES");

            if (string.IsNullOrWhiteSpace(programfiles)) {
                Directory.Delete(tempFolder, true);
                Error("No program files directory found.");
            }

            var installto = Path.Combine(programfiles, "CSScript");

            if (Directory.Exists(installto)) {
                Directory.Delete(tempFolder, true);
                Error($"Installation directory `{installto}` already exists.");
            }

            try {
                Directory.CreateDirectory(installto);
            }
            catch (UnauthorizedAccessException) {
                Directory.Delete(tempFolder, true);
                Error($"Installation directory `{installto}` cannot be accessed.");
            }

            Console.Write($"Installing to `{installto}`...");
            FsHelper.MoveContent(tempFolder, installto);
            Console.WriteLine("done.");

            Console.Write($"Registering CSScript...");
            var installscript = Path.Combine(installto, "install_silent.cmd");

            if (!File.Exists(installscript)) {
                Directory.Delete(tempFolder, true);
                Directory.Delete(installto, true);
                Error($"Registration script `{installscript}` not found!", 2);
            }

            var ret = ProcessHelper.Execute(installscript, workingDirectory: installto);

            if (ret < 0) {
                Directory.Delete(tempFolder, true);
                Directory.Delete(installto, true);
                Error("CSScript registration failed.", 2);
            }

            Console.WriteLine("done.");
            Console.Write("Cleaning up...");
            Directory.Delete(tempFolder, true);
            Console.WriteLine("done.");

            Console.WriteLine();
            Console.WriteLine("SUCCESS: Installation finished.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void Error(string err, int exitCode = 1) {
            Console.WriteLine($"ERROR: {err}");
            Console.Write("Press any key to continye...");
            Console.ReadKey();
            Environment.Exit(exitCode);
        }
    }
}
