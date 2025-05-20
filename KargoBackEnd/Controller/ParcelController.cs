using KargoBackEnd.Context;
using KargoUygulamasiBackEnd.DTOs;
using KargoBackEnd.Models;
using KargoUygulamasiBackEnd.DTOs;

using KargoUygulamasiBackEnd.Models;
// using KargoUygulamasiBackEnd.Models; // DeliveryLogType enum'ı için (eğer buradaysa)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KargoBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParcelsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ParcelsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ParcelResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateParcel([FromBody] ParcelCreateDto parcelCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized(new { message = "Kullanıcı kimliği belirlenemedi." });
            }

            var requestingUser = await _context.Users.FindAsync(currentUserId);
            if (requestingUser == null)
            {
                return Unauthorized(new { message = "Geçersiz kullanıcı." });
            }

            var parcel = new Parcel
            {
                SenderName = parcelCreateDto.Sender.Name,
                SenderContactInfo = parcelCreateDto.Sender.ContactInfo,
                ReceiverName = parcelCreateDto.Receiver.Name,
                ReceiverContactInfo = parcelCreateDto.Receiver.ContactInfo,
                CargoDescription = parcelCreateDto.Cargo.Description,
                CargoPickupLocation = parcelCreateDto.Cargo.PickupLocation,
                CargoDeliveryLocation = parcelCreateDto.Cargo.DeliveryLocation,
                CargoSpecialInstructions = parcelCreateDto.Cargo.SpecialInstructions,
                PaymentProposedFee = parcelCreateDto.Payment?.ProposedFee,
                RequestedByUserId = currentUserId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TrackingNumber = Guid.NewGuid().ToString().Substring(0, 10).ToUpper(),
            };

            _context.Parcels.Add(parcel);

            var deliveryLog = new DeliveryLog
            {
                Parcel = parcel,
                ConfirmedByUserId = currentUserId,
                ConfirmedAt = DateTime.UtcNow,
                EventType = DeliveryLogType.Created,
                Notes = "Kargo talebi oluşturuldu."
            };
            _context.DeliveryLogs.Add(deliveryLog);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Kargo oluşturulurken bir veritabanı hatası oluştu." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Kargo oluşturulurken beklenmedik bir hata oluştu." });
            }

            var parcelResponse = new ParcelResponseDto
            {
                Id = parcel.Id,
                TrackingNumber = parcel.TrackingNumber,
                Status = parcel.Status,
                ArrivalTime = parcel.ArrivalTime,
                QrToken = parcel.QrToken,
                QrExpiresAt = parcel.QrExpiresAt,
                SenderName = parcel.SenderName,
                SenderContactInfo = parcel.SenderContactInfo,
                ReceiverName = parcel.ReceiverName,
                ReceiverContactInfo = parcel.ReceiverContactInfo,
                CargoDescription = parcel.CargoDescription,
                CargoPickupLocation = parcel.CargoPickupLocation,
                CargoDeliveryLocation = parcel.CargoDeliveryLocation,
                CargoSpecialInstructions = parcel.CargoSpecialInstructions,
                PaymentProposedFee = parcel.PaymentProposedFee,
                RequestedByUser = new UserMinimalDto
                {
                    Id = requestingUser.Id,
                    UserName = requestingUser.UserName,
                    FirstName = requestingUser.FirstName,
                    LastName = requestingUser.LastName,
                    ProfilePicture = requestingUser.ProfilePicture,
                    Rating = requestingUser.Rating
                },
                AssignedToUser = null,
                CreatedAt = parcel.CreatedAt,
                UpdatedAt = parcel.UpdatedAt,
                DeliveryLogs = new List<DeliveryLogResponseDto>
                {
                    new DeliveryLogResponseDto
                    {
                        Id = deliveryLog.Id,
                        ConfirmedAt = deliveryLog.ConfirmedAt,
                        EventType = deliveryLog.EventType.ToString(),
                        Notes = deliveryLog.Notes,
                        ConfirmedBy = new UserMinimalDto
                        {
                            Id = requestingUser.Id,
                            UserName = requestingUser.UserName,
                            FirstName = requestingUser.FirstName,
                            LastName = requestingUser.LastName,
                            ProfilePicture = requestingUser.ProfilePicture,
                            Rating = requestingUser.Rating
                        },
                        VerifiedByAdmin = null
                    }
                }
            };
            return StatusCode(StatusCodes.Status201Created, parcelResponse);
        }

        [Authorize]
        [HttpPut("{id}/complete")]
        [ProducesResponseType(typeof(ParcelResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompleteParcel(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized(new { message = "Kullanıcı kimliği belirlenemedi." });
            }

            var parcel = await _context.Parcels
                .Include(p => p.RequestedByUser)
                .Include(p => p.AssignedToUser)
                .Include(p => p.DeliveryLogs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parcel == null)
                return NotFound(new { message = "Kargo bulunamadı." });

            if (parcel.AssignedToUserId != currentUserId)
                return BadRequest(new { message = "Sadece size atanmış kargoyu teslim edebilirsiniz." });

            if (parcel.Status == "Delivered")
                return BadRequest(new { message = "Kargo zaten teslim edilmiş." });

            // Kargoyu teslim edildi olarak işaretle
            parcel.Status = "Delivered";
            parcel.UpdatedAt = DateTime.UtcNow;

            // Teslimat logu ekle
            var deliveryLog = new DeliveryLog
            {
                ParcelId = parcel.Id,
                ConfirmedByUserId = currentUserId,
                ConfirmedAt = DateTime.UtcNow,
                EventType = DeliveryLogType.Delivered,
                Notes = "Kargo teslim edildi."
            };
            _context.DeliveryLogs.Add(deliveryLog);

            await _context.SaveChangesAsync();

            // Response DTO oluştur
            var parcelResponse = new ParcelResponseDto
            {
                Id = parcel.Id,
                TrackingNumber = parcel.TrackingNumber,
                Status = parcel.Status,
                ArrivalTime = parcel.ArrivalTime,
                QrToken = parcel.QrToken,
                QrExpiresAt = parcel.QrExpiresAt,
                SenderName = parcel.SenderName,
                SenderContactInfo = parcel.SenderContactInfo,
                ReceiverName = parcel.ReceiverName,
                ReceiverContactInfo = parcel.ReceiverContactInfo,
                CargoDescription = parcel.CargoDescription,
                CargoPickupLocation = parcel.CargoPickupLocation,
                CargoDeliveryLocation = parcel.CargoDeliveryLocation,
                CargoSpecialInstructions = parcel.CargoSpecialInstructions,
                PaymentProposedFee = parcel.PaymentProposedFee,
                RequestedByUser = parcel.RequestedByUser != null ? new UserMinimalDto
                {
                    Id = parcel.RequestedByUser.Id,
                    UserName = parcel.RequestedByUser.UserName,
                    FirstName = parcel.RequestedByUser.FirstName,
                    LastName = parcel.RequestedByUser.LastName,
                    ProfilePicture = parcel.RequestedByUser.ProfilePicture,
                    Rating = parcel.RequestedByUser.Rating
                } : null,
                AssignedToUser = parcel.AssignedToUser != null ? new UserMinimalDto
                {
                    Id = parcel.AssignedToUser.Id,
                    UserName = parcel.AssignedToUser.UserName,
                    FirstName = parcel.AssignedToUser.FirstName,
                    LastName = parcel.AssignedToUser.LastName,
                    ProfilePicture = parcel.AssignedToUser.ProfilePicture,
                    Rating = parcel.AssignedToUser.Rating
                } : null,
                CreatedAt = parcel.CreatedAt,
                UpdatedAt = parcel.UpdatedAt,
                DeliveryLogs = parcel.DeliveryLogs?.Select(dl => new DeliveryLogResponseDto
                {
                    Id = dl.Id,
                    ConfirmedAt = dl.ConfirmedAt,
                    EventType = dl.EventType.ToString(),
                    Notes = dl.Notes,
                    QrTokenVerified = dl.QrTokenVerified,
                    ConfirmedBy = dl.ConfirmedBy != null ? new UserMinimalDto
                    {
                        Id = dl.ConfirmedBy.Id,
                        UserName = dl.ConfirmedBy.UserName,
                        FirstName = dl.ConfirmedBy.FirstName,
                        LastName = dl.ConfirmedBy.LastName,
                        ProfilePicture = dl.ConfirmedBy.ProfilePicture,
                        Rating = dl.ConfirmedBy.Rating
                    } : null,
                    VerifiedByAdmin = dl.VerifiedByAdmin != null ? new UserMinimalDto
                    {
                        Id = dl.VerifiedByAdmin.Id,
                        UserName = dl.VerifiedByAdmin.UserName,
                        FirstName = dl.VerifiedByAdmin.FirstName,
                        LastName = dl.VerifiedByAdmin.LastName,
                        ProfilePicture = dl.VerifiedByAdmin.ProfilePicture,
                        Rating = dl.VerifiedByAdmin.Rating
                    } : null
                }).ToList() ?? new List<DeliveryLogResponseDto>()
            };

            return Ok(parcelResponse);
        }

        [Authorize]
        [HttpPut("{id}/cancel")]
        [ProducesResponseType(typeof(ParcelResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelParcel(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized(new { message = "Kullanıcı kimliği belirlenemedi." });
            }

            var parcel = await _context.Parcels
                .Include(p => p.RequestedByUser)
                .Include(p => p.AssignedToUser)
                .Include(p => p.DeliveryLogs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parcel == null)
                return NotFound(new { message = "Kargo bulunamadı." });

            if (parcel.RequestedByUserId != currentUserId)
                return BadRequest(new { message = "Sadece kendi oluşturduğunuz kargoyu iptal edebilirsiniz." });

            if (parcel.Status == "Cancelled")
                return BadRequest(new { message = "Kargo zaten iptal edilmiş." });

            if (parcel.Status == "Delivered")
                return BadRequest(new { message = "Teslim edilmiş kargo iptal edilemez." });

            // Kargoyu iptal et
            parcel.Status = "Cancelled";
            parcel.UpdatedAt = DateTime.UtcNow;

            // Teslimat logu ekle
            var deliveryLog = new DeliveryLog
            {
                ParcelId = parcel.Id,
                ConfirmedByUserId = currentUserId,
                ConfirmedAt = DateTime.UtcNow,
                EventType = DeliveryLogType.Cancelled,
                Notes = "Kargo talebi iptal edildi."
            };
            _context.DeliveryLogs.Add(deliveryLog);

            await _context.SaveChangesAsync();

            // Response DTO oluştur
            var parcelResponse = new ParcelResponseDto
            {
                Id = parcel.Id,
                TrackingNumber = parcel.TrackingNumber,
                Status = parcel.Status,
                ArrivalTime = parcel.ArrivalTime,
                QrToken = parcel.QrToken,
                QrExpiresAt = parcel.QrExpiresAt,
                SenderName = parcel.SenderName,
                SenderContactInfo = parcel.SenderContactInfo,
                ReceiverName = parcel.ReceiverName,
                ReceiverContactInfo = parcel.ReceiverContactInfo,
                CargoDescription = parcel.CargoDescription,
                CargoPickupLocation = parcel.CargoPickupLocation,
                CargoDeliveryLocation = parcel.CargoDeliveryLocation,
                CargoSpecialInstructions = parcel.CargoSpecialInstructions,
                PaymentProposedFee = parcel.PaymentProposedFee,
                RequestedByUser = parcel.RequestedByUser != null ? new UserMinimalDto
                {
                    Id = parcel.RequestedByUser.Id,
                    UserName = parcel.RequestedByUser.UserName,
                    FirstName = parcel.RequestedByUser.FirstName,
                    LastName = parcel.RequestedByUser.LastName,
                    ProfilePicture = parcel.RequestedByUser.ProfilePicture,
                    Rating = parcel.RequestedByUser.Rating
                } : null,
                AssignedToUser = parcel.AssignedToUser != null ? new UserMinimalDto
                {
                    Id = parcel.AssignedToUser.Id,
                    UserName = parcel.AssignedToUser.UserName,
                    FirstName = parcel.AssignedToUser.FirstName,
                    LastName = parcel.AssignedToUser.LastName,
                    ProfilePicture = parcel.AssignedToUser.ProfilePicture,
                    Rating = parcel.AssignedToUser.Rating
                } : null,
                CreatedAt = parcel.CreatedAt,
                UpdatedAt = parcel.UpdatedAt,
                DeliveryLogs = parcel.DeliveryLogs?.Select(dl => new DeliveryLogResponseDto
                {
                    Id = dl.Id,
                    ConfirmedAt = dl.ConfirmedAt,
                    EventType = dl.EventType.ToString(),
                    Notes = dl.Notes,
                    QrTokenVerified = dl.QrTokenVerified,
                    ConfirmedBy = dl.ConfirmedBy != null ? new UserMinimalDto
                    {
                        Id = dl.ConfirmedBy.Id,
                        UserName = dl.ConfirmedBy.UserName,
                        FirstName = dl.ConfirmedBy.FirstName,
                        LastName = dl.ConfirmedBy.LastName,
                        ProfilePicture = dl.ConfirmedBy.ProfilePicture,
                        Rating = dl.ConfirmedBy.Rating
                    } : null,
                    VerifiedByAdmin = dl.VerifiedByAdmin != null ? new UserMinimalDto
                    {
                        Id = dl.VerifiedByAdmin.Id,
                        UserName = dl.VerifiedByAdmin.UserName,
                        FirstName = dl.VerifiedByAdmin.FirstName,
                        LastName = dl.VerifiedByAdmin.LastName,
                        ProfilePicture = dl.VerifiedByAdmin.ProfilePicture,
                        Rating = dl.VerifiedByAdmin.Rating
                    } : null
                }).ToList() ?? new List<DeliveryLogResponseDto>()
            };

            return Ok(parcelResponse);
        }


        [HttpGet("my-created")]
        [ProducesResponseType(typeof(List<ParcelResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCreatedParcels()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized(new { message = "Kullanıcı kimliği belirlenemedi." });
            }

            var parcels = await _context.Parcels
                .Where(p => p.RequestedByUserId == currentUserId)
                .Include(p => p.RequestedByUser)
                .Include(p => p.AssignedToUser)
                .Include(p => p.DeliveryLogs)
                    .ThenInclude(dl => dl.ConfirmedBy)
                .Include(p => p.DeliveryLogs)
                    .ThenInclude(dl => dl.VerifiedByAdmin)
                .AsNoTracking()
                .ToListAsync();

            var parcelResponses = parcels.Select(parcel => new ParcelResponseDto
            {
                Id = parcel.Id,
                TrackingNumber = parcel.TrackingNumber,
                Status = parcel.Status,
                ArrivalTime = parcel.ArrivalTime,
                QrToken = parcel.QrToken,
                QrExpiresAt = parcel.QrExpiresAt,
                SenderName = parcel.SenderName,
                SenderContactInfo = parcel.SenderContactInfo,
                ReceiverName = parcel.ReceiverName,
                ReceiverContactInfo = parcel.ReceiverContactInfo,
                CargoDescription = parcel.CargoDescription,
                CargoPickupLocation = parcel.CargoPickupLocation,
                CargoDeliveryLocation = parcel.CargoDeliveryLocation,
                CargoSpecialInstructions = parcel.CargoSpecialInstructions,
                PaymentProposedFee = parcel.PaymentProposedFee,
                RequestedByUser = parcel.RequestedByUser != null ? new UserMinimalDto
                {
                    Id = parcel.RequestedByUser.Id,
                    UserName = parcel.RequestedByUser.UserName,
                    FirstName = parcel.RequestedByUser.FirstName,
                    LastName = parcel.RequestedByUser.LastName,
                    ProfilePicture = parcel.RequestedByUser.ProfilePicture,
                    Rating = parcel.RequestedByUser.Rating
                } : null,
                AssignedToUser = parcel.AssignedToUser != null ? new UserMinimalDto
                {
                    Id = parcel.AssignedToUser.Id,
                    UserName = parcel.AssignedToUser.UserName,
                    FirstName = parcel.AssignedToUser.FirstName,
                    LastName = parcel.AssignedToUser.LastName,
                    ProfilePicture = parcel.AssignedToUser.ProfilePicture,
                    Rating = parcel.AssignedToUser.Rating
                } : null,
                CreatedAt = parcel.CreatedAt,
                UpdatedAt = parcel.UpdatedAt,
                DeliveryLogs = parcel.DeliveryLogs?.Select(dl => new DeliveryLogResponseDto
                {
                    Id = dl.Id,
                    ConfirmedAt = dl.ConfirmedAt,
                    EventType = dl.EventType.ToString(),
                    Notes = dl.Notes,
                    QrTokenVerified = dl.QrTokenVerified,
                    ConfirmedBy = dl.ConfirmedBy != null ? new UserMinimalDto
                    {
                        Id = dl.ConfirmedBy.Id,
                        UserName = dl.ConfirmedBy.UserName,
                        FirstName = dl.ConfirmedBy.FirstName,
                        LastName = dl.ConfirmedBy.LastName,
                        ProfilePicture = dl.ConfirmedBy.ProfilePicture,
                        Rating = dl.ConfirmedBy.Rating
                    } : null,
                    VerifiedByAdmin = dl.VerifiedByAdmin != null ? new UserMinimalDto
                    {
                        Id = dl.VerifiedByAdmin.Id,
                        UserName = dl.VerifiedByAdmin.UserName,
                        FirstName = dl.VerifiedByAdmin.FirstName,
                        LastName = dl.VerifiedByAdmin.LastName,
                        ProfilePicture = dl.VerifiedByAdmin.ProfilePicture,
                        Rating = dl.VerifiedByAdmin.Rating
                    } : null
                }).ToList() ?? new List<DeliveryLogResponseDto>()
            }).ToList();

            return Ok(parcelResponses);
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<ParcelResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllParcels()
        {
            var parcels = await _context.Parcels
                .Include(p => p.RequestedByUser)
                .Include(p => p.AssignedToUser)
                .Include(p => p.DeliveryLogs)
                    .ThenInclude(dl => dl.ConfirmedBy)
                .Include(p => p.DeliveryLogs)
                    .ThenInclude(dl => dl.VerifiedByAdmin)
                .AsNoTracking()
                .ToListAsync();

            var parcelResponses = parcels.Select(parcel => new ParcelResponseDto
            {
                Id = parcel.Id,
                TrackingNumber = parcel.TrackingNumber,
                Status = parcel.Status,
                ArrivalTime = parcel.ArrivalTime,
                QrToken = parcel.QrToken,
                QrExpiresAt = parcel.QrExpiresAt,
                SenderName = parcel.SenderName,
                SenderContactInfo = parcel.SenderContactInfo,
                ReceiverName = parcel.ReceiverName,
                ReceiverContactInfo = parcel.ReceiverContactInfo,
                CargoDescription = parcel.CargoDescription,
                CargoPickupLocation = parcel.CargoPickupLocation,
                CargoDeliveryLocation = parcel.CargoDeliveryLocation,
                CargoSpecialInstructions = parcel.CargoSpecialInstructions,
                PaymentProposedFee = parcel.PaymentProposedFee,
                RequestedByUser = parcel.RequestedByUser != null ? new UserMinimalDto
                {
                    Id = parcel.RequestedByUser.Id,
                    UserName = parcel.RequestedByUser.UserName,
                    FirstName = parcel.RequestedByUser.FirstName,
                    LastName = parcel.RequestedByUser.LastName,
                    ProfilePicture = parcel.RequestedByUser.ProfilePicture,
                    Rating = parcel.RequestedByUser.Rating
                } : null,
                AssignedToUser = parcel.AssignedToUser != null ? new UserMinimalDto
                {
                    Id = parcel.AssignedToUser.Id,
                    UserName = parcel.AssignedToUser.UserName,
                    FirstName = parcel.AssignedToUser.FirstName,
                    LastName = parcel.AssignedToUser.LastName,
                    ProfilePicture = parcel.AssignedToUser.ProfilePicture,
                    Rating = parcel.AssignedToUser.Rating
                } : null,
                CreatedAt = parcel.CreatedAt,
                UpdatedAt = parcel.UpdatedAt,
                DeliveryLogs = parcel.DeliveryLogs?.Select(dl => new DeliveryLogResponseDto
                {
                    Id = dl.Id,
                    ConfirmedAt = dl.ConfirmedAt,
                    EventType = dl.EventType.ToString(),
                    Notes = dl.Notes,
                    QrTokenVerified = dl.QrTokenVerified,
                    ConfirmedBy = dl.ConfirmedBy != null ? new UserMinimalDto
                    {
                        Id = dl.ConfirmedBy.Id,
                        UserName = dl.ConfirmedBy.UserName,
                        FirstName = dl.ConfirmedBy.FirstName,
                        LastName = dl.ConfirmedBy.LastName,
                        ProfilePicture = dl.ConfirmedBy.ProfilePicture,
                        Rating = dl.ConfirmedBy.Rating
                    } : null,
                    VerifiedByAdmin = dl.VerifiedByAdmin != null ? new UserMinimalDto
                    {
                        Id = dl.VerifiedByAdmin.Id,
                        UserName = dl.VerifiedByAdmin.UserName,
                        FirstName = dl.VerifiedByAdmin.FirstName,
                        LastName = dl.VerifiedByAdmin.LastName,
                        ProfilePicture = dl.VerifiedByAdmin.ProfilePicture,
                        Rating = dl.VerifiedByAdmin.Rating
                    } : null
                }).ToList() ?? new List<DeliveryLogResponseDto>()
            }).ToList();

            return Ok(parcelResponses);
        }

        [Authorize]
        [HttpPut("{id}/accept")]
        [ProducesResponseType(typeof(ParcelResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AcceptParcel(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized(new { message = "Kullanıcı kimliği belirlenemedi." });
            }

            var parcel = await _context.Parcels
                .Include(p => p.RequestedByUser)
                .Include(p => p.AssignedToUser)
                .Include(p => p.DeliveryLogs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parcel == null)
                return NotFound(new { message = "Kargo bulunamadı." });

            if (parcel.RequestedByUserId == currentUserId)
                return BadRequest(new { message = "Kendi oluşturduğunuz kargoyu kabul edemezsiniz." });

            if (parcel.AssignedToUserId != null)
                return BadRequest(new { message = "Kargo zaten bir kullanıcıya atanmış." });

            // Kargoyu kullanıcıya ata ve durumunu güncelle
            parcel.AssignedToUserId = currentUserId;
            parcel.Status = "Accepted";
            parcel.UpdatedAt = DateTime.UtcNow;

            // Teslimat logu ekle
            var deliveryLog = new DeliveryLog
            {
                ParcelId = parcel.Id,
                ConfirmedByUserId = currentUserId,
                ConfirmedAt = DateTime.UtcNow,
                EventType = DeliveryLogType.Accepted, // Enum'da yoksa uygun bir değer ekleyin
                Notes = "Kargo kabul edildi."
            };
            _context.DeliveryLogs.Add(deliveryLog);

            await _context.SaveChangesAsync();

            // Response DTO oluştur
            var assignedUser = await _context.Users.FindAsync(currentUserId);
            var parcelResponse = new ParcelResponseDto
            {
                Id = parcel.Id,
                TrackingNumber = parcel.TrackingNumber,
                Status = parcel.Status,
                ArrivalTime = parcel.ArrivalTime,
                QrToken = parcel.QrToken,
                QrExpiresAt = parcel.QrExpiresAt,
                SenderName = parcel.SenderName,
                SenderContactInfo = parcel.SenderContactInfo,
                ReceiverName = parcel.ReceiverName,
                ReceiverContactInfo = parcel.ReceiverContactInfo,
                CargoDescription = parcel.CargoDescription,
                CargoPickupLocation = parcel.CargoPickupLocation,
                CargoDeliveryLocation = parcel.CargoDeliveryLocation,
                CargoSpecialInstructions = parcel.CargoSpecialInstructions,
                PaymentProposedFee = parcel.PaymentProposedFee,
                RequestedByUser = parcel.RequestedByUser != null ? new UserMinimalDto
                {
                    Id = parcel.RequestedByUser.Id,
                    UserName = parcel.RequestedByUser.UserName,
                    FirstName = parcel.RequestedByUser.FirstName,
                    LastName = parcel.RequestedByUser.LastName,
                    ProfilePicture = parcel.RequestedByUser.ProfilePicture,
                    Rating = parcel.RequestedByUser.Rating
                } : null,
                AssignedToUser = assignedUser != null ? new UserMinimalDto
                {
                    Id = assignedUser.Id,
                    UserName = assignedUser.UserName,
                    FirstName = assignedUser.FirstName,
                    LastName = assignedUser.LastName,
                    ProfilePicture = assignedUser.ProfilePicture,
                    Rating = assignedUser.Rating
                } : null,
                CreatedAt = parcel.CreatedAt,
                UpdatedAt = parcel.UpdatedAt,
                DeliveryLogs = parcel.DeliveryLogs?.Select(dl => new DeliveryLogResponseDto
                {
                    Id = dl.Id,
                    ConfirmedAt = dl.ConfirmedAt,
                    EventType = dl.EventType.ToString(),
                    Notes = dl.Notes,
                    QrTokenVerified = dl.QrTokenVerified,
                    ConfirmedBy = dl.ConfirmedBy != null ? new UserMinimalDto
                    {
                        Id = dl.ConfirmedBy.Id,
                        UserName = dl.ConfirmedBy.UserName,
                        FirstName = dl.ConfirmedBy.FirstName,
                        LastName = dl.ConfirmedBy.LastName,
                        ProfilePicture = dl.ConfirmedBy.ProfilePicture,
                        Rating = dl.ConfirmedBy.Rating
                    } : null,
                    VerifiedByAdmin = dl.VerifiedByAdmin != null ? new UserMinimalDto
                    {
                        Id = dl.VerifiedByAdmin.Id,
                        UserName = dl.VerifiedByAdmin.UserName,
                        FirstName = dl.VerifiedByAdmin.FirstName,
                        LastName = dl.VerifiedByAdmin.LastName,
                        ProfilePicture = dl.VerifiedByAdmin.ProfilePicture,
                        Rating = dl.VerifiedByAdmin.Rating
                    } : null
                }).ToList() ?? new List<DeliveryLogResponseDto>()
            };

            return Ok(parcelResponse);
        }

        [HttpGet("my-accepted")]
        [ProducesResponseType(typeof(List<ParcelResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyAcceptedParcels()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized(new { message = "Kullanıcı kimliği belirlenemedi." });
            }

            var parcels = await _context.Parcels
                .Where(p => p.AssignedToUserId == currentUserId && p.Status == "Accepted")
                .Include(p => p.RequestedByUser)
                .Include(p => p.AssignedToUser)
                .Include(p => p.DeliveryLogs)
                    .ThenInclude(dl => dl.ConfirmedBy)
                .Include(p => p.DeliveryLogs)
                    .ThenInclude(dl => dl.VerifiedByAdmin)
                .AsNoTracking()
                .ToListAsync();

            var parcelResponses = parcels.Select(parcel => new ParcelResponseDto
            {
                Id = parcel.Id,
                TrackingNumber = parcel.TrackingNumber,
                Status = parcel.Status,
                ArrivalTime = parcel.ArrivalTime,
                QrToken = parcel.QrToken,
                QrExpiresAt = parcel.QrExpiresAt,
                SenderName = parcel.SenderName,
                SenderContactInfo = parcel.SenderContactInfo,
                ReceiverName = parcel.ReceiverName,
                ReceiverContactInfo = parcel.ReceiverContactInfo,
                CargoDescription = parcel.CargoDescription,
                CargoPickupLocation = parcel.CargoPickupLocation,
                CargoDeliveryLocation = parcel.CargoDeliveryLocation,
                CargoSpecialInstructions = parcel.CargoSpecialInstructions,
                PaymentProposedFee = parcel.PaymentProposedFee,
                RequestedByUser = parcel.RequestedByUser != null ? new UserMinimalDto
                {
                    Id = parcel.RequestedByUser.Id,
                    UserName = parcel.RequestedByUser.UserName,
                    FirstName = parcel.RequestedByUser.FirstName,
                    LastName = parcel.RequestedByUser.LastName,
                    ProfilePicture = parcel.RequestedByUser.ProfilePicture,
                    Rating = parcel.RequestedByUser.Rating
                } : null,
                AssignedToUser = parcel.AssignedToUser != null ? new UserMinimalDto
                {
                    Id = parcel.AssignedToUser.Id,
                    UserName = parcel.AssignedToUser.UserName,
                    FirstName = parcel.AssignedToUser.FirstName,
                    LastName = parcel.AssignedToUser.LastName,
                    ProfilePicture = parcel.AssignedToUser.ProfilePicture,
                    Rating = parcel.AssignedToUser.Rating
                } : null,
                CreatedAt = parcel.CreatedAt,
                UpdatedAt = parcel.UpdatedAt,
                DeliveryLogs = parcel.DeliveryLogs?.Select(dl => new DeliveryLogResponseDto
                {
                    Id = dl.Id,
                    ConfirmedAt = dl.ConfirmedAt,
                    EventType = dl.EventType.ToString(),
                    Notes = dl.Notes,
                    QrTokenVerified = dl.QrTokenVerified,
                    ConfirmedBy = dl.ConfirmedBy != null ? new UserMinimalDto
                    {
                        Id = dl.ConfirmedBy.Id,
                        UserName = dl.ConfirmedBy.UserName,
                        FirstName = dl.ConfirmedBy.FirstName,
                        LastName = dl.ConfirmedBy.LastName,
                        ProfilePicture = dl.ConfirmedBy.ProfilePicture,
                        Rating = dl.ConfirmedBy.Rating
                    } : null,
                    VerifiedByAdmin = dl.VerifiedByAdmin != null ? new UserMinimalDto
                    {
                        Id = dl.VerifiedByAdmin.Id,
                        UserName = dl.VerifiedByAdmin.UserName,
                        FirstName = dl.VerifiedByAdmin.FirstName,
                        LastName = dl.VerifiedByAdmin.LastName,
                        ProfilePicture = dl.VerifiedByAdmin.ProfilePicture,
                        Rating = dl.VerifiedByAdmin.Rating
                    } : null
                }).ToList() ?? new List<DeliveryLogResponseDto>()
            }).ToList();

            return Ok(parcelResponses);
        }

        [HttpGet("my-completed")]
        [ProducesResponseType(typeof(List<ParcelResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCompletedParcels()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized(new { message = "Kullanıcı kimliği belirlenemedi." });
            }

            // Tamamlanmış statüler
            var completedStatuses = new[] { "Delivered", "Cancelled" };

            var parcels = await _context.Parcels
                .Where(p => p.AssignedToUserId == currentUserId && completedStatuses.Contains(p.Status))
                .Include(p => p.RequestedByUser)
                .Include(p => p.AssignedToUser)
                .Include(p => p.DeliveryLogs)
                    .ThenInclude(dl => dl.ConfirmedBy)
                .Include(p => p.DeliveryLogs)
                    .ThenInclude(dl => dl.VerifiedByAdmin)
                .AsNoTracking()
                .ToListAsync();

            var parcelResponses = parcels.Select(parcel => new ParcelResponseDto
            {
                Id = parcel.Id,
                TrackingNumber = parcel.TrackingNumber,
                Status = parcel.Status,
                ArrivalTime = parcel.ArrivalTime,
                QrToken = parcel.QrToken,
                QrExpiresAt = parcel.QrExpiresAt,
                SenderName = parcel.SenderName,
                SenderContactInfo = parcel.SenderContactInfo,
                ReceiverName = parcel.ReceiverName,
                ReceiverContactInfo = parcel.ReceiverContactInfo,
                CargoDescription = parcel.CargoDescription,
                CargoPickupLocation = parcel.CargoPickupLocation,
                CargoDeliveryLocation = parcel.CargoDeliveryLocation,
                CargoSpecialInstructions = parcel.CargoSpecialInstructions,
                PaymentProposedFee = parcel.PaymentProposedFee,
                RequestedByUser = parcel.RequestedByUser != null ? new UserMinimalDto
                {
                    Id = parcel.RequestedByUser.Id,
                    UserName = parcel.RequestedByUser.UserName,
                    FirstName = parcel.RequestedByUser.FirstName,
                    LastName = parcel.RequestedByUser.LastName,
                    ProfilePicture = parcel.RequestedByUser.ProfilePicture,
                    Rating = parcel.RequestedByUser.Rating
                } : null,
                AssignedToUser = parcel.AssignedToUser != null ? new UserMinimalDto
                {
                    Id = parcel.AssignedToUser.Id,
                    UserName = parcel.AssignedToUser.UserName,
                    FirstName = parcel.AssignedToUser.FirstName,
                    LastName = parcel.AssignedToUser.LastName,
                    ProfilePicture = parcel.AssignedToUser.ProfilePicture,
                    Rating = parcel.AssignedToUser.Rating
                } : null,
                CreatedAt = parcel.CreatedAt,
                UpdatedAt = parcel.UpdatedAt,
                DeliveryLogs = parcel.DeliveryLogs?.Select(dl => new DeliveryLogResponseDto
                {
                    Id = dl.Id,
                    ConfirmedAt = dl.ConfirmedAt,
                    EventType = dl.EventType.ToString(),
                    Notes = dl.Notes,
                    QrTokenVerified = dl.QrTokenVerified,
                    ConfirmedBy = dl.ConfirmedBy != null ? new UserMinimalDto
                    {
                        Id = dl.ConfirmedBy.Id,
                        UserName = dl.ConfirmedBy.UserName,
                        FirstName = dl.ConfirmedBy.FirstName,
                        LastName = dl.ConfirmedBy.LastName,
                        ProfilePicture = dl.ConfirmedBy.ProfilePicture,
                        Rating = dl.ConfirmedBy.Rating
                    } : null,
                    VerifiedByAdmin = dl.VerifiedByAdmin != null ? new UserMinimalDto
                    {
                        Id = dl.VerifiedByAdmin.Id,
                        UserName = dl.VerifiedByAdmin.UserName,
                        FirstName = dl.VerifiedByAdmin.FirstName,
                        LastName = dl.VerifiedByAdmin.LastName,
                        ProfilePicture = dl.VerifiedByAdmin.ProfilePicture,
                        Rating = dl.VerifiedByAdmin.Rating
                    } : null
                }).ToList() ?? new List<DeliveryLogResponseDto>()
            }).ToList();

            return Ok(parcelResponses);
        }




        [HttpGet("{id}", Name = "GetParcelById")]
        [ProducesResponseType(typeof(ParcelResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetParcelById(int id)
        {
            var parcel = await _context.Parcels
                .Include(p => p.RequestedByUser)
                .Include(p => p.AssignedToUser)
                .Include(p => p.DeliveryLogs)
                    .ThenInclude(dl => dl.ConfirmedBy)
                .Include(p => p.DeliveryLogs)
                    .ThenInclude(dl => dl.VerifiedByAdmin)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parcel == null)
            {
                return NotFound(new { message = "Kargo bulunamadı." });
            }

            var parcelResponse = new ParcelResponseDto
            {
                Id = parcel.Id,
                TrackingNumber = parcel.TrackingNumber,
                Status = parcel.Status,
                ArrivalTime = parcel.ArrivalTime,
                QrToken = parcel.QrToken,
                QrExpiresAt = parcel.QrExpiresAt,
                SenderName = parcel.SenderName,
                SenderContactInfo = parcel.SenderContactInfo,
                ReceiverName = parcel.ReceiverName,
                ReceiverContactInfo = parcel.ReceiverContactInfo,
                CargoDescription = parcel.CargoDescription,
                CargoPickupLocation = parcel.CargoPickupLocation,
                CargoDeliveryLocation = parcel.CargoDeliveryLocation,
                CargoSpecialInstructions = parcel.CargoSpecialInstructions,
                PaymentProposedFee = parcel.PaymentProposedFee,
                RequestedByUser = parcel.RequestedByUser != null ? new UserMinimalDto
                {
                    Id = parcel.RequestedByUser.Id,
                    UserName = parcel.RequestedByUser.UserName,
                    FirstName = parcel.RequestedByUser.FirstName,
                    LastName = parcel.RequestedByUser.LastName,
                    ProfilePicture = parcel.RequestedByUser.ProfilePicture,
                    Rating = parcel.RequestedByUser.Rating
                } : null,
                AssignedToUser = parcel.AssignedToUser != null ? new UserMinimalDto
                {
                    Id = parcel.AssignedToUser.Id,
                    UserName = parcel.AssignedToUser.UserName,
                    FirstName = parcel.AssignedToUser.FirstName,
                    LastName = parcel.AssignedToUser.LastName,
                    ProfilePicture = parcel.AssignedToUser.ProfilePicture,
                    Rating = parcel.AssignedToUser.Rating
                } : null,
                CreatedAt = parcel.CreatedAt,
                UpdatedAt = parcel.UpdatedAt,
                DeliveryLogs = parcel.DeliveryLogs.Select(dl => new DeliveryLogResponseDto
                {
                    Id = dl.Id,
                    ConfirmedAt = dl.ConfirmedAt,
                    EventType = dl.EventType.ToString(),
                    Notes = dl.Notes,
                    QrTokenVerified = dl.QrTokenVerified,
                    ConfirmedBy = dl.ConfirmedBy != null ? new UserMinimalDto
                    {
                        Id = dl.ConfirmedBy.Id,
                        UserName = dl.ConfirmedBy.UserName,
                        FirstName = dl.ConfirmedBy.FirstName,
                        LastName = dl.ConfirmedBy.LastName,
                        ProfilePicture = dl.ConfirmedBy.ProfilePicture,
                        Rating = dl.ConfirmedBy.Rating
                    } : null,
                    VerifiedByAdmin = dl.VerifiedByAdmin != null ? new UserMinimalDto
                    {
                        Id = dl.VerifiedByAdmin.Id,
                        UserName = dl.VerifiedByAdmin.UserName,
                        FirstName = dl.VerifiedByAdmin.FirstName,
                        LastName = dl.VerifiedByAdmin.LastName,
                        ProfilePicture = dl.VerifiedByAdmin.ProfilePicture,
                        Rating = dl.VerifiedByAdmin.Rating
                    } : null
                }).ToList()
            };
            return Ok(parcelResponse);
        }
    }
}