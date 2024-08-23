namespace ApiBasesDeDatosProyecto.Repository
{
    public class MonitoringDataRepository : IMonitoringDataRepository
    {
        private readonly Contexto _context;

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
        }

        public async Task UpdateAsync(MonitoringData monitoringData)
        {
            _context.MonitoringDatas.Update(monitoringData);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var monitoringData = await _context.MonitoringDatas.FindAsync(id);
            if (monitoringData != null)
            {
                _context.MonitoringDatas.Remove(monitoringData);
                await _context.SaveChangesAsync();
            }
        }
    }
}
