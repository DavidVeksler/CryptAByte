using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptAByte.Domain.DataContext
{
    public class SelfDestructingMessageAttachment
    {

      //  [Key, ForeignKey("SelfDestructingMessage")]
        [Key]
        public int AttachmentId { get; set; }

        public int MessageId { get; set; }

       // public SelfDestructingMessage SelfDestructingMessage { get; set; }

        public string Attachment { get; set; }

       // public string AttachmentName { get; set; }

        public DateTime? SentDate { get; set; }

        //public string AttachmentName
        //{
        //    get { throw new NotImplementedException(); }
        //}
    }
}