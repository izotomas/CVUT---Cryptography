using System;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

namespace RC4
{
   
    public class RC4: IRC4
    {
        private readonly RC4Engine cipher;
        private readonly ICipherParameters cipherParams;

        private readonly string _iv;
        private readonly string _key;
        public string Key => _key;
        public string IV => _iv;

        // Generate both key and IV (keyLenght optional) - for testing purposes only
        public RC4(int keyLength = Utility.DefaultGeneratedPasswordLength): this(null, null, keyLength, true) { }
        // Inject key (IV injection optional) 
        public RC4(string key, string iv = null): this(key, iv, Utility.DefaultGeneratedPasswordLength) { }
        // Inject key (IV generating optional)
        public RC4(string key, bool enableIV): this(key, null, Utility.DefaultGeneratedPasswordLength, enableIV) { }

        private RC4(string key = null, string iv = null, int keyLength = Utility.DefaultGeneratedPasswordLength, bool generateIV = false)
        {
            this._key = key ?? Utility.GeneratePassword(keyLength);
            this._iv = generateIV ? Utility.ConvertToString(Utility.GenerateIV()) : iv;
            this.cipher = new RC4Engine();
            cipherParams = new KeyParameter(Encoding.UTF8.GetBytes(this.Key + this.IV));
        }

        public string Encrypt(string input, bool hexInput = false, bool hexOutput = false)
        {
            var ct = ProcessInput(true, input, hexInput);
            var ctStr = hexOutput
                ? Utility.ConvertToHexString(ct)
                : Utility.ConvertToString(ct);
            return ctStr;
        }

        public string Decrypt(string input, bool hexInput = false, bool hexOutput = false)
        {
            var pt = ProcessInput(false, input, hexInput);
            var ptStr = hexOutput
                ? Utility.ConvertToHexString(pt)
                : Utility.ConvertToString(pt);
            return ptStr;
        }

        private byte[] ProcessInput(bool forEncrypting, string input, bool hexInput = false)
        {
            var inBytes = hexInput 
                ? Utility.ConvertHexToBytes(input) 
                : Utility.ConvertToBytes(input);
            var buffer = new byte[inBytes.Length];
            cipher.Init(forEncrypting, cipherParams);
            cipher.ProcessBytes(inBytes, 0, inBytes.Length, buffer, 0);
            cipher.Reset();
            return buffer;
        }
    }
}
