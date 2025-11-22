using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using CryptAByte.CryptoLibrary.EncryptionLibraries;

namespace CryptAByte.CryptoLibrary.CryptoProviders
{
    public class KeyPair
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }

    public class AsymmetricCryptoProvider : ICryptoProvider
    {
        private const int RsaKeySize = 1024;

        public string EncryptWithKey(string secret, string publicKey)
        {
            return RSAPublicKeyEncryption.EncryptString(secret, RsaKeySize, publicKey);
        }

        public string DecryptWithKey(string secret, string privateKey)
        {
            return RSAPublicKeyEncryption.DecryptString(secret, RsaKeySize, privateKey);
        }

        public string EncryptMessageWithKey(string message, string publicKey, out string encryptedPassword,
                                            out string hashOfMessage)
        {
            hashOfMessage = SymmetricCryptoProvider.GetSecureHashForString(message);

            string encryptionKeyForAES = SymmetricCryptoProvider.GenerateKeyPhrase();

            string encryptedMessage = new SymmetricCryptoProvider().EncryptWithKey(message, encryptionKeyForAES);

            encryptedPassword = EncryptWithKey(encryptionKeyForAES, publicKey);

            return encryptedMessage;
        }

        public string DecryptMessageWithKey(string privateKey, string messageData, string encryptedDecryptionKey,
                                            string hashOfMessage, out string encryptionKey)
        {
            encryptionKey = DecryptWithKey(encryptedDecryptionKey, privateKey);

            Contract.Assert(encryptionKey != string.Empty, "Encryption key is null");

            string decryptedMessage = new SymmetricCryptoProvider().DecryptWithKey(messageData, encryptionKey);

            Contract.Assert(SymmetricCryptoProvider.GetSecureHashForString(decryptedMessage) == hashOfMessage,
                            "Original hash does not equal decrypted hash!");

            return decryptedMessage;
        }

        public static KeyPair GenerateKeys()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(RsaKeySize);

            var key = new KeyPair {PublicKey = rsa.ToXmlString(false), PrivateKey = rsa.ToXmlString(true)};

            return key;
        }
    }
}