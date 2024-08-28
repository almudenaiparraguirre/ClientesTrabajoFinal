namespace ApiBasesDeDatosProyecto.Servicios
{
    public interface IClienteRepository: IRepository<Cliente>
    {
        Task AddClienteAsync(Cliente cliente);
        Task<Cliente?> ObtenerPorEmail(string email);
        Task EditClienteAsync(Cliente cliente);
        Task EliminarClienteAsync(Cliente cliente);

        Task<Cliente?> ObtenerClientesPais(int id);
        Task<List<ProAlmClientePorPaisDto>> ObtenerClientesPorPaisAsync(int paisId);
    }
}
