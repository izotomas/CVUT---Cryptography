using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Markup;

namespace DESBitmap
{

    public class DESUtility
    {
        private const int CHUNK_SIZE = 1024;
        private const int SECRET_LENGTH = 8;
        private static readonly CipherMode[] ALLOWED_MODES = { CipherMode.ECB, CipherMode.CBC };
        private static readonly Regex InputRegex = new Regex(@"^(?<filename>.+)\.bmp$");

        public CipherMode CipherMode { get; private set; }

        public byte[] Secret { get; private set; }

        public DESUtility(CipherMode operationMode, string secret) : this(operationMode, Encoding.ASCII.GetBytes(secret)) { }
        
        public DESUtility(CipherMode operationMode, byte[] secret)
        {
            SetValidOperationMode(operationMode);
            SetValidSecret(secret); 
        }        

        public void EncryptData(string fileName)
        {
            var outName = GetOutName(fileName, true);
            ProcessInput(fileName, outName, true);
        }

        public void DecryptData(string fileName)
        {
            var outName = GetOutName(fileName, false);
            ProcessInput(fileName, outName, false);
        }
        
        public static byte[] GenerateKey()
        {
            var rand = new RNGCryptoServiceProvider();
            var buffer = new byte[SECRET_LENGTH];
            rand.GetBytes(buffer);
            rand.Dispose();
            return buffer;
        }
        
        #region Helpers

        private void ProcessInput(string inName, string outName, bool forEncryption)
        {
            //Create the file streams to handle the input and output files.
            var fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
            var fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            //Create variables to help with read and write.
            var buff = new byte[CHUNK_SIZE];
            long processedBytes = 0;
            long totalBytes = fin.Length;
            DES des = new DESCryptoServiceProvider();
            des.Mode = this.CipherMode;
            des.Key = this.Secret;
            var cryptoStream = forEncryption
                ? new CryptoStream(fout, des.CreateEncryptor(), CryptoStreamMode.Write)
                : new CryptoStream(fout, des.CreateDecryptor(), CryptoStreamMode.Write);

            //Read from the input file, then encrypt and write to the output file.
            processedBytes += ProcessHeader(buff, fin,fout, cryptoStream, out var cryptoUsed);
            
            while(processedBytes < totalBytes)
            {
                var len = fin.Read(buff, 0, CHUNK_SIZE);
                cryptoStream.Write(buff, 0, len);
                processedBytes += len;
                cryptoUsed = true;
            }
            
            if (cryptoUsed) cryptoStream.Close();  
            fout.Close();
            fin.Close();                
        }
        
        private long ProcessHeader(byte[] buffer, FileStream fin, FileStream fout, CryptoStream cryptoStream, out bool cryptoUsed)
        {
            cryptoUsed = false;
            var len = fin.Read(buffer, 0, CHUNK_SIZE);
            if (len > 10)
            {
                var index = BitConverter.ToUInt32(buffer, 10);
                var imageDataBegin = Convert.ToInt32(index);
                fout.Write(buffer, 0, imageDataBegin); //todo: case when imageDataBegin is in next frame...
                cryptoStream.Write(buffer, imageDataBegin, len - imageDataBegin);
                cryptoUsed = true;
            }
            else
            {
                fout.Write(buffer, 0, len);
            }

            return Convert.ToInt64(len);
        }
        
        private void SetValidOperationMode(CipherMode mode)
        {
            if (!ALLOWED_MODES.Contains(mode))
            {
                throw new ArgumentException("Only ECB and CBC modes are supported");
            }
            
            this.CipherMode = mode;
        }

        private void SetValidSecret(byte[] secret)
        {
            if (secret.Length != SECRET_LENGTH)
            {
                throw new ArgumentException("Secret must be 8-bytes long");
            }

            this.Secret = secret;
        }

        private string GetOutName(string inName, bool forEncryption)
        {
            var match = InputRegex.Match(inName);
            if (!match.Success)
            {
                throw new ArgumentException(inName + ": not a valid bitmap file");
            }
            var sb = new StringBuilder();
            sb.Append(match.Groups["filename"].Value);
            switch (CipherMode)
            {
                case CipherMode.ECB:
                    sb.Append(forEncryption
                        ? "_ecb"
                        : "_dec");
                    break;
                case CipherMode.CBC:
                    sb.Append(forEncryption
                        ? "_cbc"
                        : "_cbc_dec");
                    break;
            }
            sb.Append(".bmp");

            return sb.ToString();
        }
        
        #endregion
    }
}
