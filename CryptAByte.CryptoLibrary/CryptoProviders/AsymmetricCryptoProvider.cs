using System.Diagnostics;
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
        #region ICryptoProvider Members

        public string EncryptWithKey(string secret, string publicKey)
        {
            return RSAPublicKeyEncryption.EncryptString(secret, 1024, publicKey);
        }

        public string DecryptWithKey(string secret, string privateKey)
        {
            Debug.WriteLine("private key:" + privateKey);
            return RSAPublicKeyEncryption.DecryptString(secret, 1024, privateKey);
        }

        #endregion

        #region message encryption

        // https://code.google.com/p/cryptico/

        public string EncryptMessageWithKey(string message, string publicKey, out string encryptedPassword,
                                            out string hashOfMessage)
        {
            // get a hash of the message
            hashOfMessage = SymmetricCryptoProvider.GetSecureHashForString(message);

            // generate a password:
            string encryptionKeyForAES = SymmetricCryptoProvider.GenerateKeyPhrase();
            Debug.WriteLine("encryptionKey for AES: " + encryptionKeyForAES);

            // encrypt the message with AES using the encryption key
            string encryptedMessage = new SymmetricCryptoProvider().EncryptWithKey(message, encryptionKeyForAES);

            // encrypt the Key:
            encryptedPassword = EncryptWithKey(encryptionKeyForAES, publicKey);
            Debug.WriteLine("encryptedPassword for AES: " + encryptedPassword);

            return encryptedMessage;
        }

        public string DecryptMessageWithKey(string privateKey, string messageData, string encryptedDecryptionKey,
                                            string hashOfMessage, out string encryptionKey)
        {
            Debug.WriteLine("encryptedPassword for AES: " + encryptedDecryptionKey);

            encryptionKey = DecryptWithKey(encryptedDecryptionKey, privateKey);

            Debug.WriteLine("Decrypted password for AES: " + encryptionKey);

            Contract.Assert(encryptionKey != string.Empty, "Encryption key is null");

            // decrypt message
            string decryptedMessage = new SymmetricCryptoProvider().DecryptWithKey(messageData, encryptionKey);

            //verify hash:
            Contract.Assert(SymmetricCryptoProvider.GetSecureHashForString(decryptedMessage) == hashOfMessage,
                            "Original hash does not equal decrypted hash!");

            return decryptedMessage;
        }

        #endregion message encryption

        public static KeyPair GenerateKeys()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            var key = new KeyPair {PublicKey = rsa.ToXmlString(false), PrivateKey = rsa.ToXmlString(true)};

            return key;
        }
    }
}