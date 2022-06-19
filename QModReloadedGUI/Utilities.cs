using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace QModReloadedGUI
{
    internal static class Utilities
    {
        internal static string UpdateRequestCounts(NameValueCollection responseHeaders, string userName)
        {
            var dailyRemaining = responseHeaders.GetValues("x-rl-daily-remaining")?[0];
            var dailyLimit = responseHeaders.GetValues("x-rl-daily-limit")?[0];
            return $@"{userName}, Daily Requests: {dailyRemaining}/{dailyLimit}";
        }

        public class Validate
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("is_premium")]
            public bool IsPremium { get; set; }

            [JsonPropertyName("profile_url")]
            public string ProfileUrl { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }
        }

        public class PairedKeys
        {
            public byte[] Lock { get; set; }
            public byte[] Vector { get; set; }
        }

        //thanks to Stardrop author for this
        internal class Obscure
        {
            internal byte[] Key { get; set; }
            internal byte[] Vector { get; set; }

            public Obscure()
            {
                using (Aes aes = Aes.Create())
                {
                    Key = aes.Key;
                    Vector = aes.IV;
                }
            }

            internal static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
            {
                byte[] encrypted;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }

                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }

                return encrypted;
            }

            internal static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
            {
                string plaintext = null;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return plaintext;
            }

        }

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