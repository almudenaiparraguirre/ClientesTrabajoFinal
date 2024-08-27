using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiBasesDeDatosProyecto.Repository
{
    public class MonitoringDataRepository : IMonitoringDataRepository
    {
        private readonly Contexto _context;
        private static readonly HttpClient _httpClient = new HttpClient();

        public MonitoringDataRepository(Contexto context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MonitoringData>> GetAllAsync()
        {
            return await _context.MonitoringDatas.ToListAsync();
        }

        public async Task<MonitoringData> GetByIdAsync(int id)
        {
            return await _context.MonitoringDatas.FindAsync(id);
        }

        public async Task AddAsync(MonitoringData monitoringData)
        {
            _context.MonitoringDatas.Add(monitoringData);
            await _context.SaveChangesAsync();
            await NotifyChangesAsync(); // Llama a Notify después de la operación
        }

        public async Task UpdateAsync(MonitoringData monitoringData)
        {
            _context.MonitoringDatas.Update(monitoringData);
            await _context.SaveChangesAsync();
            await NotifyChangesAsync(); // Llama a Notify después de la operación
        }

        public async Task DeleteAsync(int id)
        {
            var monitoringData = await _context.MonitoringDatas.FindAsync(id);
            if (monitoringData != null)
            {
                _context.MonitoringDatas.Remove(monitoringData);
                await _context.SaveChangesAsync();
                await NotifyChangesAsync(); // Llama a Notify después de la operación
            }
        }

        private async Task NotifyChangesAsync()
        {
            // Asume que la API está alojada localmente; ajusta la URL según sea necesario
            var notifyUrl = "https://localhost:7107/api/Monitoring/notify";
            var response = await _httpClient.PostAsync(notifyUrl, null);

            if (!response.IsSuccessStatusCode)
            {
                // Manejo de errores (log, excepciones, etc.)
                Console.WriteLine("Error notifying changes.");
            }
        }
    }
}
