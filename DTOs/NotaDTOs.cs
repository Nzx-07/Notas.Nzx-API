namespace NotasNzx.DTOs;

// Notas
public record CrearNotaRequest(string Titulo, string Contenido);
public record ActualizarNotaRequest(string? Titulo, string? Contenido);
public record NotaRespuesta(Guid Id, string Titulo, string Contenido, DateTime CreadoEn, Guid? CarpetaId);
public record MoverNotaRequest(Guid? CarpetaId);

// Carpetas
public record CrearCarpetaRequest(string Nombre);
public record ActualizarCarpetaRequest(string Nombre);
public record CarpetaRespuesta(Guid Id, string Nombre, DateTime CreadoEn);

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
    int LimiteRequests,
    string TemaActivo
);

// Temas
public record TemaRespuesta(
    string Id,
    string Nombre,
    string Descripcion,
    bool EsPro,
    bool EstaActivo
);
public record ActivarTemaRequest(string TemaId);

// Envoltura estándar
public record Respuesta<T>(bool Exito, string Mensaje, T? Data);