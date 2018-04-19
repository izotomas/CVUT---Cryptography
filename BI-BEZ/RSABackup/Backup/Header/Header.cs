using System;
using System.Linq;
using System.Security.Cryptography;
using BackupUtility.Crypto;
using Org.BouncyCastle.Crypto;

namespace BackupUtility.Backup.Header
{
    public class Header
    {
        public const int HEADER_SIZE = 1024;
        public const int BYTESTREAM_BYTE_OFFSTET = 4;

        // indexes of header properties
        private const int ALGORITHM_ID_I   = 0;
        private const int CIPHER_MODE_I    = ALGORITHM_ID_I  + 1;
        private const int PADDING_MODE_I   = CIPHER_MODE_I   + 1;
        private const int BLOCKSIZE_I      = PADDING_MODE_I  + 1;
        private const int IV_LEN_I         = BLOCKSIZE_I     + BYTESTREAM_BYTE_OFFSTET;
        private const int KEY_LEN_I        = IV_LEN_I  + BYTESTREAM_BYTE_OFFSTET;
        private const int IV_START_I       = KEY_LEN_I + BYTESTREAM_BYTE_OFFSTET;

        private const int LEN_CONSTANTS    = IV_START_I;
        private readonly byte[] _buffer    = new byte[HEADER_SIZE];
        private readonly int LEN_IV_AND_KEY;
        private readonly int KEY_START_I;

        #region Properties
        

        public byte AlgorithmId
        {
            get => _buffer[ALGORITHM_ID_I];
            private set => _buffer[ALGORITHM_ID_I] = value;
        }

        public byte CipherMode
        {
            get => _buffer[CIPHER_MODE_I];
            private set => _buffer[CIPHER_MODE_I] = value;
        }
        
        public byte PaddingMode
        {
            get => _buffer[PADDING_MODE_I];
            private set => _buffer[PADDING_MODE_I] = value;
        }

        public byte[] BlockSize
        {
            get => HeaderConverter.SubArray(_buffer, BLOCKSIZE_I);
            private set => HeaderConverter.InsertTo(value, _buffer, BLOCKSIZE_I);
        }

        public byte[] IVLength
        {
            get => HeaderConverter.SubArray(_buffer, IV_LEN_I);
            private set => HeaderConverter.InsertTo(value, _buffer, IV_LEN_I);
        }

        public byte[] KeyLength
        {
            get => HeaderConverter.SubArray(_buffer, KEY_LEN_I);
            private set => HeaderConverter.InsertTo(value, _buffer, KEY_LEN_I);
        }

        public byte[] IV
        {
            get => HeaderConverter.SubArray(_buffer, IV_START_I, BitConverter.ToInt32(IVLength, 0) / 8).ToArray();
            private set => HeaderConverter.InsertTo(value, _buffer, IV_START_I);
        }

        public byte[] Key
        {
            //get => _buffer.Skip(KEY_START_I).Take(BitConverter.ToInt32(KeyLength, 0) / 8).ToArray();
            get => HeaderConverter.SubArray(_buffer, KEY_START_I, HeaderConverter.ToInt32(KeyLength) / 8).ToArray();
            private set => Array.Copy(value, 0, _buffer, KEY_START_I, value.Length);
        }


        #endregion

        public Header(AESParameters aes, AsymmetricKeyParameter keyParameter)
        {
            var encryptedKey    = EncryptKey(aes.Key, keyParameter);
            this.LEN_IV_AND_KEY = HeaderConverter.ByteCount(aes.IVLength) + encryptedKey.Length;
            if (!IsWithinBounds())
            {
                throw new ArgumentOutOfRangeException(nameof(aes)+"," + nameof(keyParameter) +": Header capacity of [" + HEADER_SIZE + "]bits exceeded.");
            }

            // single byte properties
            this.AlgorithmId = HeaderConverter.ToByte(aes.AlgorithmId);
            this.CipherMode  = HeaderConverter.ToByte(aes.CipherMode);
            this.PaddingMode = HeaderConverter.ToByte(aes.PaddingMode);

            // multi byte properties
            this.BlockSize   = HeaderConverter.ToBytes(aes.BlockSize);
            this.IVLength    = HeaderConverter.ToBytes(aes.IVLength);
            this.KeyLength   = HeaderConverter.ToBytes(encryptedKey.Length * 8);

            // IV & Key
            this.IV          = HeaderConverter.Copy(aes.IV);
            var ivLen        = HeaderConverter.ByteCount(HeaderConverter.ToInt32(this.IVLength));
            this.KEY_START_I   = IV_START_I + ivLen;
            this.Key         = HeaderConverter.Copy(encryptedKey);
        }

