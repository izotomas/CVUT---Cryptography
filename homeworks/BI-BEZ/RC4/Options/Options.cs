using System;
using CommandLine;

namespace Options
{
    public class Options
    {
        [Option('c',"ciphertext", Required = true)]
        public string CipherHexText { get; set; }
        
        [Option('u',"unknown", Required = true)]
        public string UnknownHexText { get; set; }
        
        [Option('p',"plaintext", Default = "abcdefghijklmnopqrstuvwxyz0123")]
        public string PlainText { get; set; }
        
        [Option('o',"output", HelpText = "Output file name")]
        public string OutPutFile { get; set; }
    }
}