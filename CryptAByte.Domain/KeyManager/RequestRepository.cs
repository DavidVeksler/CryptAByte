using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using CryptAByte.CryptoLibrary.CryptoProviders;
using Ionic.Zip;
using CryptAByte.CryptoLibrary;
using CryptAByte.Domain.DataContext;

namespace CryptAByte.Domain.KeyManager
{
    public class RequestRepository : IRequestRepository
    {
        public void AddRequest(CryptoKey request)
        {
            //  TODO: Validate key

            var db = new CryptAByteContext();
            db.Keys.Add(request);
            db.SaveChanges();
        }

        public CryptoKey GetRequest(string token)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(token), "Token/Identifier is required to retrieve the key!");

            var db = new CryptAByteContext();
            var request = db.Keys.Include("Messages").Include("Notifications").SingleOrDefault(key => key.KeyToken.Equals(token));

            if (request == null)
                return null;

            if (!request.IsReleased)
            {
                request.PrivateKey = null;
            }

            if (request == null)
            {
                throw new KeyNotFoundException("Key not found for this token!");
            }
            return request;
        }

        public void AttachMessageToRequest(string token, string plainTextMessage)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new Exception("Token/Identifier is required to attach message!");

            // compress message:
            string compressedMessage = GzipCompression.Compress(plainTextMessage);

            AttachDataToKey(token, compressedMessage, false);

            NotifyOnMessageReceived(token);
        }

        public void AttachEncryptedMessageToRequest(string token, string encryptedmessage, string encryptionkey)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new Exception("Token/Identifier is required to attach message!");

            AttachDataToKey(token, encryptedmessage, false, encryptionkey);

            NotifyOnMessageReceived(token);
        }

        public void AttachFileToRequest(string token, byte[] fileData, string fileName)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new Exception("Token/Identifier is required to attach message!");

            if (fileData.Length == 0) throw new Exception("File must not be empty.");

            MemoryStream streamOfOriginalFile = new MemoryStream(1024);

            using (ZipFile zip = new ZipFile())
            {
                zip.AddEntry(fileName, fileData);
                zip.Save(streamOfOriginalFile);
            }

            byte[] zippedFile = ReadFully(streamOfOriginalFile);
            string fileAsString = Convert.ToBase64String(zippedFile);

            AttachDataToKey(token, fileAsString, true);

            NotifyOnMessageReceived(token);
        }

        private static void AttachDataToKey(string token, string compressedMessage, bool isFile, string encryptionKey = null)
        {
            string hash = null;
            string encryptedPassword;

            var db = new CryptAByteContext();
            var request = db.Keys.SingleOrDefault(key => key.KeyToken == token);

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

            db.SaveChanges();
        }



        public List<Message> GetDecryptedMessagesWithPassphrase(string keyToken, string passphrase)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(keyToken), "Token/Identifier is required to retrieve the messages!");

            var db = new CryptAByteContext();
            var request = db.Keys.Include("Messages").SingleOrDefault(key => key.KeyToken == keyToken);

            if (request == null) throw new ArgumentOutOfRangeException("keyToken", "Request not found for this token.");

            try
            {
                string privateKey = new SymmetricCryptoProvider().DecryptWithKey(request.PrivateKey, passphrase);

                return GetDecryptedMessagesWithPrivateKey(keyToken, privateKey);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentOutOfRangeException("passphrase", "error decrypting private key");
            }
            catch (CryptographicException)
            {
                throw new ArgumentOutOfRangeException("passphrase", "error decrypting private key");
            }

        }

        public Message GetMessageByMessageId(int messageId)
        {
            var db = new CryptAByteContext();
            var message = db.Messages.SingleOrDefault(m => m.MessageId == messageId);

            return message;
        }

        public void DeleteKeyWithPassphrase(string token, string passphrase)
        {
            var db = new CryptAByteContext();
            var key = db.Keys.Include("Messages").SingleOrDefault(k => k.KeyToken == token);

            if (key == null)
            {
                throw new ArgumentOutOfRangeException("Key for this token not found.  Was it already deleted?");
            }

            var crypto = new SymmetricCryptoProvider();

            try
            {
                var plaintext = crypto.DecryptWithKey(key.PrivateKey, passphrase);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Failed to verify passphrase.  A correct passphrase is required to verify the delete request.");
            }

            db.Keys.Remove(key);
            db.SaveChanges();
        }

        public List<Message> GetEncryptedMessages(string token, string privateKeyHash)
        {
            var db = new CryptAByteContext();
            var request = db.Keys.Include("Messages").SingleOrDefault(key => key.KeyToken == token);

            if (request == null) throw new ArgumentOutOfRangeException("keyToken", "Request not found for this token.");

            if (!request.IsReleased)
            {
                throw new ArgumentOutOfRangeException("Request is not released");
            }

            if (request.PrivateKeyHash != privateKeyHash)
            {
                throw new ArgumentOutOfRangeException("privatekeyhash does not match stored PrivateKeyHash field (or no hash stored)");
            }

            return request.Messages.ToList();
            
        }

        public List<Message> GetDecryptedMessagesWithPrivateKey(string token, string privateKey)
        {
            var db = new CryptAByteContext();
            var request = db.Keys.Include("Messages").SingleOrDefault(key => key.KeyToken == token);
            var crypto = new AsymmetricCryptoProvider();

            if (!request.IsReleased)
            {
                throw new ArgumentOutOfRangeException("Request is not released");
            }

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
                                                          else
                                                          {
                                                              // this is a zip file

                                                          }

                                                          plaintextMessages.Add(retrievedMessage);


                                                      });

                if (request.DeleteMessagesAfterReading || request.DeleteKeyAfterReading)
                {
                    if (request.DeleteMessagesAfterReading || request.DeleteKeyAfterReading)
                    {
                        request.Messages.ToList().ForEach(message => db.Messages.Remove(message));
                    }

                    if (request.DeleteKeyAfterReading)
                    {
                        db.Keys.Remove(request);
                    }

                    db.SaveChanges();
                }
            }



            return plaintextMessages;
        }

        public void NotifyOnMessageReceived(string token)
        {
            try
            {
                var db = new CryptAByteContext();
                var request = db.Keys.Include("Notifications").SingleOrDefault(key => key.KeyToken == token);

                if (request.Notifications.Any())
                {
                    request.Notifications.ToList().ForEach(n =>
                    {
                        // Send email 

                        const string notification = "You have received a message at {0}.  You can check it at https://cryptabyte.com/#{1}.";

                        MailMessage message = new MailMessage { From = new MailAddress("webmaster@cryptabyte.com") };
                        message.To.Add(new MailAddress(n.Email));

                        message.Subject = "New Message received";


                        message.Body = string.Format(notification, DateTime.Now.ToString(), request.KeyToken); ;

                        SmtpClient client = new SmtpClient();
                        client.Send(message);


                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                input.Position = 0;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {

                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}