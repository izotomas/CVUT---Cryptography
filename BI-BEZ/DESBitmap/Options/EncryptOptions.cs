using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace DESBitmap.Options
{
    [Verb("encrypt", HelpText = "encrypts bitmap using DES")]
    public class EncryptOptions: BaseOptions
    {
        [Option('m', "mode", HelpText = "Operation mode. Could be ECB or CBC (Default is ECB) ", Default = "ECB")]
        public string Mode { get; set; }

        public override IEnumerable<Example> Examples
        {
            get { 
                yield return new Example(
                    "Encrypting", 
                    new EncryptOptions
                    {
                        Mode = "ecb", Key = "passwd12", InputFile = "image.bmp"
                        
                    });
                }
        }

    }

}