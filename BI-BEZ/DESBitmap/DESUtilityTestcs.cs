using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace DESBitmap
{
    [TestFixture]
    public class DESUtilityTestcs
    {
        [TestCase("homer.bmp","homer_ecb.bmp", "homer_ecb_ecb_dec.bmp")]
        [TestCase("Mad_scientist.bmp","Mad_scientist_ecb.bmp", "Mad_scientist_ecb_ecb_dec.bmp")]
        [TestCase("a.bmp","a_ecb.bmp", "a_ecb_ecb_dec.bmp")]
        [TestCase("MARBLES.bmp","MARBLES_ecb.bmp","MARBLES_ecb_ecb_dec.bmp")]
        public void ECBTest(string fileIn, string encFileOut, string decFileOut)
        {
            var key = DESUtility.GenerateKey();
            var des = new DESUtility(CipherMode.ECB, key);
            Assert.That(() => des.EncryptData(fileIn), Throws.Nothing);
            Assert.That(() => des.DecryptData(encFileOut), Throws.Nothing);
            var h1 = MD5(fileIn);
            var h2 = MD5(decFileOut);
            Assert.That(h1.Equals(h2));
        }
        
        [TestCase("homer.bmp","homer_cbc.bmp","homer_cbc_cbc_dec.bmp")]
        [TestCase("Mad_scientist.bmp","Mad_scientist_cbc.bmp","Mad_scientist_cbc_cbc_dec.bmp")]
        [TestCase("a.bmp","a_cbc.bmp","a_cbc_cbc_dec.bmp")]
        [TestCase("MARBLES.bmp","MARBLES_cbc.bmp","MARBLES_cbc_cbc_dec.bmp")]
        public void CBCTest(string fileIn, string encFileOut, string decFileOut)
        {
            var key = DESUtility.GenerateKey();
            var des = new DESUtility(CipherMode.CBC, key);
            des.EncryptData(fileIn);
            des.DecryptData(encFileOut);
            var h1 = MD5(fileIn);
            var h2 = MD5(decFileOut);
            Assert.That(h1.Equals(h2));
        }

        private static string MD5(string filename)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash =  md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}