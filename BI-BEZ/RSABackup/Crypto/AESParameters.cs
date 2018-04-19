using System.Security.Cryptography;

namespace BackupUtility.Crypto
{
   

    public class AESParameters
    {
        public const int DEFAULT_BLOCKSIZE_BITCOUNT = 128;
        public const int DEFAULT_KEY_BITCOUNT = 256;

        public Backup.BackupUtility.Algorithm AlgorithmId { get; set; }
        public CipherMode CipherMode { get; set; }
        public PaddingMode PaddingMode { get; set; }
        public int BlockSize { get; set; }
        public int KeyLength { get; set; }
        public int IVLength { get; set; }
        public byte[] IV { get; set; }
        public byte[] Key { get; set; }


    }
}
