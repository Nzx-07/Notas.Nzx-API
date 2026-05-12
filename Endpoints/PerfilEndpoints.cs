using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using NotasNzx.Datos;
using NotasNzx.DTOs;
using NotasNzx.Modelos;

namespace NotasNzx.Endpoints;

public static class PerfilEndpoints
{
    public static void MapearPerfilEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/api/perfil")
                       .WithTags("Perfil")
                       .RequireAuthorization();

        // GET /api/perfil
        grupo.MapGet("/", async (ClaimsPrincipal claims, AppDbContext db) =>
        {
            var usuarioId = Guid.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var usuario = await db.Usuarios.FindAsync(usuarioId);

            if (usuario is null)
                return Results.NotFound();

            var limite = usuario.Plan == Plan.Free ? 100 : -1;

            return Results.Ok(new Respuesta<PerfilRespuesta>(
                Exito: true,
                Mensaje: "Perfil obtenido correctamente",
                Data: new PerfilRespuesta(
                    Email: usuario.Email,
                    Plan: usuario.Plan.ToString(),
                    ApiKey: usuario.ApiKey,
                    RequestsHoy: usuario.RequestsHoy,
                    LimiteRequests: limite,
                    TemaActivo: usuario.TemaActivo
                )
            ));
        })
        .WithSummary("Obtiene tu perfil, plan y API Key");

        // POST /api/perfil/upgrade
        grupo.MapPost("/upgrade", async (ClaimsPrincipal claims, AppDbContext db) =>
        {
            var usuarioId = Guid.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var usuario = await db.Usuarios.FindAsync(usuarioId);

            if (usuario is null)
                return Results.NotFound();

            if (usuario.Plan == Plan.Pro)
                return Results.BadRequest(new Respuesta<object>(
                    Exito: false,
                    Mensaje: "Ya tienes el plan Pro",
                    Data: null
                ));

            usuario.Plan = Plan.Pro;
            await db.SaveChangesAsync();

            return Results.Ok(new Respuesta<object>(
                Exito: true,
                Mensaje: "Plan actualizado a Pro correctamente",
                Data: null
            ));
        })
        .WithSummary("Actualiza tu plan a Pro");
    }
    
}