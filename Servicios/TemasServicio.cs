using Microsoft.EntityFrameworkCore;
using NotasNzx.Datos;
using NotasNzx.DTOs;
using NotasNzx.Modelos;

namespace NotasNzx.Servicios;

public interface ITemasServicio
{
    Task<IEnumerable<TemaRespuesta>> ObtenerTemas(Guid usuarioId);
    Task<(bool Exito, string? Error)> ActivarTema(string temaId, Guid usuarioId);
    Task<string> ObtenerTemaActivo(Guid usuarioId);
}

public class TemasServicio(AppDbContext db) : ITemasServicio
{
    // Temas disponibles — Free y Pro
    private static readonly List<(string Id, string Nombre, string Descripcion, bool EsPro)> TemasDisponibles =
    [
        ("blanco",    "Blanco",    "Limpio y luminoso, perfecto para el día",          false),
        ("noche",     "Noche",     "Oscuro y elegante, ideal para trabajar de noche",   false),
        ("bosque",    "Bosque",    "Tonos verdes naturales, relajante para la vista",   false),
        ("aurora",    "Aurora",    "Morados y azules nocturnos",                        true),
        ("desierto",  "Desierto",  "Tonos arena y naranja cálido",                      true),
        ("oceano",    "Océano",    "Azules profundos y turquesa",                       true),
        ("volcan",    "Volcán",    "Rojos y naranjas intensos",                         true),
        ("niebla",    "Niebla",    "Grises suaves y blancos",                           true),
        ("cafe",      "Café",      "Marrones cálidos tipo sepia",                       true),
        ("sakura",    "Sakura",    "Rosas suaves japoneses",                            true),
        ("obsidiana", "Obsidiana", "Negro puro con acentos morados leves",              true),
        ("menta",     "Menta",     "Verdes claros y frescos",                           true),
        ("lavanda",   "Lavanda",   "Púrpuras suaves y relajantes",                      true),
    ];

    public async Task<IEnumerable<TemaRespuesta>> ObtenerTemas(Guid usuarioId)
    {
        var usuario = await db.Usuarios.FindAsync(usuarioId);
        if (usuario is null) return [];

        return TemasDisponibles.Select(t => new TemaRespuesta(
            Id: t.Id,
            Nombre: t.Nombre,
            Descripcion: t.Descripcion,
            EsPro: t.EsPro,
            EstaActivo: usuario.TemaActivo == t.Id
        ));
    }

    public async Task<(bool Exito, string? Error)> ActivarTema(string temaId, Guid usuarioId)
    {
        var usuario = await db.Usuarios.FindAsync(usuarioId);
        if (usuario is null) return (false, "Usuario no encontrado");

        var tema = TemasDisponibles.FirstOrDefault(t => t.Id == temaId);
        if (tema == default) return (false, "Tema no encontrado");

        // Verificar que el usuario Pro puede acceder a temas Pro
        if (tema.EsPro && usuario.Plan == Plan.Free)
            return (false, "Este tema es exclusivo del plan Pro. Actualiza tu plan para acceder.");

        usuario.TemaActivo = temaId;
        await db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<string> ObtenerTemaActivo(Guid usuarioId)
    {
        var usuario = await db.Usuarios.FindAsync(usuarioId);
        return usuario?.TemaActivo ?? "blanco";
    }
}