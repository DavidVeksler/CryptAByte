using System;
using System.ComponentModel.DataAnnotations;

namespace CryptAByte.Domain.DataContext
{
    public class SelfDestructingMessageAttachment
    {
        [Key]
        public int AttachmentId { get; set; }

        public int MessageId { get; set; }

        public string Attachment { get; set; }

        public DateTime? SentDate { get; set; }
    }
}