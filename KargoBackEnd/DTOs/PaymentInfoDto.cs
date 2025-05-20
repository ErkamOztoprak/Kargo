using System.ComponentModel.DataAnnotations;

namespace KargoUygulamasiBackEnd.DTOs
{
    public class PaymentInfoDto
    {
        [Range(0, double.MaxValue, ErrorMessage = "Önerilen ücret negatif olamaz.")]
        public decimal? ProposedFee { get; set; }
    }
}
