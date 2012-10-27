using System;
using System.IO;
using System.Reflection;

namespace ROZSED.Std
{
    /// <summary>
    /// Perform actions like time control and loging.
    /// </summary>
    public static class Ctrl
    {
        internal static StreamWriter errorLogStream = null;
        internal static StreamWriter warningLogStream = null;
        public static bool writeConsole = true;

        static Action endAction = null;
        static readonly DateTime startApp = DateTime.Now; // čas 'startApp' nastaven na čas prvního použití, slouží k měření trvání celé aplikace
        static DateTime start = DateTime.Now; // čas 'start' nastaven na čas prvního použití této třídy, může být však přepsán novým
        static int cursorLeft, cursorTop, tempCursorLeft, tempCursorTop;
        static bool cursorStored;

        /// <summary>
        /// Create TextWriter instance for using WriteLine method of this class.
        /// </summary>
        /// <param name="errorLogPath">Full path to txt file for writing error log information.</param>
        /// <param name="warningLogPath">Full path to txt file for writing warning log information. You can use the same like errorLogPath.</param>
        /// <param name="append">If false, log file will be recreated.</param>
        public static void OpenLogs(string errorLogPath, string warningLogPath = null, bool append = false)
        {
            if (errorLogPath != null && errorLogPath != "")
                try
                {
                    errorLogStream = new StreamWriter(errorLogPath, append);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("ERROR: Can't open or create log file:");
                    Console.Error.WriteLine(errorLogPath);
                    Console.Error.WriteLine("    " + e.Message);
                    EndApp();
                }

            if (errorLogPath == warningLogPath)
                warningLogPath = errorLogPath;
            else if (warningLogPath != null && warningLogPath != "")
                try
                {
                    warningLogStream = new StreamWriter(warningLogPath, append);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("ERROR: Can't open or create log file:");
                    Console.Error.WriteLine(warningLogPath);
                    Console.Error.WriteLine("    " + e.Message);
                    EndApp();
                }
        }
        /// <summary>
        /// Close TextWriter member, which was created by BeginLogging
        /// </summary>
        public static void CloseLogs()
        {
            if (errorLogStream != null)
            {
                errorLogStream.Close();
                errorLogStream = null;
            }
            if (warningLogStream != null)
            {
                warningLogStream.Close();
                warningLogStream = null;
            }
        }
        /// <summary>
        /// Asking user: Continue, or exit console?
        /// </summary>
        public static void ContinueDialog()
        {
            Console.Error.Write("Continue? (y/n) ");
            if (YesNoQuestion())
            {
                Console.Error.WriteLine();
                return;
            }
            else
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("Application exit by user.");
                EndApp(false, true);
            }
        }
        /// <summary>
        /// Asking user: 'y' or 'n'?
        /// </summary>
        public static bool YesNoQuestion()
        {
            ConsoleKeyInfo key;
            while (true)
            {
                key = Console.ReadKey();
                if (key.KeyChar == 'y')
                    return true;
                if (key.KeyChar == 'n')
                    return false;
                Console.Error.Write("\nPress 'y' or 'n': ");
            }
        }

        #region Rewrite
        /// <summary>
        /// Store the cursor position for rewrite Console
        /// </summary>
        public static void StoreCursor()
        {
            cursorLeft = Console.CursorLeft;
            cursorTop = Console.CursorTop == 299 ? 298 : Console.CursorTop;
            cursorStored = true;
        }
        static void setCursor()
        {
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }
        static void storeTempCursor()
        {
            tempCursorLeft = Console.CursorLeft;
            tempCursorTop = Console.CursorTop == 299 ? 298 : Console.CursorTop;
        }
        static void setTempCursor() 
        {
            Console.SetCursorPosition(tempCursorLeft, tempCursorTop);
        }
        /// <summary>
        /// <para>Write line from stored cursor position (if stored) and then set back to position like was before call this method.</para>
        /// <para>storeTempCursor(); setCursor(); Console.WriteLine(str, par); setTempCursor();</para>
        /// </summary>
        public static void Rewrite(string str, params object[] par)
        {
            storeTempCursor();
            if (cursorStored) setCursor();
            Console.WriteLine(str, par);
            setTempCursor();
        }
        /// <summary>
        /// StoreCursor(); Console.WriteLine(str, par); setCursor();
        /// </summary>
        public static void TempWrite(string str, params object[] par)
        {
            StoreCursor();
            Console.WriteLine(str, par);
            setCursor();
        }
        /// <summary>
        /// StoreCursor(); Console.WriteLine(obj); setCursor();
        /// </summary>
        public static void TempWrite(object obj)
        {
            if (writeConsole)
            {
                StoreCursor();
                Console.WriteLine(obj);
                setCursor();
            }
        }
        #endregion
        #region Writing to Console and to log files
        /// <summary>
        /// Writes to Console.Out and all logs.
        /// </summary>
        public static void WriteLine(string str, params object[] par)
        {
            if (writeConsole)
                Console.WriteLine(str, par);

            if (errorLogStream != null)
            {
                errorLogStream.WriteLine(str, par);
                errorLogStream.Flush();
            }

            if (warningLogStream != null)
            {
                warningLogStream.WriteLine(str, par);
                warningLogStream.Flush();
            }
        }
        /// <summary>
        /// Writes to Console.Out and all logs.
        /// </summary>
        public static void WriteLine(string str = "")
        {
            if (writeConsole)
                Console.WriteLine(str);

            if (errorLogStream != null)
            {
                errorLogStream.WriteLine(str);
                errorLogStream.Flush();
            }

            if (warningLogStream != null)
            {
                warningLogStream.WriteLine(str);
                warningLogStream.Flush();
            }
        }
        /// <summary>
        /// Writes to Console.Out and all logs.
        /// </summary>
        public static void WriteLine(object obj)
        {
            WriteLine(obj.ToString());
        }
        /// <summary>
        /// Writes to Console.Out and all logs.
        /// </summary>
        public static void Write(string str, params object[] par)
        {
            if (writeConsole)
                Console.Write(str, par);

            if (errorLogStream != null)
            {
                errorLogStream.Write(str, par);
                errorLogStream.Flush();
            }

            if (warningLogStream != null)
            {
                warningLogStream.Write(str, par);
                warningLogStream.Flush();
            }
        }

