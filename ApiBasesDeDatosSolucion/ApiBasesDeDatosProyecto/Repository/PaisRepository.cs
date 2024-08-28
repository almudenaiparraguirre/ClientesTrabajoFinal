namespace ApiBasesDeDatosProyecto.Repository;

public class PaisRepository : Repository<Pais>, IPaisRepository
{
    private readonly Contexto contexto;

    public PaisRepository(Contexto contexto): base(contexto)
    {
        this.contexto = contexto;
    }

    public async Task<Pais?> ObtenerPorNombre(string nombre)
    {
        return await contexto.Paises
            .Where(p => p.Nombre == nombre)
            .FirstOrDefaultAsync();
    }
}