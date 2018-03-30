using System;
using CommandLine;

namespace HashFinder
{
    public class Program
    {
        [Option('o', "output")]
        public string OutputFileName { get; set; }

        public static void Main(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<Program>(args);
            result.WithParsed(opts =>
                SHA256MessageFinder.LookForMessageWithHashPattern(opts.OutputFileName, "AA", "BB"));
        }
    }
}
