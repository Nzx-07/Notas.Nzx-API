using Microsoft.EntityFrameworkCore;
using NotasNzx.Datos;
using NotasNzx.DTOs;
using NotasNzx.Modelos;

namespace NotasNzx.Servicios;

public interface ICarpetasServicio
{
    Task<IEnumerable<CarpetaRespuesta>> ObtenerTodas(Guid usuarioId);
    Task<(CarpetaRespuesta? Carpeta, string? Error)> Crear(string nombre, Guid usuarioId);
    Task<CarpetaRespuesta?> Actualizar(Guid id, string nombre, Guid usuarioId);
    Task<bool> Eliminar(Guid id, Guid usuarioId);
    Task<bool> MoverNota(Guid notaId, Guid? carpetaId, Guid usuarioId);
    Task<int> ContarCarpetas(Guid usuarioId);
}

public class CarpetasServicio(AppDbContext db) : ICarpetasServicio
{
    private const int LimiteCarpetasFree = 2;

    public async Task<IEnumerable<CarpetaRespuesta>> ObtenerTodas(Guid usuarioId)
        => await db.Carpetas
            .Where(c => c.UsuarioId == usuarioId)
            .OrderBy(c => c.CreadoEn)
            .Select(c => new CarpetaRespuesta(c.Id, c.Nombre, c.CreadoEn))
            .ToListAsync();

    public async Task<(CarpetaRespuesta? Carpeta, string? Error)> Crear(string nombre, Guid usuarioId)
    {
        // Verificar límite Free
        var usuario = await db.Usuarios.FindAsync(usuarioId);
        if (usuario is null) return (null, "Usuario no encontrado");

        if (usuario.Plan == Plan.Free)
        {
            var totalCarpetas = await db.Carpetas.CountAsync(c => c.UsuarioId == usuarioId);
            if (totalCarpetas >= LimiteCarpetasFree)
                return (null, $"Has alcanzado el límite de {LimiteCarpetasFree} carpetas del plan Free. Actualiza a Pro para crear carpetas ilimitadas.");
        }

        var carpeta = new Carpeta
        {
            Nombre = nombre.Trim(),
            UsuarioId = usuarioId
        };
        db.Carpetas.Add(carpeta);
        await db.SaveChangesAsync();
        return (new CarpetaRespuesta(carpeta.Id, carpeta.Nombre, carpeta.CreadoEn), null);
    }

    public async Task<CarpetaRespuesta?> Actualizar(Guid id, string nombre, Guid usuarioId)
    {
        var carpeta = await db.Carpetas
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
        if (carpeta is null) return null;
        carpeta.Nombre = nombre.Trim();
        await db.SaveChangesAsync();
        return new CarpetaRespuesta(carpeta.Id, carpeta.Nombre, carpeta.CreadoEn);
    }

    public async Task<bool> Eliminar(Guid id, Guid usuarioId)
    {
        var carpeta = await db.Carpetas
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
        if (carpeta is null) return false;
        db.Carpetas.Remove(carpeta);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MoverNota(Guid notaId, Guid? carpetaId, Guid usuarioId)
    {
        var nota = await db.Notas
            .FirstOrDefaultAsync(n => n.Id == notaId && n.UsuarioId == usuarioId);
        if (nota is null) return false;
        nota.CarpetaId = carpetaId;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<int> ContarCarpetas(Guid usuarioId)
        => await db.Carpetas.CountAsync(c => c.UsuarioId == usuarioId);
}