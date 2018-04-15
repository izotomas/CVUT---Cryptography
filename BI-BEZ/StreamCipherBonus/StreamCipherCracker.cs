using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace StreamCipherBonus
{
    public class StreamCipherCracker
    {
        public readonly Dictionary<string, byte[]> Words = new Dictionary<string, byte[]>{
            {"the", new []{(byte) 116, (byte)104, (byte)101}},
        };
        public readonly Regex Regex = new Regex(@"^(\s?[A-Za-z]+\s?)+$");
        public List<string> CTHexList { get; set; }
        public List<byte[]> CTByteList { get; set; }
        public Dictionary<(byte[], byte[]), byte[]> CTXoredPairs { get; set; }

        
        public static List<string> ProcessInput(string inName)
        {
            var hexTexts = new List<string>();

            string line;
            using (var sr = new StreamReader(inName)) 
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "") continue;
                    hexTexts.Insert(0 ,line);
                }
            }
            return hexTexts;
        }
        
        public static List<byte[]> ToBytes(IEnumerable<string> ctHexList)
        {
            var ctByteList = ctHexList.Select(CConverter.ToBytes).ToList();
            return ctByteList;
        }

        public static Dictionary<(byte[], byte[]), byte[]> BuildDictionary(List<byte[]> ctByteList)
        {
            var arr = ctByteList;
            var dict = new Dictionary<(byte[], byte[]), byte[]>();
            for (var i = 0; i < ctByteList.Count - 1; i++)
            {
                for (var j = i + 1; j < ctByteList.Count; j++)
                {
                    var xor = CConverter.XOR(arr[i], arr[j]);
                    dict.Add((arr[i], arr[j]), xor);
                } 
            }
            
            return dict;
        }

        public void CribDrag(byte[] xored, string word)
        {
            var byteWord = CConverter.ToBytes(word);
            byte[] cribXOR;
            string line, cribWord;
            Match match;
            for (var i = 0; i <= xored.Length - byteWord.Length; i++)
            {
                cribXOR = CConverter.XOR(xored, byteWord, i);
                line = CConverter.ToASCIIString(cribXOR);
                cribWord = line.Substring(i, word.Length);
                match = Regex.Match(cribWord);
                if (match.Success)
                {
                    Console.WriteLine("result[{0}]: {1}", i, cribWord);
                }
               //else Console.Error.WriteLine("result[{0}]: {1}", i, cribWord);
            }
        }
        
    }
}