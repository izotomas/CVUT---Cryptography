using System;
using System.IO;
using CommandLine;
using Cracker;

namespace RC4
{

    public class Program
    {

        public static void Main(string[] args)
        {
            var options = CommandLine.Parser.Default.ParseArguments<Options.Options>(args);
            options.WithParsed(opts =>
            {
                var result = RC4Cracker.DecryptToStr(
                    opts.PlainText,
                    opts.CipherHexText,
                    opts.UnknownHexText
                    );
                
                Console.WriteLine("Decrypted: {0}", result);
                if (opts.OutPutFile != null)
                {
                    try
                    {
                        File.WriteAllText(opts.OutPutFile, result);
                        Console.WriteLine("Decrypted text written to: {0}", opts.OutPutFile);
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
