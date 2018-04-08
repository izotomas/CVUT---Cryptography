using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace DESBitmap.Options
{
    public abstract class BaseOptions
    {
        [Option('i', "input", HelpText = "Input must be name of .bmp file", Required = true)]
        public string InputFile { get; set; }

        [Option('k', "key", HelpText = "Secret Key used for encryption/decryption", Required = true)]
        public string Key { get; set; }
        
        [Usage(ApplicationAlias = "des.exe")] 
        public abstract IEnumerable<Example> Examples { get; }
    }
}