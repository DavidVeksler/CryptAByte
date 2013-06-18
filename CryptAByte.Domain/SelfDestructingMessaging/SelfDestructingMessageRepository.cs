using System;
using System.IO;
using System.Linq;
using CryptAByte.CryptoLibrary;
using CryptAByte.CryptoLibrary.CryptoProviders;
using CryptAByte.Domain.DataContext;
using CryptAByte.Domain.KeyManager;
using Ionic.Zip;

namespace CryptAByte.Domain.SelfDestructingMessaging
{
    public class SelfDestructingMessageRepository : ISelfDestructingMessageRepository
    {
        public int StoreMessage(SelfDestructingMessage selfDestructingMessage, string passphrase, string attachmentName = null,
                                byte[] attachmentData = null)
        {
            if (selfDestructingMessage == null)
            {
                throw new ArgumentOutOfRangeException("selfDestructingMessage required");
            }

            selfDestructingMessage.Message = GzipCompression.Compress(selfDestructingMessage.Message);


            var crypto = new SymmetricCryptoProvider();
            selfDestructingMessage.Message = crypto.EncryptWithKey(selfDestructingMessage.Message, passphrase);

            var db = new CryptAByteContext();
            SelfDestructingMessageAttachment attachment = null;

            // save attachment, if it exists
            if (attachmentData != null && attachmentData.Length > 0)
            {
                MemoryStream streamOfOriginalFile = new MemoryStream(1024);

                using (ZipFile zip = new ZipFile())
                {
                    zip.AddEntry(attachmentName, attachmentData);
                    // zip.AddEntry(self, fileData);
                    zip.Save(streamOfOriginalFile);
                }

                byte[] zippedFile = RequestRepository.ReadFully(streamOfOriginalFile);
                string fileAsString = Convert.ToBase64String(zippedFile);

                attachment = new SelfDestructingMessageAttachment { Attachment = fileAsString };

                attachment.Attachment = crypto.EncryptWithKey(fileAsString, passphrase);
                attachment.SentDate = DateTime.Now;

                //db.SelfDestructingMessageAttachments.Add(attachment);
            }

            db.SelfDestructingMessages.Add(selfDestructingMessage);
            db.SaveChanges();

            if (attachment != null)
            {
                attachment.MessageId = selfDestructingMessage.MessageId;
                db.SelfDestructingMessageAttachments.Add(attachment);

                db.ChangeTracker.DetectChanges();
                db.SaveChanges();
            }

            return selfDestructingMessage.MessageId;
        }

        public SelfDestructingMessage GetMessage(int messageId, string passphrase)
        {
            var db = new CryptAByteContext();

            SelfDestructingMessage message = db.SelfDestructingMessages.SingleOrDefault(m => m.MessageId == messageId);
            //.Include("SelfDestructingMessageAttachment")

            if (message == null)
            {
                throw new ArgumentOutOfRangeException("messageId", "Message not found.  Was it already read?");
            }

            var crypto = new SymmetricCryptoProvider();


            try
            {
                message.Message = crypto.DecryptWithKey(message.Message, passphrase);

                var attachment = db.SelfDestructingMessageAttachments.FirstOrDefault(a => a.MessageId == messageId);

                if (attachment != null)
                {
                    message.HasAttachment = true;
                    // todo: get filename here

                }
            }
            catch (Exception)
            {
                throw new ArgumentOutOfRangeException("passphrase", "server error decrypting message");
            }

            message.Message = GzipCompression.Decompress(message.Message);

            db.SelfDestructingMessages.Remove(message);
            db.SaveChanges();

            message.SelfDestructingMessageAttachment = new SelfDestructingMessageAttachment
                {
                    //   AttachmentName = attachmentName
                };

            return message;

        }

        public SelfDestructingMessageAttachment GetAttachment(int messageId, string passphrase)
        {
            var db = new CryptAByteContext();
            var crypto = new SymmetricCryptoProvider();

            var attachment = db.SelfDestructingMessageAttachments.SingleOrDefault(m => m.MessageId == messageId);

            if (attachment != null)
            {
                attachment.Attachment = crypto.DecryptWithKey(attachment.Attachment, passphrase);

                db.SelfDestructingMessageAttachments.Remove(attachment);

                // todo: move decompression to this class
            }
            db.SaveChanges();

            return attachment;
        }


    }
}
