namespace RegistroInventario.Services;

public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje){}
}
public record SesionUsuario(int Id, string NombreUsuario, String NombreCompleto, string Rol);
public record ResumenCategoria(string Categoria, int Cntida);