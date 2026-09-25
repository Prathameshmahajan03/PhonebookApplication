using System.ComponentModel.DataAnnotations;

namespace PhonebookApplication.Models
{
    public class LoginRequest
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string Password { get; set; } = string.Empty;
    }
}
