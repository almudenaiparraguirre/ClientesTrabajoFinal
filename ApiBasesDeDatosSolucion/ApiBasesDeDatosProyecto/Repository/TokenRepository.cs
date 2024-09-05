using System.IdentityModel.Tokens.Jwt;

namespace ApiBasesDeDatosProyecto.Repository
{
    public class TokenRepository : ITokenRepository
    {
        public TokenDecodeDTO DecodeJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtDto = new TokenDecodeDTO();

            // Verifica si el token es un JWT válido
            if (handler.CanReadToken(token))
            {
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                // Recorre los claims y asigna los valores relevantes al DTO
                foreach (var claim in jsonToken.Claims)
                {
                    switch (claim.Type)
                    {
                        case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress":
                            jwtDto.Email = claim.Value;
                            break;
                        case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name":
                            jwtDto.Name = claim.Value;
                            break;
                        case "http://schemas.microsoft.com/ws/2008/06/identity/claims/role":
                            jwtDto.Role = claim.Value;
                            break;
                        case "jti":
                            jwtDto.Jti = claim.Value;
                            break;
                        case "exp":
                            jwtDto.Exp = long.Parse(claim.Value);
                            break;
                        case "iss":
                            jwtDto.Issuer = claim.Value;
                            break;
                        case "aud":
                            jwtDto.Audience = claim.Value;
                            break;
                    }
                }
            }
            else
            {
                // Si el token no es válido, puedes manejar el error a tu manera
                throw new ArgumentException("Token no válido");
            }

            return jwtDto;
        }
    }
}
