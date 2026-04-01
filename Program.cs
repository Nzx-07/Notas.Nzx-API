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
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Base de datos — PostgreSQL en producción, SQLite en local
if (builder.Environment.IsProduction())
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var npgsqlConnection = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
        builder.Services.AddDbContext<AppDbContext>(opciones =>
            opciones.UseNpgsql(npgsqlConnection));
    }
}
else
{
    builder.Services.AddDbContext<AppDbContext>(opciones =>
        opciones.UseSqlite(builder.Configuration.GetConnectionString("BaseDatos")));
}

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

// Migraciones automáticas solo en producción
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
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

// Middleware de API Key
app.UseMiddleware<ApiKeyMiddleware>();

// Endpoints
app.MapearAuthEndpoints();
app.MapearEndpoints();
app.MapearPerfilEndpoints();

app.Run();