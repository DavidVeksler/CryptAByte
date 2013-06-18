using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CryptAByte.WebUI.Models
{
    public class NewKeyModel
    {
        public NewKeyModel()
        {
            ReleaseDate = DateTime.Now.ToUniversalTime();
        }

        [DataType(DataType.Password)]
        [StringLength(128)]
        [Display(Name = "Passphrase (optional)")]
        public string Passphrase { get; set; }

        [Display(Name = "RSA Public Key")]
        public string PublicKey { get; set; }

        [Display(Name = "SHA256 hash of private key (only used to authenticate API message requests)")]
        public string PrivateKeyHash { get; set; }

        [Display(Name = "Burn After Reading: successfuly retrieving messages will delete all messages")]
        public bool DeleteMessagesAfterReading { get; set; }

        [Display(Name = "Burn Notice: retrieving messages will delete all messages and the key so no future messages can be delivered")]
        public bool DeleteKeyAfterReading { get; set; }

        [Display(Name = "Release Date: messages can be delivered, but cannot be retrieved before this (UTC) date")]
        public DateTime? ReleaseDate { get; set; }

        [Display(Name = "Lock Date: no messages can be delivered after this date")]
        public DateTime? LockDate { get; set; }

        [Display(Name = "Notify Me: send an email to this address when messages are received")]
        [RegularExpression(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Please enter a valid email address")]
        public string NotifyEmail { get; set; }

    }
}