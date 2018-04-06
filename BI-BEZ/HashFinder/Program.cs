using System;
using System.Runtime.CompilerServices;
using CommandLine;

namespace HashFinder
{
    public class Program
    {
        class Options
        {
            [Option('o', "output", Default = null)]
            public string OutputFileName { get; set; }
        }
        
        public static void Main(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<Options>(args);
            result.WithParsed(opts =>
                {
                    SHA256MessageFinder.LookForMessageWithHashPattern(opts.OutputFileName, "AA", "BB");
                });
            Environment.Exit(0);
        }
    }
}
