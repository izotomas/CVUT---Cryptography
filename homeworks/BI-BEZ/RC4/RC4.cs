using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

namespace RC4
{
   
    public class RC4: IRC4
    {
        public const string GeneratorPasswordCharSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789[]{}(),.:;/\\!@#$%^&*";
        public const int GeneratorPasswordDefaultLength = 16;
        public const int GeneratorIVDefaultLength = 16;

        private readonly RC4Engine cipher;
        private readonly ICipherParameters cipherParams;

        private readonly string _iv;
        private readonly string _key;
        public string Key => _key;
        public string IV => _iv;

        // Generate both key and IV (keyLenght optional)
        public RC4(int keyLength = GeneratorPasswordDefaultLength): this(null, null, keyLength, true) { }
        // Pass key (IV passing optional) 
        public RC4(string key, string iv = null): this(key, iv, GeneratorPasswordDefaultLength) { }
        // Pass key (IV generating optional)
        public RC4(string key, bool enableIV): this(key, null, GeneratorPasswordDefaultLength, enableIV) { }

        private RC4(string key = null, string iv = null, int keyLength = GeneratorPasswordDefaultLength, bool useIV = false)
        {
            this._key = key ?? KeyGen(keyLength);
            this._iv = useIV ? IVGen() : iv;
            this.cipher = new RC4Engine();
            cipherParams = new KeyParameter(Encoding.UTF8.GetBytes(this.Key + this.IV));
        }

        public byte[] Encrypt(string plainText)
        {
            var plainTextBytes = ConvertToBytes(plainText);
            var ct = new byte[plainTextBytes.Length];
            cipher.Init(true, cipherParams);
            cipher.ProcessBytes(plainTextBytes, 0, plainTextBytes.Length, ct, 0);
            //cipher.Reset(); //necessary??
            return ct;
        }

        public string EncryptToStr(string plainText)
        {
            var ct = Encrypt(plainText);
            var ctStr = ConvertToString(ct);
            return ctStr;
        }

        public byte[] Decrypt(string cipherText)
        {
            var cipherTextBytes = ConvertToBytes(cipherText);
            var pt = new byte[cipherTextBytes.Length];
            cipher.Init(false, cipherParams);
            cipher.ProcessBytes(cipherTextBytes, 0, cipherTextBytes.Length, pt, 0);
            //cipher.Reset(); //necessary?
            return pt;
        }

        public string DecryptToStr(string cipherText)
        {
            var pt = Decrypt(cipherText);
            var ptStr = ConvertToString(pt);
            return ptStr;
        }

        # region Helpers 

        private string KeyGen(int len)
        {
            var builder = new StringBuilder();
            var rand = new RNGCryptoServiceProvider();
            var buffer = new byte[len];
            rand.GetBytes(buffer);
            rand.Dispose();
            foreach (var b in buffer)
            {
                var i = b % GeneratorPasswordCharSet.Length;
                builder.Append(GeneratorPasswordCharSet[i]);
            }
            
            return builder.ToString();
        }

        private string IVGen()
        {
            var rand = new RNGCryptoServiceProvider();
            var buffer = new byte[GeneratorIVDefaultLength];
            rand.GetBytes(buffer);
            rand.Dispose();
            var iv = ConvertToString(buffer);
            return iv;
        }

        private static byte[] ConvertToBytes(string text)
        {
            return text?.Select(Convert.ToByte).ToArray();
        }

        private static string ConvertToString(byte[] bytes)
        {
            return new string(bytes?.Select(Convert.ToChar).ToArray());
        }
        
        #endregion


        public static void Main(string[] args)
        {
            const string key = "FDeaAs@TRc]Vi9V9";
            const string plainText = "AHOJ"; 
            var r1 = new RC4(key, null);
            var r2 = new RC4(key, true);
            // encrypting without IV
            var result = r1.Encrypt(plainText);
            Console.WriteLine("RC4 plain");
            Console.WriteLine("CT: " + string.Join("",Encoding.UTF8.GetChars(result)));
            Console.WriteLine("CT (bytes): " + BitConverter.ToString(result));
            Console.WriteLine("KEY: " + r1.Key);
            Console.WriteLine("IV: " + r1.IV);
            var result2 = r2.Encrypt(plainText);
            Console.WriteLine("RC4 with IV");
            Console.WriteLine("CT: " + string.Join("",Encoding.UTF8.GetChars(result2)));
            Console.WriteLine("CT (bytes): " + BitConverter.ToString(result2));
            Console.WriteLine("KEY: " + r2.Key);
            Console.WriteLine("IV: " + r2.IV);

            Console.ReadLine();
        }
    }
}
