using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace ROZSED.Std
{
    /// <summary>
    /// <para>Overwrite this class to create settings for program. See example Settings class in this file.</para>
    /// <pre><code>
    /// public class Settings : CMDLineOptions
    /// {
    ///     public bool check;
    ///     public double maxArea = double.MaxValue;
    /// 
    ///     protected override bool Options()
    ///     {
    ///         if (arg == "-overwrite")
    ///             overwrite = true;
    ///         else if (arg == "-check")
    ///             check = true;
    ///         else if (arg.StartsWith("-maxArea"))
    ///             return GetDouble(out maxArea);
    ///         else
    ///             return false;
    ///         return true;
    ///     }
    /// }
    /// </code></pre>
    /// </summary>
    public class CMDLineOptions
    {
        [Obsolete("Use Options() method instead.")]
        public List<string> inputs = new List<string>();
        [Obsolete("Use Options() method instead.")]
        public List<string> outputs = new List<string>();
        public bool overwrite = false, quiet = false;
        public TextWriter log = Console.Error;
        protected int _i = 0;
        protected string[] _args;
        protected string _arg;

        /// <summary>
        /// Name of this program.
        /// </summary>
        public string Name
        {
            get { return Assembly.GetEntryAssembly().GetName().Name; }
        }
        /// <summary>
        /// Path to this executable program.
        /// </summary>
        public string Path
        {
            get { return Assembly.GetEntryAssembly().Location; }
        }
        /// <summary>
        /// Parse only parameters starts with '-' according to Options() method.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="usage"></param>
        public bool Read(string[] args, Action usage = null)
        {
            this._args = args;

            if (usage == null)
                usage = delegate { Usage(); };

            for (_i = 0; _i < args.Length; _i++)
            {
                _arg = args[_i];
                if (_arg.StartsWith("-"))
                {
                    if (Options())
                        continue;

                    if (_arg == "-help" || _arg == "--help")
                    {
                        usage.Invoke();
                        return false;
                    }

                    if (_arg == "-overwrite" || _arg == "--overwrite")
                        overwrite = true;
                    else if (_arg == "-q" || _arg == "-quiet" || _arg == "--q" || _arg == "--quiet")
                        quiet = true;
                    else
                    {
                        log.WriteLine("Can't parse argument: " + _arg);
                        usage.Invoke();
                        log.Flush();
                        return false;
                    }
                }
                else
                {
                    log.WriteLine("Can't parse argument: " + _arg);
                    usage.Invoke();
                    log.Flush();
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Read argument with (by Options() method) or without '-' (in count defined here, first fill and check inputs).
        /// </summary>
        /// <param name="args"></param>
        /// <param name="minInputs"></param>
        /// <param name="minOutputs"></param>
        /// <param name="maxInOut"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        public bool Read(string[] args, int minInputs, int minOutputs = -1, int maxInOut = -1, Action usage = null)
        {
            this._args = args;

            if (usage == null)
                usage = delegate { Usage(); };

            for (_i = 0; _i < args.Length; _i++)
            {
                _arg = args[_i];
                if (_arg.StartsWith("-"))
                {
                    if (_arg == "-help" || _arg == "--help")
                    {
                        usage.Invoke();
                        return false;
                    }
                    if (_arg == "-overwrite" || _arg == "--overwrite")
                    {
                        overwrite = true;
                        continue;
                    }
                    if (_arg == "-q" || _arg == "-quiet" || _arg == "--q" || _arg == "--quiet")
                    {
                        quiet = true;
                        continue;
                    }

                    if (!Options())
                    {
                        log.WriteLine("Can't parse argument: " + _arg);
                        usage.Invoke();
                        return false;
                    }
                }
                else if (maxInOut-- == 0)
                {
                    log.WriteLine("Too many input argumets.");
                    usage.Invoke();
                    return false;
                }
                else if (minInputs != 0)
                {
                    if (CheckInput(_arg))
                    {
                        inputs.Add(_arg);
                        minInputs--;
                    }
                    else
                        return false;
                }
                else
                {
                    outputs.Add(_arg);
                    minOutputs--;
                }
            }

            if (minInputs > 0 || minOutputs > 0)
            {
                log.WriteLine("Too few input arguments.");
                usage.Invoke();
                return false;
            }

            foreach (var output in outputs)
                if (!CheckOutputFile(output))
                    return false;

            return true;
        }
        /// <summary>
        /// Write line to log stream and flush it.
        /// </summary>
        public void WriteLine(string line)
        {
            log.WriteLine(line);
            log.Flush();
        }
        /// <summary>
        /// Say if current argument can be parsed.
        /// </summary>
        protected virtual bool Options()
        {
            return false;
        }
        /// <summary>
        /// Say if current argument match 'arg'. If arg doesn't start with '-' insert one.
        /// </summary>
        protected bool IsArg(string arg)
        {
            if (!arg.StartsWith("-"))
                arg = "-" + arg;
            return _arg.StartsWith(arg + "=") || _arg == arg;
        }
        /// <summary>
        /// Writes usage to standard output (Console class).
        /// </summary>
        public virtual void Usage() { }
        /// <summary>
        /// First try this is directory, then this is file. Adds '\\' at end of 'arg' if 'arg' is directory and without '\\' at end.
        /// </summary>
        public virtual bool CheckInput(string arg)
        {
            if (Directory.Exists(arg))
            {
                if (!arg.EndsWith("\\") && !arg.EndsWith("/"))
                    arg = arg.Insert(arg.Length, "\\");
                return true;
            }
            else if (File.Exists(arg))
            {
                return true;
            }
            else
            {
                log.WriteLine("Input doesn't exist: " + arg);
                return false;
            }
        }
        /// <summary>
        /// Adds '\\' at end of 'arg' if 'arg' is directory and without '\\' at end. Return false if directory doesn't exists.
        /// </summary>
        public virtual bool CheckInputDir(ref string arg)
        {
            if (arg == null || arg == "")
            {
                log.WriteLine("You must specify input directory.");
                Usage();
                return false;
            }
            if (Directory.Exists(arg))
            {
                if (!arg.EndsWith("\\") && !arg.EndsWith("/"))
                    arg = arg.Insert(arg.Length, "\\");
                return true;
            }
            else
            {
                log.WriteLine("Input directory doesn't exist: " + arg);
                log.Flush();
                return false;
            }
        }
        /// <summary>
        /// Ask for existing of file.
        /// </summary>
        public virtual bool CheckInputFile(string arg)
        {
            if (File.Exists(arg))
            {
                return true;
            }
            else
            {
                log.WriteLine("Input file doesn't exist: " + arg);
                return false;
            }
        }
        /// <summary>
        /// Adds backflash if is not on the end. Then try create directory. If throw return false (e.g non valid path).
        /// </summary>
        /// <param name="outDir"></param>
        public virtual bool CheckOutputDir(ref string outDir)
        {
            if (outDir == null || outDir == "")
            {
                log.WriteLine("You must specify output directory.");
                Usage();
                return false;
            }

            if (!outDir.EndsWith("\\") && !outDir.EndsWith("/"))
                outDir = outDir.Insert(outDir.Length, "\\");

            try { Directory.CreateDirectory(outDir); }
            catch
            {
                log.WriteLine("Cannot create output directory: " + outDir);
                return false;
            }

            return true;
        }
        /// <summary>
        /// If file exists, check parametr overwrite and try writting. If file doesn't exists create directories on path.
        /// </summary>
        /// <param name="output"></param>
        public virtual bool CheckOutputFile(string output)
        {
            if (File.Exists(output))
            {
                if (!overwrite)
                {
                    log.WriteLine("Output file already exists. Use -overwrite if you want to overwrite it.");
                    log.WriteLine("File: " + output);
                    return false;
                }
                FileStream file = null;
                try
                {
                    file = File.OpenWrite(output);
                }
                catch (Exception e)
                {
                    log.WriteLine("Can't open file for writting.");
                    log.WriteLine("Message: " + e.Message);
                    return false;
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }
            else
                Fls.CreateDir(output);

            return true;
        }
        /// <summary>
        /// Parse this type of argumets A) -arg=0.0 B) -arg 0.0
        /// </summary>
        protected virtual bool GetDouble(out double outVal)
        {
            return double.TryParse(GetString(), out outVal);
        }
        /// <summary>
        /// Parse this type of argumets A) -arg=0 B) -arg 0
        /// </summary>
        protected virtual bool GetInt(out int outVal)
        {
            return int.TryParse(GetString(), out outVal);
        }
        /// <summary>
        /// Parse this type of argumets A) -arg=value B) -arg value C) arg=
        /// </summary>
        protected virtual string GetString()
        {
            var i = _arg.IndexOf("=");
            if (i < 0)
                return RemoveQuotes(_args[++_i]);
            else if (++i == _arg.Length)
                return "";
            else
                return _arg.Substring(i, _arg.Length - i);
        }
        /// <summary>
        /// Parse this type of argumets A) -arg=value1,value2 B) -arg value1 value2 C) -arg=
        /// </summary>
        protected virtual List<string> GetStrings(string splitPattern = ",")
        {
            var list = new List<string>();
            var i = _arg.IndexOf("=");
            if (i < 0)
            {
                while (++_i < _args.Length && !_args[_i].StartsWith("-"))
                    list.Add(RemoveQuotes(_args[_i]));
                _i--;
            }
            else if (++i != _arg.Length)
                list = Regex.Split(_arg.Substring(i, _arg.Length - i), splitPattern, RegexOptions.Compiled).ToList(x => RemoveQuotes(x)); ;

            return list;
        }
        /// <summary>
        /// Parse this type of argumets A) -arg=value1,value2 B) -arg value1 value2 C) -arg=
        /// </summary>
        protected virtual bool GetDoubles(out List<double> doubles, string splitPattern = ",")
        {
            doubles = new List<double>();
            double num;

            foreach(var str in GetStrings(splitPattern))
            {
                if(!double.TryParse(str, out num))
                    return false;
                doubles.Add(num);
            }
            return true;
        }
        /// <summary>
        /// return Regex.Replace(str, @"^['""]?(.*?)['""]?$", "$1", RegexOptions.Compiled);
        /// </summary>
        public static string RemoveQuotes(string str)
        {
            return Regex.Replace(str, @"^['""]?(.*?)['""]?$", "$1", RegexOptions.Compiled);
        }
    }
    /// <summary>
    /// Example setting class
    /// </summary>
    class Settings : CMDLineOptions
    {
        public bool check;
        public double value;
        public List<string> skipPatterns;

        protected override bool Options()
        {
            if (IsArg("-check"))
                check = true;
            else if (IsArg("-value"))
                return GetDouble(out value);
            else if (_arg.StartsWith("-skip"))
                foreach (Match match in Regex.Matches(_arg, "'(.*?)'"))
                {
                    var pattern = match.Groups[1].ToString();
                    if (pattern.Length != 0)
                        skipPatterns.Add(pattern.Replace("*", ".*").Replace("?", "."));
                }
            else
                return false;
            return true;
        }
        public override void Usage()
        {
            Console.WriteLine(
"Usage: " + Name + @".exe [-help] [-overwrite] [-quiet]

-overwrite   : overwrite output if exists
-quiet       : no outputs to console
-help        : show this help

support      : martin.setnicka@seznam.cz");
        }
    }
}
