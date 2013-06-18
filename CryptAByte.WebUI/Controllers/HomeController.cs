using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;
using CryptAByte.CryptoLibrary.CryptoProviders;
using CryptAByte.Domain;
using CryptAByte.Domain.KeyManager;
using CryptAByte.WebUI.Models;

namespace CryptAByte.WebUI.Controllers
{
    public class HomeController : Controller
    {

        private readonly IRequestRepository requestRepository;

        public HomeController(IRequestRepository requestRepository)
        {
            this.requestRepository = requestRepository;

            if (FilePasswords != null)
            {
                FilePasswords.Where(p => p.Value.Expires < DateTime.Now).ToList().ForEach(
                    f => FilePasswords.Remove(f.Key));
            }
        }

        public ActionResult Index()
        {
            //if (!Request.IsLocal && !Request.IsSecureConnection)
            //{
            //    return RedirectPermanent("https://cryptabyte.com/");
            //}

            return View();
        }

        public ActionResult RequestKeys()
        {
            return View();
        }


        public ActionResult About()
        {
            return View();
        }

        #region Contact Form

        public ActionResult Contact()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Subscribe(ContactFormModel model)
        //{
        //    if (string.IsNullOrWhiteSpace(model.Email))
        //    {
        //        return Content("Email required.");
        //    }

        //    System.IO.File.AppendAllText(@"H:\web\SecureKey\CryptAByte.WebUI\subs.txt",model.Email + Environment.NewLine);

        //    //this.requestRepository.SubscribeEmail/(model.Email);

        //    //MailMessage message = new MailMessage { From = new MailAddress("mail@mises.org") };
        //    //message.To.Add(new MailAddress("david@mises.org"));

        //    //message.Subject = "Subscription from CryptAByte: " + model.Email;
        //    //message.Body = model.Email;

        //    //SmtpClient client = new SmtpClient();
        //    //client.Send(message);

        //    return Content("Subscription success.");
        //}

        [HttpPost]
        public ActionResult SubmitContact(ContactFormModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return Content("Email required");
            }

            try
            {
                MailMessage message = new MailMessage { From = new MailAddress("mail@mises.org") };
                message.To.Add(new MailAddress("david@mises.org"));

                message.Subject = "Feedback from CryptAByte: " + model.Email;
                message.Body = string.Format(@"
From: {0}
{1}
", model.Name, model.Message);

                SmtpClient client = new SmtpClient();
                client.Send(message);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"H:\web\SecureKey\CryptAByte.WebUI\messages.txt", model.Email + Environment.NewLine);
                System.IO.File.AppendAllText(@"H:\web\SecureKey\CryptAByte.WebUI\messages.txt", model.Message + Environment.NewLine);

                System.IO.File.AppendAllText(@"H:\web\SecureKey\CryptAByte.WebUI\messages.txt", ex + Environment.NewLine);

                return Content("Message sent");
            }


            return Content("Message sent");
        }

        #endregion Contact Form

        #region Crypto Actions

        public ActionResult CreateKey(NewKeyModel newKeyModel)
        {
            if (string.IsNullOrWhiteSpace(newKeyModel.Passphrase))
            {
                ViewBag.Passphrase =
                    newKeyModel.Passphrase = PronounceablePasswordGenerator.Generate(14);
            }
            else
            {
                ViewBag.Passphrase = "*******";
            }

            var request = CryptoKey.CreateRequestWithPassPhrase(newKeyModel.Passphrase);

            request.LockDate = newKeyModel.LockDate;
            if (newKeyModel.ReleaseDate != null) request.ReleaseDate = (DateTime)newKeyModel.ReleaseDate;
            request.DeleteKeyAfterReading = newKeyModel.DeleteKeyAfterReading;
            request.DeleteMessagesAfterReading = newKeyModel.DeleteMessagesAfterReading;

            if (!string.IsNullOrWhiteSpace(newKeyModel.NotifyEmail))
            {
                request.Notifications.Add(new Notification() { Email = newKeyModel.NotifyEmail });
            }

            if (newKeyModel.LockDate == null)
            {
                newKeyModel.LockDate = DateTime.Now.AddDays(30);
            }
            requestRepository.AddRequest(request);
            return View("NewKeyDetails", request);
        }

