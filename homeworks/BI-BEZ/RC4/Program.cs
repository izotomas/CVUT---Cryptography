using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using CommandLine;

namespace RC4
{

    public class Program
    {
        public enum Mode
        {
            Crack,
            Encrypt,
            Decrypt
        }    

        [Option('m',"mode", Required = true)]
        public Mode SpecifiedMode { get; set; }

        [Option('o',"output")]
        public string OutPutFile { get; set; }


        public static void Main(string[] args)
        {
            var options = CommandLine.Parser.Default.ParseArguments<Program>(args);
            options.WithParsed(opts =>
            {
                string res = null;
                switch (opts.SpecifiedMode)
                {
                    case Mode.Crack:

                        break;
                    case Mode.Decrypt:
                        break;
                    case Mode.Encrypt:
                        break;
                    default:
                        Console.Error.WriteLine("Supported modes are: crack, decrypt, or encrypt");
                        break;
                }

                if (opts.OutPutFile != null)
                {
                    try
                    {
                        File.WriteAllText(opts.OutPutFile, res);
                        Console.WriteLine("Result written to: {0}", opts.OutPutFile);
                    }
                    catch (IOException e)
                    {
                        Console.Error.WriteLine("Error writing to file \"{0}\"\n{1}", opts.OutPutFile, e);

                    }
                }
            });
        }
    }
}
