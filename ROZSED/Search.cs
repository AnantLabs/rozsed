using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ROZSED.Std
{
    public class Search
    {
        /// <summary>
        /// Set position and returns the index within this stream of the first occurrence of the specified byte array. If it is not found, return -1.
        /// </summary>
        /// <param name="haystack">The stream to be scanned.</param>
        /// <param name="needle">The target array to search.</param>
        /// <param name="bufferSize">Length in bytes of reading buffer.</param>
        /// <returns>The start index of the array.</returns>
        public static long IndexOf(Stream stream, byte[] needle, int bufferSize = (int)1e7)
        {
            //var stream = streamReader.BaseStream;
            //byte[] needle = streamReader.CurrentEncoding.GetBytes(sneedle);

            var streamL = stream.Length;
            var needleL = needle.Length;
            var needleLM = needleL - 1;

            if (needleL == 0 || needleL == streamL)
                return 0;
            if (needleL > streamL)
                return -1;

            int[] charTable = MakeCharTable(needle);
            int[] offsetTable = MakeOffsetTable(needle);
            byte[] buffer = new byte[bufferSize];

            int i, j, numBytes, shift = 0;
            while ((numBytes = stream.Read(buffer, 0, bufferSize)) > 0)
            {
                for (i = needleLM; i < numBytes; )
                {
                    for (j = needleLM; needle[j] == buffer[i]; --i, --j)
                        if (j == 0)
                            return stream.Seek(i - numBytes, SeekOrigin.Current);

                    // i += needle.Length - j; // For naive method
                    i += Math.Max(offsetTable[needleLM - j], charTable[buffer[i]]);
                }
                shift = i - bufferSize - needleLM;
                if (shift != 0)
                    stream.Seek(shift, SeekOrigin.Current);
            }
            return -1;
        }
        /// <summary>
        /// Returns the index within this string of the first occurrence of the specified substring. If it is not a substring, return -1.
        /// </summary>
        /// <param name="haystack">The string to be scanned</param>
        /// <param name="needle">The target string to search</param>
        /// <returns>The start index of the substring</returns>
        public static int IndexOf(byte[] haystack, byte[] needle)
        {
            if (needle.Length == 0)
            {
                return 0;
            }
            int[] charTable = MakeCharTable(needle);
            int[] offsetTable = MakeOffsetTable(needle);
            for (int i = needle.Length - 1, j; i < haystack.Length; )
            {
                for (j = needle.Length - 1; needle[j] == haystack[i]; --i, --j)
                {
                    if (j == 0)
                    {
                        return i;
                    }
                }
                // i += needle.Length - j; // For naive method
                i += Math.Max(offsetTable[needle.Length - 1 - j], charTable[haystack[i]]);
            }
            return -1;
        }
        /// <summary>
        /// Makes the jump table based on the mismatched character information.
        /// </summary>
        private static int[] MakeCharTable(byte[] needle)
        {
            const int ALPHABET_SIZE = 256;
            int[] table = new int[ALPHABET_SIZE];
            for (int i = 0; i < table.Length; ++i)
            {
                table[i] = needle.Length;
            }
            for (int i = 0; i < needle.Length - 1; ++i)
            {
                table[needle[i]] = needle.Length - 1 - i;
            }
            return table;
        }
        /// <summary>
        /// Makes the jump table based on the scan offset which mismatch occurs.
        /// </summary>
        private static int[] MakeOffsetTable(byte[] needle)
        {
            int[] table = new int[needle.Length];
            int lastPrefixPosition = needle.Length;
            for (int i = needle.Length - 1; i >= 0; --i)
            {
                if (IsPrefix(needle, i + 1))
                {
                    lastPrefixPosition = i + 1;
                }
                table[needle.Length - 1 - i] = lastPrefixPosition - i + needle.Length - 1;
            }
            for (int i = 0; i < needle.Length - 1; ++i)
            {
                int slen = SuffixLength(needle, i);
                table[slen] = needle.Length - 1 - i + slen;
            }
            return table;
        }
        /// <summary>
        /// Is needle[p:end] a prefix of needle?
        /// </summary>
        private static bool IsPrefix(byte[] needle, int p)
        {
            for (int i = p, j = 0; i < needle.Length; ++i, ++j)
            {
                if (needle[i] != needle[j])
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Returns the maximum length of the substring ends at p and is a suffix.
        /// </summary>
        private static int SuffixLength(byte[] needle, int p)
        {
            int len = 0;
            for (int i = p, j = needle.Length - 1;
                 i >= 0 && needle[i] == needle[j]; --i, --j)
            {
                len += 1;
            }
            return len;
        }
    }
}
