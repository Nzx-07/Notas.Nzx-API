using NotasNzx.DTOs;
using NotasNzx.Servicios;

namespace NotasNzx.Endpoints;

public static class AuthEndpoints
{
    public static void MapearAuthEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/api/auth").WithTags("Autenticación");

        grupo.MapPost("/registrar", async (RegistrarRequest solicitud, IAuthServicio servicio) =>
        {
            if (string.IsNullOrWhiteSpace(solicitud.Email) ||
                string.IsNullOrWhiteSpace(solicitud.Contraseña))
                return Results.BadRequest(new Respuesta<object>(
                    Exito: false,
                    Mensaje: "Email y contraseña son requeridos",
                    Data: null
                ));

            if (solicitud.Contraseña.Length < 6)
                return Results.BadRequest(new Respuesta<object>(
                    Exito: false,
                    Mensaje: "La contraseña debe tener al menos 6 caracteres",
                    Data: null
                ));

            var usuario = await servicio.Registrar(solicitud.Email, solicitud.Contraseña);

            if (usuario is null)
                return Results.Conflict(new Respuesta<object>(
                    Exito: false,
                    Mensaje: "El email ya está registrado",
                    Data: null
                ));

            return Results.Ok(new Respuesta<object>(
                Exito: true,
                Mensaje: "Usuario registrado correctamente",
                Data: null
            ));
        })
        .WithSummary("Registrar nuevo usuario");

        grupo.MapPost("/login", async (LoginRequest solicitud, IAuthServicio servicio) =>
{
    try
    {
        var token = await servicio.Login(solicitud.Email, solicitud.Contraseña);

        if (token is null)
            return Results.Unauthorized();

        return Results.Ok(new Respuesta<LoginRespuesta>(
            Exito: true,
            Mensaje: "Login exitoso",
            Data: new LoginRespuesta(token, solicitud.Email)
        ));
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithSummary("Iniciar sesión");
    }
}