using System.ComponentModel.DataAnnotations;

namespace SoundMatchAPI.Data.DTOs
{
    public class UserCreateRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{12,}$", ErrorMessage = "Password must be at least 12 characters long, contain at least one uppercase letter and one number.")]
        public string Password { get; set; } = string.Empty;
    }
}
