namespace RC4.Cipher
{
    public interface IRC4
    {
        string Encrypt(string input, bool hexInput = false, bool hexOutput = false);
        string Decrypt(string input, bool hexInput = false, bool hexOutPut = false);
    }
}
