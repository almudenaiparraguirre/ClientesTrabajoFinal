using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> ObtenerPorIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<List<T>> ObtenerTodosAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AgregarAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await GuardarCambiosAsync();
    }

    public async Task ActualizarAsync(T entity)
    {
        _dbSet.Update(entity);
        await GuardarCambiosAsync();
    }

    public async Task EliminarAsync(T entity)
    {
        _dbSet.Remove(entity);
        await GuardarCambiosAsync();
    }

    public async Task<bool> GuardarCambiosAsync()
    {
        try
        {
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return false;
        }
    }
}
