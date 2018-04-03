using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;

namespace HashFinder
{
    [TestFixture]
    public class SHA256MessageFinderTest
    {
        
        [TestCase("AA")]
        [TestCase("AA","BB")]
        //[TestCase("AA","BB", "CC")]
        public void Test(params string[] pattern)
        {
            var result = SHA256MessageFinder.LookForMessageWithHashPattern(null, pattern);

            // check if result was found
            Assert.NotNull(result);
            
            // check if pattern matches its SHA-256 hash
            var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(result);              
            Assert.True(!pattern.Where((element, index) => Convert.ToByte(element, 16) != hash[index]).Any());
        }
    }
}
