using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KargoBackEnd.Models
{
    [Table("logusers")]
    public class LogUser
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        [MaxLength(100)]
        public string Action { get; set; } // Login, Logout, PasswordChange vb.
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
