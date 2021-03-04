using System;
using System.Diagnostics;
using System.IO;

namespace TempExtract
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            foreach (string file in args)
            {
                if (File.Exists(file) && file.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
                {
                    var zp = new ZipProcessor(file);
                    string directory = zp.Extract();
                    OpenDirectory(directory);
                }
            }
        }

        private static void OpenDirectory(string directory)
        {
            var psi = new ProcessStartInfo
            {
                FileName = Environment.OSVersion.Platform == PlatformID.Win32NT ? "explorer.exe" : "xdg-open"
            };
            psi.ArgumentList.Add(directory);
            Process.Start(psi);
        }
    }
}
