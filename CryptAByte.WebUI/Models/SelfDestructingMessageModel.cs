using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CryptAByte.WebUI.Models
{
    public class SelfDestructingMessageModel
    {
        //[Required]
        //[StringLength(32)]
        //[DataType(DataType.Password)]
        //public string Passphrase { get; set; }

        [Required]
        [StringLength(200)]
        [RegularExpression(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Message")]
        [DataType(DataType.Text)]
        public string MessageText { get; set; }

        public bool InvalidMessageId { get; set; }

        public byte[] Attachment { get; set; }

        public string AttachmentName { get; set; }

        public string TemporaryDownloadId { get; set; }

        public bool HasAttachment { get; set; }
    }
}

