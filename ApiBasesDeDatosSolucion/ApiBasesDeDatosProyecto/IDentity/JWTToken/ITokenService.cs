public interface ITokenService
{
Task<string> GenerateJwtToken(ApplicationUser user);}
