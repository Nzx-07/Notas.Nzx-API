using Microsoft.EntityFrameworkCore;
using NotasNzx.Datos;
using NotasNzx.Modelos;

namespace NotasNzx.Middleware;

public class ApiKeyMiddleware(RequestDelegate siguiente)
{
    private const int LimiteFreePorDia = 100;

    public async Task InvokeAsync(HttpContext contexto, AppDbContext db)
    {
        // Solo aplica a /api/notas
        if (!contexto.Request.Path.StartsWithSegments("/api/notas"))
        {
            await siguiente(contexto);
            return;
        }

        // Verificar que venga el header X-Api-Key
        if (!contexto.Request.Headers.TryGetValue("X-Api-Key", out var apiKey))
        {
            contexto.Response.StatusCode = 401;
            await contexto.Response.WriteAsJsonAsync(new
            {
                exito = false,
                mensaje = "Se requiere API Key. Incluye el header X-Api-Key"
            });
            return;
        }

        // Buscar usuario por API Key
        var usuario = await db.Usuarios
            .FirstOrDefaultAsync(u => u.ApiKey == apiKey.ToString());

        if (usuario is null)
        {
            contexto.Response.StatusCode = 401;
            await contexto.Response.WriteAsJsonAsync(new
            {
                exito = false,
                mensaje = "API Key inválida"
            });
            return;
        }

        // Resetear contador si es un nuevo día
        if (usuario.UltimoReset.Date < DateTime.UtcNow.Date)
        {
            usuario.RequestsHoy = 0;
            usuario.UltimoReset = DateTime.UtcNow.Date;
        }

        // Verificar límite para plan Free
        if (usuario.Plan == Plan.Free && usuario.RequestsHoy >= LimiteFreePorDia)
        {
            contexto.Response.StatusCode = 429;
            await contexto.Response.WriteAsJsonAsync(new
            {
                exito = false,
                mensaje = $"Límite de {LimiteFreePorDia} requests/día alcanzado. Actualiza a Pro para requests ilimitados."
            });
            return;
        }

        // Incrementar contador
        usuario.RequestsHoy++;
        await db.SaveChangesAsync();

        // Pasar el usuario al contexto para usarlo en los endpoints
        contexto.Items["Usuario"] = usuario;

        await siguiente(contexto);
    }
}