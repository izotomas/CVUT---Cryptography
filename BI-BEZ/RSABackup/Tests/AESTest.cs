using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using BackupUtility.Crypto;
using NUnit.Framework;

namespace BackupUtility.Tests
{
    [TestFixture]
    public class AESTest
    {
        private readonly IEnumerable<(string, string, string)> files = TestHelpers.FileNames;

        [Test, TestCaseSource(nameof(files))]
        public void EncryptionDecryptionTest((string, string, string) fileNames)
        {
            var inName = fileNames.Item1;
            var encName = fileNames.Item2;
            var decName = fileNames.Item3;

            var fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
            var fout = new FileStream(encName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            var aesParams = new AESParameters()
            {
                AlgorithmId = Backup.BackupUtility.Algorithm.Rijndael256,
                CipherMode = CipherMode.CBC,
                PaddingMode = PaddingMode.PKCS7,
                BlockSize = AESParameters.DEFAULT_BLOCKSIZE_BITCOUNT,
                IVLength = AESParameters.DEFAULT_BLOCKSIZE_BITCOUNT,
                KeyLength = AESParameters.DEFAULT_KEY_BITCOUNT,
                IV = Backup.BackupUtility.RndBytes(AESParameters.DEFAULT_BLOCKSIZE_BITCOUNT / 8),
                Key = Backup.BackupUtility.RndBytes(AESParameters.DEFAULT_KEY_BITCOUNT / 8),
            };

            Console.WriteLine("Encrypting to " + encName);
            TestHelpers.MeasuredAction(() =>
            {
                AES.ProcessInput(true, fin, fout, aesParams);
            });

            fin = new FileStream(encName, FileMode.Open, FileAccess.Read);
            fout = new FileStream(decName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            Console.WriteLine("Decrypting to " + decName);
            TestHelpers.MeasuredAction(() =>
            {
                AES.ProcessInput(false, fin, fout, aesParams);
            });

            // check hash
            var h1 = TestHelpers.MD5(inName);
            var h2 = TestHelpers.MD5(decName);
            Assert.That(h1.Equals(h2));
        }

    }
}

