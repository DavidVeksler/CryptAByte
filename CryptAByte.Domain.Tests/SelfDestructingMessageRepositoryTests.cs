using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using CryptAByte.Domain.DataContext;
using CryptAByte.Domain.KeyManager;
using CryptAByte.Domain.SelfDestructingMessaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptAByte.Domain.Tests
{
    [TestClass]
    public class SelfDestructingMessageRepositoryTests
    {
        public SelfDestructingMessageRepositoryTests()
        {
            // update DB if there are changes:
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CryptAByteContext>());
        }

        const string secret = "secret";
        const string passphrase = "password";

        [TestMethod]
        public void Can_Create_And_Read_Message()
        {
            var repository = new SelfDestructingMessageRepository();

            int messageId = repository.StoreMessage(new SelfDestructingMessage() { Message = secret, SentDate = DateTime.Now }, passphrase);

            var retrieved = repository.GetMessage(messageId, passphrase);

            Assert.IsTrue(secret == retrieved.Message);
        }

        [TestMethod]
        public void Can_Create_And_Read_Message_With_Attachment()
        {
            var repository = new SelfDestructingMessageRepository();

            const string attachmentName = "test.txt";

            int messageId = repository.StoreMessage(new SelfDestructingMessage()
            {
                Message = secret,
                SentDate = DateTime.Now,
                SelfDestructingMessageAttachment = new SelfDestructingMessageAttachment() { 
                    //AttachmentName = attachmentName, 
                    Attachment = "TEST DATA" }
            }, passphrase,
            attachmentName,
            Encoding.ASCII.GetBytes("HELLO")
            );

            var retrieved = repository.GetMessage(messageId, passphrase);

            Assert.IsTrue(secret == retrieved.Message);

           // todo Assert.AreEqual(retrieved.SelfDestructingMessageAttachment.AttachmentName, attachmentName);

            var attachment = repository.GetAttachment(messageId, passphrase);
            Assert.IsNotNull(attachment);

            // todo:
            //Encoding.ASCII.GetBytes("HELLO")
            
        }

        [TestMethod]
        public void Cannot_Read_Messages_After_Retrieve()
        {
            var repository = new SelfDestructingMessageRepository();

            int messageId = repository.StoreMessage(new SelfDestructingMessage() { Message = secret, SentDate = DateTime.Now }, passphrase);

            var retrieved = repository.GetMessage(messageId, passphrase);

            try
            {
                repository.GetMessage(messageId, passphrase);

                Assert.Fail("Should not be able to succeed");
            }
            catch
            {
                // this should fail
            }


        }

    }
}
