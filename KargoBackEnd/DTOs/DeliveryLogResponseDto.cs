using KargoUygulamasiBackEnd.DTOs;
namespace KargoUygulamasiBackEnd.DTOs
{
    public class DeliveryLogResponseDto
    {
        public int Id { get; set; }
        public DateTime ConfirmedAt { get; set; }
        public string EventType { get; set; } 
        public string? Notes { get; set; }
        public UserMinimalDto ConfirmedBy { get; set; }
        public UserMinimalDto? VerifiedByAdmin { get; set; } // Opsiyonel
        public string? QrTokenVerified { get; set; } // Opsiyonel
    }
}
