using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace CryptAByte.Domain.DataContext
{
    public class SelfDestructingMessage
    {
        [Key]
        public int MessageId { get; set; }

        public string Message { get; set; }

        public DateTime? SentDate { get; set; }

        [NotMapped]
        public virtual SelfDestructingMessageAttachment SelfDestructingMessageAttachment { get; set; }

        [NotMapped]
        public bool HasAttachment { get; internal set; }

        //[NotMapped]
        //public string AttachmentName { get; internal set; }
    }
}
