using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace KargoUygulamasiBackEnd.Models
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
