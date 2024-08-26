namespace ApiBasesDeDatosProyecto.Repository;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    private readonly Contexto _contexto;

    public UsuarioRepository(Contexto contexto): base (contexto)
    {
        _contexto = contexto;
    }
}