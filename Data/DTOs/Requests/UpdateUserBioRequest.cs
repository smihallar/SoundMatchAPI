using System.ComponentModel.DataAnnotations;

namespace SoundMatchAPI.Data.DTOs.Requests
{
    public class UpdateUserBioRequest
    {
        [Required]
        [StringLength(500, ErrorMessage = "Bio cannot be longer than 500 characters.", MinimumLength = 0)]
        public string Bio { get; set; } = string.Empty;
    }
}
