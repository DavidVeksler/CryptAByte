using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CryptAByte.Domain.KeyManager;

namespace CryptAByte.Domain.Tests
{
    // TODO
    [Ignore]
    [TestClass]
    public class KeySecurityTests
    {
        private const string secret = "This is a secret";
        
        // Time Lock Process

        // 1.	Request key + release date
        //2.	Grant public key + key token
        //3.	Request private key via key token
        //4.	Grant private key 

        [TestMethod]
        public void CannotGetKeyBeforeReleaseDate()
        {
            var request = CryptoKey.CreateWithGeneratedKeys(DateTime.Now.AddDays(1));
            Assert.IsNull(request.GetPrivateKey);
        }

    }
}
