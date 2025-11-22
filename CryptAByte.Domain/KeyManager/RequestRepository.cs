using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CryptAByte.CryptoLibrary;
using CryptAByte.CryptoLibrary.CryptoProviders;
using CryptAByte.Domain.DataContext;
using CryptAByte.Domain.Services;
using CryptAByte.Domain.Utilities;

namespace CryptAByte.Domain.KeyManager
{
    public class RequestRepository : IRequestRepository
    {
        private readonly CryptAByteContext _context;
        private readonly IEmailService _emailService;

        public RequestRepository(CryptAByteContext context, IEmailService emailService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public void AddRequest(CryptoKey request)
        {
            _context.Keys.Add(request);
            _context.SaveChanges();
        }

        public CryptoKey GetRequest(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token/Identifier is required to retrieve the key!", nameof(token));

            var request = _context.Keys.Include("Messages").Include("Notifications").SingleOrDefault(key => key.KeyToken.Equals(token));

            if (request == null)
                return null;

            if (!request.IsReleased)
            {
                request.PrivateKey = null;
            }

            return request;
        }

        public async Task AttachMessageToRequestAsync(string token, string plainTextMessage)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token/Identifier is required to attach message!", nameof(token));

            string compressedMessage = GzipCompression.Compress(plainTextMessage);

            AttachDataToKey(token, compressedMessage, false);

            await NotifyKeyOwnerOfNewMessageAsync(token).ConfigureAwait(false);
        }

        public async Task AttachEncryptedMessageToRequestAsync(string token, string encryptedMessage, string encryptionKey)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token/Identifier is required to attach message!", nameof(token));

            AttachDataToKey(token, encryptedMessage, false, encryptionKey);

            await NotifyKeyOwnerOfNewMessageAsync(token).ConfigureAwait(false);
        }

        public async Task AttachFileToRequestAsync(string token, byte[] fileData, string fileName)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token/Identifier is required to attach message!", nameof(token));

