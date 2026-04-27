using System.Security.Claims;
using NotasNzx.DTOs;
using NotasNzx.Servicios;

namespace NotasNzx.Endpoints;

public static class CarpetasEndpoints
{
    public static void MapearCarpetasEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/api/carpetas")
                       .WithTags("Carpetas")
                       .RequireAuthorization();

        // GET todas las carpetas
        grupo.MapGet("/", async (ClaimsPrincipal usuario, ICarpetasServicio servicio) =>
        {
            var usuarioId = ObtenerUsuarioId(usuario);
            var carpetas = await servicio.ObtenerTodas(usuarioId);
            return Results.Ok(new Respuesta<IEnumerable<CarpetaRespuesta>>(
                Exito: true, Mensaje: "Carpetas obtenidas", Data: carpetas));
        }).WithSummary("Obtiene todas las carpetas");

        // POST crear carpeta
grupo.MapPost("/", async (CrearCarpetaRequest solicitud, ClaimsPrincipal usuario, ICarpetasServicio servicio) =>
{
    if (string.IsNullOrWhiteSpace(solicitud.Nombre))
        return Results.BadRequest(new Respuesta<CarpetaRespuesta>(
            Exito: false, Mensaje: "El nombre no puede estar vacío", Data: null));

    var usuarioId = ObtenerUsuarioId(usuario);
    var (carpeta, error) = await servicio.Crear(solicitud.Nombre, usuarioId);

    if (error is not null)
        return Results.BadRequest(new Respuesta<CarpetaRespuesta>(
            Exito: false, Mensaje: error, Data: null));

    return Results.Created($"/api/carpetas/{carpeta!.Id}", new Respuesta<CarpetaRespuesta>(
        Exito: true, Mensaje: "Carpeta creada", Data: carpeta));
}).WithSummary("Crea una carpeta");

        // PUT mover nota a carpeta
        grupo.MapPut("/mover-nota/{notaId:guid}", async (Guid notaId, MoverNotaRequest solicitud, ClaimsPrincipal usuario, ICarpetasServicio servicio) =>
        {
            var usuarioId = ObtenerUsuarioId(usuario);
            var movida = await servicio.MoverNota(notaId, solicitud.CarpetaId, usuarioId);
            if (!movida)
                return Results.NotFound(new Respuesta<object>(
                    Exito: false, Mensaje: "Nota no encontrada", Data: null));
            return Results.Ok(new Respuesta<object>(
                Exito: true, Mensaje: "Nota movida correctamente", Data: null));
        }).WithSummary("Mueve una nota a una carpeta");
    }

    private static Guid ObtenerUsuarioId(ClaimsPrincipal usuario)
    {
        var id = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(id!);
    }
}