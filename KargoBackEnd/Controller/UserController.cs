using KargoBackEnd.Context;
using KargoBackEnd.Models;
using KargoUygulamasiBackEnd.Helpers;
using KargoUygulamasiBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KargoUygulamasiBackEnd.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;
        private readonly PasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext appDbContext, PasswordHasher passwordHasher,IConfiguration configuration)
        {
            _authContext = appDbContext;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { Message = "User ID not found in token" });
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest(new { Message = "Invalid User ID" });
            }

            var user = await _authContext.Users
                .Include(u => u.Notification) 
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var userProfile = new
            {
                
                user.FirstName,
                user.LastName,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.ProfilePicture,
                user.Rating,
                user.IsVerified,
                
            };

            return Ok(userProfile);
        }
        [Authorize]
        [HttpPut("profile/update")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] ProfileUpdateDto profileUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Message = "User ID not found in token" });

            if (!int.TryParse(userIdClaim, out int userIdNum))
                return BadRequest(new { Message = "Invalid User ID" });

            var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Id == userIdNum);

            if (user == null)
                return NotFound(new { Message = "User not found" });

            if (profileUpdateDto.FirstName == null &&
                profileUpdateDto.LastName == null &&
                profileUpdateDto.Email == null &&
                profileUpdateDto.PhoneNumber == null)
            {
                return BadRequest(new { Message = "At least one field must be provided for update." });
            }
            user.FirstName = profileUpdateDto.FirstName ?? user.FirstName;
            user.LastName = profileUpdateDto.LastName ?? user.LastName;
            user.PhoneNumber = profileUpdateDto.PhoneNumber ?? user.PhoneNumber;


            if (!string.IsNullOrEmpty(profileUpdateDto.Email) && !string.Equals(user.Email, profileUpdateDto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _authContext.Users.AnyAsync(u => u.Email == profileUpdateDto.Email && u.Id != userIdNum);
                if (existingUser)
                    return BadRequest(new { Message = "Email already exists" });

                user.Email = profileUpdateDto.Email;
            }

            user.UpdatedAt = DateTime.UtcNow;

            try
            {
                _authContext.Users.Update(user);
                await _authContext.SaveChangesAsync();
                return Ok(new { Message = "Profile updated successfully",User = user });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while updating the profile" });
            }
            

        }
        private async Task<bool> UserExists(int id)
        {
            return await _authContext.Users.AnyAsync(e => e.Id == id);
        }

        [HttpPost("authenticate")]//login
        public async Task<IActionResult> Authenticate([FromBody] LoginRequestDto userObj)
        {
            Console.WriteLine($"Received User Data: {System.Text.Json.JsonSerializer.Serialize(userObj)}");

            if (userObj == null || string.IsNullOrEmpty(userObj.UserName) || string.IsNullOrEmpty(userObj.Password))
                return BadRequest(new { Message = "Invalid request" });

            try
            {
                var user = await _authContext.Users.FirstOrDefaultAsync(x => x.UserName == userObj.UserName);

                if (user == null || !_passwordHasher.Verify(userObj.Password, user.Password, user.Salt))
                    return Unauthorized(new { Message = "Invalid Credentials" });

                var jwtToken = GenerateJwtToken(user);

                user.LastLogin = DateTime.UtcNow;
                _authContext.Users.Update(user);
                await _authContext.SaveChangesAsync();

                return Ok(new AuthenticateResponseDto
                {
                    Token = jwtToken,
                    ExpiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"]) * 60,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = user.Role
                    

                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during authentication: {ex.Message}");
                return StatusCode(500, new { Message = "Authentication Failed" });
            }
        }
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            if (!string.IsNullOrEmpty(user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]



            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
        public class ApiResponseDto
        {
            public string Message { get; set; }
            public object Data { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto request)
        {
            if (request == null)
                return BadRequest("User Data is Required");

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Email and Password is Required");

            if (await _authContext.Users.AnyAsync(x => x.Email == request.Email))
                return BadRequest("Email is Already Exist");

            if(!IsPasswordValid(request.Password))
                return BadRequest("Password is Not Strong Enough"); 

            string salt;
            request.Password = _passwordHasher.Hash(request.Password,out salt);

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = request.Password,
                Salt = salt,
                Role = "User",
                Status = UserStatus.Active,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow,
                Rating = 3.0,
            }; 

            await _authContext.Users.AddAsync(user);
            await _authContext.SaveChangesAsync();

            return Ok(new { Message = "User Registered Successfully " });

        }

        [Authorize(Roles ="Admin")]
        [HttpPut("update-role/{userId}")]
        public async Task<IActionResult> UpdateRole(int userId, [FromBody] string newRole)
        {
            if (string.IsNullOrEmpty(newRole) || (newRole != "Admin" && newRole != "User"))
                return BadRequest(new { Message = "Invalid Role" });
            
            var user = await _authContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User Not Found" });

            user.Role = newRole;
            _authContext.Users.Update(user);

            return Ok(new { Message = "Role Updated Successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult Admin()
        {
            return Ok(new { Message = "Admin access granted" });
        }

        private bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;
            if (password.Length < 8)
                return false;
            if (!password.Any(char.IsUpper))
                return false;
            if (!password.Any(char.IsLower))
                return false;
            if (!password.Any(char.IsDigit))
                return false;
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                return false;

            return true;
        }
        [Authorize]
        [HttpGet("protected-endpoint")]
        public IActionResult ProtectedEndpoint()
        {
            return Ok(new { Message = "This is a protected endpoint" });
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update-isverified/{userId}")]
        public async Task<IActionResult> UpdateIsVerified(int userId, [FromBody] bool isVerified)
        {
            
            var user = await _authContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            
            user.IsVerified = isVerified;
            _authContext.Users.Update(user);
            await _authContext.SaveChangesAsync();

            return Ok(new { Message = "User verification status updated successfully", IsVerified = user.IsVerified });
        }

    }
}

