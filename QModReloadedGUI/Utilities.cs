using System;
using System.IO;
using System.Security.Cryptography;

namespace QModReloadedGUI
{
    internal static class Utilities
    {

        public static string CalculateMd5(string file)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(file);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public static void WriteLog(string message, string gameLocation)
        {
            using var streamWriter = new StreamWriter(Path.Combine(gameLocation, "qmod_reloaded_log.txt"),
                true);
            streamWriter.WriteLine(message);
        }
    }
}
