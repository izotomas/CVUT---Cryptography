using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace DESBitmap
{
    [TestFixture]
    public class DESUtilityTestcs
    {
        [TestCase("homer.bmp","homer_ecb.bmp")]
        [TestCase("Mad_scientist.bmp","Mad_scientist_ecb.bmp")]
        [TestCase("a.bmp","a_ecb.bmp")]
        [TestCase("MARBLES.bmp","MARBLES_ecb.bmp")]
        public void ECBTest(string fileIn, string encFileOut)
        {
            var key = DESUtility.GenerateKey();
            var des = new DESUtility(CipherMode.ECB, key);
            des.EncryptData(fileIn);
            des.DecryptData(encFileOut);
        }
        
        [TestCase("homer.bmp","homer_cbc.bmp")]
        [TestCase("Mad_scientist.bmp","Mad_scientist_cbc.bmp")]
        [TestCase("a.bmp","a_cbc.bmp")]
        [TestCase("MARBLES.bmp","MARBLES_cbc.bmp")]
        public void CBCTest(string fileIn, string encFileOut)
        {
            var key = DESUtility.GenerateKey();
            var des = new DESUtility(CipherMode.CBC, key);
            des.EncryptData(fileIn);
            des.DecryptData(encFileOut);
        }

    }
}