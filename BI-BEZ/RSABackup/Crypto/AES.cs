using System.IO;
using System.Security.Cryptography;

namespace BackupUtility.Crypto
{
    public static class AES
    {
        // configurables
        private const int MB100 = 100 * 1024 * 1024;
        private const int MB10 = 10 * 1024 * 1024;

        // use 100MB ram buffer (also for small files)
        private const int BUFFER_SIZE = MB100;

        public static long ProcessInput(
            bool forEncryption, 
            FileStream fin, 
            FileStream fout, 
            AESParameters aesParams
            )
        {

            var aes = new RijndaelManaged
            {
                KeySize = aesParams.KeyLength,
                BlockSize = aesParams.IVLength,
                Key = aesParams.Key,
                IV = aesParams.IV,
                Padding = aesParams.PaddingMode,
                Mode = aesParams.CipherMode
            };

            var cryptoStream = forEncryption
                ? new CryptoStream(fout, aes.CreateEncryptor(), CryptoStreamMode.Write)
                : new CryptoStream(fout, aes.CreateDecryptor(), CryptoStreamMode.Write);

            var buffer = new byte[BUFFER_SIZE];
            long total = 0;
            try
            {
                int len;
                while ((len = fin.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cryptoStream.Write(buffer, 0, len);
                    total += len;
                }
            }
            finally
            {
                cryptoStream.Close();
                fout.Close();
                fin.Close();
            }
            return total;
        }
    }
}
