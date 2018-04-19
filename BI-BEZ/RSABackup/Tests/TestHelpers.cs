using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BackupUtility.Crypto;

namespace BackupUtility.Tests
{
    public static class TestHelpers
    {
        public const string PrivateKey = "privkey.pem";
        public const string PublicKey = "pubkey.pem";
        public const string InvalidKey = "test.txt";

        public static IEnumerable<(string, string, string)> FileNames
        {
            get
            {
                yield return ("test.txt", "test.txt_enc", "test.txt_dec");
                yield return ("MB1", "MB1_enc", "MB1_dec");
                yield return ("MB10", "MB10_enc", "MB10_dec");
                yield return ("MB100", "MB100_enc", "MB100_dec");
            }
        }

        public static IEnumerable<AESParameters> AesParamenters
        {
            get
            {
                yield return new AESParameters()
                {
                    AlgorithmId = Backup.BackupUtility.Algorithm.Rijndael256,
                    CipherMode = CipherMode.CBC,
                    PaddingMode = PaddingMode.PKCS7,
                    BlockSize = AESParameters.DEFAULT_BLOCKSIZE_BITCOUNT,
                    IVLength = AESParameters.DEFAULT_BLOCKSIZE_BITCOUNT,
                    KeyLength = AESParameters.DEFAULT_KEY_BITCOUNT,
                    IV = Backup.BackupUtility.RndBytes(AESParameters.DEFAULT_BLOCKSIZE_BITCOUNT / 8),
                    Key = Backup.BackupUtility.RndBytes(AESParameters.DEFAULT_KEY_BITCOUNT / 8)
                };
            }
        }

        public static void MeasuredAction(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            Console.WriteLine("Elapsed time: {0}", stopwatch.Elapsed);
        }


        public static string MD5(string filename)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

    }
}
