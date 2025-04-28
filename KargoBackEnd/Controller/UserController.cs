using KargoBackEnd.Context;
using KargoBackEnd.Models;
using KargoUygulamasiBackEnd.Helpers;
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

        [HttpPost("authenticate")]//login
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if (userObj == null || string.IsNullOrEmpty(userObj.UserName) || string.IsNullOrEmpty(userObj.Password))
                return BadRequest(new { Message = "Invalid request" });

            try
            {
                var user = await _authContext.Users.FirstOrDefaultAsync(x => x.UserName == userObj.UserName);

                if (user == null || !_passwordHasher.Verify(userObj.Password, user.Password, user.Salt))
                    return Unauthorized(new { Message = "Invalid Credentials" });

                var jwtToken = GenerateJwtToken(user);

                return Ok(new AuthenticateResponseDto
                {
                    Token = jwtToken,
                    ExpiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"]) * 60,
                    message = "Login successful!",
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
        public class AuthenticateResponseDto
        {
            public string Token { get; set; }
            public int ExpiresIn { get; set; }
            public string message { get; set; }
            public string UserName { get; set; } // New field
            public string Email { get; set; }    // New field
            public string Role { get; set; }
        }
        public class ApiResponseDto
        {
            public string Message { get; set; }
            public object Data { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userobj)
        {
            if (userobj == null)
                return BadRequest("User Data is Required");

            if (string.IsNullOrEmpty(userobj.Email) || string.IsNullOrEmpty(userobj.Password))
                return BadRequest("Email and Password is Required");

            if (await _authContext.Users.AnyAsync(x => x.Email == userobj.Email))
                return BadRequest("Email is Already Exist");

            if(!IsPasswordValid(userobj.Password))
                return BadRequest("Password is Not Strong Enough"); 

            string salt;
            userobj.Password = _passwordHasher.Hash(userobj.Password,out salt);
            userobj.Salt = salt;

            if (string.IsNullOrEmpty(userobj.Role))
            {
                userobj.Role = "User";
            }

            await _authContext.Users.AddAsync(userobj);
            await _authContext.SaveChangesAsync();

            return Ok(new { Message = "User Login Successfully ", userobj.Password, userobj.Salt });

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
    }
}

