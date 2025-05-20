using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace KargoUygulamasiBackEnd.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
