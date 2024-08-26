
namespace ApiBasesDeDatosProyecto.Servicios
{
    public interface IPaisRepository: IRepository<Pais>
    {
        Task<Pais?> ObtenerPorNombre(string nombre);
    }
}
