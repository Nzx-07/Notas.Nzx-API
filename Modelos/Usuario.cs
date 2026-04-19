namespace NotasNzx.Modelos;

public enum Plan { Free, Pro }

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string HashContraseña { get; set; } = string.Empty;
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    // Plan y API Key
    public Plan Plan { get; set; } = Plan.Free;
    public string ApiKey { get; set; } = GenerarApiKey();
    public int RequestsHoy { get; set; } = 0;
    public DateTime UltimoReset { get; set; } = DateTime.UtcNow.Date;

    public List<Nota> Notas { get; set; } = [];
    public List<Carpeta> Carpetas { get; set; } = [];

    private static string GenerarApiKey() =>
        $"nzx_{Guid.NewGuid():N}{Guid.NewGuid():N}"[..48];
}