using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using NotasNzx.Datos;
using NotasNzx.Endpoints;
using NotasNzx.Middleware;
using NotasNzx.Servicios;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("Frontend", politica =>
    {
        politica
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Base de datos SQLite
builder.Services.AddDbContext<AppDbContext>(opciones =>
    opciones.UseSqlite(builder.Configuration.GetConnectionString("BaseDatos")));

// Servicios
builder.Services.AddScoped<INotasServicio, NotasServicio>();
builder.Services.AddScoped<IAuthServicio, AuthServicio>();

// JWT
var claveJwt = builder.Configuration["Jwt:Clave"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Emisor"],
            ValidAudience = builder.Configuration["Jwt:Audiencia"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveJwt))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

// Aplicar migraciones automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapOpenApi();
app.MapScalarApiReference(opciones =>
{
    opciones.Title = "Notas.Nzx";
    opciones.AddHttpAuthentication("Bearer", bearer =>
    {
        bearer.Token = "tu-token-jwt-aquí";
    });
});

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

// Middleware de API Key (solo para /api/notas)
app.UseMiddleware<ApiKeyMiddleware>();

// Endpoints
app.MapearAuthEndpoints();
app.MapearEndpoints();
app.MapearPerfilEndpoints();

app.Run();