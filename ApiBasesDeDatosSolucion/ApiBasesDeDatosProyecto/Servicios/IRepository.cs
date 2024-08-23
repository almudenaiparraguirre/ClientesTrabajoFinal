using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRepository<T> where T : class
{
    Task<T?> ObtenerPorIdAsync(int id);
    Task<List<T>> ObtenerTodosAsync();
    Task AgregarAsync(T entity);
    Task ActualizarAsync(T entity);
    Task EliminarAsync(T entity);
    Task<bool> GuardarCambiosAsync();
}
