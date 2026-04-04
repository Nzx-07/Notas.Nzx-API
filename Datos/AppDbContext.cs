using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NotasNzx.Modelos;

namespace NotasNzx.Datos;

public class AppDbContext(DbContextOptions<AppDbContext> opciones) : DbContext(opciones)
{
    public DbSet<Nota> Notas => Set<Nota>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelo)
    {
        // Email único por usuario
        modelo.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Configurar Guid como uuid en PostgreSQL
        modelo.Entity<Usuario>()
            .Property(u => u.Id)
            .HasColumnType("uuid");

        modelo.Entity<Nota>()
            .Property(n => n.Id)
            .HasColumnType("uuid");

        modelo.Entity<Nota>()
            .Property(n => n.UsuarioId)
            .HasColumnType("uuid");

        // Un usuario tiene muchas notas
        modelo.Entity<Nota>()
            .HasOne(n => n.Usuario)
            .WithMany(u => u.Notas)
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}