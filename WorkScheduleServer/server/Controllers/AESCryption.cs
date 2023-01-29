using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WorkScheduleServer.Controllers
{
    public static class AESCryption
    {
        private const string AES_IV = @"2V599708dBEn3114";
        private const string AES_Key = @"7wF37X63f6k7Tzwx";

        /// <summary>
        /// AES暗号を使って文字列を暗号化する
        /// </summary>
        /// <param name="text">暗号化する文字列</param>
        /// <param name="iv">対称アルゴリズムの初期ベクター</param>
        /// <param name="key">対称アルゴリズムの共有鍵</param>
        /// <returns>暗号化された文字列</returns>
        public static string Encrypt(string text, string iv = AES_IV, string key = AES_Key)
        {
            // Create an Aes object
            // with the specified key and IV.
            using var aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            // Create an encryptor to perform the stream transform.
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                //Write all data to the stream.
                swEncrypt.Write(text);
            }
            var encrypted = msEncrypt.ToArray();

            // Return the encrypted string from the memory stream.
            return Convert.ToBase64String(encrypted); 
        }

        /// <summary>
        /// AES鍵暗号を使って暗号文を復号する
        /// </summary>
        /// <param name="cipher">暗号化された文字列</param>
        /// <param name="iv">対称アルゴリズムの初期ベクター</param>
        /// <param name="key">対称アルゴリズムの共有鍵</param>
        /// <returns>復号された文字列</returns>
        public static string Decrypt(string cipher, string iv = AES_IV, string key = AES_Key)
        {
            // Create an Aes object
            // with the specified key and IV.
            using var aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            // Create a decryptor to perform the stream transform.
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using var msDecrypt = new MemoryStream(Convert.FromBase64String(cipher));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            // Read the decrypted bytes from the decrypting stream
            // and place them in a string.
            string plaintext = srDecrypt.ReadToEnd();

            return plaintext;
        }
    }
}
