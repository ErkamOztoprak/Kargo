using System.ComponentModel.DataAnnotations;
namespace KargoUygulamasiBackEnd.DTOs
{

    public class SenderInfoDto
    {
        [Required(ErrorMessage = "Gönderici adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Gönderici adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Gönderici iletişim bilgisi gereklidir.")]
        [StringLength(100, ErrorMessage = "Gönderici iletişim bilgisi en fazla 100 karakter olabilir.")]
        public string ContactInfo { get; set; } 
    }
    
}
