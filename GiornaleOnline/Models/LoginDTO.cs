using System.ComponentModel.DataAnnotations;

namespace GiornaleOnline.Models
{
    public class LoginDTO
    {

        [Required]
        public string? Password { get; set; }

        [Required]
        public string? Username { get; set; }

    }
}
