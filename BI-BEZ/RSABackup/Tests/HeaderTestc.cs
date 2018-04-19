using System.Collections.Generic;
using System.Security.Cryptography;
using BackupUtility.Backup;
using BackupUtility.Backup.Header;
using BackupUtility.Crypto;
using NUnit.Framework;
using Org.BouncyCastle.Crypto;

namespace BackupUtility.Tests
{
    [TestFixture]
    public class HeaderTestc
    {
        private AsymmetricCipherKeyPair privateKey;
        private readonly IEnumerable<AESParameters> aesParameters = TestHelpers.AesParamenters;

        [SetUp]
        public void SetUp()
        {
            privateKey = Crypto.RSA.ParsePEM<AsymmetricCipherKeyPair>(TestHelpers.PrivateKey);
        }

        [Test, TestCaseSource(nameof(aesParameters))]
        public void ValidHeaderConversionTest(AESParameters aesParams)
        {
            var h1 = new Header(aesParams, privateKey.Public);
            var a1 = h1.ToAESParameters(privateKey.Private);
            var buff = h1.ToArray();
            var h2 = new Header(buff);
            var a2 = h2.ToAESParameters(privateKey.Private);

            // Simple tests
            Assert.That(h1.AlgorithmId == h2.AlgorithmId);
            Assert.That(h1.PaddingMode == h2.PaddingMode);
            Assert.That(h1.CipherMode == h2.CipherMode);
            Assert.That(h1.BlockSize.Length == h2.BlockSize.Length);
            Assert.That(h1.KeyLength.Length == h2.KeyLength.Length);
            Assert.That(h1.IVLength.Length == h2.IVLength.Length);
            Assert.That(h1.IV.Length == h2.IV.Length);
            Assert.That(h1.Key.Length == h2.Key.Length);

            // iterative tests
            for (var i = 0; i < Header.BYTESTREAM_BYTE_OFFSTET; i++)
            {
                Assert.That(h1.IVLength[i] == h2.IVLength[i]);
                Assert.That(h1.KeyLength[i] == h2.KeyLength[i]);
                Assert.That(h1.BlockSize[i] == h2.BlockSize[i]);
            }

            for (var i = 0; i < h1.Key.Length; i++)
            {
                Assert.That(h1.Key[i] == h2.Key[i]);
            }
            for (var i = 0; i < h1.IV.Length; i++)
            {
                Assert.That(h1.IV[i] == h2.IV[i]);
            }

            // AESParams -> Header -> AESParams test
            Assert.That(aesParams.AlgorithmId == a1.AlgorithmId);
            Assert.That(aesParams.AlgorithmId == a2.AlgorithmId);
            Assert.That(aesParams.PaddingMode == a1.PaddingMode);
            Assert.That(aesParams.PaddingMode == a2.PaddingMode);
            Assert.That(aesParams.CipherMode == a1.CipherMode);
            Assert.That(aesParams.CipherMode == a2.CipherMode);
            Assert.That(aesParams.BlockSize == a2.BlockSize);
            Assert.That(aesParams.BlockSize == a1.BlockSize);
            Assert.That(aesParams.IVLength == a1.IVLength);
            Assert.That(aesParams.IVLength == a2.IVLength);
            Assert.That(aesParams.KeyLength == a1.KeyLength);
            Assert.That(aesParams.KeyLength == a2.KeyLength);
            Assert.That(aesParams.IV.Length == a2.IV.Length);
            Assert.That(aesParams.IV.Length == a1.IV.Length);
            Assert.That(aesParams.Key.Length == a1.Key.Length);
            Assert.That(aesParams.Key.Length == a2.Key.Length);

            for (var i = 0; i < aesParams.IV.Length; i++)
            {
                Assert.That(aesParams.IV[i] == a1.IV[i]);
                Assert.That(aesParams.IV[i] == a2.IV[i]);
            }


            for (var i = 0; i < aesParams.Key.Length; i++)
            {
                Assert.That(aesParams.Key[i] == a1.Key[i]);
                Assert.That(aesParams.Key[i] == a2.Key[i]);
            }
        }
    }
}
