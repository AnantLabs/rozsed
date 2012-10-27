using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace ROZSED.Std
{
    public static class Str
    {
        /// <summary>
        /// Retrun true if entire this instance correspond to pattern. String pattern (1.par) use * and ? wildcard.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern">String pattern, which use * (zero or more times) and ? (one times) wildcard.</param>
        /// <returns>Retrun true if all this instance correspond to pattern.</returns>
        public static bool Match(this string str, string pattern)
        {
            pattern = pattern.Replace("*", ".*").Replace("?", ".");
            return Regex.Match(str, pattern).Value == str;
        }
        /// <summary>
        /// Split this according to delimiter. Using Regex class.
        /// </summary>
        /// <param name="thisStr"></param>
        /// <param name="delimiter">@"\s+" means white character(s)</param>
        public static string[] SplitR(this string thisStr, string delimiter = @"\s+")
        {
            Regex reg = new Regex(delimiter);
            return reg.Split(thisStr);
        }

        // Parsing ================================================================================
        /// <summary>
        /// return double.Parse(this);
        /// </summary>
        public static double Double(this string thisStr)
        {
            return double.Parse(thisStr);
        }
        /// <summary>
        /// return int.Parse(this);
        /// </summary>
        public static int Int(this string thisStr)
        {
            return int.Parse(thisStr);
        }

        // Path ===================================================================================
        /// <summary>
        /// If path doesn't exist append '.ext' a and try again. If doesn't exist retrun false.
        /// </summary>
        /// <param name="path">Full path to shapefile.</param>
        /// <param name="ext">Extension without dot (.)</param>
        public static bool CheckExt(string path, string ext)
        {
            if (File.Exists(path))
                return true;

            if (!path.Match(@"*." + ext))
            {
                path += "." + ext;
                if (File.Exists(path))
                    return true;
            }

            return false;
        }

        // List ===================================================================================
        /// <summary>
        /// Sort list and remove duplicities.
        /// </summary>
        public static void Unique(this List<string> list)
        {
            list.Sort();
            for (int i = 1; i < list.Count; )
            {
                if (list[i] == list[i - 1])
                    list.RemoveAt(i);
                else
                    i++;
            }
        }

        /// <summary>
        /// All characters to lowercase, remove diacritics and replace ' ' to '_'
        /// </summary>
        /// <param name="s"></param>
        public static string SafeName(this string s)
        {
            s = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
                if (CharUnicodeInfo.GetUnicodeCategory(s[i]) != UnicodeCategory.NonSpacingMark) sb.Append(s[i]);
            return sb.ToString().Replace(" ", "_").ToLowerInvariant();
        }
    }
}
