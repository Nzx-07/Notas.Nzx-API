namespace NotasNzx.DTOs;

// Notas
public record CrearNotaRequest(string Contenido);

public record NotaRespuesta(
    Guid Id,
    string Contenido,
    DateTime CreadoEn
);

// Autenticación
public record RegistrarRequest(string Email, string Contraseña);
public record LoginRequest(string Email, string Contraseña);
public record LoginRespuesta(string Token, string Email);

// Perfil de usuario
public record PerfilRespuesta(
    string Email,
    string Plan,
    string ApiKey,
    int RequestsHoy,
    int LimiteRequests
);

// Envoltura estándar
public record Respuesta<T>(bool Exito, string Mensaje, T? Data);