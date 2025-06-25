using System;
using System.Collections.Generic;
using System.IO;

namespace NXOpenSetUPCSharp
{
    public class FileManager
    {
        /// <summary>
        /// Check wether a directory exists. If it does not, throws a DirectoryNotFoundException.
        /// </summary>
        /// <param name="directory">full path of the directory</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void checkDirectoryExistance(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException($"Directory does not exist: {directory}");
            }
        }

        /// <summary>
        /// Ensures that the specified directory exists, creating it if it does not.
        /// </summary>
        /// <remarks>If the directory does not exist, this method attempts to create it.  If an error
        /// occurs during directory creation, the error is logged using the <c>NXLogger</c> instance, if
        /// available.</remarks>
        /// <param name="directory">The path of the directory to check and create if it does not exist. Cannot be null or empty.</param>
        public static void CheckDirectoryExistanceCreateIfNot(string directory)
        {
            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception ex)
                {
                    // You can optionally log or rethrow depending on your handling policy
                    NXLogger.Instance?.Log($"Error creating directory: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Checks whether the specified file exists at the given path.
        /// </summary>
        /// <param name="filePath">The full path of the file to check. This cannot be null or empty.</param>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist at the specified path.</exception>
        public static void CheckFileExistance(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File does not exist: {filePath}");
            }
        }

        /// <summary>
        /// Deletes all files in the specified directory.
        /// </summary>
        /// <param name="directory">Directory whose files are to be deleted.</param>
        public static void DeleteFilesInDirectory(string directory)
        {
            try
            {
                checkDirectoryExistance(directory);
                foreach (string file in Directory.GetFiles(directory))
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                // You can optionally log or rethrow depending on your handling policy
                NXLogger.Instance?.Log($"Error deleting files: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a directory. By default, deletes all subdirectories and files inside it.
        /// </summary>
        /// <param name="path">Full path of the directory to delete.</param>
        /// <param name="recursive">If true, deletes all contents inside (default is true).</param>
        public static void DeleteDirectory(string path, bool recursive = true)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive);
        }

        /// <summary>
        /// Returns a list of file paths in the given directory, optionally filtered by search pattern.
        /// </summary>
        /// <param name="path">Full path to the directory.</param>
        /// <param name="searchPattern">File search pattern (e.g., "*.txt", default is "*.*").</param>
        /// <returns>List of full file paths.</returns>
        public static List<string> GetFilesInDirectory(string path, string searchPattern = "*.*")
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("Directory not found: " + path);

            return new List<string>(Directory.GetFiles(path, searchPattern));
        }

        /// <summary>
        /// Copies all files from a source directory to a destination directory.
        /// </summary>
        /// <param name="sourceDir">Full path of the source directory.</param>
        /// <param name="destDir">Full path of the destination directory.</param>
        /// <param name="searchPattern">Optional file pattern (e.g., "*.txt"). Default is "*.*" (all files).</param>
        /// <param name="overwrite">Whether to overwrite files if they already exist in destination. Default is true.</param>
        public static void CopyAllFiles(string sourceDir, string destDir, string searchPattern = "*.*", bool overwrite = true)
        {
            if (!Directory.Exists(sourceDir))
                throw new DirectoryNotFoundException("Source directory not found: " + sourceDir);

            // Create destination directory if it doesn't exist
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            // Get all files matching the pattern
            string[] files = Directory.GetFiles(sourceDir, searchPattern);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destPath = Path.Combine(destDir, fileName);

                // Copy the file
                File.Copy(file, destPath, overwrite);
            }
        }

        // ---------------- TEXT FILE ----------------

        /// <summary>
        /// Writes the specified content to a text file at the given path.
        /// If the file does not exist, it will be created.
        /// </summary>
        /// <param name="path">Full path to the text file.</param>
        /// <param name="content">Content to write to the file.</param>
        public static void WriteTextFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        /// <summary>
        /// Reads and returns the entire content of a text file.
        /// </summary>
        /// <param name="path">Full path to the text file.</param>
        /// <returns>String content of the file.</returns>
        public static string ReadTextFile(string path)
        {
            return File.ReadAllText(path);
        }
        // ---------------- CSV FILE ----------------

        /// <summary>
        /// Writes data to a CSV file.
        /// Each inner string array represents a row.
        /// </summary>
        /// <param name="path">Full path to the CSV file.</param>
        /// <param name="rows">List of rows, each represented as a string array.</param>
        /// <param name="delimiter">Delimiter to separate values (default is comma).</param>
        public static void WriteCsvFile(string path, List<string[]> rows, char delimiter = ',')
        {
            using (var writer = new StreamWriter(path))
            {
                foreach (var row in rows)
                {
                    writer.WriteLine(string.Join(delimiter.ToString(), row));
                }
            }
        }

        /// <summary>
        /// Reads data from a CSV file and returns it as a list of string arrays.
        /// Each array is a row, split by the given delimiter.
        /// </summary>
        /// <param name="path">Full path to the CSV file.</param>
        /// <param name="delimiter">Delimiter used in the file (default is comma).</param>
        /// <returns>List of rows, each row as a string array.</returns>
        public static List<string[]> ReadCsvFile(string path, char delimiter = ',')
        {
            var result = new List<string[]>();
            foreach (var line in File.ReadLines(path))
            {
                result.Add(line.Split(delimiter));
            }
            return result;
        }

    }
}
