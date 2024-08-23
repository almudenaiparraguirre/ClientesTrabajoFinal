using ApiBasesDeDatosProyecto.Context;
using ApiBasesDeDatosProyecto.Entities;
using ApiBasesDeDatosProyecto.Servicios;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiBasesDeDatosProyecto.Repository
{
    public class VistaClientesPaisesRepository : IVistaClientesPaisesRepository
    {
        private readonly Contexto contexto;

        public VistaClientesPaisesRepository(Contexto contexto)
        {
            this.contexto = contexto;
        }

        public async Task<List<VistaClientesPaises>> ObtenerTodos()
        {
            return await contexto.VistaClientesPaises.ToListAsync();
        }

        public async Task<VistaClientesPaises?> ObtenerPorClienteId(int clienteId)
        {
            return await contexto.VistaClientesPaises
                .FirstOrDefaultAsync(v => v.ClienteId == clienteId);
        }
    }
}
