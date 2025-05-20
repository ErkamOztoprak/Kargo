using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KargoBackEnd.Models;
using Microsoft.EntityFrameworkCore;
using KargoUygulamasiBackEnd.Models;

namespace KargoBackEnd.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options): base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<DeliveryLog> DeliveryLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<LogUser> LogUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Parcel>().ToTable("parcels");
            modelBuilder.Entity<DeliveryLog>().ToTable("deliverylogs");
            modelBuilder.Entity<Notification>().ToTable("notifications");
            modelBuilder.Entity<LogUser>().ToTable("logusers");

            modelBuilder.Entity<User>()
                .HasMany(u => u.RequestedParcels)
                .WithOne(p => p.RequestedByUser)
                .HasForeignKey(p => p.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.AssignedParcelsToDeliver)
                .WithOne(n => n.AssignedToUser)
                .HasForeignKey(n => n.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Parcel>()
                .HasMany(p => p.DeliveryLogs)
                .WithOne(d => d.Parcel)
                .HasForeignKey(d => d.ParcelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.DeliveryLogs)
                .WithOne(d => d.ConfirmedBy)
                .HasForeignKey(d => d.ConfirmedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DeliveryLog>()
                .HasOne(d => d.VerifiedByAdmin)
                .WithMany()
                .HasForeignKey(d => d.VerifiedByAdminId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notification)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<LogUser>()
                .HasOne(l => l.User)
                .WithMany(u => u.LogUser)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Parcel>()
                .Property(p => p.Status)
                .HasMaxLength(50);

            modelBuilder.Entity<Parcel>()
                .Property(p => p.PaymentProposedFee)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<DeliveryLog>()
                .Property(dl => dl.EventType)
                .HasConversion<string>() 
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

        }
    }
}