namespace ApiBasesDeDatosProyecto.Perfiles
{
    public class usuariosPerfil : Profile
    {

        public usuariosPerfil()
        {
            CreateMap<EditUserModel, ApplicationUser>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Nombre + " " + src.Apellido))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeMilliseconds(src.FechaNacimiento).UtcDateTime));
        }
    }
}
