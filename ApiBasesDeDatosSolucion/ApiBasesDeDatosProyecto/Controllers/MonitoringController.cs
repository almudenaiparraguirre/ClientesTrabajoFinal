using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ApiBasesDeDatosProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<MonitoringData>> GetMonitoringData()
        {
            var random = new Random();
            var data = new List<MonitoringData>();

            string[] paises = { "México", "Estados Unidos", "Canadá", "Brasil", "Argentina" };
            string[] clientes = { "Cliente A", "Cliente B", "Cliente C", "Cliente D", "Cliente E" };

            // Genera un número aleatorio entre 10 y 20 para el total de transferencias
            int numberOfTransfers = random.Next(10, 21);

            for (int i = 0; i < numberOfTransfers; i++)
            {
                data.Add(new MonitoringData
                {
                    Id = i,
                    Name = $"Transacción {i + 1}",
                    PaisOrigen = paises[random.Next(paises.Length)],
                    PaisDestino = paises[random.Next(paises.Length)],
                    ClienteOrigen = clientes[random.Next(clientes.Length)],
                    ClienteDestino = clientes[random.Next(clientes.Length)],
                    Value = random.NextDouble() * 10000,  // Asumiendo que el valor es una cantidad monetaria o métrica similar
                    Timestamp = DateTime.Now.AddSeconds(-random.Next(0, 60))
                });
            }

            return Ok(data);
        }
    }
}
