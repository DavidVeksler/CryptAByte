using System;
using System.Collections.Generic;
using System.Linq;
using CryptAByte.CryptoLibrary;
using CryptAByte.CryptoLibrary.CryptoProviders;
using CryptAByte.Domain.DataContext;
using CryptAByte.Domain.Utilities;

namespace CryptAByte.Domain.SelfDestructingMessaging
{
    public class SelfDestructingMessageRepository : ISelfDestructingMessageRepository
    {
        private readonly CryptAByteContext _context;

        public SelfDestructingMessageRepository(CryptAByteContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int StoreMessage(SelfDestructingMessage selfDestructingMessage, string passphrase, string attachmentName = null,
                                byte[] attachmentData = null)
        {
            if (selfDestructingMessage == null)
                throw new ArgumentNullException(nameof(selfDestructingMessage), "Self-destructing message is required.");

            selfDestructingMessage.Message = GzipCompression.Compress(selfDestructingMessage.Message);


            var crypto = new SymmetricCryptoProvider();
            selfDestructingMessage.Message = crypto.EncryptWithKey(selfDestructingMessage.Message, passphrase);

            SelfDestructingMessageAttachment attachment = null;

            // save attachment, if it exists
            if (attachmentData != null && attachmentData.Length > 0)
            {
                var compressedFileResult = FileUtilities.CompressAndEncodeFile(attachmentName, attachmentData);
                var compressedFile = compressedFileResult.Match(
                    onSuccess: value => value,
                    onFailure: error => throw new InvalidOperationException($"Failed to compress file: {error}")
                );

                attachment = new SelfDestructingMessageAttachment
                {
                    Attachment = crypto.EncryptWithKey(compressedFile, passphrase),
                    SentDate = DateTime.Now
                };
            }

            _context.SelfDestructingMessages.Add(selfDestructingMessage);
            _context.SaveChanges();

            if (attachment != null)
            {
                attachment.MessageId = selfDestructingMessage.MessageId;
                _context.SelfDestructingMessageAttachments.Add(attachment);

                _context.ChangeTracker.DetectChanges();
                _context.SaveChanges();
            }

            return selfDestructingMessage.MessageId;
        }

        public SelfDestructingMessage GetMessage(int messageId, string passphrase)
        {
            SelfDestructingMessage message = _context.SelfDestructingMessages.SingleOrDefault(m => m.MessageId == messageId);

            if (message == null)
                throw new KeyNotFoundException($"Message with ID {messageId} not found. Was it already read?");

            var crypto = new SymmetricCryptoProvider();


            try
            {
                message.Message = crypto.DecryptWithKey(message.Message, passphrase);

                var attachment = _context.SelfDestructingMessageAttachments.FirstOrDefault(a => a.MessageId == messageId);

                if (attachment != null)
                {
                    message.HasAttachment = true;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error decrypting message. Invalid passphrase.", nameof(passphrase), ex);
            }

            message.Message = GzipCompression.Decompress(message.Message);

            _context.SelfDestructingMessages.Remove(message);
            _context.SaveChanges();

            return message;

        }

        public SelfDestructingMessageAttachment GetAttachment(int messageId, string passphrase)
        {
            var crypto = new SymmetricCryptoProvider();

            var attachment = _context.SelfDestructingMessageAttachments.SingleOrDefault(m => m.MessageId == messageId);

            if (attachment != null)
            {
                attachment.Attachment = crypto.DecryptWithKey(attachment.Attachment, passphrase);
                _context.SelfDestructingMessageAttachments.Remove(attachment);
            }
            _context.SaveChanges();

            return attachment;
        }


    }
}
