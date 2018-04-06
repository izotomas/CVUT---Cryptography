using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace RC4
{
    public class Utility
    {
        public const string GeneratedPasswordCharSet =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789[]{}(),.:;/\\!@#$%^&*";

        public const int DefaultGeneratedPasswordLength = 16;
        public const int DefaultGeneratedIVLength = 16;

        private static readonly Regex RegexForHexValidation = new Regex(@"^([\da-f]{2})+$", RegexOptions.IgnoreCase);

        private Utility()
        {
        }

        public static string GeneratePassword(int length = DefaultGeneratedPasswordLength)
        {
            var builder = new StringBuilder();
            var rand = new RNGCryptoServiceProvider();
            var buffer = new byte[length];
            rand.GetBytes(buffer);
            rand.Dispose();
            foreach (var b in buffer)
            {
                var i = b % GeneratedPasswordCharSet.Length;
                builder.Append(GeneratedPasswordCharSet[i]);
            }

            return builder.ToString();
        }

        public static byte[] GenerateIV(int length = DefaultGeneratedIVLength)
        {
            var rand = new RNGCryptoServiceProvider();
            var buffer = new byte[length];
            rand.GetBytes(buffer);
            rand.Dispose();
            return buffer;
        }

        public static byte[] ConvertToBytes(string text)
        {
            return text?.Select(Convert.ToByte).ToArray();
        }

        public static byte[] ConvertHexToBytes(string text)
        {
            if (!IsHex(text)) throw new ArgumentException("Input is not in HEX format", text);
            var buffer = new byte[text.Length / 2];
            for (var i = 0; i < text.Length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(text.Substring(i, 2), 16);
            }

            return buffer;
        }

        public static string ConvertToString(byte[] bytes)
        {
            return new string(bytes?.Select(Convert.ToChar).ToArray());
        }

        public static string ConvertToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        public static bool IsHex(string input)
        {
            var match = RegexForHexValidation.Match(input);
            return match.Success;
        }
    }
}
