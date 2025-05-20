namespace KargoUygulamasiBackEnd.DTOs
{
    public class UserMinimalDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicture { get; set; } 
        public double? Rating { get; set; }
    }
}
