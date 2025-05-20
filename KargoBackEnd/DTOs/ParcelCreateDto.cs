using System.ComponentModel.DataAnnotations;

namespace KargoUygulamasiBackEnd.DTOs
{
    public class ParcelCreateDto
    {
        [Required]
        public SenderInfoDto Sender { get; set; }

        [Required]
        public ReceiverInfoDto Receiver { get; set; }

        [Required]
        public CargoInfoDto Cargo { get; set; }

        public PaymentInfoDto? Payment { get; set; }
    }
}
