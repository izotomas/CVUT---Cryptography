using System;
using System.IO;
using System.Security.Cryptography;
using BackupUtility.Backup.Header;
using BackupUtility.Crypto;
using Org.BouncyCastle.Crypto;

namespace BackupUtility.Backup
{
    public static class BackupUtility
    {
        public enum Algorithm
        {
            Rijndael256 = 1
        }

        public static long AES_Encrypt(string inName, string pubKey)
        {
            var key = Crypto.RSA.ParsePEM<AsymmetricKeyParameter>(pubKey);
            var outName = inName + "_enc";
            var fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
            var fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);
            Console.WriteLine("Encrypting to " + outName);
            Console.WriteLine("Any such file will be overwritten!");
            
            // Set Parameters
            var aesParams = new AESParameters()
            {
                AlgorithmId = Algorithm.Rijndael256,
                CipherMode = CipherMode.CBC,
                PaddingMode = PaddingMode.PKCS7,
                BlockSize = AESParameters.DEFAULT_BLOCKSIZE_BITCOUNT,
                IVLength = AESParameters.DEFAULT_BLOCKSIZE_BITCOUNT,
                KeyLength = AESParameters.DEFAULT_KEY_BITCOUNT,
                IV = RndBytes(AESParameters.DEFAULT_BLOCKSIZE_BITCOUNT / 8),
                Key = RndBytes(AESParameters.DEFAULT_KEY_BITCOUNT / 8),
            };
            // Write Header
            var header = new Header.Header(aesParams, key);
            fout.Write(header.ToArray(), 0, Header.Header.HEADER_SIZE);

            // Encrypt the rest
            var total = AES.ProcessInput(true, fin, fout, aesParams);
            return total + Header.Header.HEADER_SIZE;
        }

        public static long AES_Decrypt(string inName, string privKey)
        {
            var key = Crypto.RSA.ParsePEM<AsymmetricCipherKeyPair>(privKey);
            var outName = inName + "_dec";
            var fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
            var fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);
            Console.WriteLine("Decrypting to " + outName);
            Console.WriteLine("Any such file will be overwritten!");

            // read Header
            var buff = new byte[Header.Header.HEADER_SIZE];
            if (Header.Header.HEADER_SIZE != fin.Read(buff, 0, Header.Header.HEADER_SIZE))
            {
                throw new HeaderException("Missing Header data. Required lenght of header is " + Header.Header.HEADER_SIZE + " bytes.");
            }

            var header = new Header.Header(buff);
            var aesParams = header.ToAESParameters(key.Private);
            var total = AES.ProcessInput(false, fin, fout, aesParams);
            return total + Header.Header.HEADER_SIZE;
        }

        #region Helpers

        public static byte[] RndBytes(int byteCount)
        {
            var rand = new RNGCryptoServiceProvider();
            var buffer = new byte[byteCount];
            rand.GetBytes(buffer);
            rand.Dispose();
            return buffer;
        }


        #endregion
    }
}