        public Header(byte[] header)
        {
            if (header?.Length != HEADER_SIZE)
            {
                throw new ArgumentException("Header must be exactly " + HEADER_SIZE + " bytes long");
            }

            // single byte properties + check if given enum exists
            HeaderConverter.CrossValidate(this.AlgorithmId = header[ALGORITHM_ID_I], HeaderConverter.ToEnum<BackupUtility.Algorithm>);
            HeaderConverter.CrossValidate(this.CipherMode  = header[CIPHER_MODE_I], HeaderConverter.ToEnum<CipherMode>);
            HeaderConverter.CrossValidate(this.PaddingMode = header[PADDING_MODE_I], HeaderConverter.ToEnum<PaddingMode>);

            // multi byte properties + check convertability to int
            HeaderConverter.CrossValidate(this.BlockSize = HeaderConverter.SubArray(header, BLOCKSIZE_I), HeaderConverter.ToInt32);
            HeaderConverter.CrossValidate(this.IVLength  = HeaderConverter.SubArray(header, IV_LEN_I), HeaderConverter.ToInt32);
            HeaderConverter.CrossValidate(this.KeyLength = HeaderConverter.SubArray(header, KEY_LEN_I), HeaderConverter.ToInt32);
            
            // IV & Key
            var ivByteCount  = HeaderConverter.ByteCount(bits: HeaderConverter.ToInt32(this.IVLength));
            var keyByteCount = HeaderConverter.ByteCount(bits: HeaderConverter.ToInt32(this.KeyLength));
            this.LEN_IV_AND_KEY = ivByteCount + keyByteCount;
            if (!IsWithinBounds())
            {
                throw new ArgumentOutOfRangeException(nameof(header) +": Header capacity of [" + HEADER_SIZE + "]bits exceeded.");
            }
            this.IV          = HeaderConverter.SubArray(header, IV_START_I, ivByteCount);
            this.KEY_START_I   = IV_START_I + ivByteCount;
            this.Key         = HeaderConverter.SubArray(header, this.KEY_START_I, keyByteCount);
        }

        public byte[] ToArray()
        {
            var buff = new byte[HEADER_SIZE];
            Array.Copy(this._buffer, buff, HEADER_SIZE);
            return buff;
        }

        public AESParameters ToAESParameters(AsymmetricKeyParameter privateKey)
        {
            var decryptedKey = DecryptKey(this.Key, privateKey);

            var aes = new AESParameters()
            {
                AlgorithmId = HeaderConverter.ToEnum<BackupUtility.Algorithm>(this.AlgorithmId),
                CipherMode  = HeaderConverter.ToEnum<CipherMode>(this.CipherMode),
                PaddingMode = HeaderConverter.ToEnum<PaddingMode>(this.PaddingMode),
                BlockSize   = HeaderConverter.ToInt32(this.BlockSize),
                IVLength    = HeaderConverter.ToInt32(this.IVLength),
                KeyLength   = decryptedKey.Length * 8,
                IV          = this.IV,
                Key         = decryptedKey
            };
            return aes;
        }

        private bool IsWithinBounds()
        {
            return HEADER_SIZE >= LEN_CONSTANTS + LEN_IV_AND_KEY;
        }

        private static byte[] DecryptKey(byte[] keyBytes, AsymmetricKeyParameter keyParameter)
        {
            return Crypto.RSA.Decrypt(keyBytes, keyParameter);
        }

        private static byte[] EncryptKey(byte[] keyBytes, AsymmetricKeyParameter keyParameters)
        {
            return Crypto.RSA.Encrypt(keyBytes, keyParameters);
        }
    }
}