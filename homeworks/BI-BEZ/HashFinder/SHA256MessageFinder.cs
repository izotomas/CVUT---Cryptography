using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace HashFinder
{
    // Task: find a message such that it's hash (SHA-256) starts with bytes 0xAA,0xBB
    public class SHA256MessageFinder
    {
        public static readonly int MaxBufferLength = int.MaxValue;

        private SHA256MessageFinder()
        {
        }

        public static byte[] LookForMessageWithHashPattern(string fileName, params string[] startingPattern)
        {
            var result = LookForMessageWithHashPattern(startingPattern);

            if (result != null)
            {
                if (fileName != null)
                {
                    try
                    {
                        File.WriteAllBytes(fileName, result);
                        Console.WriteLine("Match (ASCII) Written to : {0}", fileName);
                    }
                    catch (IOException e)
                    {
                        Console.Error.WriteLine("Error writing to file \"{0}\"\n{1}", fileName, e);
                    }
                }
                return result;
            }
            return null;
        }

        private static byte[] LookForMessageWithHashPattern(params string[] startingPattern)
        {
            var pattern = StringPatternToByteArray(startingPattern);
            var sha256 = SHA256.Create();
            var buffer = new byte[] {0};
            const int max8BitNumber = 255;
            var index = 0;
            while (buffer.Length < MaxBufferLength)
            {
                var hash = sha256.ComputeHash(buffer);
                if (IsMatchingHashPattern(hash, pattern))
                {
                    Console.WriteLine("Found a match to pattern {0}\nMatch (hex): {1}\nMatch (hash): {2}\n",
                        string.Join("-", startingPattern),
                        BitConverter.ToString(buffer),
                        string.Join("-", sha256.ComputeHash(buffer).Select(b => b.ToString("X")))
                    );
                    return buffer;
                }
                if (buffer[index] < max8BitNumber)
                {
                    buffer[index]++;
                    continue;
                }
                if (buffer.Length - 1 > index)
                {
                    index++;
                    continue;
                }
                buffer = new byte[buffer.Length + 1];
                index = 0;
            }
            Console.WriteLine("Maximum buffer length (2^32) reached, searching for SHA-256 hash with starting pattern {0} unsuccesfull", string.Join("-", startingPattern));
            return null;
        }

        private static bool IsMatchingHashPattern(IReadOnlyList<byte> hash, IEnumerable<byte> pattern)
        {
            return !pattern.Where((t, i) => t != hash[i]).Any();
        }

        private static IReadOnlyList<byte> StringPatternToByteArray(params string[] strPattern)
        {
            if (strPattern is null)
            {
                throw new ArgumentNullException(nameof(strPattern));
            }

            try
            {
                var bytes = strPattern.Select(s => Convert.ToByte(s, 16)).ToList();
                return bytes;
            }
            catch (OverflowException e)
            {
                throw new ArgumentException("pattern contains invalid value", e);
            }
        }

    }
}
