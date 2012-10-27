using System;
using System.IO;

namespace ROZSED.Std
{
    public static class Fls
    {
        /// <summary>
        /// Merge files as array of bytes.
        /// </summary>
        /// <param name="outPath"></param>
        /// <param name="pathsToMerge"></param>
        public static void MergeFiles(this string outPath, params string[] pathsToMerge)
        {
            var length = pathsToMerge.Length;
            if (length == 0)
                throw new ArgumentException("No files to merge.");

            using (var fsOut = File.Open(outPath, FileMode.Append))
            {
                FileStream fsToMerge;
                int fsToMergeLength;
                byte[] fsToMergeContent;
                foreach (var path in pathsToMerge)
                    using (fsToMerge = File.Open(path, FileMode.Open))
                    {
                        fsToMergeLength = (int)fsToMerge.Length;
                        fsToMergeContent = new byte[fsToMergeLength];
                        fsToMerge.Read(fsToMergeContent, 0, fsToMergeLength);
                        fsOut.Write(fsToMergeContent, 0, fsToMergeLength);
                    }
            }
        }
        /// <summary>
        /// Copy all directories, subdirectories and files from <paramref name="sourcePath"/> to <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="overwriteExisting"></param>
        public static void CopyDirectory(this string sourcePath, string destinationPath, bool overwriteExisting)
        {
            sourcePath = sourcePath.EndsWith(@"\") ? sourcePath : sourcePath + @"\";
            destinationPath = destinationPath.EndsWith(@"\") ? destinationPath : destinationPath + @"\";

            if (!Directory.Exists(sourcePath))
                throw new ArgumentException("Source path doesn't exist.");

            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            foreach (string fls in Directory.GetFiles(sourcePath))
            {
                FileInfo flinfo = new FileInfo(fls);
                flinfo.CopyTo(destinationPath + flinfo.Name, overwriteExisting);
            }
            foreach (string drs in Directory.GetDirectories(sourcePath))
            {
                DirectoryInfo drinfo = new DirectoryInfo(drs);
                CopyDirectory(drs, destinationPath + drinfo.Name, overwriteExisting);
            }
        }
        /// <summary>
        /// Merge files by directories (1) in 'topDirPath' (0). Output file name = (1) + outFilExt.
        /// </summary>
        /// <param name="topDirPath"></param>
        /// <param name="outFilExt"></param>
        /// <param name="dirPattern">Pattern to search directories (1) in 'topDirPath' (0).</param>
        /// <param name="filPattern">Pattern to search files to merge in directories (1) in 'topDirPath' (0).</param>
        public static void MergeByDir(this string topDirPath, string outFilExt, string dirPattern = "*", string filPattern = "*")
        {
            foreach (var dir in Directory.GetDirectories(topDirPath, dirPattern))
            {
                var outFile = dir + outFilExt;
                outFile.MergeFiles(Directory.GetFiles(dir, filPattern));
            }
        }

        // PATH EXTENSIONS =====================================================
        /// <summary>
        /// Create directories, rename if overwrite == false.
        /// </summary>
        public static string NewPath(this string path, bool overwrite = false)
        {
            var newPath = path;

            if (!overwrite)
                for (int i = 0; File.Exists(newPath); i++)
                    newPath = path.AddBeforeExt("_" + i);

            return newPath;
        }
        /// <summary>
        /// <para>Doesn't change this path.</para>
        /// <para>Replace part from last '.' (dot) to end (DOT INCLUDED) with 'newExt'.</para>
        /// <para>Add 'newExt' if path don't contains any '.' (dot).</para>
        /// <para>Replace max. 5 chars (max. length of extension is 4). Ignore if DOT is at index '0'.</para>
        /// </summary>
        public static string SwitchExt(this string path, string newExt)
        {
            var idx = path.LastIndexOf('.');
            if (idx <= 0 || idx < path.Length - 5)
                return path + newExt;

            var rem = path.Remove(idx);
            return rem.Insert(rem.Length, newExt);
        }
        /// <summary>
        /// return Path.GetRandomFileName();
        /// </summary>
        public static string RandomName()
        {
            return Path.GetRandomFileName();
        }
        /// <summary>
        /// Extract directory path from the full path (path included).
        /// </summary>
        public static string DirPath(this string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (dir == "")
                return ".";
            else
                return dir;
        }
        /// <summary>
        /// Extract directory path from the full path (path included) and create all directories if don't exist.
        /// </summary>
        public static string CreateDir(this string path)
        {
            var dir = path.DirPath();
            if (dir != "." && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }
        /// <summary>
        /// Extract directory name (without path).
        /// </summary>
        public static string DirName(this string path)
        {
            return Path.GetFileName(Path.GetDirectoryName(path));
        }
        /// <summary>
        /// Extract file name (without path)
        /// </summary>
        public static string FileName(this string path, bool withoutExt = false)
        {
            if (withoutExt)
                return Path.GetFileNameWithoutExtension(path);
            else
                return Path.GetFileName(path);
        }
        /// <summary>
        /// If path (this) is 'x.ext', delete all files that match pattern 'x.*'
        /// </summary>
        public static void DeleteAll(this string path, string excPattern = "")
        {
            string withoutExt = Path.GetFileNameWithoutExtension(path);
            foreach (string file in Directory.GetFiles(path.DirPath(), withoutExt + "*"))
                if (excPattern.Length == 0 || !file.Contains(excPattern))
                    File.Delete(file);
        }
        /// <summary>
        /// If path (this) is 'x.ext', delete all files that match pattern 'x.*'
        /// </summary>
        public static void CopyAll(this string path, string newNameWithoutExt, string excPattern = "")
        {
            string withoutExt = Path.GetFileNameWithoutExtension(path);
            foreach (string file in Directory.GetFiles(path.DirPath(), withoutExt + ".*"))
                if (excPattern.Length == 0 || !file.Contains(excPattern))
                    File.Copy(file, newNameWithoutExt + Path.GetExtension(file));
        }
        /// <summary>
        /// If path (this) is 'x.ext', delete all files that match pattern 'x.*'. Overwrite outputs.
        /// </summary>
        public static void CopyAllOver(this string path, string newNameWithoutExt, string excPattern = "")
        {
            newNameWithoutExt.DeleteAll(excPattern);
            path.CopyAll(newNameWithoutExt, excPattern);
        }
        /// <summary>
        /// Add <paramref name="add"/> before extension of file given by <paramref name="path"/>. Original <paramref name="path"/> isn't changed.
        /// </summary>
        public static string AddBeforeExt(this string path, string add)
        {
            var dot = path.LastIndexOf('.');
            var sep = Math.Max(path.LastIndexOf('\\'), path.LastIndexOf('/'));

            if (dot > sep)
                return path.Insert(dot, add);
            else // no extension
                return path + add;
        }
        /// <summary>
        /// Add <paramref name="add"/> before name of file given by <paramref name="path"/>. Original <paramref name="path"/> isn't changed.
        /// </summary>
        public static string AddBeforeName(this string path, string add)
        {
            var sep = Math.Max(path.LastIndexOf('\\'), path.LastIndexOf('/'));
            return path.Insert(sep + 1, add);
        }
    }
}
