using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CryptAByte.WebUI.Models
{
    public class SendMessageModel : IValidatableObject
    {
        [Display(Name = "Key Id")]
        [Required]
        [StringLength(16)]
        public string KeyToken { get; set; }

        //[Required]
        [Display(Name = "Message")]
        public string MessageText { get; set; }

        [Display(Name = "Select file (100MB max)")]
        public HttpPostedFileBase UploadFile { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((!string.IsNullOrEmpty(MessageText) && UploadFile == null) || (string.IsNullOrEmpty(MessageText) && UploadFile != null))
                yield return new ValidationResult("Either a message or a file is required.");
        }
    }
}