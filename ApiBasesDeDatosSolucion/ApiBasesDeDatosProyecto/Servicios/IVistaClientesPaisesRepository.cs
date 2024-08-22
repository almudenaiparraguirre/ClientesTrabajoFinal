namespace ApiBasesDeDatosProyecto.Servicios
{
    public interface IVistaClientesPaisesRepository
    {
        Task<List<VistaClientesPaises>> ObtenerTodos();
        Task<VistaClientesPaises?> ObtenerPorClienteId(int clienteId);
    }
}
