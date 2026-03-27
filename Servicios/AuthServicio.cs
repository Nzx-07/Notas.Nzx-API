using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotasNzx.Datos;
using NotasNzx.DTOs;
using NotasNzx.Modelos;
using BC = BCrypt.Net.BCrypt;

namespace NotasNzx.Servicios;

public interface IAuthServicio
{
    Task<Usuario?> Registrar(string email, string contraseña);
    Task<string?> Login(string email, string contraseña);
}

public class AuthServicio(AppDbContext db, IConfiguration config) : IAuthServicio
{
    public async Task<Usuario?> Registrar(string email, string contraseña)
    {
        var existe = await db.Usuarios.AnyAsync(u => u.Email == email);
        if (existe) return null;

        var usuario = new Usuario
        {
            Email = email.ToLower().Trim(),
            HashContraseña = BC.HashPassword(contraseña)
        };

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
        return usuario;
    }

    public async Task<string?> Login(string email, string contraseña)
    {
        var usuario = await db.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());

        if (usuario is null) return null;
        if (!BC.Verify(contraseña, usuario.HashContraseña)) return null;

        return GenerarToken(usuario);
    }

    private string GenerarToken(Usuario usuario)
    {
        var clave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:Clave"]!));

        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email)
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Emisor"],
            audience: config["Jwt:Audiencia"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}