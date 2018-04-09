using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace DESBitmap.Options
{
    [Verb("encrypt", HelpText = "encrypts bitmap using DES")]
    public class EncryptOptions: BaseOptions
    {

        public override IEnumerable<Example> Examples
        {
            get { 
                yield return new Example(
                    "Encrypting", 
                    new EncryptOptions
                    {
                        Mode = "ecb", Key = "ab12fX79", InputFile = "image.bmp"
                        
                    });
                }
        }

    }

}