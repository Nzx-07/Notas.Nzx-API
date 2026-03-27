using Microsoft.EntityFrameworkCore;
using NotasNzx.Modelos;

namespace NotasNzx.Datos;

public class AppDbContext(DbContextOptions<AppDbContext> opciones) : DbContext(opciones)
{
    public DbSet<Nota> Notas => Set<Nota>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelo)
    {
        // Email único por usuario
        modelo.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Un usuario tiene muchas notas
        modelo.Entity<Nota>()
            .HasOne(n => n.Usuario)
            .WithMany(u => u.Notas)
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}