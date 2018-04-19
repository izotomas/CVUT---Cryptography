using CommandLine;

namespace BackupUtility.Options
{
    public class Options
    {
        
        [Option('k',"key", Required = true, HelpText = "Input RSA key (public or private) file name")]
        public string Key { get; set; }

        [Option('i', "in",Required = true, HelpText = "Input file name")]
        public string File { get; set; }

        [Option('d', Default = false, HelpText = "Flag for decryption ")]
        public bool ForDecription { get; set; }
    }
}
