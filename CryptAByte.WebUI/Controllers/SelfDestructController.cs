using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using CryptAByte.CryptoLibrary.CryptoProviders;
using CryptAByte.Domain.DataContext;
using CryptAByte.Domain.KeyManager;
using CryptAByte.Domain.SelfDestructingMessaging;
using CryptAByte.WebUI.Models;
using Ionic.Zip;

namespace CryptAByte.WebUI.Controllers
{
    public class SelfDestructController : Controller
    {
        private readonly ISelfDestructingMessageRepository selfDestructingMessageRepository;

        public SelfDestructController(ISelfDestructingMessageRepository selfDestructingMessageRepository)
        {
            this.selfDestructingMessageRepository = selfDestructingMessageRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Send(SelfDestructingMessageModel message)
        {
            // verify email

            if (!Domain.Validation.IsValidEmail(message.Email))
            {
                throw new ArgumentException("email", "invalid email format");
            }


            string passphrase = PronounceablePasswordGenerator.Generate(32);

            passphrase = HttpUtility.UrlEncode(passphrase);

            int messageId = selfDestructingMessageRepository.StoreMessage(new SelfDestructingMessage()
            {
                Message = message.MessageText
            },
            passphrase,
            message.AttachmentName,
            message.Attachment);

            string hash = SymmetricCryptoProvider.GetSecureHashForString(message.MessageText);

            const string notification = @"
Hello,

You have received a self-destructing message.  This message will be decrypted and erased when you open the link below.

You can read it at 

https://{0}:{1}/selfdestruct/read/?messageId={2}&passphrase={3}&hash={4}


CryptAByte.com is not responsible for the contents of messages.  For more information, please visit https://CryptAByte.com/SelfDestruct
";

            MailMessage mailMessage = new MailMessage { From = new MailAddress("webmaster@cryptabyte.com") };
            mailMessage.To.Add(new MailAddress(message.Email));

            mailMessage.Subject = "New self-destructing message @ CryptAByte";

            if (Request == null)
            {
                string messageText = string.Format(notification, "cryptabyte.com", 443, messageId, passphrase, hash);

                Debug.WriteLine(messageText);

                mailMessage.Body = messageText;
            }
            else
            {
                mailMessage.Body = string.Format(notification, Request.Url.Host, Request.Url.Port, messageId, passphrase, hash); ;
            }

            SmtpClient client = new SmtpClient();
            client.Send(mailMessage);

            return Content("Message sent");
        }

        public ActionResult Read(int messageId, string passphrase, string hash)
        {
            var model = new SelfDestructingMessageModel();

            try
            {
                var message = selfDestructingMessageRepository.GetMessage(messageId, passphrase);

                string originalHash = SymmetricCryptoProvider.GetSecureHashForString(message.Message);

                if (hash != originalHash)
                {
                    model.MessageText = "Error: hash of retrieved message does not match the original message hash." + Environment.NewLine +
                        "The message may have been tampered with!";
                    model.InvalidMessageId = true;
                    return View("Read", model);
                }

                // File Attachments
                if (message.HasAttachment)
                {
                    Message attachment = new Message { EncryptionKey = passphrase, MessageId = message.MessageId };
                    StoreEncryptedFileInTemporaryMemory(attachment);

                    model.HasAttachment = true;
                    model.TemporaryDownloadId = attachment.TemporaryDownloadId;
                    //model.AttachmentName = message.AttachmentName;
                }
                else
                {
                    model.HasAttachment = false;
                }

                model.MessageText = message.Message;
            }
            catch (Exception ex)
            {
                model.InvalidMessageId = true;
                model.MessageText = ex.Message;
            }

            return View("Read", model);
        }

        public ActionResult GetAttachment(string fileId)
        {
            var key = HomeController.FilePasswords.FirstOrDefault(p => p.Key == fileId && p.Value.Expires > DateTime.Now).Value;

            if (key == null)
            {
                Response.StatusCode = 500;
                return Content("Temporary password has expired.  Close this popup and re-enter your password.");
            }

            try
            {

                SelfDestructingMessageAttachment attachment = selfDestructingMessageRepository.GetAttachment(key.MessageId, key.Passphrase);

                if (attachment == null)
                {
                    Response.StatusCode = 500;
                    return Content("File not found. It may have been already downloaded");
                }

                //string decryptedString = new SymmetricCryptoProvider().DecryptWithKey(attachment.Attachment, key.Passphrase);
                byte[] decryptedArray = Convert.FromBase64String(attachment.Attachment);

                var zipStream = new MemoryStream(decryptedArray);
                var outputFileStream = new MemoryStream();

                using (ZipFile zip = ZipFile.Read(zipStream))
                {
                    zip.First().Extract(outputFileStream);
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + zip.First().FileName);
                }

                Response.ContentType = "application/octet-stream";
                outputFileStream.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(outputFileStream, "application/zip");

            }
            catch (ArgumentNullException ex)
            {
                Response.StatusCode = 500;
                return Content("File does not exist.  Was it already downloaded?");
            }
        }


        private static void StoreEncryptedFileInTemporaryMemory(Message message)
        {
            message.TemporaryDownloadId =
                SymmetricCryptoProvider.GenerateKeyPhrase(64);

            HomeController.FilePasswords.Add(message.TemporaryDownloadId,
                              new TemporaryDownloadKey
                              {
                                  Expires = DateTime.Now.AddSeconds(HomeController.PasswordExpiresInSeconds),
                                  MessageId = message.MessageId,
                                  Passphrase = message.EncryptionKey
                              });


        }


    }
}
