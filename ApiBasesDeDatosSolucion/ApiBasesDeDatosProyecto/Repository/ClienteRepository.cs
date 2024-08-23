﻿using ApiBasesDeDatosProyecto.Context;
using Microsoft.Data.SqlClient;

namespace ApiBasesDeDatosProyecto.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly Contexto contexto;

        public ClienteRepository(Contexto contexto)
        {
            this.contexto = contexto;
        }
        public void Actualizar(Cliente cliente)
        {
            contexto.Clientes.Update(cliente);
        }
        public async Task AddClienteAsync(Cliente cliente)
        {
            contexto.Clientes.Add(cliente);
            await contexto.SaveChangesAsync();
        }

        public void Agregar(Cliente cliente)
        {
            contexto.Clientes.Add(cliente);
        }

        public void Eliminar(Cliente cliente)
        {

            contexto.Clientes.Remove(cliente);
        }

        public async Task<bool> GuardarCambios()
        {
            bool result = false;
            try
            {
                result = await contexto.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }

        public async Task<Cliente?> ObtenerPorId(int id)
        {
            return await contexto.Clientes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Cliente?> ObtenerClientesPais(int id)
        {
            return await contexto.Clientes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Cliente>> ObtenerTodos()
        {
            return await contexto.Clientes.ToListAsync();
        }

        public async Task<Cliente?> ObtenerPorEmail(string email)
        {
            return await contexto.Clientes.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task EditClienteAsync(Cliente cliente)
        {
            contexto.Clientes.Update(cliente);
            await contexto.SaveChangesAsync();
        }

        public async Task EliminarClienteAsync(Cliente cliente)
        {
            contexto.Clientes.Remove(cliente);
            await contexto.SaveChangesAsync();
        }

        public async Task<List<ProAlmClientePorPaisDto>> ObtenerClientesPorPaisAsync(int paisId)
        {
            var paisIdParam = new SqlParameter("@PaisId", paisId);
             var clientes = await contexto.ProAlmClientePorPaisDtos
                .FromSqlRaw("EXEC GetClientesPorPais @PaisId", paisIdParam)
                .ToListAsync();

            

            return clientes;
        }

    }
}
