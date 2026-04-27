using Microsoft.EntityFrameworkCore;
using NotasNzx.Datos;
using NotasNzx.DTOs;
using NotasNzx.Modelos;

namespace NotasNzx.Servicios;

public interface INotasServicio
{
    Task<IEnumerable<NotaRespuesta>> ObtenerTodas(Guid usuarioId);
    Task<NotaRespuesta?> ObtenerPorId(Guid id, Guid usuarioId);
    Task<(NotaRespuesta? Nota, string? Error)> Crear(string contenido, Guid usuarioId);
    Task<NotaRespuesta?> Actualizar(Guid id, string contenido, Guid usuarioId);
    Task<bool> Eliminar(Guid id, Guid usuarioId);
    Task<int> ContarNotas(Guid usuarioId);
}

public class NotasServicio(AppDbContext db) : INotasServicio
{
    private const int LimiteNotasFree = 7;

    public async Task<IEnumerable<NotaRespuesta>> ObtenerTodas(Guid usuarioId)
        => await db.Notas
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.CreadoEn)
            .Select(n => new NotaRespuesta(n.Id, n.Contenido, n.CreadoEn, n.CarpetaId))
            .ToListAsync();

    public async Task<NotaRespuesta?> ObtenerPorId(Guid id, Guid usuarioId)
    {
        var nota = await db.Notas
            .FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == usuarioId);

        return nota is null ? null : new NotaRespuesta(nota.Id, nota.Contenido, nota.CreadoEn, nota.CarpetaId);
    }

    public async Task<(NotaRespuesta? Nota, string? Error)> Crear(string contenido, Guid usuarioId)
    {
        // Verificar límite Free
        var usuario = await db.Usuarios.FindAsync(usuarioId);
        if (usuario is null) return (null, "Usuario no encontrado");

        if (usuario.Plan == Plan.Free)
        {
            var totalNotas = await db.Notas.CountAsync(n => n.UsuarioId == usuarioId);
            if (totalNotas >= LimiteNotasFree)
                return (null, $"Has alcanzado el límite de {LimiteNotasFree} notas del plan Free. Actualiza a Pro para crear notas ilimitadas.");
        }

        var nota = new Nota
        {
            Contenido = contenido.Trim(),
            UsuarioId = usuarioId
        };

        db.Notas.Add(nota);
        await db.SaveChangesAsync();
        return (new NotaRespuesta(nota.Id, nota.Contenido, nota.CreadoEn, nota.CarpetaId), null);
    }

    public async Task<bool> Eliminar(Guid id, Guid usuarioId)
    {
        var nota = await db.Notas
            .FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == usuarioId);

        if (nota is null) return false;

        db.Notas.Remove(nota);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<NotaRespuesta?> Actualizar(Guid id, string contenido, Guid usuarioId)
    {
        var nota = await db.Notas
            .FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == usuarioId);

        if (nota is null) return null;

        nota.Contenido = contenido.Trim();
        await db.SaveChangesAsync();
        return new NotaRespuesta(nota.Id, nota.Contenido, nota.CreadoEn, nota.CarpetaId);
    }

    public async Task<int> ContarNotas(Guid usuarioId)
        => await db.Notas.CountAsync(n => n.UsuarioId == usuarioId);
}