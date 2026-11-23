using System.Security.Claims;
using GoldCrew.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
namespace GoldCrew.Web.Security;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _storage; 
    private static readonly ClaimsPrincipal Anonimo = new(new ClaimsIdentity());
    public CustomAuthStateProvider(ProtectedSessionStorage storage) => _storage = storage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var r = await _storage.GetAsync<SesionUsuario>("sesion");

            if (r.Success && r.Value is not null)

                return new AuthenticationState(Crear(r.Value));
        }
        catch {  }
        return new AuthenticationState(Anonimo);
    }

    public async Task IniciarSesionAsync(SesionUsuario s)
    {
        await _storage.SetAsync("sesion", s);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Crear(s))));
    }



    public async Task CerrarSesionAsync()

    {
        await _storage.DeleteAsync("sesion");

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonimo)));
    }

    private static ClaimsPrincipal Crear(SesionUsuario s) => new(new ClaimsIdentity(new[]

    {
        new Claim(ClaimTypes.NameIdentifier, s.Id.ToString()),
        new Claim(ClaimTypes.Name, s.NombreUsuario),
        new Claim("NombreCompleto", s.NombreCompleto),
        new Claim(ClaimTypes.Role, s.Rol),
    }, "GoldCrewAuth"));
}