namespace KargoUygulamasiBackEnd.DTOs
{
        public class AuthenticateResponseDto
        {
            public string Token { get; set; }
            public int ExpiresIn { get; set; }
            public string message { get; set; }
            public string UserName { get; set; } // New field
            public string Email { get; set; }    // New field
            public string Role { get; set; }
            
        }
    
}
