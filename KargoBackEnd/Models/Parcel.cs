using KargoBackEnd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KargoUygulamasiBackEnd.Models
{
    [Table("parcels")]
    public class Parcel
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? TrackingNumber { get; set; }

        public DateTime? ArrivalTime { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public string? QrToken { get; set; }
        public DateTime? QrExpiresAt { get; set; }

        [Required(ErrorMessage = "Gönderici adı gereklidir.")]
        [MaxLength(100, ErrorMessage = "Gönderici adı en fazla 100 karakter olabilir.")]
        public string SenderName { get; set; }

        [Required(ErrorMessage = "Gönderici iletişim bilgisi gereklidir.")]
        [MaxLength(100, ErrorMessage = "Gönderici iletişim bilgisi en fazla 100 karakter olabilir.")]
        public string SenderContactInfo { get; set; }

        [Required(ErrorMessage = "Alıcı adı gereklidir.")]
        [MaxLength(100, ErrorMessage = "Alıcı adı en fazla 100 karakter olabilir.")]
        public string ReceiverName { get; set; }

        [Required(ErrorMessage = "Alıcı iletişim bilgisi gereklidir.")]
        [MaxLength(100, ErrorMessage = "Alıcı iletişim bilgisi en fazla 100 karakter olabilir.")]
        public string ReceiverContactInfo { get; set; }

        [Required(ErrorMessage = "Kargo açıklaması/içeriği gereklidir.")]
        [MaxLength(500, ErrorMessage = "Kargo açıklaması en fazla 500 karakter olabilir.")]
        public string CargoDescription { get; set; }

        [Required(ErrorMessage = "Kargonun alınacağı yer gereklidir.")]
        [MaxLength(255, ErrorMessage = "Alım konumu en fazla 255 karakter olabilir.")]
        public string CargoPickupLocation { get; set; }

        [Required(ErrorMessage = "Teslim edileceği yer gereklidir.")]
        [MaxLength(255, ErrorMessage = "Teslimat konumu en fazla 255 karakter olabilir.")]
        public string CargoDeliveryLocation { get; set; }

        [MaxLength(500, ErrorMessage = "Özel notlar en fazla 500 karakter olabilir.")]
        public string? CargoSpecialInstructions { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PaymentProposedFee { get; set; }

        [Required]
        public int RequestedByUserId { get; set; }

        [InverseProperty("RequestedParcels")]
        public virtual User RequestedByUser { get; set; }
        
        public int? AssignedToUserId { get; set; }

        [InverseProperty("AssignedParcelsToDeliver")]
        public virtual User? AssignedToUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<DeliveryLog>? DeliveryLogs { get; set; }

        public Parcel()
        {
            DeliveryLogs = new HashSet<DeliveryLog>();
        }
    }
}