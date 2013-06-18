using System;
using System.Diagnostics;
using CryptAByte.CryptoLibrary.CryptoProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptAByte.Domain.Tests
{
    [TestClass]
    public class AssymetricCryptoTests
    {
        private const string secret = "d379506609775a01f30990c2b83788baf49c7b1ba56d40423c4c95f6e80e7da1";
        

        [TestMethod]
        public void GenerateKeys_ReturnsKeys()
        {
            var key = AsymmetricCryptoProvider.GenerateKeys();
            Assert.IsNotNull(key.PrivateKey);
            Assert.IsNotNull(key.PublicKey);
        }

        [TestMethod]
        public void Encrypt_Decrypt_ReturnsOriginalValue()
        {
            var crypto = new AsymmetricCryptoProvider();
            var key = AsymmetricCryptoProvider.GenerateKeys();

            var cryptoText = crypto.EncryptWithKey(secret, key.PublicKey);
            var original = crypto.DecryptWithKey(cryptoText, key.PrivateKey);

            Assert.AreEqual(secret,original);

        }

        [TestMethod]
        public void Message_Encrypt_Decrypt_ReturnsOriginalValue()
        {
            // Arrange
            var crypto = new AsymmetricCryptoProvider();
            var key = AsymmetricCryptoProvider.GenerateKeys();
            string hash;
            string encryptedPassword;

            // Act
            var encryptedMessage = crypto.EncryptMessageWithKey(secret, key.PublicKey, out encryptedPassword, out hash);
            
            string messageDecryptionKey;

            var decryptedSecret = crypto.DecryptMessageWithKey(key.PrivateKey, encryptedMessage, encryptedPassword, hash, out messageDecryptionKey);

            // Assert
            Assert.AreEqual(secret, decryptedSecret);
            Assert.AreEqual(SymmetricCryptoProvider.GetSecureHashForString(secret),hash,"hashes do not match");

        }
    }
}
