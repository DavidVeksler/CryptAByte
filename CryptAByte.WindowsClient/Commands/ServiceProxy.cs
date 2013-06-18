using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CryptAByte.CryptoLibrary.CryptoProviders;
using CryptAByte.Domain.DataContext;
using CryptAByte.Domain.KeyManager;
using Message = CryptAByte.Domain.KeyManager.Message;

namespace CryptAByte.WindowsClient.Commands
{
    public class ServiceProxy
    {
        public string ServiceUrl { get; set; }

        public ServiceProxy()
        {
            ServiceUrl = "http://localhost:60888/Service/";
        }

        #region RESTfull methods

        public CryptoKey CreateKey(string passphrase)
        {
            CryptoKey request = CryptoKey.CreateRequestWithPassPhrase(passphrase);

            WebClient client = new WebClient();

            NameValueCollection data = new NameValueCollection()
                {
                    {"PublicKey", request.PublicKey},
                    {"PrivateKey", HttpUtility.HtmlEncode(request.PrivateKey)},
                    {"PrivateKeyHash", HttpUtility.HtmlEncode(request.PrivateKeyHash)},
                };

            if (request.Notifications.Count > 0)
            {
                data.Add("Notifcations", request.Notifications.First().Email);
            }

            try
            {
                byte[] response = client.UploadValues(ServiceUrl, "POST", data);

                var responseText = Encoding.Default.GetString(response);

                var jss = new JavaScriptSerializer();
                CryptoKey resource = jss.Deserialize<CryptoKey>(responseText);
                return resource;
            }
            catch (WebException e)
            {
                HandleWebException(e);
                return null;
                // throw;
            }
        }


        public bool SendMessage(string token, string message)
        {
            WebClient client = new WebClient();

            NameValueCollection data = new NameValueCollection()
                {
                    {"token", HttpUtility.HtmlEncode(token)},
                    {"message", HttpUtility.HtmlEncode(message)},
                };

            try
            {
                byte[] response = client.UploadValues(ServiceUrl, "PUT", data);

                var responseText = Encoding.Default.GetString(response);

                var jss = new JavaScriptSerializer();
                var resource = jss.Deserialize<bool>(responseText);
                return resource;
            }
            catch (WebException e)
            {
                HandleWebException(e);
                //throw;
                return false;
            }

        }

        public List<Message> GetMessages(string token, string passphrase)
        {
            WebClient client = new WebClient();

            client.Headers.Add("token", token);
            client.Headers.Add("passphrase", passphrase);

            try
            {
                byte[] response = client.DownloadData(ServiceUrl);

                var responseText = Encoding.Default.GetString(response);

                var jss = new JavaScriptSerializer();
                var resource = jss.Deserialize<List<Message>>(responseText);
                return resource;
            }
            catch (WebException e)
            {
                HandleWebException(e);
                //throw;
                return null;
            }
        }


        public List<Message> GetEncryptedMessages(string token, string privatekey)
        {
            string privatekeyhash = SymmetricCryptoProvider.GetSecureHashForString(privatekey);

            WebClient client = new WebClient();

            client.Headers.Add("token", token);
            client.Headers.Add("privatekeyhash", privatekeyhash);

            try
            {
                byte[] response = client.DownloadData(ServiceUrl);

                var responseText = Encoding.Default.GetString(response);

                var jss = new JavaScriptSerializer();
                var resource = jss.Deserialize<List<Message>>(responseText);
                return resource;
            }
            catch (WebException e)
            {
                HandleWebException(e);
                //throw;
                return null;
            }
        }


        public bool SendSelfDestructingMessage(string email, string message, string selfDestructingFilePath)
        {
            WebClient client = new WebClient();
            

            NameValueCollection data = new NameValueCollection()
                {
                    {"email", HttpUtility.HtmlEncode(email)},
                    {"message", HttpUtility.HtmlEncode(message)},
                };

            if (!string.IsNullOrWhiteSpace(selfDestructingFilePath))
            {
                 string filename = HttpUtility.HtmlEncode(new FileInfo(selfDestructingFilePath).Name);

                data.Add("attachmentName",filename);
                data.Add("attachment", HttpUtility.HtmlEncode(Convert.ToBase64String(File.ReadAllBytes(selfDestructingFilePath))));
            }

            try
            {
                byte[] response = client.UploadValues(ServiceUrl + "SelfDestruct/", "POST", data);

                var responseText = Encoding.Default.GetString(response);

                var jss = new JavaScriptSerializer();
                var resource = jss.Deserialize<bool>(responseText);
                return resource;
            }
            catch (WebException e)
            {
                HandleWebException(e);
                //throw;
                return false;
            }
        }


        #endregion

        #region Request Helpers


        private void HandleWebException(WebException webException)
        {
            WebResponse errResp = ((WebException)webException).Response;

            if (errResp == null)
            {
                MessageBox.Show("Error establishing secure (SSL/TLS) connection to server:" + Environment.NewLine + webException.ToString(),
                    "Service Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (Stream respStream = errResp.GetResponseStream())
            {
                // read the error response

                byte[] errMessage = new byte[respStream.Length];

                respStream.Read(errMessage, 0, (int)respStream.Length);

                var error = Encoding.Default.GetString(errMessage);

                MessageBox.Show(error, "Service Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine(error);

            }
        }

        #endregion Request Helpers

    }
}