        //public ActionResult Details(string key)
        //{
        //    var request = requestRepository.GetRequest(key);

        //    return View("KeyDetails", request);
        //}

        public ActionResult SendMessage(SendMessageModel message)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(message.MessageText) && Request.Files.Count == 0)
                {
                    Response.StatusCode = 400;
                    return Content("Either a message or a file is required.");
                }

                if (Request.Files.Count > 0)
                {
                    var uploadFile = Request.Files[0];

                    byte[] fileData = RequestRepository.ReadFully(uploadFile.InputStream);
                    requestRepository.AttachFileToRequest(message.KeyToken, fileData, uploadFile.FileName);

                    return Content("<h1>File uploaded and sent to Key Id</h1>");
                }
                else
                {
                    requestRepository.AttachMessageToRequest(message.KeyToken, message.MessageText);
                    return Content("<h1>Message encrypted and sent to Key Id</h1>");
                }

            }
            catch (Exception ex)
            {
                //Response.StatusCode = 401;
                return Content("Unable to send message.  Please check the Key Id field.");
            }
        }

        public ActionResult GetMessages(GetMessagesModel credentials)
        {
            try
            {
                var messages = requestRepository.GetDecryptedMessagesWithPassphrase(credentials.KeyTokenIdentifier,
                                                                                credentials.Passphrase);

                StoreEncryptionKeysInApplicationMemory(messages);

                // show messages
                return View("MessagesDetail", messages.ToList());
            }
            catch (Exception ex)
            {
                //Response.StatusCode = 401;
                return Content("Unable to retrieve message.  Please check the Key Id and passphrase");
            }

        }

        #endregion Crypto Actions



        #region File Download Helper Methods

        public const int PasswordExpiresInSeconds = 120;
        private const string keyName = "FilePasswords";

        internal static void StoreEncryptionKeysInApplicationMemory(List<Message> messages)
        {
            messages.Where(m => m.IsFile).ToList().ForEach(StoreEncryptedFileInTemporaryMemory);
        }

        private static void StoreEncryptedFileInTemporaryMemory(Message message)
        {
            message.TemporaryDownloadId =
                SymmetricCryptoProvider.GenerateKeyPhrase(64);

            FilePasswords.Add(message.TemporaryDownloadId,
                              new TemporaryDownloadKey
                                  {
                                      Expires = DateTime.Now.AddSeconds(PasswordExpiresInSeconds),
                                      MessageId =message.MessageId,
                                      Passphrase =message.EncryptionKey
                                  });

            // get filename
            byte[] decryptedArray = Convert.FromBase64String(message.MessageData);

            var zipStream = new MemoryStream(decryptedArray);

            using (ZipFile zip = ZipFile.Read(zipStream))
            {
                message.MessageData = zip.First().FileName;
            }
        }

        internal static Dictionary<string, TemporaryDownloadKey> FilePasswords
        {
            get
            {
                var cache = HttpRuntime.Cache;

                if (cache.Get(keyName) == null)
                {
                    cache[keyName] = new Dictionary<string, TemporaryDownloadKey>();
                }

                return cache[keyName] as Dictionary<string, TemporaryDownloadKey>;
            }
            set
            {
                var cache = HttpRuntime.Cache;
                cache[keyName] = value;
            }
        }


        public ActionResult GetFile(string fileId)
        {
            var key = FilePasswords.FirstOrDefault(p => p.Key == fileId && p.Value.Expires > DateTime.Now).Value;

            if (key == null)
            {
                Response.StatusCode = 500;
                return Content("Temporary password has expired.  Close this popup and re-enter your password.");
            }

            Message message = requestRepository.GetMessageByMessageId(key.MessageId);
            string decryptedString = new SymmetricCryptoProvider().DecryptWithKey(message.MessageData, key.Passphrase);
            byte[] decryptedArray = Convert.FromBase64String(decryptedString);

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

        #endregion File Downloads

        public ActionResult Api()
        {
            return View("ServiceInfo");
        }

        public ActionResult Apps()
        {
            return View("About-Apps");
        }
    }
}