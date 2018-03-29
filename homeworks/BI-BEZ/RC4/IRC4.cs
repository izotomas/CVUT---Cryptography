using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC4
{
    public interface IRC4
    {
        string EncryptToStr(string plainText);
        string DecryptToStr(string plainText);
        byte[] Encrypt(string plainText);
        byte[] Decrypt(string plainText);
    }
}
