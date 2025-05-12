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
        public string TrackingNumber { get; set; }

        [MaxLength(100)]
        public string SenderName { get; set; }

        public DateTime ArrivalTime { get; set; }

        [Required,MaxLength(20)]
        public string Status { get; set; }

        [Required]
        public string QrToken { get; set; }
        public DateTime QrExpiresAt { get; set; }   
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<DeliveryLog> DeliveryLogs { get; set; }
    }
}
