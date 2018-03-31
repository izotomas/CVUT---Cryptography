using System;
using System.Collections;
using RC4;

namespace Cracker
{
    public class RC4Cracker 
    {
        private RC4Cracker() { }

        public static byte[] Decrypt(string knownPlainText, string knownHexCipherText, string unknownCipherText)
        {
            if (!AreValidParameters(knownPlainText, knownHexCipherText, unknownCipherText))
            {
                throw new ArgumentException("Both CTs must be in hex format, their length must match and the length of PT must be half of its CT");
            }
            var buffer = KnownPlainTextAttack(knownPlainText, knownHexCipherText, unknownCipherText);
            return buffer;
        }

        public static string DecryptToStr(string knownPlainText, string knownCipherText, string unknownCipherText)
        {
            var bytes = Decrypt(knownPlainText, knownCipherText, unknownCipherText);
            var ptStr = Utility.ConvertToString(bytes);
            return ptStr;
        }

        private static byte[] KnownPlainTextAttack(string pt, string ct, string unkwnown)
        {
            var buffer = new byte[pt.Length];
            var a = new BitArray(Utility.ConvertToBytes(pt));
            var b = new BitArray(Utility.ConvertHexToBytes(ct));
            var c = new BitArray(Utility.ConvertHexToBytes(unkwnown));
            var cracked = c.Xor(a).Xor(b);
            cracked.CopyTo(buffer, 0);
            return buffer;
        }

        private static bool AreValidParameters(string pt, string ct, string unknown)
        {
            return AreMatchingLengths(pt, ct, unknown) && Utility.IsHex(ct) && Utility.IsHex(unknown);
        }

        private static bool AreMatchingLengths(string pt, string ct1, string ct2)
        {
            return ct1.Length == ct2.Length && ct1.Length == 2 * pt.Length;
        }
    }
}
