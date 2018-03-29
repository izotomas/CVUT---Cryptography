using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RC4
{
    public class Generator
    {
        public const string GeneratorPasswordCharSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789[]{}(),.:;/\\!@#$%^&*";
        public const int GeneratorPasswordDefaultLength = 16;
        public const int GeneratorIVDefaultLength = 16;

        public static string GeneratePassword(int length = GeneratorPasswordDefaultLength)
        {
            var builder = new StringBuilder();
            var rand = new RNGCryptoServiceProvider();
            var buffer = new byte[length];
            rand.GetBytes(buffer);
            rand.Dispose();
            foreach (var b in buffer)
            {
                var i = b % GeneratorPasswordCharSet.Length;
                builder.Append(GeneratorPasswordCharSet[i]);
            }
            return builder.ToString();
        }

        public static byte[] GenerateIV(int length = GeneratorIVDefaultLength)
        {
            var rand = new RNGCryptoServiceProvider();
            var buffer = new byte[length];
            rand.GetBytes(buffer);
            rand.Dispose();
            return buffer;
        }

        public static string GenerateIVString(int length = GeneratorIVDefaultLength)
        {
            var iv = GenerateIV(length);
            return new string(iv?.Select(Convert.ToChar).ToArray());
        }
    }
}
