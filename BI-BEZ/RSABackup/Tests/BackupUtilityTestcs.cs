using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace BackupUtility.Tests
{
    [TestFixture]
    public class BackupUtilityTestcs
    {
        private readonly IEnumerable<(string, string, string)> fileNames = TestHelpers.FileNames;

        [Test,TestCaseSource(nameof(fileNames))]
        public void EncryptionDecryptionTest((string, string, string) files)
        {
            var privKey = TestHelpers.PrivateKey;
            var pubKey = TestHelpers.PublicKey;
            var inFile = files.Item1;
            var encFile = files.Item2;
            var decFile = files.Item2 + "_dec";

            TestHelpers.MeasuredAction(() =>
            {
                Backup.BackupUtility.AES_Encrypt(inFile, pubKey);
            });

            TestHelpers.MeasuredAction(() =>
            {
                Backup.BackupUtility.AES_Decrypt(encFile, privKey);
            });

            var h1 = TestHelpers.MD5(inFile);
            var h2 = TestHelpers.MD5(decFile);

            Assert.That(h1.Equals(h2));
        } 
    }
}
