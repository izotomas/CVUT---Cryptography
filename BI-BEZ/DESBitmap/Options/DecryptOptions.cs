using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace DESBitmap.Options
{
    [Verb("decrypt", HelpText = "decrypts")]
    public class DecryptOptions: BaseOptions
    {
        public override IEnumerable<Example> Examples
        {
            get { yield return new Example("Decrypting", 
                new DecryptOptions
                {
                    Key = "passwd12", InputFile = "image.bmp", Mode = "CBC"
                });
            }
        } 
    }
}