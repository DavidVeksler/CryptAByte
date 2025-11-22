using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    }
}
