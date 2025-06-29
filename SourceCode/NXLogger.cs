using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using NXOpenPracticeCSharp;

namespace NXOpenPracticeCSharp
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }

    public class NXLogger : IDisposable
    {
        private static NXLogger _instance;                       // Singleton instance
        private static readonly object _lock = new object();    // Lock for thread safety  

        private StreamWriter _writer;                          // File writer
        private ListingWindow _listingWindow;                 // NX Listing Window
        private string _filePath;                            // Log file path
        private bool _logToFile;                            // Flag: log to file
        private bool _logToListing;                        // Flag: log to NX listing window

        /// <summary>
        /// Private constructor to initialize the logger.
        /// </summary>
        private NXLogger(string logFileBaseName = "NXLog", bool logToFile = true, bool logToListing = true)
        {
            _logToFile = logToFile;
            _logToListing = logToListing;

            // Generate log file path with timestamp
            string tempDir = Environment.GetEnvironmentVariable("TEMP") ?? @"C:\Temp";
            string timestampedFile = $"{logFileBaseName}_{DateTime.Now:ddMMyyy_HHmmss}.txt";
            _filePath = Path.Combine(tempDir, timestampedFile);

            // Initialize StreamWriter for file logging
            if (_logToFile)
            {
                _writer = new StreamWriter(_filePath, append: false) { AutoFlush = true };
            }

            // Initialize NX Listing Window
            if (_logToListing)
            {
                _listingWindow = Session.GetSession().ListingWindow;
                _listingWindow.Open();
            }

            Log("===== NX Logging Started =====", LogLevel.Info);
            LogEnvironmentInfo();

            // Ensure cleanup when NX unloads the DLL
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Dispose();
        }

        /// <summary>
        /// Singleton accessor.
        /// </summary>
        public static NXLogger Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new NXLogger();
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Optional re-initializer (e.g., to change filename or flags).
        /// </summary>
        public static void Init(string fileName = "NXLog", bool logToFile = true, bool logToListing = true)
        {
            lock (_lock)
            {
                if (_instance != null)
                    _instance.Dispose();

                _instance = new NXLogger(fileName, logToFile, logToListing);
            }
        }

        /// <summary>
        /// Logs a message with a given log level.
        /// </summary>
        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string prefix = $"[{timeStamp}] [{level}] ";
            string fullMessage = prefix + message;

            if (_logToFile)
                _writer.WriteLine(fullMessage);

            if (_logToListing)
                _listingWindow?.WriteLine(fullMessage);
        }

        /// <summary>
        /// Logs an exception with detailed information.
        /// </summary>
        public void LogException(Exception ex)
        {
            Log("Exception: " + ex.Message, LogLevel.Error);
            Log("Stack Trace: " + ex.StackTrace, LogLevel.Debug);
        }

        /// <summary>
        /// Writes a separator line (e.g., =======).
        /// </summary>
        public void WriteSeparator(char character = '=', int length = 80)
        {
            string line = new string(character, length);
            if (_logToFile) _writer.WriteLine(line);
            if (_logToListing) _listingWindow?.WriteLine(line);
        }

        /// <summary>
        /// Writes a section title with underline (e.g., ----).
        /// </summary>
        public void WriteSection(string title, char underlineChar = '-')
        {
            Log(title, LogLevel.Info);
            Log(new string(underlineChar, title.Length), LogLevel.Info);
        }

        /// <summary>
        /// Emphasizes a message by surrounding it with separators.
        /// </summary>
        public void WriteImportant(string message)
        {
            WriteSeparator('*');
            Log(message, LogLevel.Warning);
            WriteSeparator('*');
        }

        /// <summary>
        /// Logs user and NX version information.
        /// </summary>
        private void LogEnvironmentInfo()
        {
            string user = Environment.UserName;
            string nxVersion = Session.GetSession().GetEnvironmentVariableValue("UGII_VERSION");
            Log($"User: {user}, NX Version: {nxVersion}", LogLevel.Debug);
        }

        /// <summary>
        /// Returns the log file path.
        /// </summary>
        public string GetLogFilePath()
        {
            return _filePath;
        }

        /// <summary>
        /// Manually flushes the file stream.
        /// </summary>
        public void Flush()
        {
            _writer?.Flush();
        }

        /// <summary>
        /// Disposes resources and finalizes logging.
        /// </summary>
        public void Dispose()
        {
            Log("===== NX Logging Ended =====", LogLevel.Info);
            _writer?.Flush();
            _writer?.Close();
            _writer?.Dispose();
            _writer = null;
            _instance = null;
        }
    }
}

//USE CASE
//########################################################################################
// This is a sample use case for the NXLogger class.
//public static int Main(string[] args)
//{
//    try
//    {
//        NXLogger.Instance.WriteSection("NX Plugin Start");
//        NXLogger.Instance.Log("Processing part data...", LogLevel.Info);

//        var workPart = Session.GetSession().Parts.Work;
//        NXLogger.Instance.Log($"Current Part: {workPart?.Name ?? "None"}", LogLevel.Debug);

//        // Do NX operations here...

//        NXLogger.Instance.WriteSection("NX Plugin End");
//    }
//    catch (Exception ex)
//    {
//        NXLogger.Instance.LogException(ex);
//    }
//    finally
//    {
//        NXLogger.Instance.Dispose();  // Ensure clean shutdown
//    }

//    return 0;
//}

