// Archivo: IAccessMonitoringDataRepository.cs
using ApiBasesDeDatosProyecto.Models;
using System.Threading.Tasks;

namespace ApiBasesDeDatosProyecto.Repository
{
    public interface IAccessMonitoringDataRepository
    {
        /// <summary>
        /// Obtiene todos los registros de AccessMonitoringData.
        /// </summary>
        /// <returns>Una lista de AccessMonitoringData.</returns>
        Task<IEnumerable<AccessMonitoringData>> GetAllAsync();

        /// <summary>
        /// Obtiene el último registro de AccessMonitoringData.
        /// </summary>
        /// <returns>El último registro de AccessMonitoringData.</returns>
        Task<AccessMonitoringData> GetLastAsync();

        /// <summary>
        /// Añade un nuevo registro de AccessMonitoringData a la base de datos.
        /// </summary>
        /// <param name="model">El modelo de AccessMonitoringData a añadir.</param>
        /// <returns>El modelo de AccessMonitoringData añadido.</returns>
        Task<AccessMonitoringData> AddCliente(AccessMonitoringData model);

        Task AddAsync(AccessMonitoringData monitoringData);
    }
}
