namespace ApiBasesDeDatosProyecto.Servicios
{
    public interface ITokenRepository
    {
        TokenDecodeDTO DecodeJwt(string token);
    }
}
