using System.IO;

namespace CSScriptInstaller {
    public static class FsHelper {
        public static void MoveContent(string source, string destination, string pattern = "*", bool overwrite = false) {
            var sourcedir = new DirectoryInfo(source);
            var destdir = new DirectoryInfo(destination);

            foreach (var file in sourcedir.GetFiles(pattern)) {
                file.MoveTo(Path.Combine(destdir.FullName, file.Name));
            }

            foreach (var dir in sourcedir.GetDirectories(pattern)) {
                dir.MoveTo(Path.Combine(destdir.FullName, dir.Name));
            }
        }
    }
}
