using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CryptAByte.CryptoLibrary.CryptoProviders;
using CryptAByte.Domain.DataContext;
using CryptAByte.Domain.KeyManager;
using CryptAByte.Domain.SelfDestructingMessaging;
using CryptAByte.Domain.Services;
using CryptAByte.Domain.Utilities;
using CryptAByte.WebUI.Models;

namespace CryptAByte.WebUI.Controllers
{
    public class SelfDestructController : Controller
    {
        private readonly ISelfDestructingMessageRepository __selfDestructingMessageRepository;
        private readonly IEmailService _emailService;

        public SelfDestructController(ISelfDestructingMessageRepository _selfDestructingMessageRepository, IEmailService emailService)
        {
            __selfDestructingMessageRepository = _selfDestructingMessageRepository ?? throw new ArgumentNullException(nameof(_selfDestructingMessageRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
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

            int messageId = _selfDestructingMessageRepository.StoreMessage(new SelfDestructingMessage()
            {
                Message = message.MessageText
            },
            passphrase,
            message.AttachmentName,
            message.Attachment);

            string hash = SymmetricCryptoProvider.GetSecureHashForString(message.MessageText);

            const string notificationTemplate = @"
Hello,

You have received a self-destructing message. This message will be decrypted and erased when you open the link below.

You can read it at

https://{0}:{1}/selfdestruct/read/?messageId={2}&passphrase={3}&hash={4}


CryptAByte.com is not responsible for the contents of messages. For more information, please visit https://CryptAByte.com/SelfDestruct
";

            string host = Request?.Url?.Host ?? "cryptabyte.com";
            int port = Request?.Url?.Port ?? 443;
            string messageBody = string.Format(notificationTemplate, host, port, messageId, passphrase, hash);

            if (Request == null)
            {
                Debug.WriteLine(messageBody);
            }

            _emailService.SendEmail(message.Email, "New self-destructing message @ CryptAByte", messageBody);

            return Content("Message sent");
        }

        public ActionResult Read(int messageId, string passphrase, string hash)
        {
            var model = new SelfDestructingMessageModel();

            try
            {
                var message = _selfDestructingMessageRepository.GetMessage(messageId, passphrase);

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
                SelfDestructingMessageAttachment attachment = _selfDestructingMessageRepository.GetAttachment(key.MessageId, key.Passphrase);

                if (attachment == null)
                {
                    Response.StatusCode = 500;
                    return Content("File not found. It may have been already downloaded");
                }

                string fileName;
                byte[] fileData = FileUtilities.DecodeAndDecompressFile(attachment.Attachment, out fileName);

                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.ContentType = "application/octet-stream";

                return new FileStreamResult(new MemoryStream(fileData), "application/zip");
            }
            catch (ArgumentNullException)
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
