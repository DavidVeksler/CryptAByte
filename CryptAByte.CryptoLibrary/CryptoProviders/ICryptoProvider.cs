namespace CryptAByte.CryptoLibrary.CryptoProviders
{
    public interface ICryptoProvider
    {
        string EncryptWithKey(string secret, string key);

        string DecryptWithKey(string secret, string key);

        // dynamic GenerateKey();

        //string[] GeneratePublicPrivateKeyPair();

        //string GetSecureHashForString(string original);
    }
}