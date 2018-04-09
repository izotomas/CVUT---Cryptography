using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace DESBitmap.Options
{
    public abstract class BaseOptions
    {
        [Option('i', "input", HelpText = "Input must be name of .bmp file", Required = true)]
        public string InputFile { get; protected set; }

        [Option('k', "key", HelpText = "Secret Key used for encryption/decryption", Required = true)]
        public string Key { get; protected set; }

        private string _mode;
        [Option('m', "mode", HelpText = "Operation mode. Could be ECB or CBC (Default is ECB) ", Required = true)]
        public string Mode {
            get => _mode;
            protected set
            {
                value = value.ToUpperInvariant();
                if (!value.Equals("CBC") && !value.Equals("ECB"))
                {
                    throw new ArgumentException("Only ECB or CBC modes are supported");
                }
                _mode = value;
            }
        }
        
        [Usage(ApplicationAlias = "des.exe")] 
        public abstract IEnumerable<Example> Examples { get; }
    }
}