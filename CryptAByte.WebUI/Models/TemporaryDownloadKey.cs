using System;

namespace CryptAByte.WebUI.Models
{
    public class TemporaryDownloadKey
    {
        public int MessageId { get; set; }
        public DateTime Expires { get; set; }
        public string Passphrase { get; set; }
        
    }
}