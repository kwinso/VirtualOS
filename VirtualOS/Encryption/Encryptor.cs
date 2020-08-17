using System;
using System.Security.Cryptography;
using System.Text;

namespace VirtualOS.Encryption
{
    public static class Encryptor
    {
        public static string GenerateHash(string plainText)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256.Create()) {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(plainText));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public static bool CompareWithHash(string plainText, string hash)
        {
            var hashedText = GenerateHash(plainText);
            return (hashedText == hash);
        }
    }
}