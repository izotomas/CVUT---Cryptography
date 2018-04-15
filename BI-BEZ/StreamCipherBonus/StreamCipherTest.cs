using System;
using System.Collections;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace StreamCipherBonus
{
    [TestFixture]
    public class StreamCipherTest
    {
        private const string inName = "bonusTask.txt";
        private StreamCipherCracker cracker;
        
        [SetUp]
        public void Setup()
        {
            cracker = new StreamCipherCracker
            {
                CTHexList = StreamCipherCracker.ProcessInput(inName)
            };
            cracker.CTByteList = StreamCipherCracker.ToBytes(cracker.CTHexList);
            cracker.CTXoredPairs = StreamCipherCracker.BuildDictionary(cracker.CTByteList);
        }
            
        
        [TestCase("the")]
        public void RegexTest(string args)
        {
            var match = cracker.Regex.Match(args);
            Assert.True(match.Success);
        }


        [TestCase("hello")]
        [TestCase("the")]
        [TestCase("the p")]
        public void CribDragTest(string word)
        {
            var a = CConverter.ToBytes("Hello World");
            var b = CConverter.ToBytes("the program");
            var xor = CConverter.XOR(a, b);
            cracker.CribDrag(xor, word);
        }

        [Test]
        public void HelloWorldXORTest()
        {
            var a = CConverter.ToBytes("Hello World");
            var b = CConverter.ToBytes("the program");
            var xor = CConverter.XOR(a, b);
            var str = CConverter.ToHexString(xor);
            Console.WriteLine(str);
            str = CConverter.ToHexString("the");
            Console.WriteLine(str);
            str = CConverter.ToASCIIString(CConverter.XOR(xor, CConverter.ToBytes(str)));
            Console.WriteLine(str); 
        }

        [TestCase("The ")]
        [TestCase(" the ")]
        [TestCase("to")]    
        [TestCase(" and ")]    
        [TestCase("the g")]
        [TestCase("But b")]
        [TestCase(" is as")]
        public void CribTest(string word)
        {
            foreach (var item in cracker.CTXoredPairs)
            {
                var a = item.Key.Item1;
                var b = item.Key.Item2;
                var xor = item.Value;
                cracker.CribDrag(xor, word);
            }
        }
    }
}