namespace ApiBasesDeDatosProyecto.Models
{
    public class TokenDecodeDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Jti { get; set; } // Token ID
        public long Exp { get; set; }   // Expiration time
        public string Issuer { get; set; }
        public string Audience { get; set; }

    }
}
