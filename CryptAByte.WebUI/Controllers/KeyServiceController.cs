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
        private readonly IRequestRepository requestRepository;
        private readonly ISelfDestructingMessageRepository selfDestructingMessageRepository;

        public ServiceController(IRequestRepository requestRepository, ISelfDestructingMessageRepository selfDestructingMessageRepository)
        {
            this.requestRepository = requestRepository;
            this.selfDestructingMessageRepository = selfDestructingMessageRepository;
        }

        [ValidateInput(false)]
        public ActionResult Index(string passphrase = null, string publickey = null, string privatekey = null, string privatekeyhash = null, string token = null, string message = null, string encryptedmessage = null, string encryptionkey = null)
        {
            #region Optionally - send sensitive parameters in request header

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

            #endregion Optionally - send parameters in request header

            if (Request.HttpMethod == "GET" && token != null)
            {
                return GetMessages(token, passphrase, privatekeyhash);
            }
            if (Request.HttpMethod == "POST") // Create Key
            {
                return Create(passphrase, publickey, privatekey);
            }
            if (Request.HttpMethod == "PUT") // Send Message
            {
                return SendMessage(token, message, encryptedmessage, encryptionkey);
            }
            if (Request.HttpMethod == "DELETE") // Delete Token
            {
                return DeleteKey(token, passphrase);
            }
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(string passphrase = null, string publickey = null, string privatekey = null, string privatekeyhash = null, DateTime? releasedate = null)
        {
            if (string.IsNullOrWhiteSpace(passphrase) && string.IsNullOrWhiteSpace(publickey))
            {
                Response.StatusCode = 400;
                // Response.Status = "MissingPassphraseOrPublicKey";
                return SerializeResult("Either a passphrase or a publickey parameter is required");
            }

            if (!string.IsNullOrWhiteSpace(publickey) && string.IsNullOrWhiteSpace(privatekey))
            {
                Response.StatusCode = 400;
                return SerializeResult("When sneding the public key, you must also send the privatekeyhash");
            }

            CryptoKey key = !string.IsNullOrWhiteSpace(passphrase)
                                ? CryptoKey.CreateRequestWithPassPhrase(passphrase)
                                : CryptoKey.CreateRequestWithPublicKey(publickey, privatekey, true, privatekeyhash);

            if (releasedate != null)
            {
                key.ReleaseDate = (DateTime)releasedate;
            }

            requestRepository.AddRequest(key);

            return SerializeResult(key);
        }



        //http://localhost:62633/Service/Get?token=a073f62e-e2de-48b9-8002-ac5994528038
        public ActionResult GetToken(string token)
        {
            var request = requestRepository.GetRequest(token);

            return SerializeResult(request);
        }

        // http://localhost:62633/Service/AddMessage?token=a073f62e-e2de-48b9-8002-ac5994528038&message=hello world
        //[HttpPost]
        public ActionResult SendMessage(string token, string message, string encryptedmessage, string encryptionkey)
        {
            if (Request.Files.Count == 0 && string.IsNullOrWhiteSpace(message) && string.IsNullOrWhiteSpace(encryptedmessage))
            {
                Response.StatusCode = 400;
                return SerializeResult("Message or encryptedmessage or file required");
            }

            if (Request.Files.Count > 0)
            {
                var uploadFile = Request.Files[0];

                byte[] fileData = RequestRepository.ReadFully(uploadFile.InputStream);
                requestRepository.AttachFileToRequest(token, fileData, uploadFile.FileName);
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                requestRepository.AttachMessageToRequest(token, message);
            }

            if (!string.IsNullOrWhiteSpace(encryptedmessage))
            {
                requestRepository.AttachEncryptedMessageToRequest(token, encryptedmessage, encryptionkey);
            }

            return SerializeResult(true);
        }

        // http://localhost:62633/Service/AddMessage?token=a073f62e-e2de-48b9-8002-ac5994528038&message=hello world
        //[HttpPost]
        public ActionResult GetMessages(string token, string passphrase, string privatekeyhash)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Response.StatusCode = 400;
                //  Response.Status = "MissingToken";
                return SerializeResult("token is required");

            }

            if (string.IsNullOrWhiteSpace(passphrase) && string.IsNullOrWhiteSpace(privatekeyhash))
            {
                Response.StatusCode = 400;
                //  Response.Status = "MissingPassphrase";
                return SerializeResult("passphrase is required");
            }

            try
            {
                if (string.IsNullOrWhiteSpace(passphrase))
                {
                    var messages = requestRepository.GetDecryptedMessagesWithPassphrase(token, passphrase);

                    HomeController.StoreEncryptionKeysInApplicationMemory(messages);

                    if (messages == null || messages.Count == 0)
                    {
                        Response.StatusCode = 304; // not modified
                        return SerializeResult(new List<Message>());

                    }
                    return SerializeResult(messages);
                }
                else
                {
                    // return messages
                    var messages = requestRepository.GetEncryptedMessages(token, privatekeyhash);
                    return SerializeResult(messages);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Response.StatusCode = 404;
                return SerializeResult(new { ex.Message });
            }
            catch (Exception ex)
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
                requestRepository.DeleteKeyWithPassphrase(token, passphrase);

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