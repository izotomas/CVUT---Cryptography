using System;
using CommandLine;

namespace BackupUtility
{
    public static class Program
    {
        public static void Main(string[] args)
        {
             CommandLine.Parser.Default.ParseArguments<Options.Options>(args)
                .WithParsed(opts =>
                {
                    if (opts.ForDecription)
                    {
                        Console.WriteLine("Decrypting file: " + opts.File);
                        var total = Backup.BackupUtility.AES_Decrypt(opts.File, opts.Key);
                        Console.WriteLine("Decryption complete");
                    }
                    else
                    {
                        Console.WriteLine("Encrypting file: " + opts.File);
                        var total = Backup.BackupUtility.AES_Encrypt(opts.File, opts.Key);
                        Console.WriteLine("Encryption complete. {0}bytes writen", total);
                    }
                    Console.WriteLine("Program is exiting...");
                    Environment.Exit(0);
                });
        }
    }
}