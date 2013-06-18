using System.ComponentModel.DataAnnotations;

namespace CryptAByte.WebUI.Models
{
    public class ContactFormModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        [RegularExpression(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        

        [Required]
        [DataType(DataType.Text)]
        public string Message { get; set; }



    }
}