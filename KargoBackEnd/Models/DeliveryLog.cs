using KargoBackEnd.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace KargoUygulamasiBackEnd.Models
{
    [Table("deliverylogs")]
    public class DeliveryLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Parcel))]
        public int ParcelId { get; set; }
        public Parcel Parcel { get; set; }

        [ForeignKey(nameof(ConfirmedBy))]
        public int ConfirmedByUserId { get; set; }
        public User ConfirmedBy { get; set; }

        [ForeignKey(nameof(VerifiedByAdmin))]
        public int? VerifiedByAdminId { get; set; }
        public User VerifiedByAdmin { get; set; }
        
        public DateTime ConfirmedAt { get; set; }
        public string QrTokenVerified { get; set; }

    }
}


