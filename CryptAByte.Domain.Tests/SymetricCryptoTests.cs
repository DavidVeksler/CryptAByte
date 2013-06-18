using System;
using CryptAByte.CryptoLibrary.CryptoProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptAByte.Domain.Tests
{
    [TestClass]
    public class SymetricCryptoTests
    {
        private const string secret = "this is a secret";
        const string password = "password";

        [TestMethod]
        public void Encrypt_Decrypt_ReturnsOriginalValue()
        {
            var crypto = new SymmetricCryptoProvider();

            var cryptoText = crypto.EncryptWithKey(secret, password);
            var original = crypto.DecryptWithKey(cryptoText, password);

            Assert.AreEqual(secret,original);

        }

        [TestMethod]
        public void Encrypt_Decrypt_Hash_ReturnsOriginalValue()
        {
            var crypto = new SymmetricCryptoProvider();

            string hashedPassword = SymmetricCryptoProvider.GetSecureHashForString(password);

            var cryptoText = crypto.EncryptWithKey(secret, hashedPassword);
            var original = crypto.DecryptWithKey(cryptoText, hashedPassword);

            Assert.AreEqual(secret, original);

        }

    }
}