            string compressedFile = FileUtilities.CompressAndEncodeFile(fileName, fileData);
            AttachDataToKey(token, compressedFile, true);
            await NotifyKeyOwnerOfNewMessageAsync(token).ConfigureAwait(false);
        }

        private void AttachDataToKey(string token, string compressedMessage, bool isFile, string encryptionKey = null)
        {
            string hash = null;
            string encryptedPassword;

            var request = _context.Keys.SingleOrDefault(key => key.KeyToken == token);

            string encryptedMessage;

            if (string.IsNullOrWhiteSpace(encryptionKey))
            {
                var crypto = new AsymmetricCryptoProvider();
                encryptedMessage = crypto.EncryptMessageWithKey(compressedMessage, request.PublicKey, out encryptedPassword,
                                                                       out hash);    
            }
            else
            {
                encryptedMessage = compressedMessage;
                encryptedPassword = encryptionKey;
            }

            if (request.Messages == null) request.Messages = new Collection<Message>();

            request.Messages.Add(new Message()
            {
                MessageData = encryptedMessage,
                EncryptionKey = encryptedPassword,
                MessageHash = hash,
                Created = DateTime.Now,
                IsFile = isFile
            });

            _context.SaveChanges();
        }

        public List<Message> GetDecryptedMessagesWithPassphrase(string keyToken, string passphrase)
        {
            if (string.IsNullOrWhiteSpace(keyToken))
                throw new ArgumentException("Token/Identifier is required to retrieve the messages!", nameof(keyToken));

            var request = _context.Keys.Include("Messages").SingleOrDefault(key => key.KeyToken == keyToken);

            if (request == null)
                throw new KeyNotFoundException($"Request not found for token: {keyToken}");

            try
            {
                string privateKey = new SymmetricCryptoProvider().DecryptWithKey(request.PrivateKey, passphrase);
                return GetDecryptedMessagesWithPrivateKey(keyToken, privateKey);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentException("Error decrypting private key. Invalid passphrase.", nameof(passphrase), ex);
            }
            catch (CryptographicException ex)
            {
                throw new ArgumentException("Error decrypting private key. Invalid passphrase.", nameof(passphrase), ex);
            }

        }

        public Message GetMessageByMessageId(int messageId)
        {
            return _context.Messages.SingleOrDefault(m => m.MessageId == messageId);
        }

        public void DeleteKeyWithPassphrase(string token, string passphrase)
        {
            var key = _context.Keys.Include("Messages").SingleOrDefault(k => k.KeyToken == token);

            if (key == null)
                throw new KeyNotFoundException($"Key for token '{token}' not found. Was it already deleted?");

            var crypto = new SymmetricCryptoProvider();

            try
            {
                var plaintext = crypto.DecryptWithKey(key.PrivateKey, passphrase);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Failed to verify passphrase.  A correct passphrase is required to verify the delete request.");
            }

            _context.Keys.Remove(key);
            _context.SaveChanges();
        }

        public List<Message> GetEncryptedMessages(string token, string privateKeyHash)
        {
            var request = _context.Keys.Include("Messages").SingleOrDefault(key => key.KeyToken == token);

            if (request == null)
                throw new KeyNotFoundException($"Request not found for token: {token}");

            if (!request.IsReleased)
                throw new InvalidOperationException("Request is not released yet.");

            if (request.PrivateKeyHash != privateKeyHash)
                throw new UnauthorizedAccessException("Private key hash does not match stored hash.");

            return request.Messages.ToList();
            
        }

        public List<Message> GetDecryptedMessagesWithPrivateKey(string token, string privateKey)
        {
            var request = _context.Keys.Include("Messages").SingleOrDefault(key => key.KeyToken == token);

            if (request == null)
                throw new KeyNotFoundException($"Request not found for token: {token}");

            if (!request.IsReleased)
                throw new InvalidOperationException("Request is not released yet.");

            var crypto = new AsymmetricCryptoProvider();

            var plaintextMessages = new List<Message>();

            if (request.Messages != null)
            {
                request.Messages.ToList().ForEach(retrievedMessage =>
                                                      {
                                                          string messageDecryptionKey;

                                                          var decryptedMessage = crypto.DecryptMessageWithKey(privateKey, retrievedMessage.MessageData,
                                                                    retrievedMessage.EncryptionKey,
                                                                    retrievedMessage.MessageHash, out messageDecryptionKey);

                                                          retrievedMessage.MessageData = decryptedMessage;
                                                          retrievedMessage.EncryptionKey = messageDecryptionKey;

                                                          if (!retrievedMessage.IsFile)
                                                          {
                                                              retrievedMessage.MessageData =
                                                                  GzipCompression.Decompress(retrievedMessage.MessageData);
                                                          }

                                                          plaintextMessages.Add(retrievedMessage);


                                                      });

                if (request.DeleteMessagesAfterReading || request.DeleteKeyAfterReading)
                {
                    if (request.DeleteMessagesAfterReading)
                    {
                        request.Messages.ToList().ForEach(message => _context.Messages.Remove(message));
                    }

                    if (request.DeleteKeyAfterReading)
                    {
                        _context.Keys.Remove(request);
                    }

                    _context.SaveChanges();
                }
            }

            return plaintextMessages;
        }

        private async Task NotifyKeyOwnerOfNewMessageAsync(string keyToken)
        {
            try
            {
                var cryptoKey = _context.Keys.Include("Notifications").SingleOrDefault(key => key.KeyToken == keyToken);

                if (cryptoKey?.Notifications == null || !cryptoKey.Notifications.Any())
                    return;

                foreach (var notification in cryptoKey.Notifications)
                {
                    string emailBody = $"You have received a message at {DateTime.Now}. You can check it at https://cryptabyte.com/#{cryptoKey.KeyToken}.";
                    await _emailService.SendEmailAsync(notification.Email, "New Message Received", emailBody).ConfigureAwait(false);
                }
            }
            catch
            {
                // Notification failures should not prevent message storage
            }
        }
    }
}