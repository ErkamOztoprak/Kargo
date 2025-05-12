using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KargoUygulamasiBackEnd.Models;

namespace KargoBackEnd.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required, MaxLength(50)]
        public string UserName { get; set; }

        [Required, MaxLength(100), EmailAddress]
        public string Email { get; set; }

        [MaxLength(20), Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string Role { get; set; }

        [Required]
        public UserStatus Status { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Salt { get; set; }

        [Required]
        public bool IsVerified { get; set; }

        [MaxLength(255)]
        public string ProfilePicture { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }

        public double Rating { get; set; }

        public ICollection<DeliveryLog> DeliveryLogs { get; set; }
        public ICollection<Notification> Notification { get; set; }
        public ICollection<LogUser> LogUser { get; set; }

    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Suspended
    }
}