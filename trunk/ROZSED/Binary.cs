using System;
using System.IO;

namespace ROZSED.Std
{
    public static class Binary
    {
        /// <summary>
        /// Reads a 4-byte signed integer using the big-endian layout 
        /// from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        public static int ReadInt32BE(this BinaryReader reader)
        {
            byte[] byteArray = new byte[4];
            int iBytesRead = reader.Read(byteArray, 0, 4);
            System.Array.Reverse(byteArray);
            return BitConverter.ToInt32(byteArray, 0);
        }
    }
}
