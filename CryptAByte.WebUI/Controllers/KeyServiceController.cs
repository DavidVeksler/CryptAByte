using System;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Web.Mvc;
using CryptAByte.Domain.KeyManager;
using CryptAByte.Domain.SelfDestructingMessaging;
using CryptAByte.WebUI.Models;

namespace CryptAByte.WebUI.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IRequestRepository _requestRepository;
        private readonly ISelfDestructingMessageRepository _selfDestructingMessageRepository;

        public ServiceController(IRequestRepository requestRepository, ISelfDestructingMessageRepository selfDestructingMessageRepository)
        {
            _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
            _selfDestructingMessageRepository = selfDestructingMessageRepository ?? throw new ArgumentNullException(nameof(selfDestructingMessageRepository));
        }

        [ValidateInput(false)]
        public ActionResult Index(string passphrase = null, string publicKey = null, string privateKey = null, string privateKeyHash = null, string token = null, string message = null, string encryptedMessage = null, string encryptionKey = null)
        {
            if (Request.Headers["publickey"] != null)
            {
                token = Request.Headers["publickey"];
            }

            if (Request.Headers["privatekey"] != null)
            {
                token = Request.Headers["privatekey"];
            }

            if (Request.Headers["token"] != null)
            {
                token = Request.Headers["token"];
            }

            if (Request.Headers["passphrase"] != null)
            {
                passphrase = Request.Headers["passphrase"];
            }

            if (Request.HttpMethod == "GET" && token != null)
            {
                return GetMessages(token, passphrase, privateKeyHash);
            }
            if (Request.HttpMethod == "POST")
            {
                return Create(passphrase, publicKey, privateKey);
            }
            if (Request.HttpMethod == "PUT")
            {
                return SendMessage(token, message, encryptedMessage, encryptionKey);
            }
            if (Request.HttpMethod == "DELETE")
            {
                return DeleteKey(token, passphrase);
            }
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(string passphrase = null, string publicKey = null, string privateKey = null, string privateKeyHash = null, DateTime? releaseDate = null)
        {
            if (string.IsNullOrWhiteSpace(passphrase) && string.IsNullOrWhiteSpace(publicKey))
            {
                Response.StatusCode = 400;
                return SerializeResult("Either a passphrase or a public key parameter is required");
            }

            if (!string.IsNullOrWhiteSpace(publicKey) && string.IsNullOrWhiteSpace(privateKey))
            {
                Response.StatusCode = 400;
                return SerializeResult("When sending the public key, you must also send the private key hash");
            }

            CryptoKey key = !string.IsNullOrWhiteSpace(passphrase)
                                ? CryptoKey.CreateWithPassphraseProtectedKeys(passphrase)
                                : CryptoKey.CreateWithProvidedKeys(publicKey, privateKey, true, privateKeyHash);

            if (releaseDate != null)
            {
                key.ReleaseDate = (DateTime)releaseDate;
            }

            _requestRepository.AddRequest(key);

            return SerializeResult(key);
        }

        public ActionResult GetToken(string token)
        {
            var request = _requestRepository.GetRequest(token);

            return SerializeResult(request);
        }

        public ActionResult SendMessage(string token, string message, string encryptedMessage, string encryptionKey)
        {
            if (Request.Files.Count == 0 && string.IsNullOrWhiteSpace(message) && string.IsNullOrWhiteSpace(encryptedMessage))
            {
                Response.StatusCode = 400;
                return SerializeResult("Message, encrypted message, or file required");
            }

            if (Request.Files.Count > 0)
            {
                var uploadFile = Request.Files[0];
                byte[] fileData = RequestRepository.ReadFully(uploadFile.InputStream);
                _requestRepository.AttachFileToRequestAsync(token, fileData, uploadFile.FileName).Wait();
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                _requestRepository.AttachMessageToRequestAsync(token, message).Wait();
            }

            if (!string.IsNullOrWhiteSpace(encryptedMessage))
            {
                _requestRepository.AttachEncryptedMessageToRequestAsync(token, encryptedMessage, encryptionKey).Wait();
            }

            return SerializeResult(true);
        }

        public ActionResult GetMessages(string token, string passphrase, string privateKeyHash)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Response.StatusCode = 400;
                return SerializeResult("Token is required");
            }

            if (string.IsNullOrWhiteSpace(passphrase) && string.IsNullOrWhiteSpace(privateKeyHash))
            {
                Response.StatusCode = 400;
                return SerializeResult("Passphrase or private key hash is required");
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(passphrase))
                {
                    var messages = _requestRepository.GetDecryptedMessagesWithPassphrase(token, passphrase);

                    HomeController.StoreEncryptionKeysInApplicationMemory(messages);

                    if (messages == null || messages.Count == 0)
                    {
                        Response.StatusCode = 304;
                        return SerializeResult(new List<Message>());
                    }
                    return SerializeResult(messages);
                }
                else
                {
                    var messages = _requestRepository.GetEncryptedMessages(token, privateKeyHash);
                    return SerializeResult(messages);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Response.StatusCode = 404;
                return SerializeResult(new { ex.Message });
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return SerializeResult("Server error decrypting messages.");
            }
        }


        #region Self-Destructing Messages

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult SelfDestruct()
        {
            Response.StatusCode = 400;
            return SerializeResult("POST to this URL with message and email");
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SelfDestruct(string message, string email, string attachment = null, string attachmentName = null)
        {

            if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(email))
            {
                Response.StatusCode = 400;
                return SerializeResult("message and email required");
            }

            try
            {
                byte[] attachmentData = new byte[] {};
                if (attachment != null)
                {
                    attachmentData = Convert.FromBase64String(attachment);
                }
                ContentResult result = new SelfDestructController(selfDestructingMessageRepository).Send(new SelfDestructingMessageModel() { Email = email, MessageText = message, Attachment = attachmentData, AttachmentName = attachmentName }) as ContentResult;

                return SerializeResult(result.Content == "Message sent");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return SerializeResult(ex.Message);
            }

        }

        #endregion Self-Destructing Messages


        #region Private Helpers

        private ActionResult DeleteKey(string token = null, string passphrase = null)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Response.StatusCode = 400;
                return SerializeResult("token is required");

            }

            if (string.IsNullOrWhiteSpace(passphrase))
            {
                Response.StatusCode = 400;
                return SerializeResult("passphrase is required");
            }

            try
            {
                _requestRepository.DeleteKeyWithPassphrase(token, passphrase);

                return SerializeResult("Deleted.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Response.StatusCode = 400;
                return SerializeResult(ex.Message);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return SerializeResult("Unable to delete key.");
            }
        }

        private ActionResult SerializeResult(object result)
        {


            if ((Request.Headers["Accept"] != null && Request.Headers["Accept"].ToLower() == "application/xml") ||
                (Request.Unvalidated().QueryString["format"] != null && Request.Unvalidated().QueryString["format"] == "xml"))
            {
                return new XmlResult(result);

            }
            else
            {
                return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        #endregion Private Helpers
    }
}