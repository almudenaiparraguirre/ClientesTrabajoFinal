﻿namespace ApiBasesDeDatosProyecto.Servicios
{
    public interface IMonitoringDataRepository
    {
        Task<IEnumerable<MonitoringData>> GetAllAsync();
        Task<MonitoringData> GetByIdAsync(int id);
        Task<MonitoringData> GetLastAsync();
        Task AddAsync(MonitoringData monitoringData);
        Task UpdateAsync(MonitoringData monitoringData);
        Task DeleteAsync(int id);
    }
}
