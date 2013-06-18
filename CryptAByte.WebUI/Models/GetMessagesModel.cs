using System.ComponentModel.DataAnnotations;

namespace CryptAByte.WebUI.Models
{
    public class GetMessagesModel
    {
        [Required]
        [StringLength(16)]
        [Display(Name = "Key Id")]
        public string KeyTokenIdentifier { get; set; }

        [Required]
      //  [StringLength(128, MinimumLength = 0)]
        [Display(Name = "Passphrase")]
        public string Passphrase { get; set; }


    }
}