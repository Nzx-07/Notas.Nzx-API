using System.Security.Claims;
using NotasNzx.DTOs;
using NotasNzx.Servicios;

namespace NotasNzx.Endpoints;

public static class TemasEndpoints
{
    public static void MapearTemasEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/api/temas")
                       .WithTags("Temas")
                       .RequireAuthorization();

        // GET todos los temas con estado activo
        grupo.MapGet("/", async (ClaimsPrincipal usuario, ITemasServicio servicio) =>
        {
            var usuarioId = ObtenerUsuarioId(usuario);
            var temas = await servicio.ObtenerTemas(usuarioId);
            return Results.Ok(new Respuesta<IEnumerable<TemaRespuesta>>(
                Exito: true,
                Mensaje: "Temas obtenidos correctamente",
                Data: temas
            ));
        }).WithSummary("Obtiene todos los temas disponibles");

        // PUT activar tema
        grupo.MapPut("/activar", async (ActivarTemaRequest solicitud, ClaimsPrincipal usuario, ITemasServicio servicio) =>
        {
            var usuarioId = ObtenerUsuarioId(usuario);
            var (exito, error) = await servicio.ActivarTema(solicitud.TemaId, usuarioId);

            if (!exito)
                return Results.BadRequest(new Respuesta<object>(
                    Exito: false,
                    Mensaje: error!,
                    Data: null
                ));

            return Results.Ok(new Respuesta<object>(
                Exito: true,
                Mensaje: "Tema activado correctamente",
                Data: null
            ));
        }).WithSummary("Activa un tema");

        // GET tema activo del usuario
        grupo.MapGet("/activo", async (ClaimsPrincipal usuario, ITemasServicio servicio) =>
        {
            var usuarioId = ObtenerUsuarioId(usuario);
            var tema = await servicio.ObtenerTemaActivo(usuarioId);
            return Results.Ok(new Respuesta<string>(
                Exito: true,
                Mensaje: "Tema activo obtenido",
                Data: tema
            ));
        }).WithSummary("Obtiene el tema activo del usuario");
    }

    private static Guid ObtenerUsuarioId(ClaimsPrincipal usuario)
    {
        var id = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(id!);
    }
}