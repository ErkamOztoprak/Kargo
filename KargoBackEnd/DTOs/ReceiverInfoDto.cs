using System.ComponentModel.DataAnnotations;
namespace KargoUygulamasiBackEnd.DTOs
{
    public class ReceiverInfoDto
    {
        [Required(ErrorMessage = "Alıcı adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Alıcı adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Alıcı iletişim bilgisi gereklidir.")]
        [StringLength(100, ErrorMessage = "Alıcı iletişim bilgisi en fazla 100 karakter olabilir.")]
        public string ContactInfo { get; set; } 
    }
}
