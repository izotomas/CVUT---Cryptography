using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StreamCipherBonus
{
    public class CConverter
    {
        private static readonly Regex RegexForHexValidation = new Regex(@"^([\da-f]{2})+$", RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Converts hexadecimal or ASCII string to byte array 
        /// </summary>
        /// <param name="str">Must by valid hex or ASCII string</param>
        /// <returns></returns>
        public static byte[] ToBytes(string str)
        {
            if (!IsHex(str))
            {
                if (!IsASCII(str)) throw new ArgumentException("argument is not ASCII nor her");
                return Encoding.ASCII.GetBytes(str);
            }
            var buffer = new byte[str.Length / 2];
            for (var i = 0; i < str.Length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
            }

            return buffer;
        }

        public static string ToHexString(byte[] buff)
        {
            return BitConverter.ToString(buff).Replace("-", "").ToLowerInvariant();
        }
        
        public static string ToHexString(string str)
        {
            return BitConverter.ToString(Encoding.ASCII.GetBytes(str)).Replace("-","").ToLowerInvariant();
        }

        public static string ToASCIIString(byte[] buff)
        {
            return Encoding.ASCII.GetString(buff);
        }

        public static byte[] XOR(byte[] a, byte[] b, int offset = 0)
        {
            var longer = a.Length >= b.Length ? a : b;
            var shorter = a.Length < b.Length ? a : b;
            var buff = new byte[longer.Length];
            Array.Copy(longer, buff, longer.Length);
            longer = buff;
            for (var i = 0; i < shorter.Length; i++)
            {
                longer[i+offset] ^= shorter[i];
            }
            return longer;
        }

        #region Helpers

        private static bool IsHex(string input)
        {
            var match = RegexForHexValidation.Match(input);
            return match.Success;
        }

        private static bool IsASCII(string input)
        {
            return Encoding.UTF8.GetByteCount(input) == input.Length;
        }
        
        #endregion
    }
}