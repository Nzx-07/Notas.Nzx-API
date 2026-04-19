using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NotasNzx.Modelos;

namespace NotasNzx.Datos;

public class AppDbContext(DbContextOptions<AppDbContext> opciones) : DbContext(opciones)
{
    public DbSet<Nota> Notas => Set<Nota>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Carpeta> Carpetas => Set<Carpeta>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelo)
    {
        modelo.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelo.Entity<Usuario>()
            .Property(u => u.Id)
            .HasColumnType("uuid");

        modelo.Entity<Usuario>()
            .Property(u => u.CreadoEn)
            .HasColumnType("timestamp with time zone");

        modelo.Entity<Usuario>()
            .Property(u => u.UltimoReset)
            .HasColumnType("timestamp with time zone");

        modelo.Entity<Nota>()
            .Property(n => n.Id)
            .HasColumnType("uuid");

        modelo.Entity<Nota>()
            .Property(n => n.UsuarioId)
            .HasColumnType("uuid");

        modelo.Entity<Nota>()
            .Property(n => n.CarpetaId)
            .HasColumnType("uuid");

        modelo.Entity<Nota>()
            .Property(n => n.CreadoEn)
            .HasColumnType("timestamp with time zone");

        modelo.Entity<Nota>()
            .HasOne(n => n.Usuario)
            .WithMany(u => u.Notas)
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelo.Entity<Nota>()
            .HasOne(n => n.Carpeta)
            .WithMany()
            .HasForeignKey(n => n.CarpetaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelo.Entity<Carpeta>()
            .Property(c => c.Id)
            .HasColumnType("uuid");

        modelo.Entity<Carpeta>()
            .Property(c => c.UsuarioId)
            .HasColumnType("uuid");

        modelo.Entity<Carpeta>()
            .Property(c => c.CreadoEn)
            .HasColumnType("timestamp with time zone");

        modelo.Entity<Carpeta>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Carpetas)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}