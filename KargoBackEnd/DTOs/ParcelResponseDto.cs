using KargoUygulamasiBackEnd.DTOs;

namespace KargoUygulamasiBackEnd.DTOs
{
    public class ParcelResponseDto
    {
        public int Id { get; set; }
        public string TrackingNumber { get; set; }
        public string Status { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public string? QrToken { get; set; }
        public DateTime? QrExpiresAt { get; set; }

        public string SenderName { get; set; }
        public string SenderContactInfo { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverContactInfo { get; set; }

        public string CargoDescription { get; set; }
        public string CargoPickupLocation { get; set; }
        public string CargoDeliveryLocation { get; set; }
        public string? CargoSpecialInstructions { get; set; }

        public decimal? PaymentProposedFee { get; set; }

        public UserMinimalDto RequestedByUser { get; set; }
        public UserMinimalDto? AssignedToUser { get; set; } // Kargoyu teslim edecek kişi (kurye)

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<DeliveryLogResponseDto> DeliveryLogs { get; set; } = new List<DeliveryLogResponseDto>();
    }
}
