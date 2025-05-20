using KargoBackEnd.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace KargoUygulamasiBackEnd.Models
{
    public enum DeliveryLogType
    {
        Created,
        Accepted,
        AssignedToCarrier,    
        PickupScan,           
        OutForDelivery,       
        DeliveryAttemptFailed,
        Delivered,            
        Cancelled,            
        Other                 
    }

    [Table("deliverylogs")]
    public class DeliveryLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Parcel))]
        public int ParcelId { get; set; }
        public virtual Parcel Parcel { get; set; }

        [ForeignKey(nameof(ConfirmedBy))]
        public int ConfirmedByUserId { get; set; }
        public virtual User ConfirmedBy { get; set; }

        [ForeignKey(nameof(VerifiedByAdmin))]
        public int? VerifiedByAdminId { get; set; }
        public virtual User VerifiedByAdmin { get; set; }
        
        public DateTime ConfirmedAt { get; set; }
        public string? QrTokenVerified { get; set; }

        [Required]
        public DeliveryLogType EventType { get; set; }
        public string? Notes { get; set; }
    }
}


