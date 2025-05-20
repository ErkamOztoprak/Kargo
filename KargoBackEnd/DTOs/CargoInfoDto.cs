using System.ComponentModel.DataAnnotations;

namespace KargoUygulamasiBackEnd.DTOs
{
    public class CargoInfoDto
    {
        [Required(ErrorMessage = "Kargo açıklaması gereklidir.")]
        [StringLength(500, ErrorMessage = "Kargo açıklaması en fazla 500 karakter olabilir.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kargonun alınacağı yer gereklidir.")]
        [StringLength(200, ErrorMessage = "Alınacak yer en fazla 200 karakter olabilir.")]
        public string PickupLocation { get; set; }

        [Required(ErrorMessage = "Kargonun teslim edileceği yer gereklidir.")]
        [StringLength(200, ErrorMessage = "Teslim edileceği yer en fazla 200 karakter olabilir.")]
        public string DeliveryLocation { get; set; }

        [StringLength(500, ErrorMessage = "Özel talimatlar en fazla 500 karakter olabilir.")]
        public string? SpecialInstructions { get; set; }
    }
}
