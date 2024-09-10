using ApiBasesDeDatosProyecto.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApiBasesDeDatosProyecto.Context;
using System.Net.Http;

namespace ApiBasesDeDatosProyecto.Repository
{
    public class AccessMonitoringDataRepository : IAccessMonitoringDataRepository
    {
        private readonly Contexto _context;
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;
        private static readonly HttpClient _httpClient = new HttpClient();

        public AccessMonitoringDataRepository(IClienteRepository clienteRepository, Contexto context, IMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccessMonitoringData>> GetAllAsync()
        {
            return await _context.AccessMonitoringDatas.ToListAsync();

        }

        public async Task<AccessMonitoringData> GetLastAsync()
        {
            var cliente = await _context.AccessMonitoringDatas
                .OrderByDescending(c => c.FechaRecibido) // Ordenar por Id en orden descendente
                .FirstOrDefaultAsync(); // Toma el primer registro

            if (cliente == null)
                return null;

            var monitorCliente = _mapper.Map<AccessMonitoringData>(cliente);
            return monitorCliente;
        }

        public async Task AddAsync(AccessMonitoringData accmonitoringData)
        {
            _context.AccessMonitoringDatas.Add(accmonitoringData);
            await _context.SaveChangesAsync();
            await NotifyChangesAsync(); // Llama a Notify después de la operación
        }

        public async Task<AccessMonitoringData> AddCliente(AccessMonitoringData model)
        {
            // Crear una instancia del modelo Cliente
            var cliente = _mapper.Map<Cliente>(model);

            // Llamar al método de repositorio para añadir el cliente a la base de datos
            await _clienteRepository.AddClienteAsync(cliente);
            return model;
        }

        private async Task NotifyChangesAsync()
        {
            // Asume que la API está alojada localmente; ajusta la URL según sea necesario
            var notifyUrl = "https://backclientes-f4fjfwece9gta3c2.spaincentral-01.azurewebsites.net/api/clienteMonitoring/notify";
            var response = await _httpClient.PostAsync(notifyUrl, null)

            if (!response.IsSuccessStatusCode)
            {
                // Manejo de errores (log, excepciones, etc.)
                Console.WriteLine("Error notifying changes.");
            }
        }
    }
}
