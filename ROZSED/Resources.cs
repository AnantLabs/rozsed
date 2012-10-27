using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace ROZSED.Std
{
    public class Resources
    {
        /// <summary>
        /// Extract embedded resource to (default hidden) file on disk if does't exists.
        /// </summary>
        /// <param name="pathInAssembly"></param>
        /// <param name="extractToPath"></param>
        public static void Extract(string pathInAssembly, string extractToPath, bool hide = true)
        {
            if (File.Exists(extractToPath))
                return;

            using (Stream stm = Assembly.GetExecutingAssembly().GetManifestResourceStream(pathInAssembly))
            {
                using (Stream outFile = File.Create(extractToPath))
                {
                    const int sz = 4096;
                    byte[] buf = new byte[sz];
                    int nRead;
                    while (true)
                    {
                        nRead = stm.Read(buf, 0, sz);
                        if (nRead < 1)
                            break;
                        outFile.Write(buf, 0, nRead);
                    }
                }
            }
            if (hide)
                File.SetAttributes(extractToPath, FileAttributes.Hidden);
        }
    }
}
