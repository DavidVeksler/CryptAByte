using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CryptAByte.CryptoLibrary.CryptoProviders;
using CryptAByte.CryptoLibrary.Functional;
using CryptAByte.Domain.DataContext;
using CryptAByte.Domain.KeyManager;
using CryptAByte.Domain.Services;
using Ionic.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CryptAByte.Domain.Tests
{
    /// <summary>
    ///   Summary description for KeyRepositoryTests
    /// </summary>
    [TestClass]
    public class KeyRepositoryTests
    {

        #region Test Setup

        private const string password = "password";
        private const string message = "secret message";

        public KeyRepositoryTests()
        {
            // update DB if there are changes:
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CryptAByteContext>());
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        ///<summary>
        ///  Gets or sets the test context which provides information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        #region Can Create Keys

        [TestMethod]
        public void Can_Create_AndRetrieveKeys()
        {
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithGeneratedKeys(DateTime.Now);
            repository.AddRequest(request);
            var retrieved = repository.GetRequest(request.KeyToken);
            Assert.AreEqual(request.KeyToken, retrieved.KeyToken);
        }

        [TestMethod]
        public async Task Can_CreateRequestWithPublicKey_AndRetrieveKeys()
        {
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);

            var keys = AsymmetricCryptoProvider.GenerateKeys();

            var encryptedPrivateKey = new SymmetricCryptoProvider().EncryptWithKey(keys.PrivateKey, password);

            var request = CryptoKey.CreateWithProvidedKeys(keys.PublicKey, encryptedPrivateKey, true);

            repository.AddRequest(request);

            await repository.AttachMessageToRequestAsync(request.KeyToken, message);

            var retrieved = repository.GetRequest(request.KeyToken);

            var msg = repository.GetDecryptedMessagesWithPassphrase(request.KeyToken, password);

            Assert.IsTrue(request.IsPrivateKeyEncrypted);

            Assert.IsTrue(request.KeyToken == retrieved.KeyToken);

            Assert.IsTrue(msg.First().MessageData == message);
        }

        [TestMethod]
        public void Can_Create_AndRetrieveKeysWithNotifications()
        {
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithGeneratedKeys(DateTime.Now);

            const string email = "heroic@gmail.com";
            request.Notifications.Add(new Notification {Email = email});

            repository.AddRequest(request);
            var retrieved = repository.GetRequest(request.KeyToken);
            Assert.AreEqual(email, retrieved.Notifications.First().Email);
        }

        #endregion Can Create Keys

        [TestMethod]
        public void Can_Create_AndRetrieveKeysWithMessages()
        {
            // Arrange:
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithGeneratedKeys(DateTime.Now);
            const string message = "secret message";
            request.Messages.Add(new Message {MessageData = message});

            // Act:
            repository.AddRequest(request);
            var retrieved = repository.GetRequest(request.KeyToken);
            var retrievedMessage = retrieved.Messages.First().MessageData;

            // Assert:
            Assert.AreEqual(message, retrievedMessage);
        }

        [TestMethod]
        public async Task Can_Create_AndRetrieveEncryptedMessages()
        {
            // Arrange:
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithGeneratedKeys(DateTime.Now);

            const string message = "secret message";
            //request.Messages.Add(new Message { MessageData = message });

            // Act:
            repository.AddRequest(request);
            await repository.AttachMessageToRequestAsync(request.KeyToken, message);

            var repository2 = new RequestRepository(context, mockEmailService.Object);

            var messages = repository2.GetEncryptedMessages(request.KeyToken, request.PrivateKeyHash);

            // Assert:
            Assert.IsTrue(messages.Count() == 1);
        }

        [TestMethod]
        public void Create_Message_Encrypt_Decrypt_Verify()
        {
            // Arrange:
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithGeneratedKeys(DateTime.Now);
            var keys = AsymmetricCryptoProvider.GenerateKeys();
            var symmetricProvider = new SymmetricCryptoProvider();
            var randomGenerator = new CryptoRandomGenerator();
            var crypto = new AsymmetricCryptoProvider(symmetricProvider, randomGenerator);
            const string message = "secret message";

            string encryptedPassword;
            string hash;
#pragma warning disable CS0618 // Type or member is obsolete
            string encryptedMessage = crypto.EncryptMessageWithKey(message, keys.PublicKey, out encryptedPassword,
                                                                   out hash);
#pragma warning restore CS0618 // Type or member is obsolete

            request.Messages.Add(new Message
                {MessageData = encryptedMessage, EncryptionKey = encryptedPassword, MessageHash = hash});

            // Act:
            repository.AddRequest(request);
            var retrieved = repository.GetRequest(request.KeyToken);
            var retrievedMessage = retrieved.Messages.First();

            string messageDecryptionKey;

#pragma warning disable CS0618 // Type or member is obsolete
            var decryptedMessage = crypto.DecryptMessageWithKey(keys.PrivateKey, retrievedMessage.MessageData,
                                                                retrievedMessage.EncryptionKey,
                                                                retrievedMessage.MessageHash, out messageDecryptionKey);
#pragma warning restore CS0618 // Type or member is obsolete

            // Assert:
            Assert.AreEqual(message, decryptedMessage);
        }


        [TestMethod]
        public async Task Create_Request_AttachMessage_Decrypt_Verify()
        {
            // Arrange:
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithGeneratedKeys(DateTime.Now);
            const string message = "secret message";

            // Act:
            repository.AddRequest(request);
            await repository.AttachMessageToRequestAsync(request.KeyToken, message);

            var decryptedMessages = repository.GetDecryptedMessagesWithPrivateKey(request.KeyToken,
                                                                                  request.GetPrivateKey);

            // Assert:
            Assert.IsTrue(decryptedMessages.Count > 0);
            Assert.AreEqual(message, decryptedMessages.First().MessageData);
        }

        [TestMethod]
        public async Task Cannot_decrypt_locked_request()
        {
            // Arrange:
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithGeneratedKeys(DateTime.Now.AddDays(1));
            const string message = "secret message";

            // Act:
            repository.AddRequest(request);
            await repository.AttachMessageToRequestAsync(request.KeyToken, message);

            try
            {
                var decryptedMessages = repository.GetDecryptedMessagesWithPrivateKey(request.KeyToken,
                                                                                      request.GetPrivateKey);
                Assert.Fail();
            }
            catch (Exception ex)
            {
            }
        }

        [TestMethod]
        public async Task Create_Request_With_Passphrase_AttachMessage_Decrypt_Verify()
        {
            // Arrange:
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithPassphraseProtectedKeys(password);

            // Act:
            repository.AddRequest(request);
            await repository.AttachMessageToRequestAsync(request.KeyToken, message);

            var decryptedMessages = repository.GetDecryptedMessagesWithPassphrase(request.KeyToken, password);

            // Assert:
            Assert.IsTrue(decryptedMessages.Count > 0);
            Assert.AreEqual(message, decryptedMessages.First().MessageData);
        }

        [TestMethod]
        public async Task DeleteMessagesAfterReading()
        {
            // Arrange:
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithPassphraseProtectedKeys(password);
            request.DeleteMessagesAfterReading = true;

            // Act:
            repository.AddRequest(request);
            await repository.AttachMessageToRequestAsync(request.KeyToken, message);

            var decryptedMessages = repository.GetDecryptedMessagesWithPassphrase(request.KeyToken, password);

            var decryptedMessages2 = repository.GetDecryptedMessagesWithPassphrase(request.KeyToken, password);

            // Assert:
            Assert.IsTrue(decryptedMessages.Count > 0);
            Assert.IsTrue(decryptedMessages2.Count == 0);
        }

        [TestMethod]
        public async Task DeleteKeyAfterReading()
        {
            // Arrange:
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithPassphraseProtectedKeys(password);
            request.DeleteKeyAfterReading = true;

            // Act:
            repository.AddRequest(request);
            await repository.AttachMessageToRequestAsync(request.KeyToken, message);

            var decryptedMessages = repository.GetDecryptedMessagesWithPassphrase(request.KeyToken, password);

            var request2 = repository.GetRequest(request.KeyToken);

            // Assert:
            Assert.IsNull(request2);
        }

        [TestMethod]
        public async Task NotifyOnMessageReceived()
        {
            // Arrange:
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithPassphraseProtectedKeys(password);
            request.Notifications.Add(new Notification {Email = "heroic@gmail.com"});

            // Act:
            repository.AddRequest(request);
            await repository.AttachMessageToRequestAsync(request.KeyToken, message);

            //Mock<IRequestRepository> mock = new Mock<IRequestRepository>();

            //mock.Setup(m=> m.NotifyOnMessageReceived(request.KeyToken)).Verifiable("Sends notification");

            //mock.Verify(m=> m.NotifyOnMessageReceived(request.KeyToken));

            //   var decryptedMessages = repository.GetDecryptedMessagesWithPassphrase(request.KeyToken, password);

            //    var request2 = repository.GetRequest(request.KeyToken);

            // Assert:
            //   Assert.IsNull(request2);
        }

        [TestMethod]
        public void DeleteKeyFromRepository()
        {
            // Arrange:
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithPassphraseProtectedKeys(password);
            request.DeleteKeyAfterReading = true;

            // Act:
            repository.AddRequest(request);
            repository.DeleteKeyWithPassphrase(request.KeyToken, password);

            // Assert:

            var request2 = repository.GetRequest(request.KeyToken);

            Assert.IsNull(request2);
        }

        [TestMethod]
        public void DeleteKeyWithInvalidpasswordFromRepositoryFails()
        {
            // Arrange:
            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithPassphraseProtectedKeys(password);
            request.DeleteKeyAfterReading = true;

            // Act:
            repository.AddRequest(request);

            // Assert:

            try
            {
                repository.DeleteKeyWithPassphrase(request.KeyToken, "");
                Assert.Fail("This should not work.");
            }
            catch (Exception)
            {
            }
        }


        [TestMethod]
        public async Task Create_Request_With_Passphrase_AttachFile_Decrypt_Verify()
        {
            // Arrange:

            string fileName = AssemblyDirectory + @"\Test.PNG";
            byte[] fileBytes = File.ReadAllBytes(fileName);

            var context = new CryptAByteContext();
            var mockEmailService = new Mock<IEmailService>();
            var repository = new RequestRepository(context, mockEmailService.Object);
            var request = CryptoKey.CreateWithPassphraseProtectedKeys(password);

            // Act:
            repository.AddRequest(request);
            await repository.AttachFileToRequestAsync(request.KeyToken, fileBytes, fileName);
            var decryptedMessages = repository.GetDecryptedMessagesWithPassphrase(request.KeyToken, password);

            // Assert:
            Assert.IsTrue(decryptedMessages.Count > 0);

            byte[] decryptedString = Convert.FromBase64String(decryptedMessages.First().MessageData);

            var zipStream = new MemoryStream(decryptedString);

            using (ZipFile zip = ZipFile.Read(zipStream))
            {
                var files = zip.ToList();
                Assert.IsTrue(files.Count == 1);
            }
        }
    }
}