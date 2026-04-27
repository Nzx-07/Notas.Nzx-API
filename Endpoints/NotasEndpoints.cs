using System.Security.Claims;
using NotasNzx.DTOs;
using NotasNzx.Servicios;

namespace NotasNzx.Endpoints;

public static class NotasEndpoints
{
    public static async Task MapearEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/api/notas")
                       .WithTags("Notas")
                       .RequireAuthorization();

        // GET todas las notas del usuario
        grupo.MapGet("/", async (ClaimsPrincipal usuario, INotasServicio servicio) =>
        {
            var usuarioId = ObtenerUsuarioId(usuario);
            var notas = await servicio.ObtenerTodas(usuarioId);
            return Results.Ok(new Respuesta<IEnumerable<NotaRespuesta>>(
                Exito: true,
                Mensaje: "Notas obtenidas correctamente",
                Data: notas
            ));
        })
        .WithSummary("Obtiene todas las notas del usuario");

        // GET una nota por ID
        grupo.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal usuario, INotasServicio servicio) =>
        {
            var usuarioId = ObtenerUsuarioId(usuario);
            var nota = await servicio.ObtenerPorId(id, usuarioId);

            if (nota is null)
                return Results.NotFound(new Respuesta<NotaRespuesta>(
                    Exito: false,
                    Mensaje: "Nota no encontrada",
                    Data: null
                ));

            return Results.Ok(new Respuesta<NotaRespuesta>(
                Exito: true,
                Mensaje: "Nota encontrada",
                Data: nota
            ));
        })
        .WithSummary("Obtiene una nota por ID");

        // POST crear nota
       // POST crear nota
grupo.MapPost("/", async (CrearNotaRequest solicitud, ClaimsPrincipal usuario, INotasServicio servicio) =>
{
    if (string.IsNullOrWhiteSpace(solicitud.Contenido))
        return Results.BadRequest(new Respuesta<NotaRespuesta>(
            Exito: false,
            Mensaje: "El contenido no puede estar vacío",
            Data: null
        ));

    if (solicitud.Contenido.Length > 1000)
        return Results.BadRequest(new Respuesta<NotaRespuesta>(
            Exito: false,
            Mensaje: "El contenido no puede superar los 1000 caracteres",
            Data: null
        ));

    var usuarioId = ObtenerUsuarioId(usuario);
    var (nota, error) = await servicio.Crear(solicitud.Contenido, usuarioId);

    if (error is not null)
        return Results.BadRequest(new Respuesta<NotaRespuesta>(
            Exito: false,
            Mensaje: error,
            Data: null
        ));

    return Results.Created($"/api/notas/{nota!.Id}", new Respuesta<NotaRespuesta>(
        Exito: true,
        Mensaje: "Nota creada correctamente",
        Data: nota
    ));
})
.WithSummary("Crea una nueva nota");


        //PUT actualizar nota
        grupo.MapPut("/{id:guid}", async (Guid id, ActualizarNotaRequest solicitud, ClaimsPrincipal usuario, INotasServicio servicio) => 
        {
            if (string.IsNullOrWhiteSpace(solicitud.Contenido))
                return Results.BadRequest(new Respuesta <NotaRespuesta>(
                    Exito: false,
                    Mensaje: "El contenido no puede estar vacio",
                    Data: null
                ));

            var usuarioId = ObtenerUsuarioId(usuario);
            var nota = await servicio.Actualizar(id, solicitud.Contenido, usuarioId);

            if (nota is null)
                return Results.NotFound(new Respuesta<NotaRespuesta>(
                    Exito: false,
                    Mensaje: "Nota no encontrada",
                    Data: null
                ));

            return Results.Ok(new Respuesta<NotaRespuesta>(
                Exito: true,
                Mensaje: "Nota Actualizada Correctamente",
                Data: nota
            ));
        })
        .WithSummary("Actualizar una nota");



        // DELETE eliminar nota
        grupo.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal usuario, INotasServicio servicio) =>
        {
            var usuarioId = ObtenerUsuarioId(usuario);
            var eliminada = await servicio.Eliminar(id, usuarioId);

            if (!eliminada)
                return Results.NotFound(new Respuesta<object>(
                    Exito: false,
                    Mensaje: "Nota no encontrada",
                    Data: null
                ));

            return Results.Ok(new Respuesta<object>(
                Exito: true,
                Mensaje: "Nota eliminada correctamente",
                Data: null
            ));
        })
        .WithSummary("Elimina una nota por ID");
    }

    private static Guid ObtenerUsuarioId(ClaimsPrincipal usuario)
    {
        var id = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(id!);
    }
}