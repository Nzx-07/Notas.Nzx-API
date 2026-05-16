```csharp id="g7ww5x"
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

builder.Services.AddControllers();

// CORS
builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("Frontend", politica =>
    {
        politica
            .WithOrigins(
                "http://localhost:5173",
                "https://notas-nzx-web.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Base de datos
// ...

// Servicios
// ...

// JWT
// ...

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

// Middleware de API Key
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.MapOpenApi();

app.MapScalarApiReference(opciones =>
{
    opciones.Title = "Notas.Nzx";
    opciones.AddHttpAuthentication("Bearer", bearer =>
    {
        bearer.Token = "tu-token-jwt-aquí";
    });
});

// Endpoints
app.MapearAuthEndpoints();
app.MapearPerfilEndpoints();
app.MapearCarpetasEndpoints();
app.MapearTemasEndpoints();

app.Run();