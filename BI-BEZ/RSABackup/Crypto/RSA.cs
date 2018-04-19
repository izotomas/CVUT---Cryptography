using System;
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;

namespace BackupUtility.Crypto
{
    public static class RSA
    {
        public static T ParsePEM<T>(string keyFileName)
        {
            if (typeof(T) != typeof(AsymmetricKeyParameter) 
                && typeof(T) != typeof(AsymmetricCipherKeyPair))
            {
                throw new ArgumentException("Only AsymmetricKeyParameter and AsymmetricCipherKeyPair are supported");
            }

            using (var reader = File.OpenText(keyFileName))
            {
                try
                {
                    var t = (T) new PemReader(reader).ReadObject();
                    if (t == null)
                    {
                        throw new ArgumentException("EncryptionKey reading failed.");
                    }
                    return t;
                }
                catch (InvalidCastException e)
                {
                    throw new ArgumentException(e.Message);
                }
            }
        }

        public static byte[] Encrypt(byte[] data, AsymmetricKeyParameter publicKey)
        {
            if (publicKey == null || publicKey.IsPrivate)
            {
                throw new ArgumentException("Not a valid encryption key");
            }

            // purely mathematical RSA
            var engine = new RsaEngine();
            // use padding
            var cipher = new OaepEncoding(engine);
            cipher.Init(true, publicKey);

            var length = data.Length;
            var blockSize = engine.GetInputBlockSize();
            var cipherTextBytes = new List<byte>();

            for (var i = 0; i < length; i += blockSize)
            {
                var chunkSize = Math.Min(blockSize, length - i);
                cipherTextBytes.AddRange(engine.ProcessBlock(data, i, chunkSize));
            }

            return cipherTextBytes.ToArray();
        }


        public static byte[] Decrypt(byte[] data, AsymmetricKeyParameter privateKey)
        {
            if (privateKey == null || !privateKey.IsPrivate)
            {
                throw new ArgumentException("Not a valid decryption key");
            }
            // purely mathematical RSA
            var engine = new RsaEngine();
            // use padding
            var cipher = new OaepEncoding(engine);
            cipher.Init(false, privateKey);

            var length = data.Length;
            var blockSize = engine.GetInputBlockSize();
            var plainTextBytes = new List<byte>();

            for (var i = 0; i < length; i += blockSize)
            {
                var chunkSize = Math.Min(blockSize, length - i);
                plainTextBytes.AddRange(engine.ProcessBlock(data, i, chunkSize));
            }

            return plainTextBytes.ToArray();
        }
    }
}
