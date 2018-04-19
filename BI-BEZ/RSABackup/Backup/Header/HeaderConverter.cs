using System;
using System.Linq;

namespace BackupUtility.Backup.Header
{
    public static class HeaderConverter
    { 
        public static byte[] SubArray(byte[] source, int startIndex, int lenght = Header.BYTESTREAM_BYTE_OFFSTET)
        {
            return source.Skip(startIndex).Take(lenght).ToArray();
        }

        public static void InsertTo(byte[] source, byte[] destination, int destStartIndex)
        {
            Array.Copy(source, 0, destination, destStartIndex, source.Length);
        }

        public static void CrossValidate<T, TResult>(T input, Func<T, TResult> validationAction)
        {
            validationAction(input);
        }

        public static int ByteCount(int bits)
        {
            var len = (int) Math.Ceiling(bits / 8.0);
            return len;
        }

        public static byte ToByte<T>(T en) where T : IComparable, IFormattable, IConvertible
        {
            try
            {
                return Convert.ToByte(en);
            }
            catch (InvalidCastException e)
            {
                throw new HeaderException(e.Message);
            }
        }

        public static byte[] ToBytes(int i)
        {
            return BitConverter.GetBytes(i);
        }

        public static byte[] Copy(byte[] source)
        {
            if (source == null)
            {
                throw new HeaderException(nameof(source) + ": null parameter");
            }

            var buff = new byte[source.Length];
            Array.Copy(source, buff, source.Length);
            return buff;
        }

        public static T ToEnum<T>(byte b) where T : IComparable, IFormattable, IConvertible
        {
            try
            {
                var i = Convert.ToInt32(b);
                return (T) (object) i;
            }
            catch (InvalidCastException e)
            {
                throw new HeaderException(e.Message);
            }
        }

        public static int ToInt32(byte[] b)
        {
            try
            {
                return BitConverter.ToInt32(b, 0);
            }
            catch (ArgumentException e)
            {
                throw new HeaderException(e.Message);
            }
        }
    }
}
