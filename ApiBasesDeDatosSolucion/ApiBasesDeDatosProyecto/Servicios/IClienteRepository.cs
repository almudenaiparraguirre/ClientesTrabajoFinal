﻿namespace ApiBasesDeDatosProyecto.Servicios
{
    public interface IClienteRepository
    {
        Task AddClienteAsync(Cliente cliente);

        void Agregar(Cliente cliente);
        void Actualizar(Cliente cliente);
        void Eliminar(Cliente cliente);
        Task<bool> GuardarCambios();
        Task<Cliente?> ObtenerPorId(int id);
        Task<Cliente?> ObtenerPorEmail(string email);
        Task EditClienteAsync(Cliente cliente);
        Task EliminarClienteAsync(Cliente cliente);

        Task<Cliente?> ObtenerClientesPais(int id);
        Task<List<Cliente>> ObtenerClientesPorPaisId(int paisId);

        Task<List<Cliente>> ObtenerTodos();
    }
}
