using GoldCrew.Data;
using Microsoft.EntityFrameworkCore;
namespace GoldCrew.Services;

public interface IAuthService
{
    Task<SesionUsuario?> LoginAsync(string usuario, string clave);
}
public class AuthService : IAuthService
{
    private readonly IDbContextFactory<GoldCrewDbContext> _factory;
    public AuthService(IDbContextFactory<GoldCrewDbContext> factory) => _factory = factory;

    public async Task<SesionUsuario?> LoginAsync(string usuario, string clave)
    {
        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrEmpty(clave)) return null;
        await using var db = await _factory.CreateDbContextAsync();
        var u = await db.Usuarios.AsNoTracking()
            .FirstOrDefaultAsync(x => x.NombreUsuario == usuario.Trim() && x.ACtivo);
        if (u is null || !BCrypt.Net.BCrypt.Verify(clave, u.ClaveHash)) return null;
        return new SesionUsuario(u.Id, u.NombreUsuario, u.NombreCompleto, u.Rol.ToString());
    }
}