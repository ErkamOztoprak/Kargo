using System.ComponentModel.DataAnnotations;

namespace KargoUygulamasiBackEnd.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
