using System;
using System.Collections.Generic;
using BackupUtility.Crypto;
using NUnit.Framework;
using Org.BouncyCastle.Crypto;

namespace BackupUtility.Tests
{
    [TestFixture]
    public class RSATest
    {
        
        private static IEnumerable<byte[]> AESKeys
        {
            get { yield return new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16}; }
        }

        #region KeyTests

        [TestCase(TestHelpers.PrivateKey, true)]
        [TestCase(TestHelpers.PublicKey, false)]
        public void ReadValidKeyTest(string keyFileName, bool isPrivateKey)
        {
            if (isPrivateKey)
            {
                RSA.ParsePEM<AsymmetricCipherKeyPair>(keyFileName);
            }
            else
            {
                RSA.ParsePEM<AsymmetricKeyParameter>(keyFileName);
            }
        }

        [TestCase(TestHelpers.PrivateKey, false)]
        [TestCase(TestHelpers.PublicKey, true)]
        [TestCase(TestHelpers.InvalidKey, true)]
        [TestCase(TestHelpers.InvalidKey, false)]
        public void ReadInvalidPublicKeyTest(string keyFileName, bool isPrivateKey)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                if (isPrivateKey)
                {
                    RSA.ParsePEM<AsymmetricCipherKeyPair>(keyFileName);
                }
                else
                {
                    RSA.ParsePEM<AsymmetricKeyParameter>(keyFileName);
                }
            });
        }
        

        #endregion

        #region EncryptionDecryptionTests

        [Test, TestCaseSource("AESKeys")]
        public void ValidEncryptionDecryptionTest(byte[] data)
        {
            //var encryptor = new RSA(PublicKey, isPrivateKey: false);
            var shared = RSA.ParsePEM<AsymmetricKeyParameter>(TestHelpers.PublicKey);
            var secret = RSA.ParsePEM<AsymmetricCipherKeyPair>(TestHelpers.PrivateKey);

            var e1 = RSA.Encrypt(data, shared);
            var e2 = RSA.Encrypt(data, shared);
            var e3 = RSA.Encrypt(data, secret.Public);
            var e4 = RSA.Encrypt(data, secret.Public);
            for (var i = 0; i < e1.Length; i++)
            {
                Assert.AreEqual(e1[i], e2[i]);
                Assert.AreEqual(e2[i], e3[i]);
                Assert.AreEqual(e3[i], e4[i]);
            }

            var d1 = RSA.Decrypt(e1, secret.Private);
            var d2 = RSA.Decrypt(e2, secret.Private);
            var d3 = RSA.Decrypt(e3, secret.Private);
            var d4 = RSA.Decrypt(e4, secret.Private);

            Assert.AreEqual(data.Length, d1.Length);
            Assert.AreEqual(data.Length, d2.Length);
            Assert.AreEqual(data.Length, d3.Length);
            Assert.AreEqual(data.Length, d4.Length);

            for (var i = 0; i < data.Length; i++)
            {
                Assert.AreEqual(data[i], d1[i]);
                Assert.AreEqual(d1[i], d2[i]);
                Assert.AreEqual(d2[i], d3[i]);
                Assert.AreEqual(d3[i], d4[i]);
            }
        }


        [Test, TestCaseSource("AESKeys")]
        public void InvalidEncryptionDecryptionTest(byte[] data)
        {

            var shared = RSA.ParsePEM<AsymmetricKeyParameter>(TestHelpers.PublicKey);
            var secret = RSA.ParsePEM<AsymmetricCipherKeyPair>(TestHelpers.PrivateKey);

            Assert.Throws<ArgumentException>(() =>
            {
                RSA.Encrypt(data, secret.Private);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                RSA.Decrypt(data, secret.Public);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                RSA.Decrypt(data, shared);
            });
        }


        #endregion
    }
}
