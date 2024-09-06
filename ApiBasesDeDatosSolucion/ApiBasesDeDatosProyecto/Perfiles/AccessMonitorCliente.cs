namespace ApiBasesDeDatosProyecto.Perfiles
{
    public class AccesMonitorCliente : Profile
    {
        public AccesMonitorCliente()
        {
            CreateMap<Cliente, AccessMonitoringData>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.FechaNacimiento, opt => opt.MapFrom(src => src.dateOfBirth))
                .ForMember(dest => dest.Empleo, opt => opt.MapFrom(src => src.Empleo))
                .ForMember(dest => dest.PaisId, opt => opt.MapFrom(src => src.PaisId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Pais, opt => opt.Ignore()) // Ignorar porque no tiene mapeo directo
                .ForMember(dest => dest.Usuario, opt => opt.Ignore()) // Ignorar porque no existe en Cliente
                .ForMember(dest => dest.TipoAcceso, opt => opt.Ignore()) // Ignorar porque no existe en Cliente
                .ForMember(dest => dest.FechaRecibido, opt => opt.Ignore()) // Ignorar porque no existe en Cliente
                .ReverseMap()
                .ForMember(dest => dest.Pais, opt => opt.Ignore()) // Ignorar también en el mapeo inverso
                .ForMember(dest => dest.PaisId, opt => opt.MapFrom(src => src.PaisId)) // Mantener mapeo directo
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)) // Mapeo directo
                .ForMember(dest => dest.Empleo, opt => opt.MapFrom(src => src.Empleo)) // Mapeo directo
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre)) // Mapeo directo
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido)) // Mapeo directo
                .ForMember(dest => dest.dateOfBirth, opt => opt.MapFrom(src => src.FechaNacimiento)) // Mapeo directo
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id no está en AccessMonitoringData
        }
    }
}
