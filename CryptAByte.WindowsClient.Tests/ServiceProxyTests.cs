using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptAByte.WindowsClient.Tests
{
    [TestClass]
    public class ServiceProxyTests
    {
        private const string passphrase = "password";

        [TestMethod]
        public void CreateKeyReturnsKey()
        {
            var proxy = new CryptAByte.WindowsClient.Commands.ServiceProxy { ServiceUrl = "http://localhost:60888/Service/" };

            var key = proxy.CreateKey(passphrase);

            Assert.IsTrue(key.KeyId > 0);
        }

        [TestMethod]
        public void SendMessageReturnsOK()
        {
            var proxy = new CryptAByte.WindowsClient.Commands.ServiceProxy { ServiceUrl = "http://localhost:60888/Service/" };

            var key = proxy.CreateKey(passphrase);

            bool success = proxy.SendMessage(key.KeyToken, "secret");

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void GetMessagesReturnsMessages()
        {
            var proxy = new CryptAByte.WindowsClient.Commands.ServiceProxy { ServiceUrl = "http://localhost:60888/Service/" };

            var key = proxy.CreateKey(passphrase);

            proxy.SendMessage(key.KeyToken, "secret");

           var messages = proxy.GetMessages(key.KeyToken, passphrase);

           Assert.IsTrue(messages.Count > 0);
        }



    }
}