        /// <summary>
        /// Writes line to Console.Out and to warning log.
        /// </summary>
        public static void WriteWarning(string str, params object[] par)
        {
            if (writeConsole)
                Console.WriteLine(str, par);

            if (warningLogStream != null)
            {
                warningLogStream.WriteLine(str, par);
                warningLogStream.Flush();
            }
        }
        /// <summary>
        /// Writes line to Console.Out and to warning log.
        /// </summary>
        public static void WriteWarning(string str = "")
        {
            if (writeConsole)
                Console.WriteLine(str);

            if (warningLogStream != null)
            {
                warningLogStream.WriteLine(str);
                warningLogStream.Flush();
            }
        }
        /// <summary>
        /// Writes line to Console.Out and to warning log.
        /// </summary>
        public static void WriteWarning(object obj)
        {
            WriteWarning(obj.ToString());
        }
        /// <summary>
        /// Writes line to warning log only.
        /// </summary>
        public static void WriteWarningOnly(string str, params object[] par)
        {
            if (warningLogStream != null)
            {
                warningLogStream.WriteLine(str, par);
                warningLogStream.Flush();
            }
        }
        /// <summary>
        /// Writes line to Console.Error and to error log.
        /// </summary>
        public static void WriteError(string str, params object[] par)
        {
            if (writeConsole)
                Console.Error.WriteLine(str, par);

            if (errorLogStream != null)
            {
                errorLogStream.WriteLine(str, par);
                errorLogStream.Flush();
            }
        }
        /// <summary>
        /// Writes line to Console.Error and to error log.
        /// </summary>
        public static void WriteErrorOnly(string str, params object[] par)
        {
            if (errorLogStream != null)
            {
                errorLogStream.WriteLine(str, par);
                errorLogStream.Flush();
            }
        }
        /// <summary>
        /// Write contents of file to console and all logs.
        /// </summary>
        public static void WriteFile(string path)
        {
            var file = new StreamReader(path);
            Write(file.ReadToEnd());
            WriteLine();
        }
        /// <summary>
        /// Write contents of object to stream.
        /// </summary>
        public static void WriteXML<T>(T obj, StreamWriter toStream)
        {
            obj.ToXML(toStream);
            toStream.WriteLine();
            toStream.Flush();
        }
        /// <summary>
        /// Write contents of object errorLogStream.
        /// </summary>
        public static void WriteXML<T>(T obj)
        {
            obj.ToXML(errorLogStream);
            errorLogStream.WriteLine();
            errorLogStream.Flush();
        }
        #endregion
        #region Time measuring
        /// <summary>
        /// Set start member to DateTime.Now. This member is used in ToNow() method.
        /// </summary>
        public static string SetStart()
        {
            start = DateTime.Now;
            return ToNow();
        }
        /// <summary>
        /// Rerutn String represents TimeSpan (duration) from start member of this class to DateTime.Now divided by 'divide' (if divide isn't zero) in format: [D.HH:]mm:ss.ttt
        /// </summary>
        public static string ToNow(double divide = 0)
        {
            return start.ToNow(divide);
        }
        #endregion

        /// <summary>
        /// Write assembly name, version and start time.
        /// </summary>
        public static void BeginApp(bool toLogOnly = false)
        {
            var temp = writeConsole;
            if (toLogOnly)
                writeConsole = false;

            WriteLine("Assembly: {0} {1}, Started: {2}", Assembly.GetEntryAssembly().GetName().Name, Assembly.GetEntryAssembly().GetName().Version, startApp);
            WriteLine("===============================================================================");
            WriteLine();

            writeConsole = temp;
        }
        /// <summary>
        /// Sets action invoked on EndApp() will be called.
        /// </summary>
        public static void EndAction(Action endInvokedAction)
        {
            endAction = endInvokedAction;
        }
        /// <summary>
        /// <para>Summarize actions performed on end of the program and end application by Environment.Exit(0)</para>
        /// <para>You can close the application anywhere in the code.</para>
        /// </summary>
        public static void EndApp(bool keyPress = false, bool exitApp = true)
        {
            WriteLine();
            WriteLine("===============================================================================");
            WriteLine("Total duration: {0}, Time: {1}", startApp.ToNow(), DateTime.Now);

            CloseLogs();

            if (keyPress)
            {
                Console.Error.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }

            if (endAction != null)
                endAction.Invoke();

            if (exitApp)
                System.Environment.Exit(0);
        }
    }
}
